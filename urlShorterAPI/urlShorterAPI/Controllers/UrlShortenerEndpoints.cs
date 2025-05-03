using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using urlShorterAPI.Data;
using urlShorterAPI.Models;

namespace urlShorterAPI.Endpoints;

public static class UrlShortenerEndpoints
{
    public static void MapShortenerEndPoints(this IEndpointRouteBuilder app)
    {
        var loggerFactory = app.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("UrlShortenerEndpoints");
        
        app.MapPost("/shorturl", async (UrlDto url, ApplicationDbContext db, HttpContext ctx) =>
        {
            try
            {
                if (!Uri.TryCreate(url.Url, UriKind.Absolute, out var inputUrl))
                {
                    logger.LogWarning("Invalid Url provided: {Url}", url.Url);
                    return Results.BadRequest("Invalid URL has been provided");
                }

                // Check for existing non-expired URLs
                var existingUrl = await db.UrlShorteners
                    .Where(x => x.OriginalUrl == url.Url)
                    .Where(x => x.CreatedAt.AddMinutes(x.TTLMinutes) > DateTime.UtcNow)
                    .FirstOrDefaultAsync();
                    
                if (existingUrl != null)
                {
                    var existingResult = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{existingUrl.ShortenUrl}";
                    logger.LogInformation("Existing short URL found in DB for: {Url}", url.Url);
                    return Results.Ok(new UrlShortResponseDto { Url = existingResult });
                }

                var shortLink = await GenerateUniqueShortUrl(db);

                // Default TTL of 10 minutes  if not specified
                int ttlMinutes = url.TTLMinutes > 0 ? url.TTLMinutes : 10;

                var sUrl = new ShortUrl
                {
                    OriginalUrl = url.Url,
                    ShortenUrl = shortLink,
                    CreatedAt = DateTime.UtcNow,
                    TTLMinutes = ttlMinutes
                };

                db.UrlShorteners.Add(sUrl);
                await db.SaveChangesAsync();

                var result = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{sUrl.ShortenUrl}";
                logger.LogInformation("Created a new short URL for: {Url}", url.Url);

                return Results.Ok(new UrlShortResponseDto { Url = result });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating short URL.");
                return Results.Problem("Internal server error");
            }
        });

        app.MapFallback(async (ApplicationDbContext db, HttpContext ctx) =>
        {
            var path = ctx.Request.Path.ToUriComponent().Trim('/');

            var urlMatch = await db.UrlShorteners.FirstOrDefaultAsync(x => x.ShortenUrl.Trim() == path.Trim());

            if (urlMatch == null)
            {
                logger.LogWarning("Invalid short URL: {Path}", path);
                return Results.BadRequest("Invalid short URL");
            }
            
            // Check if the URL has expired based on TTL
            if (urlMatch.CreatedAt.AddMinutes(urlMatch.TTLMinutes) < DateTime.UtcNow)
            {
                logger.LogWarning("Expired short URL: {Path}", path);
                return Results.BadRequest("Expired short URL");
            }

            return Results.Redirect(urlMatch.OriginalUrl);
        });
    }

    private static async Task<string> GenerateUniqueShortUrl(ApplicationDbContext db)
    {
        while (true)
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")));
            var shortLink = Convert.ToBase64String(hashBytes).Substring(0, 8);
            var urlExists = await db.UrlShorteners.AnyAsync(u => u.ShortenUrl == shortLink);
            if (!urlExists)
            {
                return shortLink;
            }
        }
    }
}