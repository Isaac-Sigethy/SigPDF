using Microsoft.Extensions.Logging;

namespace PdfEditor.Core.Logging;

public static class LogEvents
{
    public static readonly EventId ApplicationStarting = new(1000, nameof(ApplicationStarting));
    public static readonly EventId ApplicationStarted = new(1001, nameof(ApplicationStarted));
    public static readonly EventId ApplicationStopping = new(1002, nameof(ApplicationStopping));
    public static readonly EventId SettingsLoadFailed = new(1100, nameof(SettingsLoadFailed));
    public static readonly EventId RecentDocumentsLoadFailed = new(1101, nameof(RecentDocumentsLoadFailed));
    public static readonly EventId DocumentOpenFailed = new(2000, nameof(DocumentOpenFailed));
    public static readonly EventId DocumentCloseFailed = new(2001, nameof(DocumentCloseFailed));
    public static readonly EventId RenderingFailed = new(3000, nameof(RenderingFailed));
    public static readonly EventId SaveFailed = new(4000, nameof(SaveFailed));
    public static readonly EventId RecoveryEvent = new(5000, nameof(RecoveryEvent));
    public static readonly EventId EngineError = new(6000, nameof(EngineError));
}
