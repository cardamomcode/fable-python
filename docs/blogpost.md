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

Python is the [most popular programming language](https://www.tiobe.com/tiobe-index/)
in the world. And no matter what you think of Python, it will always be the second
best language for everything. That ubiquity is exactly why Fable.Python exists.

## When to Use Fable.Python

Fable.Python is a great choice when:

- **Python ecosystem access** - You need AI/ML libraries (PyTorch, TensorFlow,
  LangChain), data science tools (Pandas, NumPy), or frameworks like Pydantic and
  FastAPI
- **F# type safety** - You want pattern matching and exhaustive checking while using
  Python libraries
- **Shared domain logic** - Write once in F#, run on .NET, JavaScript, Rust, and Python
- **Publish to PyPI** - Your F# library can be available to the entire Python ecosystem
- **Units of measure** - F#'s compile-time dimensional analysis prevents unit errors
  that Python can't catch

## When NOT to Use Fable.Python

- When your F# code depends on .NET libraries without Fable support
- Performance-critical code (Python has runtime overhead)
- Team won't learn F#

**Best fit:** You love F#, but need Python's ecosystem.

## A First Example

Let's start with F# code that compiles to Python:

```fsharp
let greet name = $"Hello, {name}!"

let message = greet "Fable.Python"
```

When compiled with Fable, this generates the following Python:

```python
def greet[_A](name: Any | None = None) -> str:
    return ("Hello, " + str(name)) + "!"

message: str = greet("Fable.Python")
```

The `name: Any | None = None` signature may look odd at first. This happens because
F# infers the type from usage - since we only call `greet` with a string, the compiler
doesn't know if it might also be called with unit `()` (no argument). If it were,
Python would call it as `greet()` instead of `greet("Fable.Python")`. Adding an
explicit type annotation `let greet (name: string) = ...` would generate a cleaner
`name: str` parameter.

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

This compiles to Python while preserving the semantic meaning. The `Shape` type
becomes a tagged class structure, and the `match` expression becomes clean
conditional logic. The compiler ensures you handle all cases - if you add a
new shape variant, the compiler will warn you about unhandled cases in
the `area` function.

## What's Next?

In the following chapters, we'll cover:

- **Getting Started** - Setting up your development environment
- **Bindings** - Working with Python libraries from F#
- **Compatibility** - Understanding what F# features are supported

Let's begin.

## Are You a Python Developer?

If you're coming from Python, welcome. This chapter will help you understand
the F# code you'll see throughout this guide. F# is more approachable than
it might appear, and many concepts are familiar.

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
    | Rectangle(width, height) -> width * height
```

The compiler warns you if you forget to handle a case. No more runtime
`AttributeError` because you forgot a shape type.

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
let numbers = [ -1; 2; -3; 4; 5 ]

// F# pipeline - reads left to right, top to bottom
let result =
    numbers |> List.filter (fun x -> x > 0) |> List.map (fun x -> x * 2) |> List.sum
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
let findUser id : Person option = if id = 1 then Some alice else None

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
| Type annotation  | `x: int`            | `x: int32`.               |
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

- [.NET SDK](https://dotnet.microsoft.com/download) (6.0 or later. We recommend
  installing the latest LTS version, currently .NET 10
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
dotnet tool install fable --version 5.0.0-alpha.20

# Add Fable.Core package
dotnet add package Fable.Core --version 5.0.0-beta.4
```

### Install Python Dependencies

Fable-generated Python code requires the `fable-library` runtime:

```bash
pip install "fable-library==5.0.0a20"
```

---

**Note:** Version pinning matters. The fable-library version must match
your Fable compiler version. PyPI uses `5.0.0a20` format instead of `5.0.0-alpha.20`.

---

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

## Python Interop

Now that you have a Fable.Python project set up, let's explore how to work
with Python libraries and the existing bindings in the Fable.Python ecosystem.

### The Fable.Python Library

The [Fable.Python](https://github.com/fable-compiler/Fable.Python) NuGet package
provides ready-to-use bindings for Python's standard library. Add it to your project:

```bash
dotnet add package Fable.Python
```

This gives you typed access to modules like `os`, `sys`, `json`, `asyncio`, and more.

### Using Standard Library Modules

Here's how to use Python's `os` module:

```fsharp
open Fable.Python.Os

let currentDir = os.getcwd ()
let files = os.listdir "."
```

The bindings follow F# naming conventions, but Fable automatically converts
to Python's snake_case when generating code.

### Working with Python's json Module

For basic JSON operations, use Python's built-in `json` module:

```fsharp
open Fable.Python.Json

// Serialize F# data to JSON string
let data = {|
    name = "Alice"
    age = 30
|}
```

Anonymous records (`{| ... |}`) are perfect for JSON - they compile to
Python dictionaries. See the Compatibility chapter for details on how F#
types map to Python types.

### Calling Python Functions

#### Basic Function Calls

Most Python functions can be called naturally through bindings:

```fsharp
open Fable.Python.Builtins

let length = builtins.len [ 1; 2; 3 ]
let absValue = builtins.abs (-42)
```

The `builtins` module provides typed access to Python's built-in functions.
These calls compile directly to `len([1, 2, 3])` and `abs(-42)` in Python.

#### Working with sys Module

```fsharp
open Fable.Python.Sys

let pythonVersion = sys.version
let args = sys.argv
```

#### Path Operations with os.path

```fsharp
let fullPath = os.path.join [| "/home"; "user"; "file.txt" |]
let fileName = os.path.basename "/path/to/file.txt"
let dirName = os.path.dirname "/path/to/file.txt"
```

The `os.path` functions work with arrays of path segments. These compile to
Python's `os.path.join`, `os.path.basename`, and `os.path.dirname` calls.

### Environment Variables

Use `os.getenv` to safely retrieve environment variables:

```fsharp
let home = os.getenv ("HOME", "")
let user = os.getenv "USER" // Returns string option
```

### File Operations

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

### Type Conversions

#### Explicit Conversions

Sometimes you need to convert between F# and Python types explicitly:

```fsharp
// F# list to Python list (usually automatic)
let fsharpList = [ 1; 2; 3 ]

// When you need a ResizeArray specifically
let asResizeArray = ResizeArray(fsharpList)
```

#### Working with obj

When dealing with dynamic Python APIs, you may encounter `obj`:

```fsharp
let handleDynamic (value: obj) =
    // Pattern match on the actual type
    match value with
    | :? string as s -> $"Got string: {s}"
    | :? int as n -> $"Got int: {n}"
    | _ -> "Got something else"
```

### Importing Python Modules

Fable provides several ways to import Python modules and functions.

#### Using import Functions

The `import` function lets you import a specific member from a module:

```fsharp
open Fable.Core.PyInterop

// Import a specific function from a module
let add5: int -> int = import "add5" "my_module"

// Import all exports as an interface
type IMathModule =
    abstract add: int -> int -> int
    abstract multiply: int -> int -> int

let mathModule: IMathModule = importAll "math_utils"
```

#### Using Import Attributes

For module-level imports, use attributes:

```fsharp
[<ImportAll("my_native_module")>]
let nativeModule: IMathModule = nativeOnly
```

The `nativeOnly` value is a placeholder - Fable replaces it with the actual import.

### Emit: Inline Python Code

When you need to write raw Python code, use `Emit`:

#### The Emit Attribute

```fsharp
[<Emit("len($0)")>]
let pyLen (x: 'a) : int = nativeOnly

[<Emit("$0 + $1")>]
let pyAdd (x: int) (y: int) : int = nativeOnly

[<Emit("isinstance($0, $1)")>]
let pyIsInstance (obj: obj) (typ: obj) : bool = nativeOnly
```

The `$0`, `$1`, etc. are placeholders for the function arguments.

#### emitPyExpr for Inline Expressions

For one-off expressions without defining a function:

```fsharp
let two: int = emitPyExpr (1, 1) "$0 + $1"
let hello: string = emitPyExpr () "\"Hello\""
```

#### emitPyStatement for Multi-line Code

For more complex Python code with statements:

```fsharp
let factorial (count: int) : int =
    emitPyStatement
        count
        """if $0 < 2:
        return 1
    else:
        return $0 * factorial($0 - 1)
"""
```

### StringEnum: Type-Safe String Constants

`StringEnum` creates discriminated unions that compile to Python strings:

```fsharp
[<StringEnum>]
type Direction =
    | North
    | South
    | [<CompiledName("E")>] East // Custom string value
    | West

// North compiles to "north", East compiles to "E"
```

#### StringEnum with Case Rules

Control the string format with `CaseRules`:

```fsharp
[<StringEnum(CaseRules.SnakeCase)>]
type UserStatus =
    | ActiveUser // -> "active_user"
    | InactiveUser // -> "inactive_user"

[<StringEnum(CaseRules.KebabCase)>]
type CssBoxSizing =
    | ContentBox // -> "content-box"
    | BorderBox // -> "border-box"
```

Available case rules: `None`, `LowerFirst`, `SnakeCase`, `SnakeCaseAllCaps`, `KebabCase`, `LowerAll`.

### Erased Unions

Erased unions let you create type-safe wrappers that disappear at runtime:

```fsharp
[<Erase>]
type StringOrInt =
    | AsString of string
    | AsInt of int

    member this.Describe() =
        match this with
        | AsString s -> $"String: {s}"
        | AsInt n -> $"Int: {n}"

// AsString "hello" compiles to just "hello" in Python
// AsInt 42 compiles to just 42
```

This is useful for APIs that accept multiple types (like Python's duck typing).

### Python Decorators

Fable.Python supports Python decorators through several mechanisms.

#### Creating F#-Side Decorators

You can create custom decorators that wrap functions at compile time:

```fsharp
type LogAttribute(msg: string) =
    inherit Py.DecoratorAttribute()

    override _.Decorate(fn) =
        Py.argsFunc (fun args ->
            printfn $"LOG: {msg}"
            fn.Invoke(args))

[<Log("calling myFunction")>]
let myFunction x = x + 1
```

#### Using Py.Decorate for Python Decorators

Apply Python decorators to classes using `Py.Decorate`. The attribute takes
the decorator name, the module to import from, and optional parameters:

```fsharp
[<Py.Decorate("dataclass", "dataclasses")>]
[<Py.ClassAttributes(Py.ClassAttributeStyle.Attributes, false)>]
type DecoratedUser() =
    member val Name: string = "" with get, set
    member val Age: int = 0 with get, set
```

This generates:

```python
@dataclass
class DecoratedUser:
    Age: int32 = int32.ZERO
    Name: str = ""
```

### Class Attributes and DataClasses

#### Py.ClassAttributes

Control how class members are generated for Python frameworks like Pydantic:

```fsharp
[<Py.ClassAttributes(Py.ClassAttributeStyle.Attributes, false)>]
type PydanticModel() =
    member val Name: string = "" with get, set
    member val Age: int = 0 with get, set
```

This generates class-level type annotations suitable for Pydantic:

```python
class PydanticModel:
    Age: int32 = int32.ZERO
    Name: str = ""
```

#### Py.DataClass Shorthand

`Py.DataClass` is shorthand for `ClassAttributes(Attributes, false)`:

```fsharp
[<Py.DataClass>]
type User2() =
    member val Username: string = "" with get, set
    member val Email: string = "" with get, set
```

#### AttachMembers

Use `AttachMembers` to generate Python-style classes with methods directly attached:

```fsharp
[<AttachMembers>]
type Counter(initial: int) =
    let mutable count = initial

    member _.Count = count
    member _.Increment() = count <- count + 1
    member _.Decrement() = count <- count - 1
```

### Global Bindings

Bind to Python global objects with the `Global` attribute:

```fsharp
[<Global("list")>]
type PyList =
    [<Emit("$0.append($1)")>]
    abstract append: item: obj -> unit

    [<Emit("len($0)")>]
    abstract length: int
```

### Keyword Arguments with ParamObject

Use `ParamObject` to generate Python keyword arguments:

```fsharp
[<Erase>]
type IHttpClient =
    [<ParamObject(1)>]
    abstract fetch: url: string * ?timeout: int * ?headers: obj -> obj
```

When called as `client.fetch("http://...", timeout=30)`, this generates
Python code with keyword arguments: `client.fetch("http://...", timeout=30)`.

### createEmpty for Dynamic Objects

Create empty objects that can have properties set dynamically:

```fsharp
type IConfig =
    abstract host: string with get, set
    abstract port: int with get, set

let config = createEmpty<IConfig>
// config.host <- "localhost"
// config.port <- 8080
```

### Practical Example: Reading JSON Config

Here's a complete example combining several concepts:

```fsharp
let loadConfig (path: string) =
    let content = readFile path
    // Parse JSON and work with it
    json.loads content
```

### What's Next?

Now you know how to use existing Python bindings and core interop features.
In the next chapter, we'll learn how to create your own bindings for
Python libraries that don't have F# bindings yet.

## Creating Python Bindings

When a Python library doesn't have F# bindings, you can create your own.
This chapter covers the patterns and best practices for writing type-safe
bindings that feel natural in F#.

### Core Principles

When writing bindings, follow these principles:

1. **Near-native F# experience** - Make the API feel like idiomatic F#
2. **Prefer overloads over union types** - Use multiple function overloads, not `U2<A,B>`
3. **Stay close to Python docs** - Users should be able to reference Python documentation
4. **Type safety first** - Leverage F#'s type system to catch errors at compile time

### The IExports Pattern

The recommended pattern for binding a Python module uses an erased interface:

```fsharp
open Fable.Core

[<Erase>]
type IExports =
    abstract dumps: obj: obj -> string
    abstract loads: s: string -> obj

[<ImportAll("json")>]
let json: IExports = nativeOnly
```

This generates: `import json`

The `[<Erase>]` attribute means the interface only exists at compile time
(erased = no code generated for it). The `nativeOnly` placeholder tells Fable
the value will be resolved at runtime.

### Import Attributes

#### ImportAll - Import Entire Module

`[<ImportAll("module")>]` imports the module and binds it to a value:

```fsharp
[<Erase>]
type IOsExports =
    abstract getcwd: unit -> string
    abstract listdir: path: string -> string array

[<ImportAll("os")>]
let os: IOsExports = nativeOnly

// Usage: os.getcwd()
```

#### Import - Import Specific Member

`[<Import("member", "module")>]` imports a specific class or function:

```fsharp
[<Import("Path", "pathlib")>]
type Path =
    abstract exists: unit -> bool
    abstract is_file: unit -> bool
    abstract read_text: unit -> string
```

This generates: `from pathlib import Path`

#### ImportMember - Import by Name

`[<ImportMember("module")>]` imports a member matching the F# value name:

```fsharp
[<ImportMember("datetime")>]
let datetime: obj = nativeOnly

// Generates: from datetime import datetime
```

### The Emit Attribute

For Python syntax that can't be expressed with imports, use `[<Emit>]`:

```fsharp
[<Emit("len($0)")>]
let len (x: 'a) : int = nativeOnly

[<Emit("isinstance($0, $1)")>]
let isinstance (obj: obj) (typ: obj) : bool = nativeOnly

[<Emit("$0[$1]")>]
let getItem (obj: 'a) (key: 'b) : 'c = nativeOnly
```

The `$0`, `$1`, `$2` placeholders represent arguments in order.

For methods on objects, use `$0` for `self`:

```fsharp
[<Emit("$0.upper()")>]
let upper (s: string) : string = nativeOnly
```

### Function Overloads

**Why prefer overloads over erased unions?** Erased unions like `U2<string, bytes>`
require callers to wrap values explicitly, creating friction. Instead of:

```fsharp
// ❌ Avoid this - creates friction for callers
abstract parse: source: U2<string, bytes> -> AST
```

Use multiple overloads:

```fsharp
[<Erase>]
type IAstExports =
    // ✅ Multiple overloads - easy to call
    abstract parse: source: string -> obj
    abstract parse: source: string * filename: string -> obj
    abstract parse: source: string * filename: string * mode: string -> obj
```

This matches how Python's `ast.parse()` works - optional parameters
become additional overloads.

### String Enums

For Python APIs that use string constants, use `[<StringEnum>]`:

```fsharp
[<StringEnum>]
[<RequireQualifiedAccess>]
type HttpMethod =
    | [<CompiledName("GET")>] Get
    | [<CompiledName("POST")>] Post
    | [<CompiledName("PUT")>] Put
    | [<CompiledName("DELETE")>] Delete

let method = HttpMethod.Get // Compiles to: "GET"
```

The `[<CompiledName>]` attribute controls the exact string value.
Use `[<RequireQualifiedAccess>]` to avoid polluting the namespace.

#### Case Rules

Without `[<CompiledName>]`, you can use case rules:

```fsharp
[<StringEnum(CaseRules.SnakeCase)>]
type FileMode =
    | ReadOnly // Compiles to: "read_only"
    | WriteOnly // Compiles to: "write_only"
    | ReadWrite // Compiles to: "read_write"
```

Available case rules: `None`, `LowerFirst`, `SnakeCase`, `KebabCase`.

### Named Parameters

For Python functions with keyword arguments, use `[<NamedParams>]`:

```fsharp
[<Erase>]
type IBuiltins =
    [<NamedParams(fromIndex = 1)>]
    abstract ``open``: file: string * ?mode: string * ?encoding: string -> obj
```

This generates: `open(file, mode=..., encoding=...)`

Parameters after `fromIndex` become keyword arguments.

### Binding Classes

For Python classes you want to inherit from or instantiate:

```fsharp
[<Import("BaseModel", "pydantic")>]
type BaseModel() = class end
```

For classes with methods:

```fsharp
[<Import("Counter", "collections")>]
type Counter<'T> =
    abstract most_common: ?n: int -> ('T * int) array
    abstract update: iterable: 'T seq -> unit
```

### Complete Example: Binding requests

Here's how you might bind Python's `requests` library:

```fsharp
[<StringEnum>]
[<RequireQualifiedAccess>]
type RequestMethod =
    | [<CompiledName("GET")>] Get
    | [<CompiledName("POST")>] Post

type Response =
    abstract status_code: int
    abstract text: string
    abstract json: unit -> obj

[<Erase>]
type IRequestsExports =
    abstract get: url: string -> Response
    abstract get: url: string * headers: obj -> Response

    abstract post: url: string -> Response
    abstract post: url: string * data: string -> Response
    abstract post: url: string * data: string * headers: obj -> Response

[<ImportAll("requests")>]
let requests: IRequestsExports = nativeOnly
```

Usage would be:

```fsharp
let response = requests.get "https://api.example.com/data"
printfn "Status: %d" response.status_code
```

### Best Practices

1. **Document your bindings** - Add XML doc comments from Python docs
2. **Use F# naming conventions** - Fable converts camelCase to snake_case
3. **Test in Python** - Always verify the generated code works
4. **Keep bindings focused** - One module per Python package
5. **Handle None carefully** - Use `option` types for nullable returns

### File Organization

A typical binding module structure:

```fsharp
module Fable.Python.MyLibrary

open Fable.Core

// 1. Type aliases for complex types
type Callback = string -> unit

// 2. Supporting types (enums, records)
[<StringEnum>]
type Mode = | Fast | Slow

// 3. Class imports
[<Import("Client", "mylibrary")>]
type Client = ...

// 4. Module exports interface
[<Erase>]
type IExports = ...

// 5. Module import
[<ImportAll("mylibrary")>]
let myLibrary: IExports = nativeOnly

// 6. Convenience wrappers (optional)
let createClient url = myLibrary.createClient url
```

### What's Next?

Now you know how to create bindings. The **Compatibility** chapter covers
which F# features work with Fable.Python and any limitations to be aware of.

## F# Compatibility in Fable.Python

This chapter covers supported features, limitations, and important differences
from .NET when targeting Python with Fable.

### Common Types and Objects

Some F#/.NET types have counterparts in Python. Fable takes advantage of
this to compile to native types that are more performant and reduce code
size. Native types also simplify interop with Python code and libraries.
The most important common types are:

|       F#/.NET Type       |  Python Type  |              Notes              |
| ------------------------ | ------------- | ------------------------------- |
| `string`                 | `str`         | Behaves the same                |
| `bool`                   | `bool`        | Behaves the same                |
| `char`                   | `str`         | Compiled as string of length 1  |
| `Tuple`                  | `tuple`       | Native Python tuple             |
| `ResizeArray<T>`         | `list`        | Native Python list              |
| `Dictionary<K,V>`        | `dict`        | Native Python dict              |
| `seq<T>` / `IEnumerable` | `Iterable`    | Uses `__iter__` protocol        |
| `Array`                  | `FSharpArray` | Custom wrapper for F# semantics |

### .NET Base Class Library

Fable provides support for some .NET BCL classes. The following are translated
to Python with most methods available:

|                  .NET Type                   | Python Type |
| -------------------------------------------- | ----------- |
| `System.String`                              | `str`       |
| `System.Boolean`                             | `bool`      |
| `System.Char`                                | `str`       |
| `System.DateTime`                            | `datetime`  |
| `System.Decimal`                             | `decimal`   |
| `System.Collections.Generic.List<T>`         | `list`      |
| `System.Collections.Generic.Dictionary<K,V>` | `dict`      |

### FSharp.Core

Most FSharp.Core operators are supported, including formatting with `sprintf`,
`printfn`, and `failwithf`. The following types from FSharp.Core translate to Python:

|      F# Type      |           Python           |
| ----------------- | -------------------------- |
| `Tuple`           | `tuple`                    |
| `Option<T>`       | erased to `T \| None`      |
| `string`          | `str`                      |
| `List<T>`         | `List.fs` (immutable list) |
| `Map<K,V>`        | `Map.fs` (immutable map)   |
| `Set<T>`          | `Set.fs` (immutable set)   |
| `ResizeArray<T>`  | `list`                     |
| Record types      | `@dataclass`               |
| Anonymous Records | `dict`                     |

### Interfaces and Protocols

.NET interfaces map to Python protocols and special methods:

| .NET Interface |          Python          |               Purpose               |
| -------------- | ------------------------ | ----------------------------------- |
| `IEquatable`   | `__eq__`                 | Equality comparison                 |
| `IEnumerator`  | `__next__`               | Iterator protocol                   |
| `IEnumerable`  | `__iter__`               | For-loop iteration                  |
| `IComparable`  | `__lt__` + `__eq__`      | Ordering and sorting                |
| `IDisposable`  | `__enter__` + `__exit__` | Context managers (`with` statement) |
| `ToString()`   | `__str__`                | String representation               |

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

```python
greeting: str = "Hello, Python!"

is_enabled: bool = True

coordinates: tuple[float64, float64] = (float64(10.5), float64(20.3))

numbers: FSharpList[int32] = of_array(
    Array[int32]([int32.ONE, int32.TWO, int32.THREE, int32.FOUR, int32.FIVE])
)

mutable_list: list[int32] = []
```

Each of these F# values compiles to its Python equivalent. Strings become `str`,
booleans become `bool`, and tuples become Python tuples. The F# `list` uses the
fable-library implementation for immutable semantics, while `ResizeArray`
compiles directly to Python's mutable `list`.

#### Functions and Lambdas

First-class functions work as expected:

```fsharp
let add x y = x + y
let multiply = fun x y -> x * y

let applyTwice f x = f (f x)
let result = applyTwice (add 1) 5 // 7
```

Functions are first-class values in F#. The `applyTwice` function takes another
function `f` as a parameter and applies it twice. Partial application works
naturally - `(add 1)` creates a new function that adds 1 to its argument.

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

```python
@dataclass(eq=False, repr=False, slots=True)
class Person(Record):
    name: str
    age: int32
    email: str | None
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

Options are erased at runtime, which is actually a feature rather than a limitation.
This makes interop with Python libraries seamless - you can pass F# option values
directly to Python functions expecting `T | None`:

```fsharp
let someValue = Some 42 // Compiles to just: 42
let noneValue = None // Compiles to: None
```

This erasure means Python code receives native values without any wrapper overhead.
When calling a Python library that returns `Optional[T]`, you get values that work
directly with F# pattern matching.

For the rare edge case of nested options (`Option<Option<T>>`), Fable.Python uses
a `SomeWrapper` to distinguish `Some None` from `None`. However, nested options
are uncommon in practice - the F# compiler warns about them in type annotations,
and well-designed library bindings avoid exposing them at API boundaries.

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

We can see that the mapping becomes a separate function in the generated Python code.

```python
def mapping(x_1: int32) -> int32:
    return x_1 * x_1

processed: FSharpList[int32] = map(
    mapping, of_array(Array[int32]([int32.ONE, int32.TWO, int32.THREE]))
)
```

#### Numeric Types

Numeric types in Fable.Python are implemented using custom PyO3 wrapper types
written in Rust. These wrappers maintain F#-style semantics (like proper overflow
behavior) while integrating seamlessly with Python.

|       F# Type        | .NET Type  | Python Type |                 Notes                  |
| -------------------- | ---------- | ----------- | -------------------------------------- |
| `int`                | Int32      | Int32       | Custom wrapper with overflow semantics |
| `int64`              | Int64      | Int64       | Custom wrapper                         |
| `int16`              | Int16      | Int16       | Custom wrapper                         |
| `byte`               | Byte       | UInt8       | Custom wrapper                         |
| `sbyte`              | SByte      | Int8        | Custom wrapper                         |
| `uint16`             | UInt16     | UInt16      | Custom wrapper                         |
| `uint32`             | UInt32     | UInt32      | Custom wrapper                         |
| `uint64`             | UInt64     | UInt64      | Custom wrapper                         |
| `float` / `double`   | Double     | Float64     | Custom wrapper                         |
| `float32` / `single` | Single     | Float32     | Custom wrapper                         |
| `bigint`             | BigInteger | int         | Native Python type                     |
| `nativeint`          | IntPtr     | int         | Native Python type                     |

The wrapper types ensure type safety and correct arithmetic behavior:

```fsharp
let small: int = 42
let big: bigint = 12345678901234567890I

// Wrapper types maintain proper overflow semantics
let maxInt: int = System.Int32.MaxValue
let wrapped: int = maxInt + 1 // Wraps around like .NET

// bigint uses Python's native arbitrary-precision int
let huge: bigint = 999999999999999999999999999999I
```

This generates:

```python
small: int32 = int32(42)

big: int = 12345678901234567890

wrapped: int32 = max_int + int32.ONE

huge: int = 999999999999999999999999999999
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

This ensures the use of absolute imports in generated Python. Applications
in Python must use absolute imports to run correctly.

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
- Multi-line lambda lifting, will not be anonymous
- Some .NET APIs may be missing

For most F# code, you can write idiomatic functional code and it will
compile to clean, working Python.

## Async Programming

Asynchronous programming is essential for modern applications - from web APIs to data
processing pipelines. F# offers two models for async code: `async` workflows and `task`
expressions. Understanding when to use each is key to effective Fable.Python development.

### Comparing Python and F`#` Async Models

Python's async model is built on `asyncio`. Python coroutines are **cold** - calling an
`async def` function returns a coroutine object that doesn't execute until awaited:

```python
import asyncio

async def fetch_data():
    print("Starting")  # Not printed when function is called!
    await asyncio.sleep(1)
    return "data"

coro = fetch_data()    # Returns coroutine, nothing executes yet
result = await coro    # NOW "Starting" prints and code runs

# Or more commonly:
asyncio.run(fetch_data())
```

F# provides two computation expressions that compile to Python's async model:

- **`async { }`** - F#'s original async workflows (cold, composable, multi-target)
- **`task { }`** - .NET-style tasks (hot in .NET, compiles to native `async def` in Python)

### F`#` Async Workflows

The `async` computation expression has been part of F# since the beginning. It creates
*cold* async operations - they don't start until explicitly run.

```fsharp
open System

let fetchDataAsync () =
    async {
        do! Async.Sleep 1000
        return "data from async"
    }
```

Key characteristics of `async`:

- **Cold execution** - Nothing happens until you start it
- **Composable** - Combine with `Async.Parallel`, `Async.Sequential`, etc.
- **Multi-target** - The same code works on .NET, JavaScript, AND Python
- **Cancellation** - Built-in support via `CancellationToken`

#### Running Async Workflows

There are several ways to execute an async workflow:

```fsharp
let runAsyncExample () =
    // Start immediately (non-blocking) - Ignore discards the result
    fetchDataAsync () |> Async.Ignore |> Async.StartImmediate

    // Run synchronously (blocking)
    let result = fetchDataAsync () |> Async.RunSynchronously

    // Start with explicit continuations
    Async.StartWithContinuations(
        fetchDataAsync (),
        (fun result -> printfn $"Success: {result}"),
        (fun ex -> printfn $"Error: {ex.Message}"),
        (fun cancelled -> printfn "Cancelled")
    )
```

#### Combining Async Operations

F# async shines when composing multiple operations:

```fsharp
let fetchMultipleAsync () =
    async {
        let! results =
            [ fetchDataAsync ()
              fetchDataAsync ()
              fetchDataAsync () ]
            |> Async.Parallel

        return results |> Array.toList
    }
```

The `Async.Parallel` function runs all operations concurrently and waits for all to
complete. This is much cleaner than manually managing multiple coroutines in Python.

#### Error Handling in Async

Use `try...with` inside async blocks or `Async.Catch` for explicit error handling:

```fsharp
let safeAsync () =
    async {
        try
            do! Async.Sleep 100
            failwith "Something went wrong"
            return "success"
        with ex ->
            return $"Error: {ex.Message}"
    }

let catchExample () =
    async {
        let! result = safeAsync () |> Async.Catch

        match result with
        | Choice1Of2 value -> printfn $"Got: {value}"
        | Choice2Of2 ex -> printfn $"Failed: {ex.Message}"
    }
```

### F`#` Tasks

The `task` computation expression in .NET creates *hot* tasks that start immediately.
However, when compiled to Python via Fable, tasks become Python coroutines - which are
*cold* just like Python's native `async def` functions.

A key improvement in Fable v5 is that `task { }` now compiles to Python's native
`async def` syntax. Previously, Fable generated regular functions returning `Awaitable[T]`,
which frameworks like FastAPI couldn't recognize as async endpoints.

```fsharp
open System.Threading.Tasks

let processItemTask (item: string) =
    task {
        do! Task.Delay 100
        return item.ToUpper()
    }
```

This generates:
<!-- include-python: processItemTask (not found) -->
Now frameworks like FastAPI can detect and handle these as proper async endpoints.

#### Task vs Async: Key Differences

|      Aspect      |       `async { }`       |         `task { }`         |
| ---------------- | ----------------------- | -------------------------- |
| .NET execution   | Cold (lazy)             | Hot (immediate)            |
| Python execution | Cold                    | Cold (coroutines are cold) |
| Python output    | Wrapped awaitable       | Native `async def`         |
| Framework compat | Manual bridging         | Direct (FastAPI, etc.)     |
| Multi-target     | .NET, JS, Python        | .NET, Python               |
| Composition      | Rich (`Async.Parallel`) | Basic                      |

> **Why the difference?** In .NET, an `async` method is still a regular method - when you
> call it, the method body starts executing immediately until it hits an `await`. The
> returned `Task` represents work already in progress.
>
> In Python, `async def` creates a *coroutine function*. Calling it doesn't run the body -
> it returns a coroutine object (a generator-like structure). This coroutine is just a
> "recipe" that must be driven by an event loop via `await` or `asyncio.run()`.
>
> When Fable compiles F# `task` to Python `async def`, the cold Python semantics apply.
> The advantage of `task` for Python is the native `async def` signature that frameworks
> recognize.

#### Working with Tasks

```fsharp
let fetchDataTask () =
    task {
        do! Task.Delay 100 // Do some async work
        return "data from task"
    }

let taskExample () =
    task {
        let! result = fetchDataTask ()
        return $"Processed: {result}"
    }

let taskWithLoop () =
    task {
        let mutable sum = 0
        for i in 1..10 do
            sum <- sum + i
        return sum
    }
```

### Mapping to Python

Understanding how F# async constructs map to Python helps when debugging or integrating
with Python code.

#### Async Workflows → Python

F# `async` workflows compile to a wrapped async structure:

```fsharp
let simpleAsync () =
    async {
        do! Async.Sleep 500
        return 42
    }
```

In Python, this generates:

```python
def simple_async(__unit: None = None) -> Async[int32]:
    def _arrow58(__unit: None = None) -> Async[int32]:
        def _arrow57(__unit: None = None) -> Async[int32]:
            return singleton.Return(int32(42))

        return singleton.Bind(sleep(int32(500)), _arrow57)

    return singleton.Delay(_arrow58)
```

#### Tasks → Native async def

F# `task` expressions compile directly to Python's `async def`:

```fsharp
let simpleTask () =
    task {
        do! Task.Delay 500
        return 42
    }
```

In Python, this generates:
<!-- include-python: simpleTask (not found) -->
#### Running Tasks from F`#`

To run a task and get its result in F#:

```fsharp
let runTaskExample () =
    let tsk = simpleTask ()

    // Block and wait for result
    let result = tsk.GetAwaiter().GetResult()
    printfn $"Got: {result}"
```

You can also await tasks inside other tasks:

```fsharp
let chainedTasks () =
    task {
        let! first = simpleTask ()
        let! second = simpleTask ()
        return first + second
    }
```

#### Running in Python's Event Loop

When your compiled Python code runs, you'll need an event loop. For scripts:

```python
import asyncio

async def main():
    result = await simple_task()
    print(result)

asyncio.run(main())
```

For frameworks like FastAPI, the event loop is managed for you.

### Practical Patterns

#### Async HTTP Requests

Here's a pattern for async HTTP operations (assuming you have bindings for `aiohttp`):

```fsharp
// Simulated async HTTP - in real code you'd use aiohttp bindings
let fetchUrlAsync (url: string) =
    async {
        do! Async.Sleep 100  // Simulates network delay
        return $"Response from {url}"
    }

let fetchMultipleUrls (urls: string list) =
    async {
        let! responses =
            urls
            |> List.map fetchUrlAsync
            |> Async.Parallel

        return responses |> Array.toList
    }
```

#### Sequential vs Parallel

Choose based on whether operations are independent:

```fsharp
let sequentialProcessing items =
    async {
        let results = ResizeArray()
        for item in items do
            let! result = fetchUrlAsync item
            results.Add(result)
        return results |> Seq.toList
    }

let parallelProcessing items =
    async {
        let! results =
            items
            |> List.map fetchUrlAsync
            |> Async.Parallel
        return results |> Array.toList
    }
```

#### Cancellation

F# async supports cancellation via `CancellationToken`:

```fsharp
open System.Threading

let cancellableWork (token: CancellationToken) =
    async {
        for i in 1..100 do
            token.ThrowIfCancellationRequested()
            do! Async.Sleep 50
            printfn $"Step {i}"
        return "Completed"
    }

let runWithTimeout () =
    async {
        use cts = new CancellationTokenSource(2000)  // 2 second timeout
        try
            let! result = cancellableWork cts.Token
            return Some result
        with
        | :? OperationCanceledException ->
            return None
    }
```

### When to Use What

#### Use `task { }` for Python Interop

When working with Python frameworks that expect native async functions:

```fsharp
// FastAPI endpoint (see FastAPI chapter)
let getItemTask (itemId: int) =
    task {
        do! Task.Delay 10
        return {| id = itemId; name = "Widget" |}
    }
```

#### Use `async { }` for Multi-Target Code

When you want the same async code to work on Python, .NET, AND JavaScript:

```fsharp
// This code compiles to all Fable targets
let sharedBusinessLogic (input: string) =
    async {
        do! Async.Sleep 100
        let processed = input.ToUpper()
        return processed
    }
```

#### Use `async { }` for Composition

When you need rich composition primitives:

```fsharp
let complexWorkflow () =
    async {
        // Run three operations in parallel
        let! results =
            [ fetchDataAsync ()
              fetchDataAsync ()
              fetchDataAsync () ]
            |> Async.Parallel

        // Then do something sequential
        do! Async.Sleep 100

        return results |> Array.toList
    }
```

### Summary

|       Scenario       | Recommendation |
| -------------------- | -------------- |
| FastAPI endpoints    | `task { }`     |
| aiohttp/asyncio libs | `task { }`     |
| Multi-target library | `async { }`    |
| Complex composition  | `async { }`    |
| Cancellation-heavy   | `async { }`    |
| Simple one-off async | Either works   |

The key insight: **`task` for Python-native `async def` integration (FastAPI, etc.),
`async` for Fable portability and rich composition**. Both are cold in Python.

In the next chapter, we'll look at Fable v5 features that make Python development even
smoother.

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
in **Rust** using PyO3. The motivation is **correctness**, not performance:

#### Why Rust?

- **Correct .NET semantics** - Sized/signed integers (int8, int16, int32, int64, uint8, etc.)
- **Proper overflow behavior** - Matches .NET exactly
- **Fixed-size arrays** - No more Python list quirks for byte streams
- **Reliable numerics** - Fable 4's pure Python numerics were a constant source of bugs

While Rust is fast, don't expect dramatic speedups for typical F# code. Many F#
functions are higher-order and callback to Python - `List.map`, `List.filter`,
`Seq.fold`, etc. all invoke your Python lambdas. The Rust core handles the
data structures correctly; your code still runs at Python speed.

### fable-library via PyPI

Before Fable v5, the runtime was bundled in the NuGet package and copied
to your output directory. Now it's a simple pip/uv dependency:

```bash
# Install with pip
pip install fable-library

# Or with uv (recommended)
uv add fable-library
```

For projects, pin your dependencies in `pyproject.toml`. For stable releases use
a minimum version constraint:

```toml
dependencies = ["fable-library>=5.0.0"]
```

For alpha/beta releases, pin the exact version to avoid surprises:

```toml
dependencies = ["fable-library==5.0.0a21"]
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
dotnet tool install fable --version 5.0.0-alpha.21

# Add Fable.Core to your project
dotnet add package Fable.Core --version 5.0.0-beta.4

# Install the Python runtime
uv add fable-library==5.0.0a17
```

Then compile your F# to Python:

```bash
dotnet fable YourProject.fsproj --lang python -o output/
```

The generated Python code will be modern, type-hinted, and ready to run.

## Pydantic Interop

### What is Pydantic?

[Pydantic](https://docs.pydantic.dev/) is Python's most popular data validation
library. It's the de facto standard for modern Python APIs - FastAPI, LangChain,
and countless other frameworks rely on it.

Pydantic gives you:

- **Runtime type validation** - Catch bad data before it causes problems
- **Automatic serialization** - JSON/dict conversion built-in
- **Schema generation** - OpenAPI/JSON Schema for free
- **IDE support** - Full autocomplete from type hints

Fable v5 introduces attributes that make F# and Pydantic work together seamlessly.

### Creating Models in F`#`

#### Using ClassAttributes

The `Py.ClassAttributes` attribute controls how class members are generated,
which is essential for Pydantic compatibility:

```fsharp
[<Py.ClassAttributes(style = Py.ClassAttributeStyle.Attributes, init = false)>]
type User() =
    inherit BaseModel()
    member val Name: string = "" with get, set
    member val Age: int = 0 with get, set
    member val Email: string option = None with get, set
```

This generates clean Pydantic code:

```python
from pydantic import BaseModel

class User(BaseModel):
    Name: str = ""
    Age: int = 0
    Email: str | None = None
```

The `style = Attributes` tells Fable to generate class-level attributes (what
Pydantic expects) rather than instance attributes set in `__init__`.

#### The Decorator Attribute

For simpler cases like dataclasses, use `Py.Decorate`:

```fsharp
[<Py.Decorate("dataclasses.dataclass")>]
type Person = {
    Name: string
    Age: int
}
```

This generates:

```python
@dataclass(eq=False, repr=False, slots=True)
class Person(Record):
    name: str
    age: int32
```

You can pass parameters to decorators:

```fsharp
[<Py.Decorate("dataclasses.dataclass", "frozen=True, slots=True")>]
type Point = {
    X: float
    Y: float
}
```

The `frozen=True` makes instances immutable (matching F# record semantics).

### Fields and Validation

Pydantic's `Field()` function lets you add constraints and metadata to fields.
The `Fable.Python.Pydantic` module provides typed helpers:

```fsharp
[<Py.ClassAttributes(style = Py.ClassAttributeStyle.Attributes, init = false)>]
type Product() =
    inherit BaseModel()

    member val Name: string = "" with get, set

    // Field with description
    member val Description: Field<string> =
        Field.Description "Product description" with get, set

    // Field with numeric constraints
    member val Price: Field<float> =
        Field.Ge 0.0 with get, set  // price >= 0

    // Field with string constraints
    member val Sku: Field<string> =
        Field.Pattern "^[A-Z]{2}-[0-9]{4}$" with get, set  // e.g., "AB-1234"
```

Available field constraints:

|      Function       |      Constraint       |
| ------------------- | --------------------- |
| `Field.Gt`          | Greater than          |
| `Field.Ge`          | Greater than or equal |
| `Field.Lt`          | Less than             |
| `Field.Le`          | Less than or equal    |
| `Field.MinLength`   | Minimum string length |
| `Field.MaxLength`   | Maximum string length |
| `Field.Pattern`     | Regex pattern         |
| `Field.Default`     | Default value         |
| `Field.Description` | Field description     |

### Importing Python-Defined Models

Sometimes you need to use Pydantic models defined in Python - perhaps from an
OpenAPI generator, a Python team, or an existing codebase. Here's the pattern:

Given a Python model in `models.py`:

```python
from pydantic import BaseModel

class Customer(BaseModel):
    id: int
    name: str
    email: str | None = None
```

Create F# bindings:

```fsharp
/// Customer model imported from models.py
[<Import("Customer", "models")>]
type Customer =
    abstract id: int with get, set
    abstract name: string with get, set
    abstract email: string option with get, set

/// Helper module for creating instances
[<RequireQualifiedAccess>]
module Customer =
    [<Import("Customer", "models")>]
    [<Emit("$0(id=$1, name=$2, email=$3)")>]
    let create (id: int) (name: string) (email: string option) : Customer = nativeOnly
```

Now you can use the Python model from F# with full type safety:

```fsharp
let customer = Customer.create 1 "Alice" (Some "alice@example.com")

let showCustomer (c: Customer) =
    printfn "Customer %d: %s" c.id c.name
    match c.email with
    | Some email -> printfn "  Email: %s" email
    | None -> printfn "  No email on file"
```

This pattern is useful when you want to:

- Use models generated from OpenAPI specs
- Integrate with an existing Python codebase
- Share models between Python and F# code

### Type Mappings

F# types map naturally to Python/Pydantic types:

|   F# Type   | Python Type  |             Notes              |
| ----------- | ------------ | ------------------------------ |
| `string`    | `str`        |                                |
| `int`       | `int`        |                                |
| `float`     | `float`      |                                |
| `bool`      | `bool`       |                                |
| `'T option` | `T \| None`  | Modern union syntax            |
| `'T list`   | `list[T]`    |                                |
| `'T array`  | `list[T]`    |                                |
| Record      | `class`      | With `@dataclass` or BaseModel |
| DU          | Tagged class | See below                      |

#### F# Option to Python Union

Notice how `string option` becomes `str | None` in Python. Fable v5 uses
modern Python union syntax for optional types, making the generated code
feel native to Python developers.

### Serialization

Pydantic models have built-in serialization methods:

```fsharp
let serializationExample () =
    let user = User()
    user.Name <- "Alice"
    user.Age <- 30
    user.Email <- Some "alice@example.com"

    // Convert to dictionary
    let dict = user.model_dump()

    // Convert to JSON string
    let json = user.model_dump_json()

    // Pretty-printed JSON
    let prettyJson = user.model_dump_json_indented 2

    printfn "JSON: %s" json
```

The `model_dump()` and `model_dump_json()` methods are available on any
class that inherits from `BaseModel`.

### The DTO Boundary Pattern

A Pydantic model is not your domain - it's a **Data Transfer Object (DTO)**.
This distinction is important for well-architected applications:

```text
┌─────────────────┐         ┌─────────────────┐         ┌─────────────────┐
│   F# Domain     │   →→→   │   Pydantic DTO  │   →→→   │   JSON / API    │
│                 │   map   │                 │   dump  │                 │
│  UserId (Guid)  │         │  Id: str        │         │  "id": "a1b2.." │
│  Age: int32     │         │  Age: int       │         │  "age": 42      │
│  Balance: Money │         │  Amount: float  │         │  "amount": 3.14 │
└─────────────────┘         └─────────────────┘         └─────────────────┘
```

#### Different Concerns, Different Types

|  Concern   |        Domain Types        |        Transfer Types        |
| ---------- | -------------------------- | ---------------------------- |
| Purpose    | Model business logic       | Cross-boundary communication |
| Semantics  | Rich (overflow, precision) | Simple (JSON-compatible)     |
| Validation | Business rules             | Schema conformance           |
| Stability  | Can evolve internally      | API contract                 |

#### Domain Types vs DTO Types

```fsharp
/// Domain model - uses precise F# types
type UserId = UserId of System.Guid

type Money = { Amount: decimal; Currency: string }

type DomainUser = {
    Id: UserId
    Name: string
    Age: int32           // Bounded, wrapping arithmetic
    Balance: Money
}

/// DTO - uses Python-native types for serialization
[<Py.ClassAttributes(style = Py.ClassAttributeStyle.Attributes, init = false)>]
type UserDTO() =
    inherit BaseModel()
    member val Id: string = "" with get, set
    member val Name: string = "" with get, set
    member val Age: int = 0 with get, set
    member val BalanceAmount: float = 0.0 with get, set
    member val BalanceCurrency: string = "" with get, set
```

#### The Mapping Layer

Explicit transformation between domain and DTO:

#### Why This Pattern?

The "boilerplate" of separate DTO types is actually valuable:

1. **Serialization just works** - DTOs use Python-native types
2. **Domain integrity preserved** - Your `int32` still has proper wrapping behavior
3. **Clear boundaries** - The mapping layer handles validation and transformation
4. **API evolution** - DTOs can change independently of domain types

The visual difference between F# records and Pydantic classes is a **feature** -
it's a speed bump that makes you think about the boundary you're crossing.

### Why This Matters

This interop enables powerful patterns:

1. **Define models in F#** with full type safety and pattern matching
2. **Generate Python classes** that integrate with the Python ecosystem
3. **Use Pydantic validation** in FastAPI, LangChain, and other frameworks
4. **Publish to PyPI** - Your F# types become Python packages

You get the best of both worlds: F#'s type safety during development,
and Python's rich ecosystem at runtime.

In the next chapter, we'll see how to use these Pydantic models with FastAPI
to build type-safe web APIs.

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
seconds gives meters-per-second. This is all checked at compile time.

### Preventing Errors

Try to add incompatible units and the compiler stops you:

```fsharp
let distance = 100.0<m>
let mass = 50.0<kg>

// This won't compile:
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

## Fable.Literate: The Strange Loop

You've made it to the end - and here's where things get delightfully meta.

**The blog post you're reading was generated by the code in this chapter.**

This is Fable.Literate, a literate programming converter inspired by
[jupytext](https://github.com/mwouts/jupytext) and
[FSharp.Formatting](https://fsprojects.github.io/FSharp.Formatting/).
It's written in F#, compiled to Python via Fable, and it processes the
`.fs` files that make up this blog - including itself.

The chain goes like this:

1. Each chapter is an F# file with embedded Markdown comments
2. Fable compiles the F# to Python
3. Fable.Literate (this code, running as Python) extracts the documentation
4. The output is the Markdown you're reading right now

It's a strange loop - the snake eating its tail. And it proves that
Fable.Python isn't just a toy: you're looking at a real project that works.

### How It Works

The converter follows a compiler-like architecture with three phases:

1. **Parse**: Convert source lines into a Block AST
2. **Transform**: Filter hidden blocks, resolve Python includes
3. **Print**: Render the AST as Markdown

The input syntax:

- Lines inside `(** ... *)` blocks become Markdown
- F# code outside those blocks is wrapped in fenced code blocks
- `(*** hide ***)` sections are excluded from output
- `(*** include-python: symbol1, symbol2 ***)` extracts generated Python code

### AST Types

The document is represented as a list of blocks. Each block represents
a distinct section of the literate source file:

```fsharp
/// A single block in the document AST.
type Block =
    /// Raw markdown content from (** ... *) blocks
    | Markdown of content: string
    /// F# code that should be wrapped in fenced blocks
    | FSharpCode of lines: string list
    /// Hidden content - filtered out by Transform.filterHidden
    | Hidden of lines: string list
    /// Unresolved directive to include Python symbols (from parsing)
    /// Resolved to PythonCode by Transform.resolvePythonIncludes
    | IncludePython of symbols: string list
    /// Resolved Python code (after Transform.resolvePythonIncludes)
    | PythonCode of content: string

/// A parsed document is a list of blocks.
type Document = Block list
```

### Utils Module

Utility functions for naming conversion and line classification:

### Parser Module

The parser converts source lines into a Block AST using a fold:

### Transform Module

Pure transformations on the document AST:

### MarkdownPrinter Module

Renders the document AST to markdown:

### Pipeline Module

Composes the phases into a complete pipeline:

### Including Generated Python Code

One of Fable.Literate's unique features is the ability to show the generated Python
alongside the F# source. The include-python directive extracts specific symbols
from the transpiled output.

When you pass `--python-file path` to Fable.Literate, it reads the transpiled
Python and extracts the named symbols (functions, classes, or variables).
This lets readers see exactly what Python code Fable generates from the F#.

The extraction is smart about Python syntax:

- It finds the symbol definition by matching patterns like def symbol or class symbol
- It walks backwards to include any decorators
- For multi-line definitions, it captures everything until the next top-level definition
- It stops before dunder methods to avoid pulling in too much

For example, the extractSymbol function in F# generates this Python:

```python
def extract_symbol(symbol: str, lines: Array[str]) -> str | None:
    """Extracts a single symbol definition from Python source lines."""

    def mapping(def_index: int32, symbol: Any = symbol, lines: Any = lines) -> str:
        start_index: int32 = find_decorator_start(lines, def_index)
        if is_multiline_definition(lines[def_index]):
            return extract_multiline_body(start_index, def_index, lines)

        else:
            return lines[def_index]

    return map(mapping, find_definition_index(symbol, lines))
```

### Main Entry Point

Read the input file, convert it, and print the result:

```fsharp
/// Gets the value following a flag argument (e.g., --python-file path.py).
let getFlagValue (flag: string) (args: string[]) : string option =
    // Find the index of the flag in args
    args
    |> Array.tryFindIndex ((=) flag)
    // Return the next argument if it exists
    |> Option.bind (fun i -> if i + 1 < args.Length then Some args.[i + 1] else None)

/// Extracts positional arguments (file paths) from command line args.
/// Filters out flags (--foo) and their values (--python-file path.py).
let getPositionalArgs (args: string[]) : string[] =
    let isFlag (arg: string) = arg.StartsWith "--"
    let isValueOfFlag i = i > 0 && args.[i - 1] = "--python-file"

    // Pair each argument with its index
    args
    |> Array.indexed
    // Keep only non-flags that aren't values of flags
    |> Array.filter (fun (i, arg) -> not (isFlag arg) && not (isValueOfFlag i))
    // Extract just the argument strings
    |> Array.map snd

/// Main entry point. Converts a literate F# file to Markdown.
/// Use --increase-headers flag to bump all header levels by one.
/// Use --python-file <path> to enable include-python directives.
[<EntryPoint>]
let main (args: string[]) =
    let hasFlag flag = args |> Array.contains flag
    let pythonFilePath = getFlagValue "--python-file" args
    let files = getPositionalArgs args

    if files.Length < 1 then
        printfn "Usage: python app.py [--increase-headers] [--python-file <path.py>] <input.fs>"
        1
    else
        // Thanks to the contributor! (Fable-style)
        eprintln $"Fable.Literate: Thanks to the contributor! {randomContributor ()}"

        // Load Python file content if provided
        let pythonContent = pythonFilePath |> Option.map readFile

        let content = readFile files.[0]
        let lines = content.Split('\n')

        // Pipeline: parse -> transform -> print
        let markdown = lines |> Pipeline.standard pythonContent

        let output =
            if hasFlag "--increase-headers" then
                MarkdownPrinter.adjustHeaderLevels markdown
            else
                markdown

        printRaw output
        0
```

### Building and Running

```bash
# Transpile to Python
dotnet fable Fable.Literate/ --lang python -o output/Fable.Literate/

# Convert a literate file
python output/Fable.Literate/app.py chapters/introduction.fs > docs/introduction.md
```

That's it! A complete literate programming converter in under 200 lines of F#.

### The Punchline

If you're reading this, the code worked.

This entire blog post - every chapter, every code example, every explanation -
was processed by the F# code you just read, compiled to Python, and output
as Markdown. The proof is in the reading.

Welcome to Fable.Python. Now go build something.
