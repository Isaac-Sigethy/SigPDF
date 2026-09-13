using System.Text.Json;
using Microsoft.Extensions.Logging;
using PdfEditor.Core.Logging;
using PdfEditor.Core.Models;

namespace PdfEditor.Core.Hosting;

public sealed class FileSettingsStore : ISettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly string _filePath;
    private readonly ILogger<FileSettingsStore> _logger;

    public FileSettingsStore(string filePath, ILogger<FileSettingsStore> logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ApplicationSettings Load()
    {
        if (!File.Exists(_filePath))
        {
            return ApplicationSettings.CreateDefault();
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            var settings = JsonSerializer.Deserialize<ApplicationSettings>(json, JsonOptions)
                           ?? ApplicationSettings.CreateDefault();
            settings.Normalize();
            return settings;
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            _logger.LogWarning(
                LogEvents.SettingsLoadFailed,
                ex,
                "Failed to load settings from {SettingsPath}. Using defaults.",
                LogSanitizer.SanitizePath(_filePath));
            return ApplicationSettings.CreateDefault();
        }
    }

    public void Save(ApplicationSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        settings.Normalize();

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(settings, JsonOptions);
        var temporaryPath = _filePath + ".tmp";
        File.WriteAllText(temporaryPath, json);
        File.Move(temporaryPath, _filePath, overwrite: true);
    }
}
