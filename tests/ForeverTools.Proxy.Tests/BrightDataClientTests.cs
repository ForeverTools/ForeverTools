using ForeverTools.Proxy;
using ForeverTools.Proxy.Extensions;
using ForeverTools.Proxy.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ForeverTools.Proxy.Tests;

public class BrightDataClientTests
{
    private const string TestCustomerId = "test_customer_123";
    private const string TestZone = "test_zone";
    private const string TestPassword = "test_password";

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidCredentials_CreatesClient()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithOptions_CreatesClient()
    {
        var options = new ProxyOptions
        {
            CustomerId = TestCustomerId,
            Zone = TestZone,
            Password = TestPassword,
            DefaultProxyType = ProxyType.Datacenter,
            DefaultCountry = "us"
        };

        var client = new BrightDataClient(options);
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithNullCustomerId_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new BrightDataClient(string.Empty, TestZone, TestPassword));
    }

    [Fact]
    public void Constructor_WithNullZone_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new BrightDataClient(TestCustomerId, string.Empty, TestPassword));
    }

    [Fact]
    public void Constructor_WithNullPassword_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new BrightDataClient(TestCustomerId, TestZone, string.Empty));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BrightDataClient((ProxyOptions)null!));
    }

    #endregion

    #region Credential Generation Tests

    [Fact]
    public void GetCredentials_ReturnsValidCredentials()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var credentials = client.GetCredentials();

        Assert.NotNull(credentials);
        Assert.Equal("brd.superproxy.io", credentials.Host);
        Assert.Equal(TestPassword, credentials.Password);
        Assert.Contains(TestCustomerId, credentials.Username);
        Assert.Contains(TestZone, credentials.Username);
    }

    [Fact]
    public void GetCredentials_ForResidential_UsesCorrectPort()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var credentials = client.GetCredentials(ProxyType.Residential);

        Assert.Equal(BrightDataPorts.Residential, credentials.Port);
        Assert.Equal(ProxyType.Residential, credentials.ProxyType);
    }

    [Fact]
    public void GetCredentials_ForDatacenter_UsesCorrectPort()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var credentials = client.GetCredentials(ProxyType.Datacenter);

        Assert.Equal(BrightDataPorts.Datacenter, credentials.Port);
        Assert.Equal(ProxyType.Datacenter, credentials.ProxyType);
    }

    [Fact]
    public void GetCredentials_WithCountry_IncludesCountryInUsername()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var credentials = client.GetCredentialsForCountry("us");

        Assert.Contains("country-us", credentials.Username);
    }

    [Fact]
    public void GetCredentials_WithGeoTarget_IncludesAllTargeting()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var geoTarget = new GeoTarget
        {
            Country = "us",
            State = "ca",
            City = "los_angeles"
        };

        var credentials = client.GetCredentials(geoTarget);

        Assert.Contains("country-us", credentials.Username);
        Assert.Contains("state-ca", credentials.Username);
        Assert.Contains("city-los_angeles", credentials.Username);
    }

    [Fact]
    public void GetCredentials_WithSession_IncludesSessionInUsername()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var credentials = client.GetCredentialsWithSession("mysession123");

        Assert.Contains("session-mysession123", credentials.Username);
        Assert.Equal("mysession123", credentials.SessionId);
    }

    [Fact]
    public void GetCredentials_WithAutoSession_GeneratesSessionId()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var credentials = client.GetCredentialsWithSession();

        Assert.NotNull(credentials.SessionId);
        Assert.Contains($"session-{credentials.SessionId}", credentials.Username);
    }

    [Fact]
    public void GetCredentials_UsernameFormat_IsCorrect()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var credentials = client.GetCredentials();

        Assert.StartsWith("brd-customer-", credentials.Username);
        Assert.Contains($"customer-{TestCustomerId}", credentials.Username);
        Assert.Contains($"zone-{TestZone}", credentials.Username);
    }

    #endregion

    #region ProxyCredentials Model Tests

    [Fact]
    public void ProxyCredentials_ProxyUrl_FormatsCorrectly()
    {
        var credentials = new ProxyCredentials
        {
            Host = "proxy.example.com",
            Port = 8080,
            Username = "user",
            Password = "pass",
            Protocol = ProxyProtocol.Http
        };

        Assert.Equal("http://user:pass@proxy.example.com:8080", credentials.ProxyUrl);
    }

    [Fact]
    public void ProxyCredentials_ProxyUrl_Socks5_FormatsCorrectly()
    {
        var credentials = new ProxyCredentials
        {
            Host = "proxy.example.com",
            Port = 1080,
            Username = "user",
            Password = "pass",
            Protocol = ProxyProtocol.Socks5
        };

        Assert.Equal("socks5://user:pass@proxy.example.com:1080", credentials.ProxyUrl);
    }

    [Fact]
    public void ProxyCredentials_ProxyAddress_FormatsCorrectly()
    {
        var credentials = new ProxyCredentials
        {
            Host = "proxy.example.com",
            Port = 8080
        };

        Assert.Equal("proxy.example.com:8080", credentials.ProxyAddress);
    }

    [Fact]
    public void ProxyCredentials_ToWebProxy_CreatesValidProxy()
    {
        var credentials = new ProxyCredentials
        {
            Host = "proxy.example.com",
            Port = 8080,
            Username = "user",
            Password = "pass"
        };

        var webProxy = credentials.ToWebProxy();

        Assert.NotNull(webProxy);
        Assert.NotNull(webProxy.Credentials);
    }

    [Fact]
    public void ProxyCredentials_ToHttpClientHandler_CreatesValidHandler()
    {
        var credentials = new ProxyCredentials
        {
            Host = "proxy.example.com",
            Port = 8080,
            Username = "user",
            Password = "pass"
        };

        var handler = credentials.ToHttpClientHandler();

        Assert.NotNull(handler);
        Assert.True(handler.UseProxy);
        Assert.NotNull(handler.Proxy);
    }

    [Fact]
    public void ProxyCredentials_ToCurlCommand_FormatsCorrectly()
    {
        var credentials = new ProxyCredentials
        {
            Host = "proxy.example.com",
            Port = 8080,
            Username = "user",
            Password = "pass"
        };

        var curl = credentials.ToCurlCommand("https://example.com");

        Assert.Contains("proxy.example.com:8080", curl);
        Assert.Contains("user:pass", curl);
        Assert.Contains("https://example.com", curl);
    }

    #endregion

    #region GeoTarget Tests

    [Fact]
    public void GeoTarget_ForCountry_CreatesCorrectTarget()
    {
        var target = GeoTarget.ForCountry("US");

        Assert.Equal("us", target.Country);
        Assert.Null(target.City);
        Assert.Null(target.State);
    }

    [Fact]
    public void GeoTarget_ForCity_CreatesCorrectTarget()
    {
        var target = GeoTarget.ForCity("US", "New York");

        Assert.Equal("us", target.Country);
        Assert.Equal("new_york", target.City);
    }

    [Fact]
    public void GeoTarget_ForUSState_CreatesCorrectTarget()
    {
        var target = GeoTarget.ForUSState("CA");

        Assert.Equal("us", target.Country);
        Assert.Equal("ca", target.State);
    }

    [Fact]
    public void GeoTarget_ForEU_CreatesCorrectTarget()
    {
        var target = GeoTarget.ForEU();

        Assert.Equal("eu", target.Country);
    }

    [Fact]
    public void GeoTarget_Locations_AreCorrect()
    {
        Assert.Equal("us", GeoTarget.Locations.UnitedStates.Country);
        Assert.Equal("uk", GeoTarget.Locations.UnitedKingdom.Country);
        Assert.Equal("de", GeoTarget.Locations.Germany.Country);
        Assert.Equal("new_york", GeoTarget.Locations.NewYork.City);
        Assert.Equal("london", GeoTarget.Locations.London.City);
        Assert.Equal("ca", GeoTarget.Locations.California.State);
    }

    #endregion

    #region ProxyType Extension Tests

    [Fact]
    public void ProxyType_GetDefaultPort_ReturnsCorrectPorts()
    {
        Assert.Equal(BrightDataPorts.Residential, ProxyType.Residential.GetDefaultPort());
        Assert.Equal(BrightDataPorts.Datacenter, ProxyType.Datacenter.GetDefaultPort());
        Assert.Equal(BrightDataPorts.ISP, ProxyType.ISP.GetDefaultPort());
        Assert.Equal(BrightDataPorts.Mobile, ProxyType.Mobile.GetDefaultPort());
    }

    [Fact]
    public void ProxyType_GetDefaultPort_Socks5_ReturnsCorrectPort()
    {
        Assert.Equal(BrightDataPorts.Socks5, ProxyType.Residential.GetDefaultPort(ProxyProtocol.Socks5));
        Assert.Equal(BrightDataPorts.Socks5, ProxyType.Datacenter.GetDefaultPort(ProxyProtocol.Socks5));
    }

    #endregion

    #region HttpClient Factory Tests

    [Fact]
    public void CreateHttpClient_ReturnsConfiguredClient()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        using var httpClient = client.CreateHttpClient();

        Assert.NotNull(httpClient);
    }

    [Fact]
    public void CreateHttpClient_ForProxyType_ReturnsConfiguredClient()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        using var httpClient = client.CreateHttpClient(ProxyType.Datacenter);

        Assert.NotNull(httpClient);
    }

    [Fact]
    public void CreateHttpClient_WithGeoTarget_ReturnsConfiguredClient()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        using var httpClient = client.CreateHttpClient(GeoTarget.Locations.UnitedStates);

        Assert.NotNull(httpClient);
    }

    [Fact]
    public void CreateHttpClientForCountry_ReturnsConfiguredClient()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        using var httpClient = client.CreateHttpClientForCountry("uk");

        Assert.NotNull(httpClient);
    }

    #endregion

    #region WebProxy Factory Tests

    [Fact]
    public void CreateWebProxy_ReturnsConfiguredProxy()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var webProxy = client.CreateWebProxy();

        Assert.NotNull(webProxy);
        Assert.NotNull(webProxy.Credentials);
    }

    [Fact]
    public void CreateWebProxy_ForProxyType_ReturnsConfiguredProxy()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var webProxy = client.CreateWebProxy(ProxyType.ISP);

        Assert.NotNull(webProxy);
    }

    [Fact]
    public void CreateWebProxyForCountry_ReturnsConfiguredProxy()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var webProxy = client.CreateWebProxyForCountry("de");

        Assert.NotNull(webProxy);
    }

    #endregion

    #region Session Management Tests

    [Fact]
    public void CreateSession_ReturnsValidSession()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        using var session = client.CreateSession();

        Assert.NotNull(session);
        Assert.NotNull(session.SessionId);
        Assert.NotNull(session.Credentials);
        Assert.True(session.IsValid);
    }

    [Fact]
    public void CreateSession_WithGeoTarget_IncludesTargeting()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        using var session = client.CreateSession(GeoTarget.Locations.UnitedKingdom);

        Assert.Contains("country-uk", session.Credentials.Username);
    }

    [Fact]
    public void ProxySession_CreateHttpClient_ReturnsConfiguredClient()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        using var session = client.CreateSession();
        using var httpClient = session.CreateHttpClient();

        Assert.NotNull(httpClient);
    }

    [Fact]
    public void ProxySession_GetWebProxy_ReturnsProxy()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        using var session = client.CreateSession();
        var webProxy = session.GetWebProxy();

        Assert.NotNull(webProxy);
    }

    #endregion

    #region Proxy Type Shortcut Tests

    [Fact]
    public void GetResidentialCredentials_ReturnsCorrectType()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var credentials = client.GetResidentialCredentials();

        Assert.Equal(ProxyType.Residential, credentials.ProxyType);
        Assert.Equal(BrightDataPorts.Residential, credentials.Port);
    }

    [Fact]
    public void GetDatacenterCredentials_ReturnsCorrectType()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var credentials = client.GetDatacenterCredentials();

        Assert.Equal(ProxyType.Datacenter, credentials.ProxyType);
        Assert.Equal(BrightDataPorts.Datacenter, credentials.Port);
    }

    [Fact]
    public void GetISPCredentials_ReturnsCorrectType()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var credentials = client.GetISPCredentials();

        Assert.Equal(ProxyType.ISP, credentials.ProxyType);
    }

    [Fact]
    public void GetMobileCredentials_ReturnsCorrectType()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var credentials = client.GetMobileCredentials();

        Assert.Equal(ProxyType.Mobile, credentials.ProxyType);
    }

    #endregion

    #region Dependency Injection Tests

    [Fact]
    public void AddForeverToolsProxy_WithCredentials_RegistersServices()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsProxy(TestCustomerId, TestZone, TestPassword);

        var provider = services.BuildServiceProvider();
        var client = provider.GetService<BrightDataClient>();

        Assert.NotNull(client);
    }

    [Fact]
    public void AddForeverToolsProxy_WithOptions_RegistersServices()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsProxy(options =>
        {
            options.CustomerId = TestCustomerId;
            options.Zone = TestZone;
            options.Password = TestPassword;
            options.DefaultProxyType = ProxyType.Datacenter;
        });

        var provider = services.BuildServiceProvider();
        var client = provider.GetService<BrightDataClient>();
        var options = provider.GetService<ProxyOptions>();

        Assert.NotNull(client);
        Assert.NotNull(options);
        Assert.Equal(ProxyType.Datacenter, options.DefaultProxyType);
    }

    [Fact]
    public void AddForeverToolsProxy_RegistersSingleton()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsProxy(TestCustomerId, TestZone, TestPassword);

        var provider = services.BuildServiceProvider();
        var client1 = provider.GetService<BrightDataClient>();
        var client2 = provider.GetService<BrightDataClient>();

        Assert.Same(client1, client2);
    }

    #endregion

    #region ProxyTestResult Tests

    [Fact]
    public void ProxyTestResult_HasCorrectDefaults()
    {
        var result = new ProxyTestResult();

        Assert.False(result.Success);
        Assert.Null(result.ExternalIp);
        Assert.Null(result.Country);
        Assert.Null(result.City);
        Assert.Equal(0, result.ResponseTimeMs);
        Assert.Null(result.Error);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        client.Dispose();
        client.Dispose(); // Should not throw
    }

    [Fact]
    public void ProxySession_Dispose_CanBeCalledMultipleTimes()
    {
        var client = new BrightDataClient(TestCustomerId, TestZone, TestPassword);
        var session = client.CreateSession();
        session.Dispose();
        session.Dispose(); // Should not throw
    }

    #endregion
}
