---
paths:
  - "**/*.cs"
---

# C# code guidelines

## XML documentation

- Every `public` method has a `/// <summary>` that says what the method does, in one or two sentences.
- Describe the behaviour a caller relies on, such as what is returned when nothing is found (for example `<c>null</c>`), not how it is implemented.
- When a method implements an interface method that already has a summary, use `/// <inheritdoc />` instead of repeating it.
- Generated code, such as EF Core migrations, is excluded.
