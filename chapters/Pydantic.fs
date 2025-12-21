(**
# Pydantic Interop

## What is Pydantic?

[Pydantic](https://docs.pydantic.dev/) is Python's most popular data validation
library. It's the de facto standard for modern Python APIs - FastAPI, LangChain,
and countless other frameworks rely on it.

Pydantic gives you:

- **Runtime type validation** - Catch bad data before it causes problems
- **Automatic serialization** - JSON/dict conversion built-in
- **Schema generation** - OpenAPI/JSON Schema for free
- **IDE support** - Full autocomplete from type hints

Fable v5 introduces attributes that make F# and Pydantic work together seamlessly.

## Creating Models in F`#`

### Using ClassAttributes

The `Py.ClassAttributes` attribute controls how class members are generated,
which is essential for Pydantic compatibility:
*)

(*** hide ***)
module Pydantic

open Fable.Core
open Fable.Python.Pydantic

(**
*)

[<Py.ClassAttributes(style = Py.ClassAttributeStyle.Attributes, init = false)>]
type User() =
    inherit BaseModel()
    member val Name: string = "" with get, set
    member val Age: int = 0 with get, set
    member val Email: string option = None with get, set

(**
This generates clean Pydantic code:

```python
from pydantic import BaseModel

class User(BaseModel):
    Name: str = ""
    Age: int = 0
    Email: str | None = None
```

The `style = Attributes` tells Fable to generate class-level attributes (what
Pydantic expects) rather than instance attributes set in `__init__`.

### The Decorator Attribute

For simpler cases like dataclasses, use `Py.Decorate`:
*)

[<Py.Decorate("dataclasses.dataclass")>]
type Person = {
    Name: string
    Age: int
}

(**
This generates:
*)

(*** include-python: Person ***)

(**
You can pass parameters to decorators:
*)

[<Py.Decorate("dataclasses.dataclass", "frozen=True, slots=True")>]
type Point = {
    X: float
    Y: float
}

(**
The `frozen=True` makes instances immutable (matching F# record semantics).

## Fields and Validation

Pydantic's `Field()` function lets you add constraints and metadata to fields.
The `Fable.Python.Pydantic` module provides typed helpers:
*)

[<Py.ClassAttributes(style = Py.ClassAttributeStyle.Attributes, init = false)>]
type Product() =
    inherit BaseModel()

    member val Name: string = "" with get, set

    // Field with description
    member val Description: Field<string> = Field.Description "Product description" with get, set

    // Field with numeric constraints
    member val Price: Field<float> = Field.Ge 0.0 with get, set // price >= 0

    // Field with string constraints
    member val Sku: Field<string> = Field.Pattern "^[A-Z]{2}-[0-9]{4}$" with get, set // e.g., "AB-1234"

(**
Available field constraints:

|      Function       |      Constraint       |
| ------------------- | --------------------- |
| `Field.Gt`          | Greater than          |
| `Field.Ge`          | Greater than or equal |
| `Field.Lt`          | Less than             |
| `Field.Le`          | Less than or equal    |
| `Field.MinLength`   | Minimum string length |
| `Field.MaxLength`   | Maximum string length |
| `Field.Pattern`     | Regex pattern         |
| `Field.Default`     | Default value         |
| `Field.Description` | Field description     |

## Importing Python-Defined Models

Sometimes you need to use Pydantic models defined in Python - perhaps from an
OpenAPI generator, a Python team, or an existing codebase. Here's the pattern:

Given a Python model in `models.py`:

```python
from pydantic import BaseModel

class Customer(BaseModel):
    id: int
    name: str
    email: str | None = None
```

Create F# bindings:
*)

/// Customer model imported from models.py
[<Import("Customer", "models")>]
type Customer =
    abstract id: int with get, set
    abstract name: string with get, set
    abstract email: string option with get, set

/// Helper module for creating instances
[<RequireQualifiedAccess>]
module Customer =
    [<Import("Customer", "models")>]
    [<Emit("$0(id=$1, name=$2, email=$3)")>]
    let create (id: int) (name: string) (email: string option) : Customer = nativeOnly

(**
Now you can use the Python model from F# with full type safety:
*)

let customer = Customer.create 1 "Alice" (Some "alice@example.com")

let showCustomer (c: Customer) =
    printfn "Customer %d: %s" c.id c.name

    match c.email with
    | Some email -> printfn "  Email: %s" email
    | None -> printfn "  No email on file"

(**
This pattern is useful when you want to:

- Use models generated from OpenAPI specs
- Integrate with an existing Python codebase
- Share models between Python and F# code

## Type Mappings

F# types map naturally to Python/Pydantic types:

|   F# Type   |    Python Type    |             Notes              |
| ----------- | ----------------- | ------------------------------ |
| `string`    | `str`             |                                |
| `int`       | `int`             |                                |
| `float`     | `float`           |                                |
| `bool`      | `bool`            |                                |
| `'T option` | `Optional[T]` (*) | Modern union syntax            |
| `'T list`   | `list[T]`         |                                |
| `'T array`  | `list[T]`         |                                |
| Record      | `class`           | With `@dataclass` or BaseModel |
| DU          | Tagged class      | See below                      |

(*) Generated as `T | None` in Python 3.12+

### F# Option to Python Union

Notice how `string option` becomes `str | None` in Python. Fable v5 uses
modern Python union syntax for optional types, making the generated code
feel native to Python developers.

## Serialization

Pydantic models have built-in serialization methods:
*)

let serializationExample () =
    let user = User()
    user.Name <- "Alice"
    user.Age <- 30
    user.Email <- Some "alice@example.com"

    // Convert to dictionary
    let dict = user.model_dump ()

    // Convert to JSON string
    let json = user.model_dump_json ()

    // Pretty-printed JSON
    let prettyJson = user.model_dump_json_indented 2

    printfn "JSON: %s" json

(**
The `model_dump()` and `model_dump_json()` methods are available on any
class that inherits from `BaseModel`.

## The DTO Boundary Pattern

A Pydantic model is not your domain - it's a **Data Transfer Object (DTO)**.
This distinction is important for well-architected applications:

```text
┌─────────────────┐         ┌─────────────────┐         ┌─────────────────┐
│   F# Domain     │   →→→   │   Pydantic DTO  │   →→→   │   JSON / API    │
│                 │   map   │                 │   dump  │                 │
│  UserId (Guid)  │         │  Id: str        │         │  "id": "a1b2.." │
│  Age: int32     │         │  Age: int       │         │  "age": 42      │
│  Balance: Money │         │  Amount: float  │         │  "amount": 3.14 │
└─────────────────┘         └─────────────────┘         └─────────────────┘
```

### Different Concerns, Different Types

|  Concern   |        Domain Types        |        Transfer Types        |
| ---------- | -------------------------- | ---------------------------- |
| Purpose    | Model business logic       | Cross-boundary communication |
| Semantics  | Rich (overflow, precision) | Simple (JSON-compatible)     |
| Validation | Business rules             | Schema conformance           |
| Stability  | Can evolve internally      | API contract                 |

### Domain Types vs DTO Types
*)

/// Domain model - uses precise F# types
type UserId = UserId of System.Guid

type Money = {
    Amount: decimal
    Currency: string
}

type DomainUser = {
    Id: UserId
    Name: string
    Age: int32 // Bounded, wrapping arithmetic
    Balance: Money
}

/// DTO - uses Python-native types for serialization
[<Py.ClassAttributes(style = Py.ClassAttributeStyle.Attributes, init = false)>]
type UserDTO() =
    inherit BaseModel()
    member val Id: string = "" with get, set
    member val Name: string = "" with get, set
    member val Age: int = 0 with get, set
    member val BalanceAmount: float = 0.0 with get, set
    member val BalanceCurrency: string = "" with get, set

(**
### The Mapping Layer

Explicit transformation between domain and DTO:
*)

module UserMapping =
    let toDTO (user: DomainUser) : UserDTO =
        let dto = UserDTO()

        dto.Id <-
            match user.Id with
            | UserId guid -> string guid

        dto.Name <- user.Name
        dto.Age <- int user.Age
        dto.BalanceAmount <- float user.Balance.Amount
        dto.BalanceCurrency <- user.Balance.Currency
        dto

    let fromDTO (dto: UserDTO) : Result<DomainUser, string> =
        try
            Ok {
                Id = UserId(System.Guid.Parse dto.Id)
                Name = dto.Name
                Age = int32 dto.Age
                Balance = {
                    Amount = decimal dto.BalanceAmount
                    Currency = dto.BalanceCurrency
                }
            }
        with ex ->
            Error ex.Message

(**
### Why This Pattern?

The "boilerplate" of separate DTO types is actually valuable:

1. **Serialization just works** - DTOs use Python-native types
2. **Domain integrity preserved** - Your `int32` still has proper wrapping behavior
3. **Clear boundaries** - The mapping layer handles validation and transformation
4. **API evolution** - DTOs can change independently of domain types

The visual difference between F# records and Pydantic classes is a **feature** -
it's a speed bump that makes you think about the boundary you're crossing.

## Why This Matters

This interop enables powerful patterns:

1. **Define models in F#** with full type safety and pattern matching
2. **Generate Python classes** that integrate with the Python ecosystem
3. **Use Pydantic validation** in FastAPI, LangChain, and other frameworks
4. **Publish to PyPI** - Your F# types become Python packages

You get the best of both worlds: F#'s type safety during development,
and Python's rich ecosystem at runtime.

In the next chapter, we'll see how to use these Pydantic models with FastAPI
to build type-safe web APIs.
*)
