---
layout: page
title: Prelude
permalink: /prelude/
nav_order: 5
---

# Prelude -> Shorthand functions

In functional programming, the `Prelude` is a module that contains a set of commonly used helper functions that are always available. In Haskell, the Prelude is imported by default giving the developer access to fundamental functions without any additional effort. In Funk, the `Prelude` is a static class that you import as a static reference to access its functions directly, making them feel like language-level constructs.

```c#
using static Funk.Prelude;
```

This single import gives you access to all the shorthand functions described below. Without this import, you would need to use the factory methods on each type directly (e.g., `Maybe.Create` instead of `may`). The idea is to make your code more concise and expressive, so you can focus on the domain logic rather than the ceremony.

## Unit

```c#
var unit = empty; // Unit
```

The simplest member of the Prelude. Returns a `Unit` value — a proper representation of an empty value. `Unit` is used extensively throughout Funk, especially with the `Maybe` type to represent the absence of data. See [Unit](/Funk/types/unit/) for more details.

## Maybe

```c#
var name = may("John"); // Maybe<string>

int? nullable = null;
var number = may(nullable); // Maybe<int> (not Maybe<int?> — resolves nullable automatically)
```

The `may` function creates `Maybe` objects with type inference. It's equivalent to `Maybe.Create` or the `.AsMaybe()` extension but shorter and reads more naturally. The important thing to note here is that `may` resolves the nullable type automatically. So, the number is of type `Maybe<int>` and not `Maybe<int?>`. See [Maybe](/Funk/types/maybe/) for more details.

## Record

```c#
var single = rec(customer); // Record<Customer>
var pair = rec("John", 30); // Record<string, int>
var triple = rec("John", "Doe", 30); // Record<string, string, int>

// From ValueTuples
var fromTuple = rec(("Jane", 28)); // Record<string, int>
```

The `rec` function creates `Record` objects with full type inference. It is available for arities 1-5, with overloads for both individual values and `ValueTuple` inputs. This makes it really useful to easily replace all the `ValueTuple` objects in your code. See [Record](/Funk/types/record/) for more details.

## Exc

```c#
var ok = success<string, Exception>("value"); // Exc<string, Exception>
var err = failure<string, FormatException>(new FormatException()); // Exc<string, FormatException>
var errList = failure<string, Exception>(exception(new Exception("root"), "message")); // Exc<string, Exception>
```

The `success` and `failure` functions create `Exc` objects directly. The `failure` function has two overloads — one that accepts a single exception and one that accepts an `EnumerableException` for error accumulation scenarios. See [Exc](/Funk/types/exc/) for more details.

## EnumerableException

```c#
var single = exception(new FormatException("bad"), "Parsing failed"); // EnumerableException<FormatException>
var multiple = exception(new[] { ex1, ex2 }, "Multiple errors"); // EnumerableException<Exception>
```

The `exception` function wraps exceptions into `EnumerableException` objects for use with the `Exc` type and error accumulation. It accepts either a single exception or a sequence of exceptions, with an optional message parameter. This is especially useful in combination with the `failure` function to create failed `Exc` objects with detailed error information.

## Lists

```c#
var items = list(1, 2, 3); // IImmutableList<int>
var fromMaybes = list(may(1), may<int>(null), may(3)); // IImmutableList<int> -> [1, 3] (filters empty)
var fromSequence = list(enumerable); // IImmutableList<T>
var emptyList = list<string>(); // IImmutableList<string> (empty)

var numbers = range(1, 5); // IImmutableList<int> -> [1, 2, 3, 4, 5]
var repeated = repeat("x", 3); // IImmutableList<string> -> ["x", "x", "x"]
```

The `list` function creates immutable lists. The overload that takes `Maybe` values automatically filters out empty ones — this is especially useful when collecting results from operations that may or may not produce a value. Instead of ending up with a list of `Maybe` objects that you have to filter and unwrap manually, you get a clean list of the underlying values.

`range` and `repeat` mirror `Enumerable.Range` and `Enumerable.Repeat` but return immutable lists directly. This way, you don't have to chain `.ToImmutableList()` every time you need an immutable collection.

## Tasks

```c#
var completed = result(42); // Task<int> (already completed)

var background = run(() => ExpensiveComputation()); // Task<int>
var withToken = run(() => ExpensiveComputation(), cancellationToken); // Task<int>

// Async variants
var asyncWork = run(() => FetchDataAsync()); // Task<Data>
```

The `result` function wraps a value in a completed `Task` (equivalent to `Task.FromResult`). The `run` function queues work on the thread pool (equivalent to `Task.Run`). Both have multiple overloads — `run` supports `Action` and `Func` delegates as well as their async counterparts, with optional `CancellationToken` support. These read more naturally in functional pipelines where you want to stay expressive.

## Type inference helpers

```c#
var f = func((int x) => x * 2); // Func<int, int>
var a = act((string s) => Console.WriteLine(s)); // Action<string>
var e = exp((int x) => x > 0); // Expression<Func<int, bool>>
```

The `func`, `act`, and `exp` functions help C# infer delegate and expression types without requiring explicit type annotations. This is particularly useful when passing lambdas to generic methods or when the compiler can't infer the type from context.

```c#
// Without inference helpers - compiler can't infer types
Method((Func<int, int>)(x => x * 2));

// With inference helpers - clean and readable
Method(func((int x) => x * 2));
```

Available for arities 0-5 for all three categories. `func` infers `Func` delegates, `act` infers `Action` delegates, and `exp` infers `Expression` types for both `Func` and `Action` expressions.

## Summary

The `Prelude` makes Funk feel like a natural extension of C#. By importing it as a static reference, you get concise, expressive syntax that reduces boilerplate and keeps your code focused on the domain logic. Instead of writing `Maybe.Create`, `Record.Create`, `Task.FromResult`, or casting lambdas to delegate types, you write `may`, `rec`, `result`, and `func` — short, clear, and to the point.
