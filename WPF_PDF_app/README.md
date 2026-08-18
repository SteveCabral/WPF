# PDF Organizer - WPF Application

A Windows desktop application for organizing, viewing, and manipulating PDF documents using WPF and PDFsharp.

## Current Version: 0.1.0 (Proof of Concept)

**Status:** ✅ Complete  
**Based on:** Design Document Section 15

This version validates that PDFsharp-WPF can successfully open, parse, and display PDF documents.

## Features (v0.1.0)

- ✅ Open PDF files via file dialog
- ✅ Display accurate page count
- ✅ Show dimensions for all pages (in points)
- ✅ Render PDF pages to high-quality images
- ✅ Display rendered pages with scroll support
- ✅ Comprehensive error handling
- ✅ US Letter/Legal/A4 dimension detection

## Technology Stack

- **Framework:** .NET 10.0
- **UI:** WPF (Windows Presentation Foundation)
- **PDF Library:** PDFsharp-wpf 6.2.4
- **IDE:** Visual Studio 2026

## Documentation

### 📘 Comprehensive Documentation
Open [**Docs/ProofOfConcept.html**](Docs/ProofOfConcept.html) in your browser for:
- WPF concepts primer for beginners
- PDFsharp API introduction
- Step-by-step implementation guide
- Code explanations with examples
- Testing guidelines and troubleshooting

### 🗺️ Project Roadmap
See [**Docs/RoadMap.html**](Docs/RoadMap.html) for:
- Version history and release notes
- Upcoming features by phase
- Detailed version timeline

## Building the Application

### Using Visual Studio
1. Open `WPF_PDF_app.sln`
2. Press `Ctrl+Shift+B` to build
3. Press `F5` to run

### Using Command Line
```powershell
cd C:\Repos\WPF\WPF_PDF_app
dotnet restore
dotnet build
dotnet run
```

### Output Location
- **Debug:** `bin\Debug\net10.0-windows\WPF_PDF_app.exe`
- **Release:** `bin\Release\net10.0-windows\WPF_PDF_app.exe`

## Usage

1. Launch the application
2. Click **"Open PDF"** button
3. Select a PDF file from your computer
4. View:
   - Total page count
   - Dimensions of all pages (in points)
   - Rendered preview of the first page

## Testing

Test with various PDF types:
- Simple text PDFs
- Multi-page documents (10+, 50+, 100+ pages)
- PDFs with images
- Mixed page sizes (Letter and Legal)

See [Testing Guidelines](Docs/ProofOfConcept.html#testing) for details.

## Known Limitations

- **Rendering:** Placeholder rendering; full PDF content rendering planned for v0.2.0
- **Performance:** Large documents (100+ pages) not yet optimized
- **Features:** Read-only viewing; editing features coming in Phases 4-5

## Version Roadmap

| Version | Phase | Status | Features |
|---------|-------|--------|----------|
| **0.1.0** | Proof of Concept | ✅ Complete | Open PDF, count pages, read dimensions, basic rendering |
| 0.2.0 | Page Thumbnails | 🔜 Planned | Thumbnail gallery, page selection, click to preview |
| 0.3.0 | Filtering | 🔜 Planned | Filter by page size (Letter/Legal/A4), portrait/landscape |
| 0.4.0 | Page Organization | 🔜 Planned | Drag-and-drop reordering, delete pages, rotate pages |
| 0.5.0 | Saving & Insertion | 🔜 Planned | Save modified PDF, add blank pages, import from other PDFs |
| 1.0.0 | Advanced Features | 🔜 Planned | Undo/redo, combine PDFs, import images, annotations |

See [RoadMap.html](Docs/RoadMap.html) for detailed breakdown.

## Project Structure

```
WPF_PDF_app/
├── WPF_PDF_app.sln              # Visual Studio solution
├── WPF_PDF_app.csproj           # Project file
├── README.md                     # This file
├── App.xaml                      # WPF application definition
├── App.xaml.cs                   # Application code-behind
├── MainWindow.xaml               # Main window UI (XAML)
├── MainWindow.xaml.cs            # Main window logic (C#)
└── Docs/
	├── ProofOfConcept.html      # Implementation documentation
	└── RoadMap.html             # Version history & future plans
```

## Dependencies

- **PDFsharp-wpf** (6.2.4) - PDF manipulation and rendering

## Learning Resources

- [WPF Documentation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [PDFsharp Official Docs](https://docs.pdfsharp.net/)
- [Project Documentation](Docs/ProofOfConcept.html)

## Version Information

**Current Release:** v0.1.0 (Proof of Concept)  
**Release Date:** 2024  
**Corresponds to:** Design Document Section 15  
**Next Milestone:** v0.2.0 (Page Thumbnails - Phase 2)

---

**Author:** Steve Cabral  
**Repository:** [https://github.com/SteveCabral/WPF](https://github.com/SteveCabral/WPF)  
**Development Environment:** Visual Studio 2026, .NET 10, C#
