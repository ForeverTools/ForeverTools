using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ForeverTools.Postmark.Extensions;

/// <summary>
/// Extension methods for registering Postmark services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Postmark email services to the service collection.
    /// Get your API key at: https://www.postmarkapp.com?via=8ac781
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serverToken">Your Postmark Server API Token.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsPostmark(this IServiceCollection services, string serverToken)
    {
        return services.AddForeverToolsPostmark(options =>
        {
            options.ServerToken = serverToken;
        });
    }

    /// <summary>
    /// Adds Postmark email services to the service collection with configuration.
    /// Get your API key at: https://www.postmarkapp.com?via=8ac781
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsPostmark(this IServiceCollection services, Action<PostmarkOptions> configure)
    {
        services.Configure(configure);

        services.AddHttpClient<PostmarkClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<PostmarkOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("X-Postmark-Server-Token", options.ServerToken);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<PostmarkOptions>>().Value;
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(PostmarkClient));
            return new PostmarkClient(options, httpClient);
        });

        return services;
    }

    /// <summary>
    /// Adds Postmark email services to the service collection from IConfiguration.
    /// Reads from the "Postmark" section by default.
    /// Get your API key at: https://www.postmarkapp.com?via=8ac781
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The configuration section name.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsPostmark(this IServiceCollection services, IConfiguration configuration, string sectionName = PostmarkOptions.SectionName)
    {
        var section = configuration.GetSection(sectionName);
        services.Configure<PostmarkOptions>(section);

        services.AddHttpClient<PostmarkClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<PostmarkOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("X-Postmark-Server-Token", options.ServerToken);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<PostmarkOptions>>().Value;
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(PostmarkClient));
            return new PostmarkClient(options, httpClient);
        });

        return services;
    }
}
