# Version 0.2.0 - Complete Source Code Backup

This file contains all the source code created for Version 0.2.0 in case files need to be recreated.

---

## WPF_PDF_app.Core/Models/PageOrientation.cs

```csharp
namespace WPF_PDF_app.Core.Models;

/// <summary>
/// Represents the orientation of a PDF page.
/// </summary>
public enum PageOrientation
{
	/// <summary>
	/// Portrait orientation (height >= width).
	/// </summary>
	Portrait,

	/// <summary>
	/// Landscape orientation (width > height).
	/// </summary>
	Landscape
}
```

---

## WPF_PDF_app.Core/Models/PdfPageModel.cs

```csharp
namespace WPF_PDF_app.Core.Models;

/// <summary>
/// Represents the metadata and properties of a single PDF page.
/// This is an immutable domain model containing page information.
/// </summary>
public class PdfPageModel
{
	/// <summary>
	/// Gets the one-based page number (1 = first page).
	/// </summary>
	public int PageNumber { get; init; }

	/// <summary>
	/// Gets the width of the page in points (72 points = 1 inch).
	/// </summary>
	public double Width { get; init; }

	/// <summary>
	/// Gets the height of the page in points (72 points = 1 inch).
	/// </summary>
	public double Height { get; init; }

	/// <summary>
	/// Gets the orientation of the page.
	/// </summary>
	public PageOrientation Orientation { get; init; }

	/// <summary>
	/// Gets whether the page is in portrait orientation (height >= width).
	/// </summary>
	public bool IsPortrait => Orientation == PageOrientation.Portrait;

	/// <summary>
	/// Gets whether the page is in landscape orientation (width > height).
	/// </summary>
	public bool IsLandscape => Orientation == PageOrientation.Landscape;

	/// <summary>
	/// Gets the aspect ratio (width / height) of the page.
	/// </summary>
	public double AspectRatio => Height > 0 ? Width / Height : 1.0;

	/// <summary>
	/// Gets the width in inches (width in points / 72).
	/// </summary>
	public double WidthInches => Width / 72.0;

	/// <summary>
	/// Gets the height in inches (height in points / 72).
	/// </summary>
	public double HeightInches => Height / 72.0;

	/// <summary>
	/// Creates a new instance of PdfPageModel.
	/// </summary>
	/// <param name="pageNumber">The one-based page number.</param>
	/// <param name="width">The width in points.</param>
	/// <param name="height">The height in points.</param>
	public PdfPageModel(int pageNumber, double width, double height)
	{
		PageNumber = pageNumber;
		Width = width;
		Height = height;
		Orientation = DetermineOrientation(width, height);
	}

	/// <summary>
	/// Determines the orientation based on width and height.
	/// </summary>
	private static PageOrientation DetermineOrientation(double width, double height)
	{
		return height >= width ? PageOrientation.Portrait : PageOrientation.Landscape;
	}

	/// <summary>
	/// Returns a string representation of the page model.
	/// </summary>
	public override string ToString()
	{
		return $"Page {PageNumber}: {Width:F1} x {Height:F1} pts ({Orientation})";
	}
}
```

---

## WPF_PDF_app.Core/ViewModels/PdfPageViewModel.cs

```csharp
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF_PDF_app.Core.Models;

namespace WPF_PDF_app.Core.ViewModels;

/// <summary>
/// View model for a single PDF page, providing observable properties for UI binding.
/// Wraps PdfPageModel and adds UI-specific properties like thumbnail image and selection state.
/// </summary>
public partial class PdfPageViewModel : ObservableObject
{
	/// <summary>
	/// Gets the underlying page model with metadata.
	/// </summary>
	public PdfPageModel PageModel { get; }

	/// <summary>
	/// Gets or sets the thumbnail image for this page.
	/// This is loaded asynchronously when the page becomes visible.
	/// </summary>
	[ObservableProperty]
	private BitmapSource? _thumbnailImage;

	/// <summary>
	/// Gets or sets whether this page is currently selected.
	/// </summary>
	[ObservableProperty]
	private bool _isSelected;

	/// <summary>
	/// Gets or sets whether the thumbnail is currently being loaded.
	/// </summary>
	[ObservableProperty]
	private bool _isLoading;

	/// <summary>
	/// Gets or sets whether the thumbnail has been loaded at least once.
	/// Used to implement lazy loading - only load when needed.
	/// </summary>
	[ObservableProperty]
	private bool _isLoaded;

	/// <summary>
	/// Gets the one-based page number for display.
	/// </summary>
	public int PageNumber => PageModel.PageNumber;

	/// <summary>
	/// Gets the display text for the page number.
	/// </summary>
	public string PageNumberText => $"Page {PageModel.PageNumber}";

	/// <summary>
	/// Gets the width in points.
	/// </summary>
	public double Width => PageModel.Width;

	/// <summary>
	/// Gets the height in points.
	/// </summary>
	public double Height => PageModel.Height;

	/// <summary>
	/// Gets the orientation of the page.
	/// </summary>
	public PageOrientation Orientation => PageModel.Orientation;

	/// <summary>
	/// Gets the dimensions display text.
	/// </summary>
	public string DimensionsText => $"{PageModel.Width:F0} × {PageModel.Height:F0} pts";

	/// <summary>
	/// Gets the dimensions in inches display text.
	/// </summary>
	public string DimensionsInchesText => $"{PageModel.WidthInches:F2}\" × {PageModel.HeightInches:F2}\"";

	/// <summary>
	/// Creates a new PdfPageViewModel.
	/// </summary>
	/// <param name="pageModel">The underlying page model.</param>
	public PdfPageViewModel(PdfPageModel pageModel)
	{
		PageModel = pageModel ?? throw new ArgumentNullException(nameof(pageModel));
	}

	/// <summary>
	/// Marks this page as loaded and sets the thumbnail image.
	/// Called by the rendering service when the thumbnail is ready.
	/// </summary>
	/// <param name="thumbnail">The rendered thumbnail image.</param>
	public void SetThumbnail(BitmapSource? thumbnail)
	{
		ThumbnailImage = thumbnail;
		IsLoaded = true;
		IsLoading = false;
	}

	/// <summary>
	/// Marks the thumbnail as loading.
	/// Called when lazy loading begins.
	/// </summary>
	public void BeginLoading()
	{
		IsLoading = true;
	}

	/// <summary>
	/// Returns a string representation of this view model.
	/// </summary>
	public override string ToString()
	{
		return $"PageVM: {PageModel}";
	}
}
```

---

## WPF_PDF_app.Core/ViewModels/MainViewModel.cs

```csharp
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
		// Create OpenFileDialog
		var dialog = new Microsoft.Win32.OpenFileDialog
		{
			Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
			Title = "Select a PDF File",
			InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
		};

		// Show dialog
		bool? result = dialog.ShowDialog();
		if (result != true)
		{
			return; // User cancelled
		}

		await LoadPdfAsync(dialog.FileName);
	}

	/// <summary>
	/// Loads a PDF file and creates view models for all pages.
	/// </summary>
	private async Task LoadPdfAsync(string filePath)
	{
		try
		{
			IsLoading = true;
			LoadingProgress = 0;
			LoadingMessage = "Opening PDF...";

			// Clear existing pages
			Pages.Clear();
			SelectedPage = null;
			PreviewImage = null;

			// Open the PDF
			await _pdfService.OpenPdfAsync(filePath);

			FileName = Path.GetFileName(filePath);
			PageCount = _pdfService.PageCount;

			LoadingMessage = $"Loading {PageCount} pages...";
			LoadingProgress = 10;

			// Create view models for all pages
			for (int i = 0; i < _pdfService.PageCount; i++)
			{
				var pageModel = _pdfService.GetPageModel(i);
				var pageViewModel = new PdfPageViewModel(pageModel);
				Pages.Add(pageViewModel);

				// Update progress
				LoadingProgress = 10 + (int)((i + 1) / (double)_pdfService.PageCount * 30);
			}

			LoadingMessage = "Rendering thumbnails...";

			// Render thumbnails for all pages (in background)
			await RenderAllThumbnailsAsync();

			LoadingProgress = 100;
			LoadingMessage = "Complete!";

			// Select first page by default
			if (Pages.Count > 0)
			{
				SelectedPage = Pages[0];
				await UpdatePreviewAsync();
			}

			// Show success message
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

	/// <summary>
	/// Renders thumbnails for all pages.
	/// </summary>
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

				// Update progress
				LoadingProgress = 40 + (int)((i + 1) / (double)Pages.Count * 60);
			}
			catch (Exception ex)
			{
				// Log error but continue with other pages
				System.Diagnostics.Debug.WriteLine($"Error rendering page {i + 1}: {ex.Message}");
				pageViewModel.SetThumbnail(null);
			}
		}
	}

	/// <summary>
	/// Updates the large preview image when selection changes.
	/// </summary>
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

	/// <summary>
	/// Called when the SelectedPage property changes.
	/// Updates the IsSelected state and preview image.
	/// </summary>
	partial void OnSelectedPageChanged(PdfPageViewModel? oldValue, PdfPageViewModel? newValue)
	{
		// Deselect old page
		if (oldValue != null)
		{
			oldValue.IsSelected = false;
		}

		// Select new page
		if (newValue != null)
		{
			newValue.IsSelected = true;

			// Update preview asynchronously
			_ = UpdatePreviewAsync();
		}
	}
}
```

---

## WPF_PDF_app.Core/Services/IPdfService.cs

```csharp
using PdfSharp.Pdf;
using WPF_PDF_app.Core.Models;

namespace WPF_PDF_app.Core.Services;

/// <summary>
/// Service interface for PDF document operations.
/// Handles opening, reading, and managing PDF documents.
/// </summary>
public interface IPdfService : IDisposable
{
	/// <summary>
	/// Gets whether a PDF document is currently loaded.
	/// </summary>
	bool IsDocumentLoaded { get; }

	/// <summary>
	/// Gets the file path of the currently loaded PDF document.
	/// </summary>
	string? CurrentFilePath { get; }

	/// <summary>
	/// Gets the page count of the currently loaded document.
	/// Returns 0 if no document is loaded.
	/// </summary>
	int PageCount { get; }

	/// <summary>
	/// Opens a PDF document from the specified file path.
	/// </summary>
	Task OpenPdfAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the page model for a specific page index.
	/// </summary>
	PdfPageModel GetPageModel(int pageIndex);

	/// <summary>
	/// Gets the PdfPage for a specific page index.
	/// </summary>
	PdfPage GetPage(int pageIndex);

	/// <summary>
	/// Closes the currently loaded PDF document and releases resources.
	/// </summary>
	void CloseDocument();
}
```

---

## WPF_PDF_app.Core/Services/PdfService.cs

```csharp
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using WPF_PDF_app.Core.Models;

namespace WPF_PDF_app.Core.Services;

/// <summary>
/// Implementation of IPdfService for PDF document operations.
/// </summary>
public class PdfService : IPdfService
{
	private PdfDocument? _currentDocument;
	private string? _currentFilePath;
	private bool _disposed;

	public bool IsDocumentLoaded => _currentDocument != null;
	public string? CurrentFilePath => _currentFilePath;
	public int PageCount => _currentDocument?.PageCount ?? 0;

	public async Task OpenPdfAsync(string filePath, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(filePath))
			throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

		if (!File.Exists(filePath))
			throw new FileNotFoundException($"The file '{filePath}' does not exist.", filePath);

		if (!Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
			throw new ArgumentException("The file must have a .pdf extension.", nameof(filePath));

		CloseDocument();

		try
		{
			_currentDocument = await Task.Run(() =>
			{
				cancellationToken.ThrowIfCancellationRequested();
				return PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
			}, cancellationToken);

			_currentFilePath = filePath;

			if (_currentDocument.PageCount == 0)
			{
				CloseDocument();
				throw new InvalidOperationException("The PDF document contains no pages.");
			}
		}
		catch
		{
			CloseDocument();
			throw;
		}
	}

	public PdfPageModel GetPageModel(int pageIndex)
	{
		if (!IsDocumentLoaded)
			throw new InvalidOperationException("No PDF document is currently loaded.");

		if (pageIndex < 0 || pageIndex >= PageCount)
			throw new ArgumentOutOfRangeException(nameof(pageIndex));

		PdfPage page = _currentDocument!.Pages[pageIndex];
		return new PdfPageModel(pageIndex + 1, page.Width.Point, page.Height.Point);
	}

	public PdfPage GetPage(int pageIndex)
	{
		if (!IsDocumentLoaded)
			throw new InvalidOperationException("No PDF document is currently loaded.");

		if (pageIndex < 0 || pageIndex >= PageCount)
			throw new ArgumentOutOfRangeException(nameof(pageIndex));

		return _currentDocument!.Pages[pageIndex];
	}

	public void CloseDocument()
	{
		if (_currentDocument != null)
		{
			_currentDocument.Dispose();
			_currentDocument = null;
			_currentFilePath = null;
		}
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			CloseDocument();
			_disposed = true;
		}
		GC.SuppressFinalize(this);
	}
}
```

---

## WPF_PDF_app.Core/Services/IPdfRenderingService.cs

```csharp
using System.Windows.Media.Imaging;
using PdfSharp.Pdf;

namespace WPF_PDF_app.Core.Services;

/// <summary>
/// Service interface for rendering PDF pages to images.
/// </summary>
public interface IPdfRenderingService
{
	Task<BitmapSource> RenderPageAsync(
		PdfPage page,
		int targetWidth,
		int dpi = 96,
		CancellationToken cancellationToken = default);

	Task<BitmapSource> RenderPageWithSizeAsync(
		PdfPage page,
		int width,
		int height,
		int dpi = 96,
		CancellationToken cancellationToken = default);
}
```

---

## WPF_PDF_app.Core/Services/PdfRenderingService.cs

```csharp
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace WPF_PDF_app.Core.Services;

/// <summary>
/// Implementation of IPdfRenderingService for rendering PDF pages to images.
/// Uses PDFsharp's XPdfForm for proper PDF content rendering.
/// </summary>
public class PdfRenderingService : IPdfRenderingService
{
	public async Task<BitmapSource> RenderPageAsync(
		PdfPage page,
		int targetWidth,
		int dpi = 96,
		CancellationToken cancellationToken = default)
	{
		if (page == null)
			throw new ArgumentNullException(nameof(page));

		if (targetWidth <= 0)
			throw new ArgumentOutOfRangeException(nameof(targetWidth));

		if (dpi <= 0)
			throw new ArgumentOutOfRangeException(nameof(dpi));

		double aspectRatio = page.Height.Point / page.Width.Point;
		int targetHeight = (int)(targetWidth * aspectRatio);

		return await RenderPageWithSizeAsync(page, targetWidth, targetHeight, dpi, cancellationToken);
	}

	public async Task<BitmapSource> RenderPageWithSizeAsync(
		PdfPage page,
		int width,
		int height,
		int dpi = 96,
		CancellationToken cancellationToken = default)
	{
		if (page == null)
			throw new ArgumentNullException(nameof(page));

		if (width <= 0 || height <= 0)
			throw new ArgumentOutOfRangeException("Width and height must be positive.");

		if (dpi <= 0)
			throw new ArgumentOutOfRangeException(nameof(dpi));

		return await Task.Run(() =>
		{
			cancellationToken.ThrowIfCancellationRequested();

			DrawingVisual drawingVisual = new DrawingVisual();

			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				XGraphics gfx = XGraphics.FromDrawingContext(
					drawingContext,
					new XSize(width, height),
					XGraphicsUnit.Pixel);

				try
				{
					gfx.DrawRectangle(XBrushes.White, 0, 0, width, height);

					using (MemoryStream stream = new MemoryStream())
					{
						using (PdfDocument tempDoc = new PdfDocument())
						{
							tempDoc.AddPage(page);
							tempDoc.Save(stream, false);
							stream.Position = 0;

							using (PdfDocument renderDoc = PdfReader.Open(stream, PdfDocumentOpenMode.Import))
							{
								XPdfForm form = XPdfForm.FromDocument(renderDoc);

								double scaleX = width / form.PixelWidth;
								double scaleY = height / form.PixelHeight;
								double scale = Math.Min(scaleX, scaleY);

								double scaledWidth = form.PixelWidth * scale;
								double scaledHeight = form.PixelHeight * scale;
								double x = (width - scaledWidth) / 2;
								double y = (height - scaledHeight) / 2;

								gfx.DrawImage(form, x, y, scaledWidth, scaledHeight);
							}
						}
					}
				}
				catch (Exception ex)
				{
					DrawErrorMessage(gfx, width, height, ex.Message);
				}
			}

			cancellationToken.ThrowIfCancellationRequested();

			RenderTargetBitmap bitmap = new RenderTargetBitmap(
				width, height, dpi, dpi, PixelFormats.Pbgra32);
			bitmap.Render(drawingVisual);
			bitmap.Freeze();

			return bitmap;
		}, cancellationToken);
	}

	private void DrawErrorMessage(XGraphics gfx, int width, int height, string errorMessage)
	{
		gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(255, 255, 240, 240)), 0, 0, width, height);

		XFont boldFont = new XFont("Segoe UI", 14, XFontStyleEx.Bold);
		XFont font = new XFont("Segoe UI", 12, XFontStyleEx.Regular);

		string title = "⚠ Rendering Error";
		string message = $"Failed to render page:\n{errorMessage}";

		gfx.DrawString(title, boldFont, XBrushes.DarkRed, 
			new XRect(10, 10, width - 20, height - 20), XStringFormats.TopLeft);
		gfx.DrawString(message, font, XBrushes.Black, 
			new XRect(10, 40, width - 20, height - 50), XStringFormats.TopLeft);
	}
}
```

---

## WPF_PDF_app/Converters/ValueConverters.cs

```csharp
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF_PDF_app.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value is bool boolValue && boolValue ? Visibility.Visible : Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value is Visibility visibility && visibility == Visibility.Visible;
	}
}

public class InverseBoolToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value is bool boolValue && boolValue ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value is Visibility visibility && visibility != Visibility.Visible;
	}
}

public class NullToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value == null ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}

public class ZeroToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is int intValue)
		{
			return intValue == 0 ? Visibility.Visible : Visibility.Collapsed;
		}
		return Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
```

---

**End of Source Code Backup**

Use this file to recreate any missing source files if needed.
