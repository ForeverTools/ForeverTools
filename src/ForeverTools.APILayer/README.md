# ForeverTools.APILayer

A unified .NET client for the [APILayer marketplace](https://apilayer.com?fpr=chris72) APIs. Access IP geolocation, currency exchange rates, phone validation, email verification, and more with a single API key.

## Features

- **IP Geolocation** - Look up geographic location, ISP, and security data for any IP address
- **Currency Exchange** - Real-time and historical exchange rates for 168+ currencies
- **Phone Validation** - Validate phone numbers and get carrier/line type information
- **Email Validation** - Verify email deliverability, detect disposables, and more
- **Multi-targeting** - Supports .NET 8.0, .NET 6.0, and .NET Standard 2.0
- **Async/await** - Full async support with cancellation tokens
- **Dependency Injection** - Built-in support for Microsoft.Extensions.DependencyInjection

## Installation

```bash
dotnet add package ForeverTools.APILayer
```

## Quick Start

Get your API key at [apilayer.com](https://apilayer.com?fpr=chris72).

```csharp
using ForeverTools.APILayer;

// Create client
var client = new ApiLayerClient("your-api-key");

// IP Geolocation
var geoResult = await client.GetIpGeolocationAsync("8.8.8.8");
if (geoResult.Success)
{
    Console.WriteLine($"Country: {geoResult.Data.CountryName}");
    Console.WriteLine($"City: {geoResult.Data.City}");
}

// Currency Exchange
var ratesResult = await client.GetExchangeRatesAsync("USD", new[] { "EUR", "GBP", "JPY" });
if (ratesResult.Success)
{
    Console.WriteLine($"EUR: {ratesResult.Data.GetRate("EUR")}");
}

// Currency Conversion
var convertResult = await client.ConvertCurrencyAsync("USD", "EUR", 100);
if (convertResult.Success)
{
    Console.WriteLine($"100 USD = {convertResult.Data.Result} EUR");
}

// Phone Validation
var phoneResult = await client.ValidatePhoneAsync("+14155552671");
if (phoneResult.Success && phoneResult.Data.Valid)
{
    Console.WriteLine($"Carrier: {phoneResult.Data.Carrier}");
    Console.WriteLine($"Type: {phoneResult.Data.LineType}");
}

// Email Validation
var emailResult = await client.ValidateEmailAsync("test@example.com");
if (emailResult.Success)
{
    Console.WriteLine($"Valid format: {emailResult.Data.FormatValid}");
    Console.WriteLine($"Deliverable: {emailResult.Data.IsDeliverable}");
    Console.WriteLine($"Disposable: {emailResult.Data.Disposable}");
}
```

## Dependency Injection

```csharp
// In Program.cs or Startup.cs
services.AddForeverToolsApiLayer("your-api-key");

// Or from configuration
services.AddForeverToolsApiLayer(configuration);
```

appsettings.json:
```json
{
  "APILayer": {
    "ApiKey": "your-api-key",
    "TimeoutSeconds": 30
  }
}
```

Inject and use:
```csharp
public class MyService
{
    private readonly ApiLayerClient _client;

    public MyService(ApiLayerClient client)
    {
        _client = client;
    }

    public async Task<string> GetUserCountry(string ipAddress)
    {
        var result = await _client.GetIpGeolocationAsync(ipAddress);
        return result.Success ? result.Data.CountryName : "Unknown";
    }
}
```

## API Reference

### IP Geolocation

```csharp
// Look up any IP address
var result = await client.GetIpGeolocationAsync("8.8.8.8");

// Look up caller's IP
var myIp = await client.GetMyIpGeolocationAsync();

// Access detailed data
result.Data.CountryCode    // "US"
result.Data.City           // "Mountain View"
result.Data.Latitude       // 37.386
result.Data.TimeZone.Id    // "America/Los_Angeles"
result.Data.Currency.Code  // "USD"
result.Data.Security.IsProxy  // false
```

### Currency Exchange

```csharp
// Latest rates
var rates = await client.GetExchangeRatesAsync("USD");

// Historical rates
var historical = await client.GetHistoricalRatesAsync("2024-01-15", "USD");

// Convert currency
var conversion = await client.ConvertCurrencyAsync("USD", "EUR", 100);

// Time series (365 days max)
var series = await client.GetTimeSeriesAsync("2024-01-01", "2024-01-31", "USD");

// Rate fluctuation
var fluctuation = await client.GetFluctuationAsync("2024-01-01", "2024-01-31", "USD");

// Available symbols
var symbols = await client.GetCurrencySymbolsAsync();
```

### Phone Validation

```csharp
var result = await client.ValidatePhoneAsync("+14155552671");

result.Data.Valid               // true
result.Data.LocalFormat         // "4155552671"
result.Data.InternationalFormat // "+14155552671"
result.Data.CountryCode         // "US"
result.Data.Carrier             // "AT&T Mobility LLC"
result.Data.LineType            // "mobile"
```

### Email Validation

```csharp
var result = await client.ValidateEmailAsync("user@example.com");

result.Data.FormatValid    // true
result.Data.MxFound        // true
result.Data.SmtpCheck      // true
result.Data.Disposable     // false
result.Data.Free           // false
result.Data.Role           // false
result.Data.Score          // 0.85
result.Data.IsDeliverable  // true (computed)
result.Data.Quality        // EmailQuality.High
```

## Error Handling

All methods return `ApiLayerResponse<T>` which includes error information:

```csharp
var result = await client.GetIpGeolocationAsync("invalid");

if (!result.Success)
{
    Console.WriteLine($"Error: {result.Error.Message}");
    Console.WriteLine($"Code: {result.Error.Code}");
}
```

## Environment Variables

```csharp
// Uses APILAYER_KEY by default
var client = ApiLayerClient.FromEnvironment();

// Or specify a custom variable
var client = ApiLayerClient.FromEnvironment("MY_API_KEY");
```

## Other ForeverTools Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.ImageGen** | AI image generation with social media presets (DALL-E, Flux, SD) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ImageGen.svg)](https://www.nuget.org/packages/ForeverTools.ImageGen) |
| **ForeverTools.OCR** | AI-powered OCR using GPT-4 Vision, Claude 3, and Gemini | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.OCR.svg)](https://www.nuget.org/packages/ForeverTools.OCR) |
| **ForeverTools.Postmark** | Transactional email sending with templates and tracking | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation and CAPTCHA bypass | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |
| **ForeverTools.Proxy** | Premium proxy rotation with BrightData (Residential, ISP, Mobile) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Proxy.svg)](https://www.nuget.org/packages/ForeverTools.Proxy) |
| **ForeverTools.Translate** | AI-powered translation with 100+ languages (GPT-4, Claude) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Translate.svg)](https://www.nuget.org/packages/ForeverTools.Translate) |

## License

MIT License - see LICENSE file for details.

## Links

- [Get API Key](https://apilayer.com?fpr=chris72)
- [API Documentation](https://apilayer.com/docs)
- [GitHub Repository](https://github.com/ForeverTools/ForeverTools)
- [NuGet Package](https://www.nuget.org/packages/ForeverTools.APILayer)
