# ForeverTools.Translate

AI-powered translation for .NET using GPT-4, Claude, Llama and 400+ AI models. Translate text between 100+ languages with automatic language detection, batch translation, and context-aware results.

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.Translate.svg)](https://www.nuget.org/packages/ForeverTools.Translate/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ForeverTools.Translate.svg)](https://www.nuget.org/packages/ForeverTools.Translate/)

## Features

- **100+ Languages**: Translate between any language pair
- **Auto Language Detection**: Automatically detect source language
- **Multiple AI Models**: GPT-4, Claude, Llama, Gemini, Mistral, and more
- **Batch Translation**: Translate multiple texts efficiently
- **Context-Aware**: Provide context for more accurate translations
- **Custom Glossaries**: Ensure consistent terminology
- **Multiple Styles**: Natural, formal, casual, technical, and more
- **Easy Integration**: Simple API with dependency injection support

## Getting Your API Key

This package uses the [AI/ML API](https://aimlapi.com?via=forevertools) which provides access to 400+ AI models including GPT-4, Claude, Llama, and more.

1. Sign up at [aimlapi.com](https://aimlapi.com?via=forevertools)
2. Get your API key from the dashboard
3. Start translating!

## Installation

```bash
dotnet add package ForeverTools.Translate
```

## Quick Start

### Basic Translation

```csharp
using ForeverTools.Translate;

// Create client with your API key
var client = new TranslationClient("your-api-key");

// Translate to Spanish (auto-detect source language)
var spanish = await client.TranslateAsync("Hello, how are you?", "es");
// Output: "Hola, ¿cómo estás?"

// Translate to English
var english = await client.TranslateToEnglishAsync("Bonjour le monde");
// Output: "Hello world"
```

### Using Environment Variables

```csharp
// Set AIML_API_KEY or TRANSLATION_API_KEY environment variable
var client = TranslationClient.FromEnvironment();
```

## Language Detection

```csharp
var result = await client.DetectLanguageAsync("Bonjour, comment allez-vous?");

Console.WriteLine($"Language: {result.LanguageName}");  // French
Console.WriteLine($"Code: {result.LanguageCode}");      // fr
Console.WriteLine($"Confidence: {result.Confidence}");  // 0.98
```

## Batch Translation

```csharp
// Translate multiple texts at once
var texts = new[] { "Hello", "Goodbye", "Thank you" };
var results = await client.TranslateBatchAsync(texts, "es");

foreach (var result in results.Results)
{
    Console.WriteLine($"{result.OriginalText} → {result.TranslatedText}");
}
// Hello → Hola
// Goodbye → Adiós
// Thank you → Gracias
```

### Translate to Multiple Languages

```csharp
// Translate one text to multiple languages simultaneously
var languages = new[] { "es", "fr", "de", "ja" };
var results = await client.TranslateToMultipleLanguagesAsync("Hello world", languages);

foreach (var result in results)
{
    Console.WriteLine($"{result.TargetLanguage}: {result.TranslatedText}");
}
// es: Hola mundo
// fr: Bonjour le monde
// de: Hallo Welt
// ja: こんにちは世界
```

## Translation Styles

```csharp
// Formal (business documents)
var formal = await client.TranslateWithStyleAsync(
    "Hey, what's up?",
    "es",
    TranslationStyle.Formal);
// "Buenos días, ¿cómo se encuentra?"

// Casual (conversational)
var casual = await client.TranslateWithStyleAsync(
    "Greetings, how do you do?",
    "es",
    TranslationStyle.Casual);
// "¡Hola! ¿Qué tal?"

// Technical (preserve terminology)
var technical = await client.TranslateWithStyleAsync(
    "The API returns a JSON response with pagination.",
    "de",
    TranslationStyle.Technical);
```

Available styles:
- `Natural` - Fluent, natural-sounding translation (default)
- `Formal` - Professional, business-appropriate
- `Casual` - Friendly, conversational
- `Technical` - Preserves technical terminology
- `Literal` - Stays close to source structure
- `Creative` - More freedom for adaptation

## Context-Aware Translation

```csharp
// Provide context for more accurate translation
var translation = await client.TranslateWithContextAsync(
    "Apple announced new products today.",
    "es",
    "This is a technology news article about the company Apple Inc.");
// "Apple anunció nuevos productos hoy." (not the fruit!)
```

## Custom Glossaries

```csharp
// Ensure consistent terminology
var glossary = new Dictionary<string, string>
{
    ["cloud"] = "la nube",
    ["server"] = "el servidor",
    ["API"] = "API"  // Keep as-is
};

var translation = await client.TranslateWithGlossaryAsync(
    "The cloud server exposes an API.",
    "es",
    glossary);
// "El servidor en la nube expone una API."
```

## Using Different AI Models

```csharp
// Use GPT-4 for highest quality
var options = new TranslationOptions
{
    ApiKey = "your-api-key",
    DefaultModel = TranslationModels.Gpt4o
};
var client = new TranslationClient(options);

// Or specify per-request
var request = new TranslationRequest
{
    Text = "Hello world",
    TargetLanguage = "ja",
    Model = TranslationModels.Claude35Sonnet
};
var result = await client.TranslateWithDetailsAsync(request);
```

Available models include:
- `TranslationModels.Gpt4o` - Best quality (default)
- `TranslationModels.Gpt4oMini` - Fast and affordable
- `TranslationModels.Claude35Sonnet` - Excellent for nuanced translation
- `TranslationModels.Gemini15Pro` - Google's latest
- `TranslationModels.Llama3170B` - Open source option
- `TranslationModels.Qwen72B` - Excellent for Asian languages

## Predefined Languages

```csharp
// Use predefined language constants
using ForeverTools.Translate;

var spanish = Languages.Spanish;      // es
var japanese = Languages.Japanese;    // ja
var chinese = Languages.ChineseSimplified;  // zh-CN

// Or use language codes directly
var result = await client.TranslateAsync("Hello", "es");
```

## Dependency Injection

### ASP.NET Core

```csharp
// In Program.cs
builder.Services.AddForeverToolsTranslation("your-api-key");

// Or with configuration
builder.Services.AddForeverToolsTranslation(options =>
{
    options.ApiKey = "your-api-key";
    options.DefaultModel = TranslationModels.Gpt4oMini;
    options.DefaultTargetLanguage = "es";
    options.Style = TranslationStyle.Formal;
});
```

### From Configuration

```json
// appsettings.json
{
    "Translation": {
        "ApiKey": "your-api-key",
        "DefaultModel": "gpt-4o",
        "DefaultTargetLanguage": "en",
        "Style": "Natural"
    }
}
```

```csharp
builder.Services.AddForeverToolsTranslation(builder.Configuration);
```

### Using in Services

```csharp
public class MyService
{
    private readonly TranslationClient _translator;

    public MyService(TranslationClient translator)
    {
        _translator = translator;
    }

    public async Task<string> TranslateContentAsync(string content, string language)
    {
        return await _translator.TranslateAsync(content, language);
    }
}
```

## Detailed Results

```csharp
var result = await client.TranslateWithDetailsAsync(
    "Bonjour le monde",
    null,  // Auto-detect source
    "en");

Console.WriteLine($"Original: {result.OriginalText}");
Console.WriteLine($"Translated: {result.TranslatedText}");
Console.WriteLine($"Source Language: {result.SourceLanguage}");
Console.WriteLine($"Target Language: {result.TargetLanguage}");
Console.WriteLine($"Model Used: {result.Model}");
Console.WriteLine($"Language Detected: {result.WasLanguageDetected}");
```

## Supported Languages

The package supports 50+ predefined languages including:

| Language | Code | Language | Code |
|----------|------|----------|------|
| English | en | Spanish | es |
| French | fr | German | de |
| Italian | it | Portuguese | pt |
| Russian | ru | Japanese | ja |
| Korean | ko | Chinese (Simplified) | zh-CN |
| Chinese (Traditional) | zh-TW | Arabic | ar |
| Hindi | hi | Vietnamese | vi |
| Thai | th | Turkish | tr |
| Dutch | nl | Polish | pl |
| Swedish | sv | Greek | el |

And many more! Any language can be used by passing its ISO 639-1 code.

## Error Handling

```csharp
try
{
    var translation = await client.TranslateAsync("Hello", "es");
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

1. **Reuse the client**: Create one `TranslationClient` and reuse it
2. **Use batch translation**: For multiple texts, use `TranslateBatchAsync`
3. **Provide context**: For ambiguous text, use `TranslateWithContextAsync`
4. **Choose the right model**: Use GPT-4o for quality, GPT-4o-mini for speed
5. **Use glossaries**: For consistent terminology across translations

## Other ForeverTools Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.APILayer** | IP geolocation, currency exchange, phone & email validation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.APILayer.svg)](https://www.nuget.org/packages/ForeverTools.APILayer) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.ImageGen** | AI image generation with social media presets (DALL-E, Flux, SD) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ImageGen.svg)](https://www.nuget.org/packages/ForeverTools.ImageGen) |
| **ForeverTools.OCR** | AI-powered OCR using GPT-4 Vision, Claude 3, and Gemini | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.OCR.svg)](https://www.nuget.org/packages/ForeverTools.OCR) |
| **ForeverTools.Postmark** | Transactional email sending with templates and tracking | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation and CAPTCHA bypass | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |
| **ForeverTools.Proxy** | Premium proxy rotation with BrightData (Residential, ISP, Mobile) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Proxy.svg)](https://www.nuget.org/packages/ForeverTools.Proxy) |

## Support

- [GitHub Issues](https://github.com/ForeverTools/ForeverTools/issues)
- [AI/ML API Documentation](https://docs.aimlapi.com/)

## License

MIT License - see LICENSE file for details.
