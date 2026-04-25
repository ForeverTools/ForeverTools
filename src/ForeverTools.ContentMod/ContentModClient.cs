using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ForeverTools.ContentMod.Models;

namespace ForeverTools.ContentMod;

/// <summary>
/// Client for content moderation via the AI/ML API.
/// Detects toxic, NSFW, spam, and hate content in text.
/// Get your API key at https://aimlapi.com?via=forevertools
/// </summary>
public class ContentModClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly ContentModOptions _options;
    private readonly bool _ownsHttpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // -------------------------------------------------------------------------
    // Constructors
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a new <see cref="ContentModClient"/> with the given options.
    /// </summary>
    /// <param name="options">Client configuration options.</param>
    /// <param name="httpClient">Optional pre-configured <see cref="HttpClient"/>. When omitted, one is created internally.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the API key is missing or the BaseUrl is invalid.</exception>
    public ContentModClient(ContentModOptions options, HttpClient? httpClient = null)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new ArgumentException(
                "API key is required. Get your key at https://aimlapi.com?via=forevertools",
                nameof(options));
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
    /// Creates a new <see cref="ContentModClient"/> with a plain API key.
    /// </summary>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    public ContentModClient(string apiKey) : this(new ContentModOptions { ApiKey = apiKey }) { }

    // -------------------------------------------------------------------------
    // Static factory
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a <see cref="ContentModClient"/> by reading the API key from an environment variable.
    /// </summary>
    /// <param name="envVar">The environment variable name. Defaults to <c>AIML_API_KEY</c>.</param>
    /// <returns>A ready-to-use <see cref="ContentModClient"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the environment variable is not set.</exception>
    public static ContentModClient FromEnvironment(string envVar = "AIML_API_KEY")
    {
        return new ContentModClient(ContentModOptions.FromEnvironment(envVar));
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Moderates a single piece of text and returns a <see cref="ModerationResult"/>.
    /// </summary>
    /// <param name="text">The text to moderate.</param>
    /// <param name="categories">Optional override for which categories to check. When null, uses the options default.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="ModerationResult"/> containing flags, scores, and metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="text"/> is empty or whitespace.</exception>
    public async Task<ModerationResult> ModerateAsync(
        string text,
        IEnumerable<string>? categories = null,
        CancellationToken ct = default)
    {
        ValidateText(text);
        var sw = Stopwatch.StartNew();
        var cats = ResolveCategories(categories);
        var result = await CallApiAsync(text, cats, ct).ConfigureAwait(false);
        sw.Stop();
        result.InputText = text;
        result.ProcessingMs = sw.ElapsedMilliseconds;
        return result;
    }

    /// <summary>
    /// Moderates multiple texts in sequence and returns one <see cref="ModerationResult"/> per input.
    /// </summary>
    /// <param name="texts">The texts to moderate. Must not be null.</param>
    /// <param name="categories">Optional override for which categories to check.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of <see cref="ModerationResult"/> in the same order as the input.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="texts"/> is null.</exception>
    public async Task<IReadOnlyList<ModerationResult>> ModerateAsync(
        IEnumerable<string> texts,
        IEnumerable<string>? categories = null,
        CancellationToken ct = default)
    {
        if (texts == null) throw new ArgumentNullException(nameof(texts));
        var cats = ResolveCategories(categories);
        var results = new List<ModerationResult>();
        foreach (var text in texts)
        {
            ct.ThrowIfCancellationRequested();
            results.Add(await ModerateAsync(text, cats, ct).ConfigureAwait(false));
        }
        return results.AsReadOnly();
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private IReadOnlyList<string>? ResolveCategories(IEnumerable<string>? overrideCategories)
    {
        if (overrideCategories != null)
        {
            var list = overrideCategories.ToList();
            return list.Count > 0 ? list.AsReadOnly() : null;
        }
        return _options.Categories;
    }

    private async Task<ModerationResult> CallApiAsync(
        string text,
        IReadOnlyList<string>? categories,
        CancellationToken ct)
    {
        object payload;
        if (categories != null && categories.Count > 0)
        {
            payload = new { text, categories };
        }
        else
        {
            payload = new { text };
        }

        var json = JsonSerializer.Serialize(payload);
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_options.BaseUrl.TrimEnd('/')}/v1/moderation")
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

        return ParseResult(body);
    }

    private static ModerationResult ParseResult(string json)
    {
        JsonNode? node;
        try
        {
            node = JsonNode.Parse(json);
        }
        catch (JsonException ex)
        {
            throw new FormatException($"The API returned invalid JSON: {ex.Message}\nRaw: {json}", ex);
        }

        if (node == null)
            throw new FormatException("The API returned a null JSON document.");

        var flagged     = node["flagged"]?.GetValue<bool>() ?? false;
        var safeForWork = node["safe_for_work"]?.GetValue<bool>() ?? !flagged;

        var categories = new CategoryFlags();
        var catNode = node["categories"];
        if (catNode != null)
        {
            categories.Toxic = catNode["toxic"]?.GetValue<bool>() ?? false;
            categories.Nsfw  = catNode["nsfw"]?.GetValue<bool>()  ?? false;
            categories.Spam  = catNode["spam"]?.GetValue<bool>()  ?? false;
            categories.Hate  = catNode["hate"]?.GetValue<bool>()  ?? false;
        }

        var scores = new CategoryScores();
        var scoresNode = node["scores"];
        if (scoresNode != null)
        {
            scores.Toxic = scoresNode["toxic"]?.GetValue<double>() ?? 0.0;
            scores.Nsfw  = scoresNode["nsfw"]?.GetValue<double>()  ?? 0.0;
            scores.Spam  = scoresNode["spam"]?.GetValue<double>()  ?? 0.0;
            scores.Hate  = scoresNode["hate"]?.GetValue<double>()  ?? 0.0;
        }

        return new ModerationResult
        {
            Flagged     = flagged,
            SafeForWork = safeForWork,
            Categories  = categories,
            Scores      = scores
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
