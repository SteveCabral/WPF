using System.Windows.Media.Imaging;
using PdfSharp.Pdf;

namespace WPF_PDF_app.Core.Services;

/// <summary>
/// Service interface for rendering PDF pages to images.
/// Provides high-quality rendering of PDF content to BitmapSource for WPF display.
/// </summary>
public interface IPdfRenderingService
{
    /// <summary>
    /// Renders a PDF page to a BitmapSource image.
    /// </summary>
    /// <param name="page">The PDF page to render.</param>
    /// <param name="targetWidth">The target width in pixels for the rendered image.</param>
    /// <param name="dpi">The DPI (dots per inch) for rendering quality. Default is 96.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A BitmapSource containing the rendered page.</returns>
    /// <exception cref="ArgumentNullException">Thrown if page is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if targetWidth or dpi is invalid.</exception>
    Task<BitmapSource> RenderPageAsync(
        PdfPage page,
        int targetWidth,
        int dpi = 96,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders a PDF page with specific dimensions.
    /// </summary>
    /// <param name="page">The PDF page to render.</param>
    /// <param name="width">The target width in pixels.</param>
    /// <param name="height">The target height in pixels.</param>
    /// <param name="dpi">The DPI for rendering quality. Default is 96.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A BitmapSource containing the rendered page.</returns>
    Task<BitmapSource> RenderPageWithSizeAsync(
        PdfPage page,
        int width,
        int height,
        int dpi = 96,
        CancellationToken cancellationToken = default);
}
