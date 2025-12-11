# ForeverTools.OCR

AI-powered OCR for .NET using GPT-4 Vision, Claude 3, and Gemini. Extract text from images, documents, screenshots, receipts, and scanned files with state-of-the-art accuracy.

## Features

- **Simple Text Extraction** - Extract all text from any image
- **Multiple Input Formats** - File path, bytes, base64, URL, or stream
- **Structured Output** - Get text with paragraphs, lines, and blocks
- **Table Extraction** - Extract tables as structured data or CSV
- **Form Recognition** - Extract form fields and values
- **Receipt/Invoice OCR** - Parse receipts with merchant, items, totals
- **Multiple Models** - GPT-4o, Claude 3, Gemini, and more
- **Async/Await** - Fully asynchronous API
- **Dependency Injection** - Built-in ASP.NET Core support
- **Multi-Target** - .NET 8, .NET 6, .NET Standard 2.0

## Installation

```bash
dotnet add package ForeverTools.OCR
```

## Quick Start

Get your API key at [aimlapi.com](https://aimlapi.com?via=forevertools).

```csharp
using ForeverTools.OCR;

var client = new OcrClient("your-api-key");

// Extract text from an image file
var result = await client.ExtractTextFromFileAsync("document.png");
if (result.Success)
{
    Console.WriteLine(result.Text);
}

// Extract from URL
var urlResult = await client.ExtractTextFromUrlAsync("https://example.com/image.jpg");

// Extract from bytes
byte[] imageData = File.ReadAllBytes("photo.jpg");
var bytesResult = await client.ExtractTextAsync(imageData);
```

## Model Selection

Choose the best model for your use case:

```csharp
// General purpose (default) - best balance
var result = await client.ExtractTextFromFileAsync("doc.png", OcrModels.Gpt4o);

// Fast and cheap - for clear printed text
var fast = await client.ExtractTextFromFileAsync("screenshot.png", OcrModels.Gpt4oMini);

// Handwriting recognition - highest accuracy
var handwriting = await client.ExtractTextFromFileAsync("notes.jpg", OcrModels.Claude3Opus);

// Non-English text - best multilingual support
var foreign = await client.ExtractTextFromFileAsync("document.png", OcrModels.Gemini15Pro);

// Use recommendations helper
var receipt = await client.ExtractTextFromFileAsync("receipt.jpg", OcrModels.Recommendations.Receipts);
```

## Structured Extraction

### Extract with Layout

```csharp
var result = await client.ExtractStructuredAsync(imageBytes);

Console.WriteLine("Paragraphs:");
foreach (var paragraph in result.Paragraphs)
{
    Console.WriteLine($"  - {paragraph}");
}

Console.WriteLine("\nText Blocks:");
foreach (var block in result.Blocks)
{
    Console.WriteLine($"  [{block.BlockType}] {block.Text}");
}
```

### Extract Tables

```csharp
var result = await client.ExtractTablesAsync(imageBytes);

foreach (var table in result.Tables)
{
    Console.WriteLine($"Table: {table.ColumnCount} columns, {table.RowCount} rows");
    Console.WriteLine(table.ToCsv());
}
```

### Extract Form Fields

```csharp
var result = await client.ExtractFormFieldsAsync(imageBytes);

foreach (var field in result.Fields)
{
    Console.WriteLine($"{field.Key}: {field.Value}");
}

// Get specific field
var name = result.GetField("Name");
var date = result.GetField("Date");
```

### Extract Receipts

```csharp
var result = await client.ExtractReceiptAsync(imageBytes);

Console.WriteLine($"Merchant: {result.MerchantName}");
Console.WriteLine($"Date: {result.Date}");
Console.WriteLine($"Total: {result.Total}");
Console.WriteLine($"Tax: {result.Tax}");

Console.WriteLine("\nItems:");
foreach (var item in result.Items)
{
    Console.WriteLine($"  {item.Description} x{item.Quantity} = {item.TotalPrice}");
}
```

## Custom Extraction

Use custom prompts for specialized extraction:

```csharp
var result = await client.ExtractWithPromptAsync(
    imageBytes,
    "Extract only the email addresses and phone numbers from this business card. Return as JSON with 'emails' and 'phones' arrays."
);
```

## Dependency Injection

```csharp
// Program.cs
builder.Services.AddForeverToolsOcr("your-api-key");

// Or with options
builder.Services.AddForeverToolsOcr(options =>
{
    options.ApiKey = "your-api-key";
    options.DefaultModel = OcrModels.Gpt4o;
    options.TimeoutSeconds = 90;
    options.MaxTokens = 4096;
    options.ImageDetail = "high"; // "low", "high", or "auto"
});

// Or from configuration
builder.Services.AddForeverToolsOcr(builder.Configuration);
```

appsettings.json:
```json
{
  "OCR": {
    "ApiKey": "your-api-key",
    "DefaultModel": "gpt-4o",
    "TimeoutSeconds": 60,
    "MaxTokens": 4096,
    "ImageDetail": "auto"
  }
}
```

Use in your services:
```csharp
public class DocumentService
{
    private readonly OcrClient _ocr;

    public DocumentService(OcrClient ocr)
    {
        _ocr = ocr;
    }

    public async Task<string> ProcessDocument(byte[] imageData)
    {
        var result = await _ocr.ExtractTextAsync(imageData);
        return result.Success ? result.Text : throw new Exception(result.Error);
    }
}
```

## Environment Variables

```csharp
// Uses AIML_API_KEY by default
var client = OcrClient.FromEnvironment();

// Or specify custom variable
var client = OcrClient.FromEnvironment("MY_OCR_KEY");
```

## Supported Image Formats

- PNG, JPEG, GIF, WebP, BMP, TIFF
- Automatic format detection from bytes
- URLs to images (publicly accessible)

## Error Handling

```csharp
var result = await client.ExtractTextFromFileAsync("document.png");

if (result.Success)
{
    Console.WriteLine($"Text: {result.Text}");
    Console.WriteLine($"Model: {result.Model}");
    Console.WriteLine($"Tokens: {result.TokensUsed}");
    Console.WriteLine($"Time: {result.ProcessingTimeMs}ms");
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

## Available Models

| Model | Best For | Speed | Accuracy |
|-------|----------|-------|----------|
| `gpt-4o` | General purpose | Fast | High |
| `gpt-4o-mini` | Screenshots, clear text | Fastest | Good |
| `gpt-4-turbo` | Scanned documents | Medium | High |
| `claude-3-5-sonnet` | Forms, structured docs | Fast | High |
| `claude-3-opus` | Handwriting, critical docs | Slow | Highest |
| `gemini-1.5-pro` | Non-English, multilingual | Medium | High |
| `gemini-1.5-flash` | Quick tasks | Fast | Good |

## Other ForeverTools Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, DALL-E) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.APILayer** | IP geolocation, currency exchange, phone & email validation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.APILayer.svg)](https://www.nuget.org/packages/ForeverTools.APILayer) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.Postmark** | Transactional email sending with templates | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |

## License

MIT License - see LICENSE file for details.

## Links

- [Get API Key](https://aimlapi.com?via=forevertools)
- [GitHub Repository](https://github.com/ForeverTools/ForeverTools)
- [NuGet Package](https://www.nuget.org/packages/ForeverTools.OCR)
