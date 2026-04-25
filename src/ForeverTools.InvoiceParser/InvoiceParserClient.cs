using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using ForeverTools.InvoiceParser.Models;

namespace ForeverTools.InvoiceParser;

/// <summary>
/// Client for parsing invoices via the AI/ML API.
/// Get your API key at https://aimlapi.com?via=forevertools
/// </summary>
public class InvoiceParserClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly InvoiceParserOptions _options;
    private readonly bool _ownsHttpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // -------------------------------------------------------------------------
    // Constructors
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a new <see cref="InvoiceParserClient"/> with the given options.
    /// </summary>
    /// <param name="options">Client configuration options.</param>
    /// <param name="httpClient">Optional pre-configured <see cref="HttpClient"/>. When omitted, one is created internally.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the API key is missing.</exception>
    public InvoiceParserClient(InvoiceParserOptions options, HttpClient? httpClient = null)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new ArgumentException(
                "API key is required. Get your key at https://aimlapi.com?via=forevertools",
                nameof(options));

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
    /// Creates a new <see cref="InvoiceParserClient"/> with a plain API key.
    /// </summary>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    public InvoiceParserClient(string apiKey) : this(new InvoiceParserOptions { ApiKey = apiKey }) { }

    // -------------------------------------------------------------------------
    // Static factory
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates an <see cref="InvoiceParserClient"/> by reading the API key from an environment variable.
    /// </summary>
    /// <param name="envVar">The environment variable name. Defaults to <c>AIML_API_KEY</c>.</param>
    /// <returns>A ready-to-use <see cref="InvoiceParserClient"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the environment variable is not set.</exception>
    public static InvoiceParserClient FromEnvironment(string envVar = "AIML_API_KEY")
    {
        return new InvoiceParserClient(InvoiceParserOptions.FromEnvironment(envVar));
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Parses an invoice from a local file path.
    /// </summary>
    /// <param name="filePath">Absolute path to the invoice file (PDF, PNG, JPG, etc.).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The parsed <see cref="InvoiceResult"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is empty or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    public async Task<InvoiceResult> ParseAsync(
        string filePath,
        CancellationToken ct = default)
    {
        if (filePath == null) throw new ArgumentNullException(nameof(filePath));
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path must not be empty or whitespace.", nameof(filePath));
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Invoice file not found: {filePath}", filePath);

#if NETSTANDARD2_0
        var fileBytes = File.ReadAllBytes(filePath);
        using var stream = new MemoryStream(fileBytes);
#else
        await using var stream = File.OpenRead(filePath);
#endif
        var fileName = Path.GetFileName(filePath);
        return await ParseStreamCoreAsync(stream, fileName, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Parses an invoice from a public URL.
    /// </summary>
    /// <param name="url">The publicly accessible URL of the invoice file.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The parsed <see cref="InvoiceResult"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="url"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="url"/> is empty, whitespace, or not a valid absolute URI.</exception>
    public async Task<InvoiceResult> ParseFromUrlAsync(
        string url,
        CancellationToken ct = default)
    {
        if (url == null) throw new ArgumentNullException(nameof(url));
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL must not be empty or whitespace.", nameof(url));
        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            throw new ArgumentException($"'{url}' is not a valid absolute URL.", nameof(url));

        return await CallApiWithUrlAsync(url, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Parses an invoice from a <see cref="Stream"/>.
    /// The stream does not need to be seekable.
    /// </summary>
    /// <param name="stream">The stream containing the invoice file data.</param>
    /// <param name="fileName">
    /// Optional file name hint (e.g. <c>invoice.pdf</c>) used for the multipart form-data content-disposition.
    /// Defaults to <c>invoice</c> when omitted.
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The parsed <see cref="InvoiceResult"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="stream"/> is null.</exception>
    public async Task<InvoiceResult> ParseFromStreamAsync(
        Stream stream,
        string? fileName = null,
        CancellationToken ct = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        var name = string.IsNullOrWhiteSpace(fileName) ? "invoice" : fileName;
        return await ParseStreamCoreAsync(stream, name, ct).ConfigureAwait(false);
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private async Task<InvoiceResult> ParseStreamCoreAsync(
        Stream stream, string fileName, CancellationToken ct)
    {
        using var form = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        form.Add(fileContent, "file", fileName);

        return await SendRequestAsync(form, ct).ConfigureAwait(false);
    }

    private async Task<InvoiceResult> CallApiWithUrlAsync(string url, CancellationToken ct)
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(url), "url");

        return await SendRequestAsync(form, ct).ConfigureAwait(false);
    }

    private async Task<InvoiceResult> SendRequestAsync(
        MultipartFormDataContent form, CancellationToken ct)
    {
        var endpoint = $"{_options.BaseUrl.TrimEnd('/')}/v1/invoice/parse";

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = form
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
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#else
        var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
#endif

        var result = JsonSerializer.Deserialize<InvoiceResult>(json, _jsonOptions);
        return result ?? throw new InvalidOperationException(
            "The API returned an empty or null response.");
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_ownsHttpClient) _http.Dispose();
    }
}
