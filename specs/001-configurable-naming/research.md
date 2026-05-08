# Research: Configurable Naming

## Decision: Use project-local JSON configuration

**Rationale**: The clarified spec requires project-local saved config only. JSON is human-readable, already common in .NET projects, and can be parsed with built-in platform APIs without adding dependencies. Keeping the file project-local makes test setup simple and keeps behavior visible next to the tool invocation.

**Alternatives considered**:

- User-profile config: rejected by clarification; user-profile state is out of scope.
- Both project and user-profile config: rejected by clarification and adds precedence complexity.
- Environment variables only: rejected because the feature requires saved project-local preferences.

## Decision: Use built-in .NET parsing and manual CLI option handling

**Rationale**: The feature needs a small set of flags and a single project-local config file. Standard .NET APIs are sufficient, and the constitution requires standard library first when it satisfies the need cleanly. No package additions, removals, or updates are planned.

**Alternatives considered**:

- Add a command-line parser package: rejected because the current option surface is small and does not justify dependency churn.
- Add a configuration framework package: rejected because project-local JSON plus overrides can be handled directly and testably.

## Decision: Model preference resolution as a deterministic application operation

**Rationale**: Built-in defaults, project-local config, and per-run overrides have strict precedence and validation behavior. Keeping this deterministic behavior outside external adapters makes it easy to test and avoids coupling user preference rules to Ollama or filesystem code.

**Alternatives considered**:

- Resolve preferences in infrastructure: rejected because preference precedence is business behavior, not an external adapter concern.
- Resolve preferences ad hoc in the CLI only: rejected because it makes test coverage and future reuse weaker.

## Decision: Format names after model output instead of relying only on prompts

**Rationale**: The model can be guided, but final naming conventions and max-length behavior must be predictable in 100% of formatting tests. Deterministic formatting after the model response satisfies the spec and keeps behavior testable without a model service.

**Alternatives considered**:

- Ask the model to produce final convention-specific filenames: rejected because model output is nondeterministic and harder to validate.
- Keep all cleanup in the filesystem adapter: rejected because final naming convention is application behavior, not filesystem behavior.

## Decision: Preserve existing package versions

**Rationale**: The feature changes configuration and deterministic naming behavior. Existing model transport packages remain adequate and no dependency changes are required.

**Alternatives considered**:

- Update `OllamaSharp` or `Microsoft.Extensions.AI.Abstractions`: rejected because the current feature does not require new package capabilities.
- Replace Ollama integration: rejected as outside scope.
