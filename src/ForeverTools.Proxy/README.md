# ForeverTools.Proxy

Premium proxy client for .NET with [BrightData](https://get.brightdata.com/ForeverToolsResidentialProxies) integration. Access residential, datacenter, ISP, and mobile proxies with geo-targeting, session management, and automatic rotation.

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.Proxy.svg)](https://www.nuget.org/packages/ForeverTools.Proxy/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ForeverTools.Proxy.svg)](https://www.nuget.org/packages/ForeverTools.Proxy/)

## Features

- **Multiple Proxy Types**: Residential, Datacenter, ISP, and Mobile proxies
- **Geo-Targeting**: Target by country, state, and city
- **Session Management**: Sticky sessions for maintaining the same IP
- **Protocol Support**: HTTP and SOCKS5 protocols
- **Easy Integration**: Works with HttpClient, WebProxy, and dependency injection
- **Connection Testing**: Built-in proxy testing and validation

## Getting Your BrightData Credentials

This package requires a [BrightData](https://get.brightdata.com/ForeverToolsResidentialProxies) account. Choose the proxy type that best fits your needs:

| Proxy Type | Best For | Get Started |
|------------|----------|-------------|
| **Residential** | General scraping, geo-targeting, ad verification | [Get Residential Proxies](https://get.brightdata.com/ForeverToolsResidentialProxies) |
| **ISP Proxies** | Long sessions, account management, high stability | [Get ISP Proxies](https://get.brightdata.com/ForeverToolsISP) |
| **Social Media** | Social platform automation, content access | [Get Social Media Proxies](https://get.brightdata.com/ForeverToolsSocialProxies) |
| **SERP API** | Search engine scraping, SEO monitoring | [Get SERP API](https://get.brightdata.com/ForeverToolsSerp) |

After signing up, you'll need:
- **Customer ID**: Found in your BrightData dashboard
- **Zone Name**: The name of your proxy zone
- **Zone Password**: The password for your zone

## Installation

```bash
dotnet add package ForeverTools.Proxy
```

## Quick Start

### Basic Usage

```csharp
using ForeverTools.Proxy;

// Create client with credentials
var client = new BrightDataClient("your-customer-id", "your-zone", "your-password");

// Make a request through the proxy
var html = await client.GetAsync("https://example.com");

// Or create an HttpClient for more control
using var httpClient = client.CreateHttpClient();
var response = await httpClient.GetAsync("https://example.com");
```

### Using Environment Variables

```csharp
// Set environment variables:
// BRIGHTDATA_CUSTOMER_ID, BRIGHTDATA_ZONE, BRIGHTDATA_PASSWORD

var client = BrightDataClient.FromEnvironment();
```

## Geo-Targeting

### Target by Country

```csharp
// Get content as if from the US
var usContent = await client.GetFromCountryAsync("https://example.com", "us");

// Create an HttpClient for a specific country
using var ukClient = client.CreateHttpClientForCountry("gb");
```

### Target by City

```csharp
using ForeverTools.Proxy;

// Target specific city
var geoTarget = new GeoTarget
{
    Country = "us",
    State = "ny",
    City = "new_york"
};

using var nyClient = client.CreateHttpClient(geoTarget);
var content = await nyClient.GetStringAsync("https://example.com");
```

### Using Predefined Locations

```csharp
// Common locations are predefined for convenience
var usClient = client.CreateHttpClient(GeoTarget.Locations.UnitedStates);
var ukClient = client.CreateHttpClient(GeoTarget.Locations.UnitedKingdom);
var deClient = client.CreateHttpClient(GeoTarget.Locations.Germany);
var jpClient = client.CreateHttpClient(GeoTarget.Locations.Japan);
```

## Proxy Types

### Residential Proxies
Best for general web scraping and geo-targeting. Uses real residential IPs.

```csharp
using var httpClient = client.CreateResidentialHttpClient();
// or with geo-targeting
using var usClient = client.CreateResidentialHttpClient(GeoTarget.ForCountry("us"));
```

### Datacenter Proxies
Best for high-speed scraping and bulk requests.

```csharp
using var httpClient = client.CreateDatacenterHttpClient();
```

### ISP Proxies
Best for long sessions and account management. Combines residential trust with datacenter speed.

```csharp
using var httpClient = client.CreateISPHttpClient();
```

### Mobile Proxies
Best for mobile-specific content and social media.

```csharp
using var httpClient = client.CreateMobileHttpClient();
```

## Session Management

Maintain the same IP address across multiple requests using sessions:

```csharp
// Create a session (same IP for up to 5 minutes)
using var session = client.CreateSession();

// All requests use the same IP
using var httpClient = session.CreateHttpClient();
await httpClient.GetAsync("https://example.com/page1");
await httpClient.GetAsync("https://example.com/page2");
await httpClient.GetAsync("https://example.com/page3");

// Check session validity
Console.WriteLine($"Session age: {session.Age}");
Console.WriteLine($"Still valid: {session.IsValid}");
```

### Session with Geo-Targeting

```csharp
// Create a session from a specific location
using var session = client.CreateSession(GeoTarget.ForCountry("de"));
```

## Testing Your Proxy

```csharp
// Test the proxy connection
var result = await client.TestProxyAsync();

if (result.Success)
{
    Console.WriteLine($"Proxy working!");
    Console.WriteLine($"External IP: {result.ExternalIp}");
    Console.WriteLine($"Country: {result.Country}");
    Console.WriteLine($"City: {result.City}");
    Console.WriteLine($"Response time: {result.ResponseTimeMs}ms");
}
else
{
    Console.WriteLine($"Proxy failed: {result.Error}");
}
```

### Test Multiple Countries

```csharp
var countries = new[] { "us", "gb", "de", "jp", "au" };
var results = await client.TestMultipleCountriesAsync(countries);

foreach (var result in results)
{
    Console.WriteLine($"{result.Country}: {(result.Success ? result.ExternalIp : result.Error)}");
}
```

## Dependency Injection

### Basic Registration

```csharp
// In Program.cs or Startup.cs
services.AddForeverToolsProxy("customer-id", "zone", "password");

// Inject and use
public class MyService
{
    private readonly BrightDataClient _proxyClient;

    public MyService(BrightDataClient proxyClient)
    {
        _proxyClient = proxyClient;
    }

    public async Task DoWorkAsync()
    {
        var content = await _proxyClient.GetAsync("https://example.com");
    }
}
```

### Configuration-Based Registration

```csharp
// appsettings.json
{
    "Proxy": {
        "CustomerId": "your-customer-id",
        "Zone": "your-zone",
        "Password": "your-password",
        "DefaultCountry": "us",
        "TimeoutSeconds": 60
    }
}

// Registration
services.AddForeverToolsProxy(configuration);
```

### Named HttpClient

```csharp
// Register a named HttpClient with proxy
services.AddBrightDataHttpClient(
    "ProxiedClient",
    "customer-id",
    "zone",
    "password"
);

// Use via IHttpClientFactory
public class MyService
{
    private readonly IHttpClientFactory _factory;

    public MyService(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task DoWorkAsync()
    {
        var client = _factory.CreateClient("ProxiedClient");
        var response = await client.GetAsync("https://example.com");
    }
}
```

## Working with WebProxy

If you need a `WebProxy` instance for use with other libraries:

```csharp
// Create a WebProxy
var webProxy = client.CreateWebProxy();

// Or with geo-targeting
var usProxy = client.CreateWebProxyForCountry("us");

// Use with any client that supports WebProxy
var handler = new HttpClientHandler { Proxy = webProxy, UseProxy = true };
```

## Getting Raw Credentials

Access the raw proxy credentials for custom integrations:

```csharp
var credentials = client.GetCredentials();

Console.WriteLine($"Host: {credentials.Host}");
Console.WriteLine($"Port: {credentials.Port}");
Console.WriteLine($"Username: {credentials.Username}");
Console.WriteLine($"Password: {credentials.Password}");
Console.WriteLine($"Proxy URL: {credentials.ProxyUrl}");

// Generate a curl command for testing
Console.WriteLine(credentials.ToCurlCommand("https://httpbin.org/ip"));
```

## Configuration Options

```csharp
var options = new ProxyOptions
{
    CustomerId = "your-customer-id",
    Zone = "your-zone",
    Password = "your-password",

    // Defaults
    DefaultProxyType = ProxyType.Residential,
    DefaultProtocol = ProxyProtocol.Http,
    DefaultCountry = "us",
    DefaultCity = null,
    DefaultState = null,
    TimeoutSeconds = 60,
    Host = "brd.superproxy.io"
};

var client = new BrightDataClient(options);
```

## Error Handling

```csharp
try
{
    var content = await client.GetAsync("https://example.com");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Request failed: {ex.Message}");
}
catch (TaskCanceledException)
{
    Console.WriteLine("Request timed out");
}
```

## Best Practices

1. **Reuse HttpClient**: Create one `HttpClient` and reuse it for multiple requests to the same target.

2. **Use Sessions for Related Requests**: When making multiple requests that should appear from the same IP, use sessions.

3. **Handle Timeouts**: Proxy requests may be slower than direct requests. Configure appropriate timeouts.

4. **Rotate IPs When Needed**: For scraping, create new clients without sessions to get fresh IPs.

5. **Target Appropriately**: Use geo-targeting only when necessary to maximize your proxy pool.

## Related Packages

- [ForeverTools.ImageGen](https://www.nuget.org/packages/ForeverTools.ImageGen/) - AI image generation with multiple providers
- [ForeverTools.Converters.DocumentToMarkdown](https://www.nuget.org/packages/ForeverTools.Converters.DocumentToMarkdown/) - Document to Markdown conversion

## Support

- [GitHub Issues](https://github.com/stimpy77/ForeverTools/issues)
- [BrightData Documentation](https://docs.brightdata.com/)
- [BrightData Support](https://brightdata.com/contact)

## License

MIT License - see LICENSE file for details.
