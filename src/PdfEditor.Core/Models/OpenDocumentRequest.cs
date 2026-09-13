namespace PdfEditor.Core.Models;

public sealed class OpenDocumentRequest
{
    public required string FilePath { get; init; }
    public string? Password { get; init; }
}
