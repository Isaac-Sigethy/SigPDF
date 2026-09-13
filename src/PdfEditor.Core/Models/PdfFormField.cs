using PdfEditor.Core.Geometry;
using PdfEditor.Core.Validation;

namespace PdfEditor.Core.Models;

public sealed class PdfFormField
{
    public PdfFormField(string name, FormFieldKind kind, int pageNumber, PdfRect bounds, string? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        PageNumberValidator.Validate(pageNumber);

        Name = name;
        Kind = kind;
        PageNumber = pageNumber;
        Bounds = bounds;
        Value = value;
    }

    public string Name { get; }
    public FormFieldKind Kind { get; }
    public int PageNumber { get; }
    public PdfRect Bounds { get; }
    public string? Value { get; }
}
