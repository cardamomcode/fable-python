# Fable.Python: F# Advent 2025

> Write F#, run Python - a practical guide to Fable.Python

This is a comprehensive guide to [Fable.Python](https://github.com/fable-compiler/Fable.Python/), written as literate F# that transpiles to Python and generates its own documentation.

## Chapters

1. **Introduction** - What is Fable.Python and why use it
2. **Getting Started** - Setup, your first project, hello world
3. **Bindings** - Python interop and type bindings
4. **Compatibility** - Supported F# features and limitations

## The Meta Twist

This guide is self-documenting: each chapter is an `.fs` file with embedded Markdown comments using FSharp.Formatting conventions. **Fabletext**, a literate converter inspired by [jupytext](https://github.com/mwouts/jupytext) (also written in F# and transpiled via Fable.Python), processes these files to generate the final Markdown output.

## Quick Start

```bash
# Install dependencies
just setup
just restore
npm install

# Build everything (F# → Python → Markdown)
just all

# Or generate just the blogpost
just blogpost
```

## Available Commands

```bash
just setup      # Install Fable and Python dependencies (uv)
just restore    # Restore NuGet packages
just build      # Compile F# to Python with Fable
just generate   # Convert chapters to individual markdown files
just blogpost   # Generate concatenated blogpost.md for publishing
just format     # Format Python files with ruff
just lint       # Lint Python (ruff) and Markdown (markdownlint)
just all        # Full pipeline: restore, build, generate, format, lint
just clean      # Remove generated files
```

## Project Structure

```text
chapters/
├── 01-introduction.fs      # What is Fable.Python
├── 02-getting-started.fs   # Setup and first project
├── 03-bindings.fs          # Python interop
└── 04-compatibility.fs     # F# feature support
tools/
├── fabletext.fs            # Fabletext converter (F#)
└── fabletext.fsproj
output/
├── chapters/               # Generated Python from chapters
└── tools/
    └── fabletext.py        # Generated converter (Python)
docs/
├── 01-introduction.md      # Individual chapter docs
├── ...
└── blogpost.md             # Concatenated for Hashnode
```

## Technology Stack

- **Fable 5** (alpha) - F# to Python compiler
- **uv** - Python dependency management
- **just** - Command runner
- **ruff** - Python formatter/linter
- **markdownlint** - Markdown linter

## CI/CD

GitHub Actions automatically:

- Builds all F# to Python
- Generates the blogpost
- Opens a PR when content changes

## Resources

- [Fable](https://fable.io/)
- [Fable.Python Documentation](https://fable.io/docs/getting-started/python.html)
- [Fable.Python GitHub](https://github.com/fable-compiler/Fable.Python/)

---

*Part of [F# Advent 2025](https://sergeytihon.com/fsadvent/)*
