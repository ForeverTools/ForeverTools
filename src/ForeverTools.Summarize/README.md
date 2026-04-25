# ForeverTools.Summarize

AI-powered text summarization for .NET using GPT-4, Claude, Llama and 400+ AI models. Summarize articles, documents, legal texts, research papers, meeting notes, and more.

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.Summarize.svg)](https://www.nuget.org/packages/ForeverTools.Summarize/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ForeverTools.Summarize.svg)](https://www.nuget.org/packages/ForeverTools.Summarize/)

## Features

- **Multiple Summary Styles**: Paragraph, bullet points, executive, abstract, TL;DR, and more
- **Configurable Length**: From very short TL;DR to detailed comprehensive summaries
- **Domain-Aware**: Optimized for legal, medical, financial, academic, and technical content
- **Key Points Extraction**: Extract the most important points from any text
- **Action Items**: Extract action items from meeting notes
- **Batch Processing**: Summarize multiple documents efficiently
- **Multiple AI Models**: GPT-4, Claude, Llama, Gemini, Mistral, and more
- **Easy Integration**: Simple API with dependency injection support

## Getting Your API Key

This package uses the [AI/ML API](https://aimlapi.com?via=forevertools) which provides access to 400+ AI models including GPT-4, Claude, Llama, and more.

1. Sign up at [aimlapi.com](https://aimlapi.com?via=forevertools)
2. Get your API key from the dashboard
3. Start summarizing!

## Installation

```bash
dotnet add package ForeverTools.Summarize
```

## Quick Start

### Basic Summarization

```csharp
using ForeverTools.Summarize;

// Create client with your API key
var client = new SummarizeClient("your-api-key");

// Simple summarization
var summary = await client.SummarizeAsync(longArticle);

// TL;DR - very brief
var tldr = await client.TldrAsync(longArticle);

// Bullet points
var bullets = await client.BulletPointsAsync(longArticle);

// Executive summary (for business docs)
var executive = await client.ExecutiveSummaryAsync(businessReport);
```

### Using Environment Variables

```csharp
// Set AIML_API_KEY or SUMMARIZE_API_KEY environment variable
var client = SummarizeClient.FromEnvironment();
```

## Summary Styles

```csharp
// Paragraph (default) - flowing text summary
var paragraph = await client.SummarizeAsync(text, SummaryStyle.Paragraph);

// Bullet points - easy to scan
var bullets = await client.SummarizeAsync(text, SummaryStyle.BulletPoints);

// Numbered list
var numbered = await client.SummarizeAsync(text, SummaryStyle.NumberedList);

// Executive summary - business style
var executive = await client.SummarizeAsync(text, SummaryStyle.Executive);

// Academic abstract
var abstract_ = await client.SummarizeAsync(text, SummaryStyle.Abstract);

// TL;DR - extremely brief
var tldr = await client.SummarizeAsync(text, SummaryStyle.TLDR);

// Structured - with sections
var structured = await client.SummarizeAsync(text, SummaryStyle.Structured);

// Headline - single sentence
var headline = await client.HeadlineAsync(text);

// Q&A format
var qa = await client.SummarizeAsync(text, SummaryStyle.QAndA);
```

## Summary Length

```csharp
// Very short - 1-2 sentences
var veryShort = await client.SummarizeAsync(text, SummaryStyle.Paragraph, SummaryLength.VeryShort);

// Short - 2-3 sentences
var short_ = await client.SummarizeAsync(text, SummaryStyle.Paragraph, SummaryLength.Short);

// Medium - 1 paragraph (default)
var medium = await client.SummarizeAsync(text, SummaryStyle.Paragraph, SummaryLength.Medium);

// Long - 2-3 paragraphs
var long_ = await client.SummarizeAsync(text, SummaryStyle.Paragraph, SummaryLength.Long);

// Detailed - comprehensive
var detailed = await client.SummarizeAsync(text, SummaryStyle.Paragraph, SummaryLength.Detailed);

// Custom word count
var custom = await client.SummarizeWithDetailsAsync(new SummaryRequest
{
    Text = text,
    Length = SummaryLength.Custom,
    CustomWordCount = 150
});
```

## Key Points Extraction

```csharp
// Extract key points
var keyPoints = await client.ExtractKeyPointsAsync(article, maxPoints: 10);

foreach (var point in keyPoints)
{
    Console.WriteLine($"- {point}");
}

// With themes detection
var result = await client.ExtractKeyPointsWithDetailsAsync(article);
Console.WriteLine("Key Points:");
foreach (var point in result.KeyPoints)
{
    Console.WriteLine($"- {point}");
}

Console.WriteLine("\nThemes:");
foreach (var theme in result.Themes ?? Array.Empty<string>())
{
    Console.WriteLine($"- {theme}");
}
```

## Action Items from Meeting Notes

```csharp
var meetingNotes = @"
Team meeting - Dec 15, 2025
- John will complete the API integration by Friday
- Sarah needs to review the security audit - HIGH PRIORITY
- Mike to schedule client demo for next week
- Budget review due by end of month
";

var result = await client.ExtractActionItemsAsync(meetingNotes);

foreach (var item in result.ActionItems)
{
    Console.WriteLine($"- {item.Description}");
    Console.WriteLine($"  Assignee: {item.Assignee ?? "Unassigned"}");
    Console.WriteLine($"  Due: {item.DueDate ?? "Not specified"}");
    Console.WriteLine($"  Priority: {item.Priority}");
}
```

## Domain-Specific Summarization

### Legal Documents

```csharp
var legalSummary = await client.SummarizeLegalAsync(contract);
// Preserves legal terminology and key clauses
```

### Meeting Notes

```csharp
var meetingSummary = await client.SummarizeMeetingAsync(notes);
// Includes decisions, action items, and key points
```

### Academic Papers

```csharp
var abstract_ = await client.AbstractAsync(researchPaper);
// Academic abstract style with background, methods, results, conclusions
```

## Batch Summarization

```csharp
var articles = new[] { article1, article2, article3 };

var results = await client.SummarizeBatchAsync(articles);

Console.WriteLine($"Summarized: {results.SuccessCount} / {results.TotalCount}");
Console.WriteLine($"Total original words: {results.TotalOriginalWords}");
Console.WriteLine($"Total summary words: {results.TotalSummaryWords}");

foreach (var result in results.Results)
{
    Console.WriteLine($"Summary ({result.SummaryWordCount} words, {result.ReductionPercentage}% reduction):");
    Console.WriteLine(result.Summary);
}
```

## Compare Multiple Documents

```csharp
var documents = new[] { doc1, doc2, doc3 };

var comparison = await client.CompareAndSummarizeAsync(documents);

Console.WriteLine("Combined Summary:");
Console.WriteLine(comparison.CombinedSummary);

Console.WriteLine("\nCommon Themes:");
foreach (var theme in comparison.CommonThemes)
{
    Console.WriteLine($"- {theme}");
}

Console.WriteLine("\nKey Differences:");
foreach (var diff in comparison.KeyDifferences)
{
    Console.WriteLine($"- {diff}");
}
```

## Advanced Options

### Full Request Object

```csharp
var result = await client.SummarizeWithDetailsAsync(new SummaryRequest
{
    Text = longDocument,
    Style = SummaryStyle.Structured,
    Length = SummaryLength.Long,
    Domain = ContentDomain.Legal,
    Model = SummarizeModels.Claude35Sonnet,
    TargetAudience = "Senior executives",
    FocusAreas = new[] { "financial implications", "risk factors" },
    ExtractKeyPoints = true,
    ExtractStatistics = true,
    PreserveQuotes = true,
    OutputLanguage = "Spanish"
});

Console.WriteLine($"Summary ({result.ReductionPercentage}% reduction):");
Console.WriteLine(result.Summary);

if (result.KeyPoints != null)
{
    Console.WriteLine("\nKey Points:");
    foreach (var point in result.KeyPoints)
    {
        Console.WriteLine($"- {point}");
    }
}
```

### Custom Focus Areas

```csharp
var summary = await client.SummarizeWithFocusAsync(
    article,
    focusAreas: new[] { "technology trends", "market impact" });
```

### Target Audience

```csharp
// Simplify for non-experts
var simpleSummary = await client.SummarizeForAudienceAsync(
    technicalDoc,
    "non-technical stakeholders");

// Detailed for experts
var expertSummary = await client.SummarizeForAudienceAsync(
    technicalDoc,
    "software architects");
```

## Using Different AI Models

```csharp
// Use Claude for nuanced summarization
var options = new SummarizeOptions
{
    ApiKey = "your-api-key",
    DefaultModel = SummarizeModels.Claude35Sonnet
};
var client = new SummarizeClient(options);

// Or specify per-request
var request = new SummaryRequest
{
    Text = longDocument,
    Model = SummarizeModels.Gpt4o
};
```

Available models include:
- `SummarizeModels.Gpt4o` - Best quality (default)
- `SummarizeModels.Gpt4oMini` - Fast and affordable
- `SummarizeModels.Claude35Sonnet` - Excellent for long documents
- `SummarizeModels.Claude3Opus` - Highest quality Claude model
- `SummarizeModels.Gemini15Pro` - Great for research papers
- `SummarizeModels.Llama3170B` - Open source option

## Dependency Injection

### ASP.NET Core

```csharp
// In Program.cs
builder.Services.AddForeverToolsSummarize("your-api-key");

// Or with configuration
builder.Services.AddForeverToolsSummarize(options =>
{
    options.ApiKey = "your-api-key";
    options.DefaultModel = SummarizeModels.Gpt4oMini;
    options.DefaultStyle = SummaryStyle.BulletPoints;
    options.DefaultLength = SummaryLength.Medium;
});
```

### From Configuration

```json
// appsettings.json
{
    "Summarize": {
        "ApiKey": "your-api-key",
        "DefaultModel": "gpt-4o",
        "DefaultStyle": "Paragraph",
        "DefaultLength": "Medium"
    }
}
```

```csharp
builder.Services.AddForeverToolsSummarize(builder.Configuration);
```

### Using in Services

```csharp
public class ContentService
{
    private readonly SummarizeClient _summarizer;

    public ContentService(SummarizeClient summarizer)
    {
        _summarizer = summarizer;
    }

    public async Task<string> GetArticleSummaryAsync(string articleUrl)
    {
        var content = await FetchArticleContent(articleUrl);
        return await _summarizer.SummarizeAsync(content);
    }
}
```

## Content Domains

The package supports specialized summarization for different content types:

| Domain | Best For |
|--------|----------|
| `General` | Auto-detect (default) |
| `News` | News articles |
| `Academic` | Research papers |
| `Legal` | Contracts, legal documents |
| `Technical` | Documentation |
| `Business` | Reports, memos |
| `Medical` | Healthcare content |
| `Financial` | Financial reports |
| `Meeting` | Meeting notes |
| `Book` | Long-form content |
| `Email` | Email threads |
| `Social` | Social media |

## Error Handling

```csharp
try
{
    var summary = await client.SummarizeAsync(text);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"API error: {ex.Message}");
}
```

## Best Practices

1. **Choose the right style**: Use bullet points for scannable content, paragraphs for narrative
2. **Match length to purpose**: TL;DR for quick overview, detailed for comprehensive understanding
3. **Specify domain**: Legal, medical, and technical content benefits from domain hints
4. **Use focus areas**: When you need specific topics emphasized
5. **Batch for efficiency**: Use `SummarizeBatchAsync` for multiple documents
6. **Reuse the client**: Create one `SummarizeClient` and reuse it

## Other ForeverTools Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E, Stable Diffusion) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.APILayer** | IP geolocation, currency exchange, phone & email validation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.APILayer.svg)](https://www.nuget.org/packages/ForeverTools.APILayer) |
| **ForeverTools.Apify** | Web scraping platform with 1,600+ actors (Amazon, Google, Instagram, Twitter) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Apify.svg)](https://www.nuget.org/packages/ForeverTools.Apify) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.ImageGen** | AI image generation with social media presets (DALL-E, Flux, SD) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ImageGen.svg)](https://www.nuget.org/packages/ForeverTools.ImageGen) |
| **ForeverTools.OCR** | AI-powered OCR using GPT-4 Vision, Claude 3, and Gemini | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.OCR.svg)](https://www.nuget.org/packages/ForeverTools.OCR) |
| **ForeverTools.Postmark** | Transactional email sending with templates and tracking | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |
| **ForeverTools.Proxy** | Premium proxy rotation with BrightData (Residential, ISP, Mobile) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Proxy.svg)](https://www.nuget.org/packages/ForeverTools.Proxy) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation and CAPTCHA bypass | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |
| **ForeverTools.STT** | Speech-to-Text using Whisper (transcription, subtitles, language detection) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.STT.svg)](https://www.nuget.org/packages/ForeverTools.STT) |
| **ForeverTools.Translate** | AI-powered translation with 100+ languages (GPT-4, Claude) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Translate.svg)](https://www.nuget.org/packages/ForeverTools.Translate) |
| **ForeverTools.CodeGen** | AI-powered code generation, refactoring, and explanation | *Coming soon* |
| **ForeverTools.ContentMod** | AI content moderation — detect toxicity, hate speech, adult content | *Coming soon* |
| **ForeverTools.EmailAI** | AI email composition, reply drafting, summarization, and classification | *Coming soon* |
| **ForeverTools.InvoiceParser** | AI-powered invoice and receipt data extraction | *Coming soon* |
| **ForeverTools.Sentiment** | AI sentiment analysis with emotion detection | *Coming soon* |
| **ForeverTools.TTS** | Text-to-Speech synthesis with 30+ voices | *Coming soon* |

## License

MIT License - see LICENSE file for details.

## Links

- [GitHub Organization](https://github.com/ForeverTools)
- [NuGet Profile](https://www.nuget.org/profiles/ForeverTools)
