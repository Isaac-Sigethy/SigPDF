# PDF Editor

## 1. Project Goal

Build a native Windows PDF editor that provides the most useful everyday PDF functionality without requiring Adobe Acrobat.

The application should prioritize:

1. Fast PDF opening
2. Reliable PDF rendering
3. Smooth navigation through large documents
4. Text search
5. Text selection and copying
6. Page manipulation
7. Annotations
8. Text editing
9. Image insertion and manipulation
10. Form filling
11. Merge and split operations
12. Undo/redo
13. Reliable saving
14. Crash recovery
15. Good large-document performance
16. Offline operation for core functionality
17. Maintainable architecture
18. Correctness over feature count

The application is a **desktop PDF editor**, not a web application.

Do not attempt to recreate every feature of Adobe Acrobat. Build a reliable core first and add advanced features only after the core is stable.

---

## 2. Target Platform

Primary platform:

* Windows 10+
* Windows 11

Primary UI framework:

* C#
* .NET 8+
* WinUI 3

The application should be designed as a native Windows desktop application.

Do not use Electron.

Do not build the PDF rendering engine from scratch.

---

## 3. Core Technology

Recommended stack:

### Language

C#

### Runtime

.NET 8+

### UI

WinUI 3

### PDF engine

Use a mature PDF library/engine such as:

* PDFium
* MuPDF
* another mature PDF engine if it provides the required capabilities

The PDF engine must be isolated behind application interfaces.

Do not allow the rest of the application to depend directly on a specific PDF engine wherever avoidable.

### Testing

xUnit

### Benchmarking

BenchmarkDotNet

### Packaging

MSIX or WiX

### Optional database

SQLite may be used for application-level metadata, recovery state, recent documents, or search indexes if necessary.

Do not introduce SQLite merely because it is available.

---

## 4. Repository Structure

The git repository root is the product root. Sources live under `src/` rather than a nested `PdfEditor/` folder.

```text
src/
├── PdfEditor.App/
├── PdfEditor.Core/
├── PdfEditor.Rendering/
├── PdfEditor.Engine/
└── PdfEditor.Tests/

test-data/
├── small/
├── large/
├── text-heavy/
├── image-heavy/
├── scanned/
├── encrypted/
├── forms/
├── annotations/
└── malformed/

benchmarks/
docs/
scripts/
AGENTS.md
README.md
SPEC.md
```

Responsibilities:

### PdfEditor.App

WinUI application.

Contains:

* Windows
* Views
* ViewModels
* Commands
* User interaction
* Application startup/shutdown
* UI state

The UI layer must not contain core PDF business logic.

### PdfEditor.Core

Application-independent domain logic.

Contains:

* Document models
* Editor state
* Commands
* Undo/redo
* Search abstractions
* Annotation models
* Page operations
* Interfaces
* Validation
* Application services

Core should not depend on WinUI.

### PdfEditor.Rendering

Rendering orchestration.

Contains:

* Page rendering
* Render requests
* Tile/page caching
* Visible-page management
* Thumbnail rendering
* Render invalidation

### PdfEditor.Engine

Concrete PDF engine integration.

Contains:

* PDFium/MuPDF adapter
* PDF opening
* PDF saving
* Text extraction
* Page manipulation
* PDF-specific operations

The engine should implement interfaces defined by Core.

### PdfEditor.Tests

Unit and integration tests.

---

## 5. Dependency Direction

Preferred dependency direction:

```text
PdfEditor.App
      |
      v
PdfEditor.Core
      |
      v
PdfEditor.Engine abstractions
      ^
      |
PdfEditor.Engine

PdfEditor.App
      |
      v
PdfEditor.Rendering
      |
      v
PdfEditor.Engine abstractions
```

Rules:

* Core must not depend on WinUI.
* Core must not depend directly on PDFium/MuPDF.
* UI must not directly manipulate the PDF engine.
* Rendering must use engine abstractions.
* PDF engine implementation details must stay inside PdfEditor.Engine.
* Business logic belongs in Core, not in UI code.
* App may reference Engine only as the composition root.

---

## 6. Domain Model

Separate the PDF's actual document model from the application's editing state.

At minimum:

```text
PdfDocument
PdfPage
PdfText
PdfImage
PdfAnnotation
PdfFormField
PdfBookmark
PdfMetadata
```

### PdfDocument

* DocumentId
* FilePath
* PageCount
* Metadata
* IsEncrypted
* IsReadOnly
* IsModified

### PdfPage

* PageNumber
* Width
* Height
* Rotation

Do not duplicate entire PDF pages in managed memory unnecessarily.

Large documents must be handled lazily.

---

## 7. Editor State

Keep editor-specific state separate from the underlying PDF representation.

Example:

```text
EditorDocument
├── Document
├── SelectedPage
├── SelectedObject
├── Zoom
├── ScrollPosition
├── CurrentTool
├── Selection
├── UnsavedChanges
├── UndoStack
└── RedoStack
```

Do not make UI controls the source of truth for document state.

---

## 8. PDF Engine Interface

Create an abstraction similar to `IPdfEngine`.

It should provide operations for:

```text
Open
Close
GetPageCount
GetPage
RenderPage
ExtractText
ExtractMetadata
Save
SaveAs
ModifyPage
CreatePage
DeletePage
MovePage
RotatePage
```

Additional interfaces may be introduced where they represent real boundaries.

Do not create unnecessary interfaces solely for theoretical future features.

M1 defines `IPdfEngine`, `IPdfDocumentSession`, and `IPdfRenderer`. Save, text extraction, and page mutation methods are added when those milestones implement them.

---

## 9. PDF Opening

Opening a document must:

1. Validate that the file exists.
2. Open the PDF through the engine.
3. Detect encryption.
4. Determine whether a password is required.
5. Read basic metadata.
6. Determine page count.
7. Create an application document state.
8. Display the first usable page as quickly as practical.
9. Load additional data lazily.

Do not extract every page's complete content before displaying the document.

---

## 10. Rendering

Rendering must be asynchronous.

Never block the UI thread while rendering a PDF page.

Only render pages that are:

1. Currently visible
2. Immediately adjacent to the visible viewport
3. Explicitly requested

Do not render all pages at full resolution.

---

## 11. Render Cache

Implement a bounded cache.

Example cache key:

```text
DocumentId
PageNumber
Zoom
Rotation
RenderMode
```

The cache should use an LRU-style eviction strategy or another bounded strategy.

* Cache must have a memory limit.
* Old pages must be evicted.
* Changing zoom must not permanently retain every previous render.
* Modifying a page must invalidate its cached rendering.
* Closing a document must release its cached rendering data.

---

## 12. Large Document Support

The application must be designed for 10, 100, 500, and 1000+ page documents.

Requirements:

* Lazy page loading
* Lazy thumbnail rendering
* Bounded render cache
* No full-document bitmap storage
* No unnecessary full-document text loading
* Asynchronous rendering
* Incremental thumbnail loading
* No UI freeze during navigation

---

## 13. Main UI

Initial UI:

```text
┌───────────────────────────────────────────────┐
│ File  Edit  View  ...                         │
├───────────────────────────────────────────────┤
│ Tools / formatting / page controls            │
├─────────────┬─────────────────────────────────┤
│ Thumbnails  │          PDF viewport           │
├─────────────┴─────────────────────────────────┤
│ Page X / Y       Zoom                         │
└───────────────────────────────────────────────┘
```

Keep the initial UI simple.

Do not build a complicated ribbon interface unless there is a demonstrated need.

---

## 14. Tools

Initial tool set:

```text
Select
Hand/Pan
Text
Highlight
Underline
Strikethrough
Draw
Shape
Image
Comment
```

Tools should operate through application commands where they modify document state.

---

## 15. File Operations

Support Open, Save, Save As, Export, and Close.

Preferred save process:

```text
Create temporary file
        ↓
Write PDF
        ↓
Validate temporary file
        ↓
Flush/close
        ↓
Replace original atomically where possible
```

Never leave the original document in a half-written state if a save fails.

Do not silently overwrite the original after an unexpected error.

---

## 16. Page Operations

Required page operations:

* Delete
* Insert
* Duplicate
* Move
* Rotate
* Extract
* Merge
* Split

Page operations must operate independently of the UI.

After page modifications:

* Update page count
* Update thumbnail list
* Invalidate affected rendering
* Update selection
* Mark document as modified
* Add an undoable command

---

## 17. Command System

All meaningful document modifications should use a command system.

```text
IEditorCommand
    Execute()
    Undo()
```

Commands must not depend on specific UI controls.

---

## 18. Undo / Redo

Maintain UndoStack and RedoStack.

```text
New command → Execute → Add to UndoStack → Clear RedoStack
Undo → Undo command → Move command to RedoStack
Redo → Execute again → Move command to UndoStack
```

Keyboard shortcuts: Ctrl+Z = Undo, Ctrl+Y = Redo.

Undo/redo should work for document modifications rather than merely reversing UI state.

---

## 19. Text Extraction

Text extraction must be separate from rendering.

Extracted text should retain positional information when the engine supports it.

This is required for search, text selection, copying, highlighting, and future text editing.

---

## 20. Text Selection

Support click, drag selection, copy, search-result selection, and highlight of selected text.

Use PDF engine coordinate information.

Do not implement text selection using only screen coordinates.

---

## 21. Text Search

Shortcut: Ctrl+F.

Search results should contain PageNumber, MatchingText, and Position.

The UI should provide result count, previous/next, a result list, and a matching text preview.

Clicking a result should navigate to the page, scroll to the match, and highlight the match.

For large documents, avoid repeatedly extracting the same page text.

---

## 22. Text Editing

PDFs are not Word documents.

Do not assume that arbitrary PDF text can be edited like normal document text.

Initial strategy: overlay/replace visual content rather than rewriting PDF content streams.

Do not attempt advanced arbitrary PDF content-stream rewriting until the simpler implementation is stable.

---

## 23. Annotations

Support highlight, underline, strikethrough, freehand, line, rectangle, circle, text box, and sticky note.

Base abstraction: `IAnnotation` with Id, PageNumber, Bounds, Author, CreatedTime, ModifiedTime.

Annotations should be selectable, movable where appropriate, deletable, undoable, and saveable.

---

## 24. Annotation Coordinates

Store annotation positions in document/PDF coordinates rather than screen coordinates.

Conversion must account for zoom, rotation, page dimensions, and scroll position.

---

## 25. Images

Support inserting PNG and JPEG.

Operations: insert, move, resize, rotate, delete.

Do not decode enormous images at unnecessarily high resolution when a smaller representation is sufficient.

Maintain image aspect ratio by default.

Image modifications must be undoable.

---

## 26. Forms

Initial form support: existing text fields, checkboxes, radio buttons, and dropdowns.

Users should be able to select a field, enter values, change checkbox/radio state, select dropdown values, and save the completed PDF.

Do not initially implement a full PDF form designer.

---

## 27. Merge and Split

Merge should preserve page order, handle different page sizes, preserve annotations where supported, preserve metadata appropriately, and not unnecessarily rasterize pages.

Split should allow extracting selected pages or a page range as a new PDF.

These operations must not modify the original unless explicitly requested.

---

## 28. Thumbnails

Show page previews, select page, scroll efficiently, lazy-load thumbnails, cache thumbnails, highlight the selected page, and update after page operations.

Do not render every thumbnail at full resolution.

After a page modification, invalidate only affected thumbnails.

---

## 29. Zoom and Navigation

Support zoom in/out, fit width, fit page, custom zoom, page up/down, next/previous page, mouse wheel, and scroll/pan.

Zoom changes should not cause unnecessary reprocessing of unrelated pages.

---

## 30. Metadata

Support viewing/editing Title, Author, Subject, Keywords, Creator, and Producer.

Metadata editing should be undoable if practical.

---

## 31. Bookmarks

Bookmark support may be added after the core editor is stable.

Do not prioritize bookmarks over core editing functionality.

---

## 32. Encryption and Passwords

The application must correctly detect encrypted PDFs.

If a password is required: prompt once, attempt authentication, open or report failure.

Do not repeatedly prompt without reason.

Incorrect passwords should produce a clear error.

Do not store PDF passwords by default.

---

## 33. Digital Signatures

Digital signatures are a later feature.

Do not implement custom cryptography.

---

## 34. Security

Future security features may include password protection, PDF encryption, permission restrictions, metadata removal, and redaction.

Redaction must eventually remove underlying content, not merely draw a black rectangle over it.

Do not implement custom cryptography.

---

## 35. Crash Recovery

Recovery architecture should be separate from normal saving.

Autosave/recovery data must not silently overwrite the original PDF.

After an unexpected shutdown: detect recovery state, identify the document, ask whether to recover, restore recoverable changes, and preserve the original PDF until the user explicitly saves.

Recovery data should be cleaned up after a successful normal save/close.

---

## 36. External File Changes

If the underlying PDF changes outside the application, detect the change and notify the user.

Possible actions: reload, keep current copy, or save current copy elsewhere.

Never silently overwrite external changes.

---

## 37. Error Handling

Expected error categories:

* File not found
* Permission denied
* Corrupt PDF
* Unsupported PDF
* Password required
* Incorrect password
* Invalid page
* Rendering failure
* Save failure
* Disk full
* File changed externally
* PDF engine failure
* Out-of-memory conditions

One bad page must not crash the entire application.

Do not show a popup for every individual file/page error.

---

## 38. Logging

Use structured logging.

Log application events, PDF open/close failures, rendering failures, save failures, recovery events, engine errors, and useful performance information.

Do not log PDF contents, passwords, sensitive form contents, or unnecessary personal information.

---

## 39. Performance Requirements

The UI must remain responsive during expensive operations.

First visible page should appear as quickly as practical.

Large documents must not cause uncontrolled memory growth.

Do not optimize blindly. Measure first.

---

## 40. Benchmarking

Use BenchmarkDotNet for open, render, thumbnail, text extraction, search, page operations, and save across multiple document sizes.

---

## 41. Testing

Use xUnit.

Unit tests: document model, page operations, commands, undo/redo, search, coordinates, annotation geometry, validation, recovery state, file-change detection.

Integration tests: open, render, extract text, search, annotate, save, reopen, page operations, forms, merge, split.

---

## 42. Regression PDF Test Set

Maintain representative PDFs under `test-data/` as listed in section 4.

Do not rely exclusively on one sample PDF.

---

## 43. OCR

OCR is a later optional feature, integrated through an abstraction such as `IOcrProvider`.

OCR should not be required for normal text-based PDFs.

---

## 44. AI

AI must be optional.

The core PDF editor must work without an AI provider.

Do not send document contents to an external AI provider without explicit user action.

---

## 45. AI Privacy

If an AI feature sends document data externally:

* Clearly tell the user what is being sent.
* Do not silently upload the PDF.
* Do not upload the entire document when only selected text is required.
* Allow local/offline AI where practical.

---

## 46. Autosave vs Normal Save

These must be separate concepts.

Normal save: the user explicitly saves the PDF.

Recovery/autosave: temporary recovery information used after a crash.

Recovery files should not be treated as user-created PDFs.

---

## 47. File Integrity

After saving:

1. Ensure the file was successfully written.
2. Ensure the resulting PDF can be reopened.
3. Only then consider the save successful.

If validation fails: keep the original, report failure, and preserve recovery data where possible.

---

## 48. Coordinate System

PDF editing must use document-space coordinates.

```text
PDF coordinates
      ↓
Page transformation
      ↓
Zoom
      ↓
Viewport
      ↓
Screen coordinates
```

Centralize coordinate conversion.

Do not implement separate ad-hoc coordinate calculations throughout the UI.

---

## 49. State Management

The application should have one authoritative document/editor state.

```text
EditorDocument
      |
      +-- UI observes state
      +-- Commands modify state
      +-- Engine persists state
```

The UI should reflect state rather than independently becoming the source of truth.

---

## 50. Threading

Expensive operations must be asynchronous.

UI thread should primarily handle input, rendering presentation, and UI state updates.

Do not use async merely for appearance. Ensure cancellation and lifetime management are correct.

---

## 51. Cancellation

Long-running operations should support cancellation where practical.

If the user closes a document while background work is active: cancel the work, release resources, and prevent callbacks from modifying the closed document.

---

## 52. Resource Lifetime

PDF documents and engine resources must be disposed correctly.

Pay particular attention to native PDF handles, bitmaps, render buffers, file handles, temporary files, and cache entries.

---

## 53. Keyboard Shortcuts

Initial shortcuts:

```text
Ctrl+O       Open
Ctrl+S       Save
Ctrl+Shift+S Save As
Ctrl+F       Search
Ctrl+Z       Undo
Ctrl+Y       Redo
Ctrl+C       Copy
Ctrl+V       Paste
Delete       Delete selected object/page where appropriate
Escape       Cancel current operation
```

Navigation: Page Up, Page Down, Home, End, arrow keys.

Shortcuts should be implemented through centralized commands rather than hardcoded independently in individual controls.

---

## 54. Accessibility

Where practical: keyboard navigation, accessible control names, logical tab order, sufficient visual distinction, and screen-reader-compatible controls.

Avoid mouse-only workflows for core functions.

---

## 55. Settings

Initial settings: default zoom, theme, thumbnail size, recent documents, autosave/recovery interval, rendering/cache limits.

Settings must not become a dumping ground for internal implementation details.

---

## 56. Recent Documents

Store file path and last opened time.

Do not store document contents.

Handle missing files gracefully.

---

## 57. Application Startup

Startup should:

1. Initialize application services.
2. Initialize PDF engine.
3. Load settings.
4. Load recent documents.
5. Display main window.
6. Avoid unnecessary blocking work.

Do not scan the user's entire computer on startup.

---

## 58. Packaging

The final application should be installable through a normal Windows installation process.

The application should not require developer tools on the user's computer.

---

## 59. Telemetry

Do not add telemetry unless explicitly required.

Core application functionality should work without telemetry.

---

## 60. Features Explicitly Deferred

Do not implement these early unless a milestone specifically calls for them:

* Advanced native PDF text rewriting
* Full PDF form designer
* Digital signatures
* Advanced OCR
* Redaction
* PDF compression
* PDF comparison
* Headers/footers
* Watermarks
* Advanced bookmarks
* Batch processing
* PDF-to-Word conversion
* PDF-to-HTML conversion
* Advanced table extraction
* Plugin system
* Cloud document storage
* Complex AI workflows
* Collaboration
* Browser version

---

## 61. Milestones

Implement milestones sequentially.

Do not implement the entire project in one pass.

Each milestone must leave the repository buildable, testable, internally consistent, and without known compilation errors.

### M1 - Project Architecture

Implement:

* Solution
* Projects
* Dependency structure
* Basic interfaces
* Basic document models
* Application startup
* Test project
* Basic README
* Basic logging infrastructure

Verify:

```text
dotnet build
dotnet test
```

### M2 - PDF Engine Integration

Implement PDF engine abstraction, concrete engine adapter, open, close, page count, basic metadata, and basic error handling.

Tests: open valid PDF, open invalid PDF, page count, close/dispose.

### M3 - Basic Rendering

Render page, render abstraction, basic bitmap pipeline, async rendering.

### M4 - Document Viewport

Main PDF viewport, page navigation, scroll, zoom, fit page, fit width.

### M5 - Thumbnails

Thumbnail sidebar, lazy thumbnail rendering, thumbnail cache, page selection.

### M6 - Saving

Save, Save As, temporary-file save strategy, save validation, modified state.

### M7 - Text Extraction

Text extraction abstraction, page text extraction, position information, caching where appropriate.

### M8 - Search

Ctrl+F, search results, result navigation, search highlighting, cached/lazy text index.

### M9 - Page Operations

Delete, insert, duplicate, move, rotate, extract.

### M10 - Command System

IEditorCommand, undo/redo stacks, core page-operation commands, keyboard shortcuts.

### M11 - Annotations

Highlight, underline, strikethrough, freehand, line, rectangle, circle, text box, sticky note.

### M12 - Text Editing

Initial overlay/replacement strategy.

### M13 - Image Editing

Insert, move, resize, rotate, delete, undo/redo.

### M14 - Forms

Existing text fields, checkboxes, radio buttons, dropdowns, save filled forms.

### M15 - Merge and Split

Merge PDFs, split/extract ranges, preserve page properties, save output separately.

### M16 - Recovery

Recovery state, autosave/recovery, crash detection, recovery prompt, recovery cleanup.

### M17 - Security

Password-protected PDFs, encryption, permission handling, metadata removal. No custom cryptography.

### M18 - OCR

Optional OCR abstraction and provider.

### M19 - AI

Optional IAiProvider. Must remain optional.

### M20 - Performance and Polish

Benchmarking, memory profiling, large-document testing, resource leak testing, UI polish, accessibility.

### M21 - Packaging

Windows installer, icon, versioning, install/uninstall, release configuration.

---

## 62. Definition of Done

A feature is not complete merely because the code compiles.

For each feature:

1. Implement the feature.
2. Integrate it into the correct architecture.
3. Add or update tests.
4. Build the solution.
5. Run relevant tests.
6. Fix compilation errors.
7. Fix test failures.
8. Verify UI behavior where applicable.
9. Verify error handling.
10. Check that the feature does not break previous functionality.

---

## 63. Agent Development Rules

Before making changes:

1. Inspect the repository.
2. Read `SPEC.md`.
3. Read `AGENTS.md`.
4. Inspect the existing project structure.
5. Determine which milestone is currently being implemented.
6. Identify relevant existing abstractions.
7. Identify tests that should be added or changed.

Do not blindly create new architecture if an existing abstraction already solves the problem.

---

## 64. Implementation Rules

Always prefer small coherent changes, existing abstractions, testable code, clear ownership, async operations for expensive work, explicit error handling, measurable performance, and simple implementations before advanced ones.

Avoid large unrelated refactors, duplicate abstractions, UI business logic, blocking the UI thread, unjustified global mutable state, premature optimization, premature features, speculative architecture, custom cryptography, custom PDF rendering, fake implementations for required functionality, and silently ignoring errors.

---

## 65. Dependency Rules

Before adding a dependency:

1. Determine whether the existing stack can solve the problem.
2. Check whether the dependency is mature and maintained.
3. Check licensing compatibility.
4. Determine whether it works on Windows/.NET.
5. Determine whether it adds significant deployment complexity.

Do not add dependencies simply for convenience.

---

## 66. Testing Rules for Agents

After modifying code:

```text
dotnet build
dotnet test
```

If the build fails, identify the actual cause, fix it, and rebuild.

If tests fail, determine whether the implementation or test is incorrect, fix the underlying issue, rerun the relevant tests, then run the full suite.

Never delete or disable a test merely to make the build pass.

---

## 67. Ambiguity Rules

When the specification and existing code disagree:

1. Inspect the existing architecture.
2. Determine whether the specification explicitly requires a behavior.
3. Preserve existing working behavior when possible.
4. Make the smallest coherent architectural change.
5. Do not rewrite large sections merely for stylistic consistency.

If a requirement is genuinely ambiguous, choose the simplest implementation consistent with the rest of the architecture and document the decision if it affects future development.

---

## 68. Milestone Execution Rules

Only implement the milestone explicitly requested.

Implement only prerequisites that are genuinely required.

After completing the milestone: build, test, fix, report, then stop.

---

## 69. Final Agent Report

After each milestone, report implemented work, files changed, tests added/updated, build result, test result (passed/failed counts), known issues, and the next milestone.

Do not claim a feature is complete if it is only stubbed.

---

## 70. Core Architectural Principle

```text
                    ┌──────────────────┐
                    │   WinUI App      │
                    │ Views/ViewModels │
                    └────────┬─────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │      Core        │
                    │ Document State   │
                    │ Commands         │
                    │ Undo/Redo        │
                    │ Models           │
                    │ Services         │
                    └───────┬──────────┘
                            │
                ┌───────────┴───────────┐
                ▼                       ▼
       ┌────────────────┐      ┌────────────────┐
       │ PDF Engine     │      │ Rendering      │
       └────────────────┘      └────────────────┘
```

---

## 71. Most Important Priorities

If tradeoffs are necessary, prioritize in this order:

1. Correctness
2. Data safety
3. PDF compatibility
4. UI responsiveness
5. Large-document performance
6. Maintainability
7. Testability
8. Feature count

A smaller PDF editor that reliably opens, edits, and saves documents is better than a feature-heavy application that corrupts PDFs or loses user work.

---

## 72. Explicit Non-Goals

The project is not intended to:

* Replace professional publishing software
* Be a full Word processor
* Implement a PDF renderer from scratch
* Become a cloud document platform
* Require AI to function
* Collect user documents
* Require an online account for core editing
* Implement every Acrobat feature before release

The application should be a fast, reliable, privacy-conscious native PDF editor for everyday use.
