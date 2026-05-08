# Implementation Plan: Configurable Naming

**Branch**: `001-configurable-naming` | **Date**: 2026-05-08 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-configurable-naming/spec.md`

## Summary

Add project-local configuration with per-run overrides for the Ollama model, naming convention, and maximum filename stem length. The implementation will keep deterministic preference resolution and name formatting in the application layer, keep CLI parsing and dependency composition in the console layer, keep Ollama/filesystem adapters in infrastructure, and preserve the existing single-file and recursive-directory workflows while applying the requested cleanup refactors.

## Technical Context

**Language/Version**: C# on .NET 10 (`net10.0`)  
**Primary Dependencies**: Existing `Microsoft.Extensions.AI.Abstractions` 10.4.1 and `OllamaSharp` 5.4.25; no package additions, removals, or updates planned  
**Storage**: Project-local JSON configuration file plus existing filesystem image files  
**Testing**: NUnit 4.5.1 via `dotnet test ApplicationTests\ApplicationTests.csproj -v minimal`  
**Target Platform**: .NET console app, currently developed and validated on Windows with local Ollama available for real runs  
**Project Type**: CLI console application with application and infrastructure class libraries  
**Performance Goals**: Configuration resolution and deterministic name formatting complete before the first model call; no measurable slowdown to per-image processing beyond model latency  
**Constraints**: Invalid project-local config blocks the run before overrides or renames; no new third-party packages; external model and filesystem effects remain fakeable in tests  
**Scale/Scope**: Single user, local project configuration, single-file or recursive-directory image rename runs using existing supported image extensions

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Vertical slice plan**: PASS. The feature will be split by user-visible slices: saved project config, per-run overrides, naming conventions, max-length handling, and behavior-preserving refactors. Each slice can include CLI behavior, application logic, infrastructure composition, and tests.
- **Testing approach**: PASS. Tests will exercise real preference resolution, formatting, validation, and rename orchestration. External Ollama calls and filesystem mutation may use fakes or temporary directories through established ports.
- **Dependency currency**: PASS. No package additions, removals, or updates are planned. Existing package versions remain unchanged, so no current package research is required for this plan.
- **Operation composition**: PASS. Name cleanup, convention formatting, max-length enforcement, preference resolution, model selection, and file renaming will be focused operations composed by the application/console flow instead of embedded in adapters.
- **C# standard library first**: PASS. Project-local JSON config will use built-in .NET APIs; no new parser, options, or configuration package is justified for this scope.

## Project Structure

### Documentation (this feature)

```text
specs/001-configurable-naming/
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   `-- cli-contract.md
|-- checklists/
|   `-- requirements.md
`-- tasks.md
```

### Source Code (repository root)

```text
Console/
|-- Program.cs
|-- ImageRenameCli.cs
|-- ImageRenameRunner.cs
`-- Factories/
    `-- OllamaAgentFactory.cs

Application/
|-- ImageRenamer.cs
|-- Models/
|-- Ports/
|-- ImageNamingPreferenceResolver.cs
`-- ImageNameFormatter.cs

Infrastructure/
|-- ForReadingImages/
|-- ForTalkingWithModels/
|-- Transport/
`-- Validation/

ApplicationTests/
|-- ImageRenameCliTests.cs
|-- ImageRenameRunnerTests.cs
|-- ImageRenamerTests.cs
|-- OllamaAgentTests.cs
|-- FileNameValidatorTests.cs
|-- ImageNamingPreferencesTests.cs
|-- ImageRenameOptionsTests.cs
|-- ImageNameFormatterTests.cs
`-- OllamaAgentFactoryTests.cs
```

**Structure Decision**: Keep the existing three-project structure. `Console/` owns CLI parsing, CLI option result types, project-local config discovery, and dependency composition. `Application/` owns app-neutral preference resolution, naming convention formatting, max-length behavior, and orchestration. `Infrastructure/` owns Ollama and filesystem adapters. `ApplicationTests/` owns tests with fakes for external model/filesystem boundaries.

## Complexity Tracking

No constitution violations require justification.

## Phase 0: Research Summary

Research output is captured in [research.md](./research.md). All technical decisions are resolved with no outstanding `NEEDS CLARIFICATION` markers.

## Phase 1: Design Summary

Design artifacts:

- [data-model.md](./data-model.md)
- [contracts/cli-contract.md](./contracts/cli-contract.md)
- [quickstart.md](./quickstart.md)

## Post-Design Constitution Check

- **Vertical slice plan**: PASS. Design artifacts support independent slices and acceptance tests for each user story.
- **Testing approach**: PASS. Data model and CLI contract identify deterministic behavior that can be tested without real Ollama calls or permanent filesystem mutation.
- **Dependency currency**: PASS. No dependency changes introduced by design.
- **Operation composition**: PASS. Design separates preference resolution, name formatting, validation, model naming, and filesystem rename behavior.
- **C# standard library first**: PASS. Design relies on existing platform and project capabilities rather than new packages.
