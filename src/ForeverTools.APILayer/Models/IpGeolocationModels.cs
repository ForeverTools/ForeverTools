using System.Text.Json.Serialization;

namespace ForeverTools.APILayer.Models;

/// <summary>
/// IP geolocation lookup result.
/// </summary>
public class IpGeolocationResult
{
    /// <summary>
    /// The IP address that was looked up.
    /// </summary>
    [JsonPropertyName("ip")]
    public string? Ip { get; set; }

    /// <summary>
    /// Type of IP address (ipv4 or ipv6).
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Two-letter continent code.
    /// </summary>
    [JsonPropertyName("continent_code")]
    public string? ContinentCode { get; set; }

    /// <summary>
    /// Continent name.
    /// </summary>
    [JsonPropertyName("continent_name")]
    public string? ContinentName { get; set; }

    /// <summary>
    /// Two-letter country code (ISO 3166-1 alpha-2).
    /// </summary>
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    /// <summary>
    /// Country name.
    /// </summary>
    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    /// <summary>
    /// Region/state code.
    /// </summary>
    [JsonPropertyName("region_code")]
    public string? RegionCode { get; set; }

    /// <summary>
    /// Region/state name.
    /// </summary>
    [JsonPropertyName("region_name")]
    public string? RegionName { get; set; }

    /// <summary>
    /// City name.
    /// </summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>
    /// Postal/ZIP code.
    /// </summary>
    [JsonPropertyName("zip")]
    public string? Zip { get; set; }

    /// <summary>
    /// Latitude coordinate.
    /// </summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate.
    /// </summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    /// <summary>
    /// Location details.
    /// </summary>
    [JsonPropertyName("location")]
    public IpLocation? Location { get; set; }

    /// <summary>
    /// Time zone information.
    /// </summary>
    [JsonPropertyName("time_zone")]
    public IpTimeZone? TimeZone { get; set; }

    /// <summary>
    /// Currency information for the country.
    /// </summary>
    [JsonPropertyName("currency")]
    public IpCurrency? Currency { get; set; }

    /// <summary>
    /// Connection/ISP information.
    /// </summary>
    [JsonPropertyName("connection")]
    public IpConnection? Connection { get; set; }

    /// <summary>
    /// Security threat information.
    /// </summary>
    [JsonPropertyName("security")]
    public IpSecurity? Security { get; set; }
}

/// <summary>
/// Location details for an IP address.
/// </summary>
public class IpLocation
{
    /// <summary>
    /// Geoname ID for the location.
    /// </summary>
    [JsonPropertyName("geoname_id")]
    public int? GeonameId { get; set; }

    /// <summary>
    /// Capital city of the country.
    /// </summary>
    [JsonPropertyName("capital")]
    public string? Capital { get; set; }

    /// <summary>
    /// Languages spoken in the country.
    /// </summary>
    [JsonPropertyName("languages")]
    public List<IpLanguage>? Languages { get; set; }

    /// <summary>
    /// URL to country flag image.
    /// </summary>
    [JsonPropertyName("country_flag")]
    public string? CountryFlag { get; set; }

    /// <summary>
    /// Country flag emoji.
    /// </summary>
    [JsonPropertyName("country_flag_emoji")]
    public string? CountryFlagEmoji { get; set; }

    /// <summary>
    /// Country calling code.
    /// </summary>
    [JsonPropertyName("calling_code")]
    public string? CallingCode { get; set; }

    /// <summary>
    /// Whether the country is in the European Union.
    /// </summary>
    [JsonPropertyName("is_eu")]
    public bool? IsEu { get; set; }
}

/// <summary>
/// Language information.
/// </summary>
public class IpLanguage
{
    /// <summary>
    /// Language code.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// Language name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Native language name.
    /// </summary>
    [JsonPropertyName("native")]
    public string? Native { get; set; }
}

/// <summary>
/// Time zone information.
/// </summary>
public class IpTimeZone
{
    /// <summary>
    /// IANA time zone identifier (e.g., "America/New_York").
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Current local time.
    /// </summary>
    [JsonPropertyName("current_time")]
    public string? CurrentTime { get; set; }

    /// <summary>
    /// GMT offset in seconds.
    /// </summary>
    [JsonPropertyName("gmt_offset")]
    public int? GmtOffset { get; set; }

    /// <summary>
    /// Time zone code (e.g., "EST").
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// Whether daylight saving time is active.
    /// </summary>
    [JsonPropertyName("is_daylight_saving")]
    public bool? IsDaylightSaving { get; set; }
}

/// <summary>
/// Currency information.
/// </summary>
public class IpCurrency
{
    /// <summary>
    /// Currency code (e.g., "USD").
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// Currency name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Currency symbol (e.g., "$").
    /// </summary>
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
}

/// <summary>
/// Connection/ISP information.
/// </summary>
public class IpConnection
{
    /// <summary>
    /// Autonomous System Number.
    /// </summary>
    [JsonPropertyName("asn")]
    public int? Asn { get; set; }

    /// <summary>
    /// Internet Service Provider name.
    /// </summary>
    [JsonPropertyName("isp")]
    public string? Isp { get; set; }
}

/// <summary>
/// Security threat information.
/// </summary>
public class IpSecurity
{
    /// <summary>
    /// Whether the IP is a known proxy.
    /// </summary>
    [JsonPropertyName("is_proxy")]
    public bool? IsProxy { get; set; }

    /// <summary>
    /// Type of proxy if applicable.
    /// </summary>
    [JsonPropertyName("proxy_type")]
    public string? ProxyType { get; set; }

    /// <summary>
    /// Whether the IP is a known crawler.
    /// </summary>
    [JsonPropertyName("is_crawler")]
    public bool? IsCrawler { get; set; }

    /// <summary>
    /// Crawler name if applicable.
    /// </summary>
    [JsonPropertyName("crawler_name")]
    public string? CrawlerName { get; set; }

    /// <summary>
    /// Crawler type if applicable.
    /// </summary>
    [JsonPropertyName("crawler_type")]
    public string? CrawlerType { get; set; }

    /// <summary>
    /// Whether the IP is associated with Tor.
    /// </summary>
    [JsonPropertyName("is_tor")]
    public bool? IsTor { get; set; }

    /// <summary>
    /// Threat level assessment.
    /// </summary>
    [JsonPropertyName("threat_level")]
    public string? ThreatLevel { get; set; }

    /// <summary>
    /// Threat types detected.
    /// </summary>
    [JsonPropertyName("threat_types")]
    public List<string>? ThreatTypes { get; set; }
}
