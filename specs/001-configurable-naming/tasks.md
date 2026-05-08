# Tasks: Configurable Naming

**Input**: Design documents from `/specs/001-configurable-naming/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/cli-contract.md, quickstart.md

**Tests**: Each user story includes test tasks that exercise real application logic through inputs and outputs. Use fakes only for external boundaries such as model calls and filesystem mutation.

**Organization**: Tasks are grouped by user story as vertical slices to enable independent implementation and testing.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish shared files and naming used by later vertical slices.

- [ ] T001 Add shared temporary project-local config test helper for `imagenamer.json` scenarios in ApplicationTests/ImageNamingConfigTestHelpers.cs
- [ ] T002 [P] Add empty test file for naming preferences in ApplicationTests/ImageNamingPreferencesTests.cs
- [ ] T003 [P] Add empty test file for CLI option parsing in ApplicationTests/ImageRenameOptionsTests.cs
- [ ] T004 [P] Add empty test file for deterministic formatting in ApplicationTests/ImageNameFormatterTests.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core entities and interfaces that all user stories depend on.

**CRITICAL**: No user story work can begin until this phase is complete.

- [ ] T005 Define NamingConvention enum and ImageNamingPreferences record with built-in defaults in Application/Models/ImageNamingPreferences.cs
- [ ] T006 [P] Define ProjectLocalConfig, RunOverrides, and PreferenceSource models in Application/Models/ImageNamingPreferenceSources.cs
- [ ] T007 [P] Define IForResolvingImageNamingPreferences interface in Application/Ports/Driven/IForResolvingImageNamingPreferences.cs
- [ ] T008 [P] Define IForFormattingImageName interface in Application/Ports/Driven/IForFormattingImageName.cs
- [ ] T009 [P] Define ImageRenameOptions and CLI parsing result types in Console/ImageRenameOptions.cs
- [ ] T010 Verify SDK-style test project includes new ApplicationTests/*.cs files without ApplicationTests/ApplicationTests.csproj edits

**Checkpoint**: Foundation ready; user story implementation can now begin.

---

## Phase 3: User Story 1 - Configure Naming Defaults (Priority: P1) MVP

**Goal**: Users can place valid project-local config beside the run and have model, naming convention, and max length defaults applied without per-run flags.

**Independent Test**: Create valid, missing, malformed, and invalid project-local config inputs; verify effective preferences and pre-rename failure behavior without a real model call or permanent filesystem mutation.

### Tests for User Story 1

- [ ] T011 [P] [US1] Add preference-resolution tests for built-in defaults, valid project-local config, malformed config, and invalid config in ApplicationTests/ImageNamingPreferencesTests.cs
- [ ] T012 [P] [US1] Add CLI contract tests for missing config using defaults and invalid project-local config blocking before rename in ApplicationTests/ImageRenameCliTests.cs
- [ ] T013 [P] [US1] Add Ollama factory test proving configured model name is passed into client construction seam in ApplicationTests/OllamaAgentFactoryTests.cs

### Implementation for User Story 1

- [ ] T014 [US1] Implement project-local JSON config loading and validation in Console/ImageRenameConfigurationLoader.cs
- [ ] T015 [US1] Implement deterministic preference resolution with precedence defaults then project-local config in Application/ImageNamingPreferenceResolver.cs
- [ ] T016 [US1] Update ImageRenameCli to load project-local config before constructing the runner in Console/ImageRenameCli.cs
- [ ] T017 [US1] Update OllamaAgentFactory to accept resolved model name and default to gemma4:e2b in Console/Factories/OllamaAgentFactory.cs
- [ ] T018 [US1] Add user-facing config error handling before rename execution in Console/ImageRenameCli.cs
- [ ] T019 [US1] Document project-local config defaults and examples in README.md

**Checkpoint**: User Story 1 is independently testable as the MVP.

---

## Phase 4: User Story 2 - Override Preferences Per Run (Priority: P2)

**Goal**: Users can override model, naming convention, max length, and config path for a single run without modifying saved project-local config.

**Independent Test**: Set saved defaults, supply overrides, and verify override values win for that run only; verify invalid overrides fail before any rename.

### Tests for User Story 2

- [ ] T020 [P] [US2] Add CLI option parsing tests for --model, --naming, --max-length, --config, unknown options, and invalid values in ApplicationTests/ImageRenameOptionsTests.cs
- [ ] T021 [P] [US2] Add preference-resolution tests proving run overrides win over valid project-local config and do not rescue invalid project-local config in ApplicationTests/ImageNamingPreferencesTests.cs
- [ ] T022 [P] [US2] Add CLI integration-style tests proving invalid override exits before runner construction in ApplicationTests/ImageRenameCliTests.cs

### Implementation for User Story 2

- [ ] T023 [US2] Implement manual CLI option parsing for input path and overrides in Console/ImageRenameOptionsParser.cs
- [ ] T024 [US2] Update ImageRenameCli to use parsed input path, override values, and optional --config path in Console/ImageRenameCli.cs
- [ ] T025 [US2] Extend ImageNamingPreferenceResolver to apply valid run overrides after config in Application/ImageNamingPreferenceResolver.cs
- [ ] T026 [US2] Update CLI usage text with --model, --naming, --max-length, --config, supported naming values, and defaults in Console/ImageRenameCli.cs
- [ ] T027 [US2] Update README examples for per-run overrides in README.md

**Checkpoint**: User Stories 1 and 2 both work independently.

---

## Phase 5: User Story 3 - Choose Naming Conventions (Priority: P3)

**Goal**: Users can select normal, snake, capitalized, pascal, or kebab naming and get predictable filename stems.

**Independent Test**: Supply the same generated name to the formatter for each convention and verify the exact final filename stem while preserving extension.

### Tests for User Story 3

- [ ] T028 [P] [US3] Add formatter tests for normal preserving valid punctuation and casing while removing invalid filename characters and duplicate extension text in ApplicationTests/ImageNameFormatterTests.cs
- [ ] T029 [US3] Add formatter tests for snake, capitalized, pascal, and kebab examples from the spec in ApplicationTests/ImageNameFormatterTests.cs
- [ ] T030 [P] [US3] Add ImageRenamer test proving model output is formatted before file rename in ApplicationTests/ImageRenamerTests.cs

### Implementation for User Story 3

- [ ] T031 [US3] Implement ImageNameFormatter with normal, snake, capitalized, pascal, and kebab conventions in Application/ImageNameFormatter.cs
- [ ] T032 [US3] Update ImageRenamer constructor to accept IForFormattingImageName and ImageNamingPreferences in Application/ImageRenamer.cs
- [ ] T033 [US3] Update ImageRenamer to format model-provided names before calling the file rename port in Application/ImageRenamer.cs
- [ ] T034 [US3] Update FileNameValidator to validate final filename characters for all supported conventions in Infrastructure/Validation/FileNameValidator.cs
- [ ] T035 [US3] Compose ImageNameFormatter and resolved preferences in ImageRenameCli in Console/ImageRenameCli.cs

**Checkpoint**: User Stories 1, 2, and 3 are independently functional.

---

## Phase 6: User Story 4 - Limit Name Length (Priority: P4)

**Goal**: Users can set maximum filename stem length and get readable shortened names that preserve extensions.

**Independent Test**: Configure a max length, provide overlong generated names, and verify trailing words are removed first and a single overlong word is hard-truncated.

### Tests for User Story 4

- [ ] T036 [P] [US4] Add formatter tests for word-boundary shortening, single-word hard truncation, extension preservation, and too-short max length in ApplicationTests/ImageNameFormatterTests.cs
- [ ] T037 [P] [US4] Add preference validation tests for minimum acceptable max length in ApplicationTests/ImageNamingPreferencesTests.cs

### Implementation for User Story 4

- [ ] T038 [US4] Add max-length enforcement to ImageNameFormatter using trailing-word removal before hard truncation in Application/ImageNameFormatter.cs
- [ ] T039 [US4] Add minimum max-length validation to ImageNamingPreferenceResolver in Application/ImageNamingPreferenceResolver.cs
- [ ] T040 [US4] Update CLI error messages for too-short max length in Console/ImageRenameCli.cs
- [ ] T041 [US4] Update README max-length behavior documentation in README.md

**Checkpoint**: User Story 4 works independently with deterministic tests.

---

## Phase 7: User Story 5 - Preserve Behavior During Cleanup (Priority: P5)

**Goal**: Maintain existing rename behavior while removing misleading entry points and moving naming responsibility out of filesystem infrastructure.

**Independent Test**: Run existing single-file, directory, skip, failure-continuation, and collision tests plus new configuration tests; confirm behavior is preserved.

### Tests for User Story 5

- [ ] T042 [P] [US5] Update FileOpperator tests to assert exact requested final name is used without forced lowercasing in ApplicationTests/FileOpperatorTests.cs
- [ ] T043 [P] [US5] Add regression tests for directory recursion, unsupported-file skipping, failure continuation, and collision handling after formatter integration in ApplicationTests/ImageRenameRunnerTests.cs
- [ ] T044 [P] [US5] Update Ollama transport tests to allow arbitrary system instructions from the caller in ApplicationTests/OllamaChatTransportTests.cs

### Implementation for User Story 5

- [ ] T045 [US5] Remove forced lowercasing and keep collision handling in Infrastructure/ForReadingImages/FileOpperator.cs
- [ ] T046 [US5] Rename FileOpperator to FileOperator across Infrastructure/ForReadingImages/FileOperator.cs, Infrastructure/ForReadingImages/StubFileOpperator.cs, ApplicationTests/FileOpperatorTests.cs, and Console/ImageRenameCli.cs
- [ ] T047 [US5] Remove unused AppFromDrivenSide and IImageNamerApp files from Application/AppFromDrivenSide.cs and Application/Ports/IImageNamerApp.cs
- [ ] T048 [US5] Remove OllamaAgent.Instructions equality guard from Infrastructure/Transport/OllamaChatTransport.cs
- [ ] T049 [US5] Update OllamaAgent tests for post-formatting responsibilities and retained punctuation retry behavior in ApplicationTests/OllamaAgentTests.cs
- [ ] T050 [US5] Run full test suite with dotnet test ApplicationTests/ApplicationTests.csproj -v minimal

**Checkpoint**: Cleanup preserves existing workflows and all tests pass.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Documentation, validation, and consistency across all stories.

- [ ] T051 [P] Update quickstart validation notes after implementation in specs/001-configurable-naming/quickstart.md
- [ ] T052 [P] Verify CLI contract examples use `imagenamer.json` as the default project-local config path in specs/001-configurable-naming/contracts/cli-contract.md
- [ ] T053 Review README and help text for consistent naming labels in README.md and Console/ImageRenameCli.cs
- [ ] T054 Run full validation using dotnet test ApplicationTests/ApplicationTests.csproj -v minimal

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies.
- **Foundational (Phase 2)**: Depends on setup and blocks all user stories.
- **User Stories (Phase 3+)**: Depend on foundational tasks. They are ordered by priority for incremental delivery.
- **Polish**: Depends on all desired user stories being complete.

### User Story Dependencies

- **US1 Configure Naming Defaults**: Starts after foundation; MVP scope.
- **US2 Override Preferences Per Run**: Depends on US1 preference models and resolver.
- **US3 Choose Naming Conventions**: Depends on resolved preferences from US1/US2.
- **US4 Limit Name Length**: Depends on formatter from US3.
- **US5 Preserve Behavior During Cleanup**: Best after US3/US4 so regression tests cover the final naming pipeline.

### Within Each User Story

- Tests first, expected to fail before implementation.
- CLI contract or behavior before application changes where user-facing behavior is involved.
- Application behavior before infrastructure adapters.
- Composition after application and infrastructure changes.
- Story checkpoint validates the independent increment.

## Parallel Opportunities

- Setup test-file tasks T002-T004 can run in parallel.
- Foundational model/interface/CLI result-type tasks T006-T009 can run in parallel after T005 is understood.
- Test tasks within each story can run in parallel when they touch different files.
- Documentation tasks T051-T052 can run in parallel during polish.

## Parallel Example: User Story 1

```text
Task: "Add preference-resolution tests for built-in defaults, valid project-local config, malformed config, and invalid config in ApplicationTests/ImageNamingPreferencesTests.cs"
Task: "Add CLI contract tests for missing config using defaults and invalid project-local config blocking before rename in ApplicationTests/ImageRenameCliTests.cs"
Task: "Add Ollama factory test proving configured model name is passed into client construction seam in ApplicationTests/OllamaAgentFactoryTests.cs"
```

## Parallel Example: User Story 3

```text
Task: "Add formatter tests for normal preserving valid punctuation and casing while removing invalid filename characters and duplicate extension text in ApplicationTests/ImageNameFormatterTests.cs"
Task: "Add ImageRenamer test proving model output is formatted before file rename in ApplicationTests/ImageRenamerTests.cs"
```

## Implementation Strategy

### MVP First

1. Complete Phase 1 and Phase 2.
2. Complete Phase 3 for project-local saved config and defaults.
3. Stop and validate US1 independently with the relevant tests.

### Incremental Delivery

1. Deliver US1 for project-local defaults.
2. Add US2 for per-run overrides.
3. Add US3 for convention formatting.
4. Add US4 for max-length handling.
5. Add US5 cleanup and regression preservation.

### Validation Command

```powershell
dotnet test ApplicationTests\ApplicationTests.csproj -v minimal
```
