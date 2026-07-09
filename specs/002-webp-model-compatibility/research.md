# Research: WEBP Model Compatibility

## Decision: Use stable `SkiaSharp` 4.150.0

**Rationale**: The current NuGet package page for `SkiaSharp` shows 4.150.0 as the latest stable package version and marks newer availability only as prerelease. The repository targets `net10.0`, which the package page lists as compatible, so the dependency fits the current project target without version downgrades.

**Alternatives considered**: Using a prerelease SkiaSharp build was rejected because the feature does not need preview APIs. Choosing a different image library was rejected because the requested direction is SkiaSharp and the required decode/encode operations are already supported by its official API surface.

## Decision: Convert WEBP to PNG by decoding bytes into `SKBitmap` and encoding back to PNG in memory

**Rationale**: Official SkiaSharp API documentation exposes `SKBitmap.Decode(...)` overloads for encoded image bytes and `SKBitmap.Encode(..., SKEncodedImageFormat.Png, ...)` overloads for PNG output. That gives a direct in-memory path from source WEBP bytes to PNG bytes with no intermediate file writes, which matches the feature requirement exactly.

**Alternatives considered**: Converting on disk was rejected because the spec forbids creating extra files during the rename flow. Pushing raw WEBP through unchanged was rejected because the feature exists specifically to handle model compatibility gaps.

## Decision: Keep conversion in `Infrastructure/ForReadingImages`

**Rationale**: `FileOperator.ReadFile` is already the boundary where the app validates file existence, confirms supported extensions, reads bytes, and builds the `ImageFile` record used downstream. Extending that boundary to prepare model-ready content keeps codec concerns out of `Application/ImageRenamer`, `Console`, and the transport layer while preserving the existing orchestration flow.

**Alternatives considered**: Converting in `OllamaChatTransport` was rejected because transport should package model content, not infer source-file compatibility rules. Adding conversion logic to the application layer was rejected because it would leak infrastructure-specific image processing into orchestration code.

## Decision: Preserve original file identity while allowing model-submission content to differ

**Rationale**: The rename flow uses `ImageFile.Extension` and `ImageFile.Path` to preserve the source file type and rename the original file on disk. For WEBP inputs, only `Base64Content` and `MimeType` need to reflect the converted PNG representation sent to the model. This keeps the user-visible rename result aligned with the original file.

**Alternatives considered**: Changing the file extension to `.png` after conversion was rejected because it would change on-disk semantics and violate the specification. Introducing a second application-layer record solely for the conversion step was rejected because the current record already carries both source identity and model payload fields.

## Decision: Treat invalid WEBP decode as a per-file preparation failure

**Rationale**: If SkiaSharp cannot decode the source bytes into a bitmap, the system should fail before model submission, leave the source file untouched, and let existing batch-processing behavior report the error and continue. This matches the current CLI behavior for per-file exceptions in directory runs.

**Alternatives considered**: Falling back to sending the unreadable WEBP bytes unchanged was rejected because it weakens failure clarity and makes behavior model-dependent. Silently skipping invalid WEBP files was rejected because users need explicit per-file failure reporting.

## Decision: Keep dependency scope narrow and revisit native-asset expansion only if Linux or macOS support becomes an active requirement

**Rationale**: The current repository is being developed on Windows, and the base `SkiaSharp` package lists `net10.0` compatibility on NuGet. The plan should add only the package required for the requested feature. If future runtime validation targets additional platforms, native-asset package choices can be revisited as a separate compatibility task.

**Alternatives considered**: Expanding the plan immediately to cover every possible platform-native asset combination was rejected because it is outside the requested behavior change. Avoiding SkiaSharp over native-asset concerns was rejected because it would block the requested feature without current evidence that the repository needs broader platform distribution right now.
