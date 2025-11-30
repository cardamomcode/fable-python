module Bindings

(**
# Python Bindings and Interop

One of Fable.Python's strengths is seamless interoperability with the Python
ecosystem. This chapter covers how to call Python code from F# and create
type-safe bindings.

## Basic Imports

Use the `Import` attribute to bring in Python modules:
*)

open Fable.Core

[<Import("path", "os")>]
let osPath: obj = nativeOnly

[<ImportMember("os.path")>]
let join(paths: string[]): string = nativeOnly

(**
The `nativeOnly` placeholder tells Fable this will be resolved at runtime.

## Automatic Case Conversion

Fable automatically converts F# camelCase to Python snake_case:
*)

type MyClass() =
    member _.myMethod() = "hello"  // Becomes my_method() in Python

(**
This keeps your F# code idiomatic while generating Pythonic output.

## The Emit Attribute

For direct Python code embedding, use `[<Emit>]`:
*)

[<Emit("len($0)")>]
let pyLen(x: 'a): int = nativeOnly

[<Emit("print($0, end=$1)")>]
let printWithEnd(value: string, ending: string): unit = nativeOnly

(**
The `$0`, `$1`, etc. are placeholders for arguments.

## Working with Python Types

### Lists and Sequences

F# lists compile to Python lists, making interop natural:
*)

let fsharpList = [1; 2; 3; 4; 5]
// Compiles to: [1, 2, 3, 4, 5]

let doubled = fsharpList |> List.map (fun x -> x * 2)

(**
### Dictionaries

F# Maps work with Python dicts:
*)

let config = Map.ofList [
    "host", "localhost"
    "port", "8080"
]

(**
### Tuples

F# tuples become Python tuples:
*)

let point = (10, 20)
let x, y = point

(**
## Erased Unions

For APIs that accept multiple types, use erased unions:
*)

open Fable.Core.JsInterop

[<Emit("isinstance($0, str)")>]
let isString(x: obj): bool = nativeOnly

// U2 can hold either type
let processValue (value: U2<string, int>) =
    match value with
    | U2.Case1 s -> $"String: {s}"
    | U2.Case2 n -> $"Number: {n}"

(**
## String Enums

For Python APIs that use string constants:
*)

[<StringEnum>]
type LogLevel =
    | Debug
    | Info
    | Warning
    | Error

let level = LogLevel.Info  // Compiles to: "info"

(**
## Creating Binding Libraries

For comprehensive Python library bindings, create a dedicated module:

```fsharp
module MyPythonLib

open Fable.Core

[<Import("MyClass", "my_python_lib")>]
type MyClass =
    [<Emit("$0.method_one($1)")>]
    member _.MethodOne(arg: string): string = nativeOnly

    [<Emit("$0.method_two($1, $2)")>]
    member _.MethodTwo(a: int, b: int): int = nativeOnly
```

## Creating Python Builtins

You can create bindings for Python's built-in functions:
*)

[<Emit("len($0)")>]
let len (x: 'a): int = nativeOnly

[<Emit("print($0)")>]
let pyPrint (x: 'a): unit = nativeOnly

[<Emit("range($0)")>]
let range (n: int): seq<int> = nativeOnly

[<Emit("open($0, $1)")>]
let openFile (path: string) (mode: string): obj = nativeOnly

(**
## Next Steps

Now that you understand bindings, let's explore what F# features are supported
in the **Compatibility** chapter.
*)
