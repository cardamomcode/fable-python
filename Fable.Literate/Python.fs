/// Python interop and extraction utilities for Fable.Literate.
module Fable.Literate.Python

open System
open Fable.Python.Builtins
open Fable.Python.Sys

// ============ Python Bindings ============

/// Reads the entire contents of a file as a string.
let readFile (path: string) : string = builtins.``open``(path).read ()

/// Prints a string to stdout without a trailing newline.
let printRaw (s: string) : unit = builtins.print (s, ``end`` = "")

/// Prints a string to stderr with a trailing newline.
let eprintln (s: string) : unit = builtins.print (s, file = sys.stderr)

// ============ Python Extract ============

/// Checks if a line starts a new top-level definition (not indented).
let isTopLevelDefinition (line: string) : bool =
    let trimmed = line.Trim()

    not (String.IsNullOrWhiteSpace line)
    && not (line.StartsWith " ")
    && not (line.StartsWith "\t")
    && not (line.StartsWith "#")
    && not (trimmed = ")" || trimmed = "]" || trimmed = "}")

/// Checks if a line is a decorator.
let isDecorator (line: string) : bool = line.TrimStart().StartsWith "@"

/// Checks if a line is a dunder method definition.
let isDunderMethod (line: string) : bool = line.TrimStart().StartsWith "def __"

/// Skips elements from the start of an array while the predicate is true.
let arraySkipWhile (predicate: 'a -> bool) (arr: 'a array) : 'a array =
    match arr |> Array.tryFindIndex (predicate >> not) with
    | Some idx -> arr[idx..]
    | None -> [||]

/// Takes elements from the start of an array while the predicate is true.
let arrayTakeWhile (predicate: 'a -> bool) (arr: 'a array) : 'a array =
    match arr |> Array.tryFindIndex (predicate >> not) with
    | Some idx -> arr[.. idx - 1]
    | None -> arr

/// Patterns for matching Python symbol definitions.
let symbolPatterns (symbol: string) = [
    symbol + " ="
    symbol + ": "
    "def " + symbol + "("
    "def " + symbol + "["
    "async def " + symbol + "("
    "async def " + symbol + "["
    "class " + symbol + "("
    "class " + symbol + ":"
    "class " + symbol + "["
]

/// Checks if a line matches any of the symbol definition patterns.
let matchesSymbol (symbol: string) (line: string) : bool =
    let trimmed = line.TrimStart()
    symbolPatterns symbol |> List.exists trimmed.StartsWith

/// Finds the definition index for a symbol in the source lines.
let findDefinitionIndex (symbol: string) (lines: string array) : int option =
    lines |> Array.tryFindIndex (matchesSymbol symbol)

/// Walks backwards from defIndex to find where decorators start.
let findDecoratorStart (lines: string array) (defIndex: int) : int =
    Seq.init defIndex (fun i -> defIndex - 1 - i)
    |> Seq.tryFind (fun i -> not (isDecorator lines[i]))
    |> Option.map ((+) 1)
    |> Option.defaultValue 0

/// Checks if a line starts a multiline definition.
let isMultilineDefinition (line: string) : bool =
    let trimmed = line.TrimStart()

    trimmed.StartsWith "class "
    || trimmed.StartsWith "def "
    || trimmed.StartsWith "async def "
    || trimmed.EndsWith "("
    || trimmed.EndsWith "["
    || trimmed.EndsWith "{"

/// Extracts the body of a multiline definition.
let extractMultilineBody (startIndex: int) (defIndex: int) (lines: string array) : string =
    let shouldStop idx (line: string) =
        idx > defIndex
        && (isTopLevelDefinition line || isDunderMethod line)

    // Start from decorator or definition line
    lines[startIndex..]
    // Track position for stop condition
    |> Array.indexed
    // Take until next top-level definition
    |> arrayTakeWhile (fun (i, line) -> not (shouldStop (startIndex + i) line))
    // Drop indices, keep lines
    |> Array.map snd
    // Reverse to trim from end
    |> Array.rev
    // Remove trailing blank lines
    |> arraySkipWhile String.IsNullOrWhiteSpace
    // Restore original order
    |> Array.rev
    // Join into final string
    |> String.concat "\n"

/// Extracts a single symbol definition from Python source lines.
let extractSymbol (symbol: string) (lines: string array) : string option =
    findDefinitionIndex symbol lines
    |> Option.map (fun defIndex ->
        let startIndex = findDecoratorStart lines defIndex

        if isMultilineDefinition lines[defIndex] then
            extractMultilineBody startIndex defIndex lines
        else
            lines[defIndex])

/// Extracts multiple symbols and combines them.
let extractSymbols (toPythonNaming: string -> string) (symbols: string list) (pythonContent: string) : string =
    let lines = pythonContent.Split('\n')

    symbols
    |> List.choose (fun sym -> extractSymbol (toPythonNaming sym) lines)
    |> String.concat "\n\n"
