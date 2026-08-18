using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using WPF_PDF_app.Core.Models;

namespace WPF_PDF_app.Core.Services;

/// <summary>
/// Implementation of IPdfService for PDF document operations.
/// Manages the lifecycle of PDF documents and provides access to page data.
/// </summary>
public class PdfService : IPdfService
{
    private PdfDocument? _currentDocument;
    private string? _currentFilePath;
    private bool _disposed;

    /// <inheritdoc/>
    public bool IsDocumentLoaded => _currentDocument != null;

    /// <inheritdoc/>
    public string? CurrentFilePath => _currentFilePath;

    /// <inheritdoc/>
    public int PageCount => _currentDocument?.PageCount ?? 0;

    /// <inheritdoc/>
    public async Task OpenPdfAsync(string filePath, CancellationToken cancellationToken = default)
    {
        // Validate file path
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file '{filePath}' does not exist.", filePath);
        }

        // Validate file extension
        if (!Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The file must have a .pdf extension.", nameof(filePath));
        }

        // Close any existing document
        CloseDocument();

        try
        {
            // Open the PDF document asynchronously
            // PdfReader.Open is synchronous, so we run it on a background thread
            _currentDocument = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
            }, cancellationToken);

            _currentFilePath = filePath;

            // Validate document has pages
            if (_currentDocument.PageCount == 0)
            {
                CloseDocument();
                throw new InvalidOperationException("The PDF document contains no pages.");
            }
        }
        catch (Exception)
        {
            // Ensure cleanup on error
            CloseDocument();
            throw;
        }
    }

    /// <inheritdoc/>
    public PdfPageModel GetPageModel(int pageIndex)
    {
        ValidateDocumentLoaded();
        ValidatePageIndex(pageIndex);

        PdfPage page = _currentDocument!.Pages[pageIndex];

        // Create the page model with one-based page number
        return new PdfPageModel(
            pageNumber: pageIndex + 1,
            width: page.Width.Point,
            height: page.Height.Point
        );
    }

    /// <inheritdoc/>
    public PdfPage GetPage(int pageIndex)
    {
        ValidateDocumentLoaded();
        ValidatePageIndex(pageIndex);

        return _currentDocument!.Pages[pageIndex];
    }

    /// <inheritdoc/>
    public void CloseDocument()
    {
        if (_currentDocument != null)
        {
            _currentDocument.Dispose();
            _currentDocument = null;
            _currentFilePath = null;
        }
    }

    /// <summary>
    /// Validates that a document is currently loaded.
    /// </summary>
    private void ValidateDocumentLoaded()
    {
        if (!IsDocumentLoaded)
        {
            throw new InvalidOperationException("No PDF document is currently loaded.");
        }
    }

    /// <summary>
    /// Validates that a page index is within valid range.
    /// </summary>
    private void ValidatePageIndex(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= PageCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageIndex),
                $"Page index {pageIndex} is out of range. Document has {PageCount} pages.");
        }
    }

    /// <summary>
    /// Disposes resources used by the service.
    /// </summary>
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
