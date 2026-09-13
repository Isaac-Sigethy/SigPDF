using PdfEditor.Core.Geometry;

namespace PdfEditor.Core.Models;

public sealed class PdfText
{
    public required string Content { get; init; }
    public required PdfRect Bounds { get; init; }
    public string? FontName { get; init; }
    public double? FontSize { get; init; }
}
