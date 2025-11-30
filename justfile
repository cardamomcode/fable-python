# Fable.Python F# Advent 2025
# Run `just` to see available commands

# Default: show help
default:
    @just --list

# Install .NET tools (Fable) and Python dependencies
setup:
    dotnet new tool-manifest --force
    dotnet tool install fable --version 5.0.0-alpha.17
    uv sync

# Restore NuGet packages
restore:
    dotnet restore

# Build all chapters to Python
build:
    dotnet fable fable-python.fsproj --lang python -o output/chapters/
    dotnet fable tools/fabletext.fsproj --lang python -o output/tools/

# Build the converter only
build-converter:
    dotnet fable tools/fabletext.fsproj --lang python -o output/tools/

# Watch mode for development
watch:
    dotnet fable watch fable-python.fsproj --lang python -o output/chapters/

# Generate markdown from all chapters
generate: build
    #!/usr/bin/env bash
    mkdir -p docs
    for f in chapters/*.fs; do
        name=$(basename "$f" .fs)
        uv run python output/tools/fabletext.py "$f" > "docs/${name}.md"
        echo "Generated docs/${name}.md"
    done

# Generate concatenated blogpost (first chapter as-is, rest with increased headers)
blogpost: build
    #!/usr/bin/env bash
    mkdir -p docs
    # First chapter keeps original header levels (has the title)
    uv run python output/tools/fabletext.py chapters/01-introduction.fs > docs/blogpost.md
    # Remaining chapters get headers increased by one level
    for f in chapters/02-*.fs chapters/03-*.fs chapters/04-*.fs chapters/05-*.fs chapters/06-*.fs; do
        echo "" >> docs/blogpost.md
        uv run python output/tools/fabletext.py --increase-headers "$f" >> docs/blogpost.md
    done
    echo "Generated docs/blogpost.md"

# Generate a single chapter
generate-chapter chapter: build-converter
    uv run python output/tools/fabletext.py chapters/{{chapter}}.fs

# Clean generated files
clean:
    rm -rf output/
    rm -rf docs/
    rm -rf fable_modules/

# Run a specific Python output
run file:
    uv run python output/chapters/{{file}}.py

# Format Python files with ruff
format:
    uv run ruff format output/

# Lint Python files with ruff
lint-python:
    uv run ruff check output/

# Lint markdown files
lint-markdown:
    npx markdownlint docs/*.md

# Lint all generated files
lint: lint-python lint-markdown

# Full build: restore, build, generate docs, format, lint
all: restore build generate format lint
    @echo "Build complete!"

# 🥰
amazing:
    @echo "Just amazing! 🥰"
