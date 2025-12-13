# ForeverTools.Captcha

Multi-provider captcha solving for .NET. Supports 2Captcha, CapSolver, and Anti-Captcha with a unified API.

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha)

## Features

- **Multiple Providers** - 2Captcha, CapSolver, Anti-Captcha
- **Unified API** - Same interface for all providers
- **All Captcha Types** - reCAPTCHA v2/v3, hCaptcha, Turnstile, FunCaptcha, Image
- **ASP.NET Core Ready** - Built-in dependency injection
- **Async/Await** - Fully asynchronous
- **Multi-Target** - .NET 8, .NET 6, .NET Standard 2.0

## Quick Start

### Install

```bash
dotnet add package ForeverTools.Captcha
```

### Solve a Captcha

```csharp
using ForeverTools.Captcha;

// Create client for your preferred provider
var client = CaptchaClient.For2Captcha("your-api-key");
// or: var client = CaptchaClient.ForCapSolver("your-api-key");
// or: var client = CaptchaClient.ForAntiCaptcha("your-api-key");

// Solve reCAPTCHA v2
var result = await client.SolveReCaptchaV2Async(
    "https://example.com/page",
    "6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-"
);

if (result.Success)
{
    Console.WriteLine($"Token: {result.Solution}");
}
```

## Supported Captcha Types

### reCAPTCHA v2

```csharp
var result = await client.SolveReCaptchaV2Async(
    websiteUrl: "https://example.com",
    siteKey: "6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-",
    invisible: false
);
```

### reCAPTCHA v3

```csharp
var result = await client.SolveReCaptchaV3Async(
    websiteUrl: "https://example.com",
    siteKey: "6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-",
    action: "login",
    minScore: 0.5
);
```

### hCaptcha

```csharp
var result = await client.SolveHCaptchaAsync(
    websiteUrl: "https://example.com",
    siteKey: "a5f74b19-9e45-40e0-b45d-47ff91b7a6c2"
);
```

### Cloudflare Turnstile

```csharp
var result = await client.SolveTurnstileAsync(
    websiteUrl: "https://example.com",
    siteKey: "0x4AAAAAAABS7vwvV6VFfMcD"
);
```

### Image Captcha

```csharp
// From base64
var result = await client.SolveImageAsync(imageBase64);

// From bytes
var result = await client.SolveImageAsync(imageBytes);
```

### FunCaptcha / Arkose Labs

```csharp
var result = await client.SolveFunCaptchaAsync(
    websiteUrl: "https://example.com",
    publicKey: "A2A14B1D-1AF3-C791-9BBC-EE1C98E84A77"
);
```

## Multi-Provider Setup

Configure multiple providers for failover or comparison:

```csharp
var options = new CaptchaOptions
{
    DefaultProvider = CaptchaProvider.TwoCaptcha,
    TwoCaptchaApiKey = "key1",
    CapSolverApiKey = "key2",
    AntiCaptchaApiKey = "key3"
};

var client = new CaptchaClient(options);

// Use specific provider
var result = await client.SolveReCaptchaV2Async(url, siteKey,
    provider: CaptchaProvider.CapSolver);
```

## ASP.NET Core Integration

```csharp
// Program.cs
builder.Services.AddForeverToolsCaptcha("your-api-key");

// Or with full configuration
builder.Services.AddForeverToolsCaptcha(options =>
{
    options.DefaultProvider = CaptchaProvider.TwoCaptcha;
    options.TwoCaptchaApiKey = "key1";
    options.CapSolverApiKey = "key2";
    options.TimeoutSeconds = 120;
});

// Or from appsettings.json
builder.Services.AddForeverToolsCaptcha(builder.Configuration);
```

```json
// appsettings.json
{
  "Captcha": {
    "DefaultProvider": "TwoCaptcha",
    "TwoCaptchaApiKey": "your-key",
    "TimeoutSeconds": 120
  }
}
```

## Check Balance

```csharp
var balance = await client.GetBalanceAsync();
Console.WriteLine($"Balance: ${balance.Balance}");

// All configured providers
var balances = await client.GetAllBalancesAsync();
```

## Report Incorrect Solution

```csharp
if (!solutionWorked)
{
    await client.ReportIncorrectAsync(result.TaskId, result.Provider);
}
```

## Provider Links

- [2Captcha](https://2captcha.com) - Reliable, wide captcha support
- [CapSolver](https://capsolver.com) - Fast, modern API
- [Anti-Captcha](https://anti-captcha.com) - Enterprise-grade

## Other ForeverTools Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.APILayer** | IP geolocation, currency exchange, phone & email validation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.APILayer.svg)](https://www.nuget.org/packages/ForeverTools.APILayer) |
| **ForeverTools.ImageGen** | AI image generation with social media presets (DALL-E, Flux, SD) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ImageGen.svg)](https://www.nuget.org/packages/ForeverTools.ImageGen) |
| **ForeverTools.OCR** | AI-powered OCR using GPT-4 Vision, Claude 3, and Gemini | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.OCR.svg)](https://www.nuget.org/packages/ForeverTools.OCR) |
| **ForeverTools.Postmark** | Transactional email sending with templates and tracking | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation and CAPTCHA bypass | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |
| **ForeverTools.Proxy** | Premium proxy rotation with BrightData (Residential, ISP, Mobile) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Proxy.svg)](https://www.nuget.org/packages/ForeverTools.Proxy) |
| **ForeverTools.Translate** | AI-powered translation with 100+ languages (GPT-4, Claude) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Translate.svg)](https://www.nuget.org/packages/ForeverTools.Translate) |

## Requirements

- .NET 8.0, .NET 6.0, or .NET Standard 2.0 compatible framework
- API key from at least one provider

## License

MIT License - see [LICENSE](LICENSE) for details.
