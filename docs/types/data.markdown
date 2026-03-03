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

To use `Data`, your class needs to inherit from `Data<T>` where `T` is the class itself. If you are familiar with design patterns, this is sometimes called the CRTP (Curiously Recurring Template Pattern). Properties should be read-only (get-only) to enforce immutability. The class must have public get accessors for properties that you want to modify. The `Data` type handles deep copying internally using reflection, so no serialization framework is required.

```c#
public class Customer : Data<Customer>
{
    public Customer(string name, string email, int age)
    {
        Name = name;
        Email = email;
        Age = age;
    }

    public string Name { get; }
    public string Email { get; }
    public int Age { get; }
}
```

As you see, there is nothing special here. You define your class as you normally would, but instead of inheriting from `object`, you inherit from `Data<Customer>`. That single change gives you the ability to create modified copies of your objects in a fluent and type-safe way.

## Creating modified copies

Once your class inherits from `Data<T>`, you get access to the `WithBuild` function. It creates a new object with the specified property changed while leaving the original object untouched — this is true immutability.

```c#
var john = new Customer("John Doe", "john@example.com", 30);

// Using WithBuild - creates a new object immediately
var jane = john.WithBuild(c => c.Name, "Jane Doe"); // Customer with Name = "Jane Doe"

// john is unchanged - immutability preserved
Console.WriteLine(john.Name); // "John Doe"
Console.WriteLine(jane.Name); // "Jane Doe"
```

The expression `c => c.Name` is a **type-safe** property selector — the compiler ensures you can only pass a value of the matching type. If you tried to pass an `int` where a `string` is expected, you would get a compile-time error. No runtime surprises.

## Fluent builder

When you need to modify multiple properties, creating intermediate objects with `WithBuild` for each change is wasteful. Each call deep-copies the entire object just to change one property. The `Builder<T>` type solves this by batching modifications.

```c#
var updated = john
    .With(c => c.Name, "Jane Doe")
    .With(c => c.Email, "jane@example.com")
    .With(c => c.Age, 28)
    .Build(); // Customer
```

`With` returns a `Builder<T>` object that accumulates the modifications. No copying happens until `Build` is called. This is more efficient when modifying multiple properties as the object is only deep-copied once.

`Builder<T>` also has `WithBuild` as a shortcut for the last modification in the chain. Instead of calling `With` followed by `Build`, you can use `WithBuild` as the final call.

```c#
var updated = john
    .With(c => c.Name, "Jane Doe")
    .With(c => c.Email, "jane@example.com")
    .WithBuild(c => c.Age, 28); // Customer
```

The result is the same — a new `Customer` object with all three properties modified. It just saves you the extra `Build` call.

## Copying objects

Sometimes you just need an exact copy of an object without modifying anything. The `From` function does exactly that.

```c#
var copy = Customer.From(john); // deep copy of john
```

`From` creates a deep copy of the object. The deep copy handles:

- **Primitive types, value types, strings** — returned as-is (they are already immutable)
- **Reference types** — recursively deep-copied
- **Arrays** — element-by-element deep copy
- **Circular references** — tracked and handled to prevent infinite recursion
- **Delegates** — returned as-is

If the copy operation fails, a `SerializationException` is thrown. This is a Funk-specific exception (see **[Maybe](/Funk/types/maybe)** for examples of Funk's approach to error handling).

## Nested properties

The expression selector supports nested properties and fields. Imagine you have an `Order` that contains a `Customer`.

```c#
public class Order : Data<Order>
{
    public Order(Customer customer, decimal total)
    {
        Customer = customer;
        Total = total;
    }

    public Customer Customer { get; }
    public decimal Total { get; }
}

var order = new Order(john, 100m);
var updated = order.WithBuild(o => o.Customer.Email, "new@example.com"); // Order
```

You can target nested properties through the expression tree. The `Data` type will traverse the object graph to find and modify the correct property. The original `Order` and its `Customer` remain unchanged — a completely new object graph is created.

## Implicit conversion

There is an implicit conversion from `Data<T>` to `Builder<T>`, enabling seamless integration into fluent pipelines.

```c#
Builder<Customer> builder = john; // implicit from Data<T> to Builder<T>
var result = builder.WithBuild(c => c.Name, "Jane"); // Customer
```

This can be useful when you want to pass a `Data<T>` object into a function that expects a `Builder<T>` or when you want to start building from an existing object in a more flexible way.

## Key characteristics

To summarize, the `Data<T>` type provides:

- **True immutability**: Original objects are never modified — new copies are created
- **Deep copying**: All reference types are recursively copied to prevent shared state
- **Type-safe modifications**: Expression-based selectors ensure compile-time type checking
- **Fluent API**: Chain multiple modifications before building
- **Nested property support**: Modify deeply nested properties through expression trees
- **No external dependencies**: Deep copy is implemented using reflection — no serialization framework required
