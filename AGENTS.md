# Repository Guidelines

## Project Structure & Module Organization

This repository is a self-documenting Fable.Python guide. Author chapter content as literate F# in `chapters/*.fs`; each file mixes executable examples with Markdown in `(** ... *)` comments. Keep chapter order synchronized between `fable-python.fsproj` and the `chapters` variable in `justfile`.

`Fable.Literate/` contains the F# parser and Markdown generator. Its cross-target tests live in `Fable.Literate.Tests/`. Fable writes generated Python to `output/`, while documentation is generated under `docs/`; do not hand-edit generated files. `docs/blogpost.md` is the published, tracked aggregate.

## Build, Test, and Development Commands

Use the repository `justfile` as the command entry point:

- `just setup` restores local .NET tools and synchronizes Python dependencies with `uv`.
- `just restore` restores NuGet and npm dependencies.
- `just build` transpiles the chapters and converter to Python.
- `just generate` rebuilds individual chapter Markdown; `just blogpost` rebuilds `docs/blogpost.md`.
- `just watch` recompiles chapter sources during development.
- `just test-all` runs the Expecto suite on .NET and the Pyxpecto build on Python. Use `just test` or `just test-python` for one target.
- `just format` applies Fantomas to F# and Ruff to generated Python; `just lint` checks generated Markdown.
- `just all` runs the complete restore, generation, formatting, and lint pipeline, but not tests.

## Coding Style & Naming Conventions

Use four-space indentation and let Fantomas format `.fs` files according to `.editorconfig`. Follow F# conventions: PascalCase for modules, types, and chapter filenames; camelCase for values and functions. Keep examples compatible with both .NET and Fable.Python unless a chapter explicitly demonstrates target-specific interop. Use Ruff for generated Python and markdownlint for generated documentation.

## Testing Guidelines

Tests use Expecto on .NET and Fable.Pyxpecto on Python. Add focused cases to `Fable.Literate.Tests/Tests.fs`, group them in descriptive `testList` values, and name cases as behaviors, such as `"parses simple markdown block"`. Run `just test-all` before submitting changes to the converter or generation pipeline.

## Commit & Pull Request Guidelines

Recent history uses short, imperative Conventional Commit prefixes such as `feat:`, `fix:`, `docs:`, and `chore:`. Keep each commit scoped. Pull requests should summarize the source change, note generated documentation updates, and list validation commands. Include the rebuilt `docs/blogpost.md` when chapter output changes; link relevant issues and add screenshots only when rendered output needs visual review.
