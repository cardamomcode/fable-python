# Fable.Python: F# Advent 2025

> Write F#, run Python - a practical guide to Fable.Python

This is a comprehensive guide to [Fable.Python](https://github.com/fable-compiler/Fable.Python/), written as literate F# that transpiles to Python and generates its own documentation.

## Chapters

1. **Introduction** - What is Fable.Python and why use it
2. **For Python Developers** - F# concepts explained for Pythonistas
3. **Getting Started** - Setup, your first project, hello world
4. **Interop** - Using existing Python libraries and Fable.Python bindings
5. **Bindings** - Creating your own type-safe bindings for Python libraries
6. **Compatibility** - Supported F# features and limitations
7. **Async Programming** - F# async and Python asyncio
8. **Testing** - Testing F# code with Python test runners
9. **Fable v5** - What's new in Fable v5 for Python
10. **Pydantic** - Pydantic interop with Decorate and ClassAttributes
11. **FastAPI** - Building type-safe web APIs with F#
12. **Units of Measure** - Compile-time dimensional analysis
13. **Fable.Literate** - The self-documenting converter
14. **Summary** - Wrap-up, resources, and contributing

## The Strange Loop

This guide is self-documenting: each chapter is an `.fs` file with embedded Markdown comments. **Fabletext** (the final chapter) processes these files to generate the documentation you're reading - including itself.

The chain:
1. Write F# with embedded Markdown (`chapters/*.fs`)
2. Compile to Python with Fable
3. Run Fabletext (F# compiled to Python) to extract documentation
4. Output: the Markdown you're reading

The blog post is its own proof of concept.

## Quick Start

```bash
# Install dependencies
just setup
just restore

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
just format     # Format F# (fantomas) and Python (ruff) files
just lint       # Lint Python (ruff) and Markdown (markdownlint)
just all        # Full pipeline: restore, build, generate, format, lint
just clean      # Remove generated files
```

## Project Structure

```text
chapters/
├── Introduction.fs       # What is Fable.Python
├── Python.fs             # F# for Python developers
├── GettingStarted.fs     # Setup and first project
├── Interop.fs            # Using Python libraries
├── Bindings.fs           # Creating bindings
├── Compatibility.fs      # F# feature support
├── AsyncProgramming.fs   # Async workflows
├── Testing.fs            # Testing with Python
├── FableV5.fs            # What's new in Fable v5
├── Pydantic.fs           # Pydantic interop
├── UnitsOfMeasure.fs     # Dimensional analysis
├── FableLiterate.fs      # Symlink → ../Fable.Literate/App.fs
└── Summary.fs            # Wrap-up and resources
Fable.Literate/
├── App.fs                # Fable.Literate converter (F#)
└── Fable.Literate.fsproj
output/
├── chapters/             # Generated Python from chapters
└── Fable.Literate/
    └── app.py            # Generated converter (Python)
docs/
├── Introduction.md       # Individual chapter docs
├── Python.md
├── ...
└── blogpost.md           # Concatenated for Hashnode
```

## Chapter Order

Defined in `justfile`:

```just
chapters := "Introduction Python GettingStarted Interop Bindings Compatibility AsyncProgramming Testing FableV5 Pydantic UnitsOfMeasure FableLiterate Summary"
```

To add a new chapter, just add the file and update this list.

## Technology Stack

- **Fable 5** (alpha) - F# to Python compiler
- **uv** - Python dependency management
- **just** - Command runner
- **ruff** - Python formatter/linter
- **fantomas** - F# formatter
- **markdownlint** - Markdown linter

## Related Libraries

- [Thoth.Json.Python](https://github.com/thoth-org/Thoth.Json.Python) - Type-safe JSON
- [AsyncRx](https://github.com/dbrattli/AsyncRx) - Reactive extensions
- [Fable.Giraffe](https://github.com/dbrattli/Fable.Giraffe) - Web framework
- [Feliz.ViewEngine](https://github.com/dbrattli/Feliz.ViewEngine) - HTML DSL
- [Siren](https://github.com/Freymaurer/Siren) - Mermaid diagrams
- [Fable.Pyxpecto](https://github.com/Freymaurer/Fable.Pyxpecto) - Testing
- [ARCtrl](https://github.com/nfdi4plants/ARCtrl) - Real-world multi-target library

## Resources

- [Fable](https://fable.io/)
- [Fable.Python Documentation](https://fable.io/docs/getting-started/python.html)
- [Fable.Python GitHub](https://github.com/fable-compiler/Fable.Python/)
- [BINDINGS_GUIDE.md](./BINDINGS_GUIDE.md) - Comprehensive binding patterns

---

*Part of [F# Advent 2025](https://sergeytihon.com/fsadvent/)*
