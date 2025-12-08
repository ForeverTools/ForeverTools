# ForeverTools

.NET wrapper libraries for popular third-party APIs. Clean interfaces, dependency injection ready, available on NuGet.

## Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| [ForeverTools.AIML](docs/AIML_README.md) | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E, Stable Diffusion) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |

## Coming Soon

- **ForeverTools.Captcha** - Multi-provider captcha solving (Anti-Captcha, 2Captcha, CapSolver)
- **ForeverTools.Proxy** - Proxy rotation (BrightData, SmartProxy, ScraperAPI)
- **ForeverTools.SMS** - SMS & messaging (BulkGate, Textmagic)

## Installation

All packages are available on NuGet:

```bash
# AI/ML (400+ AI models)
dotnet add package ForeverTools.AIML
```

## Quick Examples

### AI/ML API
```csharp
using ForeverTools.AIML;

var client = new AimlApiClient("your-api-key");

// Chat with GPT-4, Claude, Llama, etc.
var response = await client.ChatAsync("What is the capital of France?");

// Generate images with DALL-E, Stable Diffusion, Flux
var imageUrl = await client.GenerateImageAsync("A sunset over mountains");

// Get embeddings for RAG/search
var vector = await client.EmbedAsync("Hello world");
```

### ASP.NET Core
```csharp
// Program.cs
builder.Services.AddForeverToolsAiml("your-api-key");

// Or from configuration
builder.Services.AddForeverToolsAiml(builder.Configuration);
```

## Features

- **Modern .NET** - Targets .NET 8, .NET 6, and .NET Standard 2.0
- **Async/await** - All operations are async
- **Dependency Injection** - Built-in `IServiceCollection` extensions
- **IntelliSense** - Full XML documentation
- **Type-safe** - Model constants, no magic strings

## Links

- [GitHub Organization](https://github.com/ForeverTools)
- [NuGet Profile](https://www.nuget.org/profiles/ForeverTools)

## License

MIT
