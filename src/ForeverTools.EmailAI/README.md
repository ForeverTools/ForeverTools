# ForeverTools.EmailAI

AI-powered email assistant for .NET — compose, reply, summarise, and classify emails in one async call.

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.EmailAI.svg)](https://www.nuget.org/packages/ForeverTools.EmailAI)

Powered by [AI/ML API](https://aimlapi.com?via=forevertools) — 400+ models, one API.

## Quick Start

```csharp
using ForeverTools.EmailAI;
using ForeverTools.EmailAI.Models;

var client = new EmailAIClient(new EmailAIOptions { ApiKey = "your-api-key" });

// Compose a new email
var composed = await client.ComposeAsync(new EmailComposeRequest
{
    Subject = "Project Update",
    Recipients = new List<string> { "alice@example.com" },
    Context = "Q3 milestones are all on track. Budget is under by 5%.",
    Tone = "professional"
});
Console.WriteLine(composed.Body);

// Reply to an email
var reply = await client.ReplyAsync(new EmailReplyRequest
{
    OriginalEmail = "Can you confirm the meeting time?",
    ReplyContext = "Confirm 10am works fine",
    Tone = "friendly"
});
Console.WriteLine(reply.Body);

// Summarise an email
var summary = await client.SummarizeAsync("Long email body here...");
Console.WriteLine(summary.Summary);
Console.WriteLine(string.Join(", ", summary.ActionItems));

// Classify an email
var classification = await client.ClassifyAsync("URGENT: System is down!");
Console.WriteLine($"{classification.Category} / {classification.Priority} / {classification.Sentiment}");
```

## Dependency Injection

```csharp
// Program.cs
builder.Services.AddForeverToolsEmailAI("your-api-key");

// Or with full configuration
builder.Services.AddForeverToolsEmailAI(options =>
{
    options.ApiKey = "your-api-key";
    options.Model = "gpt-4o";
    options.Timeout = TimeSpan.FromSeconds(90);
});
```

## Batch Operations

```csharp
// Compose multiple emails
var batch = await client.BatchComposeAsync(new[]
{
    new EmailComposeRequest { Subject = "Email 1", Context = "...", Tone = "professional" },
    new EmailComposeRequest { Subject = "Email 2", Context = "...", Tone = "friendly" }
});
Console.WriteLine($"Composed {batch.Total} emails");

// Reply to multiple emails
var replies = await client.BatchReplyAsync(new[]
{
    new EmailReplyRequest { OriginalEmail = "...", ReplyContext = "...", Tone = "professional" },
    new EmailReplyRequest { OriginalEmail = "...", ReplyContext = "...", Tone = "formal" }
});
```

## Classification Categories

`ClassifyAsync` returns one of: `urgent`, `normal`, `spam`, `newsletter`, `support`, `sales`, `other`

Priority: `high`, `medium`, `low`

Sentiment: `positive`, `neutral`, `negative`

## Get Your API Key

Get your free API key at [https://aimlapi.com?via=forevertools](https://aimlapi.com?via=forevertools)

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
| **ForeverTools.CodeGen** | AI-powered code generation, refactoring, and explanation | *Coming soon* |
| **ForeverTools.ContentMod** | AI content moderation — detect toxicity, hate speech, adult content | *Coming soon* |
| **ForeverTools.InvoiceParser** | AI-powered invoice and receipt data extraction | *Coming soon* |
| **ForeverTools.Sentiment** | AI sentiment analysis with emotion detection | *Coming soon* |
| **ForeverTools.TTS** | Text-to-Speech synthesis with 30+ voices | *Coming soon* |

## License

MIT License - see LICENSE file for details.

## Links

- [GitHub Organization](https://github.com/ForeverTools)
- [NuGet Profile](https://www.nuget.org/profiles/ForeverTools)

