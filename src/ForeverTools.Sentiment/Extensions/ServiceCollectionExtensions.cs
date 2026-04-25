using Microsoft.Extensions.DependencyInjection;

namespace ForeverTools.Sentiment.Extensions;

/// <summary>
/// Extension methods for registering <see cref="SentimentClient"/> with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="SentimentClient"/> with a plain API key.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSentimentClient(this IServiceCollection services, string apiKey)
    {
        return services.AddSentimentClient(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Registers <see cref="SentimentClient"/> with a configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="SentimentOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSentimentClient(
        this IServiceCollection services,
        Action<SentimentOptions> configure)
    {
        services.AddHttpClient<SentimentClient>((sp, http) =>
        {
            var opts = new SentimentOptions();
            configure(opts);
            http.Timeout = opts.Timeout;
        });

        services.AddSingleton(sp =>
        {
            var opts = new SentimentOptions();
            configure(opts);
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var http = factory.CreateClient(nameof(SentimentClient));
            return new SentimentClient(opts, http);
        });

        return services;
    }
}
