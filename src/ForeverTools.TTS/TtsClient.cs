using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ForeverTools.TTS.Models;

namespace ForeverTools.TTS;

/// <summary>
/// Client for converting text to speech via the AI/ML API.
/// Get your API key at https://aimlapi.com?via=forevertools
/// </summary>
public class TtsClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly TtsOptions _defaultOptions;
    private readonly bool _ownsHttpClient;

    // -------------------------------------------------------------------------
    // Constructors
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a new <see cref="TtsClient"/> with the given options.
    /// </summary>
    /// <param name="options">Client configuration options.</param>
    /// <param name="httpClient">Optional pre-configured <see cref="HttpClient"/>. When omitted, one is created internally.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the API key is missing.</exception>
    public TtsClient(TtsOptions options, HttpClient? httpClient = null)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new ArgumentException(
                "API key is required. Get your key at https://aimlapi.com?via=forevertools",
                nameof(options));
        ValidateSpeed(options.Speed);

        _defaultOptions = options;

        if (httpClient != null)
        {
            _http = httpClient;
            _ownsHttpClient = false;
        }
        else
        {
            _http = new HttpClient { Timeout = options.Timeout };
            _ownsHttpClient = true;
        }
    }

    /// <summary>
    /// Creates a new <see cref="TtsClient"/> with a plain API key.
    /// </summary>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    public TtsClient(string apiKey) : this(new TtsOptions { ApiKey = apiKey }) { }

    // -------------------------------------------------------------------------
    // Static factory
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a <see cref="TtsClient"/> by reading the API key from an environment variable.
    /// </summary>
    /// <param name="envVar">The environment variable name. Defaults to <c>AIML_API_KEY</c>.</param>
    /// <returns>A ready-to-use <see cref="TtsClient"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the environment variable is not set.</exception>
    public static TtsClient FromEnvironment(string envVar = "AIML_API_KEY")
    {
        return new TtsClient(TtsOptions.FromEnvironment(envVar));
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Synthesises speech from text and returns the raw audio bytes.
    /// </summary>
    /// <param name="text">The text to convert to speech.</param>
    /// <param name="options">Per-request options. When null, the client's default options are used.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The audio data as a byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="text"/> is empty or whitespace.</exception>
    public async Task<byte[]> SynthesizeAsync(
        string text,
        TtsOptions? options = null,
        CancellationToken ct = default)
    {
        ValidateText(text);
        var opts = options ?? _defaultOptions;
        return await CallApiAsync(text, opts, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Synthesises speech from text and returns a <see cref="TtsResult"/> with audio bytes and metadata.
    /// </summary>
    /// <param name="text">The text to convert to speech.</param>
    /// <param name="options">Per-request options. When null, the client's default options are used.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="TtsResult"/> containing audio bytes and synthesis metadata.</returns>
    public async Task<TtsResult> SynthesizeWithMetadataAsync(
        string text,
        TtsOptions? options = null,
        CancellationToken ct = default)
    {
        ValidateText(text);
        var opts = options ?? _defaultOptions;
        var sw = Stopwatch.StartNew();
        var bytes = await CallApiAsync(text, opts, ct).ConfigureAwait(false);
        sw.Stop();

        return new TtsResult
        {
            AudioBytes     = bytes,
            Format         = opts.Format,
            DurationMs     = sw.ElapsedMilliseconds,
            Voice          = opts.Voice,
            Model          = opts.Model,
            CharacterCount = text.Length
        };
    }

    /// <summary>
    /// Synthesises speech and saves the audio to a file.
    /// Parent directories are created automatically if they do not exist.
    /// </summary>
    /// <param name="text">The text to convert to speech.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="options">Per-request options. When null, the client's default options are used.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task SaveToFileAsync(
        string text,
        string filePath,
        TtsOptions? options = null,
        CancellationToken ct = default)
    {
        if (filePath == null) throw new ArgumentNullException(nameof(filePath));

        var bytes = await SynthesizeAsync(text, options, ct).ConfigureAwait(false);

        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

#if NETSTANDARD2_0
        File.WriteAllBytes(filePath, bytes);
#else
        await File.WriteAllBytesAsync(filePath, bytes, ct).ConfigureAwait(false);
#endif
    }

    /// <summary>
    /// Synthesises speech and returns a readable <see cref="Stream"/> containing the audio data.
    /// The caller is responsible for disposing the stream.
    /// </summary>
    /// <param name="text">The text to convert to speech.</param>
    /// <param name="options">Per-request options. When null, the client's default options are used.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="MemoryStream"/> containing the audio data, positioned at the start.</returns>
    public async Task<Stream> GetAudioStreamAsync(
        string text,
        TtsOptions? options = null,
        CancellationToken ct = default)
    {
        var bytes = await SynthesizeAsync(text, options, ct).ConfigureAwait(false);
        return new MemoryStream(bytes);
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private async Task<byte[]> CallApiAsync(string text, TtsOptions opts, CancellationToken ct)
    {
        var payload = new
        {
            model           = opts.Model,
            input           = text,
            voice           = opts.Voice,
            response_format = opts.Format.ToApiString(),
            speed           = opts.Speed
        };

        var json = JsonSerializer.Serialize(payload);
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{opts.BaseUrl.TrimEnd('/')}/v1/audio/speech")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", opts.ApiKey);

        HttpResponseMessage response;
        try
        {
            response = await _http.SendAsync(request, ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException(
                "Invalid API key. Get your key at https://aimlapi.com?via=forevertools");

        if (response.StatusCode == (HttpStatusCode)429)
            throw new InvalidOperationException(
                "Rate limit exceeded. Please slow down your requests or upgrade your plan at https://aimlapi.com?via=forevertools");

        response.EnsureSuccessStatusCode();

#if NETSTANDARD2_0
        return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
#else
        return await response.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
#endif
    }

    private static void ValidateText(string text)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text must not be empty or whitespace.", nameof(text));
    }

    private static void ValidateSpeed(float speed)
    {
        if (speed < 0.25f || speed > 4.0f)
            throw new ArgumentOutOfRangeException(nameof(speed),
                $"Speed must be between 0.25 and 4.0 (was {speed}).");
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_ownsHttpClient) _http.Dispose();
    }
}
