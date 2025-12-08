module Compatibility

(**
# F# Compatibility in Fable.Python

Understanding what works and what doesn't is crucial when targeting Python
with Fable. This chapter covers supported features, limitations, and
important differences from .NET.

## Common Types and Objects

Some F#/.NET types have counterparts in Python. Fable takes advantage of this
to compile to native types that are more performant and reduce code size.
The most important common types are:

|       F#/.NET Type       |  Python Type  |              Notes              |
| ------------------------ | ------------- | ------------------------------- |
| `string`                 | `str`         | Behaves the same                |
| `bool`                   | `bool`        | Behaves the same                |
| `char`                   | `str`         | Compiled as string of length 1  |
| `Tuple`                  | `tuple`       | Native Python tuple             |
| `ResizeArray<T>`         | `list`        | Native Python list              |
| `Dictionary<K,V>`        | `dict`        | Native Python dict              |
| `seq<T>` / `IEnumerable` | iterator      | Uses `__iter__` protocol        |
| `Array`                  | `FSharpArray` | Custom wrapper for F# semantics |

## .NET Base Class Library

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

## FSharp.Core

Most FSharp.Core operators are supported, including formatting with `sprintf`,
`printfn`, and `failwithf`. The following types from FSharp.Core translate to Python:

|      F# Type      |           Python           |
| ----------------- | -------------------------- |
| `Tuple`           | `tuple`                    |
| `Option<T>`       | erased (see caveats)       |
| `string`          | `str`                      |
| `List<T>`         | `List.fs` (immutable list) |
| `Map<K,V>`        | `Map.fs` (immutable map)   |
| `Set<T>`          | `Set.fs` (immutable set)   |
| `ResizeArray<T>`  | `list`                     |
| Record types      | `@dataclass`               |
| Anonymous Records | `dict`                     |

## Interfaces and Protocols

.NET interfaces map to Python protocols and special methods:

| .NET Interface |          Python          |               Purpose               |
| -------------- | ------------------------ | ----------------------------------- |
| `IEquatable`   | `__eq__`                 | Equality comparison                 |
| `IEnumerator`  | `__next__`               | Iterator protocol                   |
| `IEnumerable`  | `__iter__`               | For-loop iteration                  |
| `IComparable`  | `__lt__` + `__eq__`      | Ordering and sorting                |
| `IDisposable`  | `__enter__` + `__exit__` | Context managers (`with` statement) |
| `ToString()`   | `__str__`                | String representation               |

## Fully Supported Features

### Core Types

These F# types map directly to Python equivalents:
*)

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

(*** include-python: greeting, is_enabled, coordinates, numbers, mutable_list ***)

(**
### Functions and Lambdas

First-class functions work as expected:
*)

let add x y = x + y
let multiply = fun x y -> x * y

let applyTwice f x = f (f x)
let result = applyTwice (add 1) 5 // 7

(**
### Pattern Matching

Full pattern matching support:
*)

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

(**
### Records

Records compile to Python dataclasses:
*)

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

(*** include-python: Person ***)

(**
### Discriminated Unions

DUs are fully supported with pattern matching:
*)

type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float
    | Triangle of a: float * b: float * c: float

let describe shape =
    match shape with
    | Circle r -> $"Circle with radius {r}"
    | Rectangle(w, h) -> $"Rectangle {w}x{h}"
    | Triangle(a, b, c) -> $"Triangle with sides {a}, {b}, {c}"

(**
### Object-Oriented Features

Classes, interfaces, inheritance, and overloading work:
*)

type IShape =
    abstract member Area: float

type Circle2(radius: float) =
    member _.Radius = radius

    interface IShape with
        member _.Area = System.Math.PI * radius * radius

(**
### Collections

Core collection operations are supported:
*)

let listOps =
    [ 1..10 ]
    |> List.filter (fun x -> x % 2 = 0)
    |> List.map (fun x -> x * x)
    |> List.sum

let arrayOps = [| 1; 2; 3 |] |> Array.map (fun x -> x + 1)

let setOps = Set.ofList [ 1; 2; 2; 3; 3; 3 ] // {1, 2, 3}

let mapOps = Map.ofList [ ("a", 1); ("b", 2) ]

(**
## Limitations and Differences

### Options Are Erased

Options are optimized away at runtime:
*)

let someValue = Some 42 // Compiles to just: 42
let noneValue = None // Compiles to: None

(**
Note that Fable.Python uses a `SomeWrapper` class to handle nested options correctly.
`Some None` compiles to `SomeWrapper(None)`, which is distinct from plain `None`.
This means `Some (Some x)`, `Some None`, and `None` are all properly distinguishable.

### Multi-line Lambdas

Python doesn't support multi-line lambdas. Fable lifts them to separate
functions:
*)

// This F#:
let processed =
    [ 1; 2; 3 ]
    |> List.map (fun x ->
        let doubled = x * 2
        let squared = doubled * doubled
        squared)

// Becomes a separate function in Python

(**
### Numeric Types

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
*)

let small: int = 42
let big: bigint = 12345678901234567890I

// Wrapper types maintain proper overflow semantics
let maxInt: int = System.Int32.MaxValue
let wrapped: int = maxInt + 1 // Wraps around like .NET

// bigint uses Python's native arbitrary-precision int
let huge: bigint = 999999999999999999999999999999I

(**
### Computation Expressions

Async and task computation expressions have some differences from .NET.
Use `Async.StartAsTask` for Python compatibility.

## Project Configuration

### Entry Point Applications

If your project has `[<EntryPoint>]`, you need:

```xml
<PropertyGroup>
    <OutputType>Exe</OutputType>
</PropertyGroup>
```

This ensures absolute imports in generated Python.

### Libraries

Libraries use relative imports by default, which is correct for packages.

## Best Practices

1. **Test in Python** - Always test generated code in Python, not just in F#
2. **Avoid reflection** - Reflection has limited support
3. **Use type annotations** - Helps with debugging generated code
4. **Check fable-library** - Some .NET APIs may not be implemented yet

## Summary

Fable.Python provides excellent F# support. The main things to watch for are:

- Option erasure in edge cases
- Multi-line lambda lifting
- Some .NET APIs may be missing

For most F# code, you can write idiomatic functional code and it will
compile to clean, working Python.
*)
