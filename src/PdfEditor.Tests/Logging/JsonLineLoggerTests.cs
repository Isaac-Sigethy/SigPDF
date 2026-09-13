using Microsoft.Extensions.Logging;
using PdfEditor.Core.Logging;

namespace PdfEditor.Tests.Logging;

public sealed class JsonLineLoggerTests
{
    [Fact]
    public void Logger_WritesStructuredLineAndRedactsPassword()
    {
        var directory = Path.Combine(Path.GetTempPath(), "pdfeditor-logs-" + Guid.NewGuid().ToString("N"));
        using (var provider = new JsonLineLoggerProvider(directory))
        {
            var logger = provider.CreateLogger("Tests");
            logger.LogInformation("Opened {FilePath} password-supplied={Password}", @"C:\docs\a.pdf", "hunter2");
        }

        var file = Directory.GetFiles(directory, "*.log").Single();
        var line = File.ReadAllText(file);

        Assert.Contains("\"level\":\"Information\"", line);
        Assert.Contains("[redacted]", line);
        Assert.DoesNotContain("hunter2", line);
        Assert.DoesNotContain("PDF content", line);
    }
}
