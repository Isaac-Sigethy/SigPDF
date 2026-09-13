using PdfEditor.Core.Models;

namespace PdfEditor.Tests.Models;

public sealed class PdfDocumentTests
{
    [Fact]
    public void Constructor_StoresLightweightDocumentState()
    {
        var metadata = new PdfMetadata { Title = "Sample", Author = "Ada" };
        var document = new PdfDocument(Guid.NewGuid(), @"C:\docs\sample.pdf", 12, metadata, isEncrypted: false, isReadOnly: true);

        Assert.Equal(12, document.PageCount);
        Assert.Equal("Sample", document.Metadata.Title);
        Assert.True(document.IsReadOnly);
        Assert.False(document.IsModified);
    }

    [Fact]
    public void Constructor_RejectsEmptyIdAndNegativePageCount()
    {
        Assert.Throws<ArgumentException>(() =>
            new PdfDocument(Guid.Empty, "a.pdf", 1, PdfMetadata.Empty, false, false));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PdfDocument(Guid.NewGuid(), "a.pdf", -1, PdfMetadata.Empty, false, false));
    }

    [Fact]
    public void UpdatePageCount_MarksDocumentModified()
    {
        var document = new PdfDocument(Guid.NewGuid(), "a.pdf", 3, PdfMetadata.Empty, false, false);

        document.UpdatePageCount(4);

        Assert.Equal(4, document.PageCount);
        Assert.True(document.IsModified);

        document.ClearModified();
        Assert.False(document.IsModified);
    }
}
