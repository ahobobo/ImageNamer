# Design: Lowercase Renamed Filenames

## Overview
Add a normalization step in the rename pipeline so the final target filename is converted to lowercase before the file move happens.

The preferred implementation point is the boundary where the generated name becomes the file-system target name. That keeps the rule close to the actual rename behavior and avoids changing the model prompt unless a separate reason appears later.

## Behavior
- Read the source image as usual
- Generate the suggested filename as usual
- Normalize the final target name to lowercase
- Rename the file using the normalized name

## Case Handling
- Apply lowercase conversion to the full filename, not just the base name
- Preserve the file extension while ensuring it is also lowercase
- Avoid introducing any additional formatting rules beyond lowercase normalization

## Error Handling
- Keep the existing rename errors and file-not-found behavior unchanged
- If the normalized name would conflict with an existing file, surface the existing file-system error
- Do not silently rename to a different fallback name

## Implementation Notes
- Prefer a single normalization point so the behavior stays predictable
- Add tests that cover mixed-case generated names and already-lowercase names
- If the file-system layer needs a special path for case-only renames on Windows, handle that there rather than duplicating logic elsewhere
