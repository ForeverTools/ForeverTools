namespace ForeverTools.APILayer;

/// <summary>
/// Configuration options for APILayer client.
/// Get your API key at: https://apilayer.com?fpr=chris72
/// </summary>
public class ApiLayerOptions
{
    /// <summary>
    /// Configuration section name for IConfiguration binding.
    /// </summary>
    public const string SectionName = "APILayer";

    /// <summary>
    /// Your APILayer API key.
    /// Get one at: https://apilayer.com?fpr=chris72
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Base URL for the APILayer service. Default: https://api.apilayer.com
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.apilayer.com";

    /// <summary>
    /// Request timeout in seconds. Default: 30.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Returns true if an API key is configured.
    /// </summary>
    public bool HasApiKey => !string.IsNullOrWhiteSpace(ApiKey);
}
