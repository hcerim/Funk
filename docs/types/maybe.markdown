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

In C#, we also have nullable value types (`Nullable<T> where T : struct` or simply `T?`) which is a type constructor for value types that tells us that the underlying value may not be present. From C# 8 onwards, we also have nullable reference types that can be used to warn us that the corresponding reference type object may be `null`, however, we are not forced by the compiler to handle it in any way. With `Maybe`, you will be able to address these issues with ease and have clean code without repetitive null checks, guards, etc. Additionally, you will be forced by the compiler to resolve the value before using it. **It is a value type (`struct`) and therefore can't be null and its default value is simply an empty `Maybe` object.**

## Lifting functions

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

Here, the `Get` function returns a `Customer` object which is implicitly converted to the `Maybe<Customer>` object. With the `Maybe` type, besides safety, we also ensure that our function is **honest**. We improve the level of **abstraction** as the caller is not forced to look into the implementation to see what happens and what does the function do if the item is not found. We do not lie to the caller saying that we will return the object no matter what. We are being honest and by that, we make it easier for the callers as they won't have to write `null checks`, `try-catch blocks`, etc. They will just need to resolve the `Maybe` type object and the following examples will show how we can do that with ease. 

`Maybe` is a concept present in FP languages. In F#, it is called an `Option` same as in Scala. In Haskell, it is called `Maybe` the same as in Funk. In OOP, we have a pattern that tries to accomplish a similar thing called `Optional pattern`.

In Funk, the `Maybe` type is a construct that is a `functor`, an `applicative`, and a `monad` (actually, once you satisfy the rules of being a `monad` you can easily satisfy the rules for being the first two as the `monad` is the most complex and powerful of the three). Object-oriented programmers may be unfamiliar with these terms as they come from the `Category Theory`. To get familiar with these concepts, the best resource is Bartosz Milewski's [Category Theory for Programmers](https://bartoszmilewski.com/2014/10/28/category-theory-for-programmers-the-preface/).

To use Funk, you don't have to know these concepts as you will get to know them through various examples. However, learning these concepts will open a whole new world for you and will change the way you think about the software in general.

For something to be a `monad` or a `functor`, it has to obey certain rules in a specific way. You can think of them as specific type constructors (generic types) that amplify the underlying type they wrap. So, if you have a string object, it has a certain set of functions available that you can use to perform certain operations. For example, a function `Split` splits a string into substrings based on the provided character separator. Now, what a `functor` or a `monad` can do is to lift that string into what's called an `elevated world` where that string suddenly has more functions available that can be quite useful. Besides **power** that the `elevated world` brings, it also brings **clarity** and **safety** to your codebase.

We are not going to explain these rules one by one here. Instead, we will see them and many other functions that Funk provides in action where it will be obvious what benefit they bring.

## Pattern-matching

Let's start with a `Match` function.

`Match` is a pattern-matching function that provides a nice way of handling the possible absence of data. So let's say we have a function that might return a `Customer` and we want to get its middle name and in case there is no object, return a default placeholder.

```c#
var customer = customers.Get(id); // returns Maybe<Customer>
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
    c => c.MiddleName, // c is a Customer
    _ => new CustomException("Customer not found.")
); // string
```

You can also use `Action` delegates (void, no return value) instead of `Func` delegates.

```c#
customer.Match(
    _ => Console.WriteLine("Customer not found"),
    c => Console.WriteLine(c.MiddleName)
);
```

Now, imagine a case where some customers have empty (`null`) middle names. Operating even after the `Match` function on the result would cause an exception. This is because we evaluated the `Customer` object and not its inner properties.

We can fix this by using the `Map` function.

## Functor

`Map` is useful when your functions return **plain values** and you want to transform them while staying in the world of `Maybe`. If the `Maybe` is empty, `Map` will not execute the provided function and will return an empty `Maybe`. Otherwise, it will unwrap the underlying value, apply the function, and wrap the result back into `Maybe` — because the result itself can be empty too.

```c#
var middleName = customer.Map(c => c.MiddleName); // Maybe<string>
```

With `Map`, we completely solve the issue with the possible absence of data. We can chain multiple transformations safely — if any step encounters an absent value, the rest of the pipeline is skipped.

```c#
// Chain transformations on data that may be absent at any point
var bio = config.Map(c => c.UserProfile)          // Maybe<UserProfile>
    .Map(p => p.Bio)                               // Maybe<string>
    .Map(b => b.Length > 100 ? b[..100] + "…" : b); // Maybe<string>
```

For a detailed explanation of the `functor` concept, keep reading.

Even though the `Map` function is powerful, we still need another function that can help us when working with nested `Maybe` objects. Imagine that, instead of returning a plain value, our function returns another `Maybe` — for example, looking up an optional setting that may itself be absent.

```c#
var theme = config.Map(c => c.UserProfile.AsMaybe()); // Maybe<Maybe<UserProfile>>
```

We end up with a nested `Maybe` that is tricky to unwrap. What we need is to somehow **flatten** (in FP, we call this **bind**) the result.

We can fix this by using the `FlatMap` function.

## Monad

`FlatMap` is useful when your functions return **elevated values** (`Maybe<T>`). It flattens the result so you don't end up with `Maybe<Maybe<T>>`.

```c#
// Each lookup returns Maybe<T> — data may be absent at each step
Maybe<UserProfile> GetProfile(Guid userId) => profiles.Get(userId);
Maybe<Preferences> GetPreferences(Guid profileId) => preferences.Get(profileId);
Maybe<Theme> GetTheme(string themeId) => themes.Get(themeId);

// Chain them with FlatMap — each step receives the previous result
var theme = GetProfile(userId)
    .FlatMap(p => GetPreferences(p.Id))
    .FlatMap(pref => GetTheme(pref.ThemeId)); // Maybe<Theme>
```

Each `FlatMap` in the chain receives the non-empty result of the previous operation. If any step returns an empty `Maybe`, the entire chain short-circuits and the result is empty. No nesting, no unwrapping — just a flat pipeline.

`FlatMap` is a function that makes the `Maybe` type a `monad` (along with the lifting function that was described earlier). The `Map` function uses the `FlatMap` function internally with the additional wrapping of the result. This is why a `monad` is more powerful than a `functor` as you can implement the `Map` function using the `FlatMap` function but not vice versa.

`Async` versions of `Map` and `FlatMap` are available as well (`MapAsync` and `FlatMapAsync`). `Match`, on the other hand, does not require an `async` version as you can simply return a `Task<R>` as a result of an operation.

```c#
var theme = await GetProfileAsync(userId)
    .FlatMapAsync(p => GetPreferencesAsync(p.Id))
    .FlatMapAsync(pref => GetThemeAsync(pref.ThemeId)); // Task<Maybe<Theme>>
```

`Async` versions also support transformations directly on `Task<T>` as long as the `T` is a `Maybe` object.

## Other useful functions

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

Sometimes, we just want to unwrap the value without using any of the above-mentioned functions. We can use the `GetOr` function and its `async` versions for that.

```c#
var middleName = customer.Map(c => c.MiddleName).GetOr(_ => ""); // string
```

`GetOr` tries to retrieve the underlying value. In case it is empty, it executes the function provided, and returns its result.

Properties `IsEmpty` and `NotEmpty` tell you whether the specified `Maybe` object is empty or not.

*There are also `GetOrDefault` and `UnsafeGet` functions but they should be used with caution!*

Funk provides many other helpful functions for working with the `Maybe` type and some of them will be mentioned later on.

## Applicative (applicative functor)

As mentioned, the `Maybe` type is also an `applicative`. `Applicatives` are less powerful than a `monad`, but more powerful than a `functor`. As we saw in the previous examples with `Map` and `FlatMap` functions, the functions provided as arguments are coming from the so-called `normal` (`regular`) world. With `applicatives`, even the function provided as an argument belongs in the world of elevated values.

To understand `applicatives`, it is best to understand the concept of the [partial application](/Funk/partial-application) first.

Funk provides the `Apply` function for the `Maybe` type that behaves similarly to the `Apply` function from the `partial application` but it operates in the `elevated world`. If you understand the benefit that the `partial application` brings, you can easily see the benefit of the `Apply` function for the `Maybe` type.

We can have a function that concatenates two strings if both of them are not `null` or empty.

```c#
public static Func<string, string, string> FullName => (name, surname) => $"{name} {surname}";
```

Now, we can apply arguments one by one, and to check whether the string is `null`/empty or not, we can use the `AsNotEmptyString` extension method.

```c#
var name = customer.FlatMap(c => c.Name.AsNotEmptyString());
var applied = FullName.AsMaybe().Apply(name); // Maybe<Func<string, string>>
```

As opposed to the `partial application`, here we have to lift the function to the `Maybe` value and then use the `Apply` function.

```c#
var surname = customer.FlatMap(c => c.Surname.AsNotEmptyString());
var fullName = applied.Apply(surname); // Maybe<string>
```

In this scenario, when we apply the second argument, we will execute the pipeline created and maybe get the result back. It means that, if all the previously applied arguments together with the function are not empty, we will get back the full name of the customer.

The beauty of this approach is that we didn't have to implement any logic regarding `null`/empty checks inside the `Function` as it is done for us through the `Apply` function. We also didn't have to change the signature of the function to accept `Maybe` type objects.

## LINQ compatibility

`LINQ` stands for Language Integrated Query and if you haven't noticed, it is the functional programming library as well. However, it is primarily intended for working with sequences that implement `IEnumerable`. Actually, `IEnumerable` is a `monad`. As you saw from the previous examples, lifting and flattening (`FlatMap`) functions make a certain type a `monad`. `IEnumerable` provides lifting functions through its specific implementations (e.g. `new List<T>()`) and its flattening functions are maybe better known to you as `SelectMany` functions. From this, you can probably deduce that `IEnumerable` is also a `functor` as it provides mapping functions as well. They are maybe better known to you as `Select` functions.

Developers usually tend to work with sequences functionally (relying on expressions rather than statements), but with other types, it is not the case. Suddenly, the code is full of `loops` and `if-else` statements. It breaks the fluency and makes the code unreadable.

`Maybe` type has some of the `LINQ` functions implemented. Not all of them are implemented as it simply doesn't make much sense. So instead of using the `Map` and `FlatMap` functions, you can use the corresponding `Select` and `SelectMany` functions. The `Where` function is also implemented which returns a non-empty `Maybe` object only if the item is not empty and if the `predicate` criteria are satisfied.

The purpose of these functions is to be able to write using the `query syntax` instead of the `fluent API` as sometimes it makes the code more readable.

The following example is retrieving a list of parent accounts for the list of related customers (`IEnumerable<Maybe<Customer>>`). So, we need to first get the account and then get the parent account. With the `query syntax`, this expression is readable and pretty clear.


```c#
var parentAccount = relatedCustomers.Select(customer =>
    from c in customer
    from a in accounts.Get(c.AccountId)
    select accounts.GetParentAccount(a.ParentAccountId)
);
```

The same expression can be expressed using the `fluent API` but it becomes quite hard to understand what is actually going on.

```c#
var parentAccount = relatedCustomers.Select(customer => customer.SelectMany(c => 
    accounts.Get(c.AccountId), (c, a) => accounts.GetParentAccount(a.ParentAccountId))
);
```

The `Where` function behaves similarly as the one provided for `IEnumerable` types. The following example returns the account if the balance is more than 100. It also checks whether the customer and account are not empty and only if all these conditions are met, it returns the account.

```c#
var account = from c in customers.Get(id)
              from a in accounts.Get(c.AccountId)
              where a.Balance > 100
              select a;
```

As we see, the `query syntax` makes this expression quite readable. With the `fluent API`, we can accomplish the same thing, but it makes our code quite messy.

```c#
var account = customers.Get(id).SelectMany(
    c => accounts.Get(c.AccountId),
    (_, a) => a
).Where(a => a.Balance > 100);
```

After all, the `query syntax` is a **syntactic sugar** and it uses the `fluent API` methods underneath, so why not use it in situations when it makes your code more readable :)