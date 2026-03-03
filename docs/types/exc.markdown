---
layout: page
title: Exc
parent: Types 
permalink: /types/exc/
nav_order: 4
---

# Exc -> Possible failure

In C#, error handling is done through `try-catch` blocks. There is nothing inherently wrong with exceptions as a concept, but the way they are used in mainstream OOP leads to a significant problem — **dishonest** functions. When you call a function, there is no way to know from its signature what exceptions it might throw. You are forced to look into the implementation (and all of its nested calls) to figure it out. This makes reasoning about your code extremely difficult and error-prone.

Consider a simple function `int Parse(string input)`. This signature says: "Give me a string and I will return an int". But that's a lie. It should say: "Give me a string and I will return an int, **unless** the string is not a valid number, in which case I will throw a `FormatException`". The caller has no idea about this hidden behavior.

The `Exc` type solves this problem by making failure **explicit** in the type signature. It is a coproduct of 3 values: `Unit` (empty), the underlying success value `T`, and an error in the form of `EnumerableException<E>`. **It is a value type (`struct`) and therefore can't be null and its default value is simply an empty `Exc` object.**

`Exc` is a concept present in FP languages. In F#, it is called `Result`. In Scala, you have `Either` and `Try`. In Haskell, it is called `Either`. The `Exc` type is similar to these but with the additional capability of **error accumulation** through `EnumerableException`. This means that, unlike the traditional `Either`/`Result` types that can hold only a single error, `Exc` can collect and carry multiple errors — a feature that is incredibly useful for validation scenarios.

In Funk, the `Exc` type is a construct that is a `functor`, an `applicative`, and a `monad`, just like [Maybe](/Funk/types/maybe/). If you are not familiar with these concepts, the [Maybe](/Funk/types/maybe/) page provides a thorough introduction. Here, we will focus on how they work in the context of the `Exc` type and what additional capabilities `Exc` brings to the table.

## Lifting functions

There are a few ways of creating an `Exc` object.

```c#
// Using Exc.Create - safely captures the specified exception type
var result = Exc.Create<int, FormatException>(_ => int.Parse(input)); // Exc<int, FormatException>

// Using Prelude functions
var success = success<string, Exception>("value"); // Exc<string, Exception>
var failed = failure<string, FormatException>(new FormatException("Bad format")); // Exc<string, FormatException>
```

The important thing to note here is that `Exc.Create` captures **only** the specified exception type `E`. Unhandled exceptions will propagate — this is by design to encourage **explicit** error handling. You should always specify the exact exception type that you expect the operation to throw. There is also an overload without the exception type (`Exc.Create<T>`) that catches all exceptions, but using explicit handling is preferred.

There is also an `async` version for asynchronous operations.

```c#
var asyncResult = await Exc.CreateAsync<Customer, DbException>(_ => db.GetCustomerAsync(id)); // Exc<Customer, DbException>
```

There is an implicit conversion between an object and an `Exc` of that object so the following is legal.

```c#
public Exc<Customer, DbException> GetCustomer(Guid id) => db.Find(id);
```

Here, the `Find` function returns a `Customer` object which is implicitly converted to the `Exc<Customer, DbException>` object. With the `Exc` type, we ensure that our function is **honest**. The caller knows exactly what can go wrong — a `DbException` — and they are forced to handle it. We do not lie to the caller by hiding potential failures behind a clean-looking return type.

## Pattern-matching

Let's start with a `Match` function.

`Match` is a pattern-matching function that provides a nice way of handling all three possible states of the `Exc` type. So let's say we have parsed some input and we want to handle the result.

```c#
var result = Exc.Create<int, FormatException>(_ => int.Parse(input));

// Full match - handle all 3 states
var message = result.Match(
    _ => "No value",                    // _ is Unit (empty)
    v => $"Parsed: {v}",               // v is int (success)
    e => $"Error: {e.Root}"            // e is EnumerableException<FormatException> (failure)
); // string
```

`Match` has 3 cases it covers. The first one represents an empty case when the `Exc` object is empty. Empty value is represented by `Unit`. The second case is executed if the `Exc` object is in a success state. The third case is executed if the `Exc` object is in a failure state, where the error is an `EnumerableException<E>`.

In case we didn't care about the empty case, we could use the two-argument overload.

```c#
var value = result.Match(
    v => v * 2,                         // success
    e => -1                             // failure
); // int
```

However, if the object is empty, `EmptyValueException` will be thrown.

You can also use `Action` delegates (void, no return value) instead of `Func` delegates.

```c#
result.Match(
    _ => Console.WriteLine("Empty"),
    v => Console.WriteLine($"Value: {v}"),
    e => Console.WriteLine($"Error: {e}")
);
```

For safe access to the underlying values, `Success` and `Failure` properties return `Maybe<T>` and `Maybe<EnumerableException<E>>` respectively. This way, you can work with the values without risking exceptions.

```c#
var successValue = result.Success; // Maybe<int>
var failureValue = result.Failure; // Maybe<EnumerableException<FormatException>>
```

Properties `IsSuccess`, `IsFailure`, and `IsEmpty` tell you which state the `Exc` object is in. Additionally, `RootFailure` returns `Maybe<E>` — the root exception cause inside the `EnumerableException`, and `NestedFailures` returns `Maybe<IImmutableList<E>>` — the list of nested exceptions.

`Exc` also supports deconstruction.

```c#
var (success, failure) = result; // (Maybe<int>, Maybe<EnumerableException<FormatException>>)
```

## Functor

`Map` is useful when your repositories or services return **plain values** and you want to chain operations while keeping exception safety. `Map` wraps the provided function, so if it throws the specified exception type `E`, the result will be a failed `Exc`.

```c#
// db.GetCustomer returns Customer (plain value, not Exc)
// db.GetEmail returns string (plain value)
var email = Exc.Create<Customer, DbException>(_ => db.GetCustomer(id))
    .Map(c => c.Email); // Exc<string, DbException>
```

If the `Exc` is in a failure or empty state, `Map` will not execute the provided function and will propagate the failure or empty state. This means you can chain operations safely without worrying about exceptions leaking.

```c#
// Chain multiple transformations — all exception-safe
var summary = Exc.Create<Customer, DbException>(_ => db.GetCustomer(id))
    .Map(c => db.GetOrders(c.Id))           // Exc<List<Order>, DbException>
    .Map(orders => orders.Sum(o => o.Total)) // Exc<decimal, DbException>
    .Map(total => $"Total: {total:C}");      // Exc<string, DbException>
```

For a detailed explanation of the `functor` concept, refer to the [Maybe](/Funk/types/maybe/) page.

`Async` version of `Map` is available as well.

```c#
// db.GetCustomerAsync returns Task<Customer> (plain value)
var email = await Exc.CreateAsync<Customer, DbException>(_ => db.GetCustomerAsync(id))
    .MapAsync(c => db.GetEmailAsync(c.Id)); // Task<Exc<string, DbException>>
```

## Monad

`FlatMap` is useful when your repositories or services return **elevated values** (`Exc<T, E>`). Imagine that, instead of returning a plain value, your repository already returns an `Exc` object. If we used `Map`, we would end up with `Exc<Exc<Account, DbException>, DbException>` — a nested `Exc` that becomes tricky to unwrap. `FlatMap` avoids this nesting by flattening the result.

```c#
// Both repositories return Exc<T, DbException> — they handle their own exceptions
Exc<Customer, DbException> GetCustomer(Guid id) => Exc.Create<Customer, DbException>(_ => db.Find(id));
Exc<Account, DbException> GetAccount(Guid accountId) => Exc.Create<Account, DbException>(_ => db.FindAccount(accountId));
Exc<List<Transaction>, DbException> GetTransactions(Guid accountId) => Exc.Create<List<Transaction>, DbException>(_ => db.FindTransactions(accountId));

// Chain them with FlatMap — each step receives the previous result
var transactions = GetCustomer(id)
    .FlatMap(c => GetAccount(c.AccountId))
    .FlatMap(a => GetTransactions(a.Id)); // Exc<List<Transaction>, DbException>
```

Each `FlatMap` in the chain receives the successful result of the previous operation. If any step fails, the entire chain short-circuits and propagates the failure. No nesting, no unwrapping — just a flat pipeline.

For a detailed explanation of the `monad` concept and the difference between `Map` and `FlatMap`, refer to the [Maybe](/Funk/types/maybe/) page.

`Async` versions of `Map` and `FlatMap` are available as well (`MapAsync` and `FlatMapAsync`).

```c#
var transactions = await GetCustomerAsync(id)
    .FlatMapAsync(c => GetAccountAsync(c.AccountId))
    .FlatMapAsync(a => GetTransactionsAsync(a.Id)); // Task<Exc<List<Transaction>, DbException>>
```

`Async` versions also support transformations directly on `Task<Exc<T, E>>`.

## Error recovery

Sometimes, when an operation fails, you want to recover by providing a fallback value instead of propagating the error. `OnFailure` and `OnEmpty` provide exactly this.

```c#
var result = Exc.Create<Config, IOException>(_ => LoadConfigFromFile())
    .OnFailure(e => GetDefaultConfig()); // Exc<Config, IOException>
```

`OnFailure` provides a fallback value if the operation failed. The provided function receives the `EnumerableException<E>` so you can inspect the error before deciding on the fallback. If the `Exc` is successful, the function is not evaluated.

```c#
var withDefault = Exc.Create<Config, IOException>(_ => LoadConfigFromFile())
    .OnEmpty(_ => GetDefaultConfig()); // Exc<Config, IOException>
```

`OnEmpty` provides a fallback if the result was empty. This is useful when the absence of data should be treated differently from a failure.

There are also `Flat` variants (`OnFlatFailure`, `OnFlatEmpty`) for when the recovery operation itself returns an `Exc`. This avoids nesting, just like `FlatMap` does for `Map`. `Async` versions are available as well (`OnFailureAsync`, `OnFlatFailureAsync`, `OnEmptyAsync`, `OnFlatEmptyAsync`).

## Error mapping

Sometimes you need to transform the error type to fit a broader error hierarchy. For example, you might want to convert a `FormatException` into a `ValidationException` to match the error type expected by the rest of your pipeline.

```c#
var result = Exc.Create<int, FormatException>(_ => int.Parse(input))
    .MapFailure(e => new ValidationException(e.Root.Map(r => r.Message).GetOr(_ => "Validation failed"))); // Exc<int, ValidationException>
```

`MapFailure` maps the `EnumerableException<E1>` to a new exception type `E2`. If the `Exc` is successful, the function is not evaluated. If it is empty, it stays empty. `Async` versions are available as well (`MapFailureAsync`).

## Applicative (applicative functor)

This is where the `Exc` type truly shines compared to traditional error handling. As mentioned, the `Exc` type is also an `applicative`. If you are not familiar with the concept, the [Maybe applicative](/Funk/types/maybe/#applicative-applicative-functor) section provides a thorough introduction.

The `Exc` type provides two applicative functions: `Apply` and `Validate`. The difference between them is crucial.

`Apply` behaves like `FlatMap` — it **short-circuits** on the first failure. If the first argument fails, the second one is never evaluated. This is useful for sequential operations where later steps depend on earlier ones.

`Validate`, on the other hand, **accumulates all errors**. Even if the first argument fails, the second one is still evaluated, and all errors are collected into a single `EnumerableException`. This is incredibly useful for validation scenarios where you want to report all problems at once instead of making the user fix them one by one.

```c#
Func<string, int, Customer> createCustomer = (name, age) => new Customer(name, age);

var nameResult = Exc.Create<string, ValidationException>(_ => ValidateName(input.Name));
var ageResult = Exc.Create<int, ValidationException>(_ => ValidateAge(input.Age));

// Using Validate - accumulates ALL errors
var customer = createCustomer.AsMaybe()
    .Map(Exc.Success<Func<string, int, Customer>, ValidationException>)
    .UnsafeGet()
    .Validate(nameResult)
    .Validate(ageResult); // Exc<Customer, ValidationException>
```

If both validations fail, the `EnumerableException` will contain **both** errors. You can access them through the `Failure` property.

```c#
customer.Failure.Map(errors => errors.Nested.Match(
    _ => errors.Root.Map(r => r.Message).GetOr(_ => "Unknown error"),
    list => string.Join(", ", list.Select(e => e.Message))
)).Match(
    _ => Console.WriteLine("Validation passed"),
    summary => Console.WriteLine($"Errors: {summary}")
);
```

This is **railway-oriented programming** with error collection. The `Validate` function keeps the pipeline on the "error track" while accumulating all failures, as opposed to `Apply` which derails at the first failure. This distinction is what makes the `Exc` type more powerful than a traditional `Either`/`Result` type.

## Merging results

When you have multiple `Exc` results of the same type and you want to aggregate them, you can use `Merge` and `MergeRange`.

```c#
var result1 = Exc.Create<int, FormatException>(_ => int.Parse("1"));
var result2 = Exc.Create<int, FormatException>(_ => int.Parse("2"));

var merged = result1.Merge(result2); // Exc<IImmutableList<int>, FormatException>
```

If all results are successful, you get a success with an immutable list of all values. If any fail, all failures are collected into a single `EnumerableException`. `MergeRange` works the same way but accepts a sequence of `Exc` objects.

```c#
var results = inputs.Select(i => Exc.Create<int, FormatException>(_ => int.Parse(i)));
var merged = results.First().MergeRange(results.Skip(1)); // Exc<IImmutableList<int>, FormatException>
```

## LINQ compatibility

Same as with [Maybe](/Funk/types/maybe/#linq-compatibility), `Exc` supports `query syntax` through `Select`, `SelectMany`, and `Where` implementations. Instead of using the `Map` and `FlatMap` functions, you can use the corresponding `Select` and `SelectMany` functions. The `Where` function returns a successful `Exc` only if the item is successful and the `predicate` criteria are satisfied. Otherwise, it returns an empty `Exc`.

```c#
var account = from c in Exc.Create<Customer, DbException>(_ => db.GetCustomer(id))
              from a in Exc.Create<Account, DbException>(_ => db.GetAccount(c.AccountId))
              where a.IsActive
              select a; // Exc<Account, DbException>
```

The purpose of these functions is to be able to write using the `query syntax` instead of the `fluent API` as sometimes it makes the code more readable. The same expression can be expressed using the `fluent API`.

```c#
var account = Exc.Create<Customer, DbException>(_ => db.GetCustomer(id)).SelectMany(
    c => Exc.Create<Account, DbException>(_ => db.GetAccount(c.AccountId)),
    (c, a) => a
).Where(a => a.IsActive);
```

As we see, the `query syntax` makes this expression quite readable. For a detailed explanation of how LINQ integration works with monadic types, refer to the [Maybe LINQ compatibility](/Funk/types/maybe/#linq-compatibility) section.
