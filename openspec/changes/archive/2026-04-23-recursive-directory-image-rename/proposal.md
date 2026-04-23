# Proposal: Recursive Directory Image Rename

## What
Add a new CLI option that accepts a directory path and renames all supported images found under that directory.

The command should:
- Traverse the directory recursively
- Find supported image files in subdirectories
- Rename images one at a time using the existing image renaming flow

## Why
The current CLI only handles a single image file. Bulk renaming a folder tree is a common use case, and recursive traversal makes the tool useful for organized photo libraries and nested image collections.

## Goals
- Accept a directory input from the CLI
- Process image files recursively
- Rename each image independently
- Reuse the existing rename pipeline as much as possible

## Non-Goals
- Parallel processing of multiple images
- Bulk rename previews or undo support
- Changing the image naming model itself
- Preserving original filenames as a fallback strategy

## User Impact
Users will be able to point the CLI at a folder and have every supported image renamed in place, including files nested in subdirectories.
