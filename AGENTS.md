# Agent instructions

This repository is a native Windows PDF editor. Read this file and `SPEC.md` before changing code.

## Before changing code

1. Inspect the repository.
2. Read `SPEC.md`.
3. Read this file.
4. Inspect the existing project structure.
5. Determine which milestone is currently being implemented.
6. Identify relevant existing abstractions.
7. Identify tests that should be added or changed.

Do not create new architecture when an existing abstraction already solves the problem.

## Current milestone

See `docs/milestones.md`. Implement only the milestone that was explicitly requested.

Do not implement later milestone features just because they appear in `SPEC.md`.

## Repository layout

The git repository root **is** the product root. Sources live under `src/` rather than a nested `PdfEditor/` folder.

```text
src/PdfEditor.App          WinUI 3 UI (no PDF business logic)
src/PdfEditor.Core         Domain models, editor state, interfaces, services
src/PdfEditor.Rendering    Render orchestration (uses Core engine abstractions)
src/PdfEditor.Engine       Concrete PDF engine adapter
src/PdfEditor.Tests        xUnit tests
```

## Dependency rules

- Core must not depend on WinUI or a concrete PDF engine.
- Rendering must use Core engine abstractions, not `PdfEditor.Engine` types.
- UI must not call the PDF engine directly. The application composition root may register the engine.
- Engine implementation details stay inside `PdfEditor.Engine`.
- Do not add dependencies for convenience. Prefer the existing stack.

## Implementation rules

Prefer small coherent changes, existing abstractions, testable code, async work for expensive operations, and explicit error handling.

Avoid UI business logic, blocking the UI thread, global mutable state, premature features, custom cryptography, custom PDF rendering, fake implementations of required functionality, and silently ignoring errors.

## Testing rules

After modifying code:

```text
dotnet build
dotnet test
```

If the build or tests fail, fix the cause. Never delete or disable a test to make the suite pass.

## Milestone report

After completing a requested milestone, report implemented work, files changed, tests, build/test results, known issues, and the next milestone. Then stop.

Do not claim a feature is complete if it is only stubbed.
