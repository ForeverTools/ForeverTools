using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ForeverTools.EmailAI.Models;

namespace ForeverTools.EmailAI;

/// <summary>
/// Client for AI-powered email composition, replies, summarisation, and classification via the AI/ML API.
/// Get your API key at https://aimlapi.com?via=forevertools
/// </summary>
public class EmailAIClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly EmailAIOptions _options;
    private readonly bool _ownsHttpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string ComposeSystemPrompt =
        "You are an expert email writing assistant. Given a subject, recipients, context, and desired tone, " +
        "compose a complete, polished email body ready to send. " +
        "Respond ONLY with a valid JSON object in exactly this format, with no markdown fences or extra text:\n" +
        "{\"body\":\"...\",\"subject\":\"...\",\"tone\":\"professional\",\"notes\":null}\n" +
        "The body must be the full email text including salutation and sign-off. " +
        "Escape all special characters properly. notes may be null or a short string.";

    private const string ReplySystemPrompt =
        "You are an expert email reply assistant. Given an original email and a description of what the reply should say, " +
        "compose a complete, polished reply email body. " +
        "Respond ONLY with a valid JSON object in exactly this format, with no markdown fences or extra text:\n" +
        "{\"body\":\"...\",\"tone\":\"professional\",\"notes\":null}\n" +
        "The body must be the full reply including salutation and sign-off. " +
        "Escape all special characters properly. notes may be null or a short string.";

    private const string SummarizeSystemPrompt =
        "You are an expert email summarisation assistant. Given an email body, produce a concise summary, " +
        "identify any action items, and determine the main topic. " +
        "Respond ONLY with a valid JSON object in exactly this format, with no markdown fences or extra text:\n" +
        "{\"summary\":\"...\",\"action_items\":[\"item1\",\"item2\"],\"topic\":\"...\"}\n" +
        "action_items may be an empty array if there are no action items. " +
        "Escape all special characters properly.";

    private const string ClassifySystemPrompt =
        "You are an expert email classification assistant. Given an email body, classify it and assess its priority and sentiment. " +
        "Respond ONLY with a valid JSON object in exactly this format, with no markdown fences or extra text:\n" +
        "{\"category\":\"normal\",\"priority\":\"medium\",\"sentiment\":\"neutral\",\"rationale\":\"...\"}\n" +
        "category must be one of: urgent, normal, spam, newsletter, support, sales, other.\n" +
        "priority must be one of: high, medium, low.\n" +
        "sentiment must be one of: positive, neutral, negative.\n" +
        "Escape all special characters properly.";

    /// <summary>
    /// Creates a new <see cref="EmailAIClient"/> with the provided options.
    /// </summary>
    /// <param name="options">Client configuration options.</param>
    /// <param name="httpClient">Optional pre-configured <see cref="HttpClient"/>. When omitted, one is created internally.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the API key is missing.</exception>
    public EmailAIClient(EmailAIOptions options, HttpClient? httpClient = null)
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
    /// Composes a new email from the given request parameters.
    /// </summary>
    /// <param name="request">Composition parameters including subject, recipients, context, and tone.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="EmailComposeResult"/> containing the composed email body.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when required request fields are missing.</exception>
    /// <exception cref="EmailAIException">Thrown when the API returns an error or unparseable response.</exception>
    public async Task<EmailComposeResult> ComposeAsync(
        EmailComposeRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Subject))
            throw new ArgumentException("Subject must not be empty.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.Context))
            throw new ArgumentException("Context must not be empty.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.Tone))
            throw new ArgumentException("Tone must not be empty.", nameof(request));

        var recipientList = request.Recipients != null && request.Recipients.Count > 0
            ? string.Join(", ", request.Recipients)
            : "the recipient";

        var userMessage =
            $"Subject: {request.Subject}\n" +
            $"Recipients: {recipientList}\n" +
            $"Tone: {request.Tone}\n" +
            $"Context: {request.Context}";

        var raw = await CallApiAsync(ComposeSystemPrompt, userMessage, cancellationToken).ConfigureAwait(false);
        return ParseResult<EmailComposeResult>(raw, "ComposeAsync");
    }

    /// <summary>
    /// Generates a reply to an existing email.
    /// </summary>
    /// <param name="request">Reply parameters including the original email, reply context, and tone.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="EmailReplyResult"/> containing the composed reply body.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when required request fields are missing.</exception>
    /// <exception cref="EmailAIException">Thrown when the API returns an error or unparseable response.</exception>
    public async Task<EmailReplyResult> ReplyAsync(
        EmailReplyRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.OriginalEmail))
            throw new ArgumentException("OriginalEmail must not be empty.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.ReplyContext))
            throw new ArgumentException("ReplyContext must not be empty.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.Tone))
            throw new ArgumentException("Tone must not be empty.", nameof(request));

        var userMessage =
            $"Original email:\n{request.OriginalEmail}\n\n" +
            $"Tone: {request.Tone}\n" +
            $"Reply context: {request.ReplyContext}";

        var raw = await CallApiAsync(ReplySystemPrompt, userMessage, cancellationToken).ConfigureAwait(false);
        return ParseResult<EmailReplyResult>(raw, "ReplyAsync");
    }

    /// <summary>
    /// Summarises an email body, extracting key points and action items.
    /// </summary>
    /// <param name="emailBody">The email body text to summarise.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="EmailSummarizeResult"/> with a concise summary and action items.</returns>
    /// <exception cref="ArgumentException">Thrown when emailBody is null or empty.</exception>
    /// <exception cref="EmailAIException">Thrown when the API returns an error or unparseable response.</exception>
    public async Task<EmailSummarizeResult> SummarizeAsync(
        string emailBody,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(emailBody))
            throw new ArgumentException("EmailBody must not be empty.", nameof(emailBody));

        var raw = await CallApiAsync(SummarizeSystemPrompt, emailBody, cancellationToken).ConfigureAwait(false);
        return ParseResult<EmailSummarizeResult>(raw, "SummarizeAsync");
    }

    /// <summary>
    /// Classifies an email by category, priority, and sentiment.
    /// </summary>
    /// <param name="emailBody">The email body text to classify.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="EmailClassifyResult"/> with category, priority, sentiment, and rationale.</returns>
    /// <exception cref="ArgumentException">Thrown when emailBody is null or empty.</exception>
    /// <exception cref="EmailAIException">Thrown when the API returns an error or unparseable response.</exception>
    public async Task<EmailClassifyResult> ClassifyAsync(
        string emailBody,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(emailBody))
            throw new ArgumentException("EmailBody must not be empty.", nameof(emailBody));

        var raw = await CallApiAsync(ClassifySystemPrompt, emailBody, cancellationToken).ConfigureAwait(false);
        return ParseResult<EmailClassifyResult>(raw, "ClassifyAsync");
    }

    /// <summary>
    /// Composes multiple emails in a single batch (sequential, respects cancellation).
    /// </summary>
    /// <param name="requests">A collection of email composition requests.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="BatchEmailComposeResult"/> containing results in the same order as inputs.</returns>
    /// <exception cref="ArgumentNullException">Thrown when requests is null.</exception>
    /// <exception cref="ArgumentException">Thrown when requests is empty.</exception>
    /// <exception cref="EmailAIException">Thrown when any individual composition fails.</exception>
    public async Task<BatchEmailComposeResult> BatchComposeAsync(
        IEnumerable<EmailComposeRequest> requests,
        CancellationToken cancellationToken = default)
    {
        if (requests == null) throw new ArgumentNullException(nameof(requests));

        var list = requests.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Requests collection must not be empty.", nameof(requests));

        var results = new List<EmailComposeResult>(list.Count);
        foreach (var req in list)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await ComposeAsync(req, cancellationToken).ConfigureAwait(false);
            results.Add(result);
        }

        return new BatchEmailComposeResult { Results = results };
    }

    /// <summary>
    /// Generates replies for multiple emails in a single batch (sequential, respects cancellation).
    /// </summary>
    /// <param name="requests">A collection of email reply requests.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="BatchEmailReplyResult"/> containing results in the same order as inputs.</returns>
    /// <exception cref="ArgumentNullException">Thrown when requests is null.</exception>
    /// <exception cref="ArgumentException">Thrown when requests is empty.</exception>
    /// <exception cref="EmailAIException">Thrown when any individual reply fails.</exception>
    public async Task<BatchEmailReplyResult> BatchReplyAsync(
        IEnumerable<EmailReplyRequest> requests,
        CancellationToken cancellationToken = default)
    {
        if (requests == null) throw new ArgumentNullException(nameof(requests));

        var list = requests.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Requests collection must not be empty.", nameof(requests));

        var results = new List<EmailReplyResult>(list.Count);
        foreach (var req in list)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await ReplyAsync(req, cancellationToken).ConfigureAwait(false);
            results.Add(result);
        }

        return new BatchEmailReplyResult { Results = results };
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private async Task<string> CallApiAsync(
        string systemPrompt,
        string userMessage,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            model = _options.Model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user",   content = userMessage  }
            },
            temperature = 0.3,
            max_tokens = 2048
        };

        var json = JsonSerializer.Serialize(payload);
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl.TrimEnd('/')}/chat/completions")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);

        HttpResponseMessage response;
        try
        {
            response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new EmailAIException("Request timed out. Consider increasing EmailAIOptions.Timeout.", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
#if NETSTANDARD2_0
            var errBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#else
            var errBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#endif
            throw new EmailAIException(
                $"API returned {(int)response.StatusCode} {response.StatusCode}. Body: {errBody}");
        }

#if NETSTANDARD2_0
        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#else
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#endif

        JsonNode? root;
        try { root = JsonNode.Parse(responseBody); }
        catch (JsonException ex)
        {
            throw new EmailAIException($"Invalid JSON from API: {ex.Message}", ex);
        }

        var content = root?["choices"]?[0]?["message"]?["content"]?.GetValue<string>();
        if (string.IsNullOrWhiteSpace(content))
            throw new EmailAIException("API response missing content field.");

        return content!.Trim();
    }

    private static T ParseResult<T>(string raw, string methodName)
    {
        // Strip optional markdown code fences the model might emit
        var json = raw;
        if (json.StartsWith("```"))
        {
            var start = json.IndexOf('\n');
            var end = json.LastIndexOf("```");
            if (start >= 0 && end > start)
                json = json.Substring(start + 1, end - start - 1).Trim();
        }

        try
        {
            var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);
            if (result == null)
                throw new EmailAIException($"{methodName}: Deserialized result was null.");
            return result;
        }
        catch (JsonException ex)
        {
            throw new EmailAIException($"{methodName}: Failed to parse API response. Raw: {raw}", ex);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_ownsHttpClient)
            _http.Dispose();
    }
}
