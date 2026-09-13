using PdfEditor.Core.Logging;

namespace PdfEditor.Tests.Logging;

public sealed class LogSanitizerTests
{
    [Fact]
    public void SanitizeValue_RedactsPasswords()
    {
        Assert.Equal("[redacted]", LogSanitizer.SanitizeValue("Password", "secret"));
        Assert.Null(LogSanitizer.SanitizeValue("Password", null));
        Assert.False(LogSanitizer.IsSensitiveKey("FilePath"));
        Assert.Equal(
            "password-supplied=[redacted]",
            LogSanitizer.RedactMessage("password-supplied=secret", "Password", "secret"));
    }

    [Fact]
    public void SanitizePath_ReplacesUserProfilePrefix()
    {
        var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var path = Path.Combine(profile, "Documents", "taxes.pdf");

        var sanitized = LogSanitizer.SanitizePath(path);

        Assert.StartsWith("~", sanitized);
        Assert.DoesNotContain(profile, sanitized);
        Assert.Contains("taxes.pdf", sanitized);
    }
}
