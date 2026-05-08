# CLI Contract: Configurable Naming

## Invocation

```text
ImageNamer <input_path> [options]
ImageNamer --help
ImageNamer -h
ImageNamer /?
```

`input_path` may be a supported image file or a directory containing supported images.

## Options

| Option | Value | Behavior |
|--------|-------|----------|
| `--model` | non-empty model name | Overrides the model for this run only |
| `--naming` | `normal`, `snake`, `capitalized`, `pascal`, `kebab` | Overrides naming convention for this run only |
| `--max-length` | positive integer above the minimum valid stem length | Overrides maximum filename stem length for this run only |
| `--config` | path to project-local config file | Uses the specified project-local config file for this run |

If `--config` is omitted, the app looks for `imagenamer.json` in the current working directory. Missing project-local config is not an error.

## Project-Local Config Shape

Default path: `imagenamer.json` in the current working directory.

```json
{
  "model": "gemma4:e2b",
  "naming": "normal",
  "maxLength": 20
}
```

All properties are optional. Present values must be valid.

## Preference Precedence

1. Built-in defaults: model `gemma4:e2b`, naming `normal`, max length `20`
2. Valid project-local config
3. Valid per-run overrides

Malformed or invalid project-local config blocks the run before per-run overrides are applied.

## Naming Values

| Value | Example model text | Filename stem |
|-------|--------------------|---------------|
| `normal` | `Red Sunset - Beach Photo!` | `Red Sunset - Beach Photo!` with invalid filename characters removed |
| `snake` | `red sunset beach photo` | `red_sunset_beach_photo` |
| `capitalized` | `red sunset beach photo` | `Red Sunset Beach Photo` |
| `pascal` | `red sunset beach photo` | `RedSunsetBeachPhoto` |
| `kebab` | `red sunset beach photo` | `red-sunset-beach-photo` |

## Error Behavior

- Missing input path: print usage and exit non-zero.
- Unknown option: print an option error and exit non-zero.
- Invalid override value: print the invalid option and exit non-zero before renaming.
- Malformed or invalid project-local config: print the config problem and exit non-zero before renaming.
- Missing project-local config: use built-in defaults.
- Per-file rename failure during directory processing: continue processing remaining files and return non-zero if any file failed.

## Help Output

Help output must list:

- File and directory input modes.
- Config file behavior.
- `--model`, `--naming`, `--max-length`, and `--config`.
- Supported naming values.
- Built-in defaults.
