using PdfEditor.Core.Validation;

namespace PdfEditor.Core.Models;

public sealed class PdfPage
{
    public PdfPage(int pageNumber, double width, double height, int rotation)
    {
        PageNumberValidator.Validate(pageNumber);

        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Page width must be positive.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "Page height must be positive.");
        }

        if (rotation is not (0 or 90 or 180 or 270))
        {
            throw new ArgumentOutOfRangeException(nameof(rotation), "Rotation must be 0, 90, 180, or 270 degrees.");
        }

        PageNumber = pageNumber;
        Width = width;
        Height = height;
        Rotation = rotation;
    }

    public int PageNumber { get; }
    public double Width { get; }
    public double Height { get; }
    public int Rotation { get; }
}
