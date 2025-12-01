# Fable v5: What's New

Fable v5 brings significant improvements to the Python target, with a focus on
correctness, modern Python support, and better interoperability.

## .NET 10 and F# 9.0 Support

Fable v5 uses native MSBuild for parsing projects instead of Buildalyzer.
This avoids creating fake .csproj files which could confuse IDEs.

Key improvements include:

- **Nullable Reference Types** - F# 9's compile-time null-safety
- **Many BCL additions** - Expanded .NET Base Class Library support
- **63 bug fixes** - Improved stability across all targets
- **300+ new tests** - Ensuring reliability

## Python Target Highlights

The Python target has received special attention in v5:

- **Python 3.12-3.14 support** (3.10/3.11 are deprecated)
- **fable-library via PyPI** - No more bundled runtime files
- **Modern type parameter syntax** - Better type hinting in generated code
- **`Py.Decorate` attribute** - Add Python decorators from F#
- **`Py.ClassAttributes` attribute** - Fine-grained class generation control
- **Improved Pydantic interop** - First-class support for data validation

## Rust Core with PyO3

One of the biggest changes is that the core of fable-library is now written
in **Rust** using PyO3. This isn't primarily for performance - it's for
**correctness**:

### Why Rust?

- **Correct .NET semantics** - Sized/signed integers (int8, int16, int32, int64, uint8, etc.)
- **Proper overflow behavior** - Matches .NET exactly
- **Fixed-size arrays** - No more Python list quirks for byte streams
- **Reliable numerics** - Fable 4's pure Python numerics were a constant source of bugs

## fable-library via PyPI

Before Fable v5, the runtime was bundled in the NuGet package and copied
to your output directory. Now it's a simple pip/uv dependency:

```bash
# Install with pip
pip install fable-library

# Or with uv (recommended)
uv add fable-library
```

This makes dependency management much simpler and follows Python conventions.

## Test Coverage

Fable v5 significantly increased test coverage across all targets:

| Target         | Fable 4.9 | Fable 5 | Increase     |
| -------------- | --------- | ------- | ------------ |
| **JavaScript** | 2,589     | 2,748   | +159 (+6%)   |
| **Python**     | 1,880     | 1,974   | +94 (+5%)    |
| **Rust**       | 2,118     | 2,184   | +66 (+3%)    |

That's **319 new tests** ensuring reliability across the board.

## Getting Started with Fable v5

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
