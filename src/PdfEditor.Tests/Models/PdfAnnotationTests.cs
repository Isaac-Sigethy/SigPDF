using PdfEditor.Core.Geometry;
using PdfEditor.Core.Models;

namespace PdfEditor.Tests.Models;

public sealed class PdfAnnotationTests
{
    [Fact]
    public void Constructor_StoresDocumentSpaceBounds()
    {
        var bounds = new PdfRect(10, 20, 100, 15);
        var annotation = new PdfAnnotation(
            Guid.NewGuid(),
            2,
            bounds,
            AnnotationKind.Highlight,
            "Ada",
            DateTimeOffset.Parse("2024-01-01T00:00:00Z"),
            DateTimeOffset.Parse("2024-01-02T00:00:00Z"));

        IAnnotation contract = annotation;
        Assert.Equal(2, contract.PageNumber);
        Assert.Equal(bounds, contract.Bounds);
        Assert.True(bounds.Contains(new PdfPoint(12, 21)));
        Assert.False(bounds.Contains(new PdfPoint(0, 0)));
    }
}
