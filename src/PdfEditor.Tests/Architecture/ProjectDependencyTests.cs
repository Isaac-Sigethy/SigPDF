using System.Xml.Linq;
using PdfEditor.Engine;
using PdfEditor.Tests.Support;

namespace PdfEditor.Tests.Architecture;

public sealed class ProjectDependencyTests
{
    [Fact]
    public void EngineProject_DeclaresPdfiumAdapter()
    {
        Assert.Equal("PDFium", EngineProject.PlannedAdapter);
    }

    [Fact]
    public void Core_DoesNotReferenceWinUiOrEngine()
    {
        var csproj = ReadProject("PdfEditor.Core");
        Assert.DoesNotContain("Microsoft.WindowsAppSDK", csproj, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("PdfEditor.Engine", csproj, StringComparison.Ordinal);
        Assert.DoesNotContain("PdfEditor.App", csproj, StringComparison.Ordinal);
        Assert.DoesNotContain("net8.0-windows", csproj, StringComparison.Ordinal);
    }

    [Fact]
    public void Rendering_UsesCoreAbstractionsOnly()
    {
        var references = ProjectReferences("PdfEditor.Rendering");
        Assert.Contains(references, path => path.Contains("PdfEditor.Core", StringComparison.Ordinal));
        Assert.DoesNotContain(references, path => path.Contains("PdfEditor.Engine", StringComparison.Ordinal));
        Assert.DoesNotContain(references, path => path.Contains("PdfEditor.App", StringComparison.Ordinal));
    }

    [Fact]
    public void Engine_ReferencesCoreOnly()
    {
        var references = ProjectReferences("PdfEditor.Engine");
        Assert.Single(references);
        Assert.Contains("PdfEditor.Core", references[0], StringComparison.Ordinal);
    }

    [Fact]
    public void App_IsCompositionRoot()
    {
        var references = ProjectReferences("PdfEditor.App");
        Assert.Contains(references, path => path.Contains("PdfEditor.Core", StringComparison.Ordinal));
        Assert.Contains(references, path => path.Contains("PdfEditor.Rendering", StringComparison.Ordinal));
        Assert.Contains(references, path => path.Contains("PdfEditor.Engine", StringComparison.Ordinal));
    }

    private static string ReadProject(string name)
    {
        var path = Path.Combine(RepoPaths.FindRoot(), "src", name, name + ".csproj");
        return File.ReadAllText(path);
    }

    private static IReadOnlyList<string> ProjectReferences(string name)
    {
        var path = Path.Combine(RepoPaths.FindRoot(), "src", name, name + ".csproj");
        var document = XDocument.Load(path);
        return document
            .Descendants("ProjectReference")
            .Select(element => (string?)element.Attribute("Include") ?? string.Empty)
            .ToArray();
    }
}
