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
