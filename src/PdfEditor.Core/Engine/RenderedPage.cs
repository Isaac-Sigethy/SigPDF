namespace PdfEditor.Core.Engine;

public sealed class RenderedPage : IDisposable
{
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required byte[] BgraPixels { get; init; }

    public void Dispose()
    {
        // Pixel buffers are managed arrays in M1. Native buffers will dispose here later.
    }
}
