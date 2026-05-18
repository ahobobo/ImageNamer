## Context

The `ImageNamer` solution currently contains unit tests in `ApplicationTests` using NUnit. To ensure full system integrity, an end-to-end (E2E) test suite is required. This suite will exercise the `Console` application, which orchestrates the `Application` logic and `Infrastructure` adapters.

## Goals / Non-Goals

**Goals:**
- Create a `TestsShared` library project for common test infrastructure.
- Create a dedicated `E2ETests` project.
- Verify the CLI's behavior for single file and recursive directory renaming.
- Ensure tests are isolated using a shared `TemporaryWorkingDirectory` utility.
- Provide a repeatable way to verify integration with the Ollama model.

**Non-Goals:**
- Unit testing individual components.
- Performance benchmarking.

## Decisions

### 1. Test Project Type
We will use **NUnit** as the test runner, maintaining consistency with the existing `ApplicationTests` project.

### 2. Execution Strategy
The E2E tests will execute the `Console` project using `Process.Start` or `dotnet run`. This ensures we are testing the actual entry point and full dependency injection chain.

### 3. Test Infrastructure Sharing
We will create a `TestsShared` project to host:
- `TemporaryWorkingDirectory`: Moved from `ApplicationTests`.
- Common directory copy/cleanup logic.
- Potential CLI execution helpers.

Both `ApplicationTests` and `E2ETests` will reference `TestsShared`.

### 4. Test Data Management (Isolation)
We will leverage `TemporaryWorkingDirectory`:
- **Copy on Start**: Each test will copy the contents of `TestData/` to the path provided by `TemporaryWorkingDirectory`.
- **Redirect Targets**: The CLI commands will be issued against paths inside this temporary directory.
- **Cleanup on Finish**: `TemporaryWorkingDirectory.Dispose()` (invoked via NUnit's `TearDown`) handles cleanup.

### 5. External Dependencies (Ollama)
For a true E2E test, a running Ollama instance is expected. We target the real stack.

## Risks / Trade-offs

- **[Risk] Slow Execution**: Running the full stack is slow.
  - **Mitigation**: Keep E2E tests focused on integration.
- **[Risk] File System Flakiness**: Copying and deleting many files might be flaky on some systems.
  - **Mitigation**: Use robust directory copy/delete helpers with retry logic if necessary.
- **[Risk] Binary Availability**: Tests need the `Console` binary.
  - **Mitigation**: Use `dotnet run --project ../Console/Console.csproj -- <args>`.
