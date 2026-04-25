namespace ForeverTools.InvoiceParser;

/// <summary>
/// Configuration options for <see cref="InvoiceParserClient"/>.
/// Get your API key at https://aimlapi.com?via=forevertools
/// </summary>
public class InvoiceParserOptions
{
    /// <summary>Default AI/ML API base URL.</summary>
    public const string DefaultBaseUrl = "https://api.aimlapi.com";

    /// <summary>
    /// Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The API base URL. Defaults to <c>https://api.aimlapi.com</c>.
    /// </summary>
    public string BaseUrl { get; set; } = DefaultBaseUrl;

    /// <summary>
    /// Request timeout. Defaults to 120 seconds (invoice parsing can take time for large documents).
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(120);

    /// <summary>
    /// Creates an <see cref="InvoiceParserOptions"/> by reading the API key from an environment variable.
    /// </summary>
    /// <param name="envVar">The environment variable name. Defaults to <c>AIML_API_KEY</c>.</param>
    /// <returns>Populated options with default base URL and timeout.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the environment variable is not set.</exception>
    public static InvoiceParserOptions FromEnvironment(string envVar = "AIML_API_KEY")
    {
        var apiKey = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                $"Environment variable '{envVar}' is not set. " +
                "Get your API key at https://aimlapi.com?via=forevertools");
        }
        return new InvoiceParserOptions { ApiKey = apiKey };
    }
}
