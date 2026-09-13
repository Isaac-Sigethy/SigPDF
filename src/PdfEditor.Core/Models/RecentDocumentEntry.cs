namespace PdfEditor.Core.Models;

public sealed record RecentDocumentEntry(string FilePath, DateTimeOffset LastOpenedUtc, bool Exists);
