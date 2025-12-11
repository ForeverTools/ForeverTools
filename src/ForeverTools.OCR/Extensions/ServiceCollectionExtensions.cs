using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ForeverTools.OCR.Extensions;

/// <summary>
/// Extension methods for registering OCR services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the OCR client to the service collection with the specified API key.
    /// Get your API key at: https://aimlapi.com?via=forevertools
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">Your AI/ML API key.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsOcr(this IServiceCollection services, string apiKey)
    {
        return services.AddForeverToolsOcr(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Adds the OCR client to the service collection with configuration options.
    /// Get your API key at: https://aimlapi.com?via=forevertools
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure OCR options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsOcr(
        this IServiceCollection services,
        Action<OcrOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OcrOptions>>().Value;
            return new OcrClient(options);
        });

        return services;
    }

    /// <summary>
    /// Adds the OCR client to the service collection from configuration.
    /// Get your API key at: https://aimlapi.com?via=forevertools
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration section containing OCR settings.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// Expected configuration structure:
    /// <code>
    /// {
    ///   "OCR": {
    ///     "ApiKey": "your-api-key",
    ///     "DefaultModel": "gpt-4o",
    ///     "TimeoutSeconds": 60,
    ///     "MaxTokens": 4096,
    ///     "ImageDetail": "auto"
    ///   }
    /// }
    /// </code>
    /// </remarks>
    public static IServiceCollection AddForeverToolsOcr(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("OCR");
        services.Configure<OcrOptions>(section);

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OcrOptions>>().Value;
            return new OcrClient(options);
        });

        return services;
    }
}
