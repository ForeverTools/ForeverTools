# ForeverTools MCP Server

An MCP (Model Context Protocol) server that gives Claude Code and other AI assistants access to powerful AI services: text summarization, translation (100+ languages), sentiment analysis, OCR, speech-to-text, image generation, and web tools.

## Quick Start

```bash
# Install globally
dotnet tool install -g ForeverTools.Mcp

# Set your API key (get free at https://aimlapi.com/?via=forevertools)
export AIMLAPI_KEY=your_key_here

# Add to Claude Code
claude mcp add forevertools-mcp --env AIMLAPI_KEY=$AIMLAPI_KEY
```

## Available Tools

| Tool | Description |
|------|-------------|
| `SummarizeText` | Summarize text in TL;DR, bullets, executive, or key-points style |
| `TranslateText` | Translate to any of 100+ languages with style control |
| `ListSupportedLanguages` | List all supported translation languages |
| `AnalyzeSentiment` | Detect sentiment + emotions with confidence scores |
| `AnalyzeSentimentBatch` | Analyze up to 20 texts at once |
| `ExtractTextFromImage` | OCR — extract text from any image URL |
| `TranscribeAudio` | Speech-to-text from audio URL (MP3, WAV, M4A, FLAC) |
| `GenerateImage` | AI image generation (DALL-E 3, Flux, Stable Diffusion) |
| `FetchUrlText` | Fetch any web page and return clean stripped text (no API key needed) |
| `SummarizeUrl` | Fetch a URL and summarize its content using AI |
| `ExtractKeywords` | Extract top keywords/phrases from text as list, JSON, or CSV |

## Configuration (Claude Code)

Add to your `~/.claude/settings.json`:

```json
{
  "mcpServers": {
    "forevertools": {
      "command": "forevertools-mcp",
      "env": {
        "AIMLAPI_KEY": "your_api_key_here"
      }
    }
  }
}
```

## Get an API Key

`FetchUrlText` requires no API key. All other tools require an [AI/ML API](https://aimlapi.com/?via=forevertools) key.
- **Free tier**: 10M tokens/month — more than enough for most use cases
- **Paid**: from $9/month for production workloads

## Examples

Once connected to Claude Code:

> "Summarize this article in bullet points" *(paste article)*

> "Translate this product description to Spanish, French, and German"

> "Analyze the sentiment of these 10 customer reviews"

> "Extract the text from this receipt: https://example.com/receipt.jpg"

> "Fetch the content from this documentation page" *(no API key needed)*

> "Summarize what's on this competitor's pricing page"

> "Extract the 10 most important keywords from this blog post"

## More ForeverTools Packages

Browse the complete collection at [github.com/ForeverTools/ForeverTools](https://github.com/ForeverTools/ForeverTools):

- **ForeverTools.AIML** — Access 400+ AI models (GPT-4, Claude, Llama, Gemini)
- **ForeverTools.OCR** — Standalone OCR library for .NET apps
- **ForeverTools.Translate** — Translation library with glossary support
- **ForeverTools.Summarize** — Advanced summarization with batch processing
- **ForeverTools.Sentiment** — Sentiment + emotion analysis
- **ForeverTools.ImageGen** — AI image generation (DALL-E, Stable Diffusion, Flux)
- **ForeverTools.STT** — Speech-to-text (Whisper)

## License

MIT — [github.com/ForeverTools/ForeverTools](https://github.com/ForeverTools/ForeverTools)
