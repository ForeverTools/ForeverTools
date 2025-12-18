using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForeverTools.Translate.Extensions;

/// <summary>
/// Extension methods for registering TranslationClient with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the TranslationClient to the service collection with the specified API key.
    /// Get your API key at: https://aimlapi.com?via=forevertools
    /// </summary>
    public static IServiceCollection AddForeverToolsTranslation(
        this IServiceCollection services,
        string apiKey)
    {
        return services.AddForeverToolsTranslation(options =>
        {
            options.ApiKey = apiKey;
        });
    }

    /// <summary>
    /// Adds the TranslationClient to the service collection with configuration options.
    /// </summary>
    public static IServiceCollection AddForeverToolsTranslation(
        this IServiceCollection services,
        Action<TranslationOptions> configureOptions)
    {
        var options = new TranslationOptions();
        configureOptions(options);

        services.AddSingleton(options);
        services.AddSingleton<TranslationClient>();

        return services;
    }

    /// <summary>
    /// Adds the TranslationClient to the service collection from IConfiguration.
    /// Expects configuration section "Translation" or "AimlApi" with properties:
    /// ApiKey, DefaultModel, DefaultTargetLanguage, etc.
    /// </summary>
    public static IServiceCollection AddForeverToolsTranslation(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Translation")
    {
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            section = configuration.GetSection("AimlApi");
        }

        var options = new TranslationOptions();
        section.Bind(options);

        // Allow environment variable overrides
        var envApiKey = Environment.GetEnvironmentVariable("AIML_API_KEY")
            ?? Environment.GetEnvironmentVariable("TRANSLATION_API_KEY");

        if (!string.IsNullOrEmpty(envApiKey))
            options.ApiKey = envApiKey;

        services.AddSingleton(options);
        services.AddSingleton<TranslationClient>();

        return services;
    }

    /// <summary>
    /// Adds the TranslationClient with a specific default target language.
    /// </summary>
    public static IServiceCollection AddForeverToolsTranslation(
        this IServiceCollection services,
        string apiKey,
        string defaultTargetLanguage)
    {
        return services.AddForeverToolsTranslation(options =>
        {
            options.ApiKey = apiKey;
            options.DefaultTargetLanguage = defaultTargetLanguage;
        });
    }

    /// <summary>
    /// Adds the TranslationClient configured for a specific model.
    /// </summary>
    public static IServiceCollection AddForeverToolsTranslation(
        this IServiceCollection services,
        string apiKey,
        string defaultTargetLanguage,
        string model)
    {
        return services.AddForeverToolsTranslation(options =>
        {
            options.ApiKey = apiKey;
            options.DefaultTargetLanguage = defaultTargetLanguage;
            options.DefaultModel = model;
        });
    }
}
