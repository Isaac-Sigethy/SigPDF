using Microsoft.Extensions.Logging;
using PdfEditor.Core.Logging;
using PdfEditor.Core.Models;

namespace PdfEditor.Core.Hosting;

public sealed class ApplicationRuntime : IDisposable
{
    private readonly ILogger<ApplicationRuntime> _logger;

    private ApplicationRuntime(
        ILoggerFactory loggerFactory,
        ILogger<ApplicationRuntime> logger,
        ApplicationSettings settings,
        IReadOnlyList<RecentDocumentEntry> recentDocuments,
        ISettingsStore settingsStore,
        IRecentDocumentStore recentDocumentStore,
        string applicationDataDirectory)
    {
        LoggerFactory = loggerFactory;
        _logger = logger;
        Settings = settings;
        RecentDocuments = recentDocuments;
        SettingsStore = settingsStore;
        RecentDocumentStore = recentDocumentStore;
        ApplicationDataDirectory = applicationDataDirectory;
    }

    public ILoggerFactory LoggerFactory { get; }
    public ApplicationSettings Settings { get; }
    public IReadOnlyList<RecentDocumentEntry> RecentDocuments { get; private set; }
    public ISettingsStore SettingsStore { get; }
    public IRecentDocumentStore RecentDocumentStore { get; }
    public string ApplicationDataDirectory { get; }
    public IPdfEngineRegistration EngineRegistration { get; } = new UnregisteredPdfEngine();

    public static ApplicationRuntime Start(
        string applicationDataDirectory,
        Action<ILoggingBuilder>? configureLogging = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationDataDirectory);
        Directory.CreateDirectory(applicationDataDirectory);

        var logDirectory = Path.Combine(applicationDataDirectory, "logs");
        var fileLoggerProvider = new JsonLineLoggerProvider(logDirectory);
        var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddProvider(fileLoggerProvider);
            configureLogging?.Invoke(builder);
        });

        var logger = loggerFactory.CreateLogger<ApplicationRuntime>();
        logger.LogInformation(LogEvents.ApplicationStarting, "PDF Editor starting.");

        var settingsStore = new FileSettingsStore(
            Path.Combine(applicationDataDirectory, "settings.json"),
            loggerFactory.CreateLogger<FileSettingsStore>());
        var settings = settingsStore.Load();

        var recentStore = new FileRecentDocumentStore(
            Path.Combine(applicationDataDirectory, "recent-documents.json"),
            loggerFactory.CreateLogger<FileRecentDocumentStore>());
        var recent = recentStore.Load();

        logger.LogInformation(
            LogEvents.ApplicationStarted,
            "Startup complete. Recent document count: {RecentCount}. Engine registered: {EngineRegistered}.",
            recent.Count,
            false);

        return new ApplicationRuntime(
            loggerFactory,
            logger,
            settings,
            recent,
            settingsStore,
            recentStore,
            applicationDataDirectory);
    }

    public void RememberDocument(string filePath)
    {
        RecentDocuments = RecentDocumentStore.Add(filePath, DateTimeOffset.UtcNow);
    }

    public void Dispose()
    {
        _logger.LogInformation(LogEvents.ApplicationStopping, "PDF Editor stopping.");
        LoggerFactory.Dispose();
    }
}

public interface IPdfEngineRegistration
{
    bool IsRegistered { get; }
}

file sealed class UnregisteredPdfEngine : IPdfEngineRegistration
{
    public bool IsRegistered => false;
}
