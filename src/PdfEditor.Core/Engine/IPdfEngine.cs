using PdfEditor.Core.Models;

namespace PdfEditor.Core.Engine;

public interface IPdfEngine : IDisposable
{
    Task<IPdfDocumentSession> OpenAsync(OpenDocumentRequest request, CancellationToken cancellationToken = default);
}
