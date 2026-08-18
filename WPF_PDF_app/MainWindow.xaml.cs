using System.Windows;

namespace WPF_PDF_app;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// Main window for the PDF Organizer application using MVVM pattern.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Create services using full namespace qualification
        var pdfService = new WPF_PDF_app.Core.Services.PdfService();
        var renderingService = new WPF_PDF_app.Core.Services.PdfRenderingService();

        // Create and set the view model as DataContext
        DataContext = new WPF_PDF_app.Core.ViewModels.MainViewModel(pdfService, renderingService);
    }
}
