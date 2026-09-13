using PdfEditor.Core.Errors;
using PdfEditor.Core.Models;

namespace PdfEditor.Tests.Models;

public sealed class PdfPageTests
{
    [Fact]
    public void Constructor_AcceptsValidPage()
    {
        var page = new PdfPage(1, 612, 792, 90);

        Assert.Equal(1, page.PageNumber);
        Assert.Equal(612, page.Width);
        Assert.Equal(792, page.Height);
        Assert.Equal(90, page.Rotation);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Constructor_RejectsInvalidPageNumber(int pageNumber)
    {
        var error = Assert.Throws<PdfException>(() => new PdfPage(pageNumber, 100, 100, 0));
        Assert.Equal(PdfErrorKind.InvalidPage, error.Kind);
    }

    [Theory]
    [InlineData(45)]
    [InlineData(360)]
    public void Constructor_RejectsInvalidRotation(int rotation)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PdfPage(1, 100, 100, rotation));
    }
}
