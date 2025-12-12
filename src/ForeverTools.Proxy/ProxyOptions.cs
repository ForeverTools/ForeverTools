namespace ForeverTools.Proxy;

/// <summary>
/// Configuration options for the BrightData proxy client.
/// </summary>
public class ProxyOptions
{
    /// <summary>
    /// Your BrightData customer ID.
    /// Found in your BrightData dashboard.
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// The zone name for your proxy configuration.
    /// Create zones in your BrightData dashboard.
    /// </summary>
    public string Zone { get; set; } = string.Empty;

    /// <summary>
    /// The zone password.
    /// Found in your BrightData zone settings.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The default proxy type to use.
    /// Default: Residential.
    /// </summary>
    public ProxyType DefaultProxyType { get; set; } = ProxyType.Residential;

    /// <summary>
    /// The default protocol to use.
    /// Default: Http.
    /// </summary>
    public ProxyProtocol DefaultProtocol { get; set; } = ProxyProtocol.Http;

    /// <summary>
    /// Default country code for geo-targeting (ISO 3166-1 alpha-2).
    /// Example: "us", "uk", "de".
    /// </summary>
    public string? DefaultCountry { get; set; }

    /// <summary>
    /// Default city for geo-targeting.
    /// Example: "new_york", "london".
    /// </summary>
    public string? DefaultCity { get; set; }

    /// <summary>
    /// Default state for geo-targeting (US/AU only).
    /// Example: "ca" for California, "ny" for New York.
    /// </summary>
    public string? DefaultState { get; set; }

    /// <summary>
    /// The BrightData proxy host.
    /// Default: brd.superproxy.io
    /// </summary>
    public string Host { get; set; } = "brd.superproxy.io";

    /// <summary>
    /// Timeout for proxy connections in seconds.
    /// Default: 60 seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Whether to use sticky sessions by default.
    /// When enabled, requests will use the same IP until the session expires.
    /// </summary>
    public bool UseStickySession { get; set; }

    /// <summary>
    /// Session timeout in minutes for sticky sessions.
    /// BrightData default is 5 minutes.
    /// </summary>
    public int SessionTimeoutMinutes { get; set; } = 5;
}

/// <summary>
/// Represents geo-targeting options for proxy requests.
/// </summary>
public class GeoTarget
{
    /// <summary>
    /// Country code (ISO 3166-1 alpha-2).
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// City name (lowercase, underscores for spaces).
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// State code for US/AU (lowercase, 2-letter).
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Creates a new GeoTarget for a specific country.
    /// </summary>
    public static GeoTarget ForCountry(string countryCode) => new() { Country = countryCode.ToLowerInvariant() };

    /// <summary>
    /// Creates a new GeoTarget for a specific city.
    /// </summary>
    public static GeoTarget ForCity(string countryCode, string city) => new()
    {
        Country = countryCode.ToLowerInvariant(),
        City = city.ToLowerInvariant().Replace(" ", "_")
    };

    /// <summary>
    /// Creates a new GeoTarget for a specific US state.
    /// </summary>
    public static GeoTarget ForUSState(string stateCode) => new()
    {
        Country = "us",
        State = stateCode.ToLowerInvariant()
    };

    /// <summary>
    /// Creates a new GeoTarget for the European Union.
    /// </summary>
    public static GeoTarget ForEU() => new() { Country = "eu" };

    /// <summary>
    /// Predefined targets for common locations.
    /// </summary>
    public static class Locations
    {
        public static GeoTarget UnitedStates => ForCountry("us");
        public static GeoTarget UnitedKingdom => ForCountry("uk");
        public static GeoTarget Germany => ForCountry("de");
        public static GeoTarget France => ForCountry("fr");
        public static GeoTarget Canada => ForCountry("ca");
        public static GeoTarget Australia => ForCountry("au");
        public static GeoTarget Japan => ForCountry("jp");
        public static GeoTarget Brazil => ForCountry("br");
        public static GeoTarget India => ForCountry("in");
        public static GeoTarget EuropeanUnion => ForEU();

        // US Cities
        public static GeoTarget NewYork => ForCity("us", "new_york");
        public static GeoTarget LosAngeles => ForCity("us", "los_angeles");
        public static GeoTarget Chicago => ForCity("us", "chicago");
        public static GeoTarget Miami => ForCity("us", "miami");
        public static GeoTarget SanFrancisco => ForCity("us", "san_francisco");

        // UK Cities
        public static GeoTarget London => ForCity("uk", "london");
        public static GeoTarget Manchester => ForCity("uk", "manchester");

        // US States
        public static GeoTarget California => ForUSState("ca");
        public static GeoTarget Texas => ForUSState("tx");
        public static GeoTarget Florida => ForUSState("fl");
        public static GeoTarget NewYorkState => ForUSState("ny");
    }
}
