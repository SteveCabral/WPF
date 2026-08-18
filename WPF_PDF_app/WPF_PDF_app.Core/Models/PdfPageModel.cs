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
