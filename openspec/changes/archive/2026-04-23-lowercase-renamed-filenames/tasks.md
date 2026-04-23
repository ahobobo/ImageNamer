# Tasks: Lowercase Renamed Filenames

- [x] Add filename lowercasing at the rename boundary so generated names are normalized before the file move.
- [x] Keep the extension lowercase as part of the final target filename.
- [x] Add tests covering mixed-case generated names, already-lowercase names, and rename behavior with the normalized result.
- [x] Verify the current CLI rename flow still succeeds after normalization.
