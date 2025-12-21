module GettingStarted

(**
# Getting Started with Fable.Python

Let's set up a Fable.Python project from scratch and get our first F# code running as Python.

## Prerequisites

You'll need:

- [.NET SDK](https://dotnet.microsoft.com/download) (6.0 or later. We recommend
  installing the latest LTS version, currently .NET 10)
- [Python 3.12+](https://www.python.org/downloads/) (Fable targets Python 3.12 or higher)
- [uv](https://docs.astral.sh/uv/) (recommended) - A fast Python package manager written in Rust

If you don't have `uv` installed:

```bash
# macOS/Linux
curl -LsSf https://astral.sh/uv/install.sh | sh

# Windows
powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"
```

You can also use `pip` if you prefer, but `uv` is significantly faster and handles
virtual environments automatically.

## Project Setup

Create a new directory and initialize an F# project:

```bash
mkdir my-fable-python
cd my-fable-python

# Create F# console app
dotnet new console -lang F#

# Set up local tools and install Fable 5 (alpha)
dotnet new tool-manifest
dotnet tool install fable --version 5.0.0-alpha.21

# Add Fable.Core package
dotnet add package Fable.Core --version 5.0.0-beta.4
```

## Install Python Dependencies

Fable-generated Python code requires the `fable-library` runtime:

```bash
# Using uv (recommended)
uv add "fable-library==5.0.0a21"

# Or with pip
pip install "fable-library==5.0.0a21"
```

---

**Note:** Version pinning matters. The fable-library version must match
your Fable compiler version. PyPI uses `5.0.0a21` format instead of `5.0.0-alpha.21`.

---

## Your First Program

Replace the contents of `Program.fs` with:
*)

(*** hide ***)
// This is the actual F# code that would go in Program.fs

(**

```fsharp
printfn "Hello from Fable.Python!"

let square x = x * x
let numbers = [1; 2; 3; 4; 5]
let squares = numbers |> List.map square

printfn "Squares: %A" squares
```

## Compile and Run

Transpile to Python:

```bash
dotnet fable --lang python
```

This creates `program.py` in your project directory. Run it:

```bash
# Using uv
uv run python program.py

# Or directly with python
python3 program.py
```

You should see:

```text
Hello from Fable.Python!
Squares: [1; 4; 9; 16; 25]
```

## Watch Mode

For development, use watch mode to automatically recompile on changes:

```bash
dotnet fable watch --lang python
```

Now any changes to your F# files will instantly produce updated Python output.

## Project Structure

After setup, your project looks like this:

```text
my-fable-python/
├── Program.fs          # Your F# source code
├── program.py          # Generated Python (don't edit!)
├── my-fable-python.fsproj
├── fable_modules/      # Fable runtime modules
└── .config/
    └── dotnet-tools.json
```

## Next Steps

Now that you have a working setup, let's explore how to interact with Python
libraries in the next chapter on **Bindings**.
*)
