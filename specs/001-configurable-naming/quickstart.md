# Quickstart: Configurable Naming

## Validate The Feature

From the repository root:

```powershell
dotnet test .\ApplicationTests\ApplicationTests.csproj -v minimal
```

## Run With Built-In Defaults

If no project-local config exists, the app uses:

- Model: `gemma4:e2b`
- Naming: `normal`
- Max name length: `20`

```powershell
dotnet run --project .\Console\Console.csproj -- .\TestData
```

## Add Project-Local Config

Create `imagenamer.json` in the directory where the app is run:

```json
{
  "model": "gemma4:e2b",
  "naming": "snake",
  "maxLength": 40
}
```

Then run:

```powershell
dotnet run --project .\Console\Console.csproj -- .\TestData
```

## Override For One Run

```powershell
dotnet run --project .\Console\Console.csproj -- .\TestData --model gemma4:e2b --naming kebab --max-length 30
```

Overrides apply only to that run and do not change project-local config.

## Expected Naming Examples

For model text `red sunset beach photo`:

- `snake`: `red_sunset_beach_photo`
- `capitalized`: `Red Sunset Beach Photo`
- `pascal`: `RedSunsetBeachPhoto`
- `kebab`: `red-sunset-beach-photo`

For `normal`, preserve model casing, spaces, and filesystem-valid punctuation while removing invalid filename characters and duplicate extension text.

## Failure Checks

- Malformed project-local config must fail before any rename.
- Invalid project-local config must fail before overrides are applied.
- Invalid per-run override must fail before any rename.
- Names longer than the maximum remove trailing words first; a single overlong word is hard-truncated.
