namespace PdfEditor.Core.Geometry;

public readonly record struct PdfRect(double X, double Y, double Width, double Height)
{
    public double Left => X;
    public double Bottom => Y;
    public double Right => X + Width;
    public double Top => Y + Height;

    public bool Contains(PdfPoint point) =>
        point.X >= Left && point.X <= Right && point.Y >= Bottom && point.Y <= Top;
}
