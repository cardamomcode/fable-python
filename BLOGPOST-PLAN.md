# Fable.Python Literate Converter

## Project Overview

A self-documenting literate programming converter written in F# that transpiles to Python via Fable.Python. The converter processes `.fsx` files with embedded Markdown comments and outputs GitHub-flavored Markdown suitable for publishing on platforms like Hashnode.

**The meta twist:** The converter's source code IS the blog post - it parses itself to generate the Markdown that documents how it works.

## Goals

1. Create a practical tool for converting F# literate scripts to publishable Markdown
2. Demonstrate Fable.Python capabilities through a real-world example
3. Produce a blog post about Fable.Python that dogfoods the toolchain
4. Keep it minimal - ship v1, iterate later

## Input Format

F# script files (`.fsx`) using FSharp.Formatting literate conventions:

```fsharp
(**
# This is Markdown

Regular markdown content goes here between comment delimiters.
*)

let code = "This becomes a fenced code block"

(*** hide ***)
let hiddenSetup = "This code is excluded from output"

(** More markdown explaining the next section *)

let moreCode () =
    printfn "visible in output"
```

## Output Format

GitHub-flavored Markdown with fenced code blocks:

````markdown
# This is Markdown

Regular markdown content goes here between comment delimiters.

```fsharp
let code = "This becomes a fenced code block"
```

More markdown explaining the next section

```fsharp
let moreCode () =
    printfn "visible in output"
```
````

## Core Parsing Logic

### States

The parser operates as a simple state machine:

- **Markdown mode**: Inside `(** ... *)` blocks → emit content as-is
- **Code mode**: F# code outside comment blocks → accumulate and wrap in fenced blocks
- **Hidden mode**: After `(*** hide ***)` → skip until next markdown block

### Transformations

1. `(** ... *)` → Extract inner content, trim, emit as Markdown
2. `(*** hide ***)` → Enter hidden mode, emit nothing
3. `(*** include-python: sym1, sym2 ***)` → Extract symbols from transpiled Python, emit as fenced block
4. Regular F# code → Wrap in ``` fsharp ``` fenced block
5. Consecutive blank lines in code → Preserve reasonable whitespace
6. `#load`, `#r` directives → Optionally hide or include based on config

## Implementation Approach

### File Structure

```txt
fsx2md/
├── fsx2md.fsx          # The literate source (F# + embedded docs)
├── fsx2md.py           # Generated Python output
├── README.md           # Generated from fsx2md.fsx (the blog post!)
└── test/
    └── example.fsx     # Test input file
```

### F# Implementation Sketch

```fsharp
type ParserState =
    | InMarkdown
    | InCode
    | Hidden

type Line =
    | MarkdownStart       // (**
    | MarkdownEnd         // *)
    | HideCommand         // (*** hide ***)
    | IncludePython of string list  // (*** include-python: sym1, sym2 ***)
    | CodeLine of string
    | BlankLine

let classifyLine (line: string) : Line = ...

let processFile (lines: string seq) : string seq = ...
```

### Key Design Decisions

1. **Line-by-line processing** vs parser combinators
   - Start simple with line-by-line + state machine
   - Parser combinators are overkill for this grammar

2. **Fable.Python compatibility**
   - Use `Fable.Core` attributes where needed
   - Stick to Fable-compatible F# subset
   - File I/O via Python interop (`open`, `read`, `write`)

3. **Minimal v1 scope**
   - Handle `(** *)` blocks and `(*** hide ***)`
   - Support `(*** include-python: sym1, sym2 ***)` for showing transpiled Python
   - Skip: `define`, `module=`, `lang=`, evaluation
   - These can be added in v2 if needed

## Build & Run Pipeline

```bash
# 1. Transpile F# to Python
dotnet fable fsx2md.fsx --lang python -o .

# 2. Run the converter on itself
python fsx2md.py fsx2md.fsx > README.md

# 3. Preview or publish
# Copy README.md to Hashnode or render locally
```

## Blog Post Structure (Generated Output)

The generated README.md / blog post will naturally follow the code structure:

1. **Introduction** - What we're building and why
2. **The Problem** - FSharp.Formatting outputs 4-space indented code, not fenced blocks
3. **The Solution** - A self-parsing literate converter
4. **Implementation** - Walking through the F# code with explanations
5. **Fable.Python in Action** - Showing the transpiled Python
6. **Running It** - How to use the tool
7. **Conclusion** - The recursive beauty of self-documenting tools

## Success Criteria

- [ ] Converter successfully parses its own source file
- [ ] Output Markdown renders correctly on Hashnode with syntax highlighting
- [ ] Python output is clean and readable (good Fable.Python showcase)
- [ ] Blog post is coherent and tells a good story
- [ ] Total implementation < 200 lines of F#

## Future Enhancements (v2+)

- Support `(*** define: name ***)` for reorderable code
- Support `lang=` for non-F# code blocks
- Evaluation and output embedding
- Watch mode for live preview
- Configuration file for customization
