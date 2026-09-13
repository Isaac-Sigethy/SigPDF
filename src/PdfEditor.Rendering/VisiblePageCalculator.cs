namespace PdfEditor.Rendering;

public sealed class VisiblePageRange
{
    public required IReadOnlyList<int> VisiblePageNumbers { get; init; }
    public required IReadOnlyList<int> PrefetchedPageNumbers { get; init; }

    public IReadOnlyList<int> PagesToRender
    {
        get
        {
            var pages = new List<int>(VisiblePageNumbers.Count + PrefetchedPageNumbers.Count);
            pages.AddRange(VisiblePageNumbers);
            pages.AddRange(PrefetchedPageNumbers);
            return pages;
        }
    }
}

public static class VisiblePageCalculator
{
    public static VisiblePageRange Calculate(
        IReadOnlyList<double> pageHeights,
        double scrollOffsetY,
        double viewportHeight,
        double pageGap = 0,
        int adjacentPrefetch = 1)
    {
        ArgumentNullException.ThrowIfNull(pageHeights);

        if (viewportHeight < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(viewportHeight), "Viewport height cannot be negative.");
        }

        if (pageGap < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageGap), "Page gap cannot be negative.");
        }

        if (adjacentPrefetch < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(adjacentPrefetch), "Prefetch count cannot be negative.");
        }

        if (pageHeights.Count == 0)
        {
            return new VisiblePageRange
            {
                VisiblePageNumbers = Array.Empty<int>(),
                PrefetchedPageNumbers = Array.Empty<int>()
            };
        }

        foreach (var height in pageHeights)
        {
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageHeights), "Page heights must be positive.");
            }
        }

        var viewportTop = scrollOffsetY;
        var viewportBottom = scrollOffsetY + viewportHeight;
        var visible = new List<int>();
        var y = 0.0;

        for (var i = 0; i < pageHeights.Count; i++)
        {
            var top = y;
            var bottom = y + pageHeights[i];
            if (bottom > viewportTop && top < viewportBottom)
            {
                visible.Add(i + 1);
            }

            y = bottom + pageGap;
        }

        if (visible.Count == 0)
        {
            visible.Add(scrollOffsetY <= 0 ? 1 : pageHeights.Count);
        }

        var prefetchStart = Math.Max(1, visible[0] - adjacentPrefetch);
        var prefetchEnd = Math.Min(pageHeights.Count, visible[^1] + adjacentPrefetch);
        var prefetch = new List<int>();
        for (var page = prefetchStart; page <= prefetchEnd; page++)
        {
            if (!visible.Contains(page))
            {
                prefetch.Add(page);
            }
        }

        return new VisiblePageRange
        {
            VisiblePageNumbers = visible,
            PrefetchedPageNumbers = prefetch
        };
    }
}
