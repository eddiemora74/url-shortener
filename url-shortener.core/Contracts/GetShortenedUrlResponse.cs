using url_shortener.core.Entities;

namespace url_shortener.core.Contracts;

public class GetShortenedUrlResponse
{
    public Guid Id { get; set; }
    public Uri Url { get; set; }
    public string ShortCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public GetShortenedUrlResponse()
    {
    }

    internal GetShortenedUrlResponse(Guid id, Uri url, string shortCode, DateTime createdAt, DateTime updatedAt)
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