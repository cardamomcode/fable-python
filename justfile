# Fable.Python F# Advent 2025
# Run `just` to see available commands

# Chapter order for documentation generation
# Edit this list to reorder or add chapters
chapters := "Introduction Python GettingStarted Interop Bindings Compatibility AsyncProgramming Testing FableV5 Pydantic UnitsOfMeasure FableLiterate Summary"

# Default: show help
default:
    @just --list

# Install .NET tools (Fable, Fantomas) and Python dependencies
setup:
    dotnet tool restore
    uv sync

# Restore NuGet packages and npm dependencies
restore:
    dotnet restore
    npm install

# Build all chapters to Python
build:
    dotnet fable fable-python.fsproj --lang python -o output/chapters/
    dotnet fable Fable.Literate/Fable.Literate.fsproj --lang python -o output/Fable.Literate/

# Build the converter only
build-converter:
    dotnet fable Fable.Literate/Fable.Literate.fsproj --lang python -o output/Fable.Literate/

# Watch mode for development
watch:
    dotnet fable watch fable-python.fsproj --lang python -o output/chapters/

# Generate markdown from all chapters (in order)
generate: build format-python
    #!/usr/bin/env bash
    mkdir -p docs
    for name in {{chapters}}; do
        # Convert PascalCase to snake_case for Python file naming
        pyname=$(echo "$name" | sed 's/\([A-Z]\)/_\1/g' | sed 's/^_//' | tr '[:upper:]' '[:lower:]')
        # FableLiterate uses different python output path (symlink to Fable.Literate/App.fs)
        if [ "$name" = "FableLiterate" ]; then
            pyfile="output/Fable.Literate/python.py"
        else
            pyfile="output/chapters/chapters/${pyname}.py"
        fi
        uv run python output/Fable.Literate/app.py \
            --python-file "$pyfile" \
            "chapters/${name}.fs" > "docs/${name}.md"
        echo "Generated docs/${name}.md"
    done
    # Fix markdown lint issues
    just lint-markdown

# Generate concatenated blogpost (first chapter as-is, rest with increased headers)
blogpost: build format-python
    #!/usr/bin/env bash
    mkdir -p docs
    first=true
    for name in {{chapters}}; do
        # Convert PascalCase to snake_case for Python file naming
        pyname=$(echo "$name" | sed 's/\([A-Z]\)/_\1/g' | sed 's/^_//' | tr '[:upper:]' '[:lower:]')
        # FableLiterate uses different python output path (symlink to Fable.Literate/App.fs)
        if [ "$name" = "FableLiterate" ]; then
            pyfile="output/Fable.Literate/python.py"
        else
            pyfile="output/chapters/chapters/${pyname}.py"
        fi
        if $first; then
            # First chapter keeps original header levels (has the title)
            uv run python output/Fable.Literate/app.py \
                --python-file "$pyfile" \
                "chapters/${name}.fs" > docs/blogpost.md
            first=false
        else
            # Remaining chapters get headers increased by one level
            echo "" >> docs/blogpost.md
            uv run python output/Fable.Literate/app.py \
                --python-file "$pyfile" \
                --increase-headers "chapters/${name}.fs" >> docs/blogpost.md
        fi
    done
    echo "Generated docs/blogpost.md"
    # Fix markdown lint issues
    just lint-markdown

# Generate a single chapter (e.g., just generate-chapter Introduction)
generate-chapter chapter: build format-python
    #!/usr/bin/env bash
    # Convert PascalCase to snake_case for Python file naming
    pyname=$(echo "{{chapter}}" | sed 's/\([A-Z]\)/_\1/g' | sed 's/^_//' | tr '[:upper:]' '[:lower:]')
    uv run python output/Fable.Literate/app.py \
        --python-file "output/chapters/chapters/${pyname}.py" \
        "chapters/{{chapter}}.fs"

# Clean generated files
clean:
    rm -rf output/
    rm -rf docs/
    rm -rf fable_modules/

# Run a specific Python output
run file:
    uv run python output/chapters/{{file}}.py

# Format F# files with fantomas
format-fsharp:
    dotnet fantomas chapters/ Fable.Literate/

# Format Python files with ruff (ignore gitignore for generated files)
format-python:
    uv run ruff format --no-respect-gitignore output/

# Format all source files (F# and Python)
format: format-fsharp format-python

# Lint Python files with ruff (ignore gitignore for generated files)
lint-python:
    uv run ruff check --no-respect-gitignore output/

# Lint and fix markdown files
lint-markdown:
    npx markdownlint --fix docs/*.md

# Lint all generated files
# TODO: Re-enable lint-python once Fable code generation issues are fixed
lint: lint-markdown

# Run tests (.NET)
test:
    dotnet run --project Fable.Literate.Tests/Fable.Literate.Tests.fsproj

# Build tests to Python
build-tests:
    dotnet fable Fable.Literate.Tests/ --lang python --outDir output/Fable.Literate.Tests/

# Run tests (Python)
test-python: build-tests
    uv run python output/Fable.Literate.Tests/program.py

# Run all tests (.NET and Python)
test-all: test test-python

# Full build: restore, build, generate docs, format, lint
all: restore build generate format lint
    @echo "Build complete!"

# 🥰
amazing:
    @echo "Just amazing! 🥰"
