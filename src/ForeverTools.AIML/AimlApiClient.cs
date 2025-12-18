using System.ClientModel;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Images;
using OpenAI.Audio;

namespace ForeverTools.AIML;

/// <summary>
/// Factory for creating AI/ML API clients pre-configured to use the AI/ML API endpoint.
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public class AimlApiClient
{
    private readonly OpenAIClient _client;
    private readonly AimlApiOptions _options;

    /// <summary>
    /// Creates a new AI/ML API client with the specified API key.
    /// </summary>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    public AimlApiClient(string apiKey) : this(new AimlApiOptions { ApiKey = apiKey })
    {
    }

    /// <summary>
    /// Creates a new AI/ML API client with the specified options.
    /// </summary>
    /// <param name="options">Configuration options for the client.</param>
    public AimlApiClient(AimlApiOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new ArgumentException(
                "API key is required. Get your key at https://aimlapi.com?via=forevertools",
                nameof(options));
        }

        _options = options;

        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(options.BaseUrl)
        };

        _client = new OpenAIClient(new ApiKeyCredential(options.ApiKey), clientOptions);
    }

    /// <summary>
    /// Gets the underlying OpenAI client for advanced scenarios.
    /// </summary>
    public OpenAIClient UnderlyingClient => _client;

    /// <summary>
    /// Gets the configured options.
    /// </summary>
    public AimlApiOptions Options => _options;

    #region Chat Completions

    /// <summary>
    /// Gets a ChatClient for the specified model or the default model.
    /// </summary>
    /// <param name="model">The model to use, or null to use the default chat model.</param>
    public ChatClient GetChatClient(string? model = null)
    {
        return _client.GetChatClient(model ?? _options.DefaultChatModel);
    }

    /// <summary>
    /// Sends a simple chat message and returns the response text.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="model">The model to use, or null for the default.</param>
    /// <returns>The assistant's response text.</returns>
    public async Task<string> ChatAsync(string message, string? model = null)
    {
        var client = GetChatClient(model);
        var response = await client.CompleteChatAsync(message);
        return response.Value.Content[0].Text;
    }

    /// <summary>
    /// Sends a chat message with full options and returns the complete response.
    /// </summary>
    public async Task<ChatCompletion> CompleteChatAsync(
        IEnumerable<ChatMessage> messages,
        ChatCompletionOptions? options = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        var client = GetChatClient(model);
        var response = await client.CompleteChatAsync(messages, options, cancellationToken);
        return response.Value;
    }

    /// <summary>
    /// Streams chat completion tokens as they're generated.
    /// </summary>
    public AsyncCollectionResult<StreamingChatCompletionUpdate> StreamChatAsync(
        IEnumerable<ChatMessage> messages,
        ChatCompletionOptions? options = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        var client = GetChatClient(model);
        return client.CompleteChatStreamingAsync(messages, options, cancellationToken);
    }

    #endregion

    #region Embeddings

    /// <summary>
    /// Gets an EmbeddingClient for the specified model or the default model.
    /// </summary>
    /// <param name="model">The model to use, or null to use the default embedding model.</param>
    public EmbeddingClient GetEmbeddingClient(string? model = null)
    {
        return _client.GetEmbeddingClient(model ?? _options.DefaultEmbeddingModel);
    }

    /// <summary>
    /// Generates an embedding vector for the given text.
    /// </summary>
    /// <param name="text">The text to embed.</param>
    /// <param name="model">The model to use, or null for the default.</param>
    /// <returns>The embedding vector.</returns>
    public async Task<ReadOnlyMemory<float>> EmbedAsync(string text, string? model = null)
    {
        var client = GetEmbeddingClient(model);
        var response = await client.GenerateEmbeddingAsync(text);
        return response.Value.ToFloats();
    }

    /// <summary>
    /// Generates embeddings for multiple texts.
    /// </summary>
    public async Task<IReadOnlyList<OpenAIEmbedding>> EmbedManyAsync(
        IEnumerable<string> texts,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        var client = GetEmbeddingClient(model);
        var response = await client.GenerateEmbeddingsAsync(texts, cancellationToken: cancellationToken);
        return response.Value;
    }

    #endregion

    #region Image Generation

    /// <summary>
    /// Gets an ImageClient for the specified model or the default model.
    /// </summary>
    /// <param name="model">The model to use, or null to use the default image model.</param>
    public ImageClient GetImageClient(string? model = null)
    {
        return _client.GetImageClient(model ?? _options.DefaultImageModel);
    }

    /// <summary>
    /// Generates an image from a text prompt.
    /// </summary>
    /// <param name="prompt">The image description.</param>
    /// <param name="model">The model to use, or null for the default.</param>
    /// <returns>The generated image URI.</returns>
    public async Task<Uri> GenerateImageAsync(string prompt, string? model = null)
    {
        var client = GetImageClient(model);
        var response = await client.GenerateImageAsync(prompt);
        return response.Value.ImageUri;
    }

    /// <summary>
    /// Generates an image with full options.
    /// </summary>
    public async Task<GeneratedImage> GenerateImageAsync(
        string prompt,
        ImageGenerationOptions? options = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        var client = GetImageClient(model);
        var response = await client.GenerateImageAsync(prompt, options, cancellationToken);
        return response.Value;
    }

    /// <summary>
    /// Generates multiple images from a prompt.
    /// </summary>
    public async Task<IReadOnlyList<GeneratedImage>> GenerateImagesAsync(
        string prompt,
        int count,
        ImageGenerationOptions? options = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        var client = GetImageClient(model);
        options ??= new ImageGenerationOptions();
        var response = await client.GenerateImagesAsync(prompt, count, options, cancellationToken);
        return response.Value;
    }

    #endregion

    #region Audio

    /// <summary>
    /// Gets an AudioClient for the specified model.
    /// </summary>
    public AudioClient GetAudioClient(string model)
    {
        return _client.GetAudioClient(model);
    }

    /// <summary>
    /// Generates speech from text.
    /// </summary>
    /// <param name="text">The text to convert to speech.</param>
    /// <param name="voice">The voice to use (e.g., "alloy", "echo", "fable", "onyx", "nova", "shimmer").</param>
    /// <param name="model">The TTS model to use.</param>
    /// <returns>The audio data as a BinaryData object.</returns>
    public async Task<BinaryData> TextToSpeechAsync(
        string text,
        string voice = "alloy",
        string model = AimlModels.Audio.Tts1)
    {
        var client = GetAudioClient(model);
        var response = await client.GenerateSpeechAsync(text, GeneratedSpeechVoice.Alloy);
        return response.Value;
    }

    /// <summary>
    /// Transcribes audio to text.
    /// </summary>
    /// <param name="audioStream">The audio stream to transcribe.</param>
    /// <param name="fileName">The filename (for format detection).</param>
    /// <param name="model">The transcription model to use.</param>
    /// <returns>The transcribed text.</returns>
    public async Task<string> TranscribeAsync(
        Stream audioStream,
        string fileName = "audio.mp3",
        string model = AimlModels.Audio.Whisper1)
    {
        var client = GetAudioClient(model);
        var response = await client.TranscribeAudioAsync(audioStream, fileName);
        return response.Value.Text;
    }

    #endregion

    #region Static Factory Methods

    /// <summary>
    /// Creates a new AI/ML API client from an environment variable.
    /// </summary>
    /// <param name="envVarName">The environment variable containing the API key. Defaults to "AIML_API_KEY".</param>
    public static AimlApiClient FromEnvironment(string envVarName = "AIML_API_KEY")
    {
        var apiKey = Environment.GetEnvironmentVariable(envVarName);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                $"Environment variable '{envVarName}' not set. " +
                "Get your API key at https://aimlapi.com?via=forevertools");
        }
        return new AimlApiClient(apiKey);
    }

    /// <summary>
    /// Creates a pre-configured ChatClient for quick usage.
    /// </summary>
    public static ChatClient CreateChatClient(string apiKey, string model = AimlModels.Chat.Gpt4o)
    {
        return new AimlApiClient(apiKey).GetChatClient(model);
    }

    /// <summary>
    /// Creates a pre-configured EmbeddingClient for quick usage.
    /// </summary>
    public static EmbeddingClient CreateEmbeddingClient(string apiKey, string model = AimlModels.Embedding.TextEmbedding3Small)
    {
        return new AimlApiClient(apiKey).GetEmbeddingClient(model);
    }

    /// <summary>
    /// Creates a pre-configured ImageClient for quick usage.
    /// </summary>
    public static ImageClient CreateImageClient(string apiKey, string model = AimlModels.Image.DallE3)
    {
        return new AimlApiClient(apiKey).GetImageClient(model);
    }

    #endregion
}
