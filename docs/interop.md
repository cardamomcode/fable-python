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
let data = {|
    name = "Alice"
    age = 30
|}
```

Anonymous records (`{| ... |}`) are perfect for JSON - they compile to
Python dictionaries. See the Compatibility chapter for details on how F#
types map to Python types.

## Calling Python Functions

### Basic Function Calls

Most Python functions can be called naturally through bindings:

```fsharp
open Fable.Python.Builtins

let length = builtins.len [ 1; 2; 3 ]
let absValue = builtins.abs (-42)
```

The `builtins` module provides typed access to Python's built-in functions.
These calls compile directly to `len([1, 2, 3])` and `abs(-42)` in Python.

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

The `os.path` functions work with arrays of path segments. These compile to
Python's `os.path.join`, `os.path.basename`, and `os.path.dirname` calls.

## Environment Variables

Use `os.getenv` to safely retrieve environment variables:

```fsharp
let home = os.getenv ("HOME", "")
let user = os.getenv "USER" // Returns string option
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

## Importing Python Modules

Fable provides several ways to import Python modules and functions.

### Using import Functions

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

### Using Import Attributes

For module-level imports, use attributes:

```fsharp
[<ImportAll("my_native_module")>]
let nativeModule: IMathModule = nativeOnly
```

The `nativeOnly` value is a placeholder - Fable replaces it with the actual import.

## Emit: Inline Python Code

When you need to write raw Python code, use `Emit`:

### The Emit Attribute

```fsharp
[<Emit("len($0)")>]
let pyLen (x: 'a) : int = nativeOnly

[<Emit("$0 + $1")>]
let pyAdd (x: int) (y: int) : int = nativeOnly

[<Emit("isinstance($0, $1)")>]
let pyIsInstance (obj: obj) (typ: obj) : bool = nativeOnly
```

The `$0`, `$1`, etc. are placeholders for the function arguments.

### emitPyExpr for Inline Expressions

For one-off expressions without defining a function:

```fsharp
let two: int = emitPyExpr (1, 1) "$0 + $1"
let hello: string = emitPyExpr () "\"Hello\""
```

### emitPyStatement for Multi-line Code

For more complex Python code with statements:

```fsharp
let factorial (count: int) : int =
    emitPyStatement
        count """if $0 < 2:
        return 1
    else:
        return $0 * factorial($0 - 1)
"""
```

## StringEnum: Type-Safe String Constants

`StringEnum` creates discriminated unions that compile to Python strings:

```fsharp
[<StringEnum>]
type Direction =
    | North
    | South
    | [<CompiledName("E")>] East  // Custom string value
    | West

// North compiles to "north", East compiles to "E"
```

### StringEnum with Case Rules

Control the string format with `CaseRules`:

```fsharp
[<StringEnum(CaseRules.SnakeCase)>]
type UserStatus =
    | ActiveUser      // -> "active_user"
    | InactiveUser    // -> "inactive_user"

[<StringEnum(CaseRules.KebabCase)>]
type CssBoxSizing =
    | ContentBox      // -> "content-box"
    | BorderBox       // -> "border-box"
```

Available case rules: `None`, `LowerFirst`, `SnakeCase`, `SnakeCaseAllCaps`, `KebabCase`, `LowerAll`.

## Erased Unions

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

## Python Decorators

Fable.Python supports Python decorators through several mechanisms.

### Creating F#-Side Decorators

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

### Using Py.Decorate for Python Decorators

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

## Class Attributes and DataClasses

### Py.ClassAttributes

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

### Py.DataClass Shorthand

`Py.DataClass` is shorthand for `ClassAttributes(Attributes, false)`:

```fsharp
[<Py.DataClass>]
type User2() =
    member val Username: string = "" with get, set
    member val Email: string = "" with get, set
```

### AttachMembers

Use `AttachMembers` to generate Python-style classes with methods directly attached:

```fsharp
[<AttachMembers>]
type Counter(initial: int) =
    let mutable count = initial

    member _.Count = count
    member _.Increment() = count <- count + 1
    member _.Decrement() = count <- count - 1
```

## Global Bindings

Bind to Python global objects with the `Global` attribute:

```fsharp
[<Global("list")>]
type PyList =
    [<Emit("$0.append($1)")>]
    abstract append: item: obj -> unit
    [<Emit("len($0)")>]
    abstract length: int
```

## Keyword Arguments with ParamObject

Use `ParamObject` to generate Python keyword arguments:

```fsharp
[<Erase>]
type IHttpClient =
    [<ParamObject(1)>]
    abstract fetch: url: string * ?timeout: int * ?headers: obj -> obj
```

When called as `client.fetch("http://...", timeout=30)`, this generates
Python code with keyword arguments: `client.fetch("http://...", timeout=30)`.

## createEmpty for Dynamic Objects

Create empty objects that can have properties set dynamically:

```fsharp
type IConfig =
    abstract host: string with get, set
    abstract port: int with get, set

let config = createEmpty<IConfig>
// config.host <- "localhost"
// config.port <- 8080
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

Now you know how to use existing Python bindings and core interop features.
In the next chapter, we'll learn how to create your own bindings for
Python libraries that don't have F# bindings yet.
