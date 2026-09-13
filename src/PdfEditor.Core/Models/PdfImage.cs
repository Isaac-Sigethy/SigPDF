using PdfEditor.Core.Geometry;

namespace PdfEditor.Core.Models;

public sealed class PdfImage
{
    public required Guid Id { get; init; }
    public required PdfRect Bounds { get; init; }
    public double Rotation { get; init; }
}
