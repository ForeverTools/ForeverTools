# ForeverTools.AIML

A unified .NET client for the [AI/ML API](https://aimlapi.com?via=forevertools) - access 400+ AI models including GPT-4, Claude, Llama, Gemini, DALL-E, Stable Diffusion, and more through a single API.

## Features

- **400+ AI Models** - GPT-4, Claude, Llama, Gemini, Mistral, DeepSeek, and more
- **Chat Completions** - Conversational AI with streaming support
- **Image Generation** - DALL-E 3, Stable Diffusion, Flux models
- **Embeddings** - Text embeddings for RAG and semantic search
- **Audio** - Text-to-speech and speech-to-text (Whisper)
- **Multi-targeting** - Supports .NET 8.0, .NET 6.0, and .NET Standard 2.0
- **Async/await** - Full async support with cancellation tokens
- **Dependency Injection** - Built-in support for Microsoft.Extensions.DependencyInjection

## Installation

```bash
dotnet add package ForeverTools.AIML
```

## Quick Start

Get your API key at [aimlapi.com](https://aimlapi.com?via=forevertools).

```csharp
using ForeverTools.AIML;

// Create client
var client = new AimlApiClient("your-api-key");

// Simple chat
var response = await client.ChatAsync("What is the capital of France?");
Console.WriteLine(response); // "The capital of France is Paris."

// Generate an image
var imageUrl = await client.GenerateImageAsync("A sunset over mountains");
Console.WriteLine(imageUrl);

// Get embeddings for RAG/search
var vector = await client.EmbedAsync("Hello world");
Console.WriteLine($"Vector dimension: {vector.Length}");
```

## Dependency Injection

```csharp
// In Program.cs or Startup.cs
services.AddForeverToolsAiml("your-api-key");

// Or from configuration
services.AddForeverToolsAiml(configuration);
```

appsettings.json:
```json
{
  "AIML": {
    "ApiKey": "your-api-key",
    "DefaultChatModel": "gpt-4o",
    "DefaultImageModel": "dall-e-3",
    "DefaultEmbeddingModel": "text-embedding-3-small"
  }
}
```

Inject and use:
```csharp
public class MyService
{
    private readonly AimlApiClient _client;

    public MyService(AimlApiClient client)
    {
        _client = client;
    }

    public async Task<string> GetAIResponse(string prompt)
    {
        return await _client.ChatAsync(prompt);
    }
}
```

## Available Models

### Chat Models

```csharp
// OpenAI
AimlModels.Chat.Gpt4o          // "gpt-4o"
AimlModels.Chat.Gpt4oMini      // "gpt-4o-mini"
AimlModels.Chat.Gpt4Turbo      // "gpt-4-turbo"
AimlModels.Chat.O1Preview      // "o1-preview"

// Anthropic Claude
AimlModels.Chat.Claude35Sonnet // "claude-3-5-sonnet-20241022"
AimlModels.Chat.Claude3Opus    // "claude-3-opus-20240229"
AimlModels.Chat.Claude3Sonnet  // "claude-3-sonnet-20240229"

// Google Gemini
AimlModels.Chat.Gemini15Pro    // "gemini-1.5-pro"
AimlModels.Chat.Gemini15Flash  // "gemini-1.5-flash"

// Meta Llama
AimlModels.Chat.Llama31405B    // "meta-llama/Meta-Llama-3.1-405B-Instruct-Turbo"
AimlModels.Chat.Llama3170B     // "meta-llama/Meta-Llama-3.1-70B-Instruct-Turbo"

// Mistral
AimlModels.Chat.MistralLarge   // "mistral-large-latest"
AimlModels.Chat.Mixtral8x7B    // "mistralai/Mixtral-8x7B-Instruct-v0.1"

// DeepSeek
AimlModels.Chat.DeepSeekR1     // "deepseek-r1"
AimlModels.Chat.DeepSeekChat   // "deepseek-chat"
AimlModels.Chat.DeepSeekCoder  // "deepseek-coder"
```

### Image Models

```csharp
AimlModels.Image.DallE3              // "dall-e-3"
AimlModels.Image.StableDiffusionXL   // "stable-diffusion-xl-1024-v1-0"
AimlModels.Image.StableDiffusion3    // "stable-diffusion-v3-medium"
AimlModels.Image.FluxPro             // "flux-pro"
AimlModels.Image.FluxSchnell         // "flux-schnell"
```

### Embedding Models

```csharp
AimlModels.Embedding.TextEmbedding3Large  // "text-embedding-3-large"
AimlModels.Embedding.TextEmbedding3Small  // "text-embedding-3-small"
AimlModels.Embedding.TextEmbeddingAda002  // "text-embedding-ada-002"
```

## API Reference

### Chat Completions

```csharp
// Simple chat
var response = await client.ChatAsync("Hello!", AimlModels.Chat.Gpt4o);

// Full control with messages
var messages = new List<ChatMessage>
{
    new SystemChatMessage("You are a helpful assistant."),
    new UserChatMessage("What is 2+2?")
};
var completion = await client.CompleteChatAsync(messages, model: AimlModels.Chat.Claude35Sonnet);
Console.WriteLine(completion.Content[0].Text);

// Streaming
await foreach (var update in client.StreamChatAsync(messages))
{
    Console.Write(update.ContentUpdate[0].Text);
}
```

### Image Generation

```csharp
// Simple generation
var imageUrl = await client.GenerateImageAsync("A cat wearing a hat");

// With specific model
var fluxImage = await client.GenerateImageAsync(
    "Cyberpunk city at night",
    model: AimlModels.Image.FluxPro
);

// Multiple images
var images = await client.GenerateImagesAsync("Mountain landscape", count: 4);
foreach (var img in images)
{
    Console.WriteLine(img.ImageUri);
}
```

### Embeddings

```csharp
// Single text
var vector = await client.EmbedAsync("Hello world");
Console.WriteLine($"Dimensions: {vector.Length}");

// Multiple texts (batch)
var vectors = await client.EmbedManyAsync(new[] { "Hello", "World", "Test" });
foreach (var embedding in vectors)
{
    Console.WriteLine($"Index {embedding.Index}: {embedding.ToFloats().Length} dimensions");
}
```

### Audio

```csharp
// Text-to-Speech
var audioData = await client.TextToSpeechAsync("Hello, how are you?", voice: "nova");
await File.WriteAllBytesAsync("output.mp3", audioData.ToArray());

// Speech-to-Text (Transcription)
using var audioFile = File.OpenRead("recording.mp3");
var transcript = await client.TranscribeAsync(audioFile, "recording.mp3");
Console.WriteLine(transcript);
```

## Environment Variables

```csharp
// Uses AIML_API_KEY by default
var client = AimlApiClient.FromEnvironment();

// Or specify a custom variable
var client = AimlApiClient.FromEnvironment("MY_API_KEY");
```

## Error Handling

```csharp
try
{
    var response = await client.ChatAsync("Hello!");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid argument: {ex.Message}");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"API error: {ex.Message}");
}
```

## Other ForeverTools Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.APILayer** | IP geolocation, currency exchange, phone & email validation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.APILayer.svg)](https://www.nuget.org/packages/ForeverTools.APILayer) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.ImageGen** | AI image generation with social media presets (DALL-E, Flux, SD) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ImageGen.svg)](https://www.nuget.org/packages/ForeverTools.ImageGen) |
| **ForeverTools.OCR** | AI-powered OCR using GPT-4 Vision, Claude 3, and Gemini | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.OCR.svg)](https://www.nuget.org/packages/ForeverTools.OCR) |
| **ForeverTools.Postmark** | Transactional email sending with templates and tracking | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation and CAPTCHA bypass | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |

## License

MIT License - see LICENSE file for details.

## Links

- [Get API Key](https://aimlapi.com?via=forevertools)
- [API Documentation](https://docs.aimlapi.com)
- [GitHub Repository](https://github.com/ForeverTools/ForeverTools)
- [NuGet Package](https://www.nuget.org/packages/ForeverTools.AIML)
