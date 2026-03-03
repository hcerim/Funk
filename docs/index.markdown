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

Fluent immutable updates — create modified copies without mutation. Ideal for domain models as well as ORM entities:

```csharp
public class User : Data<User>
{
    public string Name { get; private set; }
    public int Age { get; private set; }

    public User(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

var alice = new User("Alice", 30);

// Create a modified copy — original is unchanged
var older = alice.With(u => u.Age, 31).Build();

// Chain multiple modifications
var renamed = alice
    .With(u => u.Name, "Bob")
    .With(u => u.Age, 25)
    .Build();
```

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

## Documentation

For full API documentation, visit the [**Funk documentation site**](https://hcerim.github.io/Funk).

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
