# Feature Specification: WEBP Model Compatibility

**Feature Branch**: `[002-webp-model-compatibility]`

**Created**: 2026-07-09

**Status**: Draft

**Input**: User description: "$speckit-specify we need to detect and convert webp images to png before sending them over to ollama. when we find a webp we need to convert it to png in memory, send that over to the model, then continew the normal rename flow. i am thinking we will use SkiaSharp for the image conversion."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Rename WEBP Images Successfully (Priority: P1)

As a user renaming image files, I want WEBP images to go through the same model-assisted rename flow as other supported image types so they can be renamed successfully even when the model input requires a different image format.

**Why this priority**: This is the core user-visible outcome. Without it, supported WEBP files can fail before the naming step completes.

**Independent Test**: Can be fully tested by submitting a WEBP image through the existing rename entrypoint and verifying that the model receives a usable image, a descriptive new filename is returned, and the source file is renamed successfully.

**Acceptance Scenarios**:

1. **Given** a valid WEBP image file, **When** the user runs the rename command, **Then** the system submits a model-compatible image representation and completes the normal rename flow for that file.
2. **Given** a valid WEBP image file, **When** the model returns a new descriptive name, **Then** the source file is renamed using that name while keeping the original file type on disk.

---

### User Story 2 - Preserve Existing Behavior for Other Images (Priority: P2)

As a user renaming mixed image types, I want non-WEBP images to continue through the current rename flow unchanged so this compatibility improvement does not introduce regressions for already working formats.

**Why this priority**: The change is intended to expand compatibility, not alter stable behavior for other supported files.

**Independent Test**: Can be tested by running the rename command on supported non-WEBP images and verifying that the model request and rename outcome match current behavior.

**Acceptance Scenarios**:

1. **Given** a supported non-WEBP image file, **When** the user runs the rename command, **Then** the system uses the existing model submission path without additional format conversion requirements.
2. **Given** a directory containing both WEBP and non-WEBP images, **When** the user runs recursive renaming, **Then** each file is processed according to its type and the run continues with the existing per-file reporting behavior.

---

### User Story 3 - Fail Clearly on Invalid WEBP Input (Priority: P3)

As a user renaming image files, I want invalid or unreadable WEBP files to fail with a clear error for that file so I understand why the rename did not happen and other files can still be processed where applicable.

**Why this priority**: Compatibility work must keep failure handling predictable and prevent silent corruption or misleading rename results.

**Independent Test**: Can be tested by submitting an unreadable or malformed WEBP file and verifying that no rename occurs for that file, the error is surfaced clearly, and batch processing continues according to current behavior.

**Acceptance Scenarios**:

1. **Given** a malformed or unreadable WEBP image, **When** the system prepares the model request, **Then** the file is not renamed and the user receives a clear failure message for that file.
2. **Given** a batch rename run with one invalid WEBP image and other valid images, **When** processing continues, **Then** the invalid file reports an error and the remaining files still complete their own rename attempts.

### Edge Cases

- A WEBP file may be present with the expected extension but contain invalid image data; the system must reject it without renaming the file.
- A WEBP file may convert successfully for model submission while the later rename step still fails due to naming or filesystem constraints; the existing rename error handling must still apply.
- A recursive rename run may include a mix of WEBP, PNG, JPEG, GIF, and BMP images; format-specific preparation must not change per-file ordering or reporting behavior.
- The model may return a descriptive filename with a different extension than the source file; the rename result must preserve the source file's original extension rules.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST detect when a selected supported image file is a WEBP image before constructing the model request.
- **FR-002**: The system MUST prepare a model-compatible PNG representation for WEBP images before sending image data to the model.
- **FR-003**: The system MUST keep WEBP-to-PNG preparation limited to model submission and MUST continue treating the source file as a WEBP file for subsequent rename operations.
- **FR-004**: The system MUST NOT persist an additional converted image file as part of the normal rename flow.
- **FR-005**: The system MUST preserve the existing model submission behavior for supported non-WEBP image types.
- **FR-006**: The system MUST continue the existing rename flow after successful WEBP model submission, including filename sanitization, naming-convention formatting, and file rename behavior.
- **FR-007**: If WEBP image preparation fails, the system MUST stop processing that file before model submission, MUST NOT rename the file, and MUST surface a clear error for that file.
- **FR-008**: In multi-file or recursive runs, a WEBP preparation failure for one file MUST NOT prevent independent processing of later files under the existing batch error-handling behavior.
- **FR-009**: The system MUST preserve the original source file extension in the final renamed file unless a separate feature changes extension-handling rules.
- **FR-010**: The system MUST keep the user-facing command surface and normal success output unchanged except where an error message is required for WEBP preparation failure.

### Key Entities

- **Source Image File**: A supported image selected by the user for renaming, including its original path, extension, and binary content.
- **Model Submission Image**: The image representation sent to the naming model for analysis, which may differ from the source image format when compatibility preparation is required.
- **Rename Result**: The final renamed file outcome, including the proposed name, preserved source extension, and any per-file success or failure reporting.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of valid WEBP images submitted through the rename command reach the naming model using a model-compatible image representation.
- **SC-002**: 100% of successful WEBP rename operations preserve the original source file extension in the renamed output.
- **SC-003**: 100% of supported non-WEBP image rename scenarios continue to pass their existing behavior checks after the feature is introduced.
- **SC-004**: In a mixed batch containing valid and invalid WEBP files, each invalid WEBP file produces a clear per-file failure while valid files continue processing under the current batch behavior.
- **SC-005**: Users can rename a valid WEBP image through the standard CLI flow without any additional command options or manual preprocessing steps.

## Test Expectations *(mandatory)*

- **TE-001**: Unit tests must cover format detection, model-submission preparation, and extension-preservation behavior for WEBP and non-WEBP inputs.
- **TE-002**: Integration-level tests must cover a successful WEBP rename flow from file read through rename completion.
- **TE-003**: Failure-path tests must cover malformed or unreadable WEBP input and verify that the file is not renamed and the error is surfaced clearly.
- **TE-004**: Batch-processing tests must verify that one WEBP preparation failure does not stop later files from being processed.

## Assumptions

- WEBP remains a supported user input type for the CLI, and this feature is intended to improve compatibility with the model submission path rather than remove WEBP support.
- The current naming instructions, naming-convention formatting, and file rename rules remain in scope and unchanged.
- Converted model-submission image data is temporary request data only and is not intended to appear as a new file in the user's directory.
- Existing per-file success and failure reporting behavior for recursive and batch processing remains the baseline behavior for this feature.
