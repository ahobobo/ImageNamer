<!--
Sync Impact Report
Version change: template -> 1.0.0
Modified principles:
- PRINCIPLE_1_NAME -> I. Vertical Slice Delivery
- PRINCIPLE_2_NAME -> II. Testable Features and Real Logic
- PRINCIPLE_3_NAME -> III. Current Dependencies and Documentation
- PRINCIPLE_4_NAME -> IV. Composable Operation Strategies
- PRINCIPLE_5_NAME -> V. C# Standard Library First
Added sections:
- Architecture Boundaries
- Development Workflow
Removed sections:
- Placeholder section SECTION_2_NAME
- Placeholder section SECTION_3_NAME
Templates requiring updates:
- UPDATED .specify/templates/plan-template.md
- UPDATED .specify/templates/spec-template.md
- UPDATED .specify/templates/tasks-template.md
- N/A .specify/templates/commands/*.md (directory absent)
- UPDATED .agents/skills/speckit-plan/SKILL.md
- UPDATED .agents/skills/speckit-tasks/SKILL.md
- REVIEWED README.md (no principle references required changes)
- REVIEWED AGENTS.md (no principle references required changes)
Other files updated:
- .specify/extensions/git/scripts/powershell/initialize-repo.ps1
Follow-up TODOs:
- None
-->
# ImageNamer Constitution

## Core Principles

### I. Vertical Slice Delivery
Tasks MUST be organized as independently testable vertical slices of user-visible
functionality. When a feature requires coordinated CLI, application-layer, and
infrastructure changes, task generation MUST keep those changes inside the same
feature slice and order the work as CLI contract or behavior, application logic,
then infrastructure adapters. Shared setup is allowed only when it is a true
prerequisite for multiple slices.

Rationale: each increment must be runnable, reviewable, and testable without
waiting for unrelated architecture layers or later stories.

### II. Testable Features and Real Logic
Every feature MUST be specified and implemented in a testable way. Automated
tests MUST prefer input/output behavior through real application logic. Tests MAY
replace operations that cross external boundaries, including model calls,
filesystem mutation, network calls, clocks, and process execution, with fakes
that implement established application-layer interfaces. Tests MUST NOT replace
core application behavior with mocks when the real code can run deterministically.

Rationale: tests should prove the behavior users rely on while keeping external
systems controlled, repeatable, and fast.

### III. Current Dependencies and Documentation
Plans that add, remove, or modify packages MUST verify current package versions
and current usage guidance from authoritative web documentation before choosing
an implementation. Package decisions MUST record the checked version, source,
and date in the feature research or plan. Implementations MUST NOT rely on
stale package knowledge when current documentation is available.

Rationale: dependency choices age quickly, and the project must avoid building
new work on obsolete APIs or unsupported versions.

### IV. Composable Operation Strategies
Pipelines and operation sequences MUST encapsulate each operation in a focused
strategy with a clear interface. The application MUST compose those strategies
through a fluent builder or equivalent composition API that separates operation
creation from operation execution. Exceptions require explicit justification in
the plan when a single inline flow is materially simpler and still testable.

Rationale: renaming workflows, media processing, and model interactions are
easier to test and extend when operations are independently substitutable.

### V. C# Standard Library First
The project MUST use built-in C# and .NET features when they satisfy the need
cleanly. Custom infrastructure, utilities, or abstractions MUST be justified by
missing platform support, improved testability, or meaningful reduction in
duplication. Reimplementing standard library behavior without a project-specific
reason is not allowed.

Rationale: standard platform features reduce maintenance cost and make the code
more familiar to contributors.

## Architecture Boundaries

ImageNamer is a .NET console application with these intended boundaries:

- `Console/` owns CLI parsing, user-facing process behavior, and dependency
  composition.
- `Application/` owns use cases, application interfaces, operation strategies,
  and deterministic business behavior.
- `Infrastructure/` owns adapters for external systems such as Ollama and the
  filesystem.
- `ApplicationTests/` owns automated tests and test fakes for application-layer
  interfaces.

Application code MUST depend on interfaces for external model calls, filesystem
mutation, and other nondeterministic operations. Infrastructure adapters MAY
touch real external systems; application tests SHOULD use fakes for those
interfaces while preserving real application logic.

## Development Workflow

Specifications MUST define independently testable user scenarios and measurable
success criteria. Plans MUST complete the Constitution Check before design work
continues and MUST record dependency research for any package changes. Task
lists MUST include test tasks for each feature slice unless the plan explicitly
justifies why automated testing is not feasible.

Within each user story, tasks MUST be ordered to produce a runnable increment:
tests first, then CLI contract or behavior, then application logic, then
infrastructure adapters and composition. Each checkpoint MUST identify how to
run the relevant tests or otherwise validate the slice.

## Governance

This constitution supersedes conflicting guidance in templates, plans, and task
lists. Amendments require updating this file, recording a Sync Impact Report,
and propagating changes to affected Spec Kit templates or agent guidance in the
same change.

Versioning follows semantic versioning:

- MAJOR for removals or incompatible redefinitions of principles.
- MINOR for new principles, new governance sections, or materially expanded
  required practices.
- PATCH for clarifications, wording fixes, and non-semantic refinements.

All feature plans MUST pass the Constitution Check before implementation tasks
are generated. Reviews MUST treat unresolved constitution violations as blocking
issues unless the constitution itself is amended first.

**Version**: 1.0.0 | **Ratified**: 2026-05-08 | **Last Amended**: 2026-05-08
