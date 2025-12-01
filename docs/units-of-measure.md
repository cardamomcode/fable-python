# Units of Measure

One of F#'s most powerful features for scientific and engineering code is
**units of measure** - compile-time dimensional analysis that prevents
unit-related bugs.

## The Problem

Unit errors are a classic source of bugs. The famous Mars Climate Orbiter
was lost because one team used metric units while another used imperial.
Python can't catch these errors:

```python
# Python - no protection
distance = 100  # meters? feet? who knows!
time = 9.58     # seconds? minutes?
speed = distance / time  # ???
```

## F# Units of Measure

F# lets you annotate numeric types with units that are checked at compile time:

```fsharp
[<Measure>]
type m // meters

[<Measure>]
type s // seconds

[<Measure>]
type kg // kilograms
```

Now we can define values with units:

```fsharp
let distance = 100.0<m>
let time = 9.58<s>
let speed = distance / time // Automatically inferred as float<m/s>
```

The compiler tracks units through all operations. Division of meters by
seconds gives meters-per-second. This is all checked at compile time!

## Preventing Errors

Try to add incompatible units and the compiler stops you:

```fsharp
let distance = 100.0<m>
let mass = 50.0<kg>

// This won't compile!
// let nonsense = distance + mass
// Error: The unit of measure 'm' does not match 'kg'
```

## Derived Units

You can define derived units based on existing ones:

```fsharp
[<Measure>]
type N = kg * m / s^2 // Newton

[<Measure>]
type J = N * m // Joule

let force = 10.0<N>
let displacement = 5.0<m>
let work = force * displacement // Inferred as float<J>
```

## Real-World Example: Physics Simulation

Here's a practical example computing kinetic energy:

```fsharp
let kineticEnergy (mass: float<kg>) (velocity: float<m / s>) : float<J> = 0.5 * mass * velocity * velocity

let carMass = 1500.0<kg>
let carSpeed = 30.0<m / s>
let energy = kineticEnergy carMass carSpeed
```

The function signature clearly documents what units are expected and returned.
The compiler ensures you can't accidentally pass velocity where mass is expected.

## Unit Conversions

Define conversion functions with explicit unit transformations:

```fsharp
[<Measure>]
type km

[<Measure>]
type h

let metersToKm (d: float<m>) : float<km> = d / 1000.0<m / km>
let secondsToHours (t: float<s>) : float<h> = t / 3600.0<s / h>

let marathonDistance = 42195.0<m>
let marathonKm = metersToKm marathonDistance // 42.195<km>
```

## Generated Python

When compiled to Python, units are erased (they're purely a compile-time
feature), but your code is guaranteed to be unit-safe:

```python
def kinetic_energy(mass: float, velocity: float) -> float:
    return 0.5 * mass * velocity * velocity

car_mass: float = 1500.0
car_speed: float = 30.0
energy: float = kinetic_energy(car_mass, car_speed)
```

The Python code is clean and efficient. All the unit checking happened
at compile time in F#, so there's no runtime overhead.

## Why This Matters for Python

Python is widely used in scientific computing, but lacks compile-time
unit checking. With Fable.Python, you can:

1. **Write unit-safe code** in F# with full dimensional analysis
2. **Catch unit errors at compile time** before they become runtime bugs
3. **Generate clean Python** that integrates with NumPy, SciPy, etc.
4. **Document intent** - function signatures show expected units

This is especially valuable for physics simulations, financial calculations,
engineering applications, and any domain where mixing up units could be costly.
