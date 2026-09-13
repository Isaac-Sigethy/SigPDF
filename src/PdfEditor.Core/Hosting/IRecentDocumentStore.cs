using PdfEditor.Core.Models;

namespace PdfEditor.Core.Hosting;

public interface IRecentDocumentStore
{
    IReadOnlyList<RecentDocumentEntry> Load();
    IReadOnlyList<RecentDocumentEntry> Add(string filePath, DateTimeOffset lastOpenedUtc);
}
