# ForeverTools.BrightData

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.BrightData.svg)](https://www.nuget.org/packages/ForeverTools.BrightData)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ForeverTools.BrightData.svg)](https://www.nuget.org/packages/ForeverTools.BrightData)

**BrightData Web Scraper API client for .NET.** Extract structured data from any website at scale — e-commerce prices, leads, real estate listings, job postings, and more — using BrightData's managed scraping infrastructure.

🔗 **Get your API token:** [https://get.brightdata.com/ForeverToolsWebScraper](https://get.brightdata.com/ForeverToolsWebScraper)

## Features

- Trigger dataset scrape jobs with URLs or custom inputs
- Poll for completion with configurable timeouts
- Download results as JSON or typed .NET objects
- Single-call `ScrapeAsync` / `ScrapeUrlsAsync` convenience methods
- Full `IHttpClientFactory` / DI support
- Targets `net8.0`, `net6.0`, `netstandard2.0`

## Installation

```bash
dotnet add package ForeverTools.BrightData
```

## Quick Start

```csharp
using ForeverTools.BrightData;

// Create client
var client = new BrightDataClient("your-brightdata-api-token");

// Scrape a list of product pages and deserialize results
var products = await client.ScrapeUrlsAsync<MyProduct>(
    datasetId: "gd_l1vikfnt1wgvvsz95k",
    urls: new[]
    {
        "https://www.amazon.com/dp/B08N5WRWNW",
        "https://www.amazon.com/dp/B09G3HRMVB",
    });

foreach (var p in products)
    Console.WriteLine($"{p.Title}: ${p.Price}");
```

## Step-by-Step Control

```csharp
var client = new BrightDataClient("your-brightdata-api-token");

// 1. Trigger — start the job
var trigger = await client.TriggerAsync(
    datasetId: "gd_l1vikfnt1wgvvsz95k",
    inputs: new[]
    {
        new Dictionary<string, string> { ["url"] = "https://example.com/product/123" },
        new Dictionary<string, string> { ["url"] = "https://example.com/product/456" },
    });

Console.WriteLine($"Snapshot ID: {trigger.SnapshotId}");

// 2. Poll — wait for completion
var status = await client.WaitForSnapshotAsync(trigger.SnapshotId);
Console.WriteLine($"Ready: {status.Records} records");

// 3. Download — get results
var json = await client.DownloadSnapshotAsync(trigger.SnapshotId);
// or typed:
var results = await client.DownloadSnapshotAsync<MyProduct>(trigger.SnapshotId);
```

## From Environment Variable

```csharp
// Reads BRIGHTDATA_API_TOKEN from environment
var client = BrightDataClient.FromEnvironment();
```

## Dependency Injection

```csharp
// Program.cs
builder.Services.AddBrightData(builder.Configuration);

// appsettings.json
{
  "BrightData": {
    "ApiToken": "your-token",
    "MaxWaitSeconds": 300,
    "PollIntervalSeconds": 3
  }
}
```

## Configuration

| Option | Default | Description |
|---|---|---|
| `ApiToken` | — | Your BrightData API token (required) |
| `BaseUrl` | `https://api.brightdata.com` | API base URL |
| `TimeoutSeconds` | `60` | HTTP request timeout |
| `PollIntervalSeconds` | `3` | How often to poll for snapshot status |
| `MaxWaitSeconds` | `300` | Max time to wait for a snapshot to complete |

## Use Cases

- **E-commerce monitoring** — track competitor prices across thousands of SKUs
- **Lead generation** — extract contacts from business directories
- **Real estate** — aggregate listings from multiple property sites
- **Job boards** — collect job postings for market intelligence
- **SEO data** — SERPs, rankings, and featured snippets at scale

## License

MIT — see [LICENSE](https://github.com/ForeverTools/ForeverTools/blob/main/LICENSE)

---

Part of the [ForeverTools](https://github.com/ForeverTools/ForeverTools) .NET SDK collection.
