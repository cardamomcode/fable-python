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

Each line is classified to determine how to handle it:

```fsharp

/// Classification of a source line for the parser.
type LineType =
    /// Start of markdown block: (**
    | MarkdownStart
    /// End of markdown block: *)
    | MarkdownEnd
    /// Hide command: (*** hide ***)
    | HideCommand
    /// Any other content line
    | ContentLine

/// Classifies a line of source code into its LineType.
let classifyLine (line: string) : LineType =
    let trimmed = line.Trim()
    if trimmed = "(*** hide ***)" then HideCommand
    elif trimmed.StartsWith("(**") then MarkdownStart
    elif trimmed = "*)" then MarkdownEnd
    else ContentLine

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

/// Checks if a code block contains only boilerplate (module/namespace declarations).
let isBoilerplate (code: string) : bool =
    let trimmed = code.Trim()
    trimmed.StartsWith("module ") || trimmed.StartsWith("namespace ")

/// Flushes the code buffer to output as a fenced code block.
/// Skips empty or boilerplate-only code blocks.
let flushCodeBuffer (ctx: ParseContext) : ParseContext =
    if ctx.CodeBuffer.IsEmpty then ctx
    else
        let code = ctx.CodeBuffer |> List.rev |> String.concat "\n"
        // Skip empty, whitespace-only, or boilerplate code blocks
        if code.Trim().Length = 0 || isBoilerplate code then
            { ctx with CodeBuffer = [] }
        else
            // Add blank line before and after code block for markdown lint compliance
            let block = $"\n```fsharp\n{code}\n```\n\n"
            { ctx with
                CodeBuffer = []
                Output = block :: ctx.Output }

/// Processes a single line, updating the parse context based on state transitions.
let processLine (ctx: ParseContext) (line: string) : ParseContext =
    let lineType = classifyLine line
    match ctx.State, lineType with
    // Entering hidden mode
    | _, HideCommand ->
        let flushed = flushCodeBuffer ctx
        { flushed with State = Hidden }

    // Starting markdown block
    | InCode, MarkdownStart
    | Hidden, MarkdownStart ->
        let flushed = flushCodeBuffer ctx
        let trimmed = line.Trim()
        // Handle single-line markdown: (** content *)
        if trimmed.EndsWith("*)") && trimmed.Length > 5 then
            let content = trimmed.Substring(3, trimmed.Length - 5).Trim()
            { flushed with
                State = InCode
                Output = (content + "\n") :: flushed.Output }
        // Handle (** with content on same line
        elif trimmed.Length > 3 then
            let content = trimmed.Substring(3).Trim()
            if content.Length > 0 then
                { flushed with
                    State = InMarkdown
                    Output = (content + "\n") :: flushed.Output }
            else
                { flushed with State = InMarkdown }
        else
            { flushed with State = InMarkdown }

    // Ending markdown block
    | InMarkdown, MarkdownEnd ->
        { ctx with State = InCode }

    // Content inside markdown
    | InMarkdown, ContentLine ->
        { ctx with Output = (line + "\n") :: ctx.Output }

    // Code line (not hidden)
    | InCode, ContentLine ->
        { ctx with CodeBuffer = line :: ctx.CodeBuffer }

    // Hidden content - skip
    | Hidden, ContentLine -> ctx
    | Hidden, MarkdownEnd -> ctx

    // Ignore markdown markers in wrong state
    | InMarkdown, MarkdownStart -> ctx
    | InCode, MarkdownEnd -> ctx

```

## Processing a File

Read all lines, process them, and return the Markdown output:

```fsharp

/// Processes all lines from a literate F# file and returns the Markdown output.
let processLines (lines: string seq) : string =
    let finalCtx =
        lines
        |> Seq.fold processLine emptyContext
        |> flushCodeBuffer  // Flush any remaining code
    finalCtx.Output
    |> List.rev
    |> String.concat ""

```

## Header Level Adjustment

For concatenating multiple chapters into a single document, we need to
increase header levels (# becomes ##, ## becomes ###, etc.):

```fsharp

/// Increases all markdown header levels by one (# becomes ##, etc.).
/// Preserves headers inside fenced code blocks.
let adjustHeaderLevels (markdown: string) : string =
    let lines = markdown.Split('\n')
    let mutable inCodeBlock = false
    lines
    |> Array.map (fun line ->
        if line.StartsWith("```") then
            inCodeBlock <- not inCodeBlock
            line
        elif inCodeBlock then
            line
        elif line.StartsWith("#") then
            "#" + line
        else
            line)
    |> String.concat "\n"

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
