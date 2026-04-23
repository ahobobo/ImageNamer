# Design: Punctuation-Free Generated Filenames

## Overview
Add a bounded validation step in the Ollama rename flow. The agent will inspect the model output before finalizing the filename. If the returned name includes punctuation, the agent will append a corrective turn to the same conversation and ask the model to restate the filename without punctuation.

Keep the retry bounded to one correction attempt. That avoids infinite loops while still giving the model a chance to recover from a bad first response.

## Behavior
- Send the first request with the existing system instructions and the image base64 payload
- Read the streamed model response and assemble the candidate filename
- Validate the candidate for punctuation after trimming whitespace
- If punctuation is present, add a follow-up user message that says to remove punctuation and return only letters, numbers, and spaces
- Send the updated conversation to the local Ollama model once more
- If the second response still contains punctuation, fail fast instead of retrying again

## Validation Rule
Treat any non-letter, non-digit, non-space character in the model-produced name as invalid.

Keep the extension handling separate from the validation rule so the forced file extension does not trigger the punctuation check.

## Test Strategy
Add a regression test that uses `TestData/Bellwether_Zootopia.webp` and a fake chat conversation with these elements:
- the system instructions
- the image content encoded as base64
- a fake assistant response that includes punctuation
- a follow-up remove-punctuation request

The test should assert that the retry path appends the correction prompt before the second Ollama call. Keep it deterministic by stubbing the first response and capturing the outgoing transcript.

## Implementation Notes
- Reuse the existing `OllamaAgent` instructions string so the test matches the real prompt
- Factor the conversation assembly so the retry message can be inspected in tests
- Keep the forced `.webp` suffix handling independent from the punctuation guard
