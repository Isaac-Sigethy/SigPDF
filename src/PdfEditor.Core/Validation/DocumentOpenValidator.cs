using PdfEditor.Core.Errors;

namespace PdfEditor.Core.Validation;

public static class DocumentOpenValidator
{
    public static void Validate(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new PdfException(PdfErrorKind.FileNotFound, "A file path is required.");
        }

        if (Directory.Exists(filePath))
        {
            throw new PdfException(PdfErrorKind.FileNotFound, "The specified path is a directory, not a PDF file.");
        }

        if (!File.Exists(filePath))
        {
            throw new PdfException(
                PdfErrorKind.FileNotFound,
                $"The file '{Path.GetFileName(filePath)}' was not found.");
        }

        try
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new PdfException(
                PdfErrorKind.PermissionDenied,
                "The file could not be opened because access was denied.",
                ex);
        }
        catch (IOException ex)
        {
            throw new PdfException(
                PdfErrorKind.PermissionDenied,
                "The file could not be opened.",
                ex);
        }
    }
}
