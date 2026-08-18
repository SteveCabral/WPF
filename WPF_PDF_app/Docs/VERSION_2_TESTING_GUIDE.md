# Version 0.2.0 Testing Guide

**Purpose:** Once the build issue is resolved, use this guide to test Version 0.2.0 features.

---

## Pre-Test Checklist

- [ ] Solution builds without errors
- [ ] Both projects (WPF_PDF_app and WPF_PDF_app.Core) build successfully
- [ ] No compiler warnings related to namespaces or references
- [ ] Application launches without crashes

---

## Manual Test Cases

### Test 1: Application Launch
**Expected:** Application opens with three-column layout

1. Press F5 in Visual Studio to run
2. Verify window opens with title: "PDF Organizer v0.2.0 - Page Thumbnails"
3. Verify three panels visible:
   - Left: Thumbnail gallery (empty, with "Open PDF" button)
   - Center: Large preview area (shows "Open a PDF to get started")
   - Right: Document info panel

**Pass Criteria:**
- ✅ Window opens without errors
- ✅ Title bar shows "v0.2.0"
- ✅ "Open PDF" button is visible and enabled
- ✅ UI is responsive

---

### Test 2: Open PDF (Small Document)
**Test PDF:** 5-10 page document

1. Click "Open PDF" button
2. Select a small PDF file (5-10 pages)
3. Observe loading indicators

**Expected Behavior:**
- Progress bar appears at top of center panel
- Status message updates: "Opening PDF..." → "Loading X pages..." → "Rendering thumbnails..."
- Success dialog appears: "PDF loaded successfully!"
- Thumbnails appear in left panel
- First page selected and displayed in center preview
- Info panel shows:
  - File name
  - Total pages
  - Selected page number
  - Dimensions (points and inches)
  - Orientation

**Pass Criteria:**
- ✅ No errors during load
- ✅ All thumbnails rendered correctly
- ✅ First page automatically selected
- ✅ Preview image shows PDF content (not placeholder)
- ✅ Page count matches PDF

**Time Expectation:** < 5 seconds for 10-page PDF

---

### Test 3: Thumbnail Selection
**Prerequisite:** PDF loaded from Test 2

1. Click different thumbnails in left panel
2. Observe preview updates

**Expected Behavior:**
- Clicked thumbnail gets blue border
- Previously selected thumbnail loses blue border
- Large preview updates immediately
- Info panel updates with selected page info
- No lag or freezing

**Pass Criteria:**
- ✅ Selection highlighting works
- ✅ Preview updates < 1 second
- ✅ Info panel shows correct page data
- ✅ Can select any page

---

### Test 4: Scrolling Thumbnails
**Prerequisite:** PDF with 20+ pages loaded

1. Scroll through thumbnail list
2. Observe behavior

**Expected Behavior:**
- Smooth scrolling
- Thumbnails load as they come into view (lazy loading)
- No UI freezing
- Scrollbar works correctly

**Pass Criteria:**
- ✅ Scrolling is smooth
- ✅ All thumbnails eventually load
- ✅ No crashes or errors
- ✅ Memory usage reasonable

---

### Test 5: Large PDF Performance
**Test PDF:** 50-100 page document

1. Click "Open PDF"
2. Select large PDF
3. Monitor performance

**Expected Behavior:**
- Progress bar shows incremental progress
- UI remains responsive during load
- May take 10-30 seconds depending on size
- Success dialog appears when complete
- Can immediately interact with thumbnails

**Pass Criteria:**
- ✅ No crashes or timeouts
- ✅ UI doesn't freeze completely
- ✅ Progress feedback visible
- ✅ Eventually completes successfully
- ✅ Memory usage < 1 GB

**Time Expectation:** 
- 50 pages: ~15 seconds
- 100 pages: ~30 seconds

---

### Test 6: Mixed Page Sizes
**Test PDF:** Document with different page sizes (Letter, Legal, A4)

1. Open mixed-size PDF
2. Click through different pages

**Expected Behavior:**
- All pages render correctly
- Thumbnails maintain aspect ratios
- Preview shows correct proportions
- Info panel shows accurate dimensions

**Pass Criteria:**
- ✅ No distorted images
- ✅ Dimensions reported correctly
- ✅ Portrait/Landscape detection works
- ✅ All page sizes render

---

### Test 7: Landscape Pages
**Test PDF:** Document with landscape pages

1. Open landscape PDF
2. Verify rendering

**Expected Behavior:**
- Thumbnails show correct orientation
- Preview shows full page (not cropped)
- Info panel shows "Landscape" orientation

**Pass Criteria:**
- ✅ Orientation detected correctly
- ✅ Images not stretched or cropped
- ✅ Dimensions width > height

---

### Test 8: Open Multiple PDFs
**Test:** Open different PDFs in succession

1. Open PDF A
2. Verify it loads
3. Click "Open PDF" again
4. Open PDF B
5. Repeat 2-3 times

**Expected Behavior:**
- Previous PDF unloaded
- New PDF loads cleanly
- Thumbnails replaced
- No memory leaks
- No errors

**Pass Criteria:**
- ✅ Each PDF loads successfully
- ✅ Previous PDF data cleared
- ✅ Memory usage doesn't grow excessively
- ✅ No crashes after multiple opens

---

### Test 9: Error Handling - Invalid File
**Test:** Try to open non-PDF file

1. Click "Open PDF"
2. Change file filter to "All Files"
3. Select .txt or .jpg file
4. Click Open

**Expected Behavior:**
- Error dialog: "The file must have a .pdf extension"
- Application remains stable
- Can try again

**Pass Criteria:**
- ✅ Appropriate error message
- ✅ No crash
- ✅ Can open valid PDF afterward

---

### Test 10: Error Handling - Corrupted PDF
**Test:** Open corrupted PDF file

1. Click "Open PDF"
2. Select corrupted/damaged PDF
3. Observe behavior

**Expected Behavior:**
- Error dialog: "Invalid or corrupted PDF file: [details]"
- Application remains stable
- UI resets to empty state

**Pass Criteria:**
- ✅ Error caught gracefully
- ✅ User-friendly message
- ✅ No crash
- ✅ App recovers

---

## Visual Quality Checks

### Thumbnail Quality
- [ ] Text is readable in thumbnails (150px width)
- [ ] Images are clear (not pixelated)
- [ ] Colors match original PDF
- [ ] Page number overlay is visible

### Preview Quality
- [ ] High resolution (800px width, 150 DPI)
- [ ] Text is crisp and readable
- [ ] Images are high quality
- [ ] Colors accurate
- [ ] No rendering artifacts

### UI Polish
- [ ] Selection highlighting is obvious
- [ ] Hover effects work on thumbnails
- [ ] Progress bar animates smoothly
- [ ] Status messages update correctly
- [ ] Info panel formatted nicely

---

## Performance Benchmarks

| PDF Size | Expected Load Time | Memory Usage |
|----------|-------------------|--------------|
| 5 pages  | < 3 seconds       | < 100 MB     |
| 20 pages | < 8 seconds       | < 200 MB     |
| 50 pages | < 20 seconds      | < 400 MB     |
| 100 pages| < 40 seconds      | < 800 MB     |

**Note:** Times are approximate and depend on PDF complexity.

---

## Known Limitations (Expected)

These are **NOT bugs** - they are planned for future versions:

- ❌ No zoom controls yet (v0.3.0)
- ❌ No page reordering (v0.4.0)
- ❌ No delete pages (v0.4.0)
- ❌ No save modified PDF (v0.5.0)
- ❌ No undo/redo (v1.0.0)
- ❌ Thumbnails not cached between sessions
- ❌ No keyboard shortcuts yet

---

## Bug Reporting Template

If you find issues during testing, document them like this:

```
**Bug Title:** [Brief description]
**Severity:** Critical / High / Medium / Low
**Steps to Reproduce:**
1. 
2. 
3. 

**Expected Behavior:**

**Actual Behavior:**

**Error Messages:**

**Test Environment:**
- Visual Studio: 2026 (18.9.0)
- .NET: 10.0
- PDF File: [name and size]

**Screenshots/Logs:**
[Attach if available]
```

---

## Success Criteria for v0.2.0 Completion

Version 0.2.0 is considered **complete and ready for release** when:

1. ✅ All 10 manual tests pass
2. ✅ Visual quality meets standards
3. ✅ Performance within benchmarks
4. ✅ No critical or high-severity bugs
5. ✅ Code builds without warnings
6. ✅ Documentation updated
7. ✅ README.md reflects v0.2.0 features

---

## Next Steps After Testing

Once all tests pass:

1. **Update RoadMap.html** - mark v0.2.0 as "Complete" with completion date
2. **Create Git commit** - with message "Release v0.2.0 - Page Thumbnails with MVVM"
3. **Create Git tag** - `v0.2.0`
4. **Update README** - final review of feature list
5. **Plan v0.3.0** - begin designing next phase (page filtering)

---

**Good luck with testing!** 🎉
