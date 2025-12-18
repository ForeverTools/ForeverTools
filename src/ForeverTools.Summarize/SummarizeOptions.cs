namespace ForeverTools.Summarize;

/// <summary>
/// Configuration options for the SummarizeClient.
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public class SummarizeOptions
{
    /// <summary>
    /// Configuration section name for appsettings.json binding.
    /// </summary>
    public const string SectionName = "Summarize";

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
    /// The default AI model to use for summarization.
    /// Default is GPT-4o for best quality. Use GPT-4o-mini for faster/cheaper summaries.
    /// </summary>
    public string DefaultModel { get; set; } = SummarizeModels.Gpt4o;

    /// <summary>
    /// Default summary style.
    /// </summary>
    public SummaryStyle DefaultStyle { get; set; } = SummaryStyle.Paragraph;

    /// <summary>
    /// Default summary length.
    /// </summary>
    public SummaryLength DefaultLength { get; set; } = SummaryLength.Medium;

    /// <summary>
    /// Maximum number of parallel requests for batch summarization.
    /// </summary>
    public int MaxParallelRequests { get; set; } = 5;

    /// <summary>
    /// Request timeout in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// Whether to preserve important quotes from the original text.
    /// </summary>
    public bool PreserveQuotes { get; set; } = false;

    /// <summary>
    /// Whether to include key statistics/numbers from the original text.
    /// </summary>
    public bool IncludeStatistics { get; set; } = true;
}

/// <summary>
/// Available AI models for summarization.
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public static class SummarizeModels
{
    // OpenAI Models - Best quality
    public const string Gpt4o = "gpt-4o";
    public const string Gpt4oMini = "gpt-4o-mini";
    public const string Gpt4Turbo = "gpt-4-turbo";
    public const string Gpt4 = "gpt-4";
    public const string Gpt35Turbo = "gpt-3.5-turbo";

    // Anthropic Claude - Excellent for long documents
    public const string Claude35Sonnet = "claude-3-5-sonnet-20241022";
    public const string Claude3Opus = "claude-3-opus-20240229";
    public const string Claude3Sonnet = "claude-3-sonnet-20240229";
    public const string Claude3Haiku = "claude-3-haiku-20240307";

    // Google Gemini - Great for research papers
    public const string Gemini15Pro = "gemini-1.5-pro";
    public const string Gemini15Flash = "gemini-1.5-flash";

    // Meta Llama - Open source, good for general summarization
    public const string Llama31405B = "meta-llama/Meta-Llama-3.1-405B-Instruct-Turbo";
    public const string Llama3170B = "meta-llama/Meta-Llama-3.1-70B-Instruct-Turbo";
    public const string Llama318B = "meta-llama/Meta-Llama-3.1-8B-Instruct-Turbo";

    // Mistral - Good balance of speed and quality
    public const string MistralLarge = "mistral-large-latest";
    public const string Mixtral8x7B = "mistralai/Mixtral-8x7B-Instruct-v0.1";

    // Qwen - Good for technical content
    public const string Qwen72B = "Qwen/Qwen2-72B-Instruct";
}

/// <summary>
/// Summary output style options.
/// </summary>
public enum SummaryStyle
{
    /// <summary>
    /// A single cohesive paragraph summary.
    /// </summary>
    Paragraph,

    /// <summary>
    /// Summary as bullet points.
    /// </summary>
    BulletPoints,

    /// <summary>
    /// Numbered list of key points.
    /// </summary>
    NumberedList,

    /// <summary>
    /// Executive summary style (suitable for business documents).
    /// </summary>
    Executive,

    /// <summary>
    /// Academic abstract style.
    /// </summary>
    Abstract,

    /// <summary>
    /// Simple TL;DR - very brief summary.
    /// </summary>
    TLDR,

    /// <summary>
    /// Structured summary with sections (Overview, Key Points, Conclusion).
    /// </summary>
    Structured,

    /// <summary>
    /// Headline-style summary (single sentence).
    /// </summary>
    Headline,

    /// <summary>
    /// Q and A format summary.
    /// </summary>
    QAndA
}

/// <summary>
/// Summary length options.
/// </summary>
public enum SummaryLength
{
    /// <summary>
    /// Very brief - 1-2 sentences or 3-5 bullet points.
    /// </summary>
    VeryShort,

    /// <summary>
    /// Short - 2-3 sentences or 5-7 bullet points.
    /// </summary>
    Short,

    /// <summary>
    /// Medium - 1 paragraph or 7-10 bullet points.
    /// </summary>
    Medium,

    /// <summary>
    /// Long - 2-3 paragraphs or 10-15 bullet points.
    /// </summary>
    Long,

    /// <summary>
    /// Detailed - comprehensive summary preserving more detail.
    /// </summary>
    Detailed,

    /// <summary>
    /// Custom - specify exact word count or percentage.
    /// </summary>
    Custom
}

/// <summary>
/// Content domain hints for better summarization.
/// </summary>
public enum ContentDomain
{
    /// <summary>
    /// General content (auto-detect).
    /// </summary>
    General,

    /// <summary>
    /// News article.
    /// </summary>
    News,

    /// <summary>
    /// Academic/research paper.
    /// </summary>
    Academic,

    /// <summary>
    /// Legal document or contract.
    /// </summary>
    Legal,

    /// <summary>
    /// Technical documentation.
    /// </summary>
    Technical,

    /// <summary>
    /// Business report or memo.
    /// </summary>
    Business,

    /// <summary>
    /// Medical/healthcare content.
    /// </summary>
    Medical,

    /// <summary>
    /// Financial report or analysis.
    /// </summary>
    Financial,

    /// <summary>
    /// Meeting notes or transcript.
    /// </summary>
    Meeting,

    /// <summary>
    /// Book or long-form content.
    /// </summary>
    Book,

    /// <summary>
    /// Email thread or conversation.
    /// </summary>
    Email,

    /// <summary>
    /// Social media content.
    /// </summary>
    Social
}
