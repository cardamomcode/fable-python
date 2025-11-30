# Pydantic Interop

[Pydantic](https://docs.pydantic.dev/) is Python's most popular data validation
library. Fable v5 introduces new attributes that make F# and Pydantic work
together seamlessly.

## The Decorate Attribute

The `Decorate` attribute lets you add Python decorators to F# types:

```fsharp

[<Py.Decorator("dataclasses.dataclass")>]
type Person = {
    Name: string
    Age: int
}

```

This generates:

```python
@dataclasses.dataclass
class Person:
    name: str
    age: int32
```

The decorator is applied directly to the generated Python class!

## ClassAttributes for Fine-Grained Control

For more control over class generation, use `ClassAttributes`:

```fsharp

[<Py.Decorator("dataclasses.dataclass", "frozen=True, slots=True")>]
type Point = {
    X: float
    Y: float
}

```

This generates:

```python
@dataclass(frozen=True, slots=True)
class Point:
    x: float
    y: float
```

The `frozen=True` makes instances immutable (matching F# record semantics),
and `slots=True` optimizes memory usage.

## Pydantic BaseModel Integration

Here's how to create Pydantic models in F#:
```fsharp
// Import Pydantic's BaseModel
[<Import("BaseModel", "pydantic")>]
type BaseModel () = class end

// Define a Pydantic-compatible model
[<Py.ClassAttributes(style=Py.ClassAttributeStyle.Attributes, init=false)>]
type PydanticUser(Name: string, Age: int, Email: string option) =
    inherit BaseModel()
    member val Name: string = Name with get, set
    member val Age: int = Age with get, set
    member val Email: string option = None with get, set
```
This generates clean Pydantic code:

```python
from pydantic import BaseModel

class PydanticUser(BaseModel):
    Age: int
    Email: str | None
    Name: str
```

You get all of Pydantic's features:

- **Automatic validation** - Type checking at runtime
- **Serialization** - JSON/dict conversion built-in
- **Schema generation** - OpenAPI/JSON Schema support
- **IDE support** - Full autocomplete and type hints

## Why This Matters

This interop enables powerful patterns:

1. **Define models in F#** with full type safety and pattern matching
2. **Generate Python classes** that integrate with the Python ecosystem
3. **Use Pydantic validation** in FastAPI, LangChain, and other frameworks
4. **Publish to PyPI** - Your F# types become Python packages

## F# Option to Python Union

Notice how `string option` becomes `str | None` in Python. Fable v5 uses
modern Python union syntax for optional types, making the generated code
feel native to Python developers.

## Example: API Model

Here's a practical example for a REST API:
```fsharp
[<Import("BaseModel", "pydantic")>]
type BaseModel () = class end

[<Import("Field", "pydantic")>]
let Field: obj = nativeOnly

[<Py.ClassAttributes(style=Py.ClassAttributeStyle.Attributes, init=false)>]
type CreateUserRequest(username: string, email: string, age: int option) =
    inherit BaseModel()
    member val username: string = username with get, set
    member val email: string = email with get, set
    member val age: int option = None with get, set

[<Py.ClassAttributes(style=Py.ClassAttributeStyle.Attributes, init=false)>]
type UserResponse(id: int, username: string, email: string, created_at: string) =
    inherit BaseModel()
    member val id: int = id with get, set
    member val username: string = username with get, set
    member val email: string = email with get, set
    member val created_at: string = created_at with get, set
```
These models can be used directly with FastAPI:

```python
from fastapi import FastAPI
from your_fsharp_module import CreateUserRequest, UserResponse

app = FastAPI()

@app.post("/users", response_model=UserResponse)
def create_user(request: CreateUserRequest) -> UserResponse:
    # Pydantic validates the request automatically
    return UserResponse(
        id=1,
        username=request.username,
        email=request.email,
        created_at="2025-01-01T00:00:00Z"
    )
```

You get the best of both worlds: F#'s type safety during development,
and Python's rich ecosystem at runtime.
