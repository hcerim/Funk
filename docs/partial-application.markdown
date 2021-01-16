---
layout: page
title: Partial Application
permalink: /partial-application/
---

# Partial application

The concept of `partial application` is not strictly related to FP. In any programming language where functions are `first-class values` (objects in OOP) we can partially apply arguments to them.

With the `partial application`, we can reason about abstraction on a whole new level. Imagine the following function where you would apply arguments one by one once they become available.

```c#
public static Func<IClient, Configuration, Request, Response> Function => (client, configuration, request) =>
{
    client.AddConfiguration(configuration);
    return client.Process(request);
};
```

In the first layer of the construction, you would apply the `IClient` object to the `Function` using the `Apply` function provided by Funk.

```c#
var applied = Function.Apply(new DbClient()); // returns Func<Configuration, Request, Response>
```

In the second layer of the construction, we might have the necessary `Configuration` object.

```c#
var applied = previouslyApplied.Apply(GetConfiguration()); // returns Func<Request, Response>
```

In each application, we get a function of smaller arity and once we reach the `Func<T, R>` function signature, the next application executes the whole pipeline that we previously created. Each argument applied is part of this pipeline but is not visible from the function signature in the next construction layer.

If you would go fully functional, this can even allow you to use `FI` (Function Injection) instead of the more traditional `DI`.

Funk also provides the `partial application` for `Action` type delegates.