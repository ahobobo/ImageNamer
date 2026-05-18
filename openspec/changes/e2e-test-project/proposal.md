## Why

The project lacks comprehensive end-to-end tests that verify the integration of all components (CLI, application logic, infrastructure, and model interaction). Automated E2E tests will ensure that the tool correctly renames images across various formats and handles recursive directory structures as expected, providing a safety net for future changes.

## What Changes

- Add a new C# project named `TestsShared` to hold common test utilities.
- Add a new C# test project named `E2ETests`.
- Implement a test suite that executes the `ImageNamer` CLI against a copy of the `TestData` directory.
- Move/Refactor `TemporaryWorkingDirectory` to `TestsShared` and use it across test projects.
- Cover single image renaming for all supported naming conventions.
- Cover directory renaming to verify automatic recursive processing.

## Capabilities

### New Capabilities
- `e2e-verification`: Automated verification of the full application flow, from CLI input to file system output.

### Modified Capabilities
None.

## Impact

- **New Projects**: `E2ETests` and `TestsShared` projects added to the solution.
- **Refactoring**: `TemporaryWorkingDirectory` moved from `ApplicationTests` to `TestsShared`.
- **Build System**: Updated `ImageNamer.slnx` to include the new projects.
