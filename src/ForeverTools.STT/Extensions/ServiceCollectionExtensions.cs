using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForeverTools.STT;

/// <summary>
/// Extension methods for registering SpeechToTextClient with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds SpeechToTextClient to the service collection.
    /// Get your API key at: https://aimlapi.com?via=forevertools
    /// </summary>
    public static IServiceCollection AddForeverToolsSTT(
        this IServiceCollection services,
        string apiKey)
    {
        return services.AddForeverToolsSTT(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Adds SpeechToTextClient to the service collection with configuration.
    /// </summary>
    public static IServiceCollection AddForeverToolsSTT(
        this IServiceCollection services,
        Action<SpeechToTextOptions> configure)
    {
        var options = new SpeechToTextOptions();
        configure(options);

        services.AddSingleton(options);
        services.AddSingleton<SpeechToTextClient>();

        return services;
    }

    /// <summary>
    /// Adds SpeechToTextClient from IConfiguration.
    /// Reads from "SpeechToText" or "STT" section.
    /// </summary>
    public static IServiceCollection AddForeverToolsSTT(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("SpeechToText")
            ?? configuration.GetSection("STT");

        var options = new SpeechToTextOptions();

        if (section.Exists())
        {
            section.Bind(options);
        }

        // Allow override from environment variables
        var envKey = Environment.GetEnvironmentVariable("AIML_API_KEY")
            ?? Environment.GetEnvironmentVariable("STT_API_KEY");

        if (!string.IsNullOrEmpty(envKey))
        {
            options.ApiKey = envKey;
        }

        services.AddSingleton(options);
        services.AddSingleton<SpeechToTextClient>();

        return services;
    }

    /// <summary>
    /// Adds SpeechToTextClient with a pre-configured options instance.
    /// </summary>
    public static IServiceCollection AddForeverToolsSTT(
        this IServiceCollection services,
        SpeechToTextOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton<SpeechToTextClient>();

        return services;
    }
}
