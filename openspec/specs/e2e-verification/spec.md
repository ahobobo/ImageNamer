# e2e-verification Specification

## Purpose
TBD - created by archiving change e2e-test-project. Update Purpose after archive.
## Requirements
### Requirement: Test Environment Isolation
The E2E test suite SHALL create a fresh copy of the `TestData` directory for each test execution to ensure isolation and prevent side effects between tests.

#### Scenario: Successful Test Data Reset
- **WHEN** a test starts
- **THEN** a temporary directory is created containing the original structure and files from `TestData`
- **WHEN** the test finishes
- **THEN** the temporary directory is deleted or restored to its original state

### Requirement: Single Image Renaming Verification
The system SHALL accurately rename a single image file when targeted directly, following the specified naming preferences.

#### Scenario: Rename single image with snake_case naming
- **WHEN** the CLI is executed targeting `TestData/Bellwether_Zootopia.webp` with `--naming snake`
- **THEN** the file is renamed based on its content analysis using snake_case
- **AND** the original file no longer exists at the old path

### Requirement: Recursive Directory Renaming Verification
The system SHALL accurately rename all images within a directory and its subdirectories when the directory is targeted, as the program is recursive by default.

#### Scenario: Rename directory recursively
- **WHEN** the CLI is executed targeting the `TestData` directory
- **THEN** all images in `TestData` and `TestData/recursive/nested` are renamed based on their content
- **AND** the directory structure is preserved

