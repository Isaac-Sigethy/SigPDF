using Microsoft.Extensions.Logging.Abstractions;
using PdfEditor.Core.Hosting;

namespace PdfEditor.Tests.Hosting;

public sealed class FileRecentDocumentStoreTests
{
    [Fact]
    public void Add_DeduplicatesAndCapsEntries()
    {
        var path = Path.Combine(Path.GetTempPath(), "recent-" + Guid.NewGuid().ToString("N") + ".json");
        var store = new FileRecentDocumentStore(path, NullLogger<FileRecentDocumentStore>.Instance);

        for (var i = 0; i < FileRecentDocumentStore.MaximumEntries + 5; i++)
        {
            store.Add($"doc-{i}.pdf", DateTimeOffset.UtcNow.AddMinutes(i));
        }

        store.Add("doc-1.pdf", DateTimeOffset.UtcNow.AddHours(1));
        var loaded = store.Load();

        Assert.Equal(FileRecentDocumentStore.MaximumEntries, loaded.Count);
        Assert.Equal("doc-1.pdf", loaded[0].FilePath);
        Assert.Equal(1, loaded.Count(entry => entry.FilePath == "doc-1.pdf"));
    }
}
