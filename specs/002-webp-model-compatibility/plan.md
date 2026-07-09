# Implementation Plan: WEBP Model Compatibility

**Branch**: `002-webp-model-compatibility` | **Date**: 2026-07-09 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/002-webp-model-compatibility/spec.md`

## Summary

Add WEBP compatibility to the model submission path by converting WEBP image bytes to PNG in memory before they are sent to Ollama, while preserving the original `.webp` file extension and the existing rename flow on disk. The implementation should stay inside the infrastructure boundary, use the current stable SkiaSharp package for decode/encode, and expand automated coverage for successful conversion, invalid WEBP failure, and mixed batch behavior.

## Technical Context

**Language/Version**: C# on .NET 10.0

**Primary Dependencies**: Existing `Microsoft.Extensions.AI.Abstractions` and `OllamaSharp`; add `SkiaSharp` 4.150.0 for in-memory image decode/encode

**Storage**: Local filesystem reads and in-place file renames; temporary converted PNG bytes exist only in memory during model submission preparation

**Testing**: NUnit via `dotnet build ImageNamer.slnx -v minimal`, `dotnet test ApplicationTests/ApplicationTests.csproj -v minimal`, and `dotnet test E2ETests/E2ETests.csproj -v minimal`

**Target Platform**: Local command-line application on developer workstations; current repository targets `net10.0`

**Project Type**: CLI application with `Console`, `Application`, and `Infrastructure` project boundaries

**Performance Goals**: WEBP conversion must complete inline within the existing rename flow without introducing user-visible extra steps or additional files on disk

**Constraints**: Preserve current CLI options and output shape, keep rename semantics tied to the original file extension, avoid unrelated refactors, keep tests deterministic, and dispose native image resources correctly

**Scale/Scope**: Single feature slice affecting `Infrastructure/ForReadingImages`, existing application flow, and the related application and end-to-end tests

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Maintainable Architecture**: PASS. The plan keeps conversion logic in infrastructure and does not push image codec concerns into the CLI or application layers.
- **Testable Behavior**: PASS. The plan names unit and end-to-end coverage for success, failure, and batch continuation behavior.
- **Readable Code**: PASS. The change can be implemented as focused image-preparation logic without introducing broad abstractions.
- **Explicit Quality Gates**: PASS. Build and test commands are defined in Technical Context and quickstart validation.
- **Minimal, Reviewable Change**: PASS. Scope is limited to WEBP model compatibility plus the dependency required to implement it.
- **Vertical Slice Delivery**: PASS. The work is planned as one user-visible slice from CLI entry through file read, model submission preparation, and rename validation.

## Project Structure

### Documentation (this feature)

```text
specs/002-webp-model-compatibility/
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   `-- cli-webp-rename-flow.md
|-- checklists/
|   `-- requirements.md
`-- spec.md
```

### Source Code (repository root)

```text
Application/
|-- Models/
|-- Ports/
`-- *.cs

Console/
|-- Factories/
`-- *.cs

Infrastructure/
|-- ForReadingImages/
|-- ForTalkingWithModels/
`-- Transport/

ApplicationTests/
E2ETests/
TestsShared/
TestData/
```

**Structure Decision**: Keep the feature within the existing architecture. `FileOperator` remains the outer file-reading boundary and becomes responsible for preparing model-ready image content. The application layer continues to orchestrate rename behavior without learning about image codec conversion.

## Complexity Tracking

No constitution violations requiring justification.

## Phase 0: Research

Research output is captured in [research.md](research.md). Decisions cover the stable SkiaSharp version to adopt, the official decode/encode conversion path, architecture placement, failure behavior, and dependency footprint considerations.

## Phase 1: Design & Contracts

Design output is captured in:

- [data-model.md](data-model.md)
- [contracts/cli-webp-rename-flow.md](contracts/cli-webp-rename-flow.md)
- [quickstart.md](quickstart.md)

Agent context update: no agent update script is present in `.specify/scripts/powershell/`; no context update was run.

## Post-Design Constitution Check

- **Maintainable Architecture**: PASS. The data model distinguishes source-file identity from model-submission content, preserving current application boundaries.
- **Testable Behavior**: PASS. The contract and quickstart require deterministic checks for converted WEBP requests, preserved extensions, invalid WEBP failures, and mixed-batch continuation.
- **Readable Code**: PASS. The design favors a narrow helper-level change in infrastructure rather than a new cross-layer abstraction.
- **Explicit Quality Gates**: PASS. Quickstart lists the exact build and test commands needed before completion.
- **Minimal, Reviewable Change**: PASS. The only new dependency is the image codec library needed for this feature, and the contract excludes unrelated transport or CLI redesign.
- **Vertical Slice Delivery**: PASS. Implementation can proceed as a single observable slice: read image, prepare model content, call model, preserve rename semantics, and validate end to end.
