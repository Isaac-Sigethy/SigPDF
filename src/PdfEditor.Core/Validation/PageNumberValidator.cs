using PdfEditor.Core.Errors;

namespace PdfEditor.Core.Validation;

public static class PageNumberValidator
{
    public static void Validate(int pageNumber)
    {
        if (pageNumber < 1)
        {
            throw new PdfException(
                PdfErrorKind.InvalidPage,
                $"Page number {pageNumber} is invalid. Pages are numbered starting at 1.");
        }
    }

    public static void ValidateInDocument(int pageNumber, int pageCount)
    {
        if (pageCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageCount), "Page count cannot be negative.");
        }

        Validate(pageNumber);

        if (pageCount == 0 || pageNumber > pageCount)
        {
            throw new PdfException(
                PdfErrorKind.InvalidPage,
                $"Page {pageNumber} is outside the document ({pageCount} pages).");
        }
    }
}
