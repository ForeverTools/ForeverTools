namespace ForeverTools.ContentMod;

/// <summary>
/// Configuration options for <see cref="ContentModClient"/>.
/// Get your API key at https://aimlapi.com?via=forevertools
/// </summary>
public class ContentModOptions
{
    /// <summary>Default AI/ML API base URL.</summary>
    public const string DefaultBaseUrl = "https://api.aimlapi.com";

    /// <summary>All supported moderation categories.</summary>
    public static readonly IReadOnlyList<string> AllCategories =
        new[] { "toxic", "nsfw", "spam", "hate" };

    /// <summary>
    /// Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The API base URL. Defaults to <c>https://api.aimlapi.com</c>.
    /// </summary>
    public string BaseUrl { get; set; } = DefaultBaseUrl;

    /// <summary>
    /// Request timeout. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Optional list of categories to check. When null or empty, all categories are checked.
    /// Valid values: <c>toxic</c>, <c>nsfw</c>, <c>spam</c>, <c>hate</c>.
    /// </summary>
    public IReadOnlyList<string>? Categories { get; set; }

    /// <summary>
    /// Creates a <see cref="ContentModOptions"/> by reading the API key from an environment variable.
    /// </summary>
    /// <param name="envVar">Environment variable name. Defaults to <c>AIML_API_KEY</c>.</param>
    /// <returns>Populated options with default settings.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the environment variable is not set.</exception>
    public static ContentModOptions FromEnvironment(string envVar = "AIML_API_KEY")
    {
        var apiKey = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                $"Environment variable '{envVar}' is not set. " +
                "Get your API key at https://aimlapi.com?via=forevertools");
        }
        return new ContentModOptions { ApiKey = apiKey };
    }
}
