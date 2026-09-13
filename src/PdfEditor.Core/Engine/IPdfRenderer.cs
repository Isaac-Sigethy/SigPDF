namespace PdfEditor.Core.Engine;

public interface IPdfRenderer
{
    Task<RenderedPage> RenderPageAsync(
        IPdfDocumentSession session,
        RenderRequest request,
        CancellationToken cancellationToken = default);
}
