# ForeverTools.ScraperAPI

Lightweight ScraperAPI client for .NET. Scrape any website without getting blocked - automatic proxy rotation, CAPTCHA solving, and JavaScript rendering.

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI)

## Features

- **Simple Scraping** - One line to scrape any URL
- **JavaScript Rendering** - Scrape SPAs and dynamic content
- **Screenshots** - Capture full page screenshots
- **Geo-Targeting** - Scrape from specific countries
- **Premium Proxies** - Residential and mobile IPs
- **Async Jobs** - Background scraping for large batches
- **Auto-Parse** - Get structured JSON from popular sites
- **ASP.NET Core Ready** - Built-in dependency injection
- **Multi-Target** - .NET 8, .NET 6, .NET Standard 2.0

## Quick Start

### Install

```bash
dotnet add package ForeverTools.ScraperAPI
```

### Get Your API Key

Sign up at [ScraperAPI](https://www.scraperapi.com/signup?fp_ref=chris88) to get your API key with 5,000 free credits.

### Basic Usage

```csharp
using ForeverTools.ScraperAPI;

var client = new ScraperApiClient("your-api-key");

// Simple scrape
var html = await client.ScrapeAsync("https://example.com");

// With JavaScript rendering (for SPAs)
var rendered = await client.ScrapeWithJavaScriptAsync("https://spa-example.com");

// From a specific country
var usContent = await client.ScrapeFromCountryAsync("https://example.com", "us");
```

## Advanced Usage

### Full Request Configuration

```csharp
var request = new ScrapeRequest
{
    Url = "https://example.com",
    RenderJavaScript = true,
    CountryCode = "us",
    Premium = true,
    DeviceType = DeviceTypes.Mobile,
    AutoParse = true
};

var response = await client.ScrapeWithResponseAsync(request);

if (response.Success)
{
    Console.WriteLine($"Status: {response.StatusCode}");
    Console.WriteLine(response.Content);
}
```

### Screenshots

```csharp
// Get screenshot as base64
var base64 = await client.TakeScreenshotAsync("https://example.com");

// Get screenshot as bytes (for saving to file)
var bytes = await client.TakeScreenshotBytesAsync("https://example.com");
File.WriteAllBytes("screenshot.png", bytes);
```

### Async Jobs (Background Scraping)

```csharp
// Submit job and wait for completion
var response = await client.ScrapeAsyncAndWaitAsync(new ScrapeRequest
{
    Url = "https://example.com",
    RenderJavaScript = true
});

// Or manage jobs manually
var job = await client.SubmitAsyncJobAsync(request);
Console.WriteLine($"Job ID: {job.Id}");

// Check status later
var status = await client.GetAsyncJobStatusAsync(job.Id);
if (status.IsFinished)
{
    Console.WriteLine(status.Response.Body);
}
```

### Check Account Credits

```csharp
var account = await client.GetAccountInfoAsync();
Console.WriteLine($"Credits remaining: {account.RemainingCredits}");
Console.WriteLine($"Concurrent limit: {account.ConcurrencyLimit}");
```

## ASP.NET Core Integration

```csharp
// Program.cs
builder.Services.AddForeverToolsScraperApi("your-api-key");

// Or with full configuration
builder.Services.AddForeverToolsScraperApi(options =>
{
    options.ApiKey = "your-api-key";
    options.DefaultRenderJavaScript = true;
    options.DefaultCountryCode = "us";
    options.TimeoutSeconds = 90;
});

// Or from appsettings.json
builder.Services.AddForeverToolsScraperApi(builder.Configuration);
```

```json
// appsettings.json
{
  "ScraperAPI": {
    "ApiKey": "your-api-key",
    "DefaultRenderJavaScript": true,
    "DefaultCountryCode": "us"
  }
}
```

```csharp
// Inject and use
public class PriceScraperService
{
    private readonly ScraperApiClient _scraper;

    public PriceScraperService(ScraperApiClient scraper)
    {
        _scraper = scraper;
    }

    public async Task<string> GetProductPageAsync(string url)
    {
        return await _scraper.ScrapeWithJavaScriptAsync(url);
    }
}
```

## Environment Variables

```csharp
// Uses SCRAPERAPI_KEY by default
var client = ScraperApiClient.FromEnvironment();

// Or specify custom variable name
var client = ScraperApiClient.FromEnvironment("MY_SCRAPER_KEY");
```

## Request Options

| Option | Description | Credits |
|--------|-------------|---------|
| Basic scrape | Default HTML scraping | 1 |
| `RenderJavaScript` | Execute JavaScript | +10 |
| `Screenshot` | Capture page screenshot | +10 |
| `Premium` | Residential/mobile proxies | +10 |
| `UltraPremium` | Advanced bypass | +30 |
| `CountryCode` | Geo-targeting | +0 |
| `SessionNumber` | Sticky sessions | +0 |
| `AutoParse` | Structured JSON output | +0 |

## Country Codes

Common codes: `us`, `uk`, `de`, `fr`, `es`, `it`, `br`, `ca`, `au`, `jp`, `in`

[Full list of supported countries](https://docs.scraperapi.com/)

## Why ScraperAPI?

[ScraperAPI](https://www.scraperapi.com/signup?fp_ref=chris88) handles the hard parts of web scraping:

- **40M+ proxies** - Automatic rotation, never get blocked
- **CAPTCHA solving** - Built-in, no extra setup
- **JavaScript rendering** - Scrape SPAs and dynamic sites
- **99.9% uptime** - Reliable infrastructure
- **Simple pricing** - Pay per successful request

## Need Just Proxies?

If you need standalone proxy access without the scraping features, check out [BrightData](https://get.brightdata.com/ForeverToolsResidentialProxies) - the industry leader in proxy services:

| Proxy Type | Best For | Link |
|------------|----------|------|
| **Residential Proxies** | General scraping, geo-targeting | [Get Started](https://get.brightdata.com/ForeverToolsResidentialProxies) |
| **ISP Proxies** | High-speed, stable connections | [Get Started](https://get.brightdata.com/ForeverToolsISP) |
| **Social Media Proxies** | Instagram, Facebook, TikTok automation | [Get Started](https://get.brightdata.com/ForeverToolsSocialProxies) |
| **SERP API** | Search engine scraping | [Get Started](https://get.brightdata.com/ForeverToolsSerp) |

## Other ForeverTools Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.APILayer** | IP geolocation, currency exchange, phone & email validation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.APILayer.svg)](https://www.nuget.org/packages/ForeverTools.APILayer) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.ImageGen** | AI image generation with social media presets (DALL-E, Flux, SD) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ImageGen.svg)](https://www.nuget.org/packages/ForeverTools.ImageGen) |
| **ForeverTools.OCR** | AI-powered OCR using GPT-4 Vision, Claude 3, and Gemini | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.OCR.svg)](https://www.nuget.org/packages/ForeverTools.OCR) |
| **ForeverTools.Postmark** | Transactional email sending with templates and tracking | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |

## Requirements

- .NET 8.0, .NET 6.0, or .NET Standard 2.0 compatible framework
- ScraperAPI account with API key

## License

MIT License - see [LICENSE](LICENSE) for details.
