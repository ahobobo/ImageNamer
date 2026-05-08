# Data Model: Configurable Naming

## ImageNamingPreferences

Represents the effective preferences used for a rename run.

Fields:

- `ModelName`: non-empty model identifier used for image-name generation.
- `NamingConvention`: one of `normal`, `snake`, `capitalized`, `pascal`, `kebab`.
- `MaxNameLength`: maximum length of the filename stem, excluding extension.

Validation rules:

- `ModelName` must not be empty or whitespace.
- `NamingConvention` must be one of the supported values.
- `MaxNameLength` must be high enough to produce a useful valid filename stem.
- Built-in defaults are `gemma4:e2b`, `normal`, and `20`.

## ProjectLocalConfig

Represents the optional saved project-local configuration.

Fields:

- `ModelName`: optional saved model preference.
- `NamingConvention`: optional saved convention preference.
- `MaxNameLength`: optional saved maximum stem length.

Validation rules:

- Missing project-local config is valid and resolves to built-in defaults.
- Malformed JSON blocks the run before overrides are applied.
- Any present invalid value blocks the run before overrides are applied.
- Valid saved values override built-in defaults.

## RunOverrides

Represents optional per-run preference values supplied by the user.

Fields:

- `ModelName`: optional model override.
- `NamingConvention`: optional convention override.
- `MaxNameLength`: optional maximum stem length override.

Validation rules:

- Invalid override values block the run before any rename occurs.
- Overrides apply only after built-in defaults and valid project-local config.
- Overrides do not modify saved project-local config.

## PreferenceSource

Represents where an effective preference came from.

Values:

- `BuiltInDefault`
- `ProjectLocalConfig`
- `RunOverride`

Rules:

- Precedence is built-in defaults, then project-local config, then run overrides.
- Source tracking is useful for diagnostics and tests but does not need to be user-visible unless an error occurs.

## NamingConvention

Represents a deterministic conversion from generated model text to filename stem.

Values:

- `normal`: preserves model casing, spaces, and filesystem-valid punctuation; removes invalid filename characters and duplicate extension text.
- `snake`: joins words with underscores and lowercases where casing applies.
- `capitalized`: joins words with spaces and capitalizes each word where casing applies.
- `pascal`: concatenates words and capitalizes each word where casing applies.
- `kebab`: joins words with hyphens and lowercases where casing applies.

Rules:

- The original image extension is always preserved.
- Max-length shortening removes trailing words until the stem fits.
- A single overlong remaining word is hard-truncated to fit.
- The final filename must be valid before rename is attempted.

## GeneratedImageName

Represents model-provided text before deterministic formatting.

Fields:

- `Text`: raw generated name from the model response.
- `OriginalExtension`: extension from the source image.

Validation rules:

- Empty or whitespace output is invalid.
- Duplicate extension text may be removed during formatting.
- Unsafe output that cannot become a valid final filename is reported as an error.

## FinalImageFilename

Represents the safe filename used for the filesystem rename.

Fields:

- `Stem`: formatted name without extension.
- `Extension`: original image extension.
- `FullName`: stem plus extension.

Validation rules:

- `Extension` matches the original image extension.
- `Stem` satisfies the selected naming convention and max-length rule.
- `FullName` is safe for the current filesystem.
- Collisions are resolved without overwriting existing files.
