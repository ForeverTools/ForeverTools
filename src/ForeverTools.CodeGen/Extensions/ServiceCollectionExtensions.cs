using Microsoft.Extensions.DependencyInjection;

namespace ForeverTools.CodeGen.Extensions;

/// <summary>
/// Extension methods for registering <see cref="CodeGenClient"/> with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="CodeGenClient"/> with a plain API key.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCodeGenClient(this IServiceCollection services, string apiKey)
    {
        return services.AddCodeGenClient(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Registers <see cref="CodeGenClient"/> with a configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="CodeGenOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCodeGenClient(
        this IServiceCollection services,
        Action<CodeGenOptions> configure)
    {
        services.AddHttpClient<CodeGenClient>((sp, http) =>
        {
            var opts = new CodeGenOptions();
            configure(opts);
            http.Timeout = opts.Timeout;
        });

        services.AddSingleton(sp =>
        {
            var opts = new CodeGenOptions();
            configure(opts);
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var http = factory.CreateClient(nameof(CodeGenClient));
            return new CodeGenClient(opts, http);
        });

        return services;
    }
}
