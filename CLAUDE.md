# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**F# Advent 2025 blog post project** demonstrating Fable.Python capabilities.

**Fabletext** is a literate programming converter (inspired by jupytext) written in F# that transpiles to Python via Fable.Python. It processes `.fs` files with embedded Markdown comments (FSharp.Formatting conventions) and outputs GitHub-flavored Markdown suitable for publishing on platforms like Hashnode.

**Key concept:** The project is self-documenting - the chapters and converter generate the blog post that documents how they work.

## Build Commands (using just)

```bash
just setup      # Install Fable and Python dependencies
just restore    # Restore NuGet packages
just build      # Build all chapters and tools to Python
just generate   # Generate individual markdown docs from chapters
just blogpost   # Generate concatenated blogpost.md for publishing
just format     # Format Python with ruff
just lint       # Lint Python (ruff) and Markdown (markdownlint)
just watch      # Watch mode for development
just clean      # Clean generated files
just all        # Full pipeline: restore, build, generate, format, lint
```

## Architecture

### Fabletext Parser State Machine

The converter uses a line-by-line state machine with three states:

- **InMarkdown**: Inside `(** ... *)` blocks - emit content as-is
- **InCode**: F# code outside comment blocks - wrap in fenced code blocks
- **Hidden**: After `(*** hide ***)` - skip until next markdown block

### Input/Output Transformation

| Input Pattern | Output |
|--------------|--------|
| `(** content *)` | Raw markdown content |
| `(*** hide ***)` | Nothing (enters hidden mode) |
| Regular F# code | Wrapped in ```fsharp fenced blocks |

### File Structure

```text
chapters/
├── 01-introduction.fs      # What is Fable.Python, why use it
├── 02-getting-started.fs   # Setup, first project, hello world
├── 03-bindings.fs          # Python interop, type bindings
└── 04-compatibility.fs     # F# features supported, limitations
tools/
├── fabletext.fs            # Fabletext converter source (F#)
└── fabletext.fsproj
output/
├── chapters/               # Generated Python from chapters
└── tools/
    └── fabletext.py        # Generated converter (Python)
docs/
├── *.md                    # Individual chapter markdown
└── blogpost.md             # Concatenated for Hashnode
```

## Fable.Python Considerations

- Use `Fable.Core` attributes where needed
- Stick to Fable-compatible F# subset
- File I/O via Python interop (`[<Emit>]` with `open`, `read`, etc.)

## Resources

- [Fable.Python docs](https://fable.io/docs/getting-started/python.html)
- [Fable.Python GitHub](https://github.com/fable-compiler/Fable.Python/)
