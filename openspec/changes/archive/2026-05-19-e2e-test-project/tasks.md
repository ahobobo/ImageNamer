## 1. Shared Infrastructure Setup

- [x] 1.1 Create the `TestsShared` project directory.
- [x] 1.2 Create `TestsShared.csproj` as a Class Library.
- [x] 1.3 Move `TemporaryWorkingDirectory` from `ApplicationTests` to `TestsShared`.
- [x] 1.4 Update `ApplicationTests` to reference `TestsShared`.
- [x] 1.5 Register `TestsShared` in the solution file (`ImageNamer.slnx`).

## 2. E2E Project Setup

- [x] 2.1 Create the `E2ETests` project directory.
- [x] 2.2 Create `E2ETests.csproj` with NUnit and a reference to `TestsShared`.
- [x] 2.3 Register `E2ETests` in the solution file (`ImageNamer.slnx`).

## 3. Test Infrastructure

- [x] 3.1 Implement `CliDriver` in `TestsShared` or `E2ETests` to execute the `Console` application.
- [x] 3.2 Implement a base E2E test fixture that uses `TemporaryWorkingDirectory` to prepare `TestData`.

## 4. E2E Test Cases

- [x] 4.1 Implement `RenameSingleImageTests` to verify renaming a single file using `--naming snake`.
- [x] 4.2 Implement `RecursiveRenamingTests` to verify renaming the entire `TestData` directory.
- [x] 4.3 Implement tests for multiple naming conventions (Snake, Pascal, etc.).

## 5. Final Validation

- [x] 5.1 Run all tests (Unit + E2E) and verify they pass.
- [x] 5.2 Verify that `ApplicationTests` still works correctly after the refactoring.
- [x] 5.3 Confirm `TestData` remains pristine after test runs.
