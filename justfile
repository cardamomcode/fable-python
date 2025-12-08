# Fable.Python F# Advent 2025
# Run `just` to see available commands

# Chapter order for documentation generation
# Edit this list to reorder or add chapters
chapters := "introduction python getting-started interop bindings compatibility fable-v5 pydantic units-of-measure"

# Default: show help
default:
    @just --list

# Install .NET tools (Fable, Fantomas) and Python dependencies
setup:
    dotnet tool restore
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

# Generate markdown from all chapters (in order)
generate: build format-python
    #!/usr/bin/env bash
    mkdir -p docs
    for name in {{chapters}}; do
        # Convert underscores in chapter name to match Python file naming
        pyname=$(echo "$name" | tr '-' '_')
        uv run python output/tools/fabletext.py \
            --python-file "output/chapters/chapters/${pyname}.py" \
            "chapters/${name}.fs" > "docs/${name}.md"
        echo "Generated docs/${name}.md"
    done
    # Also generate fabletext documentation
    uv run python output/tools/fabletext.py \
        --python-file "output/tools/fabletext.py" \
        tools/fabletext.fs > docs/fabletext.md
    echo "Generated docs/fabletext.md"
    # Fix markdown lint issues
    just lint-markdown

# Generate concatenated blogpost (first chapter as-is, rest with increased headers)
blogpost: build format-python
    #!/usr/bin/env bash
    mkdir -p docs
    first=true
    for name in {{chapters}}; do
        # Convert underscores in chapter name to match Python file naming
        pyname=$(echo "$name" | tr '-' '_')
        if $first; then
            # First chapter keeps original header levels (has the title)
            uv run python output/tools/fabletext.py \
                --python-file "output/chapters/chapters/${pyname}.py" \
                "chapters/${name}.fs" > docs/blogpost.md
            first=false
        else
            # Remaining chapters get headers increased by one level
            echo "" >> docs/blogpost.md
            uv run python output/tools/fabletext.py \
                --python-file "output/chapters/chapters/${pyname}.py" \
                --increase-headers "chapters/${name}.fs" >> docs/blogpost.md
        fi
    done
    # Include fabletext documenting itself (the meta twist!)
    echo "" >> docs/blogpost.md
    uv run python output/tools/fabletext.py \
        --python-file "output/tools/fabletext.py" \
        --increase-headers tools/fabletext.fs >> docs/blogpost.md
    echo "Generated docs/blogpost.md"
    # Fix markdown lint issues
    just lint-markdown

# Generate a single chapter
generate-chapter chapter: build format-python
    #!/usr/bin/env bash
    pyname=$(echo "{{chapter}}" | tr '-' '_')
    uv run python output/tools/fabletext.py \
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
    dotnet fantomas chapters/ tools/

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
lint: lint-python lint-markdown

# Full build: restore, build, generate docs, format, lint
all: restore build generate format lint
    @echo "Build complete!"

# 🥰
amazing:
    @echo "Just amazing! 🥰"
