# ForeverTools.CodeGen

AI-powered code generation, refactoring, and explanation for .NET — in one async call.

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.CodeGen.svg)](https://www.nuget.org/packages/ForeverTools.CodeGen)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Features

- **Generate** — turn natural language into runnable code in any language
- **Refactor** — clean up existing code with a detailed list of improvements
- **Explain** — step-by-step breakdown of what any code does
- **Batch** — generate multiple snippets in one call
- Supports 400+ AI models via [AI/ML API](https://aimlapi.com?via=forevertools)
- Dependency injection support (`AddCodeGenClient`)
- Fully async, cancellation-token aware

## Installation

```
dotnet add package ForeverTools.CodeGen
```

## Quick start

```csharp
using ForeverTools.CodeGen;

var client = new CodeGenClient(new CodeGenOptions { ApiKey = "YOUR_API_KEY" });

// Generate code
var result = await client.GenerateAsync("read a CSV file and print the first 5 rows", "python");
Console.WriteLine(result.Code);
Console.WriteLine(result.Explanation);

// Refactor code
var refactored = await client.RefactorAsync(myLegacyCode, "csharp");
Console.WriteLine(refactored.Summary);
foreach (var improvement in refactored.Improvements)
    Console.WriteLine($"- {improvement}");

// Explain code
var explanation = await client.ExplainAsync(someCode);
Console.WriteLine(explanation.Summary);

// Batch generation
var batch = await client.GenerateBatchAsync(
    new[] { "fibonacci sequence", "bubble sort", "binary search" },
    language: "javascript");
```

## Dependency injection

```csharp
// Program.cs
builder.Services.AddCodeGenClient("YOUR_API_KEY");

// Or with full configuration
builder.Services.AddCodeGenClient(opts =>
{
    opts.ApiKey = "YOUR_API_KEY";
    opts.Model  = "gpt-4o";      // optional — defaults to gpt-4o-mini
    opts.Timeout = TimeSpan.FromSeconds(90);
});
```

## Environment variable

```csharp
// Reads AIML_API_KEY environment variable
var client = new CodeGenClient(CodeGenOptions.FromEnvironment());
```

## API key

Get your API key at **[aimlapi.com](https://aimlapi.com?via=forevertools)** — 400+ models, pay-as-you-go.

## Related packages

- [ForeverTools.Summarize](https://www.nuget.org/packages/ForeverTools.Summarize) — text summarisation
- [ForeverTools.Sentiment](https://www.nuget.org/packages/ForeverTools.Sentiment) — sentiment analysis
- [ForeverTools.ContentMod](https://www.nuget.org/packages/ForeverTools.ContentMod) — content moderation
- [ForeverTools.STT](https://www.nuget.org/packages/ForeverTools.STT) — speech-to-text transcription
- [ForeverTools.OCR](https://www.nuget.org/packages/ForeverTools.OCR) — optical character recognition

Built with [kiprio.com](https://kiprio.com) developer APIs.

## License

MIT
