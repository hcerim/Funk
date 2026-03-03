---
layout: page
title: Pattern
parent: Types 
permalink: /types/pattern/
nav_order: 6
---

# Pattern -> Lazy pattern matching

C# provides `switch` statements and switch expressions for pattern matching. While they are useful for simple scenarios, they have limitations when it comes to more complex matching logic. They are also evaluated eagerly, meaning that the expression is evaluated at the point of declaration. Additionally, they are not first-class values — you can't store a `switch` expression in a variable, pass it to another function, or reuse it.

Funk provides four types that represent **lazy** pattern-matching expressions: `Pattern<R>`, `AsyncPattern<R>`, `TypePattern<R>`, and `AsyncTypePattern<R>`. These types use C# collection initializer syntax to provide a clean and expressive way to define pattern-matching rules. The key benefit is that expressions are **lazy** — they are defined first and evaluated later when `Match` is called. Only the first matching expression is evaluated. **They are value types (`struct`) and therefore can't be null.**

## Pattern\<R\> — Value and predicate matching

`Pattern<R>` matches against specific values using equality comparison. Each entry is a tuple of `(case, function)` where the `case` is compared with the input using `SafeEquals`. The result is wrapped in `Maybe<R>` — if no pattern matches, the result is an empty `Maybe` (**see the [Maybe](/Funk/types/maybe) type for more details**).

```c#
var pattern = new Pattern<string>
{
    (1, n => $"One: {n}"),
    (2, n => $"Two: {n}"),
    (3, n => $"Three: {n}")
};

var result = pattern.Match(2); // Maybe<string> -> "Two: 2"
var noMatch = pattern.Match(5); // Maybe<string> -> empty
```

Since the result is a `Maybe` object, we can use `GetOr` to provide a default value for unmatched cases.

```c#
var message = pattern.Match(5).GetOr(_ => "Unknown number"); // "Unknown number"
```

What makes `Pattern<R>` especially powerful is the ability to use predicate-based matching. Instead of comparing against specific values, we can provide a `Func<T, bool>` predicate that determines whether the case matches. The first predicate that returns `true` is selected.

```c#
var pattern = new Pattern<string>
{
    ((int n) => n < 0, n => $"Negative: {n}"),
    ((int n) => n == 0, _ => "Zero"),
    ((int n) => n > 0, n => $"Positive: {n}")
};

var result = pattern.Match(-5); // Maybe<string> -> "Negative: -5"
```

This is really useful when we need to express complex matching logic that would otherwise require a chain of `if-else` statements. And since the pattern is a regular value, we can store it, pass it around, and reuse it across multiple `Match` calls.

## AsyncPattern\<R\> — Asynchronous matching

`AsyncPattern<R>` works the same way as `Pattern<R>` but the expression functions return `Task<R>`. This is useful when the evaluation involves asynchronous operations. The `Match` function returns a `Task<Maybe<R>>` instead of `Maybe<R>`.

```c#
var pattern = new AsyncPattern<Customer>
{
    ("db", source => db.GetCustomerAsync(source)),
    ("cache", source => cache.GetCustomerAsync(source)),
    ("api", source => api.GetCustomerAsync(source))
};

var customer = await pattern.Match("cache"); // Task<Maybe<Customer>>
```

Just as with `Pattern<R>`, we can use `GetOrAsync` or `OrAsync` to provide a default value for unmatched cases or use predicate-based matching for more complex scenarios.

## TypePattern\<R\> — Type-based matching

`TypePattern<R>` matches based on the runtime type of the input. This is similar to C#'s `switch` expression with type patterns but with the added benefit of being a first-class lazy value that can be passed around, stored, and reused. Each entry is a `Func<T, R>` where `T` defines the type to match against.

```c#
var pattern = new TypePattern<string>
{
    (int n) => $"Integer: {n}",
    (string s) => $"String: {s}",
    (bool b) => $"Boolean: {b}"
};

var result = pattern.Match(42); // Maybe<string> -> "Integer: 42"
var stringResult = pattern.Match("hello"); // Maybe<string> -> "String: hello"
var noMatch = pattern.Match(3.14); // Maybe<string> -> empty
```

Being a first-class value makes `TypePattern<R>` quite practical. Consider the following example where we define an error handler that can be reused throughout the application.

```c#
var errorHandler = new TypePattern<string>
{
    (ArgumentException e) => $"Invalid argument: {e.ParamName}",
    (IOException e) => $"IO error: {e.Message}",
    (TimeoutException e) => "Operation timed out"
};

var result = Exc.Create<Data, Exception>(_ => operation());

var message = result.Failure
    .FlatMap(e => e.Root)
    .Map(ex => errorHandler.Match(ex).GetOr(_ => $"Unexpected: {ex.Message}"));

message.Match(
    _ => logger.Info("Operation succeeded or was empty"),
    m => logger.Error(m)
);
```

We define the `errorHandler` once and can use it with any `Exc` result across the application. The `GetOr` function acts as a default case for any exception type that is not explicitly handled.

## AsyncTypePattern\<R\> — Asynchronous type-based matching

`AsyncTypePattern<R>` combines type-based matching with asynchronous evaluation. Each entry is a `Func<T, Task<R>>` and the `Match` function returns a `Task<Maybe<R>>`.

```c#
var handler = new AsyncTypePattern<Unit>
{
    async (OrderCreated e) =>
    {
        await notifications.SendAsync(e.CustomerId, "Order created");
        return Unit.Value;
    },
    async (OrderShipped e) =>
    {
        await notifications.SendAsync(e.CustomerId, "Order shipped");
        return Unit.Value;
    }
};

await handler.Match(domainEvent); // Task<Maybe<Unit>>
```

This is especially useful for handling domain events or messages where each type of event requires different asynchronous processing.

## Key characteristics

All four pattern types share the following characteristics:

- **Lazy evaluation**: Patterns are defined declaratively and evaluated only when `Match` is called
- **First match wins**: Only the first matching case-expression is evaluated
- **Safe**: Results are wrapped in `Maybe`, so no exceptions for unmatched cases
- **Null safety**: If either the case/predicate or the expression function is null, the entry is ignored. If the input is null, the result is empty
- **Composable**: Patterns are regular values — they can be stored in variables, passed as arguments, and reused across multiple `Match` calls
- The default case can be provided using `GetOr` or `Or` functions on the returned `Maybe` (or `GetOrAsync`/`OrAsync` for async variants)
