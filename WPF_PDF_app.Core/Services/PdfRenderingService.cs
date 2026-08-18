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
    /// <inheritdoc/>
    public async Task<BitmapSource> RenderPageAsync(
        PdfPage page,
        int targetWidth,
        int dpi = 96,
        CancellationToken cancellationToken = default)
    {
        // Validate parameters
        if (page == null)
        {
            throw new ArgumentNullException(nameof(page));
        }

        if (targetWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(targetWidth), "Target width must be positive.");
        }

        if (dpi <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dpi), "DPI must be positive.");
        }

        // Calculate target height maintaining aspect ratio
        double aspectRatio = page.Height.Point / page.Width.Point;
        int targetHeight = (int)(targetWidth * aspectRatio);

        return await RenderPageWithSizeAsync(page, targetWidth, targetHeight, dpi, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<BitmapSource> RenderPageWithSizeAsync(
        PdfPage page,
        int width,
        int height,
        int dpi = 96,
        CancellationToken cancellationToken = default)
    {
        // Validate parameters
        if (page == null)
        {
            throw new ArgumentNullException(nameof(page));
        }

        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException("Width and height must be positive.");
        }

        if (dpi <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dpi), "DPI must be positive.");
        }

        // Render on a background thread to avoid blocking UI
        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Create a DrawingVisual to render the PDF page
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                // Create XGraphics from DrawingContext
                XGraphics gfx = XGraphics.FromDrawingContext(
                    drawingContext,
                    new XSize(width, height),
                    XGraphicsUnit.Pixel);

                try
                {
                    // Draw white background
                    gfx.DrawRectangle(XBrushes.White, 0, 0, width, height);

                    // Create XPdfForm from the page to render PDF content
                    // We need to create a temporary document with this page to use XPdfForm
                    using (MemoryStream stream = new MemoryStream())
                    {
                        // Create a new document and add the page
                        using (PdfDocument tempDoc = new PdfDocument())
                        {
                            // Import the page into the temporary document
                            tempDoc.AddPage(page);

                            // Save to memory stream
                            tempDoc.Save(stream, false);
                            stream.Position = 0;

                            // Open the temporary document for rendering
                            using (PdfDocument renderDoc = PdfReader.Open(stream, PdfDocumentOpenMode.Import))
                            {
                                // Create XPdfForm from the first page
                                XPdfForm form = XPdfForm.FromDocument(renderDoc);

                                // Calculate scale to fit the target size
                                double scaleX = width / form.PixelWidth;
                                double scaleY = height / form.PixelHeight;
                                double scale = Math.Min(scaleX, scaleY);

                                // Calculate centered position
                                double scaledWidth = form.PixelWidth * scale;
                                double scaledHeight = form.PixelHeight * scale;
                                double x = (width - scaledWidth) / 2;
                                double y = (height - scaledHeight) / 2;

                                // Draw the PDF form
                                gfx.DrawImage(form, x, y, scaledWidth, scaledHeight);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // On rendering error, draw error message
                    DrawErrorMessage(gfx, width, height, ex.Message);
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Render the DrawingVisual to a bitmap
            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                width,
                height,
                dpi,
                dpi,
                PixelFormats.Pbgra32);

            bitmap.Render(drawingVisual);

            // Freeze the bitmap to make it cross-thread accessible
            bitmap.Freeze();

            return bitmap;
        }, cancellationToken);
    }

    /// <summary>
    /// Draws an error message on the graphics context when rendering fails.
    /// </summary>
    private void DrawErrorMessage(XGraphics gfx, int width, int height, string errorMessage)
    {
        // Draw red background to indicate error
        gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(255, 255, 240, 240)), 0, 0, width, height);

        // Draw error icon and message
        XFont font = new XFont("Segoe UI", 12, XFontStyleEx.Regular);
        XFont boldFont = new XFont("Segoe UI", 14, XFontStyleEx.Bold);

        string title = "⚠ Rendering Error";
        string message = $"Failed to render page:\n{errorMessage}";

        gfx.DrawString(title, boldFont, XBrushes.DarkRed, new XRect(10, 10, width - 20, height - 20), XStringFormats.TopLeft);
        gfx.DrawString(message, font, XBrushes.Black, new XRect(10, 40, width - 20, height - 50), XStringFormats.TopLeft);
    }
}
