# Tasks: WEBP Model Compatibility

**Input**: Design documents from `/specs/002-webp-model-compatibility/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Test tasks are REQUIRED for every behavior change. Include them before the implementation tasks they verify, and make each test deterministic and isolated from real user files or external services.

**Organization**: Tasks are grouped by user story and vertical slice to enable independent implementation and testing of each story. Avoid horizontal phases that complete an entire UI layer, service layer, or data layer before producing an integrated behavior.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Add the planned dependency required for in-memory WEBP to PNG conversion.

- [X] T001 Add the `SkiaSharp` 4.150.0 package reference to Infrastructure/Infrastructure.csproj

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish shared deterministic coverage assets and helpers needed by all WEBP behavior slices.

**CRITICAL**: No user story work can begin until this phase is complete

- [X] T002 Add deterministic WEBP and invalid WEBP fixture coverage support in ApplicationTests/TestImageFile.cs and TestData/

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Rename WEBP Images Successfully (Priority: P1) MVP

**Goal**: Allow valid WEBP images to be converted to PNG in memory for model submission while keeping the normal rename flow and `.webp` rename result on disk.

**Independent Test**: Run the application and end-to-end tests to verify a valid `.webp` file is submitted to the model as `image/png`, receives a name, and is renamed successfully with the original `.webp` extension preserved.

### Vertical Slice 1A - Successful WEBP model submission and rename

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [X] T003 [US1] Add unit tests for WEBP model-payload conversion and extension preservation in ApplicationTests/FileOperatorTests.cs and ApplicationTests/ImageRenamerTests.cs
- [X] T004 [US1] Add unit tests for model-facing WEBP payload metadata in ApplicationTests/OllamaAgentTests.cs
- [X] T005 [US1] Implement in-memory WEBP to PNG preparation in Infrastructure/ForReadingImages/FileOperator.cs
- [X] T006 [US1] Update shared image-file models for converted model payload metadata in Application/Models/FileRecords.cs
- [X] T007 [US1] Wire the converted model payload through the rename flow in Infrastructure/ForTalkingWithModels/OllamaAgent.cs
- [X] T008 [US1] Add full-slice CLI coverage for successful WEBP renaming in E2ETests/RenameSingleImageTests.cs and TestsShared/CliDriver.cs
- [X] T009 [US1] Validate the completed WEBP success slice with `dotnet test ApplicationTests/ApplicationTests.csproj -v minimal` and `dotnet test E2ETests/E2ETests.csproj -v minimal`

**Checkpoint**: At this point, User Story 1 must be fully functional and testable independently

---

## Phase 4: User Story 2 - Preserve Existing Behavior for Other Images (Priority: P2)

**Goal**: Ensure non-WEBP inputs and mixed-image directory runs continue to use current behavior except for WEBP-specific compatibility handling.

**Independent Test**: Run existing non-WEBP and mixed-directory tests to confirm non-WEBP MIME handling remains unchanged and recursive processing still preserves current ordering and reporting behavior.

### Vertical Slice 2A - Non-WEBP regression protection

- [X] T010 [US2] Add regression tests for unchanged non-WEBP submission behavior in ApplicationTests/FileOperatorTests.cs and ApplicationTests/OllamaAgentTests.cs
- [X] T011 [US2] Add mixed-type recursive CLI coverage for WEBP and non-WEBP processing in E2ETests/RecursiveRenamingTests.cs
- [X] T012 [US2] Adjust image preparation branching so only WEBP inputs are converted in Infrastructure/ForReadingImages/FileOperator.cs
- [X] T013 [US2] Validate the non-WEBP regression slice with `dotnet test ApplicationTests/ApplicationTests.csproj -v minimal` and `dotnet test E2ETests/E2ETests.csproj -v minimal`

**Checkpoint**: At this point, User Stories 1 AND 2 must both work independently

---

## Phase 5: User Story 3 - Fail Clearly on Invalid WEBP Input (Priority: P3)

**Goal**: Reject unreadable WEBP inputs before model submission, keep the source file untouched, and preserve existing batch continuation behavior.

**Independent Test**: Run invalid-WEBP tests to verify the file is not renamed, the CLI reports a clear error, and later files in a directory run still process.

### Vertical Slice 3A - Invalid WEBP failure handling

- [X] T014 [US3] Add unit tests for invalid WEBP decode failures in ApplicationTests/FileOperatorTests.cs
- [X] T015 [US3] Add CLI and batch-continuation tests for invalid WEBP input in ApplicationTests/ImageRenameRunnerTests.cs and E2ETests/RecursiveRenamingTests.cs
- [X] T016 [US3] Implement explicit invalid-WEBP failure handling during image preparation in Infrastructure/ForReadingImages/FileOperator.cs
- [X] T017 [US3] Validate the invalid-WEBP slice with `dotnet test ApplicationTests/ApplicationTests.csproj -v minimal` and `dotnet test E2ETests/E2ETests.csproj -v minimal`

**Checkpoint**: All user stories must now be independently functional

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finish documentation and run the full quality gates for the feature.

- [X] T018 [P] Update user-facing feature notes for WEBP compatibility in README.md
- [X] T019 Run the full feature validation from specs/002-webp-model-compatibility/quickstart.md using `dotnet build ImageNamer.slnx -v minimal`, `dotnet test ApplicationTests/ApplicationTests.csproj -v minimal`, and `dotnet test E2ETests/E2ETests.csproj -v minimal`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel if staffing allows, though P1 should land first because later stories extend its behavior
  - Recommended sequence: P1 -> P2 -> P3
- **Polish (Phase 6)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Foundational and establishes the core WEBP conversion path
- **User Story 2 (P2)**: Starts after Foundational and should build on the US1 implementation to lock in unchanged non-WEBP behavior
- **User Story 3 (P3)**: Starts after Foundational and should build on the US1 conversion path to handle decode failures cleanly

### Within Each User Story

- Tests MUST be written and fail before implementation
- Outer CLI-visible behavior is validated before infrastructure changes are considered complete
- Infrastructure conversion behavior must be in place before end-to-end validation
- Story complete before moving to the next priority when working solo

### Parallel Opportunities

- T018 can run in parallel with final validation work once implementation is complete
- If multiple engineers are available after T002, one can prepare US2 regression tests while another prepares US3 failure tests, but both should merge after the US1 conversion path stabilizes

---

## Parallel Example: User Story 1

```bash
# Launch independent test authoring work once fixtures are ready:
Task: "Add WEBP conversion unit tests in ApplicationTests/FileOperatorTests.cs and ApplicationTests/ImageRenamerTests.cs"
Task: "Add model-facing payload tests in ApplicationTests/OllamaAgentTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Confirm valid WEBP rename succeeds end to end
5. Demo or merge the MVP if WEBP success is the immediate priority

### Incremental Delivery

1. Complete Setup + Foundational
2. Add User Story 1 -> Validate valid WEBP rename behavior
3. Add User Story 2 -> Validate non-WEBP and mixed-run regression coverage
4. Add User Story 3 -> Validate invalid WEBP failure handling and batch continuation
5. Finish with README and full quickstart validation

### Parallel Team Strategy

With multiple developers:

1. One developer adds the dependency and shared fixtures
2. After T002:
   - Developer A: US1 implementation and end-to-end success coverage
   - Developer B: US2 regression tests for non-WEBP and mixed directories
   - Developer C: US3 invalid-WEBP failure tests
3. Merge US2 and US3 after the US1 infrastructure contract is stable

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] labels map tasks to specific user stories for traceability
- Each user story remains independently testable from the CLI boundary
- The task order preserves the repo constitution requirement for tests before implementation
- Avoid expanding scope into transport redesign or changing on-disk file extensions
