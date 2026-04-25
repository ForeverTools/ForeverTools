using ForeverTools.TTS.Models;

namespace ForeverTools.TTS;

/// <summary>
/// Configuration options for <see cref="TtsClient"/>.
/// Get your API key at https://aimlapi.com?via=forevertools
/// </summary>
public class TtsOptions
{
    /// <summary>Default AI/ML API base URL.</summary>
    public const string DefaultBaseUrl = "https://api.aimlapi.com";

    /// <summary>Default voice used for synthesis.</summary>
    public const string DefaultVoice = TtsVoices.Alloy;

    /// <summary>Default TTS model.</summary>
    public const string DefaultModel = TtsModels.Tts1;

    /// <summary>
    /// Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The voice to use for synthesis. Defaults to <c>alloy</c>.
    /// See <see cref="TtsVoices"/> for available values.
    /// </summary>
    public string Voice { get; set; } = DefaultVoice;

    /// <summary>
    /// The TTS model to use. Defaults to <c>tts-1</c>.
    /// Use <c>tts-1-hd</c> for higher quality at increased cost.
    /// </summary>
    public string Model { get; set; } = DefaultModel;

    /// <summary>
    /// The output audio format. Defaults to <see cref="TtsFormat.Mp3"/>.
    /// </summary>
    public TtsFormat Format { get; set; } = TtsFormat.Mp3;

    /// <summary>
    /// Speech speed multiplier. Must be between 0.25 and 4.0. Defaults to 1.0 (normal speed).
    /// </summary>
    public float Speed { get; set; } = 1.0f;

    /// <summary>
    /// The API base URL. Defaults to <c>https://api.aimlapi.com</c>.
    /// </summary>
    public string BaseUrl { get; set; } = DefaultBaseUrl;

    /// <summary>
    /// Request timeout. Defaults to 60 seconds (audio generation can take longer than text).
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Creates a <see cref="TtsOptions"/> by reading the API key from an environment variable.
    /// </summary>
    /// <param name="envVar">The environment variable name. Defaults to <c>AIML_API_KEY</c>.</param>
    /// <returns>Populated options with default voice, model, and format.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the environment variable is not set.</exception>
    public static TtsOptions FromEnvironment(string envVar = "AIML_API_KEY")
    {
        var apiKey = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                $"Environment variable '{envVar}' is not set. " +
                "Get your API key at https://aimlapi.com?via=forevertools");
        }
        return new TtsOptions { ApiKey = apiKey };
    }
}
