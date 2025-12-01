# Fabletext

A literate programming converter inspired by [jupytext](https://github.com/mwouts/jupytext)
and [FSharp.Formatting](https://fsprojects.github.io/FSharp.Formatting/).

Converts F# literate files (`.fs` files with embedded Markdown) into
GitHub-flavored Markdown. Written in F# and compiles to Python via Fable.Python.

## How It Works

The converter is a simple state machine that processes input line by line:

- Lines inside `(** ... *)` blocks are emitted as Markdown
- F# code outside those blocks is wrapped in fenced code blocks
- `(*** hide ***)` sections are excluded from output

## Parser State

We track three possible states as we scan through the file:

```fsharp
/// Represents the current state of the parser state machine.
type ParserState =
    /// Inside a markdown block (** ... *)
    | InMarkdown
    /// Regular F# code outside markdown blocks
    | InCode
    /// Hidden section after (*** hide ***), content is skipped
    | Hidden
```

## Line Classification

Each line is classified using an active pattern to determine how to handle it.
The pattern also extracts content from markdown start lines:

```fsharp
/// Active pattern for classifying source lines.
/// - `HideCmd`: The (*** hide ***) directive
/// - `MarkdownSingle content`: Single-line markdown (** content *)
/// - `MarkdownOpen content`: Start of markdown block, possibly with content
/// - `MarkdownClose`: End of markdown block *)
/// - `Content`: Any other line
let (|HideCmd|MarkdownSingle|MarkdownOpen|MarkdownClose|Content|) (line: string) =
    let trimmed = line.Trim()

    match trimmed with
    | "(*** hide ***)" -> HideCmd
    | s when s.StartsWith("(**") && s.EndsWith("*)") && s.Length > 5 ->
        MarkdownSingle(s.Substring(3, s.Length - 5).Trim())
    | s when s.StartsWith("(**") ->
        let content = if s.Length > 3 then s.Substring(3).Trim() else ""
        MarkdownOpen content
    | "*)" -> MarkdownClose
    | _ -> Content
```

## State Transitions

The heart of the parser - handling transitions between states:

```fsharp
/// The parsing context that tracks state, buffered code, and output.
type ParseContext = {
    /// Current parser state
    State: ParserState
    /// Accumulated code lines waiting to be flushed
    CodeBuffer: string list
    /// Accumulated output chunks (in reverse order)
    Output: string list
}

/// Initial empty parsing context.
let emptyContext = {
    State = InCode
    CodeBuffer = []
    Output = []
}

/// Active pattern that matches strings starting with any of the given prefixes.
let (|StartsWithAny|_|) (prefixes: string list) (s: string) =
    let trimmed = s.Trim()

    if prefixes |> List.exists trimmed.StartsWith then
        Some()
    else
        None

/// Boilerplate prefixes that should be excluded from code blocks.
let boilerplatePrefixes = [ "module "; "namespace " ]

/// Flushes the code buffer to output as a fenced code block.
/// Skips empty or boilerplate-only code blocks.
let flushCodeBuffer (ctx: ParseContext) : ParseContext =
    if ctx.CodeBuffer.IsEmpty then
        ctx
    else
        let code =
            ctx.CodeBuffer
            |> List.rev
            |> String.concat "\n"
            |> fun s -> s.Trim() // Remove leading/trailing empty lines
        // Skip empty, whitespace-only, or boilerplate code blocks
        match code with
        | s when String.IsNullOrWhiteSpace s -> { ctx with CodeBuffer = [] }
        | StartsWithAny boilerplatePrefixes -> { ctx with CodeBuffer = [] }
        | _ ->
            // Add blank line before and after code block for markdown lint compliance
            let block = $"\n```fsharp\n{code}\n```\n\n"

            {
                ctx with
                    CodeBuffer = []
                    Output = block :: ctx.Output
            }

/// Processes a single line, updating the parse context based on state transitions.
let processLine (ctx: ParseContext) (line: string) : ParseContext =
    match ctx.State, line with
    // Entering hidden mode
    | _, HideCmd -> { flushCodeBuffer ctx with State = Hidden }

    // Single-line markdown: (** content *)
    | (InCode | Hidden), MarkdownSingle content ->
        let flushed = flushCodeBuffer ctx

        { flushed with Output = (content + "\n") :: flushed.Output }

    // Starting markdown block with or without content
    | (InCode | Hidden), MarkdownOpen content ->
        let flushed = flushCodeBuffer ctx

        if content.Length > 0 then
            {
                flushed with
                    State = InMarkdown
                    Output = (content + "\n") :: flushed.Output
            }
        else
            { flushed with State = InMarkdown }

    // Ending markdown block
    | InMarkdown, MarkdownClose -> { ctx with State = InCode }

    // Content inside markdown
    | InMarkdown, Content -> { ctx with Output = (line + "\n") :: ctx.Output }

    // Code line (not hidden)
    | InCode, Content -> { ctx with CodeBuffer = line :: ctx.CodeBuffer }

    // Hidden content - skip
    | Hidden, (Content | MarkdownClose) -> ctx

    // Ignore markdown markers in wrong state
    | InMarkdown, (MarkdownOpen _ | MarkdownSingle _) -> ctx
    | InCode, MarkdownClose -> ctx
```

## Processing a File

Read all lines, process them, and return the Markdown output:

```fsharp
/// Processes all lines from a literate F# file and returns the Markdown output.
let processLines (lines: string seq) : string =
    let finalCtx = lines |> Seq.fold processLine emptyContext |> flushCodeBuffer // Flush any remaining code
    finalCtx.Output |> List.rev |> String.concat ""
```

## Header Level Adjustment

For concatenating multiple chapters into a single document, we need to
increase header levels (# becomes ##, ## becomes ###, etc.):

```fsharp
/// Increases all markdown header levels by one (# becomes ##, etc.).
/// Preserves headers inside fenced code blocks.
let adjustHeaderLevels (markdown: string) : string =
    let lines = markdown.Split('\n')

    let folder (inCodeBlock, acc) (line: string) =
        match line with
        | s when s.StartsWith("```") -> not inCodeBlock, line :: acc
        | _ when inCodeBlock -> inCodeBlock, line :: acc
        | s when s.StartsWith("#") -> inCodeBlock, ("#" + line) :: acc
        | _ -> inCodeBlock, line :: acc

    lines |> Array.fold folder (false, []) |> snd |> List.rev |> String.concat "\n"
```

## Python File I/O

For Fable.Python, we use Python's file operations:

```fsharp
/// Reads the entire contents of a file as a string.
[<Emit("open($0, 'r').read()")>]
let readFile (path: string) : string = nativeOnly

/// Prints a string to stdout without a trailing newline.
[<Emit("print($0, end='')")>]
let printRaw (s: string) : unit = nativeOnly
```

## Main Entry Point

Read the input file, convert it, and print the result:

```fsharp
/// Main entry point. Converts a literate F# file to Markdown.
/// Use --increase-headers flag to bump all header levels by one.
[<EntryPoint>]
let main (args: string[]) =
    let hasFlag flag = args |> Array.contains flag
    let files = args |> Array.filter (fun a -> not (a.StartsWith("--")))

    if files.Length < 1 then
        printfn "Usage: python fabletext.py [--increase-headers] <input.fs>"
        1
    else
        let content = readFile files.[0]
        let lines = content.Split('\n')
        let markdown = processLines lines

        let output =
            if hasFlag "--increase-headers" then
                adjustHeaderLevels markdown
            else
                markdown

        printRaw output
        0
```

## Building and Running

```bash
# Transpile to Python
dotnet fable tools/ --lang python -o output/tools/

# Convert a literate file
python output/tools/fabletext.py chapters/01-introduction.fs > docs/01-introduction.md
```

That's it! A complete literate programming converter in under 200 lines of F#.
