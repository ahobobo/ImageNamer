# Data Model: Codebase Refactor Audit

## Audit Finding

Represents one maintainability observation discovered during the repo review.

**Fields**:

- `id`: Stable identifier for cross-reference from proposals.
- `category`: One of readability, possible dead code, duplication, modernization, test maintainability, or architecture boundary.
- `location`: Repository-relative file path and optional line reference.
- `observation`: What was found.
- `impact`: Why the finding matters to maintainability, review cost, or regression risk.
- `confidence`: Confirmed, likely, or suspected.
- `behaviorImpact`: Expected unchanged behavior, potential behavior change, or unknown.
- `nextAction`: Recommended follow-up action.

**Validation Rules**:

- Every finding must include `category`, `location`, `observation`, `impact`, and `nextAction`.
- Dead-code findings must include `confidence`.
- Any finding with potential or unknown behavior impact must be marked medium or high risk when converted to a proposal.

## Refactor Proposal

Represents a proposed cleanup task derived from one or more findings.

**Fields**:

- `id`: Stable identifier.
- `title`: Short action-oriented name.
- `sourceFindings`: One or more audit finding identifiers.
- `priority`: P1, P2, or P3.
- `risk`: Low, medium, or high.
- `expectedBehaviorImpact`: Usually behavior unchanged; otherwise requires a separate behavior-changing feature.
- `scope`: Files or project areas expected to be touched.
- `recommendedSlice`: Smallest useful reviewable unit of work.
- `validationExpectations`: Build, tests, or manual checks required after implementation.

**Validation Rules**:

- Every proposal must reference at least one finding.
- Every proposal must include priority, risk, expected behavior impact, and validation expectations.
- Proposals must be small enough to review independently.

## Validation Expectation

Represents evidence needed to prove an audit deliverable or later refactor is acceptable.

**Fields**:

- `type`: Review, build, unit test, integration test, end-to-end test, or manual check.
- `commandOrMethod`: Exact command or review method.
- `expectedOutcome`: Observable pass condition.
- `appliesTo`: Finding or proposal identifiers covered.

**Validation Rules**:

- Production-code proposals must include at least one automated validation expectation.
- Audit-only validation may use review checks against the report contract.
- File-system behavior must be validated with isolated test data, not real user files.

## Relationships

- One audit finding may produce zero or more refactor proposals.
- One refactor proposal must reference at least one audit finding.
- One validation expectation may apply to multiple findings or proposals.
- Follow-up task generation should use refactor proposals as the task source of truth.
