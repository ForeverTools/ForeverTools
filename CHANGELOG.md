# Changelog

All notable changes to ForeverTools packages will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial release of ForeverTools.AIML

---

## ForeverTools.AIML

### [1.0.0] - 2025-12-08

#### Added
- `AimlApiClient` - Main client for accessing 400+ AI models
- Chat completions with `ChatAsync`, `CompleteChatAsync`, and `StreamChatAsync`
- Image generation with `GenerateImageAsync`
- Text embeddings with `EmbedAsync` and `EmbedManyAsync`
- Text-to-speech with `TextToSpeechAsync`
- Speech-to-text with `TranscribeAsync`
- ASP.NET Core dependency injection via `AddForeverToolsAiml()`
- Configuration binding from `appsettings.json`
- `AimlModels` constants for 50+ popular models:
  - Chat: GPT-4o, Claude 3.5 Sonnet, Gemini 1.5 Pro, Llama 3.1, Mistral, DeepSeek R1
  - Image: DALL-E 3, Stable Diffusion XL, Flux Pro
  - Embedding: text-embedding-3-large/small, ada-002
  - Audio: TTS-1, Whisper
  - Video: Sora, Runway Gen3, Kling AI
  - Code: CodeLlama, DeepSeek Coder, StarCoder2
- Multi-target support: .NET 8.0, .NET 6.0, .NET Standard 2.0
- Full XML documentation for IntelliSense
- Static factory methods: `FromEnvironment()`, `CreateChatClient()`, `CreateEmbeddingClient()`, `CreateImageClient()`

#### Technical Details
- Built on OpenAI SDK v2.1.0 for maximum compatibility
- OpenAI-compatible API endpoint (api.aimlapi.com)
- Async/await throughout
- Nullable reference types enabled
- Symbol packages (.snupkg) for debugging

---

## Future Packages (Planned)

### ForeverTools.Captcha
Multi-provider captcha solving (Anti-Captcha, 2Captcha, CapSolver)

### ForeverTools.Proxy
Proxy service abstraction (ScraperAPI, BrightData, SmartProxy)

### ForeverTools.SMS
SMS API wrapper (BulkGate, Textmagic)

### ForeverTools.Email
Email validation and sending (Postmark, Mailgun)

[Unreleased]: https://github.com/ForeverTools/ForeverTools.AIML/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/ForeverTools/ForeverTools.AIML/releases/tag/v1.0.0
