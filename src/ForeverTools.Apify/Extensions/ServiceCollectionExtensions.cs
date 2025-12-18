using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ForeverTools.Apify.Extensions;

/// <summary>
/// Extension methods for registering Apify services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Apify services to the service collection.
    /// Get your API token at: https://www.apify.com/?fpr=8hklqy
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="token">Your Apify API token.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsApify(this IServiceCollection services, string token)
    {
        return services.AddForeverToolsApify(options =>
        {
            options.Token = token;
        });
    }

    /// <summary>
    /// Adds Apify services to the service collection with configuration.
    /// Get your API token at: https://www.apify.com/?fpr=8hklqy
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsApify(this IServiceCollection services, Action<ApifyOptions> configure)
    {
        services.Configure(configure);

        services.AddHttpClient<ApifyClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ApifyOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ApifyOptions>>().Value;
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(ApifyClient));
            return new ApifyClient(options, httpClient);
        });

        return services;
    }

    /// <summary>
    /// Adds Apify services to the service collection from IConfiguration.
    /// Reads from the "Apify" section by default.
    /// Get your API token at: https://www.apify.com/?fpr=8hklqy
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The configuration section name.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsApify(this IServiceCollection services, IConfiguration configuration, string sectionName = ApifyOptions.SectionName)
    {
        var section = configuration.GetSection(sectionName);
        services.Configure<ApifyOptions>(section);

        services.AddHttpClient<ApifyClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ApifyOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ApifyOptions>>().Value;
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(ApifyClient));
            return new ApifyClient(options, httpClient);
        });

        return services;
    }
}
