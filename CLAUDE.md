# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**F# Advent 2025 blog post project** demonstrating Fable.Python capabilities.

**Fable.Literate** is a literate programming converter (inspired by jupytext) written in F# that transpiles to Python via Fable.Python. It processes `.fs` files with embedded Markdown comments (FSharp.Formatting conventions) and outputs GitHub-flavored Markdown suitable for publishing on platforms like Hashnode.

**Key concept:** The project is self-documenting - the chapters and converter generate the blog post that documents how they work.

## Build Commands (using just)

```bash
just setup      # Install Fable and Python dependencies
just restore    # Restore NuGet packages
just build      # Build all chapters and tools to Python
just generate   # Generate individual markdown docs from chapters
just blogpost   # Generate concatenated blogpost.md for publishing
just format     # Format Python with ruff
just lint       # Lint Markdown (markdownlint)
just watch      # Watch mode for development
just clean      # Clean generated files
just all        # Full pipeline: restore, build, generate, format, lint
```

## Architecture

### Fable.Literate AST Pipeline

The converter follows a compiler-like architecture with three phases:

1. **Parse**: Convert source lines into a Block AST
2. **Transform**: Filter hidden blocks, resolve Python includes
3. **Print**: Render the AST as Markdown

### Literate Directives

| Directive | Purpose |
|-----------|---------|
| `(** content *)` | Raw markdown content |
| `(*** hide ***)` | Hide following code from output |
| `(*** include-python: symbol ***)` | Include generated Python for symbol |
| Regular F# code | Wrapped in ```fsharp fenced blocks |

### Escaping F# in Headings

Use `` F`#` `` (backticks around `#`) in markdown headings to prevent markdownlint from interpreting it as ATX closed style:

```fsharp
(**
## F`#` Async Workflows
*)
```

### File Structure

```text
chapters/
├── introduction.fs       # What is Fable.Python, why use it
├── python.fs             # F# concepts for Python developers
├── getting-started.fs    # Setup, first project, hello world
├── interop.fs            # Using existing Python libraries
├── bindings.fs           # Creating Python bindings
├── compatibility.fs      # F# features supported, limitations
├── async-programming.fs  # async vs task, Python asyncio mapping
├── fable-v5.fs           # Fable v5 features, Rust core, PyPI
├── pydantic.fs           # Pydantic models, DTOs, validation
└── units-of-measure.fs   # Compile-time dimensional analysis
Fable.Literate/
├── App.fs                # Fable.Literate converter source (F#)
└── Fable.Literate.fsproj
output/
├── chapters/             # Generated Python from chapters
└── Fable.Literate/
    └── app.py            # Generated converter (Python)
docs/
├── *.md                  # Individual chapter markdown
└── blogpost.md           # Concatenated for Hashnode
```

## Chapter Writing Guidelines

- Each chapter is a literate F# file with embedded markdown
- Use `(** ... *)` for markdown content
- Use `(*** hide ***)` to hide setup code (module declarations, imports)
- Use `(*** include-python: symbolName ***)` to show generated Python
- Tables are auto-formatted by markdownlint - don't fight it
- Keep code examples self-contained and buildable

## Fable.Python Considerations

- Use `Fable.Core` attributes (`[<Emit>]`, `[<Import>]`, etc.)
- Use `Fable.Python.Pydantic` for Pydantic interop
- Stick to Fable-compatible F# subset
- `task { }` compiles to native Python `async def` (Fable v5)
- `async { }` for multi-target code (Python, .NET, JS)

## Resources

- [Fable.Python docs](https://fable.io/docs/getting-started/python.html)
- [Fable.Python GitHub](https://github.com/fable-compiler/Fable.Python/)
- [Content Plan](CONTENT-PLAN.md) - Chapter structure and TODO items
