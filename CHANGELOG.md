# Changelog

All notable changes to ForeverTools packages will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## ForeverTools.Translate

### [1.0.0] - 2025-12-13

#### Added
- `TranslationClient` - AI-powered translation using GPT-4, Claude, Llama and 400+ models
- Simple translation with `TranslateAsync` (auto-detect source language)
- Translate to English with `TranslateToEnglishAsync`
- Translate from English with `TranslateFromEnglishAsync`
- Detailed results with `TranslateWithDetailsAsync`
- Language detection with `DetectLanguageAsync`
- Batch translation with `TranslateBatchAsync`
- Multi-language translation with `TranslateToMultipleLanguagesAsync`
- Style-based translation with `TranslateWithStyleAsync` (Natural, Formal, Casual, Technical, Literal, Creative)
- Context-aware translation with `TranslateWithContextAsync`
- Custom glossaries with `TranslateWithGlossaryAsync`
- 50+ predefined languages via `Languages` class
- `TranslationModels` constants: GPT-4o, GPT-4o-mini, Claude 3.5 Sonnet, Gemini 1.5 Pro, Llama 3.1, Qwen
- ASP.NET Core dependency injection via `AddForeverToolsTranslation()`
- Configuration binding from `appsettings.json`
- Multi-target support: .NET 8.0, .NET 6.0, .NET Standard 2.0
- 38 unit tests
- Affiliate tracking: `via=forevertools` (30% recurring commission)

---

## ForeverTools.Proxy

### [1.0.0] - 2025-12-12

#### Added
- `BrightDataClient` - Premium proxy rotation with BrightData
- Create HTTP clients with `CreateHttpClient()`
- Geo-targeting with `CreateHttpClientForCountry()`
- Session management with `CreateSession()` for sticky IPs
- City and state targeting with `CreateHttpClientForCity()` and `CreateHttpClientForState()`
- ASN targeting with `CreateHttpClientForAsn()`
- Multiple proxy types: Residential, Datacenter, ISP, Mobile
- `ProxyTypes` constants for all supported proxy types
- `BrightDataSession` for maintaining same IP across requests
- ASP.NET Core dependency injection via `AddForeverToolsProxy()`
- Configuration binding from `appsettings.json`
- Multi-target support: .NET 8.0, .NET 6.0, .NET Standard 2.0
- 48 unit tests
- Affiliate tracking: BrightData partner links (50% + 15% recurring commission)

---

## ForeverTools.ScraperAPI

### [1.0.0] - 2025-12-09

#### Added
- `ScraperApiClient` - Main client for web scraping via ScraperAPI
- Simple scraping with `ScrapeAsync`
- JavaScript rendering with `ScrapeWithJavaScriptAsync`
- Geo-targeting with `ScrapeFromCountryAsync`
- Full request configuration via `ScrapeWithResponseAsync`
- Screenshot capture with `TakeScreenshotAsync` and `TakeScreenshotBytesAsync`
- Async job support: `SubmitAsyncJobAsync`, `GetAsyncJobStatusAsync`, `ScrapeAsyncAndWaitAsync`
- Account info with `GetAccountInfoAsync`
- ASP.NET Core dependency injection via `AddForeverToolsScraperApi()`
- Configuration binding from `appsettings.json`
- Request options: Premium proxies, Ultra-premium, Session stickiness, Auto-parse
- `DeviceTypes` constants: Desktop, Mobile
- `OutputFormats` constants: Html, Markdown, Text, Json, Csv
- Multi-target support: .NET 8.0, .NET 6.0, .NET Standard 2.0
- 32 unit tests
- Affiliate tracking: `fp_ref=chris88` (50% recurring commission)

---

## ForeverTools.Postmark

### [1.0.0] - 2025-12-09

#### Added
- `PostmarkClient` - Main client for Postmark transactional email API
- Single email sending with `SendEmailAsync`
- Batch sending (up to 500) with `SendBatchAsync`
- Template emails with `SendTemplateEmailAsync` and `SendTemplateBatchAsync`
- Delivery statistics with `GetDeliveryStatsAsync`
- Outbound stats with `GetOutboundStatsAsync`
- Bounce management: `GetBouncesAsync`, `ActivateBounceAsync`
- Server info with `GetServerAsync`
- ASP.NET Core dependency injection via `AddForeverToolsPostmark()`
- Configuration binding from `appsettings.json`
- `PostmarkEmail` model with full options (attachments, headers, metadata, tracking)
- `PostmarkTemplateEmail` for template-based sending
- `PostmarkAttachment.FromBytes()` with automatic MIME type detection
- `LinkTrackingOptions` constants: None, HtmlAndText, HtmlOnly, TextOnly
- `MessageStreams` constants: Outbound, Broadcast
- Multi-target support: .NET 8.0, .NET 6.0, .NET Standard 2.0
- 54 unit tests
- Affiliate tracking: `via=8ac781` (20% commission for 12 months)

---

## ForeverTools.Captcha

### [1.0.1] - 2025-12-08

#### Changed
- SEO optimization: Added 20 NuGet tags for better discoverability
- Enhanced package description with target keywords

### [1.0.0] - 2025-12-08

#### Added
- `CaptchaClient` - Unified client for multiple captcha solving providers
- Factory methods: `For2Captcha()`, `ForCapSolver()`, `ForAntiCaptcha()`
- reCAPTCHA v2 solving with `SolveReCaptchaV2Async`
- reCAPTCHA v3 solving with `SolveReCaptchaV3Async`
- hCaptcha solving with `SolveHCaptchaAsync`
- Cloudflare Turnstile solving with `SolveTurnstileAsync`
- FunCaptcha solving with `SolveFunCaptchaAsync`
- Image captcha solving with `SolveImageCaptchaAsync`
- Balance checking with `GetBalanceAsync`
- ASP.NET Core dependency injection via `AddForeverToolsCaptcha()`
- Configuration binding from `appsettings.json`
- `CaptchaProvider` enum for provider selection
- Multi-target support: .NET 8.0, .NET 6.0, .NET Standard 2.0
- 44 unit tests
- Affiliate tracking embedded:
  - 2Captcha: `soft_id: 2482` (10% commission)
  - CapSolver: `appId: 0E76F2D8-D6C0-41DD-A3B6-3708694C47B0` (5-20% commission)

---

## ForeverTools.AIML

### [1.0.1] - 2025-12-08

#### Changed
- SEO optimization: Added 20 NuGet tags for better discoverability
- Enhanced package description with target keywords

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
- 21 unit tests
- Affiliate tracking: `via=forevertools` (30% recurring commission)

#### Technical Details
- Built on OpenAI SDK v2.1.0 for maximum compatibility
- OpenAI-compatible API endpoint (api.aimlapi.com)
- Async/await throughout
- Nullable reference types enabled
- Symbol packages (.snupkg) for debugging

---

## Future Packages (Planned)

### ForeverTools.APILayer
23+ API wrappers (GeoIP, Currency, Phone validation, Email verification, etc.)

### ForeverTools.BrightData
Proxy service (residential, datacenter, web unlocker)

### ForeverTools.BulkGate
SMS API (sending, bulk campaigns, delivery reports)

---

[1.0.1]: https://github.com/ForeverTools/ForeverTools/releases/tag/v1.0.1
[1.0.0]: https://github.com/ForeverTools/ForeverTools/releases/tag/v1.0.0
