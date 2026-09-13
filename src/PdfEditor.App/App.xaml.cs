using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using PdfEditor.Core.Hosting;

namespace PdfEditor.App;

public partial class App : Application
{
    private Window? _window;

    public App()
    {
        InitializeComponent();
        UnhandledException += OnUnhandledException;
    }

    public static ApplicationRuntime Runtime { get; private set; } = null!;

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var applicationData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PdfEditor");

        Runtime = ApplicationRuntime.Start(
            applicationData,
            builder => builder.AddDebug());

        _window = new MainWindow(Runtime);
        _window.Closed += (_, _) => Runtime.Dispose();
        _window.Activate();
    }

    private static void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Runtime?.LoggerFactory
            .CreateLogger("PdfEditor.App")
            .LogError(e.Exception, "Unhandled UI exception.");
    }
}
