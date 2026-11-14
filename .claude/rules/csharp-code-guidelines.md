---
paths:
  - "**/*.cs"
---

# C# code guidelines

## Classes

- Every class that is not inherited from is `sealed`.
- Static and abstract classes are excluded, as are classes that a framework requires to be inheritable.

## Access modifiers

- A class or method that is not used from another project is `internal` instead of `public`.
- Types and members that a framework requires to be `public` are excluded.

## XML documentation

- Every `public` method has a `/// <summary>` that says what the method does, in one or two sentences.
- Describe the behaviour a caller relies on, such as what is returned when nothing is found (for example `<c>null</c>`), not how it is implemented.
- When a method implements an interface method that already has a summary, use `/// <inheritdoc />` instead of repeating it.
- Generated code, such as EF Core migrations, is excluded.
