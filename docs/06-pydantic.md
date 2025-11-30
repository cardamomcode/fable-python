# Pydantic Interop

[Pydantic](https://docs.pydantic.dev/) is Python's most popular data validation
library. Fable v5 introduces new attributes that make F# and Pydantic work
together seamlessly.

## The Decorator Attribute

The `Py.Decorator` attribute lets you add Python decorators to F# types:

```fsharp

[<Py.Decorate("dataclasses.dataclass")>]
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

## Decorator with Parameters

You can also pass parameters to decorators:

```fsharp

[<Py.Decorate("dataclasses.dataclass", "frozen=True, slots=True")>]
type Point = {
    X: float
    Y: float
}

```

This generates:

```python
@dataclasses.dataclass(frozen=True, slots=True)
class Point:
    x: float
    y: float
```

The `frozen=True` makes instances immutable (matching F# record semantics),
and `slots=True` optimizes memory usage.

## ClassAttributes for Pydantic

The `Py.ClassAttributes` attribute controls how class members are generated,
which is essential for Pydantic compatibility:

```fsharp

[<Import("BaseModel", "pydantic")>]
type BaseModel () = class end

[<Py.ClassAttributes(Py.ClassAttributeStyle.Attributes)>]
type PydanticUser() =
    inherit BaseModel()
    member val Name: string = "" with get, set
    member val Age: int = 0 with get, set
    member val Email: string option = None with get, set

```

This generates clean Pydantic code:

```python
from pydantic import BaseModel

class PydanticUser(BaseModel):
    Name: str = ""
    Age: int = 0
    Email: str | None = None
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

## Example: FastAPI Integration

These Pydantic models can be used directly with FastAPI:

```python
from fastapi import FastAPI
from your_fsharp_module import PydanticUser

app = FastAPI()

@app.post("/users")
def create_user(user: PydanticUser) -> PydanticUser:
    # Pydantic validates the request automatically
    return user
```

You get the best of both worlds: F#'s type safety during development,
and Python's rich ecosystem at runtime.
