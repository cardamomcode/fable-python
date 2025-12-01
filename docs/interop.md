# Python Interop

Now that you have a Fable.Python project set up, let's explore how to work
with Python libraries and the existing bindings in the Fable.Python ecosystem.

## The Fable.Python Library

The [Fable.Python](https://github.com/fable-compiler/Fable.Python) NuGet package
provides ready-to-use bindings for Python's standard library. Add it to your project:

```bash
dotnet add package Fable.Python
```

This gives you typed access to modules like `os`, `sys`, `json`, `asyncio`, and more.

## Using Standard Library Modules

Here's how to use Python's `os` module:

```fsharp
open Fable.Python.Os

let currentDir = os.getcwd ()
let files = os.listdir "."
```

The bindings follow F# naming conventions, but Fable automatically converts
to Python's snake_case when generating code.

## Working with Python's json Module

For basic JSON operations, use Python's built-in `json` module:

```fsharp
open Fable.Python.Json

// Serialize F# data to JSON string
let data = {| name = "Alice"; age = 30 |}
```

Anonymous records (`{| ... |}`) are perfect for JSON - they compile to
Python dictionaries.

## Working with Python Types

### F# to Python Type Mapping

Understanding how types map helps you work with Python libraries:

| F# Type | Python Type | Notes |
| ------- | ----------- | ----- |
| `string` | `str` | Direct mapping |
| `int` | `int` | Via fable-library wrapper |
| `float` | `float` | Via fable-library wrapper |
| `bool` | `bool` | Direct mapping |
| `unit` | `None` | Void/nothing |
| `'T option` | `T \| None` | Erased - `Some x` becomes just `x` |
| `'T list` | `list` | Immutable F# list |
| `'T array` | `list` | Mutable array |
| `ResizeArray<'T>` | `list` | Python's native list |
| `Map<K,V>` | `dict`-like | F# immutable map |
| Record | `dataclass` | F# records become dataclasses |
| Tuple | `tuple` | Direct mapping |

### Lists and Arrays

```fsharp
// F# list - immutable, compiles to Python list
let numbers = [ 1; 2; 3; 4; 5 ]

// Array - mutable, also compiles to Python list
let mutableNumbers = [| 1; 2; 3 |]

// ResizeArray - Python's native list type
let pythonList = ResizeArray<int>()
pythonList.Add(1)
pythonList.Add(2)
```

### Tuples

F# tuples map directly to Python tuples:

```fsharp
let point = (10, 20)
let x, y = point

// Tuples work great for multiple return values
let divmod a b = (a / b, a % b)
let quotient, remainder = divmod 17 5
```

### Anonymous Records as Dictionaries

Anonymous records are ideal for creating Python dictionaries:

```fsharp
let config =
    {| host = "localhost"
       port = 8080
       debug = true |}
```

This compiles to a Python dict: `{"host": "localhost", "port": 8080, "debug": True}`

### Option Types

F# `option` types are *erased* at runtime for efficiency:

```fsharp
let maybeName: string option = Some "Alice"
// In Python, this is just: "Alice"

let noName: string option = None
// In Python, this is: None
```

This means `Some value` becomes just `value` in Python, and `None` stays `None`.
Pattern matching still works perfectly in F#:

```fsharp
let greet nameOpt =
    match nameOpt with
    | Some name -> $"Hello, {name}!"
    | None -> "Hello, stranger!"
```

## Calling Python Functions

### Basic Function Calls

Most Python functions can be called naturally through bindings:

```fsharp
open Fable.Python.Builtins

let length = builtins.len [ 1; 2; 3 ]
let absValue = builtins.abs (-42)
```

### Working with sys Module

```fsharp
open Fable.Python.Sys

let pythonVersion = sys.version
let args = sys.argv
```

### Path Operations with os.path

```fsharp
let fullPath = os.path.join [| "/home"; "user"; "file.txt" |]
let fileName = os.path.basename "/path/to/file.txt"
let dirName = os.path.dirname "/path/to/file.txt"
```

## Environment Variables

Use `os.getenv` to safely retrieve environment variables:

```fsharp
let home = os.getenv ("HOME", "")
let user = os.getenv "USER"  // Returns string option
```

## File Operations

Reading and writing files uses Python's built-in functions:

```fsharp
open Fable.Core

[<Emit("open($0, 'r').read()")>]
let readFile (path: string) : string = nativeOnly

[<Emit("open($0, 'w').write($1)")>]
let writeFile (path: string) (content: string) : unit = nativeOnly
```

For more complex file handling, you might want to use Python's context managers
through custom bindings (covered in the Bindings chapter).

## Type Conversions

### Explicit Conversions

Sometimes you need to convert between F# and Python types explicitly:

```fsharp
// F# list to Python list (usually automatic)
let fsharpList = [ 1; 2; 3 ]

// When you need a ResizeArray specifically
let asResizeArray = ResizeArray(fsharpList)
```

### Working with obj

When dealing with dynamic Python APIs, you may encounter `obj`:

```fsharp
let handleDynamic (value: obj) =
    // Pattern match on the actual type
    match value with
    | :? string as s -> $"Got string: {s}"
    | :? int as n -> $"Got int: {n}"
    | _ -> "Got something else"
```

## Practical Example: Reading JSON Config

Here's a complete example combining several concepts:

```fsharp
let loadConfig (path: string) =
    let content = readFile path
    // Parse JSON and work with it
    json.loads content
```

## What's Next?

Now you know how to use existing Python bindings. In the next chapter,
we'll learn how to create your own bindings for Python libraries that
don't have F# bindings yet.
