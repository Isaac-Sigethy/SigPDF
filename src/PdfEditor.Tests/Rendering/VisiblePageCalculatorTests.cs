using PdfEditor.Rendering;

namespace PdfEditor.Tests.Rendering;

public sealed class VisiblePageCalculatorTests
{
    [Fact]
    public void Calculate_EmptyDocument_ReturnsNoPages()
    {
        var range = VisiblePageCalculator.Calculate(Array.Empty<double>(), 0, 100);

        Assert.Empty(range.VisiblePageNumbers);
        Assert.Empty(range.PagesToRender);
    }

    [Fact]
    public void Calculate_FirstPageVisible_PrefetchesNeighbor()
    {
        var heights = new double[] { 100, 100, 100 };
        var range = VisiblePageCalculator.Calculate(heights, scrollOffsetY: 0, viewportHeight: 80, pageGap: 10);

        Assert.Equal(new[] { 1 }, range.VisiblePageNumbers);
        Assert.Equal(new[] { 2 }, range.PrefetchedPageNumbers);
    }

    [Fact]
    public void Calculate_ScrolledToSecondPage_IncludesAdjacentPages()
    {
        var heights = new double[] { 100, 100, 100 };
        var range = VisiblePageCalculator.Calculate(heights, scrollOffsetY: 120, viewportHeight: 50, pageGap: 10);

        Assert.Equal(new[] { 2 }, range.VisiblePageNumbers);
        Assert.Contains(1, range.PrefetchedPageNumbers);
        Assert.Contains(3, range.PrefetchedPageNumbers);
        Assert.DoesNotContain(4, range.PagesToRender);
    }

    [Fact]
    public void Calculate_DoesNotEnumerateEveryPageAsVisible()
    {
        var heights = Enumerable.Repeat(200.0, 1000).ToArray();
        var range = VisiblePageCalculator.Calculate(heights, scrollOffsetY: 4000, viewportHeight: 400);

        Assert.InRange(range.VisiblePageNumbers.Count, 1, 4);
        Assert.True(range.PagesToRender.Count < 10);
    }
}
