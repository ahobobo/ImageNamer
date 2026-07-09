# Quickstart: WEBP Model Compatibility

## Prerequisites

- .NET 10 SDK installed.
- Repository restored and buildable from the project root.
- Existing test data available under `TestData/`.
- Ollama is not required for deterministic automated tests because the repository already uses test doubles for model behavior in automated coverage.

## Validate The Baseline

Run from the repository root:

```powershell
dotnet build ImageNamer.slnx -v minimal
dotnet test ApplicationTests\ApplicationTests.csproj -v minimal
dotnet test E2ETests\E2ETests.csproj -v minimal
```

Expected outcome: the current build and tests pass before implementation changes begin.

## Implement And Validate WEBP Success Path

1. Add the planned image conversion dependency to `Infrastructure/Infrastructure.csproj`.
2. Update the file-reading path in `Infrastructure/ForReadingImages/` so WEBP input is converted to PNG in memory before model submission content is built.
3. Keep `ImageFile.Extension` and rename behavior tied to the original source file.
4. Add or update unit tests covering:
   - WEBP input produces `image/png` model payload metadata
   - non-WEBP inputs keep existing MIME behavior
   - rename output still preserves `.webp`
5. Add or update integration/end-to-end coverage for a valid WEBP rename path through the CLI boundary.

Expected outcome: a valid `.webp` file can be renamed successfully without creating extra files on disk.

## Validate Invalid WEBP Handling

1. Add a deterministic invalid WEBP test fixture or synthetic invalid-byte test case.
2. Verify the file-reading preparation step throws a clear failure before model submission.
3. Verify single-file CLI handling exits non-zero.
4. Verify directory processing continues after an invalid WEBP file when later files are valid.

Expected outcome: invalid WEBP input leaves the source file untouched, reports a clear error, and preserves current batch continuation behavior.

## Final Verification Commands

Run the full validation set after implementation:

```powershell
dotnet build ImageNamer.slnx -v minimal
dotnet test ApplicationTests\ApplicationTests.csproj -v minimal
dotnet test E2ETests\E2ETests.csproj -v minimal
```

Expected outcome:

- Build succeeds.
- Application tests cover conversion, MIME selection, and rename semantics.
- End-to-end tests verify CLI-visible behavior for WEBP success and batch failure continuation.
