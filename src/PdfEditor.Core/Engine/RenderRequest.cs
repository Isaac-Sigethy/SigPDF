namespace PdfEditor.Core.Engine;

public sealed class RenderRequest
{
    public required Guid DocumentId { get; init; }
    public required int PageNumber { get; init; }
    public required double Zoom { get; init; }
    public int Rotation { get; init; }
    public string RenderMode { get; init; } = "Display";
}
