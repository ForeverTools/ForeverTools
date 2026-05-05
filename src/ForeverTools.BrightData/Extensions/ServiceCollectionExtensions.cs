using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForeverTools.BrightData.Extensions;

/// <summary>DI extension methods for registering BrightData services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="BrightDataClient"/> with the DI container.
    /// Binds options from the "BrightData" configuration section.
    /// Get your token at: https://get.brightdata.com/ForeverToolsWebScraper
    /// </summary>
    public static IServiceCollection AddBrightData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<BrightDataOptions>(configuration.GetSection("BrightData"));
        services.AddHttpClient<BrightDataClient>();
        return services;
    }

    /// <summary>
    /// Registers <see cref="BrightDataClient"/> with the DI container using an inline options action.
    /// Get your token at: https://get.brightdata.com/ForeverToolsWebScraper
    /// </summary>
    public static IServiceCollection AddBrightData(
        this IServiceCollection services,
        Action<BrightDataOptions> configure)
    {
        services.Configure(configure);
        services.AddHttpClient<BrightDataClient>();
        return services;
    }
}
