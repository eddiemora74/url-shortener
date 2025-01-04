using url_shortener.core.Entities;

namespace url_shortener.core.Contracts;

public class GetShortenedUrlResponse
{
    public Guid Id { get; }
    public Uri Url { get; }
    public string ShortCode { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    private GetShortenedUrlResponse(Guid id, Uri url, string shortCode, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        Url = url;
        ShortCode = shortCode;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    
    public static GetShortenedUrlResponse Create(Guid id, Uri url, string shortCode, DateTime createdAt,
        DateTime updatedAt) =>
        new(id, url, shortCode, createdAt, updatedAt);
    
    public static GetShortenedUrlResponse Create(ShortenedUrl shortenedUrl) 
        => new(shortenedUrl.Id, shortenedUrl.Url, shortenedUrl.ShortCode, shortenedUrl.CreatedAt, shortenedUrl.UpdatedAt);
}