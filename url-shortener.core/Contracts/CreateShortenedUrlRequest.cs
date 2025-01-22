namespace url_shortener.core.Contracts;

public class CreateShortenedUrlRequest
{
    public string Url { get; set; } = null!;
    
    public static CreateShortenedUrlRequest Create(string url) => new() { Url = url };
}