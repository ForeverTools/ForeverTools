namespace ForeverTools.ScraperAPI;

/// <summary>
/// Configuration options for ScraperAPI client.
/// Get your API key at: https://www.scraperapi.com/signup?fp_ref=chris88
/// </summary>
public class ScraperApiOptions
{
    /// <summary>
    /// Configuration section name for IConfiguration binding.
    /// </summary>
    public const string SectionName = "ScraperAPI";

    /// <summary>
    /// Your ScraperAPI API key.
    /// Get one at: https://www.scraperapi.com/signup?fp_ref=chris88
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Base URL for the ScraperAPI service. Default: https://api.scraperapi.com
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.scraperapi.com";

    /// <summary>
    /// Base URL for async scraping service. Default: https://async.scraperapi.com
    /// </summary>
    public string AsyncBaseUrl { get; set; } = "https://async.scraperapi.com";

    /// <summary>
    /// Request timeout in seconds. Default: 70 (recommended by ScraperAPI).
    /// </summary>
    public int TimeoutSeconds { get; set; } = 70;

    /// <summary>
    /// Default country code for geo-targeting (e.g., "us", "uk", "de").
    /// </summary>
    public string? DefaultCountryCode { get; set; }

    /// <summary>
    /// Enable JavaScript rendering by default. Costs 10 credits per request.
    /// </summary>
    public bool DefaultRenderJavaScript { get; set; }

    /// <summary>
    /// Use premium residential proxies by default. Costs 10 credits per request.
    /// </summary>
    public bool DefaultPremium { get; set; }

    /// <summary>
    /// Default device type for requests ("desktop" or "mobile").
    /// </summary>
    public string? DefaultDeviceType { get; set; }

    /// <summary>
    /// Returns true if an API key is configured.
    /// </summary>
    public bool HasApiKey => !string.IsNullOrWhiteSpace(ApiKey);
}
