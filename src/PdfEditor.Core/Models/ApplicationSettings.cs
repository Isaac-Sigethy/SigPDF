namespace PdfEditor.Core.Models;

public sealed class ApplicationSettings
{
    public const long DefaultRenderCacheMemoryLimitBytes = 256L * 1024 * 1024;

    public double DefaultZoom { get; set; } = 1.0;
    public string Theme { get; set; } = "System";
    public int ThumbnailSize { get; set; } = 160;
    public int RecoveryIntervalSeconds { get; set; } = 120;
    public long RenderCacheMemoryLimitBytes { get; set; } = DefaultRenderCacheMemoryLimitBytes;

    public static ApplicationSettings CreateDefault() => new();

    public void Normalize()
    {
        if (double.IsNaN(DefaultZoom) || DefaultZoom < EditorDocument.MinimumZoom || DefaultZoom > EditorDocument.MaximumZoom)
        {
            DefaultZoom = 1.0;
        }

        if (string.IsNullOrWhiteSpace(Theme))
        {
            Theme = "System";
        }

        if (ThumbnailSize < 32 || ThumbnailSize > 512)
        {
            ThumbnailSize = 160;
        }

        if (RecoveryIntervalSeconds < 15)
        {
            RecoveryIntervalSeconds = 15;
        }

        if (RenderCacheMemoryLimitBytes < 16L * 1024 * 1024)
        {
            RenderCacheMemoryLimitBytes = DefaultRenderCacheMemoryLimitBytes;
        }
    }
}
