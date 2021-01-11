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