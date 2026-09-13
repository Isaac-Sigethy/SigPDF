using PdfEditor.Core.Errors;
using PdfEditor.Core.Validation;

namespace PdfEditor.Tests.Validation;

public sealed class DocumentOpenValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_RequiresPath(string? path)
    {
        var error = Assert.Throws<PdfException>(() => DocumentOpenValidator.Validate(path));
        Assert.Equal(PdfErrorKind.FileNotFound, error.Kind);
    }

    [Fact]
    public void Validate_MissingFile_ThrowsFileNotFound()
    {
        var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".pdf");
        var error = Assert.Throws<PdfException>(() => DocumentOpenValidator.Validate(missing));
        Assert.Equal(PdfErrorKind.FileNotFound, error.Kind);
        Assert.DoesNotContain(missing, error.Message);
    }

    [Fact]
    public void Validate_Directory_ThrowsFileNotFound()
    {
        var error = Assert.Throws<PdfException>(() => DocumentOpenValidator.Validate(Path.GetTempPath()));
        Assert.Equal(PdfErrorKind.FileNotFound, error.Kind);
    }

    [Fact]
    public void Validate_ExistingFile_Succeeds()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".pdf");
        File.WriteAllText(path, "%PDF-1.4");
        try
        {
            DocumentOpenValidator.Validate(path);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
