# ForeverTools.Sentiment

Sentiment analysis and emotion scoring for .NET — powered by [AI/ML API](https://aimlapi.com?via=forevertools).

Detect **positive / negative / neutral / mixed** sentiment with confidence scores and a full emotion breakdown (joy, anger, sadness, fear, surprise, disgust) in a single async call.

> **Get your API key:** [https://aimlapi.com?via=forevertools](https://aimlapi.com?via=forevertools)
> Sign up via this link and earn a **30% recurring commission** if you refer others.

---

## Quick start

### 1. Basic sentiment analysis

```csharp
using ForeverTools.Sentiment;

var client = new SentimentClient("your-api-key");
var result = await client.AnalyzeAsync("I absolutely love this product!");

Console.WriteLine(result.Label);           // Positive
Console.WriteLine(result.Confidence);      // 0.97
Console.WriteLine(result.Emotions.Dominant); // Joy
Console.WriteLine(result.Summary);         // Enthusiastic and very satisfied tone
```

### 2. Batch analysis

```csharp
var texts = new[]
{
    "Great service, really happy!",
    "Terrible experience, never again.",
    "It was okay, nothing special."
};

var results = await client.AnalyzeBatchAsync(texts);
foreach (var r in results)
    Console.WriteLine($"{r.Label} ({r.Confidence:P0}) — {r.Summary}");
```

### 3. Dependency injection (ASP.NET Core)

```csharp
// Program.cs
builder.Services.AddSentimentClient("your-api-key");

// Or with full options:
builder.Services.AddSentimentClient(options =>
{
    options.ApiKey = "your-api-key";
    options.Model  = "gpt-4o-mini";   // fast and cheap
    options.Timeout = TimeSpan.FromSeconds(60);
});

// In your controller / service:
public class ReviewController(SentimentClient sentiment)
{
    public async Task<IActionResult> Analyse(string text)
    {
        var result = await sentiment.AnalyzeAsync(text);
        return Ok(result);
    }
}
```

---

## Response fields

| Field | Type | Description |
|---|---|---|
| `Label` | `SentimentLabel` | Positive, Negative, Neutral, or Mixed |
| `Confidence` | `double` (0–1) | How confident the model is in the label |
| `Emotions.Joy` | `double` (0–1) | Joy intensity |
| `Emotions.Anger` | `double` (0–1) | Anger intensity |
| `Emotions.Sadness` | `double` (0–1) | Sadness intensity |
| `Emotions.Fear` | `double` (0–1) | Fear intensity |
| `Emotions.Surprise` | `double` (0–1) | Surprise intensity |
| `Emotions.Disgust` | `double` (0–1) | Disgust intensity |
| `Emotions.Dominant` | `string` | Name of the highest-scoring emotion |
| `Summary` | `string` | Short human-readable tone description |
| `InputText` | `string` | The original text analysed |
| `ProcessingMs` | `long` | Round-trip time in milliseconds |

---

## Powered by AI/ML API

AI/ML API provides access to 400+ AI models through a single OpenAI-compatible endpoint.
Get your key at [https://aimlapi.com?via=forevertools](https://aimlapi.com?via=forevertools).
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
| **ForeverTools.InvoiceParser** | AI-powered invoice and receipt data extraction | *Coming soon* |
| **ForeverTools.TTS** | Text-to-Speech synthesis with 30+ voices | *Coming soon* |

## License

MIT License - see LICENSE file for details.

## Links

- [GitHub Organization](https://github.com/ForeverTools)
- [NuGet Profile](https://www.nuget.org/profiles/ForeverTools)
