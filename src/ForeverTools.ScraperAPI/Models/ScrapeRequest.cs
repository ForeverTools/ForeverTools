namespace ForeverTools.ScraperAPI.Models;

/// <summary>
/// Request configuration for scraping a URL.
/// </summary>
public class ScrapeRequest
{
    /// <summary>
    /// The URL to scrape.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Enable JavaScript rendering. Costs 10 credits (75 with ultra_premium).
    /// </summary>
    public bool RenderJavaScript { get; set; }

    /// <summary>
    /// Take a screenshot of the page. Automatically enables JavaScript rendering.
    /// </summary>
    public bool Screenshot { get; set; }

    /// <summary>
    /// Country code for geo-targeting (e.g., "us", "uk", "de", "fr").
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// Use premium residential/mobile proxies. Costs 10 credits (25 with render).
    /// Cannot combine with UltraPremium.
    /// </summary>
    public bool Premium { get; set; }

    /// <summary>
    /// Use ultra-premium proxies with advanced bypass. Costs 30 credits (75 with render).
    /// Cannot combine with Premium.
    /// </summary>
    public bool UltraPremium { get; set; }

    /// <summary>
    /// Session number for sticky sessions (reuse same proxy).
    /// Cannot combine with Premium or UltraPremium.
    /// </summary>
    public int? SessionNumber { get; set; }

    /// <summary>
    /// Device type: "desktop" or "mobile".
    /// </summary>
    public string? DeviceType { get; set; }

    /// <summary>
    /// Enable auto-parsing for supported sites. Returns JSON instead of HTML.
    /// </summary>
    public bool AutoParse { get; set; }

    /// <summary>
    /// Keep custom headers in the request.
    /// </summary>
    public bool KeepHeaders { get; set; }

    /// <summary>
    /// Follow redirects. Default: true.
    /// </summary>
    public bool FollowRedirect { get; set; } = true;

    /// <summary>
    /// Custom headers to include with the request.
    /// Requires KeepHeaders = true.
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Output format: "html" (default), "markdown", "text", "json", or "csv".
    /// </summary>
    public string? OutputFormat { get; set; }
}

/// <summary>
/// Device type constants.
/// </summary>
public static class DeviceTypes
{
    /// <summary>Desktop user agent.</summary>
    public const string Desktop = "desktop";

    /// <summary>Mobile user agent.</summary>
    public const string Mobile = "mobile";
}

/// <summary>
/// Output format constants.
/// </summary>
public static class OutputFormats
{
    /// <summary>Raw HTML output.</summary>
    public const string Html = "html";

    /// <summary>Markdown formatted output.</summary>
    public const string Markdown = "markdown";

    /// <summary>Plain text output.</summary>
    public const string Text = "text";

    /// <summary>JSON structured output.</summary>
    public const string Json = "json";

    /// <summary>CSV output (for structured data).</summary>
    public const string Csv = "csv";
}
