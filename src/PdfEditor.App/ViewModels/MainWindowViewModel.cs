using PdfEditor.Core.Hosting;
using PdfEditor.Core.Models;

namespace PdfEditor.App.ViewModels;

public sealed class MainWindowViewModel
{
    public MainWindowViewModel(ApplicationRuntime runtime)
    {
        Runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
        Title = "PDF Editor";
        StatusText = runtime.RecentDocuments.Count == 0
            ? "No document open"
            : $"No document open · {runtime.RecentDocuments.Count} recent";
        ViewportMessage = "Open a PDF to begin. File opening is implemented in milestone M2.";
        HasDocument = false;
        Settings = runtime.Settings;
    }

    public ApplicationRuntime Runtime { get; }
    public string Title { get; }
    public string StatusText { get; }
    public string ViewportMessage { get; }
    public bool HasDocument { get; }
    public ApplicationSettings Settings { get; }
}
