using PdfEditor.Core.Coordinates;
using PdfEditor.Core.Geometry;

namespace PdfEditor.Tests.Coordinates;

public sealed class CoordinateTransformTests
{
    private const double Width = 612;
    private const double Height = 792;

    [Fact]
    public void PdfToView_FlipsYAxisForUnrotatedPage()
    {
        var transform = new CoordinateTransform(Width, Height, rotation: 0, zoom: 1);

        Assert.Equal(new PdfPoint(0, Height), transform.PdfToView(new PdfPoint(0, 0)));
        Assert.Equal(new PdfPoint(0, 0), transform.PdfToView(new PdfPoint(0, Height)));
        Assert.Equal(new PdfPoint(Width / 2, Height / 2), transform.PdfToView(new PdfPoint(Width / 2, Height / 2)));
    }

    [Fact]
    public void Zoom_ScalesViewCoordinates()
    {
        var transform = new CoordinateTransform(Width, Height, rotation: 0, zoom: 2);
        var view = transform.PdfToView(new PdfPoint(10, Height - 20));

        Assert.Equal(20, view.X);
        Assert.Equal(40, view.Y);
        Assert.Equal(Width * 2, transform.ViewSize.Width);
        Assert.Equal(Height * 2, transform.ViewSize.Height);
    }

    [Fact]
    public void ScreenConversion_AppliesScrollOffset()
    {
        var transform = new CoordinateTransform(Width, Height, rotation: 0, zoom: 1);
        var scroll = new PdfPoint(40, 15);
        var pdf = new PdfPoint(100, 200);

        var screen = transform.PdfToScreen(pdf, scroll);
        var roundTrip = transform.ScreenToPdf(screen, scroll);

        Assert.Equal(pdf.X, roundTrip.X, 6);
        Assert.Equal(pdf.Y, roundTrip.Y, 6);
    }

    [Fact]
    public void Rotation90_SwapsViewSizeAndKeepsRoundTrip()
    {
        var transform = new CoordinateTransform(Width, Height, rotation: 90, zoom: 1);

        Assert.Equal(Height, transform.ViewSize.Width);
        Assert.Equal(Width, transform.ViewSize.Height);

        var pdf = new PdfPoint(80, 120);
        var roundTrip = transform.ViewToPdf(transform.PdfToView(pdf));
        Assert.Equal(pdf.X, roundTrip.X, 6);
        Assert.Equal(pdf.Y, roundTrip.Y, 6);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(90)]
    [InlineData(180)]
    [InlineData(270)]
    public void PdfViewRoundTrip_PreservesPoint(int rotation)
    {
        var transform = new CoordinateTransform(Width, Height, rotation, zoom: 1.5);
        var pdf = new PdfPoint(33.5, 701.25);
        var restored = transform.ViewToPdf(transform.PdfToView(pdf));

        Assert.Equal(pdf.X, restored.X, 6);
        Assert.Equal(pdf.Y, restored.Y, 6);
    }
}
