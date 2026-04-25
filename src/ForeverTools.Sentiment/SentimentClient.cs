using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ForeverTools.Sentiment.Models;

namespace ForeverTools.Sentiment;

/// <summary>
/// Client for performing sentiment analysis and emotion scoring via the AI/ML API.
/// Get your API key at https://aimlapi.com?via=forevertools
/// </summary>
public class SentimentClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly SentimentOptions _options;
    private readonly bool _ownsHttpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string SystemPrompt =
        "You are a sentiment analysis engine. Analyse the user's text and respond ONLY with a valid JSON object in exactly this format, with no markdown fences or extra text:\n" +
        "{\"label\":\"positive\",\"confidence\":0.92,\"emotions\":{\"joy\":0.78,\"anger\":0.02,\"sadness\":0.05,\"fear\":0.03,\"surprise\":0.12,\"disgust\":0.00},\"summary\":\"Enthusiastic and optimistic tone\"}\n" +
        "label must be one of: positive, negative, neutral, mixed. All confidence and emotion values must be between 0 and 1.";

    /// <summary>
    /// Creates a new <see cref="SentimentClient"/> with the provided options.
    /// </summary>
    /// <param name="options">Client configuration options.</param>
    /// <param name="httpClient">Optional pre-configured <see cref="HttpClient"/>. When omitted, one is created internally.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the API key is missing.</exception>
    public SentimentClient(SentimentOptions options, HttpClient? httpClient = null)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new ArgumentException(
                "API key is required. Get your key at https://aimlapi.com?via=forevertools",
                nameof(options));
        if (string.IsNullOrWhiteSpace(options.Model))
            throw new ArgumentException("Model must not be empty.", nameof(options));
        if (string.IsNullOrWhiteSpace(options.BaseUrl) || !Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _))
            throw new ArgumentException("BaseUrl must be a valid absolute URL.", nameof(options));

        _options = options;

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
    /// Creates a <see cref="SentimentClient"/> using a plain API key string.
    /// </summary>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    public SentimentClient(string apiKey) : this(new SentimentOptions { ApiKey = apiKey }) { }

    /// <summary>
    /// Creates a <see cref="SentimentClient"/> by reading the API key from an environment variable.
    /// </summary>
    /// <param name="envVar">The environment variable name. Defaults to <c>AIML_API_KEY</c>.</param>
    /// <returns>A ready-to-use <see cref="SentimentClient"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the environment variable is not set.</exception>
    public static SentimentClient FromEnvironment(string envVar = "AIML_API_KEY")
    {
        return new SentimentClient(SentimentOptions.FromEnvironment(envVar));
    }

    /// <summary>
    /// Analyses the sentiment of a single piece of text.
    /// </summary>
    /// <param name="text">The text to analyse.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="SentimentResult"/> containing the label, confidence, and emotion scores.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="text"/> is empty or whitespace.</exception>
    public async Task<SentimentResult> AnalyzeAsync(string text, CancellationToken ct = default)
    {
        ValidateText(text);
        var sw = Stopwatch.StartNew();
        var raw = await CallApiAsync(SystemPrompt, text, ct).ConfigureAwait(false);
        sw.Stop();
        var result = ParseResult(raw);
        result.InputText = text;
        result.ProcessingMs = sw.ElapsedMilliseconds;
        return result;
    }

    /// <summary>
    /// Analyses the sentiment of multiple texts in sequence.
    /// </summary>
    /// <param name="texts">The texts to analyse.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of <see cref="SentimentResult"/> in the same order as the input.</returns>
    public async Task<IReadOnlyList<SentimentResult>> AnalyzeBatchAsync(
        IEnumerable<string> texts,
        CancellationToken ct = default)
    {
        if (texts == null) throw new ArgumentNullException(nameof(texts));
        var list = new List<SentimentResult>();
        foreach (var text in texts)
        {
            ct.ThrowIfCancellationRequested();
            list.Add(await AnalyzeAsync(text, ct).ConfigureAwait(false));
        }
        return list.AsReadOnly();
    }

    /// <summary>
    /// Analyses text sentiment with additional context provided to the model.
    /// </summary>
    /// <param name="text">The text to analyse.</param>
    /// <param name="context">Additional context that helps the model interpret the text (e.g., "This is a product review").</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="SentimentResult"/>.</returns>
    public async Task<SentimentResult> AnalyzeWithContextAsync(
        string text,
        string context,
        CancellationToken ct = default)
    {
        ValidateText(text);
        var sw = Stopwatch.StartNew();
        var userMessage = string.IsNullOrWhiteSpace(context)
            ? text
            : $"Context: {context}\n\nText: {text}";
        var raw = await CallApiAsync(SystemPrompt, userMessage, ct).ConfigureAwait(false);
        sw.Stop();
        var result = ParseResult(raw);
        result.InputText = text;
        result.ProcessingMs = sw.ElapsedMilliseconds;
        return result;
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private async Task<string> CallApiAsync(string systemPrompt, string userMessage, CancellationToken ct)
    {
        var payload = new
        {
            model = _options.Model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user",   content = userMessage }
            },
            temperature = 0,
            max_tokens = 300
        };

        var json = JsonSerializer.Serialize(payload);
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl.TrimEnd('/')}/v1/chat/completions")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

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
        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#else
        var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
#endif

        // Extract the content field from the chat completion response.
        JsonNode? root;
        try
        {
            root = JsonNode.Parse(body);
        }
        catch (JsonException ex)
        {
            throw new FormatException($"Invalid JSON response from API: {ex.Message}", ex);
        }

        var content = root?["choices"]?[0]?["message"]?["content"]?.GetValue<string>();
        if (string.IsNullOrWhiteSpace(content))
            throw new FormatException("Unexpected API response shape — missing choices[0].message.content");

        return content;
    }

    private static SentimentResult ParseResult(string json)
    {
        JsonNode? node;
        try
        {
            node = JsonNode.Parse(json);
        }
        catch (JsonException ex)
        {
            throw new FormatException($"The model returned invalid JSON: {ex.Message}\nRaw: {json}", ex);
        }

        if (node == null)
            throw new FormatException("The model returned a null JSON document.");

        var labelStr = node["label"]?.GetValue<string>() ?? "neutral";
        var label = labelStr.ToLowerInvariant() switch
        {
            "positive" => SentimentLabel.Positive,
            "negative" => SentimentLabel.Negative,
            "mixed"    => SentimentLabel.Mixed,
            _          => SentimentLabel.Neutral
        };

        var confidence = node["confidence"]?.GetValue<double>() ?? 0.5;
        var summary    = node["summary"]?.GetValue<string>() ?? string.Empty;

        var emotions = new Models.EmotionScores();
        var emotionsNode = node["emotions"];
        if (emotionsNode != null)
        {
            emotions.Joy      = emotionsNode["joy"]?.GetValue<double>()      ?? 0;
            emotions.Anger    = emotionsNode["anger"]?.GetValue<double>()    ?? 0;
            emotions.Sadness  = emotionsNode["sadness"]?.GetValue<double>()  ?? 0;
            emotions.Fear     = emotionsNode["fear"]?.GetValue<double>()     ?? 0;
            emotions.Surprise = emotionsNode["surprise"]?.GetValue<double>() ?? 0;
            emotions.Disgust  = emotionsNode["disgust"]?.GetValue<double>()  ?? 0;
        }

        return new Models.SentimentResult
        {
            Label      = label,
            Confidence = confidence,
            Emotions   = emotions,
            Summary    = summary
        };
    }

    private static void ValidateText(string text)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text must not be empty or whitespace.", nameof(text));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_ownsHttpClient) _http.Dispose();
    }
}
