# Tasks: Punctuation-Free Generated Filenames

- [x] Add a punctuation validation helper and a single retry path in `OllamaAgent`.
- [x] Factor the retry prompt and conversation assembly so the exact messages sent to Ollama can be inspected in tests.
- [x] Add regression coverage using `TestData/Bellwether_Zootopia.webp`, a fake punctuation-heavy assistant response, and the follow-up remove-punctuation request.
- [x] Verify the retry path works against the local Ollama flow or the existing stubbed harness without introducing an infinite retry loop.
