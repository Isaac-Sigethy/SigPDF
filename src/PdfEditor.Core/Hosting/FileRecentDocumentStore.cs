using System.Text.Json;
using Microsoft.Extensions.Logging;
using PdfEditor.Core.Logging;
using PdfEditor.Core.Models;

namespace PdfEditor.Core.Hosting;

public sealed class FileRecentDocumentStore : IRecentDocumentStore
{
    public const int MaximumEntries = 20;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly string _filePath;
    private readonly ILogger<FileRecentDocumentStore> _logger;

    public FileRecentDocumentStore(string filePath, ILogger<FileRecentDocumentStore> logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IReadOnlyList<RecentDocumentEntry> Load()
    {
        if (!File.Exists(_filePath))
        {
            return Array.Empty<RecentDocumentEntry>();
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            var stored = JsonSerializer.Deserialize<List<StoredRecentDocument>>(json, JsonOptions)
                         ?? [];
            return ToEntries(stored);
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            _logger.LogWarning(
                LogEvents.RecentDocumentsLoadFailed,
                ex,
                "Failed to load recent documents from {RecentPath}.",
                LogSanitizer.SanitizePath(_filePath));
            return Array.Empty<RecentDocumentEntry>();
        }
    }

    public IReadOnlyList<RecentDocumentEntry> Add(string filePath, DateTimeOffset lastOpenedUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var stored = Load()
            .Select(entry => new StoredRecentDocument
            {
                FilePath = entry.FilePath,
                LastOpenedUtc = entry.LastOpenedUtc
            })
            .Where(entry => !string.Equals(entry.FilePath, filePath, StringComparison.OrdinalIgnoreCase))
            .ToList();

        stored.Insert(0, new StoredRecentDocument
        {
            FilePath = filePath,
            LastOpenedUtc = lastOpenedUtc
        });

        if (stored.Count > MaximumEntries)
        {
            stored.RemoveRange(MaximumEntries, stored.Count - MaximumEntries);
        }

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(_filePath, JsonSerializer.Serialize(stored, JsonOptions));
        return ToEntries(stored);
    }

    private static IReadOnlyList<RecentDocumentEntry> ToEntries(IEnumerable<StoredRecentDocument> stored) =>
        stored
            .Where(entry => !string.IsNullOrWhiteSpace(entry.FilePath))
            .Select(entry => new RecentDocumentEntry(
                entry.FilePath!,
                entry.LastOpenedUtc,
                File.Exists(entry.FilePath)))
            .ToArray();

    private sealed class StoredRecentDocument
    {
        public string? FilePath { get; set; }
        public DateTimeOffset LastOpenedUtc { get; set; }
    }
}
