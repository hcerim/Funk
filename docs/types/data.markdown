---
layout: page
title: Data
parent: Types 
permalink: /types/data/
nav_order: 7
---

# Data -> Immutable domain objects

Immutability is a core principle in functional programming. When objects are immutable, you don't have to worry about shared state, race conditions, or unintended side effects. However, in C#, creating immutable objects is tedious. When you need to change a single property, you have to create an entirely new object, passing all other properties through the constructor. This becomes extremely painful with objects that have many properties — change one thing and you are rewriting the entire constructor call.

The `Data<T>` type provides a fluent builder pattern for creating **new** immutable objects from existing ones. If you come from F#, think of the `with` expression for records. If you come from Scala, think of the `copy` method on case classes. `Data<T>` achieves this through deep copying and expression-based property/field modification. The modification is **type-safe** — the compiler ensures the type of the new value matches the type of the property/field being modified.

`Data<T>` is an abstract class that your domain types inherit from. `Builder<T>` is the intermediate type that holds pending modifications until `Build` is called.

## Defining Data types

To use `Data`, your class needs to inherit from `Data<T>` where `T` is the class itself. If you are familiar with design patterns, this is sometimes called the CRTP (Curiously Recurring Template Pattern). Properties should have `private set` accessors — this enforces immutability from the outside while allowing the `Data` type to modify them internally via reflection. The class must have public get accessors for properties that you want to modify. The `Data` type handles deep copying internally using reflection, so no serialization framework is required.

```c#
public sealed class Account : Data<Account>
{
    public string EmailAddress { get; private set; }
    public string Status { get; private set; }
    public string Type { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    private Account() { }
    public static Account New => new();
}
```

As you see, there is nothing special here. You define your class as you normally would, but instead of inheriting from `object`, you inherit from `Data<Account>`. That single change gives you the ability to create modified copies of your objects in a fluent and type-safe way.

## Creating modified copies

Once your class inherits from `Data<T>`, you get access to the `WithBuild` function. It creates a new object with the specified property changed while leaving the original object untouched — this is true immutability.

```c#
var account = Account.New
    .WithBuild(a => a.EmailAddress, "alice@example.com"); // Account with EmailAddress set

// account is unchanged when we create a new copy
var updated = account.WithBuild(a => a.Status, "Active"); // new Account with Status = "Active"
```

The expression `a => a.EmailAddress` is a **type-safe** property selector — the compiler ensures you can only pass a value of the matching type. If you tried to pass an `int` where a `string` is expected, you would get a compile-time error. No runtime surprises.

## Fluent builder

When you need to modify multiple properties, creating intermediate objects with `WithBuild` for each change is wasteful. Each call deep-copies the entire object just to change one property. The `Builder<T>` type solves this by batching modifications.

```c#
var account = Account.New
    .With(a => a.EmailAddress, "alice@example.com")
    .With(a => a.Status, "Active")
    .With(a => a.Type, "Personal")
    .With(a => a.FirstName, "Alice")
    .With(a => a.LastName, "Smith")
    .Build(); // Account
```

`With` returns a `Builder<T>` object that accumulates the modifications. No copying happens until `Build` is called. This is more efficient when modifying multiple properties as the object is only deep-copied once.

`Builder<T>` also has `WithBuild` as a shortcut for the last modification in the chain. Instead of calling `With` followed by `Build`, you can use `WithBuild` as the final call.

```c#
var account = Account.New
    .With(a => a.EmailAddress, "alice@example.com")
    .With(a => a.Status, "Active")
    .WithBuild(a => a.Type, "Personal"); // Account
```

The result is the same — a new `Account` object with all three properties modified. It just saves you the extra `Build` call.

## Copying objects

Sometimes you just need an exact copy of an object without modifying anything. The `From` function does exactly that.

```c#
var copy = Account.From(account); // deep copy of account
```

`From` creates a deep copy of the object. The deep copy handles:

- **Primitive types, value types, strings** — returned as-is (they are already immutable)
- **Reference types** — recursively deep-copied
- **Arrays** — element-by-element deep copy
- **Circular references** — tracked and handled to prevent infinite recursion
- **Delegates** — returned as-is

If the copy operation fails, a `SerializationException` is thrown. This is a Funk-specific exception (see **[Maybe](/Funk/types/maybe)** for examples of Funk's approach to error handling).

## Nested properties

The expression selector supports nested properties and fields. Imagine you have an `Order` that contains an `Account`.

```c#
public class Order : Data<Order>
{
    public Order(Account account, decimal total)
    {
        Account = account;
        Total = total;
    }

    public Account Account { get; private set; }
    public decimal Total { get; private set; }
}

var order = new Order(account, 100m);
var updated = order.WithBuild(o => o.Account.LastName, "Johnson"); // Order
```

You can target nested properties through the expression tree. The `Data` type will traverse the object graph to find and modify the correct property. The original `Order` and its `Account` remain unchanged — a completely new object graph is created.

## F-bounded polymorphism (type hierarchies)

When building type hierarchies with `Data<T>`, use **F-bounded polymorphism** — make the base class generic in its derived type so that `With`/`Build` always return the concrete type:

```c#
public interface IEntity
{
    Guid Id { get; }
}

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

Because `Account : Entity<Account>`, the type parameter `T` resolves to `Account`, so `With`/`Build` returns `Account` — not `Entity`:

```c#
var account = Account.New
    .With(a => a.EmailAddress, "alice@example.com")
    .With(a => a.Status, "Active")
    .With(a => a.Type, "Personal")
    .With(a => a.CreatedBy, adminId)
    .With(a => a.ModifiedBy, adminId)
    .Build(); // Account — not Entity
```

### A note on the generic constraint

The constraint `where T : Entity<T>` is stricter than `where T : Data<T>`. While both work in practice — since `Entity<T>` extends `Data<T>`, any `T` satisfying `Entity<T>` automatically satisfies `Data<T>` through inheritance — they communicate different intent:

- `where T : Data<T>` is the **minimum** required by the `With`/`Build` mechanism. It allows any `Data<T>` subclass as the type parameter.
- `where T : Entity<T>` is **stricter** — it ensures the type parameter is specifically part of the `Entity` hierarchy, not just any `Data<T>`. This is the right choice when the base class introduces domain-specific members (like `Id`, `CreatedAt`, `Version`) that derived types must inherit.

When you write `Account : Entity<Account>`, `Account` satisfies both `Data<Account>` and `Entity<Account>`. The `With`/`Build` mechanism works unchanged because it is defined on `Data<T>`.

### The non-generic interface pattern

Since `Entity<T>` is generic, you can't use it for polymorphic collections like `List<Entity>`. The non-generic `IEntity` interface solves this:

```c#
List<IEntity> entities = new() { account, otherEntity };
var ids = entities.Map(e => e.Id); // IImmutableList<Guid>
```

This gives you the best of both worlds — type-safe `With`/`Build` on concrete types, and polymorphism through the interface.

## Implicit conversion

There is an implicit conversion from `Data<T>` to `Builder<T>`, enabling seamless integration into fluent pipelines.

```c#
Builder<Account> builder = account; // implicit from Data<T> to Builder<T>
var result = builder.WithBuild(a => a.Status, "Active"); // Account
```

This can be useful when you want to pass a `Data<T>` object into a function that expects a `Builder<T>` or when you want to start building from an existing object in a more flexible way.

## Key characteristics

To summarize, the `Data<T>` type provides:

- **True immutability**: Original objects are never modified — new copies are created
- **Deep copying**: All reference types are recursively copied to prevent shared state
- **Type-safe modifications**: Expression-based selectors ensure compile-time type checking
- **Fluent API**: Chain multiple modifications before building
- **Nested property support**: Modify deeply nested properties through expression trees
- **F-bounded polymorphism**: Type hierarchies work correctly with generic base classes
- **No external dependencies**: Deep copy is implemented using reflection — no serialization framework required
