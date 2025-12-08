# ForeverTools.AIML

Access 400+ AI models through a single, unified .NET API. Built on [AI/ML API](https://aimlapi.com?via=forevertools) with an OpenAI-compatible interface.

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML)

## Features

- **400+ AI Models** - GPT-4, Claude, Llama, Gemini, Mistral, DeepSeek, and more
- **Image Generation** - DALL-E 3, Stable Diffusion, Flux, Midjourney-style models
- **Embeddings** - Text embeddings for RAG, search, and semantic analysis
- **Audio** - Text-to-speech and speech-to-text (Whisper)
- **Video Generation** - Sora, Runway, Kling AI
- **OpenAI Compatible** - Drop-in replacement using the official OpenAI SDK
- **ASP.NET Core Ready** - Built-in dependency injection support
- **Multi-Target** - Supports .NET 8, .NET 6, and .NET Standard 2.0

## Quick Start

### 1. Get Your API Key

Sign up at [aimlapi.com](https://aimlapi.com?via=forevertools) to get your API key.

### 2. Install the Package

```bash
dotnet add package ForeverTools.AIML
```

### 3. Start Using AI

```csharp
using ForeverTools.AIML;

// Simple chat
var client = new AimlApiClient("your-api-key");
var response = await client.ChatAsync("What is the capital of France?");
Console.WriteLine(response); // "The capital of France is Paris."

// Or use environment variable
var client = AimlApiClient.FromEnvironment(); // Reads AIML_API_KEY
```

## Usage Examples

### Chat Completions

```csharp
using ForeverTools.AIML;
using OpenAI.Chat;

var client = new AimlApiClient("your-api-key");

// Simple message
var answer = await client.ChatAsync("Explain quantum computing in simple terms");

// With specific model
var answer = await client.ChatAsync(
    "Write a haiku about programming",
    model: AimlModels.Chat.Claude35Sonnet
);

// Full control with messages
var messages = new List<ChatMessage>
{
    new SystemChatMessage("You are a helpful coding assistant."),
    new UserChatMessage("How do I read a file in C#?")
};

var completion = await client.CompleteChatAsync(messages);
Console.WriteLine(completion.Content[0].Text);
```

### Streaming Responses

```csharp
var messages = new[] { new UserChatMessage("Write a story about a robot") };

await foreach (var update in client.StreamChatAsync(messages))
{
    if (update.ContentUpdate.Count > 0)
    {
        Console.Write(update.ContentUpdate[0].Text);
    }
}
```

### Image Generation

```csharp
// Generate an image
var imageUrl = await client.GenerateImageAsync(
    "A futuristic city at sunset, digital art style"
);
Console.WriteLine(imageUrl);

// With specific model
var imageUrl = await client.GenerateImageAsync(
    "A cute robot reading a book",
    model: AimlModels.Image.FluxPro
);
```

### Embeddings

```csharp
// Single embedding
var vector = await client.EmbedAsync("Hello, world!");
Console.WriteLine($"Dimensions: {vector.Length}");

// Multiple embeddings
var texts = new[] { "First document", "Second document", "Third document" };
var embeddings = await client.EmbedManyAsync(texts);
```

### Text-to-Speech

```csharp
var audioData = await client.TextToSpeechAsync(
    "Hello! Welcome to ForeverTools.",
    voice: "nova"
);

await File.WriteAllBytesAsync("output.mp3", audioData.ToArray());
```

### Speech-to-Text

```csharp
using var audioFile = File.OpenRead("recording.mp3");
var transcript = await client.TranscribeAsync(audioFile, "recording.mp3");
Console.WriteLine(transcript);
```

## ASP.NET Core Integration

### Configuration

```csharp
// Program.cs
builder.Services.AddForeverToolsAiml("your-api-key");

// Or from configuration
builder.Services.AddForeverToolsAiml(builder.Configuration);
```

```json
// appsettings.json
{
  "AimlApi": {
    "ApiKey": "your-api-key",
    "DefaultChatModel": "gpt-4o",
    "DefaultImageModel": "dall-e-3"
  }
}
```

### Using in Controllers/Services

```csharp
public class ChatController : ControllerBase
{
    private readonly AimlApiClient _aiml;

    public ChatController(AimlApiClient aiml)
    {
        _aiml = aiml;
    }

    [HttpPost]
    public async Task<string> Chat([FromBody] string message)
    {
        return await _aiml.ChatAsync(message);
    }
}
```

## Available Models

### Chat Models

| Model | Constant |
|-------|----------|
| GPT-4o | `AimlModels.Chat.Gpt4o` |
| GPT-4o Mini | `AimlModels.Chat.Gpt4oMini` |
| Claude 3.5 Sonnet | `AimlModels.Chat.Claude35Sonnet` |
| Claude 3 Opus | `AimlModels.Chat.Claude3Opus` |
| Gemini 1.5 Pro | `AimlModels.Chat.Gemini15Pro` |
| Llama 3.1 405B | `AimlModels.Chat.Llama31405B` |
| Mistral Large | `AimlModels.Chat.MistralLarge` |
| DeepSeek R1 | `AimlModels.Chat.DeepSeekR1` |

### Image Models

| Model | Constant |
|-------|----------|
| DALL-E 3 | `AimlModels.Image.DallE3` |
| Stable Diffusion XL | `AimlModels.Image.StableDiffusionXL` |
| Flux Pro | `AimlModels.Image.FluxPro` |

See [AI/ML API Model Database](https://docs.aimlapi.com/api-references/model-database) for the complete list of 400+ models.

## Advanced Usage

### Direct OpenAI Client Access

```csharp
var client = new AimlApiClient("your-api-key");

// Get the underlying OpenAI client for advanced scenarios
var openAiClient = client.UnderlyingClient;

// Use any OpenAI SDK feature
var assistantClient = openAiClient.GetAssistantClient();
```

### Custom Configuration

```csharp
var options = new AimlApiOptions
{
    ApiKey = "your-api-key",
    DefaultChatModel = AimlModels.Chat.Claude35Sonnet,
    DefaultImageModel = AimlModels.Image.FluxPro,
    TimeoutSeconds = 120
};

var client = new AimlApiClient(options);
```

## Why ForeverTools.AIML?

| Feature | ForeverTools.AIML | Raw API Calls |
|---------|-------------------|---------------|
| 400+ models | Single package | Multiple SDKs |
| Type safety | Full IntelliSense | Manual JSON |
| DI support | Built-in | Manual setup |
| Model constants | Included | Magic strings |
| Async/await | Native | Manual |
| Multi-target | .NET 6/8 + Standard 2.0 | Framework-specific |

## Requirements

- .NET 8.0, .NET 6.0, or .NET Standard 2.0 compatible framework
- [AI/ML API key](https://aimlapi.com?via=forevertools)

## Links

- [Get API Key](https://aimlapi.com?via=forevertools)
- [API Documentation](https://docs.aimlapi.com/)
- [Model Database](https://docs.aimlapi.com/api-references/model-database)
- [GitHub Repository](https://github.com/ForeverTools/ForeverTools.AIML)

## License

MIT License - see [LICENSE](LICENSE) for details.
