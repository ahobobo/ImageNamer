# Feature Specification: Configurable Naming

**Feature Branch**: `001-configurable-naming`  
**Created**: 2026-05-08  
**Status**: Draft  
**Input**: User description: "lets go with option 3 of the hybrid approach and also implementing all the refactors. lets also add a \"normal\" naming convention that simply names the image whatever the llm returns."

## Clarifications

### Session 2026-05-08

- Q: Where should saved configuration live for this feature? -> A: Project-local saved config only, with per-run overrides.
- Q: What built-in defaults should apply when no saved config or overrides are present? -> A: Model `gemma4:e2b`, convention `normal`, max name length `20`.
- Q: How should the normal naming convention clean model output? -> A: Preserve model casing, spaces, and filesystem-valid punctuation; remove only invalid filename characters and duplicate extension text.
- Q: Can per-run overrides rescue malformed or invalid project-local configuration? -> A: No. Any malformed or invalid project-local configuration blocks the run before overrides are applied.
- Q: How should names be shortened when they exceed the maximum name length? -> A: Remove trailing words until within the limit; hard-truncate only a single overlong word.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Configure Naming Defaults (Priority: P1)

As a user who renames batches of images repeatedly, I want to save default model and naming preferences so that future runs use my preferred behavior without re-entering every setting.

**Why this priority**: Persistent defaults provide the core value of configuration and reduce repeated command entry for normal use.

**Independent Test**: Can be fully tested by creating a configuration with model, naming convention, and maximum length values, running the app against a supported image, and verifying the run uses those configured preferences.

**Test Boundary**: Exercise real command behavior, preference resolution, name formatting, and rename decision logic. External model responses and filesystem mutation may use controlled fakes.

**Acceptance Scenarios**:

1. **Given** a valid configuration that selects a model, naming convention, and maximum name length, **When** the user renames an image without override options, **Then** the app uses the configured model and formats the output according to the configured naming rules.
2. **Given** no project-local configuration exists, **When** the user renames an image, **Then** the app uses documented defaults and completes with the existing basic workflow.
3. **Given** project-local configuration is malformed or contains an unsupported naming convention or invalid maximum length, **When** the user starts a run, **Then** the app reports a clear configuration error before applying overrides and does not rename files.

---

### User Story 2 - Override Preferences Per Run (Priority: P2)

As a user testing different image naming models or conventions, I want to override saved defaults for a single run so that I can experiment without changing my long-term preferences.

**Why this priority**: Per-run overrides make configuration flexible and complete the hybrid behavior requested by the user.

**Independent Test**: Can be fully tested by setting saved defaults, running with explicit overrides, and verifying the override values win for that run only.

**Test Boundary**: Exercise real command behavior and preference precedence. External model calls and filesystem mutation may use fakes.

**Acceptance Scenarios**:

1. **Given** saved defaults specify one model and naming convention, **When** the user provides a different model and convention for a run, **Then** that run uses the provided values.
2. **Given** saved defaults remain unchanged after a run with overrides, **When** the user starts a later run without overrides, **Then** the saved defaults are used again.
3. **Given** the user provides an invalid override value, **When** the run starts, **Then** the app reports which value is invalid and does not rename files.

---

### User Story 3 - Choose Naming Conventions (Priority: P3)

As a user organizing images for different downstream systems, I want to choose the output naming convention so that renamed files match my target folder, website, or project standards.

**Why this priority**: Naming convention control is a key requested behavior, but it depends on the configuration and override mechanisms.

**Independent Test**: Can be fully tested by supplying the same generated descriptive name with each supported convention and verifying the resulting filename.

**Test Boundary**: Exercise real deterministic name formatting and length handling with external model and filesystem operations faked.

**Acceptance Scenarios**:

1. **Given** the model returns "red sunset beach photo", **When** the user selects snake case, **Then** the saved filename uses `red_sunset_beach_photo` before the extension.
2. **Given** the model returns "red sunset beach photo", **When** the user selects capitalized words, **Then** the saved filename uses `Red Sunset Beach Photo` before the extension.
3. **Given** the model returns "red sunset beach photo", **When** the user selects the `pascal` capitals-inside naming value, **Then** the saved filename uses `RedSunsetBeachPhoto` before the extension.
4. **Given** the model returns "red sunset beach photo", **When** the user selects the `kebab` separated-like-this naming value, **Then** the saved filename uses `red-sunset-beach-photo` before the extension.
5. **Given** the model returns "Red Sunset - Beach Photo!", **When** the user selects normal naming, **Then** the saved filename preserves the model casing, spaces, and filesystem-valid punctuation.

---

### User Story 4 - Limit Name Length (Priority: P4)

As a user who syncs files across tools with filename limits, I want to set a maximum generated name length so that renamed images stay within my preferred limit.

**Why this priority**: Length control is requested and important, but it is useful only after conventions can be selected.

**Independent Test**: Can be fully tested by configuring a maximum length, providing a generated name longer than that limit, and verifying the final filename fits the limit while preserving its extension.

**Test Boundary**: Exercise real deterministic name formatting and validation with external model and filesystem operations faked.

**Acceptance Scenarios**:

1. **Given** a maximum name length is configured, **When** the formatted name would exceed the limit, **Then** the app removes trailing words until the name portion fits the configured maximum.
2. **Given** shortening is required, **When** the final name is produced, **Then** the extension is preserved and the final name remains valid for the selected naming convention.
3. **Given** the only remaining word is longer than the configured maximum, **When** shortening is required, **Then** the app hard-truncates that word to fit the configured maximum.
4. **Given** the configured maximum length is too short to produce a useful valid name, **When** the run starts, **Then** the app reports a clear validation error and does not rename files.

---

### User Story 5 - Preserve Behavior During Cleanup (Priority: P5)

As a maintainer, I want the configuration and naming work to leave the existing rename workflow understandable and independently testable so that future changes can be made without unexpected regressions.

**Why this priority**: The requested refactors increase maintainability, but they should preserve user-facing behavior while the new feature is added.

**Independent Test**: Can be fully tested by running existing rename scenarios and new configuration scenarios, confirming existing supported file handling, recursive directory processing, collision behavior, and error continuation still work.

**Test Boundary**: Exercise real application behavior with external model calls and destructive filesystem changes controlled by fakes or temporary test locations.

**Acceptance Scenarios**:

1. **Given** a directory contains supported and unsupported files, **When** the user runs the app with default settings, **Then** supported images are processed recursively and unsupported files are skipped as before.
2. **Given** one image fails during a directory run, **When** other images remain, **Then** the app continues processing the remaining images and reports the failure as before.
3. **Given** a generated target name already exists, **When** the app renames an image, **Then** the app resolves the collision without overwriting another file.

### Edge Cases

- Missing project-local configuration is treated as default settings, not an error.
- Malformed or invalid project-local configuration prevents the run before overrides are applied and identifies the configuration problem.
- Per-run overrides with unknown convention names, empty model names, or invalid length values prevent the run before any rename occurs.
- The selected maximum length must account for the name portion while preserving the original file extension.
- Names exceeding the selected maximum length are shortened by removing trailing words until the name fits; a single overlong word is hard-truncated only when no shorter word-boundary result is possible.
- The normal naming convention may receive punctuation, mixed case, repeated spaces, or extension text from the model; the final filename must preserve filesystem-valid punctuation and casing while removing invalid filename characters and duplicate extension text.
- If two configured or formatted names collide in the same directory, the app must still create unique filenames without overwriting existing files.
- Configuration or override behavior must not change which image extensions are supported.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST support project-local saved user preferences for model name, naming convention, and maximum name length.
- **FR-002**: The system MUST support per-run overrides for model name, naming convention, and maximum name length.
- **FR-003**: The system MUST resolve preferences using this precedence: documented built-in defaults first, saved configuration second, per-run overrides last.
- **FR-004**: The system MUST document all supported preference names, accepted values, defaults, and examples.
- **FR-005**: The system MUST validate configuration and override values before renaming any files.
- **FR-005a**: Any malformed or invalid project-local configuration MUST block the run before per-run overrides are applied.
- **FR-006**: The system MUST allow users to select the model used for image-name generation.
- **FR-006a**: The built-in defaults MUST be model `gemma4:e2b`, naming convention `normal`, and maximum name length `20`.
- **FR-007**: The system MUST support these canonical CLI/config naming values: `normal`, `snake`, `capitalized`, `pascal`, and `kebab`. User-facing descriptions MAY describe `pascal` as capitals-inside naming and `kebab` as separated-like-this naming.
- **FR-008**: The normal naming convention MUST preserve model-provided casing, spaces, and filesystem-valid punctuation while removing invalid filename characters, duplicate extension text, and any content that would prevent preserving the original image extension.
- **FR-009**: Snake case MUST join words with underscores and use lowercase letters where casing is applicable.
- **FR-010**: Capitalized words MUST separate words with spaces and capitalize each word where casing is applicable.
- **FR-011**: Capitals-inside naming MUST concatenate words without separators and capitalize each word where casing is applicable.
- **FR-012**: Separated-like-this naming MUST join words with hyphens and use lowercase letters where casing is applicable.
- **FR-013**: The system MUST allow users to set a maximum name length for the name portion of the generated filename.
- **FR-013a**: When the formatted name exceeds the configured maximum, the system MUST remove trailing words until the name fits, and MUST hard-truncate only when a single remaining word still exceeds the limit.
- **FR-014**: The system MUST preserve the original file extension when applying naming convention and maximum length rules.
- **FR-015**: The system MUST ensure the final filename is valid for the current file system before attempting a rename.
- **FR-016**: The system MUST continue to support single-file input and recursive directory input.
- **FR-017**: The system MUST preserve existing behavior for unsupported files, missing paths, per-file failures, and filename collisions unless explicitly changed by the new preferences.
- **FR-018**: The system MUST present clear user-facing errors for invalid configuration, invalid overrides, unavailable required values, and unsafe generated names.
- **FR-019**: The system MUST keep model selection, preference resolution, name formatting, filename validation, and file renaming independently testable.
- **FR-020**: The system MUST remove or revise unused or misleading app entry points and naming responsibilities that conflict with the configured naming behavior, while preserving the documented user workflow.

### Key Entities *(include if feature involves data)*

- **Image Naming Preferences**: Project-local user-selected settings that influence model choice and final filename shape. Includes model name, naming convention, and maximum name length.
- **Preference Source**: The origin of a preference value. Values may come from built-in defaults, saved configuration, or per-run overrides.
- **Naming Convention**: A named rule for converting generated descriptive text into a final filename stem.
- **Generated Image Name**: The model-provided name text before final safety cleanup, convention formatting, length enforcement, and extension preservation.
- **Final Image Filename**: The safe filename used for renaming, including the original image extension.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A user can configure model, naming convention, and maximum name length once and complete a later rename run without re-entering those settings.
- **SC-002**: A user can override all configurable preferences for a single run in under 30 seconds when they already know the values they want.
- **SC-003**: For the same generated name, all five supported naming conventions produce predictable, documented filename stems in 100% of formatting tests.
- **SC-004**: In 100% of successful rename operations, the final filename preserves the original image extension.
- **SC-005**: Invalid configuration or override values are reported before any file is renamed in 100% of validation tests.
- **SC-006**: Existing supported workflows for single-file runs, recursive directory runs, collision handling, unsupported-file skipping, and per-file failure continuation remain covered by automated tests.
- **SC-007**: Maintainers can test preference resolution and name formatting without requiring a running model service or permanent filesystem changes.

## Assumptions

- Built-in defaults are model `gemma4:e2b`, naming convention `normal`, and maximum name length `20`.
- The maximum name length applies to the filename stem only, not the extension.
- The minimum acceptable maximum length will be defined during planning so that every convention can produce a useful valid name.
- Saved configuration is project-local. User-profile configuration and multi-user permission management are out of scope.
- Configuration management covers reading and applying preferences, not an interactive setup wizard.
- Network availability, model installation, and model service health remain external prerequisites for real model-backed runs.
