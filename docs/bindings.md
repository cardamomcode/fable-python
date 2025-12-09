# Creating Python Bindings

When a Python library doesn't have F# bindings, you can create your own.
This chapter covers the patterns and best practices for writing type-safe
bindings that feel natural in F#.

## Core Principles

When writing bindings, follow these principles:

1. **Near-native F# experience** - Make the API feel like idiomatic F#
2. **Prefer overloads over union types** - Use multiple function overloads, not `U2<A,B>`
3. **Stay close to Python docs** - Users should be able to reference Python documentation
4. **Type safety first** - Leverage F#'s type system to catch errors at compile time

## The IExports Pattern

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

## Import Attributes

### ImportAll - Import Entire Module

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

### Import - Import Specific Member

`[<Import("member", "module")>]` imports a specific class or function:

```fsharp
[<Import("Path", "pathlib")>]
type Path =
    abstract exists: unit -> bool
    abstract is_file: unit -> bool
    abstract read_text: unit -> string
```

This generates: `from pathlib import Path`

### ImportMember - Import by Name

`[<ImportMember("module")>]` imports a member matching the F# value name:

```fsharp
[<ImportMember("datetime")>]
let datetime: obj = nativeOnly

// Generates: from datetime import datetime
```

## The Emit Attribute

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

## Function Overloads

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

## String Enums

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

### Case Rules

Without `[<CompiledName>]`, you can use case rules:

```fsharp
[<StringEnum(CaseRules.SnakeCase)>]
type FileMode =
    | ReadOnly // Compiles to: "read_only"
    | WriteOnly // Compiles to: "write_only"
    | ReadWrite // Compiles to: "read_write"
```

Available case rules: `None`, `LowerFirst`, `SnakeCase`, `KebabCase`.

## Named Parameters

For Python functions with keyword arguments, use `[<NamedParams>]`:

```fsharp
[<Erase>]
type IBuiltins =
    [<NamedParams(fromIndex = 1)>]
    abstract ``open``: file: string * ?mode: string * ?encoding: string -> obj
```

This generates: `open(file, mode=..., encoding=...)`

Parameters after `fromIndex` become keyword arguments.

## Binding Classes

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

## Complete Example: Binding requests

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

## Best Practices

1. **Document your bindings** - Add XML doc comments from Python docs
2. **Use F# naming conventions** - Fable converts camelCase to snake_case
3. **Test in Python** - Always verify the generated code works
4. **Keep bindings focused** - One module per Python package
5. **Handle None carefully** - Use `option` types for nullable returns

## File Organization

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

## What's Next?

Now you know how to create bindings. The **Compatibility** chapter covers
which F# features work with Fable.Python and any limitations to be aware of.
