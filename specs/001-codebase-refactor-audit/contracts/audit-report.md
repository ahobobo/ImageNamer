# Contract: Maintainability Audit Report

The audit report is the user-facing deliverable for this feature. It should be stored in this feature directory during implementation, for example as `audit-report.md`.

## Required Sections

1. **Summary**
   - Overall maintainability assessment.
   - Highest-value cleanup opportunities.
   - Explicit statement that no production behavior was changed by the audit.

2. **Findings**
   - Grouped by category: readability, possible dead code, duplication, modernization, test maintainability, architecture boundary.
   - Each finding must include:
     - ID
     - Location
     - Observation
     - Impact
     - Confidence
     - Proposed next action

3. **Refactor Proposals**
   - Each proposal must include:
     - ID
     - Title
     - Source finding IDs
     - Priority
     - Risk
     - Expected behavior impact
     - Suggested validation

4. **Recommended Sequence**
   - Small, reviewable order for follow-up work.
   - Larger or riskier proposals separated from low-risk cleanup.

5. **Validation Notes**
   - Review checks used for the audit artifact.
   - Build and test commands recommended for later implementation tasks.

## Required Quality Bar

- Findings must be specific enough for a maintainer to locate the affected code quickly.
- Proposals must not bundle unrelated cleanup work.
- Dead-code recommendations must state whether removal is confirmed or needs verification.
- Duplicated test code must not be abstracted away when repetition improves test clarity.
- Modernization recommendations that require dependency or platform changes must be clearly separated from low-risk refactors.
