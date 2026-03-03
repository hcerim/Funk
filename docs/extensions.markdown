---
layout: page
title: Extensions
permalink: /extensions/
nav_order: 4
---

# Extensions -> Composition surface

Funk provides a rich set of extension methods that serve as the **primary composition surface** of the library. These extensions augment core C# types with functional operations, enabling expressive, statement-free code. They integrate naturally with the Funk types (`Maybe`, `Exc`, `Record`, etc.) and with each other, allowing you to build fluent pipelines where each step feeds into the next.

Instead of writing imperative code full of intermediate variables and statements, you can compose operations declaratively. The extensions are designed to work together — you can pipe an object through a series of transformations, safely cast it, match on its value, check for nulls, compose functions, curry them, and wrap disposables — all without leaving the expression world.

## Object extensions

### Piping (Do, DoAsync)

`Do` acts as a **pipe** — it takes an object, performs an operation, and either returns the original object or transforms it into something else. This enables fluent pipelines where you can chain side effects and transformations without breaking the flow.

When used with an `Action<T>`, `Do` performs the side effect and returns the original object. When used with a `Func<T, R>`, it transforms the value and returns the result.

```c#
var result = GetCustomer(id)
    .Do(c => logger.Log($"Found: {c.Name}"))
    .Do(c => c.Email); // pipes to result
```

Here, the first `Do` logs the customer name and returns the `Customer` object. The second `Do` transforms it by extracting the email. We expressed the whole pipeline without intermediate variables or statements.

`Async` versions are available for operations that return `Task` objects. They also support chaining directly on `Task<T>` values.

```c#
var saved = await customer
    .DoAsync(c => db.SaveAsync(c))
    .DoAsync(c => cache.InvalidateAsync(c.Id));
```

In this example, we first save the customer asynchronously and then invalidate the cache. Each `DoAsync` with a `Func<T, Task>` performs the side effect and returns the original object, keeping the pipeline intact.

### Safe casting

We often need to cast objects from one type to another and the traditional approach using the `as` keyword or direct casting can lead to `InvalidCastException` or require null checks. `SafeCast` safely attempts a cast and returns the result as a `Maybe` object. No exceptions, no null checks.

```c#
object value = "hello";
var maybeString = value.SafeCast<string>(); // Maybe<string> -> "hello"
var maybeInt = value.SafeCast<int>(); // Maybe<int> -> empty
```

If the cast succeeds, you get a non-empty `Maybe` containing the value. If it fails, you get an empty `Maybe`. This integrates seamlessly with all the `Maybe` operations like `Map`, `FlatMap`, `Match`, etc. (see [Maybe](/Funk/types/maybe/)).

### Pattern matching on objects

Funk provides `Match` overloads for value-based, predicate-based, and collection-based pattern matching on any object. These come with arities from 1 to 10, covering most practical scenarios.

```c#
var message = statusCode.Match(
    200, _ => "OK",
    404, _ => "Not Found",
    500, _ => "Server Error",
    otherwise: _ => "Unknown"
); // string
```

The `Match` function checks the object against each case using null-safe equality and returns the result of the first matching selector. If no case matches and an `otherwise` fallback is provided, it is used. You can also provide an `otherwiseThrow` function to throw a specific exception when no match is found.

For predicate-based matching, you can provide functions instead of values.

```c#
var category = temperature.Match(
    t => t < 0, _ => "Freezing",
    t => t < 20, _ => "Cold",
    t => t < 30, _ => "Warm",
    otherwise: _ => "Hot"
); // string
```

There are also `params` variants that accept dynamic arrays of value-selector or predicate-selector pairs, returning the result wrapped in `Maybe`. These complement the `Pattern` types (see [Pattern](/Funk/types/pattern/)) for cases where inline matching is more convenient.

```c#
var result = statusCode.Match(
    (200, _ => "OK"),
    (404, _ => "Not Found")
); // Maybe<string>
```

All `Match` variants also work with `Action` type delegates when you don't need to return a value.

## Null checking

Simple, expressive null checks available on any object.

```c#
var isNull = customer.IsNull(); // bool
var isNotNull = customer.IsNotNull(); // bool
```

There is also an `Initialize` function that returns the item if not null, otherwise creates and returns a new instance (for types with a parameterless constructor).

```c#
var list = possiblyNullList.Initialize(); // returns existing or new instance
```

## Equality

All equality methods are **null-safe**. You never have to worry about `NullReferenceException` when comparing objects. They handle null sequences gracefully as well.

```c#
var equal = customer.SafeEquals(other); // null-safe equality
var different = customer.SafeNotEquals(other);
```

For checking whether an item equals any or all items in a collection, Funk provides `SafeEqualsToAny` and `SafeEqualsToAll`. These also handle null sequences.

```c#
var inList = item.SafeEqualsToAny(items); // null-safe contains check
var allSame = item.SafeEqualsToAll(items);
```

There are also collection-side counterparts `SafeAnyEquals` and `SafeAllEquals` that operate on the sequence directly. Nullable value type overloads are provided as well.

## Function composition

Function composition lets you combine two functions into a single function. Funk provides `ComposeLeft` for left-to-right composition (f then g) and `ComposeRight` for right-to-left composition (g then f).

```c#
Func<string, int> parse = int.Parse;
Func<int, string> format = n => $"Number: {n}";

var composed = parse.ComposeLeft(format); // Func<string, string>
var result = composed("42"); // "Number: 42"
```

With `ComposeLeft`, the function on the left is applied first and its result is passed to the function on the right. This reads naturally in the order of execution.

```c#
var composed = format.ComposeRight(parse); // Func<string, string>
var result = composed("42"); // "Number: 42"
```

With `ComposeRight`, the function on the right is applied first — useful when you want to reason about the composition in the mathematical sense (g ∘ f).

`Action` variants are available as well, allowing you to compose side effects with functions.

## Currying

Currying transforms a function with multiple parameters into a chain of single-parameter functions. Each function in the chain takes one argument and returns the next function in the chain until all arguments have been provided.

```c#
Func<int, int, int> add = (a, b) => a + b;
var curriedAdd = add.Curry(); // Func<int, Func<int, int>>

var addFive = curriedAdd(5); // Func<int, int>
var result = addFive(3); // 8
```

Currying is available for arities 2 through 5. It is related to [partial application](/Funk/partial-application/) but produces a fully curried form where each application always takes exactly one argument. With partial application, you apply a specific argument to a multi-parameter function. With currying, you transform the function itself into a chain of single-parameter functions.

## Action to Func conversion

In FP, everything should be an expression. `Action` delegates break this rule as they return `void` which cannot be used in expressions. `ToFunc` converts `Action` delegates to `Func` delegates returning `Unit`, making them composable with other functional operations.

```c#
Action<string> log = Console.WriteLine;
var func = log.ToFunc(); // Func<string, Unit>
```

This is available for `Action` delegates with up to 5 parameters. You can also provide a custom result function if you need a specific return value instead of `Unit`.

```c#
Action<string> log = Console.WriteLine;
var func = log.ToFunc(_ => true); // Func<string, bool>
```

## Disposable operations

`DisposeAfter` and `DisposeAfterAsync` safely execute operations with `IDisposable` objects and dispose them afterwards. They are the expression-form equivalent of the `using` statement — instead of writing a block, you write an expression that returns a value.

```c#
var content = new StreamReader(path).DisposeAfter(reader => reader.ReadToEnd()); // string
```

The disposable is guaranteed to be disposed after the operation completes, whether it succeeds or fails. `Async` versions are available for asynchronous operations.

```c#
var data = await new HttpClient().DisposeAfterAsync(async client =>
    await client.GetStringAsync(url)
); // string
```

`Action` variants are also available when you don't need to return a value from the operation.

## Task extensions

`ToTask` wraps values, actions, and functions into `Task` objects. This is useful when you need to lift a synchronous value into the asynchronous world.

```c#
var task = "hello".ToTask(); // Task<string>

Action<string> operation = Console.WriteLine;
var asyncOp = operation.ToTask(); // Task
```

`WithResult` converts a `Task` (void) to `Task<Unit>` or `Task<R>`, enabling you to continue composing in the expression world instead of being stuck with a non-returning `Task`.

```c#
var unitTask = someVoidTask.WithResult(); // Task<Unit>
var resultTask = someVoidTask.WithResult(_ => "done"); // Task<string>
```

`ToTask` also supports `CancellationToken` for cancellation scenarios and works with `Func<Task>` and `Func<Task<T>>` delegates for queuing async operations on the thread pool.

## Enumerable extensions

Funk provides an extensive collection of null-safe, immutable sequence operations. All of them handle null sequences gracefully — you never get a `NullReferenceException` when the source sequence is null. The results are always returned as `IImmutableList<T>`.

```c#
IEnumerable<string> items = null;
var safe = items.Map(); // IImmutableList<string> (empty, not null)
```

The `Map` function without a mapper creates an immutable copy of the sequence, treating null as empty. With a mapper, it transforms each element.

```c#
var first = items.AsFirstOrDefault(i => i.StartsWith("A")); // Maybe<string>
var (matching, rest) = items.ConditionalSplit(i => i.Length > 3); // Record<IImmutableList<string>, IImmutableList<string>>
```

`AsFirstOrDefault` returns the first matching element wrapped in `Maybe` — no exceptions if the sequence is empty or no element matches. `ConditionalSplit` splits a sequence into two immutable lists based on a predicate, returned as a `Record` that can be deconstructed.

Other notable operations include:

- `FlatMap` — maps and flattens sequences
- `ExceptNulls` — filters out null values from a sequence
- `Flatten` — flattens nested sequences or sequences of `Maybe` objects
- `Reduce` — aggregates a sequence, returning the result as `Maybe`
- `Fold` — aggregates a sequence of `Maybe` objects, ignoring empty ones
- `MapReduce` — maps and then reduces in a single operation
- `ForEach` — executes an operation on each item, returning `Exc<Unit, E>` for safe error handling
- `AsNotEmptyList` — returns the sequence as `Maybe<IImmutableList<T>>` (empty `Maybe` if null or empty)
- `ConditionalSplit` — splits items by predicate into a `Record` of two immutable lists
- `DistinctBy` — returns unique items specified by a selector
- `Match` — pattern-matches on the sequence based on its count (empty, single, multiple)

These operations work together with the Funk types to enable fully functional pipelines over collections — no statements, no null checks, no surprises.
