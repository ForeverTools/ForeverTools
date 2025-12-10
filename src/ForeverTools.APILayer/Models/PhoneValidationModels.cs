using System.Text.Json.Serialization;

namespace ForeverTools.APILayer.Models;

/// <summary>
/// Phone number validation result.
/// </summary>
public class PhoneValidationResult
{
    /// <summary>
    /// Whether the phone number is valid.
    /// </summary>
    [JsonPropertyName("valid")]
    public bool Valid { get; set; }

    /// <summary>
    /// The phone number that was validated.
    /// </summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>
    /// Local format of the phone number.
    /// </summary>
    [JsonPropertyName("local_format")]
    public string? LocalFormat { get; set; }

    /// <summary>
    /// International format of the phone number.
    /// </summary>
    [JsonPropertyName("international_format")]
    public string? InternationalFormat { get; set; }

    /// <summary>
    /// Country prefix/calling code.
    /// </summary>
    [JsonPropertyName("country_prefix")]
    public string? CountryPrefix { get; set; }

    /// <summary>
    /// Two-letter country code.
    /// </summary>
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    /// <summary>
    /// Country name.
    /// </summary>
    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    /// <summary>
    /// Location/region within the country.
    /// </summary>
    [JsonPropertyName("location")]
    public string? Location { get; set; }

    /// <summary>
    /// Carrier/network provider.
    /// </summary>
    [JsonPropertyName("carrier")]
    public string? Carrier { get; set; }

    /// <summary>
    /// Line type (mobile, landline, voip, etc.).
    /// </summary>
    [JsonPropertyName("line_type")]
    public string? LineType { get; set; }
}

/// <summary>
/// Phone number line types.
/// </summary>
public static class PhoneLineTypes
{
    /// <summary>Mobile phone.</summary>
    public const string Mobile = "mobile";
    /// <summary>Landline/fixed line.</summary>
    public const string Landline = "landline";
    /// <summary>VoIP number.</summary>
    public const string Voip = "voip";
    /// <summary>Toll-free number.</summary>
    public const string TollFree = "toll_free";
    /// <summary>Premium rate number.</summary>
    public const string PremiumRate = "premium_rate";
    /// <summary>Shared cost number.</summary>
    public const string SharedCost = "shared_cost";
    /// <summary>Personal number.</summary>
    public const string Personal = "personal";
    /// <summary>Pager.</summary>
    public const string Pager = "pager";
    /// <summary>Unknown type.</summary>
    public const string Unknown = "unknown";
}
