using PdfEditor.Core.Errors;
using PdfEditor.Core.Validation;

namespace PdfEditor.Tests.Validation;

public sealed class PageNumberValidatorTests
{
    [Fact]
    public void ValidateInDocument_AcceptsFirstAndLastPage()
    {
        PageNumberValidator.ValidateInDocument(1, 10);
        PageNumberValidator.ValidateInDocument(10, 10);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(6, 5)]
    [InlineData(1, 0)]
    public void ValidateInDocument_RejectsOutOfRange(int page, int count)
    {
        var error = Assert.Throws<PdfException>(() => PageNumberValidator.ValidateInDocument(page, count));
        Assert.Equal(PdfErrorKind.InvalidPage, error.Kind);
    }
}
