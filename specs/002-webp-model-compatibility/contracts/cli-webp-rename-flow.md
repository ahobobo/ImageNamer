# Contract: CLI WEBP Rename Flow

## Purpose

Define the observable contract for how the CLI handles WEBP inputs once model-compatibility conversion is introduced.

## Inputs

- A single supported image file path, including `.webp`
- A directory path containing supported image files, including `.webp`
- Existing CLI options such as `--model`, `--naming`, `--max-length`, and `--config`

## Successful Single WEBP Flow

1. The CLI accepts a `.webp` path exactly as it does today.
2. The file-reading boundary prepares model-submission content in memory so the model receives a PNG-compatible payload.
3. The model returns a filename suggestion.
4. The application formats the filename according to existing naming rules.
5. The filesystem rename step renames the original WEBP file on disk and preserves the `.webp` extension.
6. The CLI reports success using the existing success message shape:

   `Renamed <old-file-name> to <new-file-name>`

## Successful Non-WEBP Flow

1. The CLI behavior remains unchanged.
2. No format-conversion step is introduced for supported non-WEBP inputs.
3. Existing success output remains unchanged.

## Invalid WEBP Flow

1. The CLI accepts the file path.
2. If the WEBP bytes cannot be decoded into a valid image for conversion, the model request is not sent.
3. The original file is not renamed.
4. For a single-file run, the CLI exits with a non-zero exit code and reports the failure through the existing error channel.
5. For a directory run, the CLI reports a per-file error using the existing error message shape:

   `Error renaming <path>: <message>`

## Batch Flow Requirements

- File ordering remains the current deterministic path ordering.
- A failure preparing one WEBP file does not stop later files from being processed.
- Success and failure counts continue to drive the final exit code exactly as they do today.

## Out Of Scope

- Changing the file extension on disk from `.webp` to `.png`
- Adding new CLI options for conversion behavior
- Writing converted PNG files into the user's directory
- Changing how non-WEBP images are packaged for the model
