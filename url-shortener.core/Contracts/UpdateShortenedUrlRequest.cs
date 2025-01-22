namespace url_shortener.core.Contracts;

public class UpdateShortenedUrlRequest
{
    public string Url { get; set; }
    
    public UpdateShortenedUrlRequest() {}
    
    private UpdateShortenedUrlRequest(string url) => Url = url;

    public static UpdateShortenedUrlRequest Create(string url) => new(url);
}