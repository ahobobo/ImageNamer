# Design: Recursive Directory Image Rename

## Overview
Extend the CLI so it can distinguish between a single file input and a directory input.

When the input is a file, the existing behavior remains unchanged.
When the input is a directory, the CLI should:
- Enumerate files recursively
- Filter to supported image extensions
- Call the existing rename flow once per image
- Continue processing remaining files if one rename fails, while surfacing the failure

## CLI Behavior
Support a directory argument such as:

```text
<executable> <directory_path>
```

The command should resolve the path and branch on:
- file input: rename that file
- directory input: walk the tree and rename matching images

## Traversal Rules
- Use recursive directory enumeration
- Skip non-image files
- Preserve the original directory structure
- Process images sequentially so file operations are predictable

## Error Handling
- If the path does not exist, fail fast with a clear message
- If a directory contains unsupported files, skip them silently
- If an individual image rename fails, report the error and continue with the next image

## Implementation Notes
- Keep traversal logic in the CLI or a small application service so the existing image rename use case remains focused on one file at a time
- Reuse the current `ImageRenamer` flow for each discovered image
- Add or adjust file-system tests around recursive discovery and sequential rename behavior
