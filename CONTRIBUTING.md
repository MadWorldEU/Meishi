# Contributing

Thanks for your interest in contributing to Byakko! Here are some guidelines to get started.

## How to Contribute

1. Fork the repository
2. Create a branch from `main` for your changes (see [Branch Naming](#branch-naming) below)
3. Make your changes and ensure the project builds without errors
4. Submit a pull request using the provided template

## Branch Naming

Every branch must be connected to a GitHub issue created from one of the provided issue templates. Use the issue number and a short kebab-case description:

| Type        | Pattern                                      | When to use                                                                 |
|-------------|----------------------------------------------|-----------------------------------------------------------------------------|
| Feature     | `feature/<issue-number>-<description>`       | New functionality requested via a feature issue                             |
| Bug         | `bugfix/<issue-number>-<description>`        | A defect reported via a bug issue                                           |
| Maintenance | `maintenance/<issue-number>-<description>`   | Refactoring, dependency updates, or chores tracked via a maintenance issue  |

**Examples:**
```
feature/42-asset-expiry-cleanup
bugfix/57-ovhcloud-versioned-delete
maintenance/63-update-dotnet-10
```

Branch names must be lowercase with hyphens. Never commit directly to `main`.

## Reporting Issues

Use the GitHub issue templates to report bugs or request features. Please provide as much detail as possible.

## Code Guidelines

- Follow the existing code style and project structure
- Ensure `dotnet build MadWorldEU.Meishi.slnx` passes with no warnings or errors
- Keep changes focused and avoid unrelated modifications

## Pull Requests

- Keep pull requests small and focused on a single change
- Fill in the pull request template
- Link any related issues

## License

By contributing, you agree that your contributions will be licensed under the MIT license.