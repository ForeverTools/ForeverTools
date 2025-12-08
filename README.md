# ForeverTools

.NET wrapper libraries for popular third-party APIs. Clean interfaces, dependency injection ready, available on NuGet.

## What is this?

ForeverTools is a collection of NuGet packages that simplify working with common developer services:

- **Captcha solving** - Anti-Captcha, 2Captcha, CapSolver behind one interface
- **Proxy rotation** - BrightData, SmartProxy, and more
- **SMS & messaging** - BulkGate, Textmagic
- **Web scraping** - ScraperAPI, Crawlbase

Each package follows the same patterns: simple configuration, dependency injection support, and sensible defaults.

## Packages

*Coming soon* - first packages currently in development.

## Installation

All packages are available on NuGet:
```bash
dotnet add package ForeverTools.Captcha
```

## Quick Example
```csharp
services.AddForeverToolsCaptcha(options =>
{
    options.Provider = CaptchaProvider.AntiCaptcha;
    options.ApiKey = "your-api-key";
});
```

## License

MIT
