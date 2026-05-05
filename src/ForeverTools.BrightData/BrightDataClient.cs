using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ForeverTools.BrightData.Models;

namespace ForeverTools.BrightData;

/// <summary>
/// Client for the BrightData Web Scraper API.
/// Extract structured data from any website at scale.
/// Get your API token at: https://get.brightdata.com/ForeverToolsWebScraper
/// </summary>
public class BrightDataClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly BrightDataOptions _options;
    private readonly bool _disposeHttpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Creates a new BrightData client with the specified API token.
    /// Get your token at: https://get.brightdata.com/ForeverToolsWebScraper
    /// </summary>
    public BrightDataClient(string apiToken)
        : this(new BrightDataOptions { ApiToken = apiToken })
    {
    }

    /// <summary>
    /// Creates a new BrightData client with the specified options.
    /// Get your token at: https://get.brightdata.com/ForeverToolsWebScraper
    /// </summary>
    public BrightDataClient(BrightDataOptions options)
        : this(options, new HttpClient())
    {
        _disposeHttpClient = true;
    }

    /// <summary>
    /// Creates a new BrightData client with options and a custom HttpClient.
    /// Get your token at: https://get.brightdata.com/ForeverToolsWebScraper
    /// </summary>
    public BrightDataClient(BrightDataOptions options, HttpClient httpClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        if (!_options.HasApiToken)
            throw new ArgumentException(
                "API token is required. Get one at: https://get.brightdata.com/ForeverToolsWebScraper",
                nameof(options));

        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiToken);
    }

    /// <summary>Creates a client from an environment variable. Default: BRIGHTDATA_API_TOKEN.</summary>
    public static BrightDataClient FromEnvironment(string envVarName = "BRIGHTDATA_API_TOKEN")
    {
        var token = Environment.GetEnvironmentVariable(envVarName);
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException(
                $"Environment variable '{envVarName}' is not set. " +
                "Get your token at: https://get.brightdata.com/ForeverToolsWebScraper");
        return new BrightDataClient(token);
    }

    /// <summary>The configured options.</summary>
    public BrightDataOptions Options => _options;

    // -------------------------------------------------------------------------
    // Dataset Management
    // -------------------------------------------------------------------------

    /// <summary>Lists all datasets (scraper zones) available in your account.</summary>
    public async Task<List<DatasetInfo>> ListDatasetsAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/datasets/v3/list";
        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<DatasetInfo>>(json, _jsonOptions) ?? new();
    }

    // -------------------------------------------------------------------------
    // Triggering Scrapes
    // -------------------------------------------------------------------------

    /// <summary>
    /// Triggers a scrape job for the given dataset and inputs.
    /// Returns a snapshot ID that can be polled for completion.
    /// </summary>
    /// <param name="datasetId">The dataset ID to scrape (e.g. "gd_l1vikfnt1wgvvsz95k").</param>
    /// <param name="inputs">Input records — each is a dict of field name to value (e.g. {"url": "..."}).</param>
    /// <param name="notifyUrl">Optional webhook URL to call when complete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<TriggerResponse> TriggerAsync(
        string datasetId,
        IEnumerable<Dictionary<string, string>> inputs,
        string? notifyUrl = null,
        CancellationToken cancellationToken = default)
    {
        var request = new TriggerRequest
        {
            Inputs = inputs.ToList(),
            NotifyUrl = notifyUrl,
        };
        return await TriggerAsync(datasetId, request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Triggers a scrape job with a single URL input.</summary>
    public async Task<TriggerResponse> TriggerUrlAsync(
        string datasetId,
        string url,
        CancellationToken cancellationToken = default)
    {
        return await TriggerAsync(
            datasetId,
            new[] { new Dictionary<string, string> { ["url"] = url } },
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Triggers a scrape job with a list of URLs.</summary>
    public async Task<TriggerResponse> TriggerUrlsAsync(
        string datasetId,
        IEnumerable<string> urls,
        CancellationToken cancellationToken = default)
    {
        var inputs = urls.Select(u => new Dictionary<string, string> { ["url"] = u });
        return await TriggerAsync(datasetId, inputs, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Triggers a scrape job with a full <see cref="TriggerRequest"/>.</summary>
    public async Task<TriggerResponse> TriggerAsync(
        string datasetId,
        TriggerRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(datasetId))
            throw new ArgumentException("Dataset ID is required.", nameof(datasetId));
        if (request.Inputs.Count == 0)
            throw new ArgumentException("At least one input is required.", nameof(request));

        var url = $"{_options.BaseUrl}/datasets/v3/trigger?dataset_id={Uri.EscapeDataString(datasetId)}&include_errors=true";
        var json = JsonSerializer.Serialize(request.Inputs, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);

        var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<TriggerResponse>(responseJson, _jsonOptions)
            ?? throw new InvalidOperationException("Empty response from trigger endpoint.");
    }

    // -------------------------------------------------------------------------
    // Snapshot Status & Download
    // -------------------------------------------------------------------------

    /// <summary>Gets the current status of a snapshot.</summary>
    public async Task<SnapshotStatus> GetSnapshotStatusAsync(
        string snapshotId,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/datasets/v3/snapshot/{Uri.EscapeDataString(snapshotId)}/progress";
        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<SnapshotStatus>(json, _jsonOptions)
            ?? throw new InvalidOperationException("Empty status response.");
    }

    /// <summary>
    /// Polls until the snapshot is ready or times out.
    /// Throws <see cref="TimeoutException"/> if <see cref="BrightDataOptions.MaxWaitSeconds"/> is exceeded.
    /// Throws <see cref="BrightDataException"/> if the snapshot fails.
    /// </summary>
    public async Task<SnapshotStatus> WaitForSnapshotAsync(
        string snapshotId,
        CancellationToken cancellationToken = default)
    {
        var deadline = DateTime.UtcNow.AddSeconds(_options.MaxWaitSeconds);
        var pollInterval = TimeSpan.FromSeconds(_options.PollIntervalSeconds);

        while (DateTime.UtcNow < deadline)
        {
            var status = await GetSnapshotStatusAsync(snapshotId, cancellationToken).ConfigureAwait(false);

            if (status.IsReady) return status;
            if (status.IsFailed)
                throw new BrightDataException($"Snapshot {snapshotId} failed: {status.Error}");

            await Task.Delay(pollInterval, cancellationToken).ConfigureAwait(false);
        }

        throw new TimeoutException(
            $"Snapshot {snapshotId} did not complete within {_options.MaxWaitSeconds}s.");
    }

    /// <summary>Downloads snapshot results as raw JSON string.</summary>
    public async Task<string> DownloadSnapshotAsync(
        string snapshotId,
        string format = "json",
        CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/datasets/v3/snapshot/{Uri.EscapeDataString(snapshotId)}?format={format}";
        var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    /// <summary>Downloads and deserializes snapshot results as a list of typed objects.</summary>
    public async Task<List<T>> DownloadSnapshotAsync<T>(
        string snapshotId,
        CancellationToken cancellationToken = default)
    {
        var json = await DownloadSnapshotAsync(snapshotId, "json", cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions) ?? new();
    }

    // -------------------------------------------------------------------------
    // Convenience: Trigger + Wait + Download
    // -------------------------------------------------------------------------

    /// <summary>
    /// Triggers a scrape, waits for completion, and returns raw JSON results.
    /// Convenience wrapper for the full trigger → poll → download cycle.
    /// </summary>
    public async Task<string> ScrapeAsync(
        string datasetId,
        IEnumerable<Dictionary<string, string>> inputs,
        CancellationToken cancellationToken = default)
    {
        var trigger = await TriggerAsync(datasetId, inputs, cancellationToken: cancellationToken).ConfigureAwait(false);
        await WaitForSnapshotAsync(trigger.SnapshotId, cancellationToken).ConfigureAwait(false);
        return await DownloadSnapshotAsync(trigger.SnapshotId, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Triggers a scrape for one or more URLs, waits, and returns typed results.
    /// </summary>
    public async Task<List<T>> ScrapeUrlsAsync<T>(
        string datasetId,
        IEnumerable<string> urls,
        CancellationToken cancellationToken = default)
    {
        var trigger = await TriggerUrlsAsync(datasetId, urls, cancellationToken).ConfigureAwait(false);
        await WaitForSnapshotAsync(trigger.SnapshotId, cancellationToken).ConfigureAwait(false);
        return await DownloadSnapshotAsync<T>(trigger.SnapshotId, cancellationToken).ConfigureAwait(false);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;
        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        throw new BrightDataException(
            $"BrightData API error {(int)response.StatusCode}: {body}");
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposeHttpClient)
            _httpClient.Dispose();
    }
}
