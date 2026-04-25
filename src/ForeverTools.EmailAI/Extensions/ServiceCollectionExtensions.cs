using Microsoft.Extensions.DependencyInjection;

namespace ForeverTools.EmailAI.Extensions;

/// <summary>
/// Extension methods for registering <see cref="EmailAIClient"/> with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="EmailAIClient"/> with a plain API key.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsEmailAI(this IServiceCollection services, string apiKey)
    {
        return services.AddForeverToolsEmailAI(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Registers <see cref="EmailAIClient"/> with a configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="EmailAIOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddForeverToolsEmailAI(
        this IServiceCollection services,
        Action<EmailAIOptions> configure)
    {
        services.AddHttpClient<EmailAIClient>((sp, http) =>
        {
            var opts = new EmailAIOptions();
            configure(opts);
            http.Timeout = opts.Timeout;
        });

        services.AddSingleton(sp =>
        {
            var opts = new EmailAIOptions();
            configure(opts);
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var http = factory.CreateClient(nameof(EmailAIClient));
            return new EmailAIClient(opts, http);
        });

        return services;
    }
}
