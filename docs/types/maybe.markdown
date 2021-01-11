---
layout: page
title: Maybe
parent: Types 
permalink: /types/maybe/
nav_order: 2
---

# Maybe -> Possible absence of data

We all know about the one-billion-dollar mistake that the invention of the `null pointer` caused. However, it's not that big of a problem if you can find a solution to abstract away the dirty business of dealing with it. So, the problem is not that the object is `null` but the fact that you are trying to do something with it when you don't know whether it is `null` or not.

In C#, the default value for reference type variables is `null`. So the object is laying somewhere on the heap (or maybe there is no object) but the variable holds no reference to it. So, when we try to do something with that object, not knowing that there is no reference to it, we get the notorious `NullReferenceException` saying `"Object reference not set to an instance of an object"`.

In C#, we also have nullable value types (`Nullable<T> where T : struct` or simply `T?`) which is a type constructor for value types that tells us that the underlying value may not be present. From C# 8 onwards, we also have nullable reference types that can be used to warn us that the corresponding reference type object may be `null`, however, we are not forced by the compiler to handle it in any way. With `Maybe`, you will be able to address these issues with ease and have clean code without repetitive null checks, guards, etc. Additionally, you will be forced by the compiler to resolve the value before using it.

There are a few explicit ways of creating a `Maybe` object.

```c#
var name = Maybe.Create("John"); // using a factory method -> Maybe<string> 
var customer = customers.GetOrNull(id).AsMaybe(); // using an extension method -> Maybe<Customer> 
int? nullable = null;
var number = may(nullable); // using a Prelude function -> Maybe<int>
```

The important thing to note here is that the `Maybe` resolves the nullable type automatically. So, the number is of type `Maybe<int>` and not `Maybe<int?>`. To use the `may` function, you need to import the `Prelude` as a static reference.

There is an implicit conversion between an object and a `Maybe` of that object so the following is legal.

```c#
public Maybe<Customer> GetCustomer(Guid id) => customers.Get(id);
```

Here, the `Get` function returns a `Customer` object which is implicitly converted to the `Maybe<Customer>` object.

`Maybe` is a concept present in FP languages. in F#, it is called an `Option` same as in Scala. In Haskell, it is called `Maybe` the same as in Funk. In OOP, we have a pattern that tries to accomplish a similar thing called `Optional pattern`.

In Funk, the `Maybe` type is a construct that is a `functor`, an `applicative`, and a `monad` (actually, once you satisfy the rules of being a `monad` you satisfy the rules for being the first two automatically as the `monad` is the most complex and powerful of the three). Object-oriented programmers may be unfamiliar with these terms as they come from the `Category Theory`. To get familiar with these concepts, the best resource is Bartosz Milewski's [Category Theory for Programmers](https://bartoszmilewski.com/2014/10/28/category-theory-for-programmers-the-preface/).

To use Funk, you don't have to know these concepts as you will get to know them through various examples. However, learning these concepts will open a whole new world for you and will change the way you think about the software in general.

For something to be a `monad` or a `functor`, it has to obey certain rules in a specific way. You can think of them as specific type constructors (generic types) that amplify the underlying type they wrap. So, if you have a string object, it has a certain set of functions available that you can use to perform certain operations. For example, a function `Split` splits a string into substrings based on the provided character separator. Now, what a `functor` or a `monad` can do is to lift that string into what's called an `elevated world` where that string suddenly has more functions available that can be quite useful.

We are not going to explain these rules one by one here. Instead, we will see them and many other functions that Funk provides in action where it will be obvious what benefit they bring.

Let's start with a `Match` function.

`Match` is a pattern-matching function that provides a nice way of handling the possible absence of data. So let's say we have a function that might return a `Customer` and we want to get its middle name and in case there is no object, return a default placeholder.

```c#
var customer = customer.Get(id); // returns Maybe<Customer>
var middleName = customer.Match(
    _ => "", // _ is a Unit
    c => c.MiddleName // c is a Customer
); // string
```

`Match` has 2 cases it covers. The first one represents an empty case when the `Maybe` object is empty. Empty value is represented by `Unit`. The second case is executed if the `Maybe` object is not empty. This way, we expressed what we want in a pretty straightforward way without using statements.

In case we didn't care about the empty case we could just write a single case when it has value.

```c#
var middleName = customer.Match(
    c => c.MiddleName // c is a Customer
); // string
```

However, if the object is empty, `EmptyValueException` will be thrown. In case, you want to throw another type of Exception you can specify that in the second case.

```c#
var middleName = customer.Match(
    c => c.MiddleName // c is a Customer
    _ => new CustomException("Customer not found.")
); // string
```

You can also use `Action` type delegates instead of `Func` in case you don't want to return a value.

```c#
customer.Match(
    _ => Console.WriteLine("Customer not found"),
    c => Console.WriteLine(c.MiddleName)
);
```

Now, imagine a case where some customers have empty (`null`) middle names. Performing an operation even after a `Match` function on the result would cause an exception. This is because we evaluated the `Customer` object and not its inner properties.

We can fix this by using the `Map` function.

```c#
var middleName = customer.Map(c => c.MiddleName); // results in Maybe<string>
```

`Map` is a function that makes the `Maybe` type a `functor`. It takes an elevated `T` object (`Maybe<T>`) and applies a function to its inner value (if present) and returns an elevated `R` object (`Maybe<R>`). With the `Map` function, we completely solve the issue with the possible absence of data. If the `Customer` object is empty, the `Map` will not execute the provided function and will return an empty `Maybe`. In case it is not empty, it will unwrap the underlying value, execute the provided function, and wrap the result back into `Maybe`. It wraps the result back as the result itself can be empty. This way, we stay in the `elevated world`. And you will see that it is better to stay in the world of elevated values as much as possible.

Even though the `Map` function is powerful, we still need another function that can help us when working with nested `Maybe` objects. Imagine that, instead of returning the middle name, we performed some operation on a `Customer` object that returns another `Maybe` object.

```c#
var account = customer.Map(c => accounts.GetAccount(c.AccountId)); // results in Maybe<Maybe<Account>>
```

Here, the `GetAccount` function returns the `Maybe<Account>` object. We end up with the nested `Maybe` object and it becomes tricky to unwrap it.

We can fix this by using the `FlatMap` function.

```c#
var account = customer.FlatMap(c => accounts.GetAccount(c.AccountId)); // results in Maybe<Account>
```

`FlatMap` is a function that makes the `Maybe` type a `monad` (along with the lifting function that was described earlier). It takes an elevated `T` object (`Maybe<T>`) and applies a function to its inner value (if present) and returns the result of that function (`Maybe<R>`). The `Map` function uses the `FlatMap` function internally with the additional wrapping of the result. This is why a `monad` is more powerful than a `functor` as you can implement the `Map` function using the `FlatMap` function but not vice versa.

`Async` versions of `Map` and `FlatMap` are available as well (`MapAsync` and `FlatMapAsync`). `Match`, on the other hand, does not require an `async` version as you can simply return a `Task<R>` as a result of an operation.

```c#
var account = customer.FlatMapAsync(c => accounts.GetAccountAsync(c.AccountId)); // results in Task<Maybe<Account>>
```

Here, `GetAccountAsync` function returns a `Task<Account>` and we need to use the `async` version of the `FlatMap`. Otherwise, the result would be `Maybe<Task<Account>>` which doesn't make much sense as you wouldn't be able to continue working with it properly.

`Async` versions also support transformations directly on `Task<T>` as long as the `T` is a `Maybe` object.

```c#
var account = customers.GetCustomerAsync(id).FlatMapAsync(c =>
    accounts.GetAccountAsync(c.AccountId)
); // results in Task<Maybe<Account>>
```

There are also properties `IsEmpty` and `NotEmpty` that tell you whether the specified `Maybe` object is empty or not.

Another interesting function is `Or` and its `async` versions. It basically says: "Give me the first non-empty `Maybe` object from the two provided". We can do many useful operations using this function. For example, we can aggregate on the list of `Maybe` objects and find the first non-empty object.

```c#
var accounts = customers.Select(c => c.GetAccount(c.AccountId)); // IEnumerable<Maybe<Account>>
var account = accounts.Aggregate((first, second) => first.Or(_ => second)); // Maybe<Account>
```

`Or` function accepts a function and in case a first object is not empty, the function is not evaluated. This is because, we do not want to call a function if it is not necessary. We try to be as lazy as possible. Consider the following example.

```c#
var avatar = await customer.FlatMapAsync(c => images.GetAvatarAsync(c.AvatarId)).OrAsync(async _ =>
    (await images.GetDefaultAvatarAsync()).AsMaybe()
); // Maybe<Avatar>
```

The `GetDefaultAvatar` function is only called if either there is no customer or if the customer did not set up the avatar image.

Sometimes, we just want to unwrap the value without using any of the above-mentioned functions. We can use the `GetOr` and its `async` versions for that.

```c#
var middleName = customer.Map(c => c.MiddleName).GetOr(_ => ""); // string
```

`GetOr` tries to retrieve the underlying value. In case it is empty, it executes the function provided and returns its result.

*There are also `GetOrDefault` and `UnsafeGet` functions but they should be used with caution!*

`Maybe` type is a value type (`struct`) and therefore can't be null and its default value is simply an empty `Maybe` object.
