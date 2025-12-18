# ForeverTools.Apify

Lightweight Apify client for .NET. Web scraping and automation platform with 1,600+ ready-made actors for Amazon, Google, Instagram, Twitter, and more.

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.Apify.svg)](https://www.nuget.org/packages/ForeverTools.Apify)

## Features

- **1,600+ Ready-Made Scrapers** - Amazon, Google, Instagram, Twitter, LinkedIn, and more
- **Run Any Actor** - Execute pre-built or custom actors with one method
- **Datasets** - Store and retrieve structured scraping results
- **Key-Value Stores** - Save files, screenshots, and JSON data
- **Schedules** - Automate recurring scrapes with cron expressions
- **Async Operations** - Start jobs and check status later
- **ASP.NET Core Ready** - Built-in dependency injection
- **Multi-Target** - .NET 8, .NET 6, .NET Standard 2.0

## Quick Start

### Install

```bash
dotnet add package ForeverTools.Apify
```

### Get Your API Token

Sign up at [Apify](https://www.apify.com/?fpr=8hklqy) to get your API token with free monthly credits.

### Basic Usage

```csharp
using ForeverTools.Apify;
using ForeverTools.Apify.Constants;

var client = new ApifyClient("your-api-token");

// Run a web scraper and get results
var results = await client.ScrapeAsync<Dictionary<string, object>>(
    PopularActors.WebScraper,
    new
    {
        startUrls = new[] { new { url = "https://example.com" } },
        maxPagesPerCrawl = 10
    });

foreach (var item in results)
{
    Console.WriteLine(item["url"]);
}
```

## Popular Actors

Use pre-built scrapers from the [Apify Store](https://www.apify.com/store?fpr=8hklqy):

```csharp
using ForeverTools.Apify.Constants;

// E-Commerce
var amazonProducts = await client.ScrapeAsync<AmazonProduct>(
    PopularActors.AmazonScraper,
    new { keyword = "laptop", maxItems = 100 });

// Social Media
var instagramPosts = await client.ScrapeAsync<InstagramPost>(
    PopularActors.InstagramScraper,
    new { usernames = new[] { "nasa" }, resultsLimit = 50 });

// Search Engines
var googleResults = await client.ScrapeAsync<SearchResult>(
    PopularActors.GoogleSearchScraper,
    new { queries = "web scraping tools", maxPagesPerQuery = 1 });

// Google Maps
var places = await client.ScrapeAsync<Place>(
    PopularActors.GoogleMapsScraper,
    new { searchStringsArray = new[] { "restaurants in New York" } });
```

### Available Actor Constants

| Category | Actors |
|----------|--------|
| **Web Scraping** | `WebScraper`, `CheerioScraper`, `PuppeteerScraper`, `PlaywrightScraper` |
| **E-Commerce** | `AmazonScraper`, `EbayScraper`, `WalmartScraper`, `ShopifyScraper`, `EtsyScraper` |
| **Social Media** | `InstagramScraper`, `TwitterScraper`, `TikTokScraper`, `YouTubeScraper`, `LinkedInScraper` |
| **Search** | `GoogleSearchScraper`, `GoogleMapsScraper`, `BingSearchScraper` |
| **Travel** | `BookingScraper`, `AirbnbScraper`, `TripAdvisorScraper` |
| **Jobs** | `IndeedScraper`, `GlassdoorScraper` |
| **Reviews** | `TrustpilotScraper`, `YelpScraper`, `AppStoreScraper` |

## Advanced Usage

### Run Actor and Wait

```csharp
// Start actor and wait for completion
var run = await client.RunActorAsync(
    "apify/web-scraper",
    new { startUrls = new[] { new { url = "https://example.com" } } },
    new ActorRunOptions
    {
        MemoryMb = 1024,
        TimeoutSeconds = 300
    });

Console.WriteLine($"Run status: {run.Status}");
Console.WriteLine($"Dataset ID: {run.DefaultDatasetId}");

// Get results from the dataset
var items = await client.GetDatasetItemsAsync<MyDataClass>(run.DefaultDatasetId);
```

### Start Actor Without Waiting

```csharp
// Start actor and get run ID immediately
var run = await client.StartActorAsync("apify/web-scraper", input);
Console.WriteLine($"Started run: {run.Id}");

// Check status later
var status = await client.GetRunAsync(run.Id);
if (status.IsSucceeded)
{
    var items = await client.GetRunDatasetItemsAsync<MyData>(run.Id);
}
```

### Get Run Output from Key-Value Store

```csharp
// Some actors store their output in key-value stores instead of datasets
var output = await client.RunAndGetOutputAsync<MyOutput>(
    "apify/some-actor",
    new { url = "https://example.com" },
    outputKey: "OUTPUT");
```

### Dataset Operations

```csharp
// Get items with pagination
var items = await client.GetDatasetItemsAsync<Product>(
    datasetId,
    new DatasetItemsOptions
    {
        Offset = 0,
        Limit = 100,
        Clean = true,
        Fields = new List<string> { "title", "price", "url" }
    });

// Push items to a dataset
await client.PushDatasetItemsAsync(datasetId, new[]
{
    new { title = "Product 1", price = 29.99 },
    new { title = "Product 2", price = 39.99 }
});

// List all datasets
var datasets = await client.ListDatasetsAsync();
```

### Key-Value Store Operations

```csharp
// Get a JSON record
var data = await client.GetKeyValueRecordAsync<MyData>(storeId, "my-key");

// Get a screenshot or file as bytes
var screenshot = await client.GetKeyValueRecordBytesAsync(storeId, "screenshot.png");
File.WriteAllBytes("screenshot.png", screenshot);

// Store data
await client.SetKeyValueRecordAsync(storeId, "my-key", new { foo = "bar" });

// Store a file
var imageBytes = File.ReadAllBytes("image.png");
await client.SetKeyValueRecordAsync(storeId, "image.png", imageBytes, "image/png");

// List all keys
var keys = await client.ListKeyValueStoreKeysAsync(storeId);
```

### Schedule Recurring Runs

```csharp
// Create a schedule to run an actor daily at 9 AM
var schedule = await client.CreateScheduleAsync(new ScheduleRequest
{
    Name = "Daily Amazon Scrape",
    CronExpression = "0 9 * * *",
    Timezone = "America/New_York",
    IsEnabled = true,
    Actions = new List<ScheduleAction>
    {
        new ScheduleAction
        {
            Type = ScheduleActionTypes.RunActor,
            ActorId = "junglee/amazon-crawler",
            RunInput = new ScheduleRunInput
            {
                ContentType = "application/json",
                Body = JsonSerializer.Serialize(new { keyword = "laptop" })
            }
        }
    }
});

// List schedules
var schedules = await client.ListSchedulesAsync();

// Update schedule
await client.UpdateScheduleAsync(schedule.Id, new ScheduleRequest
{
    IsEnabled = false
});

// Delete schedule
await client.DeleteScheduleAsync(schedule.Id);
```

### Get User Information

```csharp
var user = await client.GetUserAsync();
Console.WriteLine($"Username: {user.Username}");
Console.WriteLine($"Plan: {user.Plan?.Name}");
Console.WriteLine($"Max concurrent runs: {user.Limits?.MaxConcurrentActorJobs}");
```

## ASP.NET Core Integration

```csharp
// Program.cs
builder.Services.AddForeverToolsApify("your-api-token");

// Or with full configuration
builder.Services.AddForeverToolsApify(options =>
{
    options.Token = "your-api-token";
    options.DefaultMemoryMb = 512;
    options.DefaultTimeoutSeconds = 600;
    options.TimeoutSeconds = 300;
});

// Or from appsettings.json
builder.Services.AddForeverToolsApify(builder.Configuration);
```

```json
// appsettings.json
{
  "Apify": {
    "Token": "your-api-token",
    "DefaultMemoryMb": 512,
    "DefaultTimeoutSeconds": 600
  }
}
```

```csharp
// Inject and use
public class ProductScraperService
{
    private readonly ApifyClient _apify;

    public ProductScraperService(ApifyClient apify)
    {
        _apify = apify;
    }

    public async Task<List<Product>> ScrapeAmazonAsync(string keyword)
    {
        return await _apify.ScrapeAsync<Product>(
            PopularActors.AmazonScraper,
            new { keyword, maxItems = 100 });
    }
}
```

## Environment Variables

```csharp
// Uses APIFY_TOKEN by default
var client = ApifyClient.FromEnvironment();

// Or specify custom variable name
var client = ApifyClient.FromEnvironment("MY_APIFY_TOKEN");
```

## Error Handling

```csharp
try
{
    var results = await client.ScrapeAsync<Product>(actorId, input);
}
catch (ApifyException ex)
{
    Console.WriteLine($"Apify error: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
}
```

## Why Apify?

[Apify](https://www.apify.com/?fpr=8hklqy) is the leading web scraping and automation platform:

- **1,600+ ready-made scrapers** - No coding required for common sites
- **Powerful infrastructure** - Runs in the cloud with automatic scaling
- **Proxy management** - Built-in residential and datacenter proxies
- **Data storage** - Datasets and key-value stores included
- **Scheduling** - Automate recurring scrapes
- **Free tier** - Get started with free monthly credits

## Other ForeverTools Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.APILayer** | IP geolocation, currency exchange, phone & email validation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.APILayer.svg)](https://www.nuget.org/packages/ForeverTools.APILayer) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.ImageGen** | AI image generation with social media presets (DALL-E, Flux, SD) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ImageGen.svg)](https://www.nuget.org/packages/ForeverTools.ImageGen) |
| **ForeverTools.OCR** | AI-powered OCR using GPT-4 Vision, Claude 3, and Gemini | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.OCR.svg)](https://www.nuget.org/packages/ForeverTools.OCR) |
| **ForeverTools.Postmark** | Transactional email sending with templates and tracking | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |
| **ForeverTools.Proxy** | Premium proxy rotation with BrightData (Residential, ISP, Mobile) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Proxy.svg)](https://www.nuget.org/packages/ForeverTools.Proxy) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation and CAPTCHA solving | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |
| **ForeverTools.STT** | Speech-to-Text using Whisper (transcription, subtitles, language detection) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.STT.svg)](https://www.nuget.org/packages/ForeverTools.STT) |
| **ForeverTools.Summarize** | AI-powered text summarization (TL;DR, bullet points, executive summaries) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Summarize.svg)](https://www.nuget.org/packages/ForeverTools.Summarize) |
| **ForeverTools.Translate** | AI-powered translation with 100+ languages (GPT-4, Claude) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Translate.svg)](https://www.nuget.org/packages/ForeverTools.Translate) |

## Requirements

- .NET 8.0, .NET 6.0, or .NET Standard 2.0 compatible framework
- Apify account with API token

## License

MIT License - see [LICENSE](LICENSE) for details.
