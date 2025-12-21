module AsyncProgramming

(**
# Async Programming

Asynchronous programming is essential for modern applications - from web APIs to data
processing pipelines. F# offers two models for async code: `async` workflows and `task`
expressions. Understanding when to use each is key to effective Fable.Python development.

## Comparing Python and F`#` Async Models

Python's async model is built on `asyncio`. Python coroutines are **cold** - calling an
`async def` function returns a coroutine object that doesn't execute until awaited:

```python
import asyncio

async def fetch_data():
    print("Starting")  # Not printed when function is called!
    await asyncio.sleep(1)
    return "data"

coro = fetch_data()    # Returns coroutine, nothing executes yet
result = await coro    # NOW "Starting" prints and code runs

# Or more commonly:
asyncio.run(fetch_data())
```

F# provides two computation expressions that compile to Python's async model:

- **`async { }`** - F#'s original async workflows (cold, composable, multi-target)
- **`task { }`** - .NET-style tasks (hot in .NET, compiles to native `async def` in Python)

## F`#` Async Workflows

The `async` computation expression has been part of F# since the beginning. It creates
*cold* async operations - they don't start until explicitly run.
*)

open System

let fetchDataAsync () =
    async {
        do! Async.Sleep 1000
        return "data from async"
    }

(**
Key characteristics of `async`:

- **Cold execution** - Nothing happens until you start it
- **Composable** - Combine with `Async.Parallel`, `Async.Sequential`, etc.
- **Multi-target** - The same code works on .NET, JavaScript, AND Python
- **Cancellation** - Built-in support via `CancellationToken`

### Running Async Workflows

There are several ways to execute an async workflow:
*)

let runAsyncExample () =
    // Run synchronously (blocking) - simplest approach
    let result = fetchDataAsync () |> Async.RunSynchronously

    // Start immediately (non-blocking) - Ignore discards the result
    fetchDataAsync () |> Async.Ignore |> Async.StartImmediate

    result

(**
### Combining Async Operations

F# async shines when composing multiple operations:
*)

let fetchMultipleAsync () =
    async {
        let! results =
            [ fetchDataAsync ()
              fetchDataAsync ()
              fetchDataAsync () ]
            |> Async.Parallel

        return results |> Array.toList
    }

(**
The `Async.Parallel` function runs all operations concurrently and waits for all to
complete. This is much cleaner than manually managing multiple coroutines in Python.

### Error Handling in Async

Use `try...with` inside async blocks or `Async.Catch` for explicit error handling:
*)

let safeAsync () =
    async {
        try
            do! Async.Sleep 100
            failwith "Something went wrong"
            return "success"
        with ex ->
            return $"Error: {ex.Message}"
    }

let catchExample () =
    async {
        let! result = safeAsync () |> Async.Catch

        match result with
        | Choice1Of2 value -> printfn $"Got: {value}"
        | Choice2Of2 ex -> printfn $"Failed: {ex.Message}"
    }

(**
## F`#` Tasks

The `task` computation expression in .NET creates *hot* tasks that start immediately.
However, when compiled to Python via Fable, tasks become Python coroutines - which are
*cold* just like Python's native `async def` functions.

A key improvement in Fable v5 is that `task { }` now compiles to Python's native
`async def` syntax. Previously, Fable generated regular functions returning `Awaitable[T]`,
which frameworks like FastAPI couldn't recognize as async endpoints.
*)

open System.Threading.Tasks

let processItemTask (item: string) =
    task {
        do! Task.Delay 100
        return item.ToUpper()
    }

(**
This generates:
*)
(*** include-python: processItemTask ***)

(**
Now frameworks like FastAPI can detect and handle these as proper async endpoints.

### Task vs Async: Key Differences

|      Aspect      |       `async { }`       |         `task { }`         |
| ---------------- | ----------------------- | -------------------------- |
| .NET execution   | Cold (lazy)             | Hot (immediate)            |
| Python execution | Cold                    | Cold (coroutines are cold) |
| Python output    | Wrapped awaitable       | Native `async def`         |
| Framework compat | Manual bridging         | Direct (FastAPI, etc.)     |
| Multi-target     | .NET, JS, Python        | .NET, Python               |
| Composition      | Rich (`Async.Parallel`) | Basic                      |

> **Why the difference?** In .NET, an `async` method is still a regular method - when you
> call it, the method body starts executing immediately until it hits an `await`. The
> returned `Task` represents work already in progress.
>
> In Python, `async def` creates a *coroutine function*. Calling it doesn't run the body -
> it returns a coroutine object (a generator-like structure). This coroutine is just a
> "recipe" that must be driven by an event loop via `await` or `asyncio.run()`.
>
> When Fable compiles F# `task` to Python `async def`, the cold Python semantics apply.
> The advantage of `task` for Python is the native `async def` signature that frameworks
> recognize.

### Working with Tasks
*)

let fetchDataTask () =
    task {
        do! Task.Delay 100 // Do some async work
        return "data from task"
    }

let taskExample () =
    task {
        let! result = fetchDataTask ()
        return $"Processed: {result}"
    }

let taskWithLoop () =
    task {
        let mutable sum = 0
        for i in 1..10 do
            sum <- sum + i
        return sum
    }

(**
## Mapping to Python

Understanding how F# async constructs map to Python helps when debugging or integrating
with Python code.

### Async Workflows → Python

F# `async` workflows compile to a wrapped async structure:
*)

let simpleAsync () =
    async {
        do! Async.Sleep 500
        return 42
    }

(**
In Python, this generates:
*)

(*** include-python: simpleAsync ***)

(**
### Tasks → Native async def

F# `task` expressions compile directly to Python's `async def`:
*)

let simpleTask () =
    task {
        do! Task.Delay 500
        return 42
    }

(**
In Python, this generates:
*)

(*** include-python: simpleTask ***)

(**
### Running Tasks from F`#`

To run a task and get its result in F#:
*)

let runTaskExample () =
    let tsk = simpleTask ()

    // Block and wait for result
    let result = tsk.GetAwaiter().GetResult()
    printfn $"Got: {result}"

(**
You can also await tasks inside other tasks:
*)

let chainedTasks () =
    task {
        let! first = simpleTask ()
        let! second = simpleTask ()
        return first + second
    }

(**
### Running in Python's Event Loop

When your compiled Python code runs, you'll need an event loop. For scripts:

```python
import asyncio

async def main():
    result = await simple_task()
    print(result)

asyncio.run(main())
```

For frameworks like FastAPI, the event loop is managed for you.

## Practical Patterns

### Async HTTP Requests

Here's a pattern for async HTTP operations (assuming you have bindings for `aiohttp`):
*)

// Simulated async HTTP - in real code you'd use aiohttp bindings
let fetchUrlAsync (url: string) =
    async {
        do! Async.Sleep 100  // Simulates network delay
        return $"Response from {url}"
    }

let fetchMultipleUrls (urls: string list) =
    async {
        let! responses =
            urls
            |> List.map fetchUrlAsync
            |> Async.Parallel

        return responses |> Array.toList
    }

(**
### Sequential vs Parallel

Choose based on whether operations are independent:
*)

let sequentialProcessing items =
    async {
        let results = ResizeArray()
        for item in items do
            let! result = fetchUrlAsync item
            results.Add(result)
        return results |> Seq.toList
    }

let parallelProcessing items =
    async {
        let! results =
            items
            |> List.map fetchUrlAsync
            |> Async.Parallel
        return results |> Array.toList
    }

(**
### Cancellation

F# async supports cancellation via `CancellationToken`:
*)

open System.Threading

let cancellableWork (token: CancellationToken) =
    async {
        for i in 1..100 do
            token.ThrowIfCancellationRequested()
            do! Async.Sleep 50
            printfn $"Step {i}"
        return "Completed"
    }

let runWithTimeout () =
    async {
        use cts = new CancellationTokenSource(2000)  // 2 second timeout
        try
            let! result = cancellableWork cts.Token
            return Some result
        with
        | :? OperationCanceledException ->
            return None
    }

(**
### Advanced: StartWithContinuations

For fine-grained control over success, error, and cancellation outcomes, use
`Async.StartWithContinuations`. This is useful when you need different handling paths
for each case:
*)

let runWithContinuations () =
    Async.StartWithContinuations(
        fetchDataAsync (),
        (fun result -> printfn $"Success: {result}"),
        (fun ex -> printfn $"Error: {ex.Message}"),
        (fun _cancelled -> printfn "Cancelled")
    )

(**
## When to Use What

### Use `task { }` for Python Interop

When working with Python frameworks that expect native async functions:
*)

// FastAPI endpoint (see FastAPI chapter)
let getItemTask (itemId: int) =
    task {
        do! Task.Delay 10
        return {| id = itemId; name = "Widget" |}
    }

(**
### Use `async { }` for Multi-Target Code

When you want the same async code to work on Python, .NET, AND JavaScript:
*)

// This code compiles to all Fable targets
let sharedBusinessLogic (input: string) =
    async {
        do! Async.Sleep 100
        let processed = input.ToUpper()
        return processed
    }

(**
### Use `async { }` for Composition

When you need rich composition primitives:
*)

let complexWorkflow () =
    async {
        // Run three operations in parallel
        let! results =
            [ fetchDataAsync ()
              fetchDataAsync ()
              fetchDataAsync () ]
            |> Async.Parallel

        // Then do something sequential
        do! Async.Sleep 100

        return results |> Array.toList
    }

(**
## Summary

|       Scenario       | Recommendation |
| -------------------- | -------------- |
| FastAPI endpoints    | `task { }`     |
| aiohttp/asyncio libs | `task { }`     |
| Multi-target library | `async { }`    |
| Complex composition  | `async { }`    |
| Cancellation-heavy   | `async { }`    |
| Simple one-off async | Either works   |

The key insight: **`task` for Python-native `async def` integration (FastAPI, etc.),
`async` for Fable portability and rich composition**. Both are cold in Python.

In the next chapter, we'll look at Fable v5 features that make Python development even
smoother.
*)
