module Introduction

(**
# Introduction to Fable.Python

> This post is part of the [F# Advent Calendar
2025](https://sergeytihon.com/2025/11/03/f-advent-calendar-in-english-2025/). Thank you, Sergey Tihon, for organizing
this wonderful tradition that brings the F# community together every year!

This guide covers [Fable](https://fable.io/) and [Fable.Python](https://github.com/fable-compiler/Fable.Python/) -
a compiler that transforms F# code into Python.

## Table of Contents

1. [F# for Python Developers](#heading-are-you-a-python-developer) - Core concepts if you're coming from Python
2. [Getting Started](#heading-getting-started-with-fablepython) - Installation and your first project
3. [Python Interop](#heading-python-interop) - Calling Python libraries from F#
4. [Creating Bindings](#heading-creating-python-bindings) - Type-safe wrappers for Python packages
5. [F# Compatibility](#heading-f-compatibility-in-fablepython) - What works, what doesn't
6. [Async Programming](#heading-async-programming) - F# async and Python asyncio
7. [Testing](#heading-testing-fablepython-projects) - Using pytest with F# code
8. [Fable v5](#heading-fable-v5-whats-new) - New features and the Rust core
9. [Pydantic Integration](#heading-pydantic-interop) - Type-safe data validation
10. [FastAPI](#heading-fastapi) - Building type-safe web APIs in the Python ecosystem
11. [Units of Measure](#heading-units-of-measure) - Compile-time dimensional analysis
12. [Fable.Literate](#heading-fableliterate-the-strange-loop) - The tool that wrote this post

**A teaser:** the final chapter reveals how this entire blog post was generated. The converter that transforms F#
literate files into Markdown is itself written in F#, compiled to Python with Fable, and documented using its own
output format. It's turtles all the way down.

## What is Fable?

[Fable](https://fable.io/) is a compiler that brings F# to different platforms and ecosystems. While
Fable is best known for compiling F# to TypeScript and JavaScript, it also supports other targets
including Python, Rust, and Dart.

## Why Fable.Python?

F# is a functional-first language with powerful features like:

- **Type inference** - Write less, express more
- **Pattern matching** - Elegant handling of complex data
- **Immutability by default** - Safer, more predictable code
- **Algebraic data types** - Model your domain precisely with discriminated unions and records

These features make F# excellent for [Domain Modeling](https://www.pragprog.com/titles/swdddf/domain-modeling-made-functional/) -
expressing business rules as types that the compiler enforces.

Python is currently [the most popular programming language in the world](https://www.tiobe.com/tiobe-index/). And no
matter what you think of Python, it will always be the second best language for everything. That ubiquity is exactly why
Fable.Python exists.

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

## When Not to Use Fable.Python

- When your F# code depends on .NET libraries without Fable support
- Performance-critical code (Python has runtime overhead)
- Team won't learn F#

**Best fit:** You love F#, but need Python's ecosystem.

## A First Example

Let's begin with a simple F# example:
*)

let greet (name: string) = $"Hello, {name}!"

let message = greet "Fable.Python"

(**
When compiled with Fable, this generates the following Python:
*)

(*** include-python: greet, message ***)

(**
Notice how the explicit type annotation `(name: string)` generates clean Python with `name: str`.
Without it, F# infers from usage and Fable generates `name: Any | None = None` to handle cases
where the function might be called with no argument. Type annotations give you cleaner output.

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
This compiles to Python while preserving the semantic meaning. The `Shape` type becomes a tagged class structure, and
the `match` expression becomes clean conditional logic. The compiler ensures you handle all cases, i.e if you add a new
shape variant, the compiler will warn you about unhandled cases in the `area` function.

## What's Next?

In the following chapters, we will get started by setting up your environment, working with Python libraries, and understanding
F# compatibility with Fable. Let's begin.
*)
