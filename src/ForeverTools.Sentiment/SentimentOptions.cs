namespace ForeverTools.Sentiment;

/// <summary>
/// Configuration options for <see cref="SentimentClient"/>.
/// Get your API key at https://aimlapi.com?via=forevertools
/// </summary>
public class SentimentOptions
{
    /// <summary>Default AI/ML API base URL.</summary>
    public const string DefaultBaseUrl = "https://api.aimlapi.com";

    /// <summary>Default model used for sentiment analysis.</summary>
    public const string DefaultModel = "gpt-4o-mini";

    /// <summary>
    /// Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The chat model to use for sentiment analysis. Defaults to <c>gpt-4o-mini</c>.
    /// </summary>
    public string Model { get; set; } = DefaultModel;

    /// <summary>
    /// The API base URL. Defaults to <c>https://api.aimlapi.com</c>.
    /// </summary>
    public string BaseUrl { get; set; } = DefaultBaseUrl;

    /// <summary>
    /// Request timeout. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Creates a <see cref="SentimentOptions"/> by reading the API key from an environment variable.
    /// </summary>
    /// <param name="envVar">Environment variable name. Defaults to <c>AIML_API_KEY</c>.</param>
    /// <returns>Populated options.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the environment variable is not set.</exception>
    public static SentimentOptions FromEnvironment(string envVar = "AIML_API_KEY")
    {
        var apiKey = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                $"Environment variable '{envVar}' is not set. " +
                "Get your API key at https://aimlapi.com?via=forevertools");
        }
        return new SentimentOptions { ApiKey = apiKey };
    }
}
