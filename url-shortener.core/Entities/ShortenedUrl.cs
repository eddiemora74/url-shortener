using System.Runtime.Serialization;
using url_shortener.core.Primitives;

namespace url_shortener.core.Entities;

public class ShortenedUrl : Entity
{
    public Uri Url { get; set; } = null!;
    public string ShortCode { get; set; } = string.Empty!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [IgnoreDataMember] 
    public IList<ShortenedUrlVisit> Visits { get; set; } = [];
}