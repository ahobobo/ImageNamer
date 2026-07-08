# Research: Codebase Refactor Audit

## Decision: Use a structured Markdown audit report

**Rationale**: The feature is an internal maintainability review, so a human-readable report is the most useful deliverable. Markdown keeps findings close to the Spec Kit artifacts and supports links to source locations, categories, risk labels, and follow-up task candidates.

**Alternatives considered**: A spreadsheet would make sorting easier but adds file-format overhead. Inline code comments would scatter audit output through the repo and create noise before follow-up work is approved.

## Decision: Classify dead code by confidence level

**Rationale**: Some code can appear unused because it supports tests, manual validation, or future integration. Findings should label dead-code candidates as confirmed, likely, or suspected, and suspected cases should require verification before removal.

**Alternatives considered**: Removing all apparently unused code immediately was rejected because it risks breaking test-only or manual workflows. Ignoring uncertain dead-code findings was rejected because it hides useful cleanup opportunities.

## Decision: Evaluate duplication by readability benefit, not line count alone

**Rationale**: Test fakes and setup code can be intentionally repetitive to keep test intent clear. The audit should recommend consolidation only when shared helpers improve clarity, reduce maintenance risk, and preserve obvious behavior.

**Alternatives considered**: A strict DRY rule was rejected because it can produce abstractions that make tests harder to read. No duplication review was rejected because the user explicitly requested DRYness opportunities.

## Decision: Treat modernization as behavior-preserving unless explicitly separated

**Rationale**: Modernization can mean language cleanup, dependency updates, project structure changes, or naming improvements. The audit should separate low-risk readability modernizations from changes that alter dependencies, platform assumptions, or user behavior.

**Alternatives considered**: Bundling modernization with refactors was rejected because it would increase review risk. Excluding modernization entirely was rejected because the user requested modernization proposals.

## Decision: Use build and existing automated tests as validation gates for later refactors

**Rationale**: The repository already has application and end-to-end test projects, and the constitution requires behavior-oriented validation. Later cleanup tasks should pass the solution build plus targeted test suites relevant to the touched behavior.

**Alternatives considered**: Review-only validation was rejected for production code changes. Running only broad end-to-end tests was rejected because unit-level failures should be caught closer to the changed code.
