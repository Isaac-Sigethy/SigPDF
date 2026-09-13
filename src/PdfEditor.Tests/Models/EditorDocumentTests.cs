using PdfEditor.Core.Errors;
using PdfEditor.Core.Geometry;
using PdfEditor.Core.Models;

namespace PdfEditor.Tests.Models;

public sealed class EditorDocumentTests
{
    [Fact]
    public void Constructor_SelectsFirstPageAndDefaultTool()
    {
        var editor = new EditorDocument(CreateDocument(8), defaultZoom: 1.25);

        Assert.Equal(1, editor.SelectedPage);
        Assert.Equal(1.25, editor.Zoom);
        Assert.Equal(EditorTool.Select, editor.CurrentTool);
        Assert.False(editor.UnsavedChanges);
    }

    [Fact]
    public void SelectPage_UpdatesSelectionWithinDocument()
    {
        var editor = new EditorDocument(CreateDocument(5));

        editor.SelectPage(5);
        Assert.Equal(5, editor.SelectedPage);
    }

    [Fact]
    public void SelectPage_RejectsOutOfRangePage()
    {
        var editor = new EditorDocument(CreateDocument(2));
        var error = Assert.Throws<PdfException>(() => editor.SelectPage(3));
        Assert.Equal(PdfErrorKind.InvalidPage, error.Kind);
    }

    [Fact]
    public void SetZoom_RejectsValuesOutsideRange()
    {
        var editor = new EditorDocument(CreateDocument(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => editor.SetZoom(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => editor.SetZoom(32));
    }

    [Fact]
    public void UnsavedChanges_FollowsDocumentModifiedFlag()
    {
        var document = CreateDocument(1);
        var editor = new EditorDocument(document);

        document.MarkModified();

        Assert.True(editor.UnsavedChanges);
        editor.SetScrollPosition(new PdfPoint(10, 20));
        editor.SetTool(EditorTool.Hand);
        editor.SelectObject("annotation");
        Assert.Equal(EditorTool.Hand, editor.CurrentTool);
        Assert.Equal("annotation", editor.SelectedObject);
    }

    private static PdfDocument CreateDocument(int pageCount) =>
        new(Guid.NewGuid(), "document.pdf", pageCount, PdfMetadata.Empty, false, false);
}
