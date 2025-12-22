module Testing

(**
# Testing Fable.Python Projects

F# is a strongly typed language - if it compiles, it often just works. But
"compiles" doesn't mean "correct". We still need testing to verify our
assumptions and ensure code does what we expect. This is especially true for:

- **Parsers and transformers**: Like Fable.Literate itself, where logic
  correctness matters more than type safety
- **External dependencies**: Side effects from file I/O, network calls, or
  Python libraries can't be checked at compile time
- **Cross-platform transpilation**: When targeting Python (or JavaScript),
  we need confidence that generated code behaves identically to .NET

Fable itself demonstrates this commitment to correctness: the compiler has over
2000 unit tests for Python transpilation and more than 2600 tests for JavaScript.
Without this extensive test suite, maintaining Fable would be impossible - the
maintainers would be constantly battling regressions for every change or fix.

With Fable.Python, you can write tests in F# that run on both .NET and Python,
catching platform-specific issues before they reach production.

This chapter covers two testing approaches:

- **XUnit-style**: Familiar to many developers, uses pytest on Python
- **Expecto-style**: Functional approach using Fable.Pyxpecto

## XUnit-Style Testing with Fable.Python.Testing

The `Fable.Python.Testing` module provides a simple, cross-platform testing API
that works with pytest on Python. Just open the module and start writing tests:

```fsharp
open Fable.Python.Testing

[<Fact>]
let ``test addition works`` () =
    let result = 2 + 2
    result |> equal 4

[<Fact>]
let ``test list operations work`` () =
    let numbers = [1; 2; 3]
    numbers |> List.sum |> equal 6
    numbers |> List.length |> equal 3

[<Fact>]
let ``test string concatenation works`` () =
    let greeting = "Hello" + " " + "World"
    greeting |> equal "Hello World"
```

### Available Assertions

The module provides these assertion functions:

| Function                       | Description                                |
|--------------------------------|--------------------------------------------|
| `equal expected actual`        | Assert equality (F# style: expected first) |
| `notEqual expected actual`     | Assert inequality                          |
| `throwsError msg f`            | Assert function throws with exact message  |
| `throwsErrorContaining sub f`  | Assert error contains substring            |
| `throwsAnyError f`             | Assert function throws any error           |
| `doesntThrow f`                | Assert function completes without error    |

### Testing Exceptions

The exception helpers make it easy to test error cases:

```fsharp
[<Fact>]
let ``test throws on invalid input`` () =
    throwsAnyError (fun () ->
        failwith "something went wrong"
    )

[<Fact>]
let ``test error message contains text`` () =
    throwsErrorContaining "invalid" (fun () ->
        failwith "The input was invalid"
    )
```

### Running with Pytest

Fable transpiles `[<Fact>]` functions to Python functions prefixed with `test_`,
which pytest discovers automatically:

```bash
# Transpile tests to Python
dotnet fable test/ --lang python --outDir build/tests

# Run with pytest
pytest build/tests
```

Pytest output looks familiar:

```text
========================= test session starts =========================
collected 3 items

test_my_module.py::test_addition_works PASSED                    [ 33%]
test_my_module.py::test_list_operations_work PASSED              [ 66%]
test_my_module.py::test_string_concatenation_works PASSED        [100%]

========================== 3 passed in 0.02s ==========================
```

## Expecto-Style Testing with Pyxpecto

[Expecto](https://github.com/haf/expecto) is a functional testing library for
F#. [Fable.Pyxpecto](https://www.nuget.org/packages/Fable.Pyxpecto) brings the
same API to Fable, supporting JavaScript, Python, and .NET.

### Why Expecto-Style?

- **Composable**: Tests are values you can combine and transform
- **No magic**: No reflection, no attributes - just functions
- **Familiar F# idioms**: Uses lists and pipelines

### Setting Up Pyxpecto

Add the package to your test project:

```bash
dotnet add package Fable.Pyxpecto --version 2.0.0
```

Use conditional compilation to support both platforms:

```fsharp
#if FABLE_COMPILER
open Fable.Pyxpecto
#else
open Expecto
#endif
```

### Writing Expecto-Style Tests

Tests are built using `testCase` and `testList`:

```fsharp
let mathTests =
    testList "Math" [
        testCase "addition works" <| fun _ ->
            let result = 2 + 2
            Expect.equal result 4 "2 + 2 should equal 4"

        testCase "multiplication works" <| fun _ ->
            let result = 3 * 7
            Expect.equal result 21 "3 * 7 should equal 21"
    ]

let stringTests =
    testList "String" [
        testCase "concatenation works" <| fun _ ->
            let result = "Hello" + " " + "World"
            Expect.equal result "Hello World" "strings should concatenate"

        testCase "length is correct" <| fun _ ->
            Expect.equal ("test".Length) 4 "length should be 4"
    ]
```

### Composing Test Suites

Tests are just values, so you can compose them naturally:

```fsharp
let allTests =
    testList "All" [
        mathTests
        stringTests
    ]
```

### Running Pyxpecto Tests

Create an entry point that runs differently on each platform:

```fsharp
[<EntryPoint>]
let main args =
#if FABLE_COMPILER
    Pyxpecto.runTests [||] allTests
#else
    runTestsWithCLIArgs [] args allTests
#endif
```

Run on .NET:

```bash
dotnet run --project MyTests.fsproj
```

Run on Python:

```bash
dotnet fable MyTests/ --lang python --outDir build/tests
python build/tests/program.py
```

## Dual-Target Test Projects

For maximum confidence, run your tests on both platforms. Here's a complete
project setup:

### Project File (.fsproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Expecto" Version="10.2.1" />
    <PackageReference Include="Fable.Pyxpecto" Version="2.0.0" />
    <PackageReference Include="Fable.Core" Version="5.0.0-beta.4" />
    <PackageReference Include="Fable.Python" Version="5.0.0-alpha.21" />
  </ItemGroup>

  <ItemGroup>
    <Compile Include="Tests.fs" />
    <Compile Include="Program.fs" />
  </ItemGroup>
</Project>
```

### Justfile Commands

We recommend [just](https://github.com/casey/just) as a command runner - it's like
`make` but simpler and cross-platform. Here's how to set up test commands:

```just
# Run tests (.NET)
test:
    dotnet run --project Tests/Tests.fsproj

# Build tests to Python
build-tests:
    dotnet fable Tests/ --lang python --outDir output/tests

# Run tests (Python)
test-python: build-tests
    uv run python output/tests/program.py

# Run all tests (both platforms)
test-all: test test-python
```

## Testing Async Code

Both approaches support testing async code. With Pyxpecto:

```fsharp
testCase "async operations work" <| fun _ ->
    let computation = task {
        let! a = asyncio.sleep(0.01, 10)
        let! b = asyncio.sleep(0.01, 20)
        return a + b
    }

    let result = asyncio.run computation
    Expect.equal result 30 "async sum should work"
```

## Best Practices

1. **Test on both platforms**: Subtle differences between .NET and Python
   can cause bugs. Dual-target testing catches these early.

2. **Use descriptive test names**: F# allows backtick identifiers, so use
   them for readable names like `` `test addition works` ``.

3. **Keep tests focused**: Each test should verify one behavior.

4. **Prefer Expect assertions**: They provide better error messages than
   raw assertions.

5. **Organize with testList**: Group related tests for better output.

## Summary

| Approach               | Best For                                | Runner   |
|------------------------|-----------------------------------------|----------|
| Fable.Python.Testing   | Simple tests, pytest integration        | pytest   |
| Expecto/Pyxpecto       | Functional composition, better messages | Pyxpecto |

Both approaches work well with Fable.Python. For most projects,
`Fable.Python.Testing` provides the simplest path - just open the module and
start writing `[<Fact>]` tests that pytest discovers automatically.
*)
