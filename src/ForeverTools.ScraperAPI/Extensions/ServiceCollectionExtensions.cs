using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ForeverTools.ScraperAPI.Extensions;

/// <summary>
/// Extension methods for registering ScraperAPI services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds ScraperAPI services to the service collection.
    /// Get your API key at: https://www.scraperapi.com/signup?fp_ref=chris88
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">Your ScraperAPI API key.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsScraperApi(this IServiceCollection services, string apiKey)
    {
        return services.AddForeverToolsScraperApi(options =>
        {
            options.ApiKey = apiKey;
        });
    }

    /// <summary>
    /// Adds ScraperAPI services to the service collection with configuration.
    /// Get your API key at: https://www.scraperapi.com/signup?fp_ref=chris88
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsScraperApi(this IServiceCollection services, Action<ScraperApiOptions> configure)
    {
        services.Configure(configure);

        services.AddHttpClient<ScraperApiClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ScraperApiOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ScraperApiOptions>>().Value;
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(ScraperApiClient));
            return new ScraperApiClient(options, httpClient);
        });

        return services;
    }

    /// <summary>
    /// Adds ScraperAPI services to the service collection from IConfiguration.
    /// Reads from the "ScraperAPI" section by default.
    /// Get your API key at: https://www.scraperapi.com/signup?fp_ref=chris88
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The configuration section name.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsScraperApi(this IServiceCollection services, IConfiguration configuration, string sectionName = ScraperApiOptions.SectionName)
    {
        var section = configuration.GetSection(sectionName);
        services.Configure<ScraperApiOptions>(section);

        services.AddHttpClient<ScraperApiClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ScraperApiOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ScraperApiOptions>>().Value;
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(ScraperApiClient));
            return new ScraperApiClient(options, httpClient);
        });

        return services;
    }
}
