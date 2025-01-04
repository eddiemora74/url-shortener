namespace url_shortener.core.Utilities;

public static class UriUtilities
{
    public static bool IsValidAbsoluteUrl(this string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}