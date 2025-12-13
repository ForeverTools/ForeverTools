# ForeverTools

.NET wrapper libraries for popular third-party APIs. Clean interfaces, dependency injection ready, available on NuGet.

## Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E, Stable Diffusion) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.APILayer** | IP geolocation, currency exchange, phone & email validation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.APILayer.svg)](https://www.nuget.org/packages/ForeverTools.APILayer) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.ImageGen** | AI image generation with social media presets (DALL-E, Flux, SD) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ImageGen.svg)](https://www.nuget.org/packages/ForeverTools.ImageGen) |
| **ForeverTools.OCR** | AI-powered OCR using GPT-4 Vision, Claude 3, and Gemini | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.OCR.svg)](https://www.nuget.org/packages/ForeverTools.OCR) |
| **ForeverTools.Postmark** | Transactional email sending with templates and tracking | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |
| **ForeverTools.Proxy** | Premium proxy rotation with BrightData (Residential, ISP, Mobile) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Proxy.svg)](https://www.nuget.org/packages/ForeverTools.Proxy) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation and CAPTCHA bypass | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |
| **ForeverTools.Translate** | AI-powered translation with 100+ languages (GPT-4, Claude) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Translate.svg)](https://www.nuget.org/packages/ForeverTools.Translate) |

## Coming Soon

- **ForeverTools.SMS** - SMS & messaging (BulkGate, Textmagic)

## Premium Proxy Providers

Need high-quality proxies for scraping or automation? Check out [BrightData](https://get.brightdata.com/ForeverToolsResidentialProxies) - the industry leader:

| Proxy Type | Best For | Link |
|------------|----------|------|
| **Residential Proxies** | General scraping, geo-targeting | [Get Started](https://get.brightdata.com/ForeverToolsResidentialProxies) |
| **ISP Proxies** | High-speed, stable connections | [Get Started](https://get.brightdata.com/ForeverToolsISP) |
| **Social Media Proxies** | Instagram, Facebook, TikTok automation | [Get Started](https://get.brightdata.com/ForeverToolsSocialProxies) |
| **SERP API** | Search engine scraping | [Get Started](https://get.brightdata.com/ForeverToolsSerp) |

## Installation

All packages are available on NuGet:

```bash
# AI/ML (400+ AI models)
dotnet add package ForeverTools.AIML

# API utilities (IP geolocation, currency, phone, email validation)
dotnet add package ForeverTools.APILayer

# Captcha solving (reCAPTCHA, hCaptcha, Turnstile)
dotnet add package ForeverTools.Captcha

# OCR (AI-powered text extraction)
dotnet add package ForeverTools.OCR

# Transactional email (Postmark)
dotnet add package ForeverTools.Postmark

# Web scraping (ScraperAPI)
dotnet add package ForeverTools.ScraperAPI

# AI Image generation (DALL-E, Stable Diffusion, Flux)
dotnet add package ForeverTools.ImageGen

# Premium proxy rotation (BrightData)
dotnet add package ForeverTools.Proxy

# AI Translation (100+ languages)
dotnet add package ForeverTools.Translate
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

### APILayer (IP, Currency, Phone, Email)
```csharp
using ForeverTools.APILayer;

var client = new ApiLayerClient("your-api-key");

// IP Geolocation
var geo = await client.GetIpGeolocationAsync("8.8.8.8");
Console.WriteLine($"Country: {geo.Data.CountryName}, City: {geo.Data.City}");

// Currency conversion
var convert = await client.ConvertCurrencyAsync("USD", "EUR", 100);
Console.WriteLine($"100 USD = {convert.Data.Result} EUR");

// Phone validation
var phone = await client.ValidatePhoneAsync("+14155552671");
Console.WriteLine($"Valid: {phone.Data.Valid}, Carrier: {phone.Data.Carrier}");

// Email validation
var email = await client.ValidateEmailAsync("test@example.com");
Console.WriteLine($"Deliverable: {email.Data.IsDeliverable}");
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

### OCR (AI-Powered Text Extraction)
```csharp
using ForeverTools.OCR;

var client = new OcrClient("your-aiml-api-key");

// Extract text from an image file
var result = await client.ExtractTextFromFileAsync("document.jpg");
Console.WriteLine(result.Text);

// Extract structured data (JSON output)
var structured = await client.ExtractStructuredAsync(imageBytes, "image/png");

// Extract tables from images
var tables = await client.ExtractTablesAsync(imageBytes, "image/png");
foreach (var table in tables.Tables)
{
    Console.WriteLine(table.ToCsv());
}

// Also supports: form fields, receipts, URLs, custom prompts
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

### AI Image Generation
```csharp
using ForeverTools.ImageGen;

var client = new ImageGenClient("your-aiml-api-key");

// Generate an image
var image = await client.GenerateAsync("A sunset over mountains");
await client.SaveAsync(image, "sunset.png");

// Social media presets - perfectly sized for each platform
var instagram = await client.GenerateForInstagramAsync("Coffee shop aesthetic");
var youtube = await client.GenerateForYouTubeAsync("SHOCKING Discovery!", ImageStyle.Cinematic);
var linkedin = await client.GenerateForLinkedInAsync("Professional headshot", ImageStyle.Corporate);

// Also supports: Twitter, Facebook, Pinterest, TikTok, Blog headers, Open Graph
```

### Premium Proxies (BrightData)
```csharp
using ForeverTools.Proxy;

var client = new BrightDataClient("customer-id", "zone", "password");

// Create an HttpClient with proxy
using var httpClient = client.CreateHttpClient();
var html = await httpClient.GetStringAsync("https://example.com");

// Geo-targeting - browse from specific countries
using var usClient = client.CreateHttpClientForCountry("us");
using var ukClient = client.CreateHttpClientForCountry("gb");

// Sticky sessions - same IP across requests
using var session = client.CreateSession();
using var sessionClient = session.CreateHttpClient();

// Also supports: Residential, Datacenter, ISP, Mobile proxies
```

### AI Translation
```csharp
using ForeverTools.Translate;

var client = new TranslationClient("your-aiml-api-key");

// Translate to Spanish (auto-detect source)
var spanish = await client.TranslateAsync("Hello, how are you?", "es");
// "Hola, ¿cómo estás?"

// Translate to English
var english = await client.TranslateToEnglishAsync("Bonjour le monde");
// "Hello world"

// Detect language
var detected = await client.DetectLanguageAsync("Guten Tag");
Console.WriteLine($"Language: {detected.LanguageName}"); // German

// Batch translation
var texts = new[] { "Hello", "Goodbye", "Thank you" };
var results = await client.TranslateBatchAsync(texts, "es");

// Also supports: custom glossaries, translation styles, context-aware translation
```

### ASP.NET Core
```csharp
// Program.cs
builder.Services.AddForeverToolsAiml("your-api-key");
builder.Services.AddForeverToolsApiLayer("your-api-key");
builder.Services.AddForeverToolsCaptcha(options =>
{
    options.TwoCaptchaApiKey = "your-2captcha-key";
    options.DefaultProvider = CaptchaProvider.TwoCaptcha;
});
builder.Services.AddForeverToolsOcr("your-aiml-api-key");
builder.Services.AddForeverToolsPostmark("your-server-token");
builder.Services.AddForeverToolsScraperApi("your-api-key");
builder.Services.AddForeverToolsImageGen("your-aiml-api-key");
builder.Services.AddForeverToolsProxy("customer-id", "zone", "password");
builder.Services.AddForeverToolsTranslation("your-aiml-api-key");

// Or from configuration
builder.Services.AddForeverToolsAiml(builder.Configuration);
builder.Services.AddForeverToolsApiLayer(builder.Configuration);
builder.Services.AddForeverToolsCaptcha(builder.Configuration);
builder.Services.AddForeverToolsOcr(builder.Configuration);
builder.Services.AddForeverToolsPostmark(builder.Configuration);
builder.Services.AddForeverToolsScraperApi(builder.Configuration);
builder.Services.AddForeverToolsImageGen(builder.Configuration);
builder.Services.AddForeverToolsProxy(builder.Configuration);
builder.Services.AddForeverToolsTranslation(builder.Configuration);
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
