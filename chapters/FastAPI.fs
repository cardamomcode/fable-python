(**
# FastAPI

As an F# developer you are probably familiar with web frameworks like ASP.NET Core, Giraffe, or Oxpecker. But Fable.Python
also allows you to build web APIs that run in Python environments, using the popular FastAPI framework.

## What is FastAPI?

[FastAPI](https://fastapi.tiangolo.com/) is Python's most popular modern web framework.
It's fast, easy to use, and built on top of Pydantic for automatic request validation
and OpenAPI documentation.

FastAPI gives you:

- **High performance** - One of the fastest Python frameworks available
- **Automatic validation** - Request/response validation via Pydantic, that you already know from
  the previous chapter
- **Type hints** - Leverages Python type hints for better editor support
- **OpenAPI docs** - Interactive Swagger UI and ReDoc generated automatically
- **Async support** - Native async/await for high concurrency

Fable.Python includes bindings for FastAPI, allowing you to write type-safe APIs
using F# while leveraging Python's mature web ecosystem.

## Setting Up

Add FastAPI and uvicorn to your Python environment:

```bash
uv add fastapi uvicorn
```

Then import the FastAPI module in your F# code:
*)

(*** hide ***)
module FastAPI

open System.Threading.Tasks
open Fable.Core
open Fable.Python.FastAPI
open Fable.Python.Pydantic

(**
```fsharp
open Fable.Python.FastAPI
open Fable.Python.Pydantic
```

## Creating the Application

Create a FastAPI application instance at the module level:
*)

let app = FastAPI(title = "My API", version = "1.0.0")

(**
This generates:
*)

(*** include-python: app ***)

(**
The `app` variable name is important - the route decorators reference it.

## Defining Models

Request and response models use Pydantic's `BaseModel` (covered in the previous chapter):
*)

[<Py.ClassAttributes(style = Py.ClassAttributeStyle.Attributes, init = false)>]
type Item(Id: int, Name: string, Price: float, InStock: bool) =
    inherit BaseModel()
    member val Id: int = Id with get, set
    member val Name: string = Name with get, set
    member val Price: float = Price with get, set
    member val InStock: bool = InStock with get, set

[<Py.ClassAttributes(style = Py.ClassAttributeStyle.Attributes, init = false)>]
type CreateItemRequest(Name: string, Price: float, InStock: bool) =
    inherit BaseModel()
    member val Name: string = Name with get, set
    member val Price: float = Price with get, set
    member val InStock: bool = InStock with get, set

(**
## Defining Endpoints

### The APIClass Pattern

FastAPI endpoints are defined using a class with decorated static methods:
*)

let items = ResizeArray<Item>()

[<APIClass>]
type API() =
    /// GET /items - List all items
    [<Get("/items")>]
    static member get_items() : ResizeArray<Item> =
        items

    /// GET /items/{item_id} - Get item by ID
    [<Get("/items/{item_id}")>]
    static member get_item(item_id: int) : Task<obj> = task {
        match items |> Seq.tryFind (fun i -> i.Id = item_id) with
        | Some item -> return item :> obj
        | None -> return {| error = "Item not found" |}
    }

    /// POST /items - Create a new item
    [<Post("/items")>]
    static member create_item(request: CreateItemRequest) : Task<obj> = task {
        let newId =
            if items.Count = 0 then 1
            else (items |> Seq.map (fun i -> i.Id) |> Seq.max) + 1
        let newItem = Item(newId, request.Name, request.Price, request.InStock)
        items.Add(newItem)
        return {| status = "created"; item = newItem |}
    }

(**
This generates Python with proper FastAPI decorators:
*)

(*** include-python: API ***)

(**
### Key Points

- `[<APIClass>]` marks the class for FastAPI routing. We use a class because Fable
  can only apply decorator attributes to types and methods, not standalone functions
- Route decorators: `[<Get>]`, `[<Post>]`, `[<Put>]`, `[<Delete>]`, `[<Patch>]`
- Path parameters use `{param_name}` syntax and map to function arguments
- Pydantic models in parameters are automatically validated
- Return types can be sync or async (`Task<'T>`)

### Anonymous Records for Quick Responses

F# anonymous records compile to Python dictionaries, perfect for JSON responses:
*)

[<APIClass>]
type HealthAPI() =
    [<Get("/health")>]
    static member health() =
        {| status = "healthy"; version = "1.0.0" |}

(**
## Async Endpoints

For I/O-bound operations, use `task { }` to create async endpoints:
*)

[<APIClass>]
type AsyncAPI() =
    [<Get("/slow")>]
    static member slow_operation() = task {
        // Simulate async work (e.g., database query)
        do! Task.Delay(100)
        return {| message = "Done!" |}
    }

(**
The `task { }` computation expression compiles to Python's `async def`,
integrating naturally with FastAPI's async support.

## Path and Query Parameters

### Path Parameters

Path parameters are extracted from the URL:
*)

[<APIClass>]
type UsersAPI() =
    [<Get("/users/{user_id}")>]
    static member get_user(user_id: int) =
        {| id = user_id; name = "User " + string user_id |}

    [<Get("/users/{user_id}/posts/{post_id}")>]
    static member get_user_post(user_id: int, post_id: int) =
        {| user_id = user_id; post_id = post_id |}

(**
### Query Parameters

Query parameters are function arguments not in the path:
*)

[<APIClass>]
type SearchAPI() =
    [<Get("/search")>]
    static member search(q: string, limit: int) =
        {| query = q; limit = limit |}

(**
A request to `/search?q=hello&limit=10` maps to `search("hello", 10)`.

## Request Bodies

POST/PUT/PATCH endpoints receive request bodies as Pydantic models:
*)

[<Py.ClassAttributes(style = Py.ClassAttributeStyle.Attributes, init = false)>]
type CreateUserRequest(name: string, email: string) =
    inherit BaseModel()
    member val name: string = name with get, set
    member val email: string = email with get, set

[<APIClass>]
type UserCrudAPI() =
    [<Post("/users")>]
    static member create_user(request: CreateUserRequest) =
        // FastAPI automatically validates the request body
        {| status = "created"; name = request.name; email = request.email |}

(**
FastAPI validates the incoming JSON against the Pydantic model and returns
a 422 error if validation fails.

## HTTP Exceptions

Return proper HTTP errors using `HTTPException`:
*)

[<APIClass>]
type ErrorAPI() =
    [<Get("/protected")>]
    static member protected_route() =
        // Check authentication (simplified example)
        let isAuthenticated = false
        if not isAuthenticated then
            raise (System.Exception("Not authenticated"))
        {| message = "Secret data" |}

(**
In practice, you would use FastAPI's dependency injection for authentication.
The `HTTPException` type is available for more idiomatic error handling:

```fsharp
// For proper HTTP exceptions, use a helper that emits Python's raise
[<Emit("raise HTTPException(status_code=$0, detail=$1)")>]
let raiseHttp (code: int) (msg: string) : unit = nativeOnly

// Then in your endpoint:
if not isAuthenticated then
    raiseHttp 401 "Not authenticated"
```


## Running the Application

Compile with Fable and run with uvicorn:

```bash
# Compile F# to Python
dotnet fable --lang python --outDir build

# Run the server
cd build
uvicorn app:app --reload
```

Visit:

- `http://localhost:8000` - Your API
- `http://localhost:8000/docs` - Interactive Swagger UI
- `http://localhost:8000/redoc` - ReDoc documentation

## Development Workflow

For hot-reloading during development, run Fable in watch mode:

```bash
# Terminal 1: Watch F# files
dotnet fable --lang python --outDir build --watch

# Terminal 2: Run uvicorn with reload
cd build
uvicorn app:app --reload
```

Changes to your F# code automatically recompile and uvicorn picks up the changes.

## Complete Example

Here's a minimal but complete FastAPI application:

```fsharp
module App

open System.Threading.Tasks
open Fable.Core
open Fable.Python.FastAPI
open Fable.Python.Pydantic

// Create the app
let app = FastAPI(title = "Todo API", version = "1.0.0")

// Define the model
[<Py.ClassAttributes(style = Py.ClassAttributeStyle.Attributes, init = false)>]
type Todo(id: int, title: string, completed: bool) =
    inherit BaseModel()
    member val id: int = id with get, set
    member val title: string = title with get, set
    member val completed: bool = completed with get, set

// In-memory store
let todos = ResizeArray<Todo>()

// Define endpoints
[<APIClass>]
type TodoAPI() =
    [<Get("/")>]
    static member root() =
        {| message = "Welcome to Todo API" |}

    [<Get("/todos")>]
    static member list_todos() = todos

    [<Post("/todos")>]
    static member create_todo(title: string) =
        let todo = Todo(todos.Count + 1, title, false)
        todos.Add(todo)
        todo
```

## Why F# + FastAPI?

This combination gives you:

1. **Compile-time safety** - F# catches errors before they reach Python
2. **Runtime validation** - Pydantic validates incoming requests
3. **Auto documentation** - OpenAPI specs generated from your types
4. **Familiar ecosystem** - Deploy with standard Python tools

You write type-safe F# code, but deploy and run it like any Python web service.
*)
