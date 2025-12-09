# ForeverTools

.NET wrapper libraries for popular third-party APIs. Clean interfaces, dependency injection ready, available on NuGet.

## Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E, Stable Diffusion) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |

## Coming Soon

- **ForeverTools.Postmark** - Transactional email (Postmark)
- **ForeverTools.Proxy** - Proxy rotation (BrightData, SmartProxy, ScraperAPI)
- **ForeverTools.SMS** - SMS & messaging (BulkGate, Textmagic)

## Installation

All packages are available on NuGet:

```bash
# AI/ML (400+ AI models)
dotnet add package ForeverTools.AIML

# Captcha solving (reCAPTCHA, hCaptcha, Turnstile)
dotnet add package ForeverTools.Captcha
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

### ASP.NET Core
```csharp
// Program.cs
builder.Services.AddForeverToolsAiml("your-api-key");
builder.Services.AddForeverToolsCaptcha(options =>
{
    options.TwoCaptchaApiKey = "your-2captcha-key";
    options.DefaultProvider = CaptchaProvider.TwoCaptcha;
});

// Or from configuration
builder.Services.AddForeverToolsAiml(builder.Configuration);
builder.Services.AddForeverToolsCaptcha(builder.Configuration);
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
