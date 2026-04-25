# ForeverTools.InvoiceParser

Parse invoices and extract structured data from PDF or image files using the [AI/ML API](https://aimlapi.com?via=forevertools).

## Installation

```
dotnet add package ForeverTools.InvoiceParser
```

## Quick Start

```csharp
using ForeverTools.InvoiceParser;

var client = new InvoiceParserClient("your-api-key");

// Parse from file path
var result = await client.ParseAsync("/path/to/invoice.pdf");

Console.WriteLine($"Vendor:  {result.Vendor}");
Console.WriteLine($"Total:   {result.Total} {result.Currency}");

foreach (var item in result.LineItems)
    Console.WriteLine($"  {item.Description}  x{item.Quantity}  @ {item.UnitPrice}  = {item.Total}");
```

## API Key

Get your API key at [https://aimlapi.com?via=forevertools](https://aimlapi.com?via=forevertools).

## Methods

| Method | Description |
|---|---|
| `ParseAsync(filePath)` | Parse invoice from a local file |
| `ParseFromUrlAsync(url)` | Parse invoice from a public URL |
| `ParseFromStreamAsync(stream, fileName)` | Parse invoice from a stream |

## Dependency Injection

```csharp
builder.Services.AddInvoiceParser("your-api-key");

// or with full options:
builder.Services.AddInvoiceParser(opts =>
{
    opts.ApiKey = "your-api-key";
    opts.Timeout = TimeSpan.FromSeconds(60);
});
```

## Returned Fields

- `InvoiceNumber`, `Date`, `DueDate`
- `Vendor`, `Customer`
- `LineItems` — list of `{Description, Quantity, UnitPrice, Total}`
- `Subtotal`, `Tax`, `Total`, `Currency`

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
| **ForeverTools.EmailAI** | AI email composition, reply drafting, summarization, and classification | *Coming soon* |
| **ForeverTools.Sentiment** | AI sentiment analysis with emotion detection | *Coming soon* |
| **ForeverTools.TTS** | Text-to-Speech synthesis with 30+ voices | *Coming soon* |

## License

MIT License - see LICENSE file for details.

## Links

- [GitHub Organization](https://github.com/ForeverTools)
- [NuGet Profile](https://www.nuget.org/profiles/ForeverTools)

