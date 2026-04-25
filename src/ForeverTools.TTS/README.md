# ForeverTools.TTS

Text-to-speech (TTS) for .NET — powered by [AI/ML API](https://aimlapi.com?via=forevertools).

Convert any text to natural-sounding speech in multiple voices and formats. Save to file, get raw bytes, or stream audio — all with a single async call.

> **Get your API key:** [https://aimlapi.com?via=forevertools](https://aimlapi.com?via=forevertools)
> Sign up via this link and earn a **30% recurring commission** if you refer others.

---

## Quick start

### 1. Save audio to a file

```csharp
using ForeverTools.TTS;

var client = new TtsClient("your-api-key");
await client.SaveToFileAsync("Hello, world!", "output.mp3");
```

### 2. Get audio as bytes

```csharp
var client = new TtsClient("your-api-key");
byte[] audio = await client.SynthesizeAsync("Good morning!", new TtsOptions
{
    ApiKey  = "your-api-key",
    Voice   = TtsVoices.Nova,
    Model   = TtsModels.Tts1Hd,
    Format  = TtsFormat.Wav,
    Speed   = 1.1f
});
```

### 3. Dependency injection (ASP.NET Core)

```csharp
// Program.cs
builder.Services.AddTtsClient("your-api-key");

// Or with full options:
builder.Services.AddTtsClient(options =>
{
    options.ApiKey = "your-api-key";
    options.Voice  = TtsVoices.Onyx;
    options.Model  = TtsModels.Tts1Hd;
    options.Format = TtsFormat.Mp3;
});

// In your controller / service:
public class AudioController(TtsClient tts)
{
    public async Task<IActionResult> Speak(string text)
    {
        var audio = await tts.SynthesizeAsync(text);
        return File(audio, "audio/mpeg");
    }
}
```

---

## Voices

| Voice | Description |
|---|---|
| `alloy` | Versatile, neutral voice. Good general-purpose choice. |
| `echo` | Clear, measured masculine voice. |
| `fable` | Warm, expressive British-accented voice. |
| `onyx` | Deep, authoritative masculine voice. |
| `nova` | Bright, friendly feminine voice. |
| `shimmer` | Soft, thoughtful feminine voice. |

Use the `TtsVoices` constants to avoid typos: `TtsVoices.Nova`, `TtsVoices.Onyx`, etc.

---

## Audio formats

| Format | MIME type | Notes |
|---|---|---|
| `Mp3` | `audio/mpeg` | Default. Best compatibility. |
| `Opus` | `audio/opus` | Low latency, good for streaming. |
| `Aac` | `audio/aac` | Efficient lossy compression. |
| `Flac` | `audio/flac` | Lossless — maximum quality. |
| `Wav` | `audio/wav` | Uncompressed, widest compatibility. |
| `Pcm` | `audio/pcm` | Raw PCM data, no container. |

---

## Powered by AI/ML API

AI/ML API provides access to 400+ AI models through a single OpenAI-compatible endpoint.
Get your key at [https://aimlapi.com?via=forevertools](https://aimlapi.com?via=forevertools).
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
| **ForeverTools.Summarize** | AI-powered text summarization (TL;DR, bullet points, executive summaries) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Summarize.svg)](https://www.nuget.org/packages/ForeverTools.Summarize) |
| **ForeverTools.Translate** | AI-powered translation with 100+ languages (GPT-4, Claude) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Translate.svg)](https://www.nuget.org/packages/ForeverTools.Translate) |
| **ForeverTools.CodeGen** | AI-powered code generation, refactoring, and explanation | *Coming soon* |
| **ForeverTools.ContentMod** | AI content moderation — detect toxicity, hate speech, adult content | *Coming soon* |
| **ForeverTools.EmailAI** | AI email composition, reply drafting, summarization, and classification | *Coming soon* |
| **ForeverTools.InvoiceParser** | AI-powered invoice and receipt data extraction | *Coming soon* |
| **ForeverTools.Sentiment** | AI sentiment analysis with emotion detection | *Coming soon* |

## License

MIT License - see LICENSE file for details.

## Links

- [GitHub Organization](https://github.com/ForeverTools)
- [NuGet Profile](https://www.nuget.org/profiles/ForeverTools)
