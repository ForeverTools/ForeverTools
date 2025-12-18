namespace ForeverTools.Apify;

/// <summary>
/// Configuration options for Apify client.
/// Get your API token at: https://www.apify.com/?fpr=8hklqy
/// </summary>
public class ApifyOptions
{
    /// <summary>
    /// Configuration section name for IConfiguration binding.
    /// </summary>
    public const string SectionName = "Apify";

    /// <summary>
    /// Your Apify API token.
    /// Get one at: https://www.apify.com/?fpr=8hklqy
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Base URL for the Apify API. Default: https://api.apify.com/v2
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.apify.com/v2";

    /// <summary>
    /// Request timeout in seconds. Default: 300 (5 minutes).
    /// </summary>
    public int TimeoutSeconds { get; set; } = 300;

    /// <summary>
    /// Default memory allocation for actor runs in MB. Default: 256.
    /// </summary>
    public int DefaultMemoryMb { get; set; } = 256;

    /// <summary>
    /// Default timeout for actor runs in seconds. Default: 300 (5 minutes).
    /// </summary>
    public int DefaultTimeoutSeconds { get; set; } = 300;

    /// <summary>
    /// Default polling interval when waiting for runs in milliseconds. Default: 2000.
    /// </summary>
    public int DefaultPollIntervalMs { get; set; } = 2000;

    /// <summary>
    /// Returns true if an API token is configured.
    /// </summary>
    public bool HasToken => !string.IsNullOrWhiteSpace(Token);
}
