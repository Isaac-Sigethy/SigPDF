# PDF Editor

A native Windows PDF editor for everyday work: open, view, search, annotate, edit pages, and save reliably. It is a desktop WinUI 3 application, not a web or Electron app.

The project prefers a correct, maintainable core over a large feature list. See `SPEC.md` for the full product specification.

## Current status

Milestone **M1 – Project architecture** is complete.

The application shell starts, logging and settings load, and the domain model is in place. PDF opening and rendering are not implemented yet (M2 and M3).

## Requirements

- Windows 10 or Windows 11
- .NET 8 SDK
- Windows App SDK (restored as a NuGet package for the UI project)

## Build and test

```powershell
dotnet build
dotnet test
```

Or from `scripts/`:

```powershell
./scripts/build.ps1
```

## Repository layout

```text
src/PdfEditor.App          WinUI 3 application
src/PdfEditor.Core         Domain model, editor state, engine interfaces
src/PdfEditor.Rendering    Render orchestration
src/PdfEditor.Engine       Concrete PDF engine (adapter in M2)
src/PdfEditor.Tests        xUnit tests
test-data/                 Regression PDFs by category
benchmarks/                BenchmarkDotNet projects (later)
docs/                      Architecture and milestone notes
```

## Design rules

- Core does not depend on WinUI or a specific PDF library.
- The UI does not contain PDF business logic.
- Rendering talks to engine abstractions, not engine implementation types.
- Large documents are loaded lazily. Pages are not fully extracted on open.
- Saving and crash recovery are separate. Recovery must never silently overwrite the original file.

## License

Licensing for the application and third-party PDF engine will be recorded before distribution.
