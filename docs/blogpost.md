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

## When to Use Fable.Python

Fable.Python is a great choice when:

- **Python ecosystem access** - You need AI/ML libraries (PyTorch, TensorFlow, LangChain),
  data science tools (Pandas, NumPy), or frameworks like Pydantic and FastAPI
- **F# type safety** - You want pattern matching and exhaustive checking while using
  Python libraries
- **Shared domain logic** - Write once in F#, run on .NET, JavaScript, Rust, and Python
- **Publish to PyPI** - Your F# library can be available to the entire Python ecosystem
- **Units of measure** - F#'s compile-time dimensional analysis prevents unit errors
  that Python can't catch

## When NOT to Use Fable.Python

- When your F# code depends on .NET libraries without Fable support
- Performance-critical code (Python is still slow)
- Team won't learn F#

**Best fit:** You love F#, but need Python's ecosystem.

## A Simple Example

Let's start with something simple. Here's F# code that will compile to Python:

```fsharp
let greet name = $"Hello, {name}!"

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
    | Rectangle(width, height) -> width * height

let shapes = [ Circle 5.0; Rectangle(3.0, 4.0) ]

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

## Are You a Python Developer?

If you're coming from Python, welcome! This chapter will help you understand
the F# code you'll see throughout this guide. Don't worry - F# is more
approachable than it might first appear, and many concepts will feel familiar.

### What is F#?

F# is a functional-first language that runs on .NET. But here's the key insight
for you: **with Fable.Python, .NET is just a build tool**. You write F#, it
compiles to Python, and you run Python. Your deployment is pure Python.

Think of it like TypeScript for JavaScript - you get better tooling and type
safety during development, but the output is the language you know.

### Key Concepts You'll See

Let's map F# concepts to Python equivalents you already understand.

#### Type Inference

F# has type inference like Python's type hints, but enforced at compile time:

```python
# Python with type hints (optional, not enforced)
def greet(name: str) -> str:
    return f"Hello, {name}"
```

```fsharp
// F# - types are inferred automatically
let greet name = $"Hello, {name}"

// Or explicitly annotated (rarely needed)
let greetExplicit (name: string) : string = $"Hello, {name}"
```

The compiler figures out that `name` is a string and `greet` returns a string.
No need to write it unless you want to.

#### Pattern Matching

Python 3.10+ has `match`/`case`. F# pattern matching is similar but more powerful:

```python
# Python match/case
match command:
    case "quit":
        return exit()
    case "help":
        return show_help()
    case _:
        return unknown_command()
```

```fsharp
let handleCommand command =
    match command with
    | "quit" -> "Exiting..."
    | "help" -> "Showing help..."
    | _ -> "Unknown command"
```

F# pattern matching also destructures data, which we'll see with discriminated unions.

#### Discriminated Unions (Sum Types)

This is F#'s superpower. Think of it as a type-safe enum that can hold data:

```python
# Python - often done with classes or dataclasses
class Shape:
    pass

class Circle(Shape):
    def __init__(self, radius: float):
        self.radius = radius

class Rectangle(Shape):
    def __init__(self, width: float, height: float):
        self.width = width
        self.height = height
```

```fsharp
// F# discriminated union - much more concise
type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float

// Pattern matching ensures you handle all cases
let area shape =
    match shape with
    | Circle radius -> Math.PI * radius * radius
    | Rectangle (width, height) -> width * height
```

The compiler will warn you if you forget to handle a case. No more runtime
`AttributeError` because you forgot a shape type!

#### Records

Records are like Python's `@dataclass` but immutable by default:

```python
# Python dataclass
@dataclass
class Person:
    name: str
    age: int
    email: str | None = None
```

```fsharp
// F# record
type Person = {
    Name: string
    Age: int
    Email: string option
}

// Creating a record
let alice = {
    Name = "Alice"
    Age = 30
    Email = Some "alice@example.com"
}
```

Records are immutable - to "change" one, you create a copy with updated fields:

```fsharp
let olderAlice = { alice with Age = 31 }
```

#### The Pipeline Operator

The `|>` operator is like method chaining, but for any function:

```python
# Python - nested calls or intermediate variables
result = sum(map(lambda x: x * 2, filter(lambda x: x > 0, numbers)))

# Or with intermediate variables
positives = filter(lambda x: x > 0, numbers)
doubled = map(lambda x: x * 2, positives)
result = sum(doubled)
```

```fsharp
let numbers = [-1; 2; -3; 4; 5]

// F# pipeline - reads left to right, top to bottom
let result =
    numbers
    |> List.filter (fun x -> x > 0)
    |> List.map (fun x -> x * 2)
    |> List.sum
```

The `|>` operator takes the value on the left and passes it as the last
argument to the function on the right. It makes data transformations very
readable.

#### Option Types

F# uses `Option` instead of `None`/null. This forces you to handle missing values:

```python
# Python - None can sneak in anywhere
def find_user(id: int) -> User | None:
    ...

user = find_user(123)
print(user.name)  # Runtime error if user is None!
```

```fsharp
// F# Option - compiler ensures you handle None
let findUser id : Person option =
    if id = 1 then Some alice
    else None

let displayName userId =
    match findUser userId with
    | Some person -> person.Name
    | None -> "Unknown user"
```

You cannot accidentally use a `None` value - the compiler requires you to
unwrap the option first.

### F# vs Python: Quick Reference

| Concept          | Python              | F#                        |
| ---------------- | ------------------- | ------------------------- |
| Function def     | `def foo(x):`       | `let foo x =`             |
| Lambda           | `lambda x: x + 1`   | `fun x -> x + 1`          |
| List             | `[1, 2, 3]`         | `[1; 2; 3]`               |
| Tuple            | `(1, "a")`          | `(1, "a")`                |
| Dictionary       | `{"a": 1}`          | `Map.ofList [("a", 1)]`   |
| None check       | `if x is None:`     | `match x with None ->`    |
| String format    | `f"Hello {name}"`   | `$"Hello {name}"`         |
| Type annotation  | `x: int`            | `x: int` (same!)          |
| Comments         | `# comment`         | `// comment`              |
| Multiline string | `"""text"""`        | `"""text"""` (same!)      |

### Why Learn F#?

As a Python developer, F# gives you:

1. **Catch bugs at compile time** - No more `TypeError` or `AttributeError` at runtime
2. **Exhaustive pattern matching** - Compiler ensures you handle all cases
3. **Immutability by default** - Fewer bugs from unexpected state changes
4. **Excellent refactoring** - Change a type, compiler shows every place to update
5. **Self-documenting code** - Types serve as documentation that can't go stale

### Don't Worry About .NET

You might think "but I don't know .NET!" - and that's fine. For Fable.Python:

- You don't deploy to .NET
- You don't need to learn C# or ASP.NET
- You don't need Windows or Visual Studio

.NET is just the build toolchain. You:

1. Write F# code
2. Run `dotnet fable --lang python`
3. Get Python files
4. Run with `python`

Your deployment, your dependencies, your runtime - all Python.

### Ready to Start?

Now that you understand the basics, let's set up your first Fable.Python project
in the next chapter!

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
let join (paths: string[]) : string = nativeOnly
```

The `nativeOnly` placeholder tells Fable this will be resolved at runtime.

### Automatic Case Conversion

Fable automatically converts F# camelCase to Python snake_case:

```fsharp
type MyClass() =
    member _.myMethod() = "hello" // Becomes my_method() in Python
```

This keeps your F# code idiomatic while generating Pythonic output.

### The Emit Attribute

For direct Python code embedding, use `[<Emit>]`:

```fsharp
[<Emit("len($0)")>]
let pyLen (x: 'a) : int = nativeOnly

[<Emit("print($0, end=$1)")>]
let printWithEnd (value: string, ending: string) : unit = nativeOnly
```

The `$0`, `$1`, etc. are placeholders for arguments.

### Working with Python Types

#### Lists and Sequences

F# lists compile to Python lists, making interop natural:

```fsharp
let fsharpList = [ 1; 2; 3; 4; 5 ]
// Compiles to: [1, 2, 3, 4, 5]

let doubled = fsharpList |> List.map (fun x -> x * 2)
```

#### Dictionaries

F# Maps work with Python dicts:

```fsharp
let config = Map.ofList [ "host", "localhost"; "port", "8080" ]
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
let isString (x: obj) : bool = nativeOnly

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

let level = LogLevel.Info // Compiles to: "info"
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
let len (x: 'a) : int = nativeOnly

[<Emit("print($0)")>]
let pyPrint (x: 'a) : unit = nativeOnly

[<Emit("range($0)")>]
let range (n: int) : seq<int> = nativeOnly

[<Emit("open($0, $1)")>]
let openFile (path: string) (mode: string) : obj = nativeOnly
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
let numbers = [ 1; 2; 3; 4; 5 ]

// ResizeArray -> Python list (native)
let mutableList = ResizeArray<int>()
```

#### Functions and Lambdas

First-class functions work as expected:

```fsharp
let add x y = x + y
let multiply = fun x y -> x * y

let applyTwice f x = f (f x)
let result = applyTwice (add 1) 5 // 7
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
    | Rectangle(w, h) -> $"Rectangle {w}x{h}"
    | Triangle(a, b, c) -> $"Triangle with sides {a}, {b}, {c}"
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
    [ 1..10 ]
    |> List.filter (fun x -> x % 2 = 0)
    |> List.map (fun x -> x * x)
    |> List.sum

let arrayOps = [| 1; 2; 3 |] |> Array.map (fun x -> x + 1)

let setOps = Set.ofList [ 1; 2; 2; 3; 3; 3 ] // {1, 2, 3}

let mapOps = Map.ofList [ ("a", 1); ("b", 2) ]
```

### Limitations and Differences

#### Options Are Erased

Options are optimized away at runtime:

```fsharp
let someValue = Some 42 // Compiles to just: 42
let noneValue = None // Compiles to: None
```

This works fine for most cases, but be careful with nested options -
`Some None` vs `None` can be ambiguous.

#### Multi-line Lambdas

Python doesn't support multi-line lambdas. Fable lifts them to separate
functions:

```fsharp
// This F#:
let processed =
    [ 1; 2; 3 ]
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

## Fable v5: What's New

Fable v5 brings significant improvements to the Python target, with a focus on
correctness, modern Python support, and better interoperability.

### .NET 10 and F# 9.0 Support

Fable v5 uses native MSBuild for parsing projects instead of Buildalyzer.
This avoids creating fake .csproj files which could confuse IDEs.

Key improvements include:

- **Nullable Reference Types** - F# 9's compile-time null-safety
- **Many BCL additions** - Expanded .NET Base Class Library support
- **63 bug fixes** - Improved stability across all targets
- **300+ new tests** - Ensuring reliability

### Python Target Highlights

The Python target has received special attention in v5:

- **Python 3.12-3.14 support** (3.10/3.11 are deprecated)
- **fable-library via PyPI** - No more bundled runtime files
- **Modern type parameter syntax** - Better type hinting in generated code
- **`Py.Decorate` attribute** - Add Python decorators from F#
- **`Py.ClassAttributes` attribute** - Fine-grained class generation control
- **Improved Pydantic interop** - First-class support for data validation

### Rust Core with PyO3

One of the biggest changes is that the core of fable-library is now written
in **Rust** using PyO3. This isn't primarily for performance - it's for
**correctness**:

#### Why Rust?

- **Correct .NET semantics** - Sized/signed integers (int8, int16, int32, int64, uint8, etc.)
- **Proper overflow behavior** - Matches .NET exactly
- **Fixed-size arrays** - No more Python list quirks for byte streams
- **Reliable numerics** - Fable 4's pure Python numerics were a constant source of bugs

### fable-library via PyPI

Before Fable v5, the runtime was bundled in the NuGet package and copied
to your output directory. Now it's a simple pip/uv dependency:

```bash
# Install with pip
pip install fable-library

# Or with uv (recommended)
uv add fable-library
```

This makes dependency management much simpler and follows Python conventions.

### Test Coverage

Fable v5 significantly increased test coverage across all targets:

| Target         | Fable 4.9 | Fable 5 | Increase     |
| -------------- | --------- | ------- | ------------ |
| **JavaScript** | 2,589     | 2,748   | +159 (+6%)   |
| **Python**     | 1,880     | 1,974   | +94 (+5%)    |
| **Rust**       | 2,118     | 2,184   | +66 (+3%)    |

That's **319 new tests** ensuring reliability across the board.

### Getting Started with Fable v5

To use Fable v5, install the alpha CLI:

```bash
# Install Fable 5 CLI
dotnet tool install fable --version 5.0.0-alpha.17

# Add Fable.Core to your project
dotnet add package Fable.Core --version 5.0.0-beta.2

# Install the Python runtime
uv add fable-library==5.0.0a17
```

Then compile your F# to Python:

```bash
dotnet fable YourProject.fsproj --lang python -o output/
```

The generated Python code will be modern, type-hinted, and ready to run!

## Pydantic Interop

[Pydantic](https://docs.pydantic.dev/) is Python's most popular data validation
library. Fable v5 introduces new attributes that make F# and Pydantic work
together seamlessly.

### The Decorator Attribute

The `Py.Decorator` attribute lets you add Python decorators to F# types:

```fsharp
[<Py.Decorate("dataclasses.dataclass")>]
type Person = {
    Name: string
    Age: int
}
```

This generates:

```python
@dataclasses.dataclass
class Person:
    name: str
    age: int32
```

The decorator is applied directly to the generated Python class!

### Decorator with Parameters

You can also pass parameters to decorators:

```fsharp
[<Py.Decorate("dataclasses.dataclass", "frozen=True, slots=True")>]
type Point = {
    X: float
    Y: float
}
```

This generates:

```python
@dataclasses.dataclass(frozen=True, slots=True)
class Point:
    x: float
    y: float
```

The `frozen=True` makes instances immutable (matching F# record semantics),
and `slots=True` optimizes memory usage.

### ClassAttributes for Pydantic

The `Py.ClassAttributes` attribute controls how class members are generated,
which is essential for Pydantic compatibility:

```fsharp
[<Import("BaseModel", "pydantic")>]
type BaseModel() = class end

[<Py.ClassAttributes(style = Py.ClassAttributeStyle.Attributes, init = false)>]
type PydanticUser() =
    inherit BaseModel()
    member val Name: string = "" with get, set
    member val Age: int = 0 with get, set
    member val Email: string option = None with get, set
```

This generates clean Pydantic code:

```python
from pydantic import BaseModel

class PydanticUser(BaseModel):
    Age: int32 = int32.ZERO
    Email: str | None
    Name: str = ""
```

You get all of Pydantic's features:

- **Automatic validation** - Type checking at runtime
- **Serialization** - JSON/dict conversion built-in
- **Schema generation** - OpenAPI/JSON Schema support
- **IDE support** - Full autocomplete and type hints

### Why This Matters

This interop enables powerful patterns:

1. **Define models in F#** with full type safety and pattern matching
2. **Generate Python classes** that integrate with the Python ecosystem
3. **Use Pydantic validation** in FastAPI, LangChain, and other frameworks
4. **Publish to PyPI** - Your F# types become Python packages

### F# Option to Python Union

Notice how `string option` becomes `str | None` in Python. Fable v5 uses
modern Python union syntax for optional types, making the generated code
feel native to Python developers.

### Example: FastAPI Integration

These Pydantic models can be used directly with FastAPI:

```python
from fastapi import FastAPI
from your_fsharp_module import PydanticUser

app = FastAPI()

@app.post("/users")
def create_user(user: PydanticUser) -> PydanticUser:
    # Pydantic validates the request automatically
    return user
```

You get the best of both worlds: F#'s type safety during development,
and Python's rich ecosystem at runtime.

## Units of Measure

One of F#'s most powerful features for scientific and engineering code is
**units of measure** - compile-time dimensional analysis that prevents
unit-related bugs.

### The Problem

Unit errors are a classic source of bugs. The famous Mars Climate Orbiter
was lost because one team used metric units while another used imperial.
Python can't catch these errors:

```python
# Python - no protection
distance = 100  # meters? feet? who knows!
time = 9.58     # seconds? minutes?
speed = distance / time  # ???
```

### F# Units of Measure

F# lets you annotate numeric types with units that are checked at compile time:

```fsharp
[<Measure>]
type m // meters

[<Measure>]
type s // seconds

[<Measure>]
type kg // kilograms
```

Now we can define values with units:

```fsharp
let distance = 100.0<m>
let time = 9.58<s>
let speed = distance / time // Automatically inferred as float<m/s>
```

The compiler tracks units through all operations. Division of meters by
seconds gives meters-per-second. This is all checked at compile time!

### Preventing Errors

Try to add incompatible units and the compiler stops you:

```fsharp
let distance = 100.0<m>
let mass = 50.0<kg>

// This won't compile!
// let nonsense = distance + mass
// Error: The unit of measure 'm' does not match 'kg'
```

### Derived Units

You can define derived units based on existing ones:

```fsharp
[<Measure>]
type N = kg * m / s^2 // Newton

[<Measure>]
type J = N * m // Joule

let force = 10.0<N>
let displacement = 5.0<m>
let work = force * displacement // Inferred as float<J>
```

### Real-World Example: Physics Simulation

Here's a practical example computing kinetic energy:

```fsharp
let kineticEnergy (mass: float<kg>) (velocity: float<m / s>) : float<J> = 0.5 * mass * velocity * velocity

let carMass = 1500.0<kg>
let carSpeed = 30.0<m / s>
let energy = kineticEnergy carMass carSpeed
```

The function signature clearly documents what units are expected and returned.
The compiler ensures you can't accidentally pass velocity where mass is expected.

### Unit Conversions

Define conversion functions with explicit unit transformations:

```fsharp
[<Measure>]
type km

[<Measure>]
type h

let metersToKm (d: float<m>) : float<km> = d / 1000.0<m / km>
let secondsToHours (t: float<s>) : float<h> = t / 3600.0<s / h>

let marathonDistance = 42195.0<m>
let marathonKm = metersToKm marathonDistance // 42.195<km>
```

### Generated Python

When compiled to Python, units are erased (they're purely a compile-time
feature), but your code is guaranteed to be unit-safe:

```python
def kinetic_energy(mass: float, velocity: float) -> float:
    return 0.5 * mass * velocity * velocity

car_mass: float = 1500.0
car_speed: float = 30.0
energy: float = kinetic_energy(car_mass, car_speed)
```

The Python code is clean and efficient. All the unit checking happened
at compile time in F#, so there's no runtime overhead.

### Why This Matters for Python

Python is widely used in scientific computing, but lacks compile-time
unit checking. With Fable.Python, you can:

1. **Write unit-safe code** in F# with full dimensional analysis
2. **Catch unit errors at compile time** before they become runtime bugs
3. **Generate clean Python** that integrates with NumPy, SciPy, etc.
4. **Document intent** - function signatures show expected units

This is especially valuable for physics simulations, financial calculations,
engineering applications, and any domain where mixing up units could be costly.

## Fabletext

A literate programming converter inspired by [jupytext](https://github.com/mwouts/jupytext)
and [FSharp.Formatting](https://fsprojects.github.io/FSharp.Formatting/).

Converts F# literate files (`.fs` files with embedded Markdown) into
GitHub-flavored Markdown. Written in F# and compiles to Python via Fable.Python.

### How It Works

The converter is a simple state machine that processes input line by line:

- Lines inside `(** ... *)` blocks are emitted as Markdown
- F# code outside those blocks is wrapped in fenced code blocks
- `(*** hide ***)` sections are excluded from output

### Parser State

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

### Line Classification

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

### State Transitions

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

### Processing a File

Read all lines, process them, and return the Markdown output:

```fsharp
/// Processes all lines from a literate F# file and returns the Markdown output.
let processLines (lines: string seq) : string =
    let finalCtx = lines |> Seq.fold processLine emptyContext |> flushCodeBuffer // Flush any remaining code
    finalCtx.Output |> List.rev |> String.concat ""
```

### Header Level Adjustment

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

### Python File I/O

For Fable.Python, we use Python's file operations:

```fsharp
/// Reads the entire contents of a file as a string.
[<Emit("open($0, 'r').read()")>]
let readFile (path: string) : string = nativeOnly

/// Prints a string to stdout without a trailing newline.
[<Emit("print($0, end='')")>]
let printRaw (s: string) : unit = nativeOnly
```

### Main Entry Point

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

### Building and Running

```bash
# Transpile to Python
dotnet fable tools/ --lang python -o output/tools/

# Convert a literate file
python output/tools/fabletext.py chapters/01-introduction.fs > docs/01-introduction.md
```

That's it! A complete literate programming converter in under 200 lines of F#.
