namespace PdfEditor.Core.Models;

public sealed class PdfBookmark
{
    public PdfBookmark(string title, int? pageNumber, IReadOnlyList<PdfBookmark>? children = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        if (pageNumber is < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Bookmark page numbers are 1-based.");
        }

        Title = title;
        PageNumber = pageNumber;
        Children = children ?? Array.Empty<PdfBookmark>();
    }

    public string Title { get; }
    public int? PageNumber { get; }
    public IReadOnlyList<PdfBookmark> Children { get; }
}
