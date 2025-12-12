namespace ForeverTools.Proxy;

/// <summary>
/// Types of proxies available through BrightData.
/// </summary>
public enum ProxyType
{
    /// <summary>
    /// Residential proxies - Real IPs from ISP customers.
    /// Best for: General scraping, geo-targeting, ad verification.
    /// </summary>
    Residential,

    /// <summary>
    /// Datacenter proxies - IPs from data centers.
    /// Best for: High-speed scraping, bulk requests.
    /// </summary>
    Datacenter,

    /// <summary>
    /// ISP proxies - Static residential IPs.
    /// Best for: Long sessions, account management, sneaker bots.
    /// </summary>
    ISP,

    /// <summary>
    /// Mobile proxies - Real 3G/4G/5G mobile IPs.
    /// Best for: Social media automation, mobile-specific content.
    /// </summary>
    Mobile
}

/// <summary>
/// Protocol for proxy connections.
/// </summary>
public enum ProxyProtocol
{
    /// <summary>
    /// HTTP/HTTPS proxy protocol.
    /// </summary>
    Http,

    /// <summary>
    /// SOCKS5 proxy protocol.
    /// </summary>
    Socks5
}

/// <summary>
/// Extension methods for proxy types.
/// </summary>
public static class ProxyTypeExtensions
{
    /// <summary>
    /// Gets the default port for the proxy type.
    /// </summary>
    public static int GetDefaultPort(this ProxyType proxyType, ProxyProtocol protocol = ProxyProtocol.Http)
    {
        if (protocol == ProxyProtocol.Socks5)
        {
            return BrightDataPorts.Socks5;
        }

        return proxyType switch
        {
            ProxyType.Residential => BrightDataPorts.Residential,
            ProxyType.Datacenter => BrightDataPorts.Datacenter,
            ProxyType.ISP => BrightDataPorts.ISP,
            ProxyType.Mobile => BrightDataPorts.Mobile,
            _ => BrightDataPorts.Residential
        };
    }
}

/// <summary>
/// Default ports for BrightData proxy types.
/// </summary>
public static class BrightDataPorts
{
    /// <summary>Port for residential proxies.</summary>
    public const int Residential = 33335;

    /// <summary>Port for datacenter proxies.</summary>
    public const int Datacenter = 22225;

    /// <summary>Port for ISP proxies.</summary>
    public const int ISP = 22225;

    /// <summary>Port for mobile proxies.</summary>
    public const int Mobile = 33335;

    /// <summary>Port for SOCKS5 connections.</summary>
    public const int Socks5 = 22228;
}
