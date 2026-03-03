---
layout: page
title: OneOf
parent: Types 
permalink: /types/oneof/
nav_order: 5
---

# OneOf -> Discriminated union

In OOP, when a function can return different types of results, developers usually resort to base class hierarchies, marker interfaces, or (worse) returning `object` and type-checking later. C# doesn't have built-in discriminated unions which makes it quite difficult to express the idea that a value can be **one of several possible types** at any given time. We end up with brittle `if-else` chains, `is` type checks, and runtime casting that the compiler can't verify for us.

`OneOf` provides a type-safe way to say "this value is one of these possible types". It is a coproduct that always includes an additional empty state (`Unit`), making it a coproduct of N+1 values. It defaults to an empty value. **It is a reference type (`class`) as opposed to `Maybe` and `Record` which are value types.** `OneOf` is available with arities from 2 to 5 (`OneOf<T1, T2>` through `OneOf<T1, T2, T3, T4, T5>`).

Discriminated unions are a fundamental concept in FP languages. In F#, they are called `discriminated unions`. In Haskell, they are called `sum types`. In Scala, `sealed traits` with `case classes` serve a similar purpose. The `OneOf` type brings this capability to C# with the safety guarantees that these languages provide out of the box.

## Lifting functions

There are a few explicit ways of creating a `OneOf` object.

```c#
// Using constructors
OneOf<string, int> result = new OneOf<string, int>("hello"); // in first state
OneOf<string, int> number = new OneOf<string, int>(42); // in second state
OneOf<string, int> empty = new OneOf<string, int>(); // in empty state

// Using implicit conversions
OneOf<string, int> fromString = "hello"; // implicit from T1
OneOf<string, int> fromInt = 42; // implicit from T2

// From Unit - creates empty
OneOf<string, int> fromUnit = Unit.Value; // empty state
```

The important thing to note here is that if you pass a `null` value to the constructor, the `OneOf` will default to the empty state. This ensures type safety — you always know whether the value is present and in which state it is. There is no ambiguity. You won't end up with a `OneOf` object that claims to be in one state but actually holds nothing. This is the same safety principle that the [Maybe](/Funk/types/maybe) type provides — we are being **honest** about the possible states our data can be in.

## Accessing underlying values

Each state is accessible through a corresponding `Maybe` property.

```c#
OneOf<string, int> result = "hello";

var first = result.First; // Maybe<string> - non-empty
var second = result.Second; // Maybe<int> - empty

var isFirst = result.IsFirst; // true
var isSecond = result.IsSecond; // false
var isEmpty = result.IsEmpty; // false
```

`First`, `Second`, `Third`, etc. return `Maybe` values representing the corresponding state. This forces the consumer to handle the possible absence — you can't accidentally access the wrong state and get a runtime surprise. If the `OneOf` is in the first state, `First` will return a non-empty `Maybe` and all other properties will return empty `Maybe` objects. This design naturally pushes you towards handling every state explicitly, which is exactly the kind of safety we want.

## Pattern-matching

The primary way to work with `OneOf` is pattern-matching through the `Match` function. Let's say we have a function that either returns a `Customer` or an error message.

```c#
public OneOf<Customer, string> FindCustomer(Guid id) =>
    customers.ContainsKey(id)
        ? new OneOf<Customer, string>(customers[id])
        : new OneOf<Customer, string>("Not found");
```

Now, we can use the `Match` function to handle the result.

```c#
var result = FindCustomer(id);

// Full match - handle all states including empty
var message = result.Match(
    _ => "No result",                    // empty (Unit)
    c => $"Found: {c.Name}",            // first (Customer)
    err => $"Error: {err}"              // second (string)
); // string
```

`Match` exhaustively covers all states. The first function handles the empty state, the second one handles the first state, and so on. This way, we express all possible outcomes in a single expression without resorting to `if-else` chains or `switch` statements.

In case we don't care about the empty state, we can use the overload that omits the empty handler.

```c#
// Match without empty - throws EmptyValueException if empty
var message = result.Match(
    c => $"Found: {c.Name}",            // first
    err => $"Error: {err}"              // second
); // string
```

However, if the value happens to be empty, `EmptyValueException` will be thrown. This is the same behavior as with the [Maybe](/Funk/types/maybe) type when you use the single-case `Match` overload.

You can also use `Action` delegates (void, no return value) instead of `Func` delegates.

```c#
result.Match(
    _ => Console.WriteLine("Empty"),
    c => Console.WriteLine($"Customer: {c.Name}"),
    err => Console.WriteLine($"Error: {err}")
);
```

## Deconstruction

Same as with the [Record](/Funk/types/record) type, you can deconstruct the `OneOf` object. This gives you `Maybe` values for each state.

```c#
var (customer, error) = FindCustomer(id); // (Maybe<Customer>, Maybe<string>)

// Use Map to transform only when present
var name = customer.Map(c => c.Name);        // Maybe<string>
var errorMessage = error.Map(e => e);        // Maybe<string>
```

Exactly one of the deconstructed values will be non-empty, or all will be empty if the `OneOf` is in the empty state. This is a convenient way to extract the underlying values when you want to work with individual states separately using the `Maybe` type functions like `Map`, `FlatMap`, and others.

## Higher arities

`OneOf` with higher arities is useful for modeling complex domain outcomes. Consider a function that can succeed, fail validation, or encounter a database error.

```c#
// OneOf with 3 types - useful for representing operation outcomes
public OneOf<Customer, ValidationError, DbException> CreateCustomer(CustomerInput input)
{
    var validation = Validate(input);
    if (!validation.IsValid)
        return new OneOf<Customer, ValidationError, DbException>(validation.Error);

    return Exc.Create<Customer, DbException>(_ => db.Insert(input)).Match(
        _ => new OneOf<Customer, ValidationError, DbException>(),
        customer => new OneOf<Customer, ValidationError, DbException>(customer),
        e => new OneOf<Customer, ValidationError, DbException>(e.Root.UnsafeGet())
    );
}
```

Now, we can handle every outcome in a single expression.

```c#
var result = CreateCustomer(input);
var response = result.Match(
    _ => StatusCode(500, "Unknown error"),   // empty
    c => Ok(c),                               // Customer
    v => BadRequest(v.Message),               // ValidationError
    e => StatusCode(500, e.Message)           // DbException
); // ActionResult
```

This is much cleaner and safer than throwing and catching exceptions or returning complex result objects. The type system ensures that every possible outcome is handled.

Funk provides the `OneOf` type up to an arity of 5 (`OneOf<T1, T2, T3, T4, T5>`). However, same as with [Record](/Funk/types/record), if you need more than 5 options, it is probably time to rethink your design.

*There are also `UnsafeGetFirst`, `UnsafeGetSecond`, etc. functions but `First.GetOr(...)` is preferred as it provides a safer and more expressive approach!*

## Equality and comparison

`OneOf` provides equality comparison based on the underlying value and its state.

```c#
OneOf<string, int> a = "hello";
OneOf<string, int> b = "hello";
OneOf<string, int> c = 42;

var equal = a == b; // true
var notEqual = a == c; // false
```

Two `OneOf` objects are equal if they are in the same state with equal underlying values. A `OneOf` in the first state will never be equal to a `OneOf` in the second state, even if the underlying values happen to be structurally similar. Two empty `OneOf` objects are always equal, following the same principle as [Unit](/Funk/types/unit) where two `Unit` objects are always equal.
