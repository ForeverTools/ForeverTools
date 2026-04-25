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

## Other ForeverTools Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E, Stable Diffusion) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.APILayer** | IP geolocation, currency exchange, phone & email validation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.APILayer.svg)](https://www.nuget.org/packages/ForeverTools.APILayer) |
| **ForeverTools.Apify** | Web scraping platform with 1,600+ actors (Amazon, Google, Instagram, Twitter) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Apify.svg)](https://www.nuget.org/packages/ForeverTools.Apify) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.ImageGen** | AI image generation with social media presets (DALL-E, Flux, SD) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ImageGen.svg)](https://www.nuget.org/packages/ForeverTools.ImageGen) |
| **ForeverTools.OCR** | AI-powered OCR using GPT-4 Vision, Claude 3, and Gemini | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.OCR.svg)](https://www.nuget.org/packages/ForeverTools.OCR) |
| **ForeverTools.Postmark** | Transactional email sending with templates and tracking | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |
| **ForeverTools.Proxy** | Premium proxy rotation with BrightData (Residential, ISP, Mobile) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Proxy.svg)](https://www.nuget.org/packages/ForeverTools.Proxy) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation and CAPTCHA bypass | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |
| **ForeverTools.STT** | Speech-to-Text using Whisper (transcription, subtitles, language detection) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.STT.svg)](https://www.nuget.org/packages/ForeverTools.STT) |
| **ForeverTools.Summarize** | AI-powered text summarization (TL;DR, bullet points, executive summaries) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Summarize.svg)](https://www.nuget.org/packages/ForeverTools.Summarize) |
| **ForeverTools.Translate** | AI-powered translation with 100+ languages (GPT-4, Claude) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Translate.svg)](https://www.nuget.org/packages/ForeverTools.Translate) |
| **ForeverTools.ContentMod** | AI content moderation — detect toxicity, hate speech, adult content | *Coming soon* |
| **ForeverTools.EmailAI** | AI email composition, reply drafting, summarization, and classification | *Coming soon* |
| **ForeverTools.InvoiceParser** | AI-powered invoice and receipt data extraction | *Coming soon* |
| **ForeverTools.Sentiment** | AI sentiment analysis with emotion detection | *Coming soon* |
| **ForeverTools.TTS** | Text-to-Speech synthesis with 30+ voices | *Coming soon* |

## License

MIT License - see LICENSE file for details.

## Links

- [GitHub Organization](https://github.com/ForeverTools)
- [NuGet Profile](https://www.nuget.org/profiles/ForeverTools)
