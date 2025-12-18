namespace ForeverTools.Postmark;

/// <summary>
/// Configuration options for Postmark client.
/// </summary>
public class PostmarkOptions
{
    /// <summary>
    /// Configuration section name for IConfiguration binding.
    /// </summary>
    public const string SectionName = "Postmark";

    /// <summary>
    /// Your Postmark Server API Token.
    /// Get one at: https://www.postmarkapp.com?via=8ac781
    /// </summary>
    public string? ServerToken { get; set; }

    /// <summary>
    /// Your Postmark Account API Token (for admin operations).
    /// </summary>
    public string? AccountToken { get; set; }

    /// <summary>
    /// The base URL for the Postmark API. Defaults to https://api.postmarkapp.com
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.postmarkapp.com";

    /// <summary>
    /// Default sender email address.
    /// </summary>
    public string? DefaultFrom { get; set; }

    /// <summary>
    /// Default reply-to email address.
    /// </summary>
    public string? DefaultReplyTo { get; set; }

    /// <summary>
    /// Default message stream (outbound or broadcast).
    /// </summary>
    public string? DefaultMessageStream { get; set; }

    /// <summary>
    /// Whether to track opens by default.
    /// </summary>
    public bool? TrackOpens { get; set; }

    /// <summary>
    /// Default link tracking mode.
    /// </summary>
    public string? TrackLinks { get; set; }

    /// <summary>
    /// Request timeout in seconds. Defaults to 30.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Whether the options have a valid server token configured.
    /// </summary>
    public bool HasServerToken => !string.IsNullOrWhiteSpace(ServerToken);

    /// <summary>
    /// Whether the options have a valid account token configured.
    /// </summary>
    public bool HasAccountToken => !string.IsNullOrWhiteSpace(AccountToken);
}
