---
layout: page
title: Types
permalink: /types/
nav_order: 3
has_children: true
---

# Types


Funk introduces various types for addressing common problems and scenarios that developers face daily. These types include:

`Unit` represents an empty value and a replacement for `Void` type which cannot be used directly in C#.

`Maybe` that represents a possible absence of data.

`Record` which provides the alternative for `ValueTuple` as it makes its inner values immutable.

`OneOf` which represents a discriminated (tagged) union that can be one of more possible values at a time.

`Exc` that represents a possible failure.

`Pattern`, `AsyncPattern`, `TypePattern`, `AsyncTypePattern` that represent lazy pattern matching evaluations.

`Data` and `Builder` that provide a fluent way of building immutable objects.