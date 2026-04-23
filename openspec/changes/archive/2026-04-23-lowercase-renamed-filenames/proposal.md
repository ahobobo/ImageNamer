# Proposal: Lowercase Renamed Filenames

## What
Normalize the final filename produced by the rename flow so that every renamed file uses lowercase characters.

This should apply to the full target filename, including the extension.

## Why
Lowercase filenames are easier to sort, compare, and reuse across tooling. They also reduce inconsistent casing when the model or filesystem returns mixed-case names.

## Goals
- Ensure renamed files are written with lowercase names
- Preserve the existing rename workflow and file content behavior
- Keep the change focused on filename normalization rather than broader naming rules

## Non-Goals
- Changing how descriptive names are generated
- Removing spaces or punctuation beyond existing behavior
- Introducing new naming conventions such as slugifying or truncation

## User Impact
Users will get consistently lowercase renamed files without changing how they invoke the app or how image descriptions are generated.
