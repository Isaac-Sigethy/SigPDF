namespace PdfEditor.Core.Models;

public sealed class PdfMetadata
{
    public static PdfMetadata Empty { get; } = new();

    public string? Title { get; init; }
    public string? Author { get; init; }
    public string? Subject { get; init; }
    public string? Keywords { get; init; }
    public string? Creator { get; init; }
    public string? Producer { get; init; }
}
