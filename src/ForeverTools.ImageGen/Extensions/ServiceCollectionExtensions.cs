using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForeverTools.ImageGen.Extensions;

/// <summary>
/// Extension methods for registering ImageGen services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds ForeverTools.ImageGen services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">Your AI/ML API key.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsImageGen(this IServiceCollection services, string apiKey)
    {
        return services.AddForeverToolsImageGen(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Adds ForeverTools.ImageGen services with configuration options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsImageGen(
        this IServiceCollection services,
        Action<ImageGenOptions> configureOptions)
    {
        var options = new ImageGenOptions();
        configureOptions(options);

        services.AddSingleton(options);
        services.AddSingleton<ImageGenClient>();

        return services;
    }

    /// <summary>
    /// Adds ForeverTools.ImageGen services from IConfiguration.
    /// Reads from "ImageGen" section by default.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The configuration section name. Defaults to "ImageGen".</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsImageGen(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "ImageGen")
    {
        var options = new ImageGenOptions();
        configuration.GetSection(sectionName).Bind(options);

        // Also check for common environment variable patterns
        if (string.IsNullOrEmpty(options.ApiKey))
        {
            options.ApiKey = configuration["ImageGen:ApiKey"]
                ?? configuration["AIML_API_KEY"]
                ?? configuration["AIML:ApiKey"]
                ?? string.Empty;
        }

        services.AddSingleton(options);
        services.AddSingleton<ImageGenClient>();

        return services;
    }
}
