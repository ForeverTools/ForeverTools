namespace ForeverTools.AIML;

/// <summary>
/// Configuration options for the AI/ML API client.
/// </summary>
public class AimlApiOptions
{
    /// <summary>
    /// The configuration section name for binding from appsettings.json.
    /// </summary>
    public const string SectionName = "AimlApi";

    /// <summary>
    /// The default API base URL for AI/ML API.
    /// </summary>
    public const string DefaultBaseUrl = "https://api.aimlapi.com/v1";

    /// <summary>
    /// Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The API base URL. Defaults to the AI/ML API endpoint.
    /// </summary>
    public string BaseUrl { get; set; } = DefaultBaseUrl;

    /// <summary>
    /// The default model to use for chat completions.
    /// </summary>
    public string DefaultChatModel { get; set; } = AimlModels.Chat.Gpt4o;

    /// <summary>
    /// The default model to use for image generation.
    /// </summary>
    public string DefaultImageModel { get; set; } = AimlModels.Image.DallE3;

    /// <summary>
    /// The default model to use for embeddings.
    /// </summary>
    public string DefaultEmbeddingModel { get; set; } = AimlModels.Embedding.TextEmbedding3Small;

    /// <summary>
    /// Optional timeout for API requests in seconds.
    /// </summary>
    public int? TimeoutSeconds { get; set; }
}
