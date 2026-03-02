<p align="center">
  <img src="https://github.com/hcerim/Funk/blob/master/docs/Files/Funk.png?raw=true" width="300" />
</p>

[![Version](https://img.shields.io/nuget/vpre/Funk.svg)](https://www.nuget.org/packages/Funk)
[![Build & Test](https://github.com/hcerim/Funk/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/hcerim/Funk/actions/workflows/build-and-test.yml)

# Funk — Functional C#

A lightweight functional programming library that brings expressive, composable, and safe abstractions to C#. Less ceremony, more clarity.

### Highlights

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
