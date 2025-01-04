namespace url_shortener.core.Contracts;

public class CreateShortenedUrlRequest
{
    public string Url { get; set; } = null!;
}