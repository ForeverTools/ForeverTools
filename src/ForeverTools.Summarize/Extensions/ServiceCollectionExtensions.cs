using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForeverTools.Summarize.Extensions;

/// <summary>
/// Extension methods for registering SummarizeClient with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the SummarizeClient to the service collection with the specified API key.
    /// Get your API key at: https://aimlapi.com?via=forevertools
    /// </summary>
    public static IServiceCollection AddForeverToolsSummarize(
        this IServiceCollection services,
        string apiKey)
    {
        return services.AddForeverToolsSummarize(options =>
        {
            options.ApiKey = apiKey;
        });
    }

    /// <summary>
    /// Adds the SummarizeClient to the service collection with configuration options.
    /// </summary>
    public static IServiceCollection AddForeverToolsSummarize(
        this IServiceCollection services,
        Action<SummarizeOptions> configureOptions)
    {
        var options = new SummarizeOptions();
        configureOptions(options);

        services.AddSingleton(options);
        services.AddSingleton<SummarizeClient>();

        return services;
    }

    /// <summary>
    /// Adds the SummarizeClient to the service collection from IConfiguration.
    /// Expects configuration section "Summarize" or "AimlApi" with properties:
    /// ApiKey, DefaultModel, DefaultStyle, DefaultLength, etc.
    /// </summary>
    public static IServiceCollection AddForeverToolsSummarize(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = SummarizeOptions.SectionName)
    {
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            section = configuration.GetSection("AimlApi");
        }

        var options = new SummarizeOptions();
        section.Bind(options);

        // Allow environment variable overrides
        var envApiKey = Environment.GetEnvironmentVariable("AIML_API_KEY")
            ?? Environment.GetEnvironmentVariable("SUMMARIZE_API_KEY");

        if (!string.IsNullOrEmpty(envApiKey))
            options.ApiKey = envApiKey;

        services.AddSingleton(options);
        services.AddSingleton<SummarizeClient>();

        return services;
    }

    /// <summary>
    /// Adds the SummarizeClient with a specific default style.
    /// </summary>
    public static IServiceCollection AddForeverToolsSummarize(
        this IServiceCollection services,
        string apiKey,
        SummaryStyle defaultStyle)
    {
        return services.AddForeverToolsSummarize(options =>
        {
            options.ApiKey = apiKey;
            options.DefaultStyle = defaultStyle;
        });
    }

    /// <summary>
    /// Adds the SummarizeClient with style and length defaults.
    /// </summary>
    public static IServiceCollection AddForeverToolsSummarize(
        this IServiceCollection services,
        string apiKey,
        SummaryStyle defaultStyle,
        SummaryLength defaultLength)
    {
        return services.AddForeverToolsSummarize(options =>
        {
            options.ApiKey = apiKey;
            options.DefaultStyle = defaultStyle;
            options.DefaultLength = defaultLength;
        });
    }

    /// <summary>
    /// Adds the SummarizeClient configured for a specific model.
    /// </summary>
    public static IServiceCollection AddForeverToolsSummarize(
        this IServiceCollection services,
        string apiKey,
        string model)
    {
        return services.AddForeverToolsSummarize(options =>
        {
            options.ApiKey = apiKey;
            options.DefaultModel = model;
        });
    }
}
