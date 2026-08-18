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
