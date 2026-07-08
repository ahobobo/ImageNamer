# Tasks: Codebase Refactor Audit

**Input**: Design documents from `specs/001-codebase-refactor-audit/`

**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/audit-report.md](contracts/audit-report.md), [quickstart.md](quickstart.md)

**Tests**: This feature produces an audit artifact. Validation tasks are review-based for `audit-report.md`; automated build/test commands are captured for baseline and future refactor validation.

**Organization**: Tasks are grouped by user story and vertical slice so each story can be completed and reviewed independently.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it touches different files or independent report sections
- **[Story]**: Which user story this task belongs to
- Every task includes an exact file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the audit artifact and capture baseline validation context.

- [X] T001 Create `specs/001-codebase-refactor-audit/audit-report.md` with required sections from `specs/001-codebase-refactor-audit/contracts/audit-report.md`
- [X] T002 Record baseline validation command list and pending execution status in `specs/001-codebase-refactor-audit/audit-report.md`
- [X] T003 [P] Create source review checklist covering `Console/`, `Application/`, and `Infrastructure/` in `specs/001-codebase-refactor-audit/audit-report.md`
- [X] T004 [P] Create test review checklist covering `ApplicationTests/`, `E2ETests/`, and `TestsShared/` in `specs/001-codebase-refactor-audit/audit-report.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish consistent finding and proposal identifiers before user story work begins.

**CRITICAL**: No user story work can begin until this phase is complete.

- [X] T005 Define finding ID scheme, category labels, confidence labels, and behavior-impact labels in `specs/001-codebase-refactor-audit/audit-report.md`
- [X] T006 Define proposal ID scheme, priority labels, risk labels, and validation expectation labels in `specs/001-codebase-refactor-audit/audit-report.md`
- [X] T007 Add report review checklist matching `specs/001-codebase-refactor-audit/data-model.md` validation rules to `specs/001-codebase-refactor-audit/audit-report.md`

**Checkpoint**: Audit report structure is ready; user story work can now begin.

---

## Phase 3: User Story 1 - Identify Maintainability Findings (Priority: P1) MVP

**Goal**: Produce a categorized repo-wide findings list with concrete locations, observations, impacts, confidence labels, and proposed next actions.

**Independent Test**: Review `specs/001-codebase-refactor-audit/audit-report.md` and confirm each finding references a location, describes the maintainability concern, and explains the expected benefit.

### Vertical Slice 1A - Production Code Findings

- [X] T008 [P] [US1] Review CLI entrypoint and option/config flow in `Console/` for readability, dead-code, duplication, modernization, and boundary findings
- [X] T009 [P] [US1] Review application use cases, models, and ports in `Application/` for readability, dead-code, duplication, modernization, and boundary findings
- [X] T010 [P] [US1] Review filesystem, model-provider, transport, and validation integrations in `Infrastructure/` for readability, dead-code, duplication, modernization, and boundary findings
- [X] T011 [US1] Record production code findings with IDs, locations, observations, impacts, confidence labels, behavior-impact labels, and next actions in `specs/001-codebase-refactor-audit/audit-report.md`

### Vertical Slice 1B - Test Code Findings

- [X] T012 [P] [US1] Review unit and integration-style tests in `ApplicationTests/` for duplicated fakes, unclear setup, dead helpers, and modernization opportunities
- [X] T013 [P] [US1] Review end-to-end tests in `E2ETests/` for duplicated flows, fixture clarity, validation gaps, and maintainability risks
- [X] T014 [P] [US1] Review shared test helpers in `TestsShared/` and fixture usage in `TestData/` for consolidation and clarity opportunities
- [X] T015 [US1] Record test code findings with IDs, locations, observations, impacts, confidence labels, behavior-impact labels, and next actions in `specs/001-codebase-refactor-audit/audit-report.md`

### Vertical Slice 1C - Findings Validation

- [X] T016 [US1] Validate every finding in `specs/001-codebase-refactor-audit/audit-report.md` has category, location, observation, impact, confidence, behavior impact, and next action
- [X] T017 [US1] Validate suspected dead-code findings in `specs/001-codebase-refactor-audit/audit-report.md` are labeled as suspected or likely unless confirmed by evidence

**Checkpoint**: User Story 1 is independently complete when the findings section satisfies SC-001 and FR-001 through FR-003.

---

## Phase 4: User Story 2 - Prioritize Safe Refactors (Priority: P2)

**Goal**: Convert the highest-value findings into prioritized, risk-labeled, small refactor proposals that are safe to plan as follow-up work.

**Independent Test**: Review `specs/001-codebase-refactor-audit/audit-report.md` and confirm each proposal has priority, risk level, expected behavior impact, source finding IDs, and validation expectations.

### Vertical Slice 2A - Proposal Creation

- [X] T018 [US2] Group related findings from `specs/001-codebase-refactor-audit/audit-report.md` into candidate refactor proposals without bundling unrelated cleanup
- [X] T019 [US2] Write refactor proposals with IDs, titles, source finding IDs, scope, recommended slice, and expected behavior impact in `specs/001-codebase-refactor-audit/audit-report.md`
- [X] T020 [US2] Assign priority and risk labels to each refactor proposal in `specs/001-codebase-refactor-audit/audit-report.md`

### Vertical Slice 2B - Sequence and Validation

- [X] T021 [US2] Add suggested validation commands or review checks for each refactor proposal in `specs/001-codebase-refactor-audit/audit-report.md`
- [X] T022 [US2] Write recommended follow-up sequence separating low-risk cleanup from larger modernization or dependency work in `specs/001-codebase-refactor-audit/audit-report.md`
- [X] T023 [US2] Validate every proposal in `specs/001-codebase-refactor-audit/audit-report.md` references at least one finding and includes priority, risk, behavior impact, and validation expectations

**Checkpoint**: User Story 2 is independently complete when proposals satisfy SC-002, SC-004, FR-004, FR-005, FR-008, and FR-010.

---

## Phase 5: User Story 3 - Preserve Existing Behavior (Priority: P3)

**Goal**: Ensure the audit clearly identifies behavior that must remain unchanged and the validation needed before any future refactor is accepted.

**Independent Test**: Review `specs/001-codebase-refactor-audit/audit-report.md` and confirm proposed cleanup work names the current behavior to preserve and the tests or checks needed to protect it.

### Vertical Slice 3A - Behavior Preservation Notes

- [X] T024 [US3] Document CLI behaviors that must remain unchanged for CLI-affecting proposals in `specs/001-codebase-refactor-audit/audit-report.md`
- [X] T025 [US3] Document file handling and model-provider behaviors that must remain unchanged for application or infrastructure proposals in `specs/001-codebase-refactor-audit/audit-report.md`
- [X] T026 [US3] Document deterministic test isolation expectations for proposals touching `ApplicationTests/`, `E2ETests/`, or `TestsShared/` in `specs/001-codebase-refactor-audit/audit-report.md`

### Vertical Slice 3B - Quality Gate Mapping

- [X] T027 [US3] Map each proposal to the smallest relevant validation command from `specs/001-codebase-refactor-audit/quickstart.md` in `specs/001-codebase-refactor-audit/audit-report.md`
- [X] T028 [US3] Mark proposals requiring dependency upgrades, platform changes, or behavior changes as separate future features in `specs/001-codebase-refactor-audit/audit-report.md`
- [X] T029 [US3] Validate behavior-preservation notes in `specs/001-codebase-refactor-audit/audit-report.md` satisfy FR-004, FR-008, TE-002, TE-003, and TE-004

**Checkpoint**: User Story 3 is independently complete when future refactors have behavior-preservation and validation guidance.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final report cleanup and readiness checks.

- [X] T030 [P] Verify `specs/001-codebase-refactor-audit/audit-report.md` follows all required sections in `specs/001-codebase-refactor-audit/contracts/audit-report.md`
- [X] T031 [P] Verify `specs/001-codebase-refactor-audit/audit-report.md` satisfies success criteria SC-001 through SC-005 from `specs/001-codebase-refactor-audit/spec.md`
- [X] T032 Ensure `specs/001-codebase-refactor-audit/audit-report.md` contains no unfilled placeholders, ambiguous "TBD" entries, or unresolved uncertainty outside explicitly labeled suspected findings
- [X] T033 Update `specs/001-codebase-refactor-audit/audit-report.md` summary with the highest-value cleanup opportunities and the suggested MVP follow-up task
- [X] T034 Run the review steps from `specs/001-codebase-refactor-audit/quickstart.md` and record the result in `specs/001-codebase-refactor-audit/audit-report.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; can start immediately.
- **Foundational (Phase 2)**: Depends on Setup completion; blocks all user story work.
- **User Story 1 (Phase 3)**: Depends on Foundational completion and is the MVP.
- **User Story 2 (Phase 4)**: Depends on User Story 1 findings.
- **User Story 3 (Phase 5)**: Depends on User Story 2 proposals.
- **Polish (Phase 6)**: Depends on desired user stories being complete.

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational; no dependency on other stories.
- **User Story 2 (P2)**: Requires User Story 1 findings.
- **User Story 3 (P3)**: Requires User Story 2 proposals.

### Within Each User Story

- Inspect source areas before recording consolidated findings.
- Record findings before grouping proposals.
- Assign validation expectations before final report review.
- Complete each checkpoint before moving to the next story.

### Parallel Opportunities

- T003 and T004 can run in parallel.
- T008, T009, and T010 can run in parallel.
- T012, T013, and T014 can run in parallel.
- T030 and T031 can run in parallel after all user stories are complete.

---

## Parallel Example: User Story 1

```text
Task: "T008 [P] [US1] Review CLI entrypoint and option/config flow in Console/"
Task: "T009 [P] [US1] Review application use cases, models, and ports in Application/"
Task: "T010 [P] [US1] Review filesystem, model-provider, transport, and validation integrations in Infrastructure/"
```

```text
Task: "T012 [P] [US1] Review unit and integration-style tests in ApplicationTests/"
Task: "T013 [P] [US1] Review end-to-end tests in E2ETests/"
Task: "T014 [P] [US1] Review shared test helpers in TestsShared/ and TestData/"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational structure.
3. Complete Phase 3: User Story 1 findings.
4. Stop and validate that findings are concrete, categorized, and actionable.

### Incremental Delivery

1. Deliver US1 findings as the MVP audit inventory.
2. Add US2 prioritized proposals.
3. Add US3 behavior-preservation and validation guidance.
4. Complete polish checks and mark the audit ready for follow-up planning.

### Parallel Team Strategy

With multiple contributors, split US1 source review by project area after Phase 2. Consolidate findings before starting US2 so proposal IDs reference a stable findings list.

---

## Notes

- `[P]` tasks touch independent review areas or independent report checks.
- `[US1]`, `[US2]`, and `[US3]` labels map directly to user stories in `specs/001-codebase-refactor-audit/spec.md`.
- The feature deliverable is `specs/001-codebase-refactor-audit/audit-report.md`.
- Future production refactors should be generated from the proposals in the audit report, not bundled into this audit task list.
