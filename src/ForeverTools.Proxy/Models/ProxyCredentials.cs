using System.Net;

namespace ForeverTools.Proxy.Models;

/// <summary>
/// Represents proxy credentials with targeting options.
/// </summary>
public class ProxyCredentials
{
    /// <summary>
    /// The proxy host address.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// The proxy port.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// The username for authentication (includes targeting options).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The password for authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The proxy type being used.
    /// </summary>
    public ProxyType ProxyType { get; set; }

    /// <summary>
    /// The protocol being used.
    /// </summary>
    public ProxyProtocol Protocol { get; set; }

    /// <summary>
    /// The geo-targeting applied (if any).
    /// </summary>
    public GeoTarget? GeoTarget { get; set; }

    /// <summary>
    /// The session ID (if using sticky session).
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// Gets the proxy URL in the format protocol://username:password@host:port
    /// </summary>
    public string ProxyUrl
    {
        get
        {
            var protocol = Protocol == ProxyProtocol.Socks5 ? "socks5" : "http";
            return $"{protocol}://{Uri.EscapeDataString(Username)}:{Uri.EscapeDataString(Password)}@{Host}:{Port}";
        }
    }

    /// <summary>
    /// Gets the proxy address in the format host:port
    /// </summary>
    public string ProxyAddress => $"{Host}:{Port}";

    /// <summary>
    /// Creates a WebProxy instance from these credentials.
    /// </summary>
    public WebProxy ToWebProxy()
    {
        var proxy = new WebProxy(Host, Port)
        {
            Credentials = new NetworkCredential(Username, Password)
        };
        return proxy;
    }

    /// <summary>
    /// Creates an HttpClientHandler configured with this proxy.
    /// </summary>
    public HttpClientHandler ToHttpClientHandler()
    {
        return new HttpClientHandler
        {
            Proxy = ToWebProxy(),
            UseProxy = true
        };
    }

    /// <summary>
    /// Creates a curl command string for testing.
    /// </summary>
    public string ToCurlCommand(string targetUrl = "https://httpbin.org/ip")
    {
        return $"curl \"{targetUrl}\" --proxy {Host}:{Port} --proxy-user \"{Username}:{Password}\"";
    }
}

/// <summary>
/// Represents a proxy session for maintaining the same IP across requests.
/// </summary>
public class ProxySession : IDisposable
{
    private readonly BrightDataClient _client;
    private readonly ProxyCredentials _credentials;
    private readonly DateTime _createdAt;
    private bool _disposed;

    internal ProxySession(BrightDataClient client, ProxyCredentials credentials)
    {
        _client = client;
        _credentials = credentials;
        _createdAt = DateTime.UtcNow;
        SessionId = credentials.SessionId ?? Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// The unique session identifier.
    /// </summary>
    public string SessionId { get; }

    /// <summary>
    /// Gets the credentials for this session.
    /// </summary>
    public ProxyCredentials Credentials => _credentials;

    /// <summary>
    /// When the session was created.
    /// </summary>
    public DateTime CreatedAt => _createdAt;

    /// <summary>
    /// How long the session has been active.
    /// </summary>
    public TimeSpan Age => DateTime.UtcNow - _createdAt;

    /// <summary>
    /// Whether the session is still valid (under 5 minutes by default).
    /// </summary>
    public bool IsValid => Age.TotalMinutes < 5;

    /// <summary>
    /// Creates a new HttpClient using this session's proxy.
    /// </summary>
    public HttpClient CreateHttpClient()
    {
        return new HttpClient(_credentials.ToHttpClientHandler(), disposeHandler: true)
        {
            Timeout = TimeSpan.FromSeconds(60)
        };
    }

    /// <summary>
    /// Creates a WebProxy for use with existing HttpClient or other clients.
    /// </summary>
    public WebProxy GetWebProxy() => _credentials.ToWebProxy();

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}

/// <summary>
/// Result of a proxy test request.
/// </summary>
public class ProxyTestResult
{
    /// <summary>
    /// Whether the proxy connection was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The external IP address seen by the target.
    /// </summary>
    public string? ExternalIp { get; set; }

    /// <summary>
    /// The country of the proxy IP.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// The city of the proxy IP.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Response time in milliseconds.
    /// </summary>
    public long ResponseTimeMs { get; set; }

    /// <summary>
    /// Error message if the test failed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// The credentials used for the test.
    /// </summary>
    public ProxyCredentials? Credentials { get; set; }
}
