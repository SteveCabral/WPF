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
