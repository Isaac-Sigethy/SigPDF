using PdfEditor.Core.Geometry;
using PdfEditor.Core.Validation;

namespace PdfEditor.Core.Models;

public sealed class PdfAnnotation : IAnnotation
{
    public PdfAnnotation(
        Guid id,
        int pageNumber,
        PdfRect bounds,
        AnnotationKind kind,
        string? author,
        DateTimeOffset createdTime,
        DateTimeOffset modifiedTime)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Annotation id is required.", nameof(id));
        }

        PageNumberValidator.Validate(pageNumber);

        if (bounds.Width < 0 || bounds.Height < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bounds), "Annotation bounds cannot be negative.");
        }

        Id = id;
        PageNumber = pageNumber;
        Bounds = bounds;
        Kind = kind;
        Author = author;
        CreatedTime = createdTime;
        ModifiedTime = modifiedTime;
    }

    public Guid Id { get; }
    public int PageNumber { get; }
    public PdfRect Bounds { get; }
    public AnnotationKind Kind { get; }
    public string? Author { get; }
    public DateTimeOffset CreatedTime { get; }
    public DateTimeOffset ModifiedTime { get; }
}
