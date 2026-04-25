using Microsoft.Extensions.DependencyInjection;

namespace ForeverTools.TTS.Extensions;

/// <summary>
/// Extension methods for registering <see cref="TtsClient"/> with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="TtsClient"/> with a plain API key.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTtsClient(this IServiceCollection services, string apiKey)
    {
        return services.AddTtsClient(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Registers <see cref="TtsClient"/> with a configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="TtsOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTtsClient(
        this IServiceCollection services,
        Action<TtsOptions> configure)
    {
        services.AddHttpClient<TtsClient>((sp, http) =>
        {
            var opts = new TtsOptions();
            configure(opts);
            http.Timeout = opts.Timeout;
        });

        services.AddSingleton(sp =>
        {
            var opts = new TtsOptions();
            configure(opts);
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var http = factory.CreateClient(nameof(TtsClient));
            return new TtsClient(opts, http);
        });

        return services;
    }
}
