namespace urlShorterAPI.Models;

public class ShortUrl
{
    public int Id { get; set; }
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortenUrl { get; set; } = string.Empty;
    
    // TTL implementation - replaces ExpiryDate
    public DateTime CreatedAt { get; set; }
    public int TTLMinutes { get; set; } = 60; // Default TTL: 60 minutes
}

public class UrlDto
{
    public string Url { get; set; } = string.Empty;
    public int TTLMinutes { get; set; } = 60; // Allow clients to specify TTL
}

public class UrlShortResponseDto
{
    public string Url { get; set; } = string.Empty;
}