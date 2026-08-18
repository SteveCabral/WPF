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
