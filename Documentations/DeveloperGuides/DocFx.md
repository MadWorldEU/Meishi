# DocFx
[DocFx](https://dotnet.github.io/docfx/) is a static site generator for .NET projects. It turns Markdown files and the API reference from the source code into a single documentation website.
In Meishi it builds the documentation in the `Documentations` folder. This guide explains how to set up DocFx and run the documentation site on your own machine.

## Prerequisites
- nvm
- Node.js

## Getting Started

1. Install docfx as a global tool:

```bash
dotnet tool install -g docfx
```
   
2. Run docfx to generate the documentation:

```bash
docfx docfx.json --serve
```

3. Open your browser and navigate to http://localhost:8080 to view the generated documentation.
   