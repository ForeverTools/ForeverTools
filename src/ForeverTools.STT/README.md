# ForeverTools.STT

Speech-to-Text for .NET using Whisper, powered by AI/ML API. Transcribe audio files, generate subtitles, detect languages, and get word-level timestamps.

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.STT.svg)](https://www.nuget.org/packages/ForeverTools.STT/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ForeverTools.STT.svg)](https://www.nuget.org/packages/ForeverTools.STT/)

## Features

- **Whisper Models**: Access OpenAI Whisper and Whisper Large V3
- **Multiple Input Sources**: Files, bytes, streams, or URLs
- **Subtitle Generation**: Export to SRT or VTT format
- **Language Detection**: Auto-detect or specify source language
- **Timestamps**: Get segment and word-level timing
- **Batch Processing**: Transcribe multiple files
- **Easy Integration**: Simple API with dependency injection support

## Getting Your API Key

This package uses the [AI/ML API](https://aimlapi.com?via=forevertools) which provides access to Whisper and 400+ AI models.

1. Sign up at [aimlapi.com](https://aimlapi.com?via=forevertools)
2. Get your API key from the dashboard
3. Start transcribing!

## Installation

```bash
dotnet add package ForeverTools.STT
```

## Quick Start

### Basic Transcription

```csharp
using ForeverTools.STT;

// Create client with your API key
var client = new SpeechToTextClient("your-api-key");

// Transcribe an audio file
var text = await client.TranscribeAsync("meeting.mp3");
Console.WriteLine(text);
```

### Using Environment Variables

```csharp
// Set AIML_API_KEY or STT_API_KEY environment variable
var client = SpeechToTextClient.FromEnvironment();
```

## Transcription Examples

### From File

```csharp
// Simple - just get the text
var text = await client.TranscribeAsync("podcast.mp3");

// Detailed - get timestamps and metadata
var result = await client.TranscribeWithDetailsAsync("podcast.mp3");
Console.WriteLine($"Text: {result.Text}");
Console.WriteLine($"Language: {result.Language}");
Console.WriteLine($"Duration: {result.Duration}");

foreach (var segment in result.Segments)
{
    Console.WriteLine($"[{segment.Start:mm\\:ss} - {segment.End:mm\\:ss}] {segment.Text}");
}
```

### From Bytes or Stream

```csharp
// From bytes
byte[] audioData = File.ReadAllBytes("audio.mp3");
var text = await client.TranscribeAsync(audioData, "audio.mp3");

// From stream
using var stream = File.OpenRead("audio.mp3");
var text = await client.TranscribeAsync(stream, "audio.mp3");
```

### From URL

```csharp
// Transcribe audio from a URL
var text = await client.TranscribeFromUrlAsync("https://example.com/podcast.mp3");
```

## Subtitle Generation

### Generate SRT Subtitles

```csharp
// Get SRT format directly
var srt = await client.TranscribeToSrtAsync("video.mp3");
File.WriteAllText("video.srt", srt);

// Output:
// 1
// 00:00:00,000 --> 00:00:05,230
// Welcome to the podcast.
//
// 2
// 00:00:05,230 --> 00:00:10,500
// Today we'll be discussing...
```

### Generate VTT Subtitles

```csharp
// Get WebVTT format
var vtt = await client.TranscribeToVttAsync("video.mp3");
File.WriteAllText("video.vtt", vtt);

// Output:
// WEBVTT
//
// 00:00:00.000 --> 00:00:05.230
// Welcome to the podcast.
//
// 00:00:05.230 --> 00:00:10.500
// Today we'll be discussing...
```

### Convert Existing Segments

```csharp
var result = await client.TranscribeWithDetailsAsync("audio.mp3");

// Generate subtitles from segments
var srt = SpeechToTextClient.GenerateSrt(result.Segments);
var vtt = SpeechToTextClient.GenerateVtt(result.Segments);
```

## Language Options

### Auto-Detect Language

```csharp
// Language is auto-detected by default
var result = await client.TranscribeWithDetailsAsync("audio.mp3");
Console.WriteLine($"Detected language: {result.Language}");
```

### Specify Language

```csharp
var result = await client.TranscribeWithDetailsAsync(new TranscriptionRequest
{
    FilePath = "audio.mp3",
    Language = TranscriptionLanguages.Spanish
});
```

### Detect Language Only

```csharp
var detection = await client.DetectLanguageAsync("audio.mp3");
Console.WriteLine($"Language: {detection.LanguageName} ({detection.LanguageCode})");
```

## Advanced Options

### Full Request Configuration

```csharp
var result = await client.TranscribeWithDetailsAsync(new TranscriptionRequest
{
    FilePath = "meeting.mp3",
    Model = SttModels.WhisperLargeV3,      // Use larger model for accuracy
    Language = TranscriptionLanguages.English,
    Temperature = 0.2f,                      // Lower = more deterministic
    Prompt = "Meeting about Q4 financials",  // Guide vocabulary
    ResponseFormat = ResponseFormats.VerboseJson
});
```

### Different Models

```csharp
// Fast model (default)
var options = new SpeechToTextOptions
{
    ApiKey = "your-api-key",
    DefaultModel = SttModels.Whisper1
};

// High accuracy model
var options = new SpeechToTextOptions
{
    ApiKey = "your-api-key",
    DefaultModel = SttModels.WhisperLargeV3
};

var client = new SpeechToTextClient(options);
```

## Batch Transcription

```csharp
var files = new[] { "meeting1.mp3", "meeting2.mp3", "meeting3.mp3" };

var results = await client.TranscribeBatchAsync(files);

foreach (var result in results)
{
    Console.WriteLine($"Duration: {result.Duration}, Text: {result.Text.Substring(0, 100)}...");
}
```

## Dependency Injection

### ASP.NET Core

```csharp
// In Program.cs
builder.Services.AddForeverToolsSTT("your-api-key");

// Or with configuration
builder.Services.AddForeverToolsSTT(options =>
{
    options.ApiKey = "your-api-key";
    options.DefaultModel = SttModels.WhisperLargeV3;
    options.DefaultLanguage = "en";
});
```

### From Configuration

```json
// appsettings.json
{
    "SpeechToText": {
        "ApiKey": "your-api-key",
        "DefaultModel": "whisper-1",
        "DefaultLanguage": "en",
        "Temperature": 0
    }
}
```

```csharp
builder.Services.AddForeverToolsSTT(builder.Configuration);
```

### Using in Services

```csharp
public class TranscriptionService
{
    private readonly SpeechToTextClient _stt;

    public TranscriptionService(SpeechToTextClient stt)
    {
        _stt = stt;
    }

    public async Task<string> TranscribeMeetingAsync(string filePath)
    {
        return await _stt.TranscribeAsync(filePath);
    }
}
```

## Supported Audio Formats

| Format | Extension | MIME Type |
|--------|-----------|-----------|
| MP3 | .mp3 | audio/mpeg |
| WAV | .wav | audio/wav |
| M4A | .m4a | audio/mp4 |
| WebM | .webm | audio/webm |
| FLAC | .flac | audio/flac |
| OGG | .ogg | audio/ogg |
| MP4 | .mp4 | audio/mp4 |

## Available Models

| Model | Best For | Speed | Accuracy |
|-------|----------|-------|----------|
| `whisper-1` | General use | Fast | Good |
| `whisper-large-v3` | High accuracy | Slower | Excellent |
| `whisper-large-v3-turbo` | Balanced | Medium | Very Good |

## Error Handling

```csharp
try
{
    var text = await client.TranscribeAsync("audio.mp3");
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"File not found: {ex.FileName}");
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

1. **Choose the right model**: Use `whisper-1` for speed, `whisper-large-v3` for accuracy
2. **Specify language**: If you know the language, specify it for better results
3. **Use prompts**: For domain-specific vocabulary, provide context via prompt
4. **Handle large files**: For files over 25MB, consider splitting audio
5. **Reuse the client**: Create one `SpeechToTextClient` and reuse it

## Other ForeverTools Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.APILayer** | IP geolocation, currency exchange, phone & email validation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.APILayer.svg)](https://www.nuget.org/packages/ForeverTools.APILayer) |
| **ForeverTools.Apify** | Web scraping platform with 1,600+ actors (Amazon, Google, Instagram, Twitter) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Apify.svg)](https://www.nuget.org/packages/ForeverTools.Apify) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.ImageGen** | AI image generation with social media presets (DALL-E, Flux, SD) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ImageGen.svg)](https://www.nuget.org/packages/ForeverTools.ImageGen) |
| **ForeverTools.OCR** | AI-powered OCR using GPT-4 Vision, Claude 3, and Gemini | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.OCR.svg)](https://www.nuget.org/packages/ForeverTools.OCR) |
| **ForeverTools.Postmark** | Transactional email sending with templates and tracking | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |
| **ForeverTools.Proxy** | Premium proxy rotation with BrightData (Residential, ISP, Mobile) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Proxy.svg)](https://www.nuget.org/packages/ForeverTools.Proxy) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation and CAPTCHA bypass | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |
| **ForeverTools.Summarize** | AI-powered text summarization (TL;DR, bullet points, executive summaries) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Summarize.svg)](https://www.nuget.org/packages/ForeverTools.Summarize) |
| **ForeverTools.Translate** | AI-powered translation with 100+ languages (GPT-4, Claude) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Translate.svg)](https://www.nuget.org/packages/ForeverTools.Translate) |

## Support

- [GitHub Issues](https://github.com/ForeverTools/ForeverTools/issues)
- [AI/ML API Documentation](https://docs.aimlapi.com/)

## License

MIT License - see LICENSE file for details.
