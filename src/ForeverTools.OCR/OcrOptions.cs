namespace ForeverTools.OCR;

/// <summary>
/// Configuration options for the OCR client.
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public class OcrOptions
{
    /// <summary>
    /// The API key for authentication.
    /// Get your key at: https://aimlapi.com?via=forevertools
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The base URL for the AI/ML API.
    /// Default: https://api.aimlapi.com/v1
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.aimlapi.com/v1";

    /// <summary>
    /// The default vision model to use for OCR.
    /// Default: gpt-4o (best quality/speed balance)
    /// </summary>
    public string DefaultModel { get; set; } = OcrModels.Gpt4o;

    /// <summary>
    /// Request timeout in seconds.
    /// Default: 60 seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Maximum tokens for the response.
    /// Default: 4096
    /// </summary>
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// Image detail level for processing.
    /// "low" = faster/cheaper, "high" = better quality, "auto" = model decides
    /// Default: auto
    /// </summary>
    public string ImageDetail { get; set; } = "auto";
}
