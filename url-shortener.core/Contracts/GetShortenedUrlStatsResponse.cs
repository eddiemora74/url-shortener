using url_shortener.core.Entities;

namespace url_shortener.core.Contracts;

public class GetShortenedUrlStatsResponse : GetShortenedUrlResponse
{
    public int AccessCount { get; }

    private GetShortenedUrlStatsResponse(Guid id, Uri url, string shortCode, DateTime createdAt, DateTime updatedAt,
        int accessCount)
        : base(id, url, shortCode, createdAt, updatedAt)
    {
        AccessCount = accessCount;
    }

    public static GetShortenedUrlStatsResponse Create(ShortenedUrl shortenedUrl)
        => new(shortenedUrl.Id, shortenedUrl.Url, shortenedUrl.ShortCode, shortenedUrl.CreatedAt, 
            shortenedUrl.UpdatedAt, shortenedUrl.Visits.Count);
}