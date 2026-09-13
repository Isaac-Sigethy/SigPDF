# Architecture notes

## Repository layout

The workspace is already the product repository, so sources live at `src/` instead of a nested `PdfEditor/` directory. Project names still use the `PdfEditor.*` prefix from `SPEC.md`.

## M1 decisions

- **Engine adapter is not implemented yet.** `IPdfEngine` and `IPdfDocumentSession` live in Core. `PdfEditor.Engine` exists so the dependency direction is in place. Opening files is M2.
- **Renderer interface is defined, pipeline is not.** `IPdfRenderer` and render request types live in Core. `VisiblePageCalculator` lives in Rendering because visible-page selection is rendering orchestration and is testable without a PDF engine.
- **No extra engine interfaces.** `IPdfTextExtractor`, `IPdfPageEditor`, `IPdfFormHandler`, `IPdfAnnotationHandler`, `IOcrProvider`, and `IAiProvider` are deferred until those milestones.
- **Logging** uses `Microsoft.Extensions.Logging` plus a small JSON-lines file provider. Passwords and other sensitive keys are redacted. File paths in logs have the user profile prefix replaced.
- **No SQLite.** Settings and recent documents are JSON files under the application data directory.
- **No telemetry.**
- **WinUI 3 app is unpackaged** during development (`WindowsPackageType=None`). Packaging is M21.
- **`EnableMsixTooling` is true** even for the unpackaged app so `dotnet build` uses Windows App SDK PRI tasks instead of Visual Studio Appx tasks.
- **App may reference Engine** only as the composition root. View code must not use engine types.

## Coordinate space

Document geometry uses PDF user space (origin bottom-left). `CoordinateTransform` converts between PDF, view (top-left, page-rotated), and screen (view minus scroll). UI code should use this type instead of ad-hoc math.
