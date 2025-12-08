using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ForeverTools.AIML;

/// <summary>
/// Extension methods for registering AI/ML API services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the AI/ML API client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsAiml(this IServiceCollection services, string apiKey)
    {
        return services.AddForeverToolsAiml(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Adds the AI/ML API client to the service collection with configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure the client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsAiml(
        this IServiceCollection services,
        Action<AimlApiOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<AimlApiClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AimlApiOptions>>().Value;
            return new AimlApiClient(options);
        });
        return services;
    }

    /// <summary>
    /// Adds the AI/ML API client to the service collection, binding from configuration.
    /// Expects configuration section "AimlApi" with ApiKey property.
    /// </summary>
    /// <example>
    /// appsettings.json:
    /// {
    ///   "AimlApi": {
    ///     "ApiKey": "your-key-here",
    ///     "DefaultChatModel": "gpt-4o"
    ///   }
    /// }
    /// </example>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsAiml(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        services.Configure<AimlApiOptions>(configuration.GetSection(AimlApiOptions.SectionName));
        services.AddSingleton<AimlApiClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AimlApiOptions>>().Value;
            return new AimlApiClient(options);
        });
        return services;
    }
}
