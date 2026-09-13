namespace PdfEditor.Core.Errors;

public sealed class PdfException : Exception
{
    public PdfErrorKind Kind { get; }

    public PdfException(PdfErrorKind kind, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Kind = kind;
    }
}
