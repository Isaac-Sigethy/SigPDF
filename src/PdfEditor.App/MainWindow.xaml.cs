using Microsoft.UI.Xaml;
using PdfEditor.App.ViewModels;
using PdfEditor.Core.Hosting;

namespace PdfEditor.App;

public sealed partial class MainWindow : Window
{
    public MainWindow(ApplicationRuntime runtime)
    {
        InitializeComponent();
        ViewModel = new MainWindowViewModel(runtime);
        Title = ViewModel.Title;
        StatusTextBlock.Text = ViewModel.StatusText;
        ViewportMessageText.Text = ViewModel.ViewportMessage;
    }

    public MainWindowViewModel ViewModel { get; }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e) => Close();
}
