namespace ForeverTools.BrightData;

/// <summary>
/// Configuration options for the BrightData Web Scraper API client.
/// Get your API token at: https://get.brightdata.com/ForeverToolsWebScraper
/// </summary>
public class BrightDataOptions
{
    /// <summary>Your BrightData API token.</summary>
    public string ApiToken { get; set; } = string.Empty;

    /// <summary>Base URL for the BrightData API. Default: https://api.brightdata.com</summary>
    public string BaseUrl { get; set; } = "https://api.brightdata.com";

    /// <summary>Request timeout in seconds. Default: 60.</summary>
    public int TimeoutSeconds { get; set; } = 60;

    /// <summary>Polling interval in seconds when waiting for snapshots. Default: 3.</summary>
    public int PollIntervalSeconds { get; set; } = 3;

    /// <summary>Maximum time in seconds to wait for a snapshot to complete. Default: 300.</summary>
    public int MaxWaitSeconds { get; set; } = 300;

    internal bool HasApiToken => !string.IsNullOrWhiteSpace(ApiToken);
}
