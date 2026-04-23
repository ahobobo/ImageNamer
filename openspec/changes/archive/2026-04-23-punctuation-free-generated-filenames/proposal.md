# Proposal: Punctuation-Free Generated Filenames

## What
Add a validation and retry step around model-generated filenames. If the model returns punctuation in the candidate name, the agent will send one corrective follow-up message that explicitly asks for the same name without punctuation, while preserving the original system instructions, filename context, and image payload.

Add a regression test that uses `TestData/Bellwether_Zootopia.webp` and builds a fake chat transcript containing:
- the system instructions
- the image as base64
- a fake assistant response containing punctuation
- a follow-up request that tells the model to remove punctuation

## Why
The current prompt already asks the model to avoid punctuation, but prompt text alone is not a hard guarantee. A validation check at the application boundary makes the filename rule enforceable and gives the model one chance to correct itself before the rename happens.

## Goals
- Reject model outputs that contain punctuation characters
- Retry once with a direct remove-punctuation reminder
- Keep the existing image-based naming workflow intact
- Cover the retry path with a deterministic regression test using the test image fixture

## Non-Goals
- Changing the descriptive naming style beyond punctuation removal
- Slugifying, transliterating, or otherwise rewriting valid names
- Allowing unlimited retries or fallback name generation
- Replacing the local Ollama adapter

## User Impact
Users will still get descriptive names, but punctuation-heavy model outputs will be corrected before the file is renamed.
