using System.Text.Json.Serialization;
using url_shortener.core.Primitives;

namespace url_shortener.core.Entities;

public class ShortenedUrlVisit : Entity
{
    public string IpAddress { get; set; } = string.Empty;
    public DateTime DateAccessed { get; set; }
    public Guid ShortenedUrlId { get; set; }
    
    [JsonIgnore] 
    public ShortenedUrl ShortenedUrl { get; set; } = null!;
}