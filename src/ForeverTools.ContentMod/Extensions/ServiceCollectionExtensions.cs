using Microsoft.Extensions.DependencyInjection;

namespace ForeverTools.ContentMod.Extensions;

/// <summary>
/// Extension methods for registering <see cref="ContentModClient"/> with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="ContentModClient"/> with a plain API key.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddContentMod(this IServiceCollection services, string apiKey)
    {
        return services.AddContentMod(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Registers <see cref="ContentModClient"/> with a configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="ContentModOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddContentMod(
        this IServiceCollection services,
        Action<ContentModOptions> configure)
    {
        services.AddHttpClient<ContentModClient>((sp, http) =>
        {
            var opts = new ContentModOptions();
            configure(opts);
            http.Timeout = opts.Timeout;
        });

        services.AddSingleton(sp =>
        {
            var opts = new ContentModOptions();
            configure(opts);
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var http = factory.CreateClient(nameof(ContentModClient));
            return new ContentModClient(opts, http);
        });

        return services;
    }
}
