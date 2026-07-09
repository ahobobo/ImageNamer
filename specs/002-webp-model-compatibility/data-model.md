# Data Model: WEBP Model Compatibility

## Source Image File

Represents the user-selected file that remains the system of record for rename behavior.

**Fields**:

- `name`: Current filename including extension.
- `extension`: Original source extension, such as `.webp` or `.png`.
- `path`: Full source file path on disk.
- `base64Content`: Encoded image payload that will be sent to the model after any compatibility preparation.
- `mimeType`: MIME type paired with `base64Content` for model submission.

**Validation Rules**:

- `extension` must remain the original source file extension even when `mimeType` changes for model compatibility.
- `path` must continue to point to the original file on disk.
- `base64Content` must contain the exact bytes intended for model submission, not a path to a temporary file.

## Model Submission Image

Represents the effective image payload delivered to Ollama for analysis.

**Fields**:

- `sourceExtension`: Original source extension used to decide whether compatibility preparation is required.
- `submissionMimeType`: MIME type sent to the model.
- `submissionBytes`: In-memory binary image payload represented in code as base64 before transport packaging.
- `conversionApplied`: Boolean indicating whether compatibility preparation changed the submission format.

**Validation Rules**:

- For non-WEBP inputs, `submissionMimeType` must match the source image type and `conversionApplied` must be false.
- For WEBP inputs that convert successfully, `submissionMimeType` must be `image/png` and `conversionApplied` must be true.
- `submissionBytes` must never be written to a new user-visible file during the rename flow.

## Conversion Result

Represents the outcome of WEBP model-preparation work before the model call proceeds.

**Fields**:

- `status`: Success or failure.
- `failureReason`: Human-readable error when conversion fails.
- `preservedSourceExtension`: Original extension retained for later rename behavior.

**Validation Rules**:

- On failure, model submission must not occur for that file.
- On failure, no rename must occur for that file.
- On success, `preservedSourceExtension` must remain equal to the original source extension.

## Rename Result

Represents the final observable file outcome after the model responds.

**Fields**:

- `originalPath`: Source path before rename.
- `renamedPath`: Final path after rename.
- `finalExtension`: Extension used on disk after rename.
- `reportedOutcome`: Success or per-file error message.

**Validation Rules**:

- `finalExtension` must equal the original source extension for this feature.
- A WEBP compatibility failure must produce an error outcome without changing `originalPath`.
- Mixed directory runs must allow multiple independent `Rename Result` records in one execution.

## Relationships

- One **Source Image File** produces one **Model Submission Image**.
- One **Model Submission Image** may require one **Conversion Result** before the model request is sent.
- One successful model request produces one **Rename Result**.
- One failed **Conversion Result** produces one error **Rename Result** without a model request or file rename.
