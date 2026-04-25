using Microsoft.Extensions.DependencyInjection;

namespace ForeverTools.InvoiceParser.Extensions;

/// <summary>
/// Extension methods for registering <see cref="InvoiceParserClient"/> with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="InvoiceParserClient"/> with a plain API key.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInvoiceParser(this IServiceCollection services, string apiKey)
    {
        return services.AddInvoiceParser(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Registers <see cref="InvoiceParserClient"/> with a configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="InvoiceParserOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInvoiceParser(
        this IServiceCollection services,
        Action<InvoiceParserOptions> configure)
    {
        services.AddHttpClient<InvoiceParserClient>((sp, http) =>
        {
            var opts = new InvoiceParserOptions();
            configure(opts);
            http.Timeout = opts.Timeout;
        });

        services.AddSingleton(sp =>
        {
            var opts = new InvoiceParserOptions();
            configure(opts);
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var http = factory.CreateClient(nameof(InvoiceParserClient));
            return new InvoiceParserClient(opts, http);
        });

        return services;
    }
}
