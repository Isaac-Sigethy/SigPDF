namespace PdfEditor.Core.Models;

public sealed class PdfDocument
{
    public PdfDocument(
        Guid documentId,
        string filePath,
        int pageCount,
        PdfMetadata metadata,
        bool isEncrypted,
        bool isReadOnly)
    {
        if (documentId == Guid.Empty)
        {
            throw new ArgumentException("DocumentId is required.", nameof(documentId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (pageCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageCount), "Page count cannot be negative.");
        }

        DocumentId = documentId;
        FilePath = filePath;
        PageCount = pageCount;
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        IsEncrypted = isEncrypted;
        IsReadOnly = isReadOnly;
    }

    public Guid DocumentId { get; }
    public string FilePath { get; }
    public int PageCount { get; private set; }
    public PdfMetadata Metadata { get; }
    public bool IsEncrypted { get; }
    public bool IsReadOnly { get; }
    public bool IsModified { get; private set; }

    public void MarkModified() => IsModified = true;

    public void ClearModified() => IsModified = false;

    public void UpdatePageCount(int pageCount)
    {
        if (pageCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageCount), "Page count cannot be negative.");
        }

        if (PageCount == pageCount)
        {
            return;
        }

        PageCount = pageCount;
        MarkModified();
    }
}
