# Introduction to Fable.Python

Welcome to this guide on [Fable.Python](https://github.com/fable-compiler/Fable.Python/) -
a compiler that transforms F# code into Python.

## What is Fable?

[Fable](https://fable.io/) is a compiler that brings F# to different platforms. While
Fable is best known for compiling F# to JavaScript, it also supports other targets
including Python, Rust, and Dart.

## Why Fable.Python?

F# is a functional-first language with powerful features like:

- **Type inference** - Write less, express more
- **Pattern matching** - Elegant handling of complex data
- **Immutability by default** - Safer, more predictable code
- **Algebraic data types** - Model your domain precisely

With Fable.Python, you get all these benefits while targeting the Python ecosystem.
This means you can:

1. Use F#'s type system and functional patterns
2. Interop with Python libraries (NumPy, Pandas, etc.)
3. Deploy anywhere Python runs - no .NET runtime needed

## A Simple Example

Let's start with something simple. Here's F# code that will compile to Python:

```fsharp

let greet name =
    $"Hello, {name}!"

let message = greet "Fable.Python"

```

When compiled with Fable, this generates clean, readable Python:

```python
def greet(name):
    return f"Hello, {name}!"

message = greet("Fable.Python")
```

## The Power of Types

F# shines when modeling domain concepts. Consider this example:

```fsharp

type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float

let area shape =
    match shape with
    | Circle radius -> System.Math.PI * radius * radius
    | Rectangle (width, height) -> width * height

let shapes = [
    Circle 5.0
    Rectangle (3.0, 4.0)
]

let totalArea = shapes |> List.sumBy area

```

This compiles to Python while preserving the semantic meaning. The discriminated
union becomes a tagged class structure, and pattern matching becomes clean
conditional logic.

## What's Next?

In the following chapters, we'll cover:

- **Getting Started** - Setting up your development environment
- **Bindings** - Working with Python libraries from F#
- **Compatibility** - Understanding what F# features are supported

Let's dive in!

## Getting Started with Fable.Python

Let's set up a Fable.Python project from scratch and get our first F# code
running as Python.

### Prerequisites

You'll need:

- [.NET SDK](https://dotnet.microsoft.com/download) (6.0 or later)
- [Python 3.12+](https://www.python.org/downloads/) (Fable targets Python 3.12 or higher)

### Project Setup

Create a new directory and initialize an F# project:

```bash
mkdir my-fable-python
cd my-fable-python

# Create F# console app
dotnet new console -lang F#

# Set up local tools and install Fable 5 (alpha)
dotnet new tool-manifest
dotnet tool install fable --version 5.0.0-alpha.17

# Add Fable.Core package
dotnet add package Fable.Core --version 5.0.0-beta.2
```

### Install Python Dependencies

Fable-generated Python code requires the `fable-library` runtime:

```bash
pip install "fable-library==5.0.0a17"
```

> **Note:** Version pinning is important! The fable-library version must match
> your Fable compiler version. PyPI uses `5.0.0a17` format instead of `5.0.0-alpha.17`.

### Your First Program

Replace the contents of `Program.fs` with:

```fsharp
printfn "Hello from Fable.Python!"

let square x = x * x
let numbers = [1; 2; 3; 4; 5]
let squares = numbers |> List.map square

printfn "Squares: %A" squares
```

### Compile and Run

Transpile to Python:

```bash
dotnet fable --lang python
```

This creates `Program.py` in your project directory. Run it:

```bash
python3 Program.py
```

You should see:

```text
Hello from Fable.Python!
Squares: [1; 4; 9; 16; 25]
```

### Watch Mode

For development, use watch mode to automatically recompile on changes:

```bash
dotnet fable watch --lang python
```

Now any changes to your F# files will instantly produce updated Python output.

### Project Structure

After setup, your project looks like this:

```text
my-fable-python/
├── Program.fs          # Your F# source code
├── Program.py          # Generated Python (don't edit!)
├── my-fable-python.fsproj
├── fable_modules/      # Fable runtime modules
└── .config/
    └── dotnet-tools.json
```

### Next Steps

Now that you have a working setup, let's explore how to interact with Python
libraries in the next chapter on **Bindings**.

## Python Bindings and Interop

One of Fable.Python's strengths is seamless interoperability with the Python
ecosystem. This chapter covers how to call Python code from F# and create
type-safe bindings.

### Basic Imports

Use the `Import` attribute to bring in Python modules:

```fsharp

open Fable.Core

[<Import("path", "os")>]
let osPath: obj = nativeOnly

[<ImportMember("os.path")>]
let join(paths: string[]): string = nativeOnly

```

The `nativeOnly` placeholder tells Fable this will be resolved at runtime.

### Automatic Case Conversion

Fable automatically converts F# camelCase to Python snake_case:

```fsharp

type MyClass() =
    member _.myMethod() = "hello"  // Becomes my_method() in Python

```

This keeps your F# code idiomatic while generating Pythonic output.

### The Emit Attribute

For direct Python code embedding, use `[<Emit>]`:

```fsharp

[<Emit("len($0)")>]
let pyLen(x: 'a): int = nativeOnly

[<Emit("print($0, end=$1)")>]
let printWithEnd(value: string, ending: string): unit = nativeOnly

```

The `$0`, `$1`, etc. are placeholders for arguments.

### Working with Python Types

#### Lists and Sequences

F# lists compile to Python lists, making interop natural:

```fsharp

let fsharpList = [1; 2; 3; 4; 5]
// Compiles to: [1, 2, 3, 4, 5]

let doubled = fsharpList |> List.map (fun x -> x * 2)

```

#### Dictionaries

F# Maps work with Python dicts:

```fsharp

let config = Map.ofList [
    "host", "localhost"
    "port", "8080"
]

```

#### Tuples

F# tuples become Python tuples:

```fsharp

let point = (10, 20)
let x, y = point

```

### Erased Unions

For APIs that accept multiple types, use erased unions:

```fsharp

open Fable.Core.JsInterop

[<Emit("isinstance($0, str)")>]
let isString(x: obj): bool = nativeOnly

// U2 can hold either type
let processValue (value: U2<string, int>) =
    match value with
    | U2.Case1 s -> $"String: {s}"
    | U2.Case2 n -> $"Number: {n}"

```

### String Enums

For Python APIs that use string constants:

```fsharp

[<StringEnum>]
type LogLevel =
    | Debug
    | Info
    | Warning
    | Error

let level = LogLevel.Info  // Compiles to: "info"

```

### Creating Binding Libraries

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

### Creating Python Builtins

You can create bindings for Python's built-in functions:

```fsharp

[<Emit("len($0)")>]
let len (x: 'a): int = nativeOnly

[<Emit("print($0)")>]
let pyPrint (x: 'a): unit = nativeOnly

[<Emit("range($0)")>]
let range (n: int): seq<int> = nativeOnly

[<Emit("open($0, $1)")>]
let openFile (path: string) (mode: string): obj = nativeOnly

```

### Next Steps

Now that you understand bindings, let's explore what F# features are supported
in the **Compatibility** chapter.

## F# Compatibility in Fable.Python

Understanding what works and what doesn't is crucial when targeting Python
with Fable. This chapter covers supported features, limitations, and
important differences from .NET.

### Fully Supported Features

#### Core Types

These F# types map directly to Python equivalents:

```fsharp

// Strings -> Python str
let greeting = "Hello, Python!"

// Booleans -> Python bool
let isEnabled = true

// Tuples -> Python tuple
let coordinates = (10.5, 20.3)

// F# List -> Python list (via fable-library)
let numbers = [1; 2; 3; 4; 5]

// ResizeArray -> Python list (native)
let mutableList = ResizeArray<int>()

```

#### Functions and Lambdas

First-class functions work as expected:

```fsharp

let add x y = x + y
let multiply = fun x y -> x * y

let applyTwice f x = f (f x)
let result = applyTwice (add 1) 5  // 7

```

#### Pattern Matching

Full pattern matching support:

```fsharp

type Result<'T, 'E> =
    | Ok of 'T
    | Error of 'E

let handleResult result =
    match result with
    | Ok value -> $"Success: {value}"
    | Error err -> $"Failed: {err}"

let activePatternExample input =
    match input with
    | x when x > 0 -> "positive"
    | x when x < 0 -> "negative"
    | _ -> "zero"

```

#### Records

Records compile to Python dataclasses:

```fsharp

type Person = {
    Name: string
    Age: int
    Email: string option
}

let person = {
    Name = "Alice"
    Age = 30
    Email = Some "alice@example.com"
}

```

#### Discriminated Unions

DUs are fully supported with pattern matching:

```fsharp

type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float
    | Triangle of a: float * b: float * c: float

let describe shape =
    match shape with
    | Circle r -> $"Circle with radius {r}"
    | Rectangle (w, h) -> $"Rectangle {w}x{h}"
    | Triangle (a, b, c) -> $"Triangle with sides {a}, {b}, {c}"

```

#### Object-Oriented Features

Classes, interfaces, inheritance, and overloading work:

```fsharp

type IShape =
    abstract member Area: float

type Circle2(radius: float) =
    member _.Radius = radius
    interface IShape with
        member _.Area = System.Math.PI * radius * radius

```

#### Collections

Core collection operations are supported:

```fsharp

let listOps =
    [1..10]
    |> List.filter (fun x -> x % 2 = 0)
    |> List.map (fun x -> x * x)
    |> List.sum

let arrayOps =
    [|1; 2; 3|]
    |> Array.map (fun x -> x + 1)

let setOps =
    Set.ofList [1; 2; 2; 3; 3; 3]  // {1, 2, 3}

let mapOps =
    Map.ofList [("a", 1); ("b", 2)]

```

### Limitations and Differences

#### Options Are Erased

Options are optimized away at runtime:

```fsharp

let someValue = Some 42    // Compiles to just: 42
let noneValue = None       // Compiles to: None

```

This works fine for most cases, but be careful with nested options -
`Some None` vs `None` can be ambiguous.

#### Multi-line Lambdas

Python doesn't support multi-line lambdas. Fable lifts them to separate
functions:

```fsharp

// This F#:
let processed =
    [1; 2; 3]
    |> List.map (fun x ->
        let doubled = x * 2
        let squared = doubled * doubled
        squared)

// Becomes a separate function in Python

```

#### Numeric Types

Most numerics use custom wrappers to maintain F# semantics. `bigint` uses
Python's native `int`:

```fsharp

let small: int = 42
let big: bigint = 12345678901234567890I

```

#### Computation Expressions

Async and task computation expressions have some differences from .NET.
Use `Async.StartAsTask` for Python compatibility.

### Project Configuration

#### Entry Point Applications

If your project has `[<EntryPoint>]`, you need:

```xml
<PropertyGroup>
    <OutputType>Exe</OutputType>
</PropertyGroup>
```

This ensures absolute imports in generated Python.

#### Libraries

Libraries use relative imports by default, which is correct for packages.

### Best Practices

1. **Test in Python** - Always test generated code in Python, not just in F#
2. **Avoid reflection** - Reflection has limited support
3. **Use type annotations** - Helps with debugging generated code
4. **Check fable-library** - Some .NET APIs may not be implemented yet

### Summary

Fable.Python provides excellent F# support. The main things to watch for are:

- Option erasure in edge cases
- Multi-line lambda lifting
- Some .NET APIs may be missing

For most F# code, you can write idiomatic functional code and it will
compile to clean, working Python.
