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
    /// <param name="filePath">The path to the PDF file.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A task that completes when the document is opened.</returns>
    /// <exception cref="ArgumentException">Thrown if the file path is invalid.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
    /// <exception cref="PdfSharp.Pdf.IO.PdfReaderException">Thrown if the PDF is invalid or corrupted.</exception>
    Task OpenPdfAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the page model for a specific page index.
    /// </summary>
    /// <param name="pageIndex">The zero-based page index.</param>
    /// <returns>The page model with metadata.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no document is loaded.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the page index is invalid.</exception>
    PdfPageModel GetPageModel(int pageIndex);

    /// <summary>
    /// Gets the PdfPage for a specific page index.
    /// Used by rendering service to access the raw PDF page.
    /// </summary>
    /// <param name="pageIndex">The zero-based page index.</param>
    /// <returns>The PdfPage object.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no document is loaded.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the page index is invalid.</exception>
    PdfPage GetPage(int pageIndex);

    /// <summary>
    /// Closes the currently loaded PDF document and releases resources.
    /// </summary>
    void CloseDocument();
}
