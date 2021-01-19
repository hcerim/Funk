---
layout: page
title: Record
parent: Types 
permalink: /types/record/
nav_order: 3
---

# Record -> Product of values

A `Record` type represents a product of set values. It is a similar concept to the `ValueTuple` type but it provides additional features and its inner values are immutable. **It is a value type (`struct`) and therefore can't be null.**

The `ValueTuple` type is quite useful in situations when two distinct objects need to be part of the same structure without the need for creating a dedicated type. However, since its inner values are mutable and since it's quite tedious to transform one `ValueTuple` object to another one with the same arity, Funk provides a `Record` type to compensate for these flaws. A `Record` type is also a `functor` and a `monad` as it provides the corresponding mapping and binding functions (**see the [Maybe](/Funk/types/maybe) type for an explanation of these concepts**). Funk is trying to encourage the correct code design, so it only provides the `Record` type up to an arity of 5 (`Record<T1,..,T5>`). If you need more than that, it is probably time to rethink your design.

## Lifting functions

There are a few explicit ways of creating a `Record` object.

```c#
var recordOf2 = Record.Create("John Doe", 30); // using a factory method -> Record<string, int>
var recordOf3 = ("Jane", "Doe", 30).ToRecord(); // using an extension method -> Record<string, string, int>
var recordOf1 = rec(customers.Get(id)); // using a Prelude function -> Record<Customer>
```

In the first line, we are creating a Record object from two independent objects (we could also create it from the `ValueTuple`). In the second line, we create it from the `ValueTuple` using an extension method. The third one is the simplest and it can be really useful to easily replace all the `ValueTuple` objects in our code. To use the `rec` function, you need to import the `Prelude` as a static reference.

There is an implicit conversion between a `ValueTuple` and a `Record` of the same arity so the following code is legal.

```c#
public static Record<string, int> GetRecord((string, int) item) => item;
```

## Deconstruction

Same as with the `ValueTuple`, you can deconstruct the `Record` object. When we are working with the same type of underlying items of the `Record`, it can be really helpful to be able to deconstruct the object to avoid the potential mistake and still manage to keep the code size intact. So instead of assigning each underlying item to a variable one by one, we can do it as shown in the following example.

```c#
var (name, surname) = GetNameWithSurname(id); // Record<string, string>
```

## Immutability

`Record`'s inner values are **immutable**. From the following code we can see that the attempt of changing the value of the `Record`'s inner item results in the compile-time error.

```c#
var record = GetRecord(("John", 30));
var name = record.Item1; // "John"

record.Item1 = "Jane"; // compile-time error
```

As we see from the example, the `Record` has the same naming of its inner values as the `ValueTuple`. However, as opposed to the `ValueTuple`, it is not possible to change the inner value of the `Record` object.

We have to be careful here though as if the inner value of the `Record` object happens to be a reference type or a value type with inner properties, then modifying that object will result in the modified inner value of that `Record`. So this immutability is here actually to **prevent the direct attempt of modification**.

Immutability is great but it comes with certain costs and the biggest one is the tedious transformation process. Since we cannot modify the inner value directly, to do so, we need to create a new object. This is especially painful when we work with `Record` objects of larger arity. Because of that, the `Record` type provides mapping and binding functions that can help abstract away this issue.

## Pattern-matching

One of the ways for working with the `Record` type is using the `pattern-matching` approach. `Match` functions provided allow you to extract the underlying values and transform them into a specified result.

```c#
var john = rec("John", "Doe", 30);
var concatenated = john.Match(
    (name, surname, age) => $"{name} {surname} is {age} years old."
);
```

The `Match` function provides a fluent way of extracting all the inner values of the specific `Record` object and uses them in the provided function. It also works with the `Action` type delegates.

## Functor

The `Record` type provides the `Map` function to easily transform one `Record` object to another of the same arity. We could have two related accounts, from which we would like to retrieve corresponding contracts.

```c#
public Record<Account, Account> GetAccountWithSubAccount(Guid id)
{
    // .. implementation ..
}

// ..

var account = GetAccountWithSubAccount(id);
var (accountContract, subAccountContract) = account.Map((a, s) =>
    (accounts.GetContract(a.ContractId), accounts.GetContract(s.ContractId))
); // Record<Contract, Contract>
```

Here, we are mapping the underlying values and retrieving the `ValueTuple` object in the specified function. The `Map` function is automatically converting it to the `Record` object which we are later deconstructing. We managed to express this operation without the need for using statements.

**There is also an `async` version of the `Map` function.**

## Monad

The `Record` type also provides the `FlatMap` function in case we need to flatten the result to avoid object nesting. When the function provided as an argument inside the `Map` function returns the `Record` object instead of the `ValueTuple`, we need to use the `FlatMap` function instead.

```c#
var (registrationContract, accountContract) = await customer.GetWithAccountAsync(id).FlatMapAsync(async (c, a) =>
    rec
    (
        await customers.GetContract(c.ContractId),
        await accounts.GetContract(a.ContractId)
    )
);
```

Here, the `GetWithAccountAsync` function is returning the `Task<Record<Customer, Account>>`. We are then using the `async` version of the `FlatMap` to execute this operation asynchronously.

## Records with single value

The `Record` type supports the creation of the `Record` object of **one** value whereas the `ValueTuple` must comprise at least **two** objects. So, even when you don't have more values that should be somehow connected, you can still use the features that the `Record` type provides (**immutability**, **mapping** and **flattening** features, and **pattern-matching**) by simply lifting the object to the elevated world of the `Record` type.