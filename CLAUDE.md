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

### File Structure

```text
chapters/
├── Introduction.fs       # What is Fable.Python, why use it
├── Python.fs             # F# concepts for Python developers
├── GettingStarted.fs     # Setup, first project, hello world
├── Interop.fs            # Using existing Python libraries
├── Bindings.fs           # Creating Python bindings
├── Compatibility.fs      # F# features supported, limitations
├── AsyncProgramming.fs   # async vs task, Python asyncio mapping
├── Testing.fs            # Testing F# code with Python test runners
├── FableV5.fs            # Fable v5 features, Rust core, PyPI
├── Pydantic.fs           # Pydantic models, DTOs, validation
├── FastAPI.fs            # Type-safe web APIs with FastAPI
├── UnitsOfMeasure.fs     # Compile-time dimensional analysis
├── FableLiterate.fs      # Symlink → ../Fable.Literate/App.fs
└── Summary.fs            # Wrap-up, resources, repo link
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
- [Content Plan](CONTENT-PLAN.md) - Chapter structure and TODO items (important!)
