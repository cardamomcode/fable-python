# F# Compatibility in Fable.Python

Understanding what works and what doesn't is crucial when targeting Python
with Fable. This chapter covers supported features, limitations, and
important differences from .NET.

## Fully Supported Features

### Core Types

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

### Functions and Lambdas

First-class functions work as expected:

```fsharp
let add x y = x + y
let multiply = fun x y -> x * y

let applyTwice f x = f (f x)
let result = applyTwice (add 1) 5 // 7
```

### Pattern Matching

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

### Records

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

### Discriminated Unions

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

### Object-Oriented Features

Classes, interfaces, inheritance, and overloading work:

```fsharp
type IShape =
    abstract member Area: float

type Circle2(radius: float) =
    member _.Radius = radius

    interface IShape with
        member _.Area = System.Math.PI * radius * radius
```

### Collections

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

## Limitations and Differences

### Options Are Erased

Options are optimized away at runtime:

```fsharp
let someValue = Some 42 // Compiles to just: 42
let noneValue = None // Compiles to: None
```

This works fine for most cases, but be careful with nested options -
`Some None` vs `None` can be ambiguous.

### Multi-line Lambdas

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

### Numeric Types

Most numerics use custom wrappers to maintain F# semantics. `bigint` uses
Python's native `int`:

```fsharp
let small: int = 42
let big: bigint = 12345678901234567890I
```

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
