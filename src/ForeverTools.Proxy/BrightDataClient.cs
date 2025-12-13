using System.Diagnostics;
using System.Net;
using System.Text.Json;
using ForeverTools.Proxy.Models;

namespace ForeverTools.Proxy;

/// <summary>
/// Client for BrightData proxy services.
/// Provides access to residential, datacenter, ISP, and mobile proxies
/// with geo-targeting and session management.
/// Get your credentials at https://get.brightdata.com/ForeverToolsResidentialProxies
/// </summary>
public class BrightDataClient : IDisposable
{
    private readonly ProxyOptions _options;
    private readonly HttpClient _testHttpClient;
    private bool _disposed;

    /// <summary>
    /// Creates a new BrightDataClient with the specified credentials.
    /// </summary>
    /// <param name="customerId">Your BrightData customer ID.</param>
    /// <param name="zone">The zone name.</param>
    /// <param name="password">The zone password.</param>
    public BrightDataClient(string customerId, string zone, string password)
        : this(new ProxyOptions { CustomerId = customerId, Zone = zone, Password = password })
    {
    }

    /// <summary>
    /// Creates a new BrightDataClient with full configuration options.
    /// </summary>
    public BrightDataClient(ProxyOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(options.CustomerId))
            throw new ArgumentException("Customer ID is required.", nameof(options));
        if (string.IsNullOrWhiteSpace(options.Zone))
            throw new ArgumentException("Zone name is required.", nameof(options));
        if (string.IsNullOrWhiteSpace(options.Password))
            throw new ArgumentException("Password is required.", nameof(options));

        _testHttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds) };
    }

    /// <summary>
    /// Creates a client from environment variables.
    /// Uses BRIGHTDATA_CUSTOMER_ID, BRIGHTDATA_ZONE, BRIGHTDATA_PASSWORD.
    /// </summary>
    public static BrightDataClient FromEnvironment()
    {
        var customerId = Environment.GetEnvironmentVariable("BRIGHTDATA_CUSTOMER_ID")
            ?? throw new InvalidOperationException("Environment variable 'BRIGHTDATA_CUSTOMER_ID' not found.");
        var zone = Environment.GetEnvironmentVariable("BRIGHTDATA_ZONE")
            ?? throw new InvalidOperationException("Environment variable 'BRIGHTDATA_ZONE' not found.");
        var password = Environment.GetEnvironmentVariable("BRIGHTDATA_PASSWORD")
            ?? throw new InvalidOperationException("Environment variable 'BRIGHTDATA_PASSWORD' not found.");

        return new BrightDataClient(customerId, zone, password);
    }

    #region Credential Generation

    /// <summary>
    /// Gets proxy credentials with default settings.
    /// </summary>
    public ProxyCredentials GetCredentials()
    {
        return GetCredentials(_options.DefaultProxyType, _options.DefaultProtocol, null, null);
    }

    /// <summary>
    /// Gets proxy credentials for a specific proxy type.
    /// </summary>
    public ProxyCredentials GetCredentials(ProxyType proxyType)
    {
        return GetCredentials(proxyType, _options.DefaultProtocol, null, null);
    }

    /// <summary>
    /// Gets proxy credentials with geo-targeting.
    /// </summary>
    public ProxyCredentials GetCredentials(GeoTarget geoTarget)
    {
        return GetCredentials(_options.DefaultProxyType, _options.DefaultProtocol, geoTarget, null);
    }

    /// <summary>
    /// Gets proxy credentials for a specific country.
    /// </summary>
    public ProxyCredentials GetCredentialsForCountry(string countryCode)
    {
        return GetCredentials(GeoTarget.ForCountry(countryCode));
    }

    /// <summary>
    /// Gets proxy credentials with a sticky session.
    /// </summary>
    public ProxyCredentials GetCredentialsWithSession(string? sessionId = null)
    {
        return GetCredentials(_options.DefaultProxyType, _options.DefaultProtocol, null, sessionId ?? GenerateSessionId());
    }

    /// <summary>
    /// Gets fully customized proxy credentials.
    /// </summary>
    public ProxyCredentials GetCredentials(
        ProxyType proxyType,
        ProxyProtocol protocol,
        GeoTarget? geoTarget,
        string? sessionId)
    {
        var username = BuildUsername(proxyType, geoTarget, sessionId);
        var port = proxyType.GetDefaultPort(protocol);

        return new ProxyCredentials
        {
            Host = _options.Host,
            Port = port,
            Username = username,
            Password = _options.Password,
            ProxyType = proxyType,
            Protocol = protocol,
            GeoTarget = geoTarget,
            SessionId = sessionId
        };
    }

    /// <summary>
    /// Builds the BrightData username with all targeting options.
    /// Format: brd-customer-{id}-zone-{zone}[-country-xx][-city-xx][-state-xx][-session-xx]
    /// </summary>
    private string BuildUsername(ProxyType proxyType, GeoTarget? geoTarget, string? sessionId)
    {
        var parts = new List<string>
        {
            "brd",
            $"customer-{_options.CustomerId}",
            $"zone-{_options.Zone}"
        };

        // Apply default geo-targeting if none specified
        var geo = geoTarget ?? new GeoTarget
        {
            Country = _options.DefaultCountry,
            City = _options.DefaultCity,
            State = _options.DefaultState
        };

        if (!string.IsNullOrEmpty(geo.Country))
        {
            parts.Add($"country-{geo.Country!.ToLowerInvariant()}");
        }

        if (!string.IsNullOrEmpty(geo.State))
        {
            parts.Add($"state-{geo.State!.ToLowerInvariant()}");
        }

        if (!string.IsNullOrEmpty(geo.City))
        {
            parts.Add($"city-{geo.City!.ToLowerInvariant().Replace(" ", "_")}");
        }

        if (!string.IsNullOrEmpty(sessionId))
        {
            parts.Add($"session-{sessionId}");
        }

        return string.Join("-", parts);
    }

    #endregion

    #region HttpClient Factory

    /// <summary>
    /// Creates an HttpClient configured to use the proxy with default settings.
    /// </summary>
    public HttpClient CreateHttpClient()
    {
        return CreateHttpClient(_options.DefaultProxyType, _options.DefaultProtocol, null, null);
    }

    /// <summary>
    /// Creates an HttpClient for a specific proxy type.
    /// </summary>
    public HttpClient CreateHttpClient(ProxyType proxyType)
    {
        return CreateHttpClient(proxyType, _options.DefaultProtocol, null, null);
    }

    /// <summary>
    /// Creates an HttpClient with geo-targeting.
    /// </summary>
    public HttpClient CreateHttpClient(GeoTarget geoTarget)
    {
        return CreateHttpClient(_options.DefaultProxyType, _options.DefaultProtocol, geoTarget, null);
    }

    /// <summary>
    /// Creates an HttpClient for a specific country.
    /// </summary>
    public HttpClient CreateHttpClientForCountry(string countryCode)
    {
        return CreateHttpClient(GeoTarget.ForCountry(countryCode));
    }

    /// <summary>
    /// Creates an HttpClient with a sticky session.
    /// </summary>
    public HttpClient CreateHttpClientWithSession(string? sessionId = null)
    {
        return CreateHttpClient(_options.DefaultProxyType, _options.DefaultProtocol, null, sessionId ?? GenerateSessionId());
    }

    /// <summary>
    /// Creates a fully customized HttpClient.
    /// </summary>
    public HttpClient CreateHttpClient(
        ProxyType proxyType,
        ProxyProtocol protocol,
        GeoTarget? geoTarget,
        string? sessionId)
    {
        var credentials = GetCredentials(proxyType, protocol, geoTarget, sessionId);
        var handler = credentials.ToHttpClientHandler();

        return new HttpClient(handler, disposeHandler: true)
        {
            Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds)
        };
    }

    #endregion

    #region WebProxy Factory

    /// <summary>
    /// Creates a WebProxy with default settings.
    /// </summary>
    public WebProxy CreateWebProxy()
    {
        return GetCredentials().ToWebProxy();
    }

    /// <summary>
    /// Creates a WebProxy for a specific proxy type.
    /// </summary>
    public WebProxy CreateWebProxy(ProxyType proxyType)
    {
        return GetCredentials(proxyType).ToWebProxy();
    }

    /// <summary>
    /// Creates a WebProxy with geo-targeting.
    /// </summary>
    public WebProxy CreateWebProxy(GeoTarget geoTarget)
    {
        return GetCredentials(geoTarget).ToWebProxy();
    }

    /// <summary>
    /// Creates a WebProxy for a specific country.
    /// </summary>
    public WebProxy CreateWebProxyForCountry(string countryCode)
    {
        return CreateWebProxy(GeoTarget.ForCountry(countryCode));
    }

    #endregion

    #region Session Management

    /// <summary>
    /// Creates a new proxy session for maintaining the same IP across requests.
    /// Sessions automatically expire after 5 minutes of inactivity.
    /// </summary>
    public ProxySession CreateSession()
    {
        return CreateSession(_options.DefaultProxyType, null);
    }

    /// <summary>
    /// Creates a new proxy session with geo-targeting.
    /// </summary>
    public ProxySession CreateSession(GeoTarget geoTarget)
    {
        return CreateSession(_options.DefaultProxyType, geoTarget);
    }

    /// <summary>
    /// Creates a new proxy session for a specific proxy type.
    /// </summary>
    public ProxySession CreateSession(ProxyType proxyType, GeoTarget? geoTarget = null)
    {
        var sessionId = GenerateSessionId();
        var credentials = GetCredentials(proxyType, _options.DefaultProtocol, geoTarget, sessionId);
        return new ProxySession(this, credentials);
    }

    /// <summary>
    /// Generates a unique session ID.
    /// </summary>
    private static string GenerateSessionId()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 16);
    }

    #endregion

    #region Proxy Testing

    /// <summary>
    /// Tests the proxy connection and returns information about the exit IP.
    /// </summary>
    public async Task<ProxyTestResult> TestProxyAsync(CancellationToken cancellationToken = default)
    {
        return await TestProxyAsync(GetCredentials(), cancellationToken);
    }

    /// <summary>
    /// Tests a specific proxy configuration.
    /// </summary>
    public async Task<ProxyTestResult> TestProxyAsync(ProxyCredentials credentials, CancellationToken cancellationToken = default)
    {
        var result = new ProxyTestResult { Credentials = credentials };
        var sw = Stopwatch.StartNew();

        try
        {
            using var handler = credentials.ToHttpClientHandler();
            using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds) };

            var response = await GetStringWithCancellationAsync(client, "https://lumtest.com/myip.json", cancellationToken);
            sw.Stop();

            var json = JsonDocument.Parse(response);
            var root = json.RootElement;

            result.Success = true;
            result.ResponseTimeMs = sw.ElapsedMilliseconds;
            result.ExternalIp = root.TryGetProperty("ip", out var ip) ? ip.GetString() : null;
            result.Country = root.TryGetProperty("country", out var country) ? country.GetString() : null;
            result.City = root.TryGetProperty("geo", out var geo) && geo.TryGetProperty("city", out var city)
                ? city.GetString()
                : null;
        }
        catch (Exception ex)
        {
            sw.Stop();
            result.Success = false;
            result.ResponseTimeMs = sw.ElapsedMilliseconds;
            result.Error = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// Tests proxy connections to multiple countries and returns results.
    /// </summary>
    public async Task<IReadOnlyList<ProxyTestResult>> TestMultipleCountriesAsync(
        IEnumerable<string> countryCodes,
        CancellationToken cancellationToken = default)
    {
        var tasks = countryCodes.Select(async code =>
        {
            var credentials = GetCredentialsForCountry(code);
            return await TestProxyAsync(credentials, cancellationToken);
        });

        var results = await Task.WhenAll(tasks);
        return results;
    }

    #endregion

    #region Convenience Methods

    /// <summary>
    /// Makes a GET request through the proxy and returns the response body.
    /// </summary>
    public async Task<string> GetAsync(string url, CancellationToken cancellationToken = default)
    {
        using var client = CreateHttpClient();
        return await GetStringWithCancellationAsync(client, url, cancellationToken);
    }

    /// <summary>
    /// Makes a GET request through a proxy from a specific country.
    /// </summary>
    public async Task<string> GetFromCountryAsync(string url, string countryCode, CancellationToken cancellationToken = default)
    {
        using var client = CreateHttpClientForCountry(countryCode);
        return await GetStringWithCancellationAsync(client, url, cancellationToken);
    }

    /// <summary>
    /// Makes a GET request through a proxy with geo-targeting.
    /// </summary>
    public async Task<string> GetWithGeoTargetAsync(string url, GeoTarget geoTarget, CancellationToken cancellationToken = default)
    {
        using var client = CreateHttpClient(geoTarget);
        return await GetStringWithCancellationAsync(client, url, cancellationToken);
    }

    /// <summary>
    /// Helper method for GetStringAsync with cancellation token support (netstandard2.0 compatible).
    /// </summary>
    private static async Task<string> GetStringWithCancellationAsync(HttpClient client, string url, CancellationToken cancellationToken)
    {
#if NETSTANDARD2_0
        using var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#else
        return await client.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
#endif
    }

    /// <summary>
    /// Gets your current external IP as seen by the proxy.
    /// </summary>
    public async Task<string?> GetExternalIpAsync(CancellationToken cancellationToken = default)
    {
        var result = await TestProxyAsync(cancellationToken);
        return result.ExternalIp;
    }

    #endregion

    #region Proxy Type Shortcuts

    /// <summary>
    /// Gets credentials for residential proxies.
    /// Best for: General scraping, geo-targeting, ad verification.
    /// </summary>
    public ProxyCredentials GetResidentialCredentials(GeoTarget? geoTarget = null, string? sessionId = null)
    {
        return GetCredentials(ProxyType.Residential, ProxyProtocol.Http, geoTarget, sessionId);
    }

    /// <summary>
    /// Gets credentials for datacenter proxies.
    /// Best for: High-speed scraping, bulk requests.
    /// </summary>
    public ProxyCredentials GetDatacenterCredentials(GeoTarget? geoTarget = null, string? sessionId = null)
    {
        return GetCredentials(ProxyType.Datacenter, ProxyProtocol.Http, geoTarget, sessionId);
    }

    /// <summary>
    /// Gets credentials for ISP proxies.
    /// Best for: Long sessions, account management, sneaker bots.
    /// </summary>
    public ProxyCredentials GetISPCredentials(GeoTarget? geoTarget = null, string? sessionId = null)
    {
        return GetCredentials(ProxyType.ISP, ProxyProtocol.Http, geoTarget, sessionId);
    }

    /// <summary>
    /// Gets credentials for mobile proxies.
    /// Best for: Social media automation, mobile-specific content.
    /// </summary>
    public ProxyCredentials GetMobileCredentials(GeoTarget? geoTarget = null, string? sessionId = null)
    {
        return GetCredentials(ProxyType.Mobile, ProxyProtocol.Http, geoTarget, sessionId);
    }

    /// <summary>
    /// Creates an HttpClient for residential proxies.
    /// </summary>
    public HttpClient CreateResidentialHttpClient(GeoTarget? geoTarget = null)
    {
        return CreateHttpClient(ProxyType.Residential, ProxyProtocol.Http, geoTarget, null);
    }

    /// <summary>
    /// Creates an HttpClient for datacenter proxies.
    /// </summary>
    public HttpClient CreateDatacenterHttpClient(GeoTarget? geoTarget = null)
    {
        return CreateHttpClient(ProxyType.Datacenter, ProxyProtocol.Http, geoTarget, null);
    }

    /// <summary>
    /// Creates an HttpClient for ISP proxies.
    /// </summary>
    public HttpClient CreateISPHttpClient(GeoTarget? geoTarget = null)
    {
        return CreateHttpClient(ProxyType.ISP, ProxyProtocol.Http, geoTarget, null);
    }

    /// <summary>
    /// Creates an HttpClient for mobile proxies.
    /// </summary>
    public HttpClient CreateMobileHttpClient(GeoTarget? geoTarget = null)
    {
        return CreateHttpClient(ProxyType.Mobile, ProxyProtocol.Http, geoTarget, null);
    }

    #endregion

    public void Dispose()
    {
        if (!_disposed)
        {
            _testHttpClient.Dispose();
            _disposed = true;
        }
    }
}
