# Implementation Plan: Codebase Refactor Audit

**Branch**: `001-codebase-refactor-audit` | **Date**: 2026-07-08 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-codebase-refactor-audit/spec.md`

## Summary

Produce a repo-wide maintainability audit for ImageNamer that identifies obvious readability improvements, possible dead code, duplicated patterns, modernization opportunities, and test-maintainability improvements. The implementation approach is documentation-first: inspect the current CLI, application, infrastructure, and test projects; record findings with evidence, risk, behavior impact, and validation expectations; then use the audit to drive later small refactor tasks.

## Technical Context

**Language/Version**: C# on .NET 10.0

**Primary Dependencies**: NUnit test stack, OllamaSharp for local model integration, .NET SDK tooling

**Storage**: Local filesystem inputs and in-place file renames; feature artifacts stored as Markdown in `specs/001-codebase-refactor-audit/`

**Testing**: `dotnet build ImageNamer.slnx -v minimal`, `dotnet test ApplicationTests/ApplicationTests.csproj -v minimal`, `dotnet test E2ETests/E2ETests.csproj -v minimal`

**Target Platform**: Local command-line application on developer workstations

**Project Type**: CLI application with application and infrastructure class library boundaries

**Performance Goals**: Audit output should let a maintainer choose the next cleanup task in under 10 minutes

**Constraints**: Preserve existing CLI behavior, file-handling behavior, model-provider behavior, and deterministic test isolation; avoid broad formatting churn and unrelated dependency changes

**Scale/Scope**: Current repository projects: `Console`, `Application`, `Infrastructure`, `ApplicationTests`, `E2ETests`, `TestsShared`, and `TestData`

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Maintainable Architecture**: PASS. The feature evaluates existing boundaries and does not require production code changes during planning.
- **Testable Behavior**: PASS. The plan defines review validation for the audit artifact and build/test gates for later behavior-preserving refactors.
- **Readable Code**: PASS. The audit is explicitly focused on names, direct control flow, focused methods, and avoiding unnecessary abstractions.
- **Explicit Quality Gates**: PASS. Build and test commands are named in Technical Context and quickstart validation.
- **Minimal, Reviewable Change**: PASS. Planning outputs are documentation-only; later recommendations must be split into small follow-up tasks.
- **Vertical Slice Delivery**: PASS. Later task generation should treat each audit category or high-value proposal as an independent slice from observable behavior through validation.

## Project Structure

### Documentation (this feature)

```text
specs/001-codebase-refactor-audit/
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   `-- audit-report.md
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
|-- Transport/
`-- Validation/

ApplicationTests/
E2ETests/
TestsShared/
TestData/
```

**Structure Decision**: The feature does not introduce new production structure. Audit work should inspect the existing project boundaries and record findings in `specs/001-codebase-refactor-audit/`.

## Complexity Tracking

No constitution violations requiring justification.

## Phase 0: Research

Research output is captured in [research.md](research.md). Decisions cover the audit format, dead-code confidence model, duplication review approach, modernization boundaries, and validation strategy.

## Phase 1: Design & Contracts

Design output is captured in:

- [data-model.md](data-model.md)
- [contracts/audit-report.md](contracts/audit-report.md)
- [quickstart.md](quickstart.md)

Agent context update: no agent update script is present in `.specify/scripts/powershell/`; no context update was run.

## Post-Design Constitution Check

- **Maintainable Architecture**: PASS. The contract requires every finding to identify affected boundaries and behavior impact.
- **Testable Behavior**: PASS. The data model includes validation expectations, and the quickstart names review, build, and test validation.
- **Readable Code**: PASS. The audit categories emphasize readability and avoid abstraction for its own sake.
- **Explicit Quality Gates**: PASS. Quickstart lists exact validation commands and expected outcomes.
- **Minimal, Reviewable Change**: PASS. Contract requires small proposed next actions and risk labels.
- **Vertical Slice Delivery**: PASS. Follow-up work can be generated from prioritized proposals as independently validated slices.
