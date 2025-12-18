using System.Text.Json.Serialization;

namespace ForeverTools.APILayer.Models;

/// <summary>
/// Email validation result.
/// </summary>
public class EmailValidationResult
{
    /// <summary>
    /// The email address that was validated.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Did you mean this email? (suggestion for typos)
    /// </summary>
    [JsonPropertyName("did_you_mean")]
    public string? DidYouMean { get; set; }

    /// <summary>
    /// The username part of the email (before @).
    /// </summary>
    [JsonPropertyName("user")]
    public string? User { get; set; }

    /// <summary>
    /// The domain part of the email (after @).
    /// </summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    /// <summary>
    /// Whether the email format is valid.
    /// </summary>
    [JsonPropertyName("format_valid")]
    public bool FormatValid { get; set; }

    /// <summary>
    /// Whether MX records exist for the domain.
    /// </summary>
    [JsonPropertyName("mx_found")]
    public bool MxFound { get; set; }

    /// <summary>
    /// Whether the SMTP check passed.
    /// </summary>
    [JsonPropertyName("smtp_check")]
    public bool SmtpCheck { get; set; }

    /// <summary>
    /// Whether a catch-all is configured.
    /// </summary>
    [JsonPropertyName("catch_all")]
    public bool? CatchAll { get; set; }

    /// <summary>
    /// Whether this is a role-based email (info@, support@, etc.).
    /// </summary>
    [JsonPropertyName("role")]
    public bool Role { get; set; }

    /// <summary>
    /// Whether this is a disposable/temporary email.
    /// </summary>
    [JsonPropertyName("disposable")]
    public bool Disposable { get; set; }

    /// <summary>
    /// Whether this is a free email provider (gmail, yahoo, etc.).
    /// </summary>
    [JsonPropertyName("free")]
    public bool Free { get; set; }

    /// <summary>
    /// Overall quality score (0-1).
    /// </summary>
    [JsonPropertyName("score")]
    public double Score { get; set; }

    /// <summary>
    /// Whether the email is likely valid and deliverable.
    /// </summary>
    public bool IsDeliverable => FormatValid && MxFound && SmtpCheck && !Disposable;

    /// <summary>
    /// Quality assessment based on score.
    /// </summary>
    public EmailQuality Quality => Score switch
    {
        >= 0.8 => EmailQuality.High,
        >= 0.5 => EmailQuality.Medium,
        >= 0.2 => EmailQuality.Low,
        _ => EmailQuality.Invalid
    };
}

/// <summary>
/// Email quality levels.
/// </summary>
public enum EmailQuality
{
    /// <summary>Invalid or undeliverable email.</summary>
    Invalid,
    /// <summary>Low quality - may have issues.</summary>
    Low,
    /// <summary>Medium quality - acceptable.</summary>
    Medium,
    /// <summary>High quality - good deliverability.</summary>
    High
}
