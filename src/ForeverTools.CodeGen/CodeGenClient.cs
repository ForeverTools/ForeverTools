using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ForeverTools.CodeGen.Models;

namespace ForeverTools.CodeGen;

/// <summary>
/// Client for AI-powered code generation, refactoring, and explanation via the AI/ML API.
/// Get your API key at https://aimlapi.com?via=forevertools
/// </summary>
public class CodeGenClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly CodeGenOptions _options;
    private readonly bool _ownsHttpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string GenerateSystemPrompt =
        "You are a code generation assistant. Given a natural language description, generate clean, production-ready code in the requested language. " +
        "Respond ONLY with a valid JSON object in exactly this format, with no markdown fences or extra text:\n" +
        "{\"code\":\"...\",\"language\":\"python\",\"explanation\":\"Brief description of what the code does.\",\"notes\":\"Any dependencies or caveats, or null.\"}\n" +
        "The code value must be a complete, runnable snippet. Escape all special characters properly.";

    private const string RefactorSystemPrompt =
        "You are a code refactoring assistant. Refactor the provided code for clarity, performance, and best practices. " +
        "Respond ONLY with a valid JSON object in exactly this format, with no markdown fences or extra text:\n" +
        "{\"code\":\"...\",\"language\":\"python\",\"summary\":\"What was changed and why.\",\"improvements\":[\"improvement1\",\"improvement2\"]}\n" +
        "The code value must be the complete refactored snippet. Escape all special characters properly.";

    private const string ExplainSystemPrompt =
        "You are a code explanation assistant. Explain the provided code clearly for a developer audience. " +
        "Respond ONLY with a valid JSON object in exactly this format, with no markdown fences or extra text:\n" +
        "{\"summary\":\"High-level description.\",\"steps\":[\"Step 1: ...\",\"Step 2: ...\"],\"language\":\"python\",\"complexity\":\"simple\"}\n" +
        "complexity must be one of: simple, moderate, complex.";

    /// <summary>
    /// Creates a new <see cref="CodeGenClient"/> with the provided options.
    /// </summary>
    /// <param name="options">Client configuration options.</param>
    /// <param name="httpClient">Optional pre-configured <see cref="HttpClient"/>. When omitted, one is created internally.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the API key is missing.</exception>
    public CodeGenClient(CodeGenOptions options, HttpClient? httpClient = null)
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
    /// Generates code from a natural language description.
    /// </summary>
    /// <param name="prompt">Natural language description of the code to generate.</param>
    /// <param name="language">Target programming language (e.g. "python", "csharp", "javascript"). Defaults to "python".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="CodeGenResult"/> containing the generated code and explanation.</returns>
    /// <exception cref="ArgumentException">Thrown when prompt is null or empty.</exception>
    /// <exception cref="HttpRequestException">Thrown on network failures.</exception>
    /// <exception cref="CodeGenException">Thrown when the API returns an error or unparseable response.</exception>
    public async Task<CodeGenResult> GenerateAsync(
        string prompt,
        string language = "python",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            throw new ArgumentException("Prompt must not be empty.", nameof(prompt));
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language must not be empty.", nameof(language));

        var userMessage = $"Generate {language} code: {prompt}";
        var raw = await CallApiAsync(GenerateSystemPrompt, userMessage, cancellationToken).ConfigureAwait(false);
        return ParseResult<CodeGenResult>(raw, "GenerateAsync");
    }

    /// <summary>
    /// Refactors existing code for clarity, performance, and best practices.
    /// </summary>
    /// <param name="code">The code snippet to refactor.</param>
    /// <param name="language">Programming language of the code (e.g. "python", "csharp"). Defaults to "python".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="RefactorResult"/> containing the refactored code and change summary.</returns>
    /// <exception cref="ArgumentException">Thrown when code is null or empty.</exception>
    /// <exception cref="HttpRequestException">Thrown on network failures.</exception>
    /// <exception cref="CodeGenException">Thrown when the API returns an error or unparseable response.</exception>
    public async Task<RefactorResult> RefactorAsync(
        string code,
        string language = "python",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code must not be empty.", nameof(code));
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language must not be empty.", nameof(language));

        var userMessage = $"Refactor this {language} code:\n```\n{code}\n```";
        var raw = await CallApiAsync(RefactorSystemPrompt, userMessage, cancellationToken).ConfigureAwait(false);
        return ParseResult<RefactorResult>(raw, "RefactorAsync");
    }

    /// <summary>
    /// Explains what a code snippet does.
    /// </summary>
    /// <param name="code">The code snippet to explain.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="ExplainResult"/> with a step-by-step explanation.</returns>
    /// <exception cref="ArgumentException">Thrown when code is null or empty.</exception>
    /// <exception cref="HttpRequestException">Thrown on network failures.</exception>
    /// <exception cref="CodeGenException">Thrown when the API returns an error or unparseable response.</exception>
    public async Task<ExplainResult> ExplainAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code must not be empty.", nameof(code));

        var userMessage = $"Explain this code:\n```\n{code}\n```";
        var raw = await CallApiAsync(ExplainSystemPrompt, userMessage, cancellationToken).ConfigureAwait(false);
        return ParseResult<ExplainResult>(raw, "ExplainAsync");
    }

    /// <summary>
    /// Generates code for multiple prompts in a single call (sequential, respects cancellation).
    /// </summary>
    /// <param name="prompts">A collection of natural language descriptions.</param>
    /// <param name="language">Target programming language for all prompts. Defaults to "python".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="BatchCodeGenResult"/> containing results in the same order as inputs.</returns>
    /// <exception cref="ArgumentNullException">Thrown when prompts is null.</exception>
    /// <exception cref="ArgumentException">Thrown when prompts is empty or language is empty.</exception>
    /// <exception cref="CodeGenException">Thrown when any individual generation fails.</exception>
    public async Task<BatchCodeGenResult> GenerateBatchAsync(
        IEnumerable<string> prompts,
        string language = "python",
        CancellationToken cancellationToken = default)
    {
        if (prompts == null) throw new ArgumentNullException(nameof(prompts));
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language must not be empty.", nameof(language));

        var list = prompts.ToList();
        if (list.Count == 0)
            throw new ArgumentException("Prompts collection must not be empty.", nameof(prompts));

        var results = new List<CodeGenResult>(list.Count);
        foreach (var prompt in list)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await GenerateAsync(prompt, language, cancellationToken).ConfigureAwait(false);
            results.Add(result);
        }

        return new BatchCodeGenResult { Results = results };
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
            temperature = 0.2,
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
            throw new CodeGenException("Request timed out. Consider increasing CodeGenOptions.Timeout.", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
#if NETSTANDARD2_0
            var errBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#else
            var errBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#endif
            throw new CodeGenException(
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
            throw new CodeGenException($"Invalid JSON from API: {ex.Message}", ex);
        }

        var content = root?["choices"]?[0]?["message"]?["content"]?.GetValue<string>();
        if (string.IsNullOrWhiteSpace(content))
            throw new CodeGenException("API response missing content field.");

        return content.Trim();
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
                throw new CodeGenException($"{methodName}: Deserialized result was null.");
            return result;
        }
        catch (JsonException ex)
        {
            throw new CodeGenException($"{methodName}: Failed to parse API response. Raw: {raw}", ex);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_ownsHttpClient)
            _http.Dispose();
    }
}
