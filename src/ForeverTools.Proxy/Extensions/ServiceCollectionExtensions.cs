using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForeverTools.Proxy.Extensions;

/// <summary>
/// Extension methods for registering BrightDataClient with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the BrightDataClient to the service collection with the specified credentials.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="customerId">Your BrightData customer ID.</param>
    /// <param name="zone">The zone name.</param>
    /// <param name="password">The zone password.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsProxy(
        this IServiceCollection services,
        string customerId,
        string zone,
        string password)
    {
        return services.AddForeverToolsProxy(options =>
        {
            options.CustomerId = customerId;
            options.Zone = zone;
            options.Password = password;
        });
    }

    /// <summary>
    /// Adds the BrightDataClient to the service collection with configuration options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure the proxy options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsProxy(
        this IServiceCollection services,
        Action<ProxyOptions> configureOptions)
    {
        var options = new ProxyOptions();
        configureOptions(options);

        services.AddSingleton(options);
        services.AddSingleton<BrightDataClient>();

        return services;
    }

    /// <summary>
    /// Adds the BrightDataClient to the service collection from IConfiguration.
    /// Expects configuration section "Proxy" or "BrightData" with properties:
    /// CustomerId, Zone, Password, DefaultProxyType, DefaultCountry, etc.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <param name="sectionName">The configuration section name. Defaults to "Proxy".</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsProxy(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Proxy")
    {
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            section = configuration.GetSection("BrightData");
        }

        var options = new ProxyOptions();
        section.Bind(options);

        // Allow environment variable overrides
        var envCustomerId = Environment.GetEnvironmentVariable("BRIGHTDATA_CUSTOMER_ID");
        var envZone = Environment.GetEnvironmentVariable("BRIGHTDATA_ZONE");
        var envPassword = Environment.GetEnvironmentVariable("BRIGHTDATA_PASSWORD");

        if (!string.IsNullOrEmpty(envCustomerId))
            options.CustomerId = envCustomerId;
        if (!string.IsNullOrEmpty(envZone))
            options.Zone = envZone;
        if (!string.IsNullOrEmpty(envPassword))
            options.Password = envPassword;

        services.AddSingleton(options);
        services.AddSingleton<BrightDataClient>();

        return services;
    }

    /// <summary>
    /// Adds a named HttpClient configured to use BrightData proxies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="name">The name of the HttpClient.</param>
    /// <param name="customerId">Your BrightData customer ID.</param>
    /// <param name="zone">The zone name.</param>
    /// <param name="password">The zone password.</param>
    /// <param name="configureClient">Optional action to configure the HttpClient.</param>
    /// <returns>The IHttpClientBuilder for further configuration.</returns>
    public static IHttpClientBuilder AddBrightDataHttpClient(
        this IServiceCollection services,
        string name,
        string customerId,
        string zone,
        string password,
        Action<HttpClient>? configureClient = null)
    {
        var client = new BrightDataClient(customerId, zone, password);
        var credentials = client.GetCredentials();

        return services.AddHttpClient(name, httpClient =>
        {
            configureClient?.Invoke(httpClient);
        })
        .ConfigurePrimaryHttpMessageHandler(() => credentials.ToHttpClientHandler());
    }

    /// <summary>
    /// Adds a typed HttpClient configured to use BrightData proxies.
    /// </summary>
    /// <typeparam name="TClient">The type of the HttpClient wrapper.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="customerId">Your BrightData customer ID.</param>
    /// <param name="zone">The zone name.</param>
    /// <param name="password">The zone password.</param>
    /// <param name="configureClient">Optional action to configure the HttpClient.</param>
    /// <returns>The IHttpClientBuilder for further configuration.</returns>
    public static IHttpClientBuilder AddBrightDataHttpClient<TClient>(
        this IServiceCollection services,
        string customerId,
        string zone,
        string password,
        Action<HttpClient>? configureClient = null)
        where TClient : class
    {
        var client = new BrightDataClient(customerId, zone, password);
        var credentials = client.GetCredentials();

        return services.AddHttpClient<TClient>(httpClient =>
        {
            configureClient?.Invoke(httpClient);
        })
        .ConfigurePrimaryHttpMessageHandler(() => credentials.ToHttpClientHandler());
    }
}
