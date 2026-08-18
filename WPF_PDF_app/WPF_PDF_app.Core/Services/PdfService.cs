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
