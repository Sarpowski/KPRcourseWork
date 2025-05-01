using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using urlShorterAPI.Data;
using urlShorterAPI.Models;

namespace urlShorterAPI.Controllers;

public class UrlControllers : ControllerBase
{
    private static ILogger _logger;

    public static void MapShortenerEndPoints(IEndpointRouteBuilder app)
    {
        var loggerFactory = app.ServiceProvider.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger("UrlShortenerEndpoints");
        app.MapPost("/shorturl", CreateShortUrl);
        app.MapFallback(RedirectToOriginalUrl);
    }

    private static async Task<IResult> CreateShortUrl(UrlDto url, ApplicationDbContext db, HttpContext ctx)
    {
        try
        {
            if (!Uri.TryCreate(url.Url, UriKind.Absolute, out var inputUrl))
            {
                _logger.LogWarning("Invalid Url provided: {Url}", url.Url);
                return Results.BadRequest("Invalid URL has been provided");
            }

            var existingUrl = await db.UrlShorteners.FirstOrDefaultAsync(x => x.OriginalUrl == url.Url);
            if (existingUrl != null)
            {
                var existingResult = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{existingUrl.ShortenUrl}";
                _logger.LogInformation("Existing short URL found in DB for: {Url}", url.Url);
                return Results.Ok(new UrlShortResponseDto { Url = existingResult });
            }

            var shortLink = await GenerateUniqueShortUrl(db);

            var sUrl = new ShortUrl
            {
                OriginalUrl = url.Url,
                ShortenUrl = shortLink,
                ExpiryDate = DateTime.UtcNow.AddMinutes(1)
            };

            db.UrlShorteners.Add(sUrl);
            await db.SaveChangesAsync();

            var result = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{sUrl.ShortenUrl}";
            _logger.LogInformation("Created a new short URL for: {Url}", url.Url);

            return Results.Ok(new UrlShortResponseDto { Url = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating short URL.");
            return Results.Problem("Internal server error");
        }
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

    private static async Task<IResult> RedirectToOriginalUrl(ApplicationDbContext db, HttpContext ctx)
    {
        var path = ctx.Request.Path.ToUriComponent().Trim('/');

        var urlMatch = await db.UrlShorteners.FirstOrDefaultAsync(x => x.ShortenUrl.Trim() == path.Trim());

        if (urlMatch == null || urlMatch.ExpiryDate < DateTime.UtcNow)
        {
            _logger.LogWarning("Invalid or expired short URL: {Path}", path);
            return Results.BadRequest("Invalid or expired short URL");
        }

        return Results.Redirect(urlMatch.OriginalUrl);
    }
}
