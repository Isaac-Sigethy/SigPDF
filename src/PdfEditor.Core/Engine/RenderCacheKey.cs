namespace PdfEditor.Core.Engine;

public readonly record struct RenderCacheKey(
    Guid DocumentId,
    int PageNumber,
    double Zoom,
    int Rotation,
    string RenderMode);
