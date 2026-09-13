using PdfEditor.Core.Hosting;
using PdfEditor.Core.Models;

namespace PdfEditor.Tests.Hosting;

public sealed class ApplicationRuntimeTests
{
    [Fact]
    public void Start_CreatesAppDataAndLoadsDefaults()
    {
        var directory = Path.Combine(Path.GetTempPath(), "pdfeditor-runtime-" + Guid.NewGuid().ToString("N"));
        using var runtime = ApplicationRuntime.Start(directory);

        Assert.True(Directory.Exists(directory));
        Assert.True(Directory.Exists(Path.Combine(directory, "logs")));
        Assert.Empty(runtime.RecentDocuments);
        Assert.Equal(1.0, runtime.Settings.DefaultZoom);
        Assert.False(runtime.EngineRegistration.IsRegistered);
        Assert.True(Directory.EnumerateFiles(Path.Combine(directory, "logs"), "*.log").Any());
    }

    [Fact]
    public void SettingsStore_RoundTripsNormalizedValues()
    {
        var directory = Path.Combine(Path.GetTempPath(), "pdfeditor-settings-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        using var runtime = ApplicationRuntime.Start(directory);

        runtime.Settings.DefaultZoom = 1.5;
        runtime.Settings.Theme = "Dark";
        runtime.SettingsStore.Save(runtime.Settings);

        var reloaded = runtime.SettingsStore.Load();
        Assert.Equal(1.5, reloaded.DefaultZoom);
        Assert.Equal("Dark", reloaded.Theme);
    }

    [Fact]
    public void SettingsStore_CorruptFile_ReturnsDefaults()
    {
        var directory = Path.Combine(Path.GetTempPath(), "pdfeditor-settings-bad-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "settings.json"), "{ not-json");

        using var runtime = ApplicationRuntime.Start(directory);

        Assert.Equal(ApplicationSettings.CreateDefault().Theme, runtime.Settings.Theme);
    }

    [Fact]
    public void RecentDocuments_AddAndHandleMissingFile()
    {
        var directory = Path.Combine(Path.GetTempPath(), "pdfeditor-recent-" + Guid.NewGuid().ToString("N"));
        using var runtime = ApplicationRuntime.Start(directory);
        var missing = Path.Combine(directory, "gone.pdf");

        runtime.RememberDocument(missing);

        Assert.Single(runtime.RecentDocuments);
        Assert.False(runtime.RecentDocuments[0].Exists);
        Assert.Equal(missing, runtime.RecentDocuments[0].FilePath);
    }
}
