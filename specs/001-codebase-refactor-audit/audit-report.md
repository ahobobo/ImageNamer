# Codebase Refactor Audit

## Summary

ImageNamer is already split into clear CLI, application, infrastructure, and test projects. The strongest cleanup opportunities are to reduce CLI parsing/control-flow density, remove or relocate test-only infrastructure, consolidate repeated filesystem test setup, and make E2E model interactions deterministic. This audit changed no production behavior.

Highest-value follow-up: make the E2E suite independent from a live Ollama model by introducing a deterministic test transport or CLI injection path. That one change would turn the current failing E2E gate into a reliable refactor safety net.

## Report Metadata

- Finding IDs: `F-###`
- Proposal IDs: `P-###`
- Categories: readability, possible dead code, duplication, modernization, test maintainability, architecture boundary
- Confidence labels: confirmed, likely, suspected
- Behavior impact labels: behavior unchanged, potential behavior change, unknown
- Priority labels: P1, P2, P3
- Risk labels: low, medium, high
- Validation labels: review, build, unit test, integration test, end-to-end test, manual check

## Baseline Validation Commands

| Command | Status | Notes |
| --- | --- | --- |
| `dotnet build ImageNamer.slnx -v minimal` | PASS | Build succeeded with existing NU1900 warnings from an unavailable private Telerik NuGet source. |
| `dotnet test ApplicationTests\ApplicationTests.csproj -v minimal` | PASS | 40 tests passed with the same NU1900 warnings. |
| `dotnet test E2ETests\E2ETests.csproj -v minimal` | FAIL | 5 tests failed because CLI runs returned HTTP 404 from the live model endpoint. |

## Source Review Checklist

- [x] `Console/` CLI entrypoint, option parsing, configuration loading, dependency construction, runner flow
- [x] `Application/` use cases, preference resolution, name formatting, models, ports
- [x] `Infrastructure/` filesystem operator, image extension handling, model transport, model agent, filename validation

## Test Review Checklist

- [x] `ApplicationTests/` unit and integration-style tests
- [x] `E2ETests/` CLI-level test flow and fixtures
- [x] `TestsShared/` shared temp directory and CLI driver helpers
- [x] `TestData/` fixture usage

## Findings

### Readability

#### F-001

- Category: readability
- Location: `Console/ImageRenameOptionsParser.cs:27`
- Observation: `Parse` handles positional input, help flags, four option branches, validation, index mutation, and final option construction in one method.
- Impact: Adding another CLI option requires editing a long branch chain and preserving index side effects, which raises review cost for otherwise small changes.
- Confidence: confirmed
- Behavior impact: behavior unchanged
- Next action: Extract option handlers or a small option-read helper table while keeping the existing accepted arguments and error messages covered by tests.

#### F-002

- Category: readability
- Location: `Console/ImageRenameCli.cs:17`
- Observation: `RunAsync` parses options, loads config, resolves preferences, builds model/renamer dependencies, runs work, writes errors, and sets process exit code.
- Impact: The method is still readable, but it combines orchestration with process-facing behavior, making future changes to exit handling or dependency construction harder to isolate.
- Confidence: confirmed
- Behavior impact: behavior unchanged
- Next action: Introduce a small command handler result type or isolate dependency construction from process output/exit-code handling.

#### F-003

- Category: readability
- Location: `Application/ImageNameFormatter.cs:115`
- Observation: `EnforceMaxLength` mixes separator inference, word removal, fallback truncation, and trailing separator cleanup.
- Impact: Filename trimming rules are user-visible and easy to regress when changed, especially across normal, snake, kebab, and pascal naming conventions.
- Confidence: confirmed
- Behavior impact: behavior unchanged
- Next action: Split length enforcement into named helpers and add focused tests for separator selection and fallback truncation before editing.

### Possible Dead Code

#### F-004

- Category: possible dead code
- Location: `Infrastructure/ForReadingImages/StubFileOperator.cs:6`
- Observation: `StubFileOperator` lives in the production infrastructure project but is only referenced by `ApplicationTests/OllamaAgentTests.cs`.
- Impact: Test-only code in production assemblies weakens the application/infrastructure boundary and may be shipped unnecessarily.
- Confidence: likely
- Behavior impact: behavior unchanged
- Next action: Move the stub into the test project or replace it with a local test fixture, then verify no production caller depends on it.

#### F-005

- Category: possible dead code
- Location: `Infrastructure/ForTalkingWithModels/OllamaAgent.cs:21`
- Observation: The constructor accepts `IForValidatingFileNames fileNameValidator` but never stores or uses it.
- Impact: The unused dependency suggests either stale retry/validation behavior or an incomplete safety check, and it makes dependency construction misleading.
- Confidence: confirmed
- Behavior impact: potential behavior change
- Next action: Decide whether model output should be validated here. If not, remove the dependency; if yes, add validation behavior and tests as a separate behavior-changing feature.

#### F-006

- Category: possible dead code
- Location: `E2ETests/E2ETestBase.cs:8`
- Observation: `TestDataPath` is assigned during setup but no E2E test reads the property.
- Impact: The unused property adds state to the base class without a current consumer.
- Confidence: likely
- Behavior impact: behavior unchanged
- Next action: Remove the property if no pending tests need it, leaving the local setup variable inside `SetUp`.

### Duplication

#### F-007

- Category: duplication
- Location: `ApplicationTests/FileOperatorTests.cs:90`, `ApplicationTests/ImageRenameRunnerTests.cs:107`, `TestsShared/TemporaryWorkingDirectory.cs:7`
- Observation: Multiple tests create temp roots under `ImageNamerTests` and clean them manually instead of consistently using `TemporaryWorkingDirectory`.
- Impact: Repeated cleanup code increases the chance of leaked files or inconsistent current-directory behavior in future tests.
- Confidence: confirmed
- Behavior impact: behavior unchanged
- Next action: Extend `TemporaryWorkingDirectory` or add a lighter shared temp-directory helper for tests that do not want current-directory mutation.

#### F-008

- Category: duplication
- Location: `E2ETests/E2ETestBase.cs:15`, `TestsShared/CliDriver.cs:9`, `Infrastructure/ForReadingImages/StubFileOperator.cs:25`
- Observation: Project-root or test-data discovery is implemented in three separate upward-directory scans.
- Impact: Future layout changes must update multiple discovery loops, and the test-only discovery code has leaked into infrastructure via `StubFileOperator`.
- Confidence: confirmed
- Behavior impact: behavior unchanged
- Next action: Put repository-root/test-data discovery in one shared test helper and remove the infrastructure dependency on test-data discovery.

#### F-009

- Category: duplication
- Location: `ApplicationTests/ImageRenameCliTests.cs:123`, `ApplicationTests/ImageRenamerTests.cs:32`, `ApplicationTests/ImageRenameRunnerTests.cs:114`, `ApplicationTests/OllamaAgentTests.cs:28`
- Observation: Several tests define small local recording fakes for ports and model transports.
- Impact: Some repetition is useful for test intent, but repeated recording patterns make it harder to add cross-cutting assertions or default behaviors.
- Confidence: suspected
- Behavior impact: behavior unchanged
- Next action: Consolidate only the fakes that remain obvious at call sites, starting with filesystem/temp helpers before abstracting every local fake.

### Modernization

#### F-010

- Category: modernization
- Location: `Console/ImageRenameOptionsParser.cs:125`
- Observation: Naming convention parsing is implemented with a switch over normalized strings while usage text independently lists accepted values.
- Impact: Accepted CLI values and displayed help can drift as conventions evolve.
- Confidence: confirmed
- Behavior impact: behavior unchanged
- Next action: Centralize naming aliases in a small map used by parser tests and help text generation.

#### F-011

- Category: modernization
- Location: `TestsShared/CliDriver.cs:21`
- Observation: E2E tests invoke `dotnet run --project` for each test, causing restore/build warnings and live process startup inside assertions.
- Impact: Tests are slower, more fragile, and their captured output is polluted by restore warnings from the NuGet feed.
- Confidence: confirmed
- Behavior impact: behavior unchanged
- Next action: Prefer running the already-built CLI assembly or add a test host path that exercises the CLI without invoking `dotnet run` per test.

### Test Maintainability

#### F-012

- Category: test maintainability
- Location: `E2ETests/NamingConventionTests.cs:18`, `E2ETests/RenameSingleImageTests.cs:16`, `E2ETests/RecursiveRenamingTests.cs:12`
- Observation: E2E tests execute the real CLI path, which constructs `OllamaAgentFactory` and calls a live model endpoint.
- Impact: The E2E suite currently fails with HTTP 404 when the configured model endpoint is unavailable or the default model is missing, so it cannot reliably protect refactors.
- Confidence: confirmed
- Behavior impact: behavior unchanged
- Next action: Add deterministic model output injection for E2E tests or split live-model smoke tests from deterministic CLI behavior tests.

#### F-013

- Category: test maintainability
- Location: `ApplicationTests/ImageRenameCliTests.cs:9`
- Observation: CLI tests are marked `[NonParallelizable]` because they mutate global `Console` streams and `Environment.ExitCode`.
- Impact: Global state makes tests order-sensitive and limits parallel execution.
- Confidence: confirmed
- Behavior impact: behavior unchanged
- Next action: Refactor CLI execution behind injectable output and exit-code abstractions, then keep one thin process-level test for wiring.

#### F-014

- Category: test maintainability
- Location: `ApplicationTests/FileOperatorTests.cs:21`, `ApplicationTests/ImageRenameRunnerTests.cs:18`
- Observation: Manual `try/finally` cleanup is repeated in filesystem tests.
- Impact: Cleanup noise obscures the behavior under test and can leave temp files behind if setup fails before entering the `try`.
- Confidence: confirmed
- Behavior impact: behavior unchanged
- Next action: Use a disposable temp helper created before file setup and registered through `using var`.

### Architecture Boundary

#### F-015

- Category: architecture boundary
- Location: `Console/ImageRenameCli.cs:101`
- Observation: Default dependency construction creates application, infrastructure, formatter, and model components directly inside the CLI dependency record.
- Impact: The composition root is appropriate for the console project, but the nested lambda makes wiring harder to inspect and reuse in tests or future hosts.
- Confidence: confirmed
- Behavior impact: behavior unchanged
- Next action: Move default dependency composition to a named factory method or type in the console project.

#### F-016

- Category: architecture boundary
- Location: `Infrastructure/ForReadingImages/StubFileOperator.cs:25`
- Observation: Infrastructure contains a stub that knows about the repository `TestData` folder.
- Impact: Production infrastructure now has a compile-time concept of test fixtures, which conflicts with the intended separation between infrastructure integrations and shared test utilities.
- Confidence: confirmed
- Behavior impact: behavior unchanged
- Next action: Move the stub to `ApplicationTests` or `TestsShared`, then remove test-data lookup from the production project.

## Refactor Proposals

### P-001: Make E2E CLI Tests Deterministic

- Source findings: F-011, F-012
- Priority: P1
- Risk: medium
- Expected behavior impact: behavior unchanged
- Scope: `E2ETests/`, `TestsShared/`, possibly `Console/ImageRenameCli.cs`
- Recommended slice: Add a test-only way to provide deterministic model output, then update E2E tests to validate CLI/file behavior without live Ollama calls.
- Suggested validation: `dotnet build ImageNamer.slnx -v minimal`; `dotnet test E2ETests\E2ETests.csproj -v minimal`; keep any live Ollama smoke coverage separate and explicitly optional.

### P-002: Remove Test-Only Infrastructure Stub From Production

- Source findings: F-004, F-008, F-016
- Priority: P1
- Risk: low
- Expected behavior impact: behavior unchanged
- Scope: `Infrastructure/ForReadingImages/StubFileOperator.cs`, `ApplicationTests/OllamaAgentTests.cs`, optionally `TestsShared/`
- Recommended slice: Move or inline the stub used by `OllamaAgentTests`, then delete the production `StubFileOperator`.
- Suggested validation: `dotnet build ImageNamer.slnx -v minimal`; `dotnet test ApplicationTests\ApplicationTests.csproj -v minimal`.

### P-003: Decide and Fix Model Filename Validation

- Source findings: F-005
- Priority: P1
- Risk: medium
- Expected behavior impact: potential behavior change
- Scope: `Infrastructure/ForTalkingWithModels/OllamaAgent.cs`, `Infrastructure/Validation/FileNameValidator.cs`, `ApplicationTests/OllamaAgentTests.cs`
- Recommended slice: First decide whether `OllamaAgent` should validate model output. Remove the unused dependency if validation is already handled by `ImageNameFormatter`; otherwise add explicit validation as a separate behavior feature.
- Suggested validation: `dotnet test ApplicationTests\ApplicationTests.csproj -v minimal`; add tests for invalid model-generated names before changing behavior.

### P-004: Simplify CLI Parsing and Command Orchestration

- Source findings: F-001, F-002, F-010, F-015
- Priority: P2
- Risk: medium
- Expected behavior impact: behavior unchanged
- Scope: `Console/ImageRenameOptionsParser.cs`, `Console/ImageRenameCli.cs`, `ApplicationTests/ImageRenameOptionsTests.cs`, `ApplicationTests/ImageRenameCliTests.cs`
- Recommended slice: Centralize naming aliases first, then split parsing helpers and composition-root construction in separate small changes.
- Suggested validation: `dotnet test ApplicationTests\ApplicationTests.csproj -v minimal`; manually run `dotnet run --project Console\Console.csproj -- --help`.

### P-005: Consolidate Filesystem Test Utilities

- Source findings: F-007, F-014
- Priority: P2
- Risk: low
- Expected behavior impact: behavior unchanged
- Scope: `TestsShared/TemporaryWorkingDirectory.cs`, `ApplicationTests/FileOperatorTests.cs`, `ApplicationTests/ImageRenameRunnerTests.cs`
- Recommended slice: Add a temp directory helper that does not mutate current directory, then replace repeated `CreateTempDirectory` and `try/finally` blocks.
- Suggested validation: `dotnet test ApplicationTests\ApplicationTests.csproj -v minimal`; verify temp roots are cleaned after test failures.

### P-006: Centralize Test Root and Fixture Discovery

- Source findings: F-006, F-008
- Priority: P3
- Risk: low
- Expected behavior impact: behavior unchanged
- Scope: `TestsShared/`, `E2ETests/E2ETestBase.cs`, `TestsShared/CliDriver.cs`
- Recommended slice: Add a shared repository-root resolver and update E2E setup and CLI driver to use it; remove unused `TestDataPath` state.
- Suggested validation: `dotnet test E2ETests\E2ETests.csproj -v minimal` after P-001 makes the suite deterministic.

### P-007: Clarify Filename Formatting Length Logic

- Source findings: F-003
- Priority: P3
- Risk: low
- Expected behavior impact: behavior unchanged
- Scope: `Application/ImageNameFormatter.cs`, `ApplicationTests/ImageNameFormatterTests.cs`
- Recommended slice: Add missing edge-case tests around separator choice and truncation, then extract named helpers from `EnforceMaxLength`.
- Suggested validation: `dotnet test ApplicationTests\ApplicationTests.csproj -v minimal`.

### P-008: Reduce CLI Global State in Tests

- Source findings: F-013
- Priority: P3
- Risk: medium
- Expected behavior impact: behavior unchanged
- Scope: `Console/ImageRenameCli.cs`, `ApplicationTests/ImageRenameCliTests.cs`
- Recommended slice: Return a command result or inject output/exit handling so most CLI tests avoid global `Console` and `Environment.ExitCode` mutation.
- Suggested validation: `dotnet test ApplicationTests\ApplicationTests.csproj -v minimal`; preserve one process-level or integration-style check for actual exit-code wiring.

## Recommended Sequence

1. P-001: Make E2E CLI tests deterministic. This restores the broadest behavior gate and should happen before larger cleanup.
2. P-002: Remove test-only infrastructure stub from production. This is low risk and improves boundaries quickly.
3. P-003: Decide and fix model filename validation. Treat adding validation as a separate behavior-changing feature if that path is chosen.
4. P-005: Consolidate filesystem test utilities. This lowers test maintenance cost with little production risk.
5. P-006: Centralize test root and fixture discovery. Do this after test utilities are cleaner.
6. P-004: Simplify CLI parsing and orchestration. Keep alias centralization, parser cleanup, and composition-root extraction as separate commits.
7. P-007: Clarify filename formatting length logic.
8. P-008: Reduce CLI global state in tests.

Dependency or platform changes are not needed for the low-risk cleanup proposals. Any model dependency upgrade, SDK target change, or altered filename validation behavior should be planned as a separate future feature.

## Behavior Preservation Notes

- CLI proposals must preserve accepted invocation forms: `<file_path>`, `<directory_path>`, `--help`, `-h`, `/?`, `--model`, `--naming`, `--max-length`, and `--config`.
- CLI proposals must preserve current error behavior: invalid arguments print an error and usage, invalid config exits before rename construction, and directory rename failures produce a nonzero exit code.
- File handling proposals must preserve supported image filtering, recursive directory ordering, collision suffixes, Windows case-only rename behavior, and same-path no-op behavior.
- Model-provider proposals must preserve the current prompt contract unless explicitly planned as a behavior change.
- Test utility proposals must keep all file operations inside deterministic temp directories and must not touch real user files.

## Validation Notes

- Report review: every finding includes category, location, observation, impact, confidence, behavior impact, and next action.
- Report review: every proposal references at least one finding and includes priority, risk, expected behavior impact, scope, recommended slice, and validation expectations.
- Report review: suspected and likely dead-code findings are labeled by confidence rather than treated as automatic removals.
- Baseline build passed on 2026-07-08 with NU1900 warnings from an unavailable private NuGet feed.
- Baseline application tests passed on 2026-07-08: 40 passed, 0 failed.
- Baseline E2E tests failed on 2026-07-08: 5 failed because live CLI runs received HTTP 404 from the model endpoint.

## Review Checklist

- [x] Findings satisfy required data model fields.
- [x] Dead-code findings include confidence labels.
- [x] Proposals reference source finding IDs.
- [x] Proposals include priority, risk, expected behavior impact, and validation expectations.
- [x] Recommendations are split into reviewable follow-up slices.
- [x] Uncertainty is explicitly labeled instead of hidden.
