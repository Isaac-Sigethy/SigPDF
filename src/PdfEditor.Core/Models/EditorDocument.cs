using PdfEditor.Core.Geometry;
using PdfEditor.Core.Validation;

namespace PdfEditor.Core.Models;

public sealed class EditorDocument
{
    public const double MinimumZoom = 0.1;
    public const double MaximumZoom = 16.0;

    public EditorDocument(PdfDocument document, double defaultZoom = 1.0)
    {
        Document = document ?? throw new ArgumentNullException(nameof(document));
        SelectedPage = document.PageCount > 0 ? 1 : 0;
        SetZoom(defaultZoom);
        CurrentTool = EditorTool.Select;
    }

    public PdfDocument Document { get; }
    public int SelectedPage { get; private set; }
    public object? SelectedObject { get; private set; }
    public double Zoom { get; private set; }
    public PdfPoint ScrollPosition { get; private set; }
    public EditorTool CurrentTool { get; private set; }
    public bool UnsavedChanges => Document.IsModified;

    public void SelectPage(int pageNumber)
    {
        if (Document.PageCount == 0)
        {
            if (pageNumber != 0)
            {
                throw new Errors.PdfException(
                    Errors.PdfErrorKind.InvalidPage,
                    "The document has no pages.");
            }

            SelectedPage = 0;
            return;
        }

        PageNumberValidator.ValidateInDocument(pageNumber, Document.PageCount);
        SelectedPage = pageNumber;
    }

    public void SelectObject(object? value) => SelectedObject = value;

    public void SetZoom(double zoom)
    {
        if (double.IsNaN(zoom) || double.IsInfinity(zoom) || zoom < MinimumZoom || zoom > MaximumZoom)
        {
            throw new ArgumentOutOfRangeException(
                nameof(zoom),
                $"Zoom must be between {MinimumZoom} and {MaximumZoom}.");
        }

        Zoom = zoom;
    }

    public void SetScrollPosition(PdfPoint position) => ScrollPosition = position;

    public void SetTool(EditorTool tool) => CurrentTool = tool;
}
