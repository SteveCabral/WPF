using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PdfSharp.Pdf;
using WPF_PDF_app.Core.Services;

namespace WPF_PDF_app.Core.ViewModels;

/// <summary>
/// Main view model for the PDF Organizer application.
/// Orchestrates PDF loading, page management, and user interactions.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private const string AppVersion = "0.2.0";
    private const string AppPhase = "Page Thumbnails";

    private readonly IPdfService _pdfService;
    private readonly IPdfRenderingService _renderingService;

    /// <summary>
    /// Gets the collection of PDF pages for display.
    /// </summary>
    public ObservableCollection<PdfPageViewModel> Pages { get; } = new();

    /// <summary>
    /// Gets or sets the currently selected page.
    /// </summary>
    [ObservableProperty]
    private PdfPageViewModel? _selectedPage;

    /// <summary>
    /// Gets or sets the large preview image for the selected page.
    /// </summary>
    [ObservableProperty]
    private BitmapSource? _previewImage;

    /// <summary>
    /// Gets or sets whether the application is currently loading a PDF.
    /// </summary>
    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Gets or sets the loading progress percentage (0-100).
    /// </summary>
    [ObservableProperty]
    private int _loadingProgress;

    /// <summary>
    /// Gets or sets the loading status message.
    /// </summary>
    [ObservableProperty]
    private string _loadingMessage = string.Empty;

    /// <summary>
    /// Gets or sets the current file name.
    /// </summary>
    [ObservableProperty]
    private string _fileName = "No PDF loaded";

    /// <summary>
    /// Gets or sets the total page count.
    /// </summary>
    [ObservableProperty]
    private int _pageCount;

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public string WindowTitle => $"PDF Organizer v{AppVersion} - {AppPhase}";

    /// <summary>
    /// Gets the version information text.
    /// </summary>
    public string VersionText => $"v{AppVersion}";

    /// <summary>
    /// Creates a new MainViewModel.
    /// </summary>
    /// <param name="pdfService">The PDF service for document operations.</param>
    /// <param name="renderingService">The rendering service for creating images.</param>
    public MainViewModel(IPdfService pdfService, IPdfRenderingService renderingService)
    {
        _pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
        _renderingService = renderingService ?? throw new ArgumentNullException(nameof(renderingService));
    }

    /// <summary>
    /// Command to open a PDF file.
    /// </summary>
    [RelayCommand]
    private async Task OpenPdfAsync()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
            Title = "Select a PDF File",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        bool? result = dialog.ShowDialog();
        if (result != true)
        {
            return;
        }

        await LoadPdfAsync(dialog.FileName);
    }

    private async Task LoadPdfAsync(string filePath)
    {
        try
        {
            IsLoading = true;
            LoadingProgress = 0;
            LoadingMessage = "Opening PDF...";

            Pages.Clear();
            SelectedPage = null;
            PreviewImage = null;

            await _pdfService.OpenPdfAsync(filePath);

            FileName = Path.GetFileName(filePath);
            PageCount = _pdfService.PageCount;

            LoadingMessage = $"Loading {PageCount} pages...";
            LoadingProgress = 10;

            for (int i = 0; i < _pdfService.PageCount; i++)
            {
                var pageModel = _pdfService.GetPageModel(i);
                var pageViewModel = new PdfPageViewModel(pageModel);
                Pages.Add(pageViewModel);
                LoadingProgress = 10 + (int)((i + 1) / (double)_pdfService.PageCount * 30);
            }

            LoadingMessage = "Rendering thumbnails...";
            await RenderAllThumbnailsAsync();

            LoadingProgress = 100;
            LoadingMessage = "Complete!";

            if (Pages.Count > 0)
            {
                SelectedPage = Pages[0];
                await UpdatePreviewAsync();
            }

            MessageBox.Show(
                $"PDF loaded successfully!\n\nFile: {FileName}\nPages: {PageCount}",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (FileNotFoundException ex)
        {
            MessageBox.Show(
                $"File not found:\n{ex.Message}",
                "File Not Found",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(
                $"Invalid file:\n{ex.Message}",
                "Invalid File",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error loading PDF:\n{ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            FileName = "Error loading PDF";
            PageCount = 0;
        }
        finally
        {
            IsLoading = false;
            LoadingMessage = string.Empty;
            LoadingProgress = 0;
        }
    }

    private async Task RenderAllThumbnailsAsync()
    {
        const int thumbnailWidth = 150;
        const int thumbnailDpi = 96;

        for (int i = 0; i < Pages.Count; i++)
        {
            var pageViewModel = Pages[i];
            pageViewModel.BeginLoading();

            try
            {
                PdfPage page = _pdfService.GetPage(i);
                BitmapSource thumbnail = await _renderingService.RenderPageAsync(
                    page,
                    thumbnailWidth,
                    thumbnailDpi);

                pageViewModel.SetThumbnail(thumbnail);
                LoadingProgress = 40 + (int)((i + 1) / (double)Pages.Count * 60);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error rendering page {i + 1}: {ex.Message}");
                pageViewModel.SetThumbnail(null);
            }
        }
    }

    private async Task UpdatePreviewAsync()
    {
        if (SelectedPage == null)
        {
            PreviewImage = null;
            return;
        }

        try
        {
            const int previewWidth = 800;
            const int previewDpi = 150;

            int pageIndex = SelectedPage.PageNumber - 1;
            PdfPage page = _pdfService.GetPage(pageIndex);

            PreviewImage = await _renderingService.RenderPageAsync(
                page,
                previewWidth,
                previewDpi);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error rendering preview:\n{ex.Message}",
                "Rendering Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            PreviewImage = null;
        }
    }

    partial void OnSelectedPageChanged(PdfPageViewModel? oldValue, PdfPageViewModel? newValue)
    {
        if (oldValue != null)
        {
            oldValue.IsSelected = false;
        }

        if (newValue != null)
        {
            newValue.IsSelected = true;
            _ = UpdatePreviewAsync();
        }
    }
}
