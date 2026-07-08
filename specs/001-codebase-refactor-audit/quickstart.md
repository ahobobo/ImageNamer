# Quickstart: Codebase Refactor Audit

## Prerequisites

- .NET 10 SDK installed.
- Repository restored and buildable from the project root.
- No Ollama service is required for the audit artifact itself.

## Validate Current Baseline

Run from the repository root:

```powershell
dotnet build ImageNamer.slnx -v minimal
dotnet test ApplicationTests\ApplicationTests.csproj -v minimal
dotnet test E2ETests\E2ETests.csproj -v minimal
```

Expected outcome: build and tests pass before implementation work begins. If an existing failure is unrelated to the audit, document it in the audit validation notes before proceeding.

## Produce The Audit

1. Review the source and test areas named in [plan.md](plan.md).
2. Record findings using the model in [data-model.md](data-model.md).
3. Format the report using [contracts/audit-report.md](contracts/audit-report.md).
4. Separate confirmed cleanup from suspected cleanup that needs verification.
5. Group follow-up proposals into small, reviewable tasks.

Expected outcome: the report includes categorized findings, prioritized proposals, risk labels, behavior-impact notes, and validation expectations.

## Review The Audit Artifact

Check the report against these acceptance points:

- Every finding has a concrete location and next action.
- Every proposal has priority, risk, behavior impact, and validation expectations.
- Possible dead code is labeled by confidence.
- Duplicated test code is only proposed for consolidation when clarity improves.
- Modernization items that require dependency or platform changes are separated from simple cleanup.

## Validate Later Refactors

For each follow-up refactor, run the smallest relevant test set plus the build. Use the full command set below when the change touches multiple project boundaries:

```powershell
dotnet build ImageNamer.slnx -v minimal
dotnet test ApplicationTests\ApplicationTests.csproj -v minimal
dotnet test E2ETests\E2ETests.csproj -v minimal
```

Expected outcome: behavior remains unchanged unless a separate feature explicitly changes it.
