# ForeverTools

.NET wrapper libraries for popular third-party APIs. Clean interfaces, dependency injection ready, available on NuGet.

## Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E, Stable Diffusion) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.Postmark** | Transactional email sending with templates and tracking | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation and CAPTCHA bypass | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |

## Coming Soon

- **ForeverTools.Proxy** - Proxy rotation (BrightData, SmartProxy)
- **ForeverTools.SMS** - SMS & messaging (BulkGate, Textmagic)

## Installation

All packages are available on NuGet:

```bash
# AI/ML (400+ AI models)
dotnet add package ForeverTools.AIML

# Captcha solving (reCAPTCHA, hCaptcha, Turnstile)
dotnet add package ForeverTools.Captcha

# Transactional email (Postmark)
dotnet add package ForeverTools.Postmark

# Web scraping (ScraperAPI)
dotnet add package ForeverTools.ScraperAPI
```

## Quick Examples

### AI/ML API
```csharp
using ForeverTools.AIML;

var client = new AimlApiClient("your-api-key");

// Chat with GPT-4, Claude, Llama, etc.
var response = await client.ChatAsync("What is the capital of France?");

// Generate images with DALL-E, Stable Diffusion, Flux
var imageUrl = await client.GenerateImageAsync("A sunset over mountains");

// Get embeddings for RAG/search
var vector = await client.EmbedAsync("Hello world");
```

### Captcha Solving
```csharp
using ForeverTools.Captcha;

// Use your preferred provider
var client = CaptchaClient.For2Captcha("your-api-key");
// or: CaptchaClient.ForCapSolver("your-api-key");
// or: CaptchaClient.ForAntiCaptcha("your-api-key");

// Solve reCAPTCHA v2
var result = await client.SolveReCaptchaV2Async(
    "https://example.com",
    "6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-"
);

if (result.Success)
{
    Console.WriteLine($"Token: {result.Solution}");
}

// Also supports: reCAPTCHA v3, hCaptcha, Turnstile, FunCaptcha, Image
```

### Postmark Email
```csharp
using ForeverTools.Postmark;

var client = new PostmarkClient("your-server-token");

// Send an email
var result = await client.SendEmailAsync(
    to: "recipient@example.com",
    subject: "Hello from Postmark!",
    htmlBody: "<h1>Welcome!</h1><p>This is a test email.</p>",
    from: "sender@yourdomain.com"
);

if (result.Success)
{
    Console.WriteLine($"Sent! Message ID: {result.MessageId}");
}

// Also supports: batch sending, templates, attachments, bounce tracking
```

### Web Scraping
```csharp
using ForeverTools.ScraperAPI;

var client = new ScraperApiClient("your-api-key");

// Simple scrape
var html = await client.ScrapeAsync("https://example.com");

// With JavaScript rendering (for SPAs)
var rendered = await client.ScrapeWithJavaScriptAsync("https://spa-site.com");

// Take a screenshot
var screenshot = await client.TakeScreenshotBytesAsync("https://example.com");

// Also supports: geo-targeting, premium proxies, async jobs, auto-parsing
```

### ASP.NET Core
```csharp
// Program.cs
builder.Services.AddForeverToolsAiml("your-api-key");
builder.Services.AddForeverToolsCaptcha(options =>
{
    options.TwoCaptchaApiKey = "your-2captcha-key";
    options.DefaultProvider = CaptchaProvider.TwoCaptcha;
});
builder.Services.AddForeverToolsPostmark("your-server-token");
builder.Services.AddForeverToolsScraperApi("your-api-key");

// Or from configuration
builder.Services.AddForeverToolsAiml(builder.Configuration);
builder.Services.AddForeverToolsCaptcha(builder.Configuration);
builder.Services.AddForeverToolsPostmark(builder.Configuration);
builder.Services.AddForeverToolsScraperApi(builder.Configuration);
```

## Features

- **Modern .NET** - Targets .NET 8, .NET 6, and .NET Standard 2.0
- **Async/await** - All operations are async
- **Dependency Injection** - Built-in `IServiceCollection` extensions
- **IntelliSense** - Full XML documentation
- **Type-safe** - Model constants, no magic strings

## Links

- [GitHub Organization](https://github.com/ForeverTools)
- [NuGet Profile](https://www.nuget.org/profiles/ForeverTools)

## License

MIT
