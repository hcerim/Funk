---
layout: home
title: Home
nav_order: 1
---

# Funk — Functional C#

A lightweight functional programming library that brings expressive, composable, and safe abstractions to C#. Less ceremony, more clarity.

## Features

- **Maybe&lt;T&gt;** — explicit nullability without null reference exceptions
- **Exc&lt;T, E&gt;** — railway-oriented error handling with success, failure, and empty states
- **OneOf&lt;T1, …, T5&gt;** — type-safe discriminated unions with exhaustive matching
- **Record&lt;T1, …, T5&gt;** — immutable products with safe deconstruction and mapping
- **Pattern&lt;R&gt;** — lazy, expression-based pattern matching (sync and async)
- **Data&lt;T&gt; & Builder&lt;T&gt;** — fluent immutable object updates
- **Prelude** — terse factory functions (`may`, `rec`, `list`, …)
- **Extensions** — functional combinators on objects, tasks, enumerables, and actions

## Installation

Funk is available as a [**NuGet**](https://www.nuget.org/packages/Funk) package.

```
dotnet add package Funk
```

**Supports:** .NET 8+, .NET Standard 2.0 / 2.1

## Usage

Add the namespace and optionally import the Prelude for terse factory functions:

```csharp
using Funk;
using static Funk.Prelude;
```

### A note on Nullable Reference Types

Funk deliberately does not adopt nullable reference types (NRTs). Unlike nullable value types — where `int?` is `Nullable<int>`, a real generic struct enforced at both compile time and runtime — `string?` is not a distinct type. It is `string` at the IL level, with erasable compiler annotations that produce warnings but no runtime guarantees. `null` can still flow into a `string` parameter through reflection, interop, older libraries, or explicit suppression with `null!`.

`Maybe<T>` is the type-level solution: a `readonly struct` that forces the caller to `Match` or `Map` to extract the value. The empty case is structurally irrepresentable as a bare `T` — you must handle it. This is encoding invariants in the type system, not relying on advisory compiler hints.

### Maybe&lt;T&gt;

Represent the possible absence of a value — no more nulls.

```csharp
// Create from a value or null
Maybe<string> name = may("Funk");     // NotEmpty
Maybe<string> none = may<string>(null); // IsEmpty

// Pattern match to safely extract
string greeting = name.Match(
    _ => "No name provided",
    n => $"Hello, {n}!"
);

// Map — transform plain values while staying in Maybe
Maybe<int> length = name.Map(n => n.Length);         // Maybe<int> = 4

// FlatMap — chain operations that themselves return Maybe
Maybe<UserProfile> GetProfile(string name) => profiles.Get(name);
Maybe<Theme> GetTheme(Guid themeId) => themes.Get(themeId);

var theme = name
    .FlatMap(n => GetProfile(n))
    .FlatMap(p => GetTheme(p.ThemeId)); // Maybe<Theme>

// Get with a fallback
string value = name.GetOr(_ => "default");
```

### Exc&lt;T, E&gt;

Railway-oriented error handling — operations that can succeed, fail, or be empty.

```csharp
// Wrap an operation that might throw
Exc<int, FormatException> parsed = Exc.Create<int, FormatException>(
    _ => int.Parse("42")
);

// Map — transform plain values while keeping exception safety
Exc<string, FormatException> result = parsed.Map(n => $"The answer is {n}");

// FlatMap — chain operations that themselves return Exc
Exc<Customer, DbException> GetCustomer(Guid id) => Exc.Create<Customer, DbException>(_ => db.Find(id));
Exc<Account, DbException> GetAccount(Guid accountId) => Exc.Create<Account, DbException>(_ => db.FindAccount(accountId));

var account = GetCustomer(id)
    .FlatMap(c => GetAccount(c.AccountId)); // Exc<Account, DbException>

// Pattern match all three states
string message = parsed.Match(
    ifEmpty:   _ => "Nothing to parse",
    ifSuccess: n => $"Parsed: {n}",
    ifFailure: e => $"Error: {e.Root}"
);

// Deconstruct into success and failure as Maybe values
var (success, failure) = parsed;
// success: Maybe<int>, failure: Maybe<EnumerableException<FormatException>>

// Recover from failure with OnFailure — chain fallbacks
var config = Exc.Create<Config, IOException>(_ => LoadConfigFromFile())
    .OnFailure(e => LoadConfigFromNetwork())
    .OnFailure(e => GetDefaultConfig());

// OnFlatFailure when the recovery itself returns an Exc
var data = Exc.Create<string, DbException>(_ => db.GetFromPrimary())
    .OnFlatFailure(e => Exc.Create<string, DbException>(_ => db.GetFromReplica()));

// OnEmpty — recover when result is empty (distinct from failure)
var result = Exc.Create<Config, IOException>(_ => LoadConfigFromFile())
    .OnFailure(e => GetNullableConfig())
    .OnEmpty(_ => GetDefaultConfig());

// Async variants
var asyncResult = await Exc.CreateAsync<string, HttpRequestException>(
    _ => httpClient.GetStringAsync("https://api.example.com/data")
).OnFailureAsync(e => httpClient.GetStringAsync("https://api.example.com/fallback"));
```

### Pattern matching

Lazy, expression-based matching with collection initializer syntax:

```csharp
int statusCode = 404;

// Value-based matching
string status = new Pattern<string>
{
    (200, _ => "OK"),
    (404, _ => "Not Found"),
    (500, _ => "Internal Server Error")
}.Match(statusCode).GetOr(_ => "Unknown");

// Predicate-based matching
string range = new Pattern<string>
{
    (x => x < 200, (int _) => "Informational"),
    (x => x < 300, (int _) => "Success"),
    (x => x < 400, (int _) => "Redirection"),
    (x => x < 500, (int _) => "Client Error")
}.Match(statusCode).GetOr(_ => "Server Error");

// Type-based matching
object shape = new Circle(5);

string description = new TypePattern<string>
{
    (Circle c) => $"Circle with radius {c.Radius}",
    (Square s) => $"Square with side {s.Side}"
}.Match(shape).GetOr(_ => "Unknown shape");

// Async pattern matching
string body = await new AsyncPattern<string>
{
    (200, _ => httpClient.GetStringAsync("/ok")),
    (404, _ => httpClient.GetStringAsync("/not-found"))
}.Match(statusCode).GetOrAsync(_ => Task.FromResult("Fallback"));

// Async type-based matching
string info = await new AsyncTypePattern<string>
{
    (Circle c) => ComputeAreaAsync(c),
    (Square s) => ComputeAreaAsync(s)
}.Match(shape).GetOrAsync(_ => Task.FromResult("Unknown shape"));
```

### Data&lt;T&gt; & Builder&lt;T&gt;

Fluent immutable updates — create modified copies without mutation. Ideal for domain models and ORM entities.

`Data<T>` uses the CRTP (Curiously Recurring Template Pattern). For type hierarchies, use F-bounded polymorphism — make the base class generic in its derived type so that `With`/`Build` return the concrete type:

```csharp
public interface IEntity { Guid Id { get; } }

public abstract class Entity<T> : Data<T>, IEntity where T : Entity<T>
{
    [Key] public Guid Id { get; private set; } = Guid.NewGuid();
    [Required] public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    [Required] public DateTime ModifiedAt { get; private set; } = DateTime.UtcNow;
    [Required] public Guid CreatedBy { get; private set; }
    [Required] public Guid ModifiedBy { get; private set; }
    [Required, Min(1)] public uint Version { get; private set; } = 1;
    public IEntity WithVersion(uint version)
    {
        Version = version;
        return this;
    }
}

public sealed class Account : Entity<Account>
{
    [Required, MaxLength(255)] public string EmailAddress { get; private set; }
    [Required, MaxLength(50)] public string Status { get; private set; }
    [Required, MaxLength(50)] public string Type { get; private set; }
    private Account() { }
    public static Account New => new();
}
```

The constraint `where T : Entity<T>` is stricter than `where T : Data<T>`. While `Data<T>` is the minimum required for `With`/`Build` to work, using `Entity<T>` as the bound ensures that `T` is specifically part of the `Entity` hierarchy — not just any `Data<T>`. Since `Entity<T>` extends `Data<T>`, any `T` satisfying `Entity<T>` automatically satisfies `Data<T>` through inheritance, so the `With`/`Build` mechanism works unchanged.

```csharp
// Build a new entity — With/Build returns Account, not Entity
var account = Account.New
    .With(a => a.EmailAddress, "alice@example.com")
    .With(a => a.Status, "Active")
    .With(a => a.Type, "Personal")
    .With(a => a.CreatedBy, adminId)
    .With(a => a.ModifiedBy, adminId)
    .Build(); // Account — not Entity

// Create a modified copy — original is unchanged
var updated = account
    .With(a => a.Status, "Suspended")
    .With(a => a.ModifiedAt, DateTime.UtcNow)
    .Build();
```

#### Why Data&lt;T&gt; over C# records for EF Core entities

C# 9 records with `with` expressions solve a similar problem — creating modified copies — but fall short for EF Core entity modeling:

| Concern | C# Records + `with` | `Data<T>` + `With`/`Build` |
|---------|---------------------|---------------------------|
| **Copy depth** | Shallow (shared references) | Deep (independent graph) |
| **Nested modification** | Cascading `with` per level | Single expression, any depth |
| **Type hierarchies** | No F-bounded polymorphism | Full CRTP support |
| **Return type in hierarchies** | Base type in generic code | Concrete derived type |
| **EF Core private setters** | Uses `init` (reflection-dependent) | Uses `private set` (EF Core standard) |
| **Builder pattern** | Not available | Fluent, batched, one deep-copy |
| **Navigation property safety** | References shared after `with` | Deep-copied, independent |

The `with` expression performs a **shallow member-wise copy** — if an entity has navigation properties (collections, references), the original and the copy share the same objects. This causes change tracking conflicts when both are used with a `DbContext`. `Data<T>` performs **deep copying**, producing a completely independent object graph.

Records also cannot express F-bounded polymorphism (`Entity<T> : Data<T> where T : Entity<T>`), meaning `with` on a base record returns the base type, not the concrete derived type. `Data<T>` preserves the concrete type through the entire `With`/`Build` chain.

For a detailed analysis, see the [Data documentation](/Funk/types/data/).

### OneOf&lt;T1, …, T5&gt;

Type-safe discriminated unions:

```csharp
// A value that is either a string or an int
var result = new OneOf<string, int>("hello");

string output = result.Match(
    _ => "empty",
    s => $"String: {s}",
    n => $"Number: {n}"
);

// Access individual states safely via Maybe
Maybe<string> asString = result.First;  // NotEmpty
Maybe<int> asInt = result.Second;       // IsEmpty

// Deconstruct into Maybe values
var (first, second) = result;
// first: Maybe<string>, second: Maybe<int>
```

### Record&lt;T1, …, T5&gt;

Immutable products with safe deconstruction and mapping:

```csharp
// Create a record using the Prelude
var person = rec("Alice", 30);

// Deconstruct
var (name, age) = person;

// Map to a new record
var updated = person.Map((n, a) => (n.ToUpper(), a + 1));

// Match to extract a result
string description = person.Match((n, a) => $"{n} is {a} years old");
```

### LINQ query syntax

Maybe and Exc support C# query expressions for composing operations naturally:

```csharp
// Maybe — compose multiple lookups
Maybe<string> city =
    from user in FindUser("alice")
    from address in user.Address.AsMaybe()
    where address.Country == "US"
    select address.City;

// Exc — chain operations that can fail
Exc<decimal, Exception> total =
    from order in LoadOrder(orderId)
    from discount in ApplyDiscount(order)
    select order.Amount - discount;
```

### Piping

Transform values through fluent pipelines:

```csharp
var result = "hello"
    .Do(s => s.ToUpper())
    .Do(s => $"{s}!");    // "HELLO!"
```

### Currying

Transform multi-parameter functions into chains of single-parameter functions:

```csharp
Func<int, int, int> add = (a, b) => a + b;
var curriedAdd = add.Curry(); // Func<int, Func<int, int>>

var addFive = curriedAdd(5); // Func<int, int>
var result = addFive(3);     // 8
```

### Partial application

Apply arguments one at a time, reducing arity at each step:

```csharp
Func<string, int, string> repeat = (s, n) => string.Concat(Enumerable.Repeat(s, n));
var repeatHello = repeat.Apply("hello "); // Func<int, string>
var result = repeatHello(3);              // "hello hello hello "
```

### Function composition

Combine functions into pipelines with `ComposeLeft` (left-to-right) and `ComposeRight` (right-to-left):

```csharp
Func<string, int> parse = int.Parse;
Func<int, string> format = n => $"Number: {n}";

var pipeline = parse.ComposeLeft(format); // Func<string, string>
var result = pipeline("42");              // "Number: 42"
```

### Applicative validation

Accumulate all errors instead of short-circuiting on the first failure:

```csharp
// Apply — short-circuits on first failure (monadic)
success<Func<string, int, User>, ValidationException>(createUser)
    .Apply(ValidateName(input))   // fails → stops
    .Apply(ValidateAge(input));   // never checked

// Validate — collects ALL failures (applicative)
success<Func<string, int, User>, ValidationException>(createUser)
    .Validate(ValidateName(input))  // fails → keeps going
    .Validate(ValidateAge(input));  // also checked → both errors merged
```

## Documentation

For full API documentation, visit the [**Funk documentation site**](https://hcerim.github.io/Funk).

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
