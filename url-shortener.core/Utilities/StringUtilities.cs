namespace url_shortener.core.Utilities;

public static class StringUtilities
{
    public static string GenerateUniqueCode(int length = 7)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var bytes = Guid.NewGuid().ToByteArray();
        return new string(bytes.Select(b => chars[b % chars.Length]).Take(length).ToArray());
    }
}