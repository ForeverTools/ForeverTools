using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ForeverTools.Apify.Models;

namespace ForeverTools.Apify;

/// <summary>
/// Client for the Apify web scraping and automation platform.
/// Get your API token at: https://www.apify.com/?fpr=8hklqy
/// </summary>
public class ApifyClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ApifyOptions _options;
    private readonly bool _disposeHttpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new Apify client with the specified API token.
    /// Get your API token at: https://www.apify.com/?fpr=8hklqy
    /// </summary>
    /// <param name="token">Your Apify API token.</param>
    public ApifyClient(string token)
        : this(new ApifyOptions { Token = token })
    {
    }

    /// <summary>
    /// Creates a new Apify client with the specified options.
    /// Get your API token at: https://www.apify.com/?fpr=8hklqy
    /// </summary>
    /// <param name="options">Configuration options.</param>
    public ApifyClient(ApifyOptions options)
        : this(options, new HttpClient())
    {
        _disposeHttpClient = true;
    }

    /// <summary>
    /// Creates a new Apify client with the specified options and HttpClient.
    /// Get your API token at: https://www.apify.com/?fpr=8hklqy
    /// </summary>
    /// <param name="options">Configuration options.</param>
    /// <param name="httpClient">HttpClient instance to use.</param>
    public ApifyClient(ApifyOptions options, HttpClient httpClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        if (!_options.HasToken)
        {
            throw new ArgumentException(
                "API token is required. Get one at: https://www.apify.com/?fpr=8hklqy",
                nameof(options));
        }

        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.Token);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// The configured options.
    /// </summary>
    public ApifyOptions Options => _options;

    /// <summary>
    /// Creates a client from an environment variable.
    /// </summary>
    /// <param name="envVarName">Environment variable name. Default: APIFY_TOKEN</param>
    /// <returns>A new ApifyClient.</returns>
    public static ApifyClient FromEnvironment(string envVarName = "APIFY_TOKEN")
    {
        var token = Environment.GetEnvironmentVariable(envVarName);
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                $"Environment variable '{envVarName}' is not set. " +
                "Get your API token at: https://www.apify.com/?fpr=8hklqy");
        }
        return new ApifyClient(token);
    }

    #region Actor Operations

    /// <summary>
    /// Runs an actor and waits for completion.
    /// </summary>
    /// <param name="actorId">Actor ID or full name (e.g., "apify/web-scraper").</param>
    /// <param name="input">Input for the actor (will be serialized to JSON).</param>
    /// <param name="options">Run options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The completed actor run.</returns>
    public async Task<ActorRun> RunActorAsync(
        string actorId,
        object? input = null,
        ActorRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var run = await StartActorAsync(actorId, input, options, cancellationToken);

        if (run.Id == null)
        {
            throw new ApifyException("Failed to start actor run");
        }

        return await WaitForRunAsync(run.Id, cancellationToken);
    }

    /// <summary>
    /// Starts an actor run without waiting for completion.
    /// </summary>
    /// <param name="actorId">Actor ID or full name (e.g., "apify/web-scraper").</param>
    /// <param name="input">Input for the actor (will be serialized to JSON).</param>
    /// <param name="options">Run options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The started actor run.</returns>
    public async Task<ActorRun> StartActorAsync(
        string actorId,
        object? input = null,
        ActorRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(actorId))
            throw new ArgumentException("Actor ID is required", nameof(actorId));

        var url = $"{_options.BaseUrl}/acts/{Uri.EscapeDataString(actorId)}/runs";
        var queryParams = new List<string>();

        if (options?.MemoryMb != null)
            queryParams.Add($"memory={options.MemoryMb}");
        else if (_options.DefaultMemoryMb > 0)
            queryParams.Add($"memory={_options.DefaultMemoryMb}");

        if (options?.TimeoutSeconds != null)
            queryParams.Add($"timeout={options.TimeoutSeconds}");
        else if (_options.DefaultTimeoutSeconds > 0)
            queryParams.Add($"timeout={_options.DefaultTimeoutSeconds}");

        if (options?.Build != null)
            queryParams.Add($"build={Uri.EscapeDataString(options.Build)}");

        if (queryParams.Count > 0)
            url += "?" + string.Join("&", queryParams);

        HttpContent? content = null;
        if (input != null)
        {
            var json = JsonSerializer.Serialize(input, _jsonOptions);
            content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        using var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to start actor: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<ActorRun>>(responseBody, _jsonOptions);
        return result?.Data ?? new ActorRun();
    }

    /// <summary>
    /// Gets information about an actor.
    /// </summary>
    /// <param name="actorId">Actor ID or full name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Actor information.</returns>
    public async Task<Actor> GetActorAsync(string actorId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(actorId))
            throw new ArgumentException("Actor ID is required", nameof(actorId));

        var url = $"{_options.BaseUrl}/acts/{Uri.EscapeDataString(actorId)}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to get actor: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<Actor>>(responseBody, _jsonOptions);
        return result?.Data ?? new Actor();
    }

    /// <summary>
    /// Lists your actors.
    /// </summary>
    /// <param name="limit">Maximum number of actors to return.</param>
    /// <param name="offset">Number of actors to skip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of actors.</returns>
    public async Task<ActorListResponse> ListActorsAsync(
        int? limit = null,
        int? offset = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/acts";
        var queryParams = new List<string>();

        if (limit.HasValue) queryParams.Add($"limit={limit}");
        if (offset.HasValue) queryParams.Add($"offset={offset}");

        if (queryParams.Count > 0)
            url += "?" + string.Join("&", queryParams);

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to list actors: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<ActorListResponse>>(responseBody, _jsonOptions);
        return result?.Data ?? new ActorListResponse();
    }

    #endregion

    #region Run Management

    /// <summary>
    /// Gets an actor run by ID.
    /// </summary>
    /// <param name="runId">Run ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The actor run.</returns>
    public async Task<ActorRun> GetRunAsync(string runId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(runId))
            throw new ArgumentException("Run ID is required", nameof(runId));

        var url = $"{_options.BaseUrl}/actor-runs/{runId}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to get run: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<ActorRun>>(responseBody, _jsonOptions);
        return result?.Data ?? new ActorRun();
    }

    /// <summary>
    /// Waits for an actor run to complete.
    /// </summary>
    /// <param name="runId">Run ID.</param>
    /// <param name="pollIntervalMs">Polling interval in milliseconds.</param>
    /// <param name="maxWaitMs">Maximum wait time in milliseconds (0 = no limit).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The completed actor run.</returns>
    public async Task<ActorRun> WaitForRunAsync(
        string runId,
        CancellationToken cancellationToken = default,
        int? pollIntervalMs = null,
        int maxWaitMs = 0)
    {
        var interval = pollIntervalMs ?? _options.DefaultPollIntervalMs;
        var elapsed = 0;

        while (true)
        {
            var run = await GetRunAsync(runId, cancellationToken);

            if (run.IsFinished)
                return run;

            if (maxWaitMs > 0 && elapsed >= maxWaitMs)
            {
                throw new ApifyException($"Timed out waiting for run {runId}");
            }

            await Task.Delay(interval, cancellationToken);
            elapsed += interval;
        }
    }

    /// <summary>
    /// Aborts an actor run.
    /// </summary>
    /// <param name="runId">Run ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The aborted actor run.</returns>
    public async Task<ActorRun> AbortRunAsync(string runId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(runId))
            throw new ArgumentException("Run ID is required", nameof(runId));

        var url = $"{_options.BaseUrl}/actor-runs/{runId}/abort";

        using var response = await _httpClient.PostAsync(url, null, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to abort run: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<ActorRun>>(responseBody, _jsonOptions);
        return result?.Data ?? new ActorRun();
    }

    /// <summary>
    /// Gets the log from an actor run.
    /// </summary>
    /// <param name="runId">Run ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The run log as text.</returns>
    public async Task<string> GetRunLogAsync(string runId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(runId))
            throw new ArgumentException("Run ID is required", nameof(runId));

        var url = $"{_options.BaseUrl}/actor-runs/{runId}/log";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to get run log: {responseBody}", (int)response.StatusCode);
        }

        return responseBody;
    }

    #endregion

    #region Dataset Operations

    /// <summary>
    /// Gets items from a dataset.
    /// </summary>
    /// <typeparam name="T">Type to deserialize items to.</typeparam>
    /// <param name="datasetId">Dataset ID.</param>
    /// <param name="options">Options for retrieving items.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of items.</returns>
    public async Task<List<T>> GetDatasetItemsAsync<T>(
        string datasetId,
        DatasetItemsOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(datasetId))
            throw new ArgumentException("Dataset ID is required", nameof(datasetId));

        var url = $"{_options.BaseUrl}/datasets/{datasetId}/items";
        var queryParams = new List<string> { "format=json" };

        if (options?.Offset.HasValue == true) queryParams.Add($"offset={options.Offset}");
        if (options?.Limit.HasValue == true) queryParams.Add($"limit={options.Limit}");
        if (options?.Clean.HasValue == true) queryParams.Add($"clean={options.Clean.Value.ToString().ToLower()}");
        if (options?.Fields != null) queryParams.Add($"fields={string.Join(",", options.Fields)}");
        if (options?.OmitFields != null) queryParams.Add($"omit={string.Join(",", options.OmitFields)}");
        if (options?.Unwind != null) queryParams.Add($"unwind={options.Unwind}");
        if (options?.Flatten.HasValue == true) queryParams.Add($"flatten={options.Flatten.Value.ToString().ToLower()}");

        url += "?" + string.Join("&", queryParams);

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to get dataset items: {responseBody}", (int)response.StatusCode);
        }

        return JsonSerializer.Deserialize<List<T>>(responseBody, _jsonOptions) ?? new List<T>();
    }

    /// <summary>
    /// Gets items from a run's default dataset.
    /// </summary>
    /// <typeparam name="T">Type to deserialize items to.</typeparam>
    /// <param name="runId">Run ID.</param>
    /// <param name="options">Options for retrieving items.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of items.</returns>
    public async Task<List<T>> GetRunDatasetItemsAsync<T>(
        string runId,
        DatasetItemsOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var run = await GetRunAsync(runId, cancellationToken);

        if (string.IsNullOrEmpty(run.DefaultDatasetId))
        {
            throw new ApifyException("Run does not have a default dataset");
        }

        return await GetDatasetItemsAsync<T>(run.DefaultDatasetId!, options, cancellationToken);
    }

    /// <summary>
    /// Pushes items to a dataset.
    /// </summary>
    /// <param name="datasetId">Dataset ID.</param>
    /// <param name="items">Items to push.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task PushDatasetItemsAsync(
        string datasetId,
        IEnumerable<object> items,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(datasetId))
            throw new ArgumentException("Dataset ID is required", nameof(datasetId));

        var url = $"{_options.BaseUrl}/datasets/{datasetId}/items";
        var json = JsonSerializer.Serialize(items, _jsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PostAsync(url, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await ReadResponseAsync(response, cancellationToken);
            throw new ApifyException($"Failed to push dataset items: {responseBody}", (int)response.StatusCode);
        }
    }

    /// <summary>
    /// Gets dataset information.
    /// </summary>
    /// <param name="datasetId">Dataset ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dataset information.</returns>
    public async Task<Dataset> GetDatasetAsync(string datasetId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(datasetId))
            throw new ArgumentException("Dataset ID is required", nameof(datasetId));

        var url = $"{_options.BaseUrl}/datasets/{datasetId}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to get dataset: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<Dataset>>(responseBody, _jsonOptions);
        return result?.Data ?? new Dataset();
    }

    /// <summary>
    /// Lists your datasets.
    /// </summary>
    /// <param name="limit">Maximum number to return.</param>
    /// <param name="offset">Number to skip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of datasets.</returns>
    public async Task<DatasetListResponse> ListDatasetsAsync(
        int? limit = null,
        int? offset = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/datasets";
        var queryParams = new List<string>();

        if (limit.HasValue) queryParams.Add($"limit={limit}");
        if (offset.HasValue) queryParams.Add($"offset={offset}");

        if (queryParams.Count > 0)
            url += "?" + string.Join("&", queryParams);

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to list datasets: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<DatasetListResponse>>(responseBody, _jsonOptions);
        return result?.Data ?? new DatasetListResponse();
    }

    #endregion

    #region Key-Value Store Operations

    /// <summary>
    /// Gets a record from a key-value store.
    /// </summary>
    /// <typeparam name="T">Type to deserialize the value to.</typeparam>
    /// <param name="storeId">Store ID.</param>
    /// <param name="key">Record key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The record value.</returns>
    public async Task<T?> GetKeyValueRecordAsync<T>(
        string storeId,
        string key,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storeId))
            throw new ArgumentException("Store ID is required", nameof(storeId));
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key is required", nameof(key));

        var url = $"{_options.BaseUrl}/key-value-stores/{storeId}/records/{Uri.EscapeDataString(key)}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return default;

        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to get record: {responseBody}", (int)response.StatusCode);
        }

        return JsonSerializer.Deserialize<T>(responseBody, _jsonOptions);
    }

    /// <summary>
    /// Gets a record from a key-value store as bytes.
    /// </summary>
    /// <param name="storeId">Store ID.</param>
    /// <param name="key">Record key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The record value as bytes.</returns>
    public async Task<byte[]?> GetKeyValueRecordBytesAsync(
        string storeId,
        string key,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storeId))
            throw new ArgumentException("Store ID is required", nameof(storeId));
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key is required", nameof(key));

        var url = $"{_options.BaseUrl}/key-value-stores/{storeId}/records/{Uri.EscapeDataString(key)}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await ReadResponseAsync(response, cancellationToken);
            throw new ApifyException($"Failed to get record: {responseBody}", (int)response.StatusCode);
        }

        return await response.Content.ReadAsByteArrayAsync(
#if NET5_0_OR_GREATER
            cancellationToken
#endif
        );
    }

    /// <summary>
    /// Sets a record in a key-value store.
    /// </summary>
    /// <param name="storeId">Store ID.</param>
    /// <param name="key">Record key.</param>
    /// <param name="value">Record value.</param>
    /// <param name="contentType">Content type. Default: application/json</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SetKeyValueRecordAsync(
        string storeId,
        string key,
        object value,
        string contentType = "application/json",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storeId))
            throw new ArgumentException("Store ID is required", nameof(storeId));
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key is required", nameof(key));

        var url = $"{_options.BaseUrl}/key-value-stores/{storeId}/records/{Uri.EscapeDataString(key)}";

        HttpContent content;
        if (value is byte[] bytes)
        {
            content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }
        else if (value is string str)
        {
            content = new StringContent(str, Encoding.UTF8, contentType);
        }
        else
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        using var response = await _httpClient.PutAsync(url, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await ReadResponseAsync(response, cancellationToken);
            throw new ApifyException($"Failed to set record: {responseBody}", (int)response.StatusCode);
        }
    }

    /// <summary>
    /// Deletes a record from a key-value store.
    /// </summary>
    /// <param name="storeId">Store ID.</param>
    /// <param name="key">Record key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task DeleteKeyValueRecordAsync(
        string storeId,
        string key,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storeId))
            throw new ArgumentException("Store ID is required", nameof(storeId));
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key is required", nameof(key));

        var url = $"{_options.BaseUrl}/key-value-stores/{storeId}/records/{Uri.EscapeDataString(key)}";

        using var response = await _httpClient.DeleteAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
        {
            var responseBody = await ReadResponseAsync(response, cancellationToken);
            throw new ApifyException($"Failed to delete record: {responseBody}", (int)response.StatusCode);
        }
    }

    /// <summary>
    /// Lists keys in a key-value store.
    /// </summary>
    /// <param name="storeId">Store ID.</param>
    /// <param name="limit">Maximum number to return.</param>
    /// <param name="exclusiveStartKey">Key to start after (for pagination).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of keys.</returns>
    public async Task<KeyValueStoreKeysResponse> ListKeyValueStoreKeysAsync(
        string storeId,
        int? limit = null,
        string? exclusiveStartKey = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storeId))
            throw new ArgumentException("Store ID is required", nameof(storeId));

        var url = $"{_options.BaseUrl}/key-value-stores/{storeId}/keys";
        var queryParams = new List<string>();

        if (limit.HasValue) queryParams.Add($"limit={limit}");
        if (!string.IsNullOrEmpty(exclusiveStartKey))
            queryParams.Add($"exclusiveStartKey={Uri.EscapeDataString(exclusiveStartKey)}");

        if (queryParams.Count > 0)
            url += "?" + string.Join("&", queryParams);

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to list keys: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<KeyValueStoreKeysResponse>>(responseBody, _jsonOptions);
        return result?.Data ?? new KeyValueStoreKeysResponse();
    }

    #endregion

    #region Schedule Operations

    /// <summary>
    /// Creates a new schedule.
    /// </summary>
    /// <param name="request">Schedule configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created schedule.</returns>
    public async Task<Schedule> CreateScheduleAsync(
        ScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var url = $"{_options.BaseUrl}/schedules";
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to create schedule: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<Schedule>>(responseBody, _jsonOptions);
        return result?.Data ?? new Schedule();
    }

    /// <summary>
    /// Gets a schedule by ID.
    /// </summary>
    /// <param name="scheduleId">Schedule ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The schedule.</returns>
    public async Task<Schedule> GetScheduleAsync(string scheduleId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scheduleId))
            throw new ArgumentException("Schedule ID is required", nameof(scheduleId));

        var url = $"{_options.BaseUrl}/schedules/{scheduleId}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to get schedule: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<Schedule>>(responseBody, _jsonOptions);
        return result?.Data ?? new Schedule();
    }

    /// <summary>
    /// Updates a schedule.
    /// </summary>
    /// <param name="scheduleId">Schedule ID.</param>
    /// <param name="request">Updated schedule configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated schedule.</returns>
    public async Task<Schedule> UpdateScheduleAsync(
        string scheduleId,
        ScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scheduleId))
            throw new ArgumentException("Schedule ID is required", nameof(scheduleId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var url = $"{_options.BaseUrl}/schedules/{scheduleId}";
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PutAsync(url, content, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to update schedule: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<Schedule>>(responseBody, _jsonOptions);
        return result?.Data ?? new Schedule();
    }

    /// <summary>
    /// Deletes a schedule.
    /// </summary>
    /// <param name="scheduleId">Schedule ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task DeleteScheduleAsync(string scheduleId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scheduleId))
            throw new ArgumentException("Schedule ID is required", nameof(scheduleId));

        var url = $"{_options.BaseUrl}/schedules/{scheduleId}";

        using var response = await _httpClient.DeleteAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
        {
            var responseBody = await ReadResponseAsync(response, cancellationToken);
            throw new ApifyException($"Failed to delete schedule: {responseBody}", (int)response.StatusCode);
        }
    }

    /// <summary>
    /// Lists your schedules.
    /// </summary>
    /// <param name="limit">Maximum number to return.</param>
    /// <param name="offset">Number to skip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of schedules.</returns>
    public async Task<ScheduleListResponse> ListSchedulesAsync(
        int? limit = null,
        int? offset = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/schedules";
        var queryParams = new List<string>();

        if (limit.HasValue) queryParams.Add($"limit={limit}");
        if (offset.HasValue) queryParams.Add($"offset={offset}");

        if (queryParams.Count > 0)
            url += "?" + string.Join("&", queryParams);

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to list schedules: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<ScheduleListResponse>>(responseBody, _jsonOptions);
        return result?.Data ?? new ScheduleListResponse();
    }

    #endregion

    #region User Operations

    /// <summary>
    /// Gets information about the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User information.</returns>
    public async Task<User> GetUserAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/users/me";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseBody = await ReadResponseAsync(response, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApifyException($"Failed to get user: {responseBody}", (int)response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<ApifyDataResponse<User>>(responseBody, _jsonOptions);
        return result?.Data ?? new User();
    }

    #endregion

    #region Convenience Methods

    /// <summary>
    /// Runs an actor and returns the dataset results.
    /// </summary>
    /// <typeparam name="T">Type to deserialize results to.</typeparam>
    /// <param name="actorId">Actor ID or full name.</param>
    /// <param name="input">Input for the actor.</param>
    /// <param name="options">Run options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of results from the actor's dataset.</returns>
    public async Task<List<T>> ScrapeAsync<T>(
        string actorId,
        object? input = null,
        ActorRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var run = await RunActorAsync(actorId, input, options, cancellationToken);

        if (!run.IsSucceeded)
        {
            throw new ApifyException($"Actor run failed with status: {run.Status}");
        }

        if (string.IsNullOrEmpty(run.DefaultDatasetId))
        {
            return new List<T>();
        }

        return await GetDatasetItemsAsync<T>(run.DefaultDatasetId!, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Runs an actor and returns the output from the key-value store.
    /// </summary>
    /// <typeparam name="T">Type to deserialize output to.</typeparam>
    /// <param name="actorId">Actor ID or full name.</param>
    /// <param name="input">Input for the actor.</param>
    /// <param name="outputKey">Key to retrieve from the store. Default: "OUTPUT"</param>
    /// <param name="options">Run options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The output value.</returns>
    public async Task<T?> RunAndGetOutputAsync<T>(
        string actorId,
        object? input = null,
        string outputKey = "OUTPUT",
        ActorRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var run = await RunActorAsync(actorId, input, options, cancellationToken);

        if (!run.IsSucceeded)
        {
            throw new ApifyException($"Actor run failed with status: {run.Status}");
        }

        if (string.IsNullOrEmpty(run.DefaultKeyValueStoreId))
        {
            return default;
        }

        return await GetKeyValueRecordAsync<T>(run.DefaultKeyValueStoreId!, outputKey, cancellationToken);
    }

    #endregion

    #region Helpers

    private async Task<string> ReadResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        return await response.Content.ReadAsStringAsync(
#if NET5_0_OR_GREATER
            cancellationToken
#endif
        );
    }

    #endregion

    /// <summary>
    /// Disposes the HTTP client if it was created internally.
    /// </summary>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
    }
}

/// <summary>
/// Wrapper for Apify API responses with data property.
/// </summary>
internal class ApifyDataResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}

/// <summary>
/// Exception thrown when an Apify API request fails.
/// </summary>
public class ApifyException : Exception
{
    /// <summary>
    /// HTTP status code from the failed request.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Creates a new ApifyException.
    /// </summary>
    public ApifyException(string message, int statusCode = 0) : base(message)
    {
        StatusCode = statusCode;
    }
}
