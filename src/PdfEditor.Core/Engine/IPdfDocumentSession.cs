using PdfEditor.Core.Models;

namespace PdfEditor.Core.Engine;

public interface IPdfDocumentSession : IAsyncDisposable, IDisposable
{
    PdfDocument Document { get; }

    int GetPageCount();

    PdfPage GetPage(int pageNumber);

    PdfMetadata ExtractMetadata();
}
