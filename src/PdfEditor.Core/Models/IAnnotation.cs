using PdfEditor.Core.Geometry;

namespace PdfEditor.Core.Models;

public interface IAnnotation
{
    Guid Id { get; }
    int PageNumber { get; }
    PdfRect Bounds { get; }
    string? Author { get; }
    DateTimeOffset CreatedTime { get; }
    DateTimeOffset ModifiedTime { get; }
}
