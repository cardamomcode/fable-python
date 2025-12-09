module Introduction

(**
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
- Performance-critical code (Python has runtime overhead)
- Team won't learn F#

**Best fit:** You love F#, but need Python's ecosystem.

## A First Example

Let's start with F# code that compiles to Python:
*)

let greet name = $"Hello, {name}!"

let message = greet "Fable.Python"

(**
When compiled with Fable, this generates the following Python:
*)

(*** include-python: greet, message ***)

(**
The `name: Any | None = None` signature may look odd at first. This happens because
F# infers the type from usage - since we only call `greet` with a string, the compiler
doesn't know if it might also be called with unit `()` (no argument). If it were,
Python would call it as `greet()` instead of `greet("Fable.Python")`. Adding an
explicit type annotation `let greet (name: string) = ...` would generate a cleaner
`name: str` parameter.

## The Power of Types

F# shines when modeling domain concepts. Consider this example:
*)

type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float

let area shape =
    match shape with
    | Circle radius -> System.Math.PI * radius * radius
    | Rectangle(width, height) -> width * height

let shapes = [ Circle 5.0; Rectangle(3.0, 4.0) ]

let totalArea = shapes |> List.sumBy area

(**
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
*)
