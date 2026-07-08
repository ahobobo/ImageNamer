<!--
Sync Impact Report
Version change: 1.0.0 -> 1.1.0
Modified principles:
- Added VI. Vertical Slice Delivery
- Expanded Quality Standards with vertical-slice task sequencing
- Expanded Development Workflow with slice-level implementation order
Added sections:
- None
Removed sections:
- None
Templates requiring updates:
- Updated: .specify/templates/plan-template.md
- Updated: .specify/templates/tasks-template.md
- Reviewed, no update required: .specify/templates/spec-template.md
- Not present: .specify/templates/commands/*.md
Follow-up TODOs:
- None
-->
# ImageNamer Constitution

## Core Principles

### I. Maintainable Architecture
Production code MUST preserve clear boundaries between the console entrypoint,
application use cases, infrastructure integrations, and shared test utilities.
Dependencies MUST point inward toward application abstractions rather than binding
core behavior to CLI, filesystem, or Ollama implementation details.

Rationale: ImageNamer performs filesystem mutations through an external model
provider. Clear boundaries keep the renaming behavior easy to test and reduce
the risk of accidental file changes when integrations evolve.

### II. Testable Behavior
Every behavior change MUST include automated tests that verify the externally
observable result, including success paths, validation failures, and error
handling where applicable. Tests MUST be deterministic, isolated from the user's
real files and services, and located in the appropriate project test suite.

Rationale: The project renames local files in place, so regressions can be
costly. Tests are the primary guardrail for preserving expected behavior.

### III. Readable Code
Code MUST use clear names, direct control flow, and focused methods whose
purpose can be understood without hidden context. Comments are reserved for
non-obvious decisions, constraints, or tradeoffs; they MUST NOT restate code
mechanically.

Rationale: Readable code lowers review cost and makes future changes safer than
clever abstractions or dense implementation shortcuts.

### IV. Explicit Quality Gates
Changes MUST pass the relevant automated test suite and build before completion.
New warnings, flaky tests, skipped assertions, or failing quality checks MUST be
fixed or documented with a concrete follow-up before the change is accepted.

Rationale: A working build and trustworthy tests are required evidence that the
repository remains shippable after each change.

### V. Minimal, Reviewable Change
Each change MUST stay scoped to the requested behavior and avoid unrelated
refactoring, formatting churn, or dependency changes. Any expansion of scope
MUST be justified by a direct quality, readability, or testing need.

Rationale: Small, focused changes are easier to review, easier to revert, and
less likely to hide behavioral regressions.

### VI. Vertical Slice Delivery
Feature implementation phases MUST be organized as vertical slices, not broad
horizontal passes across UI, application, and data layers. Each slice MUST start
at the UI, CLI, API, or other outer boundary, flow through service/application
logic, reach any required data or infrastructure layer, include focused unit
tests as each layer's behavior is added, and finish with an integration test
that verifies the full slice end to end.

Rationale: Vertical slices produce working increments sooner, expose integration
risks earlier, and keep tests tied to user-visible behavior instead of isolated
layer completion.

## Quality Standards

- Public behavior MUST be described in feature specs with acceptance scenarios
  and measurable success criteria before implementation planning.
- Plans MUST identify the relevant test project, required test level, and build
  or validation commands.
- Implementation tasks MUST include test tasks for each user story that changes
  behavior before the corresponding implementation tasks.
- Task lists MUST break feature work into vertical slices. They MUST NOT group
  implementation as large UI-only, service-only, or data-only phases except for
  narrowly scoped setup work that multiple slices genuinely share.
- Code MUST follow the repository's existing .NET style, project boundaries, and
  naming conventions unless the plan explicitly justifies a change.
- File and model-provider interactions MUST be represented through abstractions
  when needed for deterministic testing.

## Development Workflow

1. Specify user-visible behavior and edge cases.
2. Plan the implementation boundaries, test approach, and quality gates.
3. Divide implementation into vertical slices that each deliver observable
   behavior through the outer boundary, application/service layer, and data or
   infrastructure layer.
4. Within each slice, write or update unit tests before implementing the layer
   behavior they cover.
5. Complete each slice with an integration test that verifies the full path.
6. Implement the smallest coherent change that satisfies the tests.
7. Run the documented build and test commands.
8. Record unresolved risks or follow-up work in the feature artifacts.

## Governance

This constitution supersedes conflicting repository guidance for code quality,
testing standards, and readability. Amendments require an update to this file,
a Sync Impact Report describing affected templates or docs, and propagation to
dependent spec-kit templates when their guidance changes.

Versioning follows semantic versioning:
- MAJOR for removing or redefining a principle in a backward-incompatible way.
- MINOR for adding a principle or materially expanding governance requirements.
- PATCH for clarifications, wording fixes, or non-semantic refinements.

Compliance is reviewed during planning, task generation, implementation review,
and before completion. Any intentional exception MUST be documented in the plan's
Complexity Tracking section with the reason and the simpler alternative rejected.

**Version**: 1.1.0 | **Ratified**: 2026-07-08 | **Last Amended**: 2026-07-08
