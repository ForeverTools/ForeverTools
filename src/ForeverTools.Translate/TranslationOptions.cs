namespace ForeverTools.Translate;

/// <summary>
/// Configuration options for the TranslationClient.
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public class TranslationOptions
{
    /// <summary>
    /// Your AI/ML API key.
    /// Get one at: https://aimlapi.com?via=forevertools
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The base URL for the AI/ML API.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.aimlapi.com/v1";

    /// <summary>
    /// The default AI model to use for translations.
    /// Default is GPT-4o for best quality. Use GPT-4o-mini for faster/cheaper translations.
    /// </summary>
    public string DefaultModel { get; set; } = TranslationModels.Gpt4o;

    /// <summary>
    /// Default source language for translations. Set to null for auto-detection.
    /// </summary>
    public string? DefaultSourceLanguage { get; set; }

    /// <summary>
    /// Default target language for translations.
    /// </summary>
    public string DefaultTargetLanguage { get; set; } = "en";

    /// <summary>
    /// Whether to preserve formatting (newlines, bullet points, etc.) in translations.
    /// </summary>
    public bool PreserveFormatting { get; set; } = true;

    /// <summary>
    /// Translation style/tone preference.
    /// </summary>
    public TranslationStyle Style { get; set; } = TranslationStyle.Natural;

    /// <summary>
    /// Maximum number of parallel requests for batch translations.
    /// </summary>
    public int MaxParallelRequests { get; set; } = 5;

    /// <summary>
    /// Request timeout in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;
}

/// <summary>
/// Available AI models for translation.
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public static class TranslationModels
{
    // OpenAI Models - Best quality
    public const string Gpt4o = "gpt-4o";
    public const string Gpt4oMini = "gpt-4o-mini";
    public const string Gpt4Turbo = "gpt-4-turbo";
    public const string Gpt4 = "gpt-4";
    public const string Gpt35Turbo = "gpt-3.5-turbo";

    // Anthropic Claude - Excellent for nuanced translation
    public const string Claude35Sonnet = "claude-3-5-sonnet-20241022";
    public const string Claude3Opus = "claude-3-opus-20240229";
    public const string Claude3Sonnet = "claude-3-sonnet-20240229";
    public const string Claude3Haiku = "claude-3-haiku-20240307";

    // Google Gemini
    public const string Gemini15Pro = "gemini-1.5-pro";
    public const string Gemini15Flash = "gemini-1.5-flash";

    // Meta Llama - Open source, good for many languages
    public const string Llama31405B = "meta-llama/Meta-Llama-3.1-405B-Instruct-Turbo";
    public const string Llama3170B = "meta-llama/Meta-Llama-3.1-70B-Instruct-Turbo";
    public const string Llama318B = "meta-llama/Meta-Llama-3.1-8B-Instruct-Turbo";

    // Mistral - Good multilingual support
    public const string MistralLarge = "mistral-large-latest";
    public const string Mixtral8x7B = "mistralai/Mixtral-8x7B-Instruct-v0.1";

    // Qwen - Excellent for Asian languages
    public const string Qwen72B = "Qwen/Qwen2-72B-Instruct";
}

/// <summary>
/// Translation style/tone options.
/// </summary>
public enum TranslationStyle
{
    /// <summary>
    /// Natural, fluent translation that reads well in the target language.
    /// </summary>
    Natural,

    /// <summary>
    /// Formal, professional tone suitable for business documents.
    /// </summary>
    Formal,

    /// <summary>
    /// Casual, conversational tone.
    /// </summary>
    Casual,

    /// <summary>
    /// Technical translation preserving specialized terminology.
    /// </summary>
    Technical,

    /// <summary>
    /// Literal translation staying close to source structure.
    /// </summary>
    Literal,

    /// <summary>
    /// Creative translation with more freedom for adaptation.
    /// </summary>
    Creative
}
