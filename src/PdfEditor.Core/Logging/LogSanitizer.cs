namespace PdfEditor.Core.Logging;

public static class LogSanitizer
{
    private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "Password",
        "Pwd",
        "Secret",
        "Token",
        "Authorization"
    };

    public static bool IsSensitiveKey(string key) => SensitiveKeys.Contains(key);

    public static string SanitizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return path;
        }

        var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (!string.IsNullOrEmpty(profile) && path.StartsWith(profile, StringComparison.OrdinalIgnoreCase))
        {
            return "~" + path[profile.Length..];
        }

        return path;
    }

    public static object? SanitizeValue(string key, object? value)
    {
        if (IsSensitiveKey(key))
        {
            return value is null ? null : "[redacted]";
        }

        if (value is string text && LooksLikePath(key))
        {
            return SanitizePath(text);
        }

        return value;
    }

    public static string RedactMessage(string message, string key, object? value)
    {
        if (!IsSensitiveKey(key) || value is not string secret || secret.Length == 0)
        {
            return message;
        }

        return message.Replace(secret, "[redacted]", StringComparison.Ordinal);
    }

    private static bool LooksLikePath(string key) =>
        key.Contains("Path", StringComparison.OrdinalIgnoreCase) ||
        key.Contains("File", StringComparison.OrdinalIgnoreCase);
}
