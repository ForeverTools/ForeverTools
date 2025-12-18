using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ForeverTools.Captcha;

/// <summary>
/// Extension methods for registering captcha services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the CaptchaClient to the service collection with a single provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">API key for the default provider (2Captcha).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsCaptcha(this IServiceCollection services, string apiKey)
    {
        return services.AddForeverToolsCaptcha(options => options.TwoCaptchaApiKey = apiKey);
    }

    /// <summary>
    /// Adds the CaptchaClient to the service collection with configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure the captcha options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsCaptcha(
        this IServiceCollection services,
        Action<CaptchaOptions> configure)
    {
        services.Configure(configure);
        services.AddHttpClient();
        services.AddSingleton<CaptchaClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<CaptchaOptions>>().Value;
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            return new CaptchaClient(options, httpClientFactory.CreateClient());
        });
        return services;
    }

    /// <summary>
    /// Adds the CaptchaClient to the service collection, binding from configuration.
    /// Expects configuration section "Captcha" with provider API keys.
    /// </summary>
    /// <example>
    /// appsettings.json:
    /// {
    ///   "Captcha": {
    ///     "DefaultProvider": "TwoCaptcha",
    ///     "TwoCaptchaApiKey": "your-key",
    ///     "CapSolverApiKey": "your-key",
    ///     "AntiCaptchaApiKey": "your-key",
    ///     "TimeoutSeconds": 120
    ///   }
    /// }
    /// </example>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsCaptcha(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CaptchaOptions>(configuration.GetSection(CaptchaOptions.SectionName));
        services.AddHttpClient();
        services.AddSingleton<CaptchaClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<CaptchaOptions>>().Value;
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            return new CaptchaClient(options, httpClientFactory.CreateClient());
        });
        return services;
    }
}
