---
layout: page
title: Unit
parent: Types 
permalink: /types/unit/
nav_order: 1
---

# Unit -> Empty value

Representing an empty value in OOP languages like C# has led to many unexpected behaviors and runtime errors. In C#, value type variables can't be empty or more accurately saying, the default value of value type variables is always some value. However, with reference types, it is different as the default value of reference type variable is nothing or more accurately it points to nothing.

`Unit` exactly addresses this issue by providing a proper representation of an empty value. It is a value type and therefore can't be `null`. It actually can be only in one form and its only value is `Unit` (set with one value -> itself). Other types provided by Funk use `Unit` extensively and especially the `Maybe` type that represents a possible absence of data, so understanding what `Unit` represents is crucial to working with this library.

The following method is a simple function that formats and prints a message to the console.

```c#
public void Write(string message)
{
    var formatted = /* message ... */
    Console.WriteLine(formatted);

    // we can't do something like
    // return new Void();
    // as it will produce a compile-time error
}
```

One problem with `void` or non-returning functions is that they break the fluency and cannot be used in expressions or in combinations with other functions. They force you to write imperative code (do this and do that, instead of how about this and how about that). We will see later how the declarative style of writing code can make your life easier by allowing you to think about edge cases without the need for writing defensive code.

What we should write instead is a function that returns `Unit`.

```c#
public Unit Write(string message)
{
    var formatted = /* message ... */
    Console.WriteLine(formatted);
    return Unit.Value;
}
```

`Prelude` provides a nice way of declaring `Unit` by simply calling `empty` property. Then, the return statement from the previous function can be shortened.

```c#
return empty;
```

Not only does it simplify the declaration but it also provides a meaningful name and makes it feel like it's directly available in the language. However, for this to work, we need to import `Prelude` as a static reference.

```c#
using static Funk.Prelude;
```

We will see later other declaration shortcuts available in `Prelude` as well.

Two `Unit` objects are always equal and calling the `ToString()` function on `Unit` produces an `"empty"` value which is especially helpful during debugging.