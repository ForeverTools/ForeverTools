using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ForeverTools.APILayer.Extensions;

/// <summary>
/// Extension methods for registering APILayer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds APILayer services to the service collection.
    /// Get your API key at: https://apilayer.com?fpr=chris72
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">Your APILayer API key.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsApiLayer(this IServiceCollection services, string apiKey)
    {
        return services.AddForeverToolsApiLayer(options =>
        {
            options.ApiKey = apiKey;
        });
    }

    /// <summary>
    /// Adds APILayer services to the service collection with configuration.
    /// Get your API key at: https://apilayer.com?fpr=chris72
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsApiLayer(this IServiceCollection services, Action<ApiLayerOptions> configure)
    {
        services.Configure(configure);

        services.AddHttpClient<ApiLayerClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ApiLayerOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            client.DefaultRequestHeaders.Add("apikey", options.ApiKey);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ApiLayerOptions>>().Value;
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(ApiLayerClient));
            return new ApiLayerClient(options, httpClient);
        });

        return services;
    }

    /// <summary>
    /// Adds APILayer services to the service collection from IConfiguration.
    /// Reads from the "APILayer" section by default.
    /// Get your API key at: https://apilayer.com?fpr=chris72
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The configuration section name.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsApiLayer(this IServiceCollection services, IConfiguration configuration, string sectionName = ApiLayerOptions.SectionName)
    {
        var section = configuration.GetSection(sectionName);
        services.Configure<ApiLayerOptions>(section);

        services.AddHttpClient<ApiLayerClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ApiLayerOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            client.DefaultRequestHeaders.Add("apikey", options.ApiKey);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ApiLayerOptions>>().Value;
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(ApiLayerClient));
            return new ApiLayerClient(options, httpClient);
        });

        return services;
    }
}
