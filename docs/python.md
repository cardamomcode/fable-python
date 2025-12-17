# Are You a Python Developer?

If you're coming from Python, welcome. This chapter will help you understand
the F# code you'll see throughout this guide. F# is more approachable than
it might appear, and many concepts are familiar.

## What is F#?

F# is a functional-first language that runs on .NET. But here's the key insight
for you: **with Fable.Python, .NET is just a build tool**. You write F#, it
compiles to Python, and you run Python. Your deployment is pure Python.

Think of it like TypeScript for JavaScript - you get better tooling and type
safety during development, but the output is the language you know.

## Key Concepts You'll See

Let's map F# concepts to Python equivalents you already understand.

### Type Inference

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

### Pattern Matching

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

### Discriminated Unions (Sum Types)

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

### Records

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

### The Pipeline Operator

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

### Option Types

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

## F# vs Python: Quick Reference

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

## Why Learn F#?

As a Python developer, F# gives you:

1. **Catch bugs at compile time** - No more `TypeError` or `AttributeError` at runtime
2. **Exhaustive pattern matching** - Compiler ensures you handle all cases
3. **Immutability by default** - Fewer bugs from unexpected state changes
4. **Excellent refactoring** - Change a type, compiler shows every place to update
5. **Self-documenting code** - Types serve as documentation that can't go stale

## Don't Worry About .NET

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

## Ready to Start?

Now that you understand the basics, let's set up your first Fable.Python project
in the next chapter!
