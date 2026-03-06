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

## Data&lt;T&gt; vs C# records and `with` for EF Core entities

C# 9 introduced records with `with` expressions — the language's native answer to creating modified copies. While records work well for simple DTOs, configuration objects, and value types where shallow copy is sufficient, they fall short for EF Core entity modeling where object graph integrity, deep copying, type hierarchies, and controlled mutation matter.

### Shallow copy vs deep copy

The C# `with` expression performs a **shallow member-wise copy**. For an entity with navigation properties, this is dangerous:

```c#
public record Order(Guid Id, string Status, List<OrderItem> Items);

var order = db.Orders.Include(o => o.Items).First();
var updated = order with { Status = "Shipped" };

// PROBLEM: updated.Items is the SAME LIST REFERENCE as order.Items.
// Mutating one mutates the other. Adding updated to the DbContext
// while order is tracked creates duplicate tracking conflicts.
```

`Data<T>` performs **deep copying** — the entire object graph is recursively cloned. Navigation collections, nested entities, and all reference types get independent copies:

```c#
public class Order : Data<Order>
{
    public Guid Id { get; private set; }
    public string Status { get; private set; }
    public List<OrderItem> Items { get; private set; }
}

var updated = order.WithBuild(o => o.Status, "Shipped");
// updated.Items is a DEEP COPY — completely independent from order.Items
```

### Nested property modification

EF Core entities often have owned types or value objects embedded within them:

```c#
// With records — cascading 'with' per level
var updated = customer with
{
    Address = customer.Address with { City = "Vienna" }
};

// With Data<T> — single expression, any depth
var updated = customer.WithBuild(c => c.Address.City, "Vienna");
```

The expression tree traversal in `Data<T>` handles arbitrary nesting depth. This scales — imagine an entity with 3 levels of nested owned types. With `with`, each level requires another `with` expression. With `Data<T>`, it is always one call.

### Type hierarchies and F-bounded polymorphism

EF Core entity hierarchies commonly use a base `Entity` class. Records cannot express F-bounded polymorphism — there is no way to make `with` on a base record return the concrete derived type:

```c#
// Records — 'with' returns the base type in generic code
public abstract record Entity(Guid Id, DateTime CreatedAt);
public record Customer(Guid Id, DateTime CreatedAt, string Email) : Entity(Id, CreatedAt);

// In generic code operating on Entity, 'with' returns Entity — not Customer.
// Records also require repeating all base parameters in the positional syntax,
// which becomes unwieldy with many base properties.
```

`Data<T>` with F-bounded polymorphism preserves the concrete type:

```c#
public abstract class Entity<T> : Data<T>, IEntity where T : Entity<T>
{
    [Key] public Guid Id { get; private set; } = Guid.NewGuid();
    [Required] public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    // ... other base properties
}

public sealed class Customer : Entity<Customer>
{
    [Required, MaxLength(255)] public string Email { get; private set; }
    private Customer() { }
    public static Customer New => new();
}

// With/Build returns Customer, not Entity
var customer = Customer.New
    .With(c => c.Email, "alice@example.com")
    .With(c => c.CreatedBy, adminId)
    .Build(); // Customer — not Entity
```

### Private setters and EF Core compatibility

EF Core fully supports `private set` properties — the change tracker uses reflection to set values. This is exactly what `Data<T>` requires. The pattern is consistent: external immutability with internal mutability via reflection.

Records with positional syntax generate `init` setters. EF Core can work with `init` (also via reflection), but there are subtle issues:

- EF Core's `Update` and `Attach` methods work best with settable properties. `init` properties can cause issues with certain change tracking strategies.
- Lazy loading proxies require `virtual` navigation properties on non-sealed classes. The conventional positional record style discourages `virtual` members.
- EF Core's `ValueComparer` and snapshot change tracking need to copy property values. `private set` gives them a reliable mutation path.

### Fluent builder vs cascading `with`

For entity creation, records require all properties in the constructor or unordered object initializers. With `Data<T>`, the builder pattern provides named, type-safe, ordered construction:

```c#
var customer = Customer.New
    .With(c => c.Email, "alice@example.com")
    .With(c => c.Status, "Active")
    .With(c => c.Type, "Personal")
    .With(c => c.CreatedBy, adminId)
    .With(c => c.ModifiedBy, adminId)
    .Build();
```

Each `With` call is checked at compile time — the expression `c => c.Email` constrains the value to be a `string`. The builder accumulates modifications and deep-copies only once at `Build()`.

### Change tracking and detached entities

When modifying a tracked entity for update:

```c#
// With records — shared references cause tracking conflicts
var order = db.Orders.Include(o => o.Items).First();  // tracked
var updated = order with { Status = "Shipped" };       // detached, shares Items reference
db.Entry(order).State = EntityState.Detached;
db.Update(updated); // RISK: shared navigation references

// With Data<T> — completely independent object graph
var order = db.Orders.Include(o => o.Items).First();
var updated = order
    .With(o => o.Status, "Shipped")
    .With(o => o.ModifiedAt, DateTime.UtcNow)
    .Build(); // deep copy — no shared state
db.Entry(order).State = EntityState.Detached;
db.Update(updated); // safe — independent object graph
```

### Summary

| Concern | C# Records + `with` | `Data<T>` + `With`/`Build` |
|---------|---------------------|---------------------------|
| **Copy depth** | Shallow (shared references) | Deep (independent graph) |
| **Nested modification** | Cascading `with` per level | Single expression, any depth |
| **Type hierarchies** | No F-bounded polymorphism | Full CRTP support |
| **Return type in hierarchies** | Base type in generic code | Concrete derived type |
| **EF Core private setters** | Uses `init` (reflection-dependent) | Uses `private set` (EF Core standard) |
| **Builder pattern** | Not available | Fluent, batched, one deep-copy |
| **Navigation property safety** | References shared after `with` | Deep-copied, independent |
| **Constructor ergonomics** | All params required positionally | Named, incremental, type-safe |

Records with `with` are the right tool for simple DTOs, configuration objects, and value types where shallow copy is sufficient. For EF Core entities — where object graph integrity, deep copying, type hierarchies, and controlled mutation matter — `Data<T>` provides guarantees that records cannot.

## Key characteristics

To summarize, the `Data<T>` type provides:

- **True immutability**: Original objects are never modified — new copies are created
- **Deep copying**: All reference types are recursively copied to prevent shared state
- **Type-safe modifications**: Expression-based selectors ensure compile-time type checking
- **Fluent API**: Chain multiple modifications before building
- **Nested property support**: Modify deeply nested properties through expression trees
- **F-bounded polymorphism**: Type hierarchies work correctly with generic base classes
- **No external dependencies**: Deep copy is implemented using reflection — no serialization framework required
