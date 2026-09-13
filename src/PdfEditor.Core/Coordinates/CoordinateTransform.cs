using PdfEditor.Core.Geometry;

namespace PdfEditor.Core.Coordinates;

/// <summary>
/// Converts between PDF user space (origin bottom-left), rotated view space
/// (origin top-left), and screen space (view minus scroll).
/// </summary>
public sealed class CoordinateTransform
{
    public CoordinateTransform(double pageWidth, double pageHeight, int rotation, double zoom)
    {
        if (pageWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageWidth), "Page width must be positive.");
        }

        if (pageHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageHeight), "Page height must be positive.");
        }

        if (rotation is not (0 or 90 or 180 or 270))
        {
            throw new ArgumentOutOfRangeException(nameof(rotation), "Rotation must be 0, 90, 180, or 270 degrees.");
        }

        if (double.IsNaN(zoom) || double.IsInfinity(zoom) || zoom <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(zoom), "Zoom must be a positive number.");
        }

        PageWidth = pageWidth;
        PageHeight = pageHeight;
        Rotation = rotation;
        Zoom = zoom;
    }

    public double PageWidth { get; }
    public double PageHeight { get; }
    public int Rotation { get; }
    public double Zoom { get; }

    public PdfSize ViewSize
    {
        get
        {
            var swapped = Rotation is 90 or 270;
            var width = (swapped ? PageHeight : PageWidth) * Zoom;
            var height = (swapped ? PageWidth : PageHeight) * Zoom;
            return new PdfSize(width, height);
        }
    }

    public PdfPoint PdfToView(PdfPoint pdf)
    {
        var unrotatedX = pdf.X;
        var unrotatedY = PageHeight - pdf.Y;
        var (viewX, viewY) = RotateClockwise(unrotatedX, unrotatedY);
        return new PdfPoint(viewX * Zoom, viewY * Zoom);
    }

    public PdfPoint ViewToPdf(PdfPoint view)
    {
        var unscaledX = view.X / Zoom;
        var unscaledY = view.Y / Zoom;
        var (unrotatedX, unrotatedY) = UnrotateClockwise(unscaledX, unscaledY);
        return new PdfPoint(unrotatedX, PageHeight - unrotatedY);
    }

    public PdfPoint ViewToScreen(PdfPoint view, PdfPoint scroll) =>
        new(view.X - scroll.X, view.Y - scroll.Y);

    public PdfPoint ScreenToView(PdfPoint screen, PdfPoint scroll) =>
        new(screen.X + scroll.X, screen.Y + scroll.Y);

    public PdfPoint PdfToScreen(PdfPoint pdf, PdfPoint scroll) =>
        ViewToScreen(PdfToView(pdf), scroll);

    public PdfPoint ScreenToPdf(PdfPoint screen, PdfPoint scroll) =>
        ViewToPdf(ScreenToView(screen, scroll));

    private (double X, double Y) RotateClockwise(double x, double y) =>
        Rotation switch
        {
            90 => (PageHeight - y, x),
            180 => (PageWidth - x, PageHeight - y),
            270 => (y, PageWidth - x),
            _ => (x, y)
        };

    private (double X, double Y) UnrotateClockwise(double x, double y) =>
        Rotation switch
        {
            90 => (y, PageHeight - x),
            180 => (PageWidth - x, PageHeight - y),
            270 => (PageWidth - y, x),
            _ => (x, y)
        };
}
