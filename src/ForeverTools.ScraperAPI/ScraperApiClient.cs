using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;
using ForeverTools.ScraperAPI.Models;

namespace ForeverTools.ScraperAPI;

/// <summary>
/// Client for the ScraperAPI web scraping service.
/// Get your API key at: https://www.scraperapi.com/signup?fp_ref=chris88
/// </summary>
public class ScraperApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ScraperApiOptions _options;
    private readonly bool _disposeHttpClient;

    /// <summary>
    /// Creates a new ScraperAPI client with the specified API key.
    /// Get your API key at: https://www.scraperapi.com/signup?fp_ref=chris88
    /// </summary>
    /// <param name="apiKey">Your ScraperAPI API key.</param>
    public ScraperApiClient(string apiKey)
        : this(new ScraperApiOptions { ApiKey = apiKey })
    {
    }

    /// <summary>
    /// Creates a new ScraperAPI client with the specified options.
    /// Get your API key at: https://www.scraperapi.com/signup?fp_ref=chris88
    /// </summary>
    /// <param name="options">Configuration options.</param>
    public ScraperApiClient(ScraperApiOptions options)
        : this(options, new HttpClient())
    {
        _disposeHttpClient = true;
    }

    /// <summary>
    /// Creates a new ScraperAPI client with the specified options and HttpClient.
    /// Get your API key at: https://www.scraperapi.com/signup?fp_ref=chris88
    /// </summary>
    /// <param name="options">Configuration options.</param>
    /// <param name="httpClient">HttpClient instance to use.</param>
    public ScraperApiClient(ScraperApiOptions options, HttpClient httpClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        if (!_options.HasApiKey)
        {
            throw new ArgumentException(
                "API key is required. Get one at: https://www.scraperapi.com/signup?fp_ref=chris88",
                nameof(options));
        }

        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    /// <summary>
    /// The configured options.
    /// </summary>
    public ScraperApiOptions Options => _options;

    /// <summary>
    /// Creates a client from an environment variable.
    /// </summary>
    /// <param name="envVarName">Environment variable name. Default: SCRAPERAPI_KEY</param>
    /// <returns>A new ScraperApiClient.</returns>
    public static ScraperApiClient FromEnvironment(string envVarName = "SCRAPERAPI_KEY")
    {
        var apiKey = Environment.GetEnvironmentVariable(envVarName);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                $"Environment variable '{envVarName}' is not set. " +
                "Get your API key at: https://www.scraperapi.com/signup?fp_ref=chris88");
        }
        return new ScraperApiClient(apiKey);
    }

    #region Simple Scraping

    /// <summary>
    /// Scrapes a URL and returns the HTML content.
    /// </summary>
    /// <param name="url">The URL to scrape.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The scraped HTML content.</returns>
    public async Task<string> ScrapeAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await ScrapeWithResponseAsync(new ScrapeRequest { Url = url }, cancellationToken);
        if (!response.Success)
        {
            throw new ScraperApiException(response.Error ?? "Scrape failed", response.StatusCode);
        }
        return response.Content ?? string.Empty;
    }

    /// <summary>
    /// Scrapes a URL with JavaScript rendering enabled.
    /// </summary>
    /// <param name="url">The URL to scrape.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The scraped HTML content after JavaScript execution.</returns>
    public async Task<string> ScrapeWithJavaScriptAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await ScrapeWithResponseAsync(new ScrapeRequest
        {
            Url = url,
            RenderJavaScript = true
        }, cancellationToken);

        if (!response.Success)
        {
            throw new ScraperApiException(response.Error ?? "Scrape failed", response.StatusCode);
        }
        return response.Content ?? string.Empty;
    }

    /// <summary>
    /// Scrapes a URL from a specific country.
    /// </summary>
    /// <param name="url">The URL to scrape.</param>
    /// <param name="countryCode">Country code (e.g., "us", "uk", "de").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The scraped HTML content.</returns>
    public async Task<string> ScrapeFromCountryAsync(string url, string countryCode, CancellationToken cancellationToken = default)
    {
        var response = await ScrapeWithResponseAsync(new ScrapeRequest
        {
            Url = url,
            CountryCode = countryCode
        }, cancellationToken);

        if (!response.Success)
        {
            throw new ScraperApiException(response.Error ?? "Scrape failed", response.StatusCode);
        }
        return response.Content ?? string.Empty;
    }

    #endregion

    #region Full Scrape Request

    /// <summary>
    /// Scrapes a URL with full request configuration.
    /// </summary>
    /// <param name="request">The scrape request configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The scrape response with content and metadata.</returns>
    public async Task<ScrapeResponse> ScrapeWithResponseAsync(ScrapeRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Url)) throw new ArgumentException("URL is required", nameof(request));

        var queryParams = BuildQueryParams(request);
        var requestUrl = $"{_options.BaseUrl}?{queryParams}";

        try
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            if (request.KeepHeaders && request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(
#if NET5_0_OR_GREATER
                cancellationToken
#endif
            );

            if (request.Screenshot && response.IsSuccessStatusCode)
            {
                return new ScrapeResponse
                {
                    Success = true,
                    ScreenshotBase64 = content,
                    StatusCode = (int)response.StatusCode,
                    Url = request.Url
                };
            }

            return new ScrapeResponse
            {
                Success = response.IsSuccessStatusCode,
                Content = content,
                StatusCode = (int)response.StatusCode,
                Url = request.Url,
                Error = response.IsSuccessStatusCode ? null : content
            };
        }
        catch (HttpRequestException ex)
        {
            return new ScrapeResponse
            {
                Success = false,
                Error = ex.Message,
                Url = request.Url
            };
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            return new ScrapeResponse
            {
                Success = false,
                Error = "Request timed out",
                Url = request.Url
            };
        }
    }

    #endregion

    #region Screenshots

    /// <summary>
    /// Takes a screenshot of a URL.
    /// </summary>
    /// <param name="url">The URL to screenshot.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Screenshot as base64 encoded string.</returns>
    public async Task<string> TakeScreenshotAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await ScrapeWithResponseAsync(new ScrapeRequest
        {
            Url = url,
            Screenshot = true
        }, cancellationToken);

        if (!response.Success)
        {
            throw new ScraperApiException(response.Error ?? "Screenshot failed", response.StatusCode);
        }
        return response.ScreenshotBase64 ?? response.Content ?? string.Empty;
    }

    /// <summary>
    /// Takes a screenshot and returns it as bytes.
    /// </summary>
    /// <param name="url">The URL to screenshot.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Screenshot as byte array.</returns>
    public async Task<byte[]> TakeScreenshotBytesAsync(string url, CancellationToken cancellationToken = default)
    {
        var base64 = await TakeScreenshotAsync(url, cancellationToken);
        return Convert.FromBase64String(base64);
    }

    #endregion

    #region Async Jobs

    /// <summary>
    /// Submits an async scrape job for background processing.
    /// </summary>
    /// <param name="request">The scrape request configuration.</param>
    /// <param name="callbackUrl">Optional webhook URL for completion notification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The async job response with job ID.</returns>
    public async Task<AsyncJobResponse> SubmitAsyncJobAsync(ScrapeRequest request, string? callbackUrl = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Url)) throw new ArgumentException("URL is required", nameof(request));

        var jobRequest = new
        {
            apiKey = _options.ApiKey,
            url = request.Url,
            render = request.RenderJavaScript,
            country_code = request.CountryCode,
            premium = request.Premium,
            ultra_premium = request.UltraPremium,
            session_number = request.SessionNumber,
            device_type = request.DeviceType,
            autoparse = request.AutoParse,
            keep_headers = request.KeepHeaders,
            callback = callbackUrl
        };

        var json = JsonSerializer.Serialize(jobRequest, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync($"{_options.AsyncBaseUrl}/jobs", content, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(
#if NET5_0_OR_GREATER
            cancellationToken
#endif
        );

        if (!response.IsSuccessStatusCode)
        {
            return new AsyncJobResponse { Status = "failed" };
        }

        return JsonSerializer.Deserialize<AsyncJobResponse>(responseContent) ?? new AsyncJobResponse();
    }

    /// <summary>
    /// Gets the status of an async job.
    /// </summary>
    /// <param name="jobId">The job ID to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The job status and results if complete.</returns>
    public async Task<AsyncJobStatus> GetAsyncJobStatusAsync(string jobId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(jobId)) throw new ArgumentException("Job ID is required", nameof(jobId));

        var url = $"{_options.AsyncBaseUrl}/jobs/{jobId}?apiKey={_options.ApiKey}";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(
#if NET5_0_OR_GREATER
            cancellationToken
#endif
        );

        return JsonSerializer.Deserialize<AsyncJobStatus>(content) ?? new AsyncJobStatus();
    }

    /// <summary>
    /// Submits an async job and waits for completion.
    /// </summary>
    /// <param name="request">The scrape request configuration.</param>
    /// <param name="pollIntervalMs">Polling interval in milliseconds. Default: 2000.</param>
    /// <param name="maxWaitMs">Maximum wait time in milliseconds. Default: 300000 (5 min).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The completed job result.</returns>
    public async Task<ScrapeResponse> ScrapeAsyncAndWaitAsync(
        ScrapeRequest request,
        int pollIntervalMs = 2000,
        int maxWaitMs = 300000,
        CancellationToken cancellationToken = default)
    {
        var job = await SubmitAsyncJobAsync(request, cancellationToken: cancellationToken);
        if (!job.Success || string.IsNullOrEmpty(job.Id))
        {
            return new ScrapeResponse { Success = false, Error = "Failed to submit async job" };
        }

        var elapsed = 0;
        while (elapsed < maxWaitMs)
        {
            await Task.Delay(pollIntervalMs, cancellationToken);
            elapsed += pollIntervalMs;

            var status = await GetAsyncJobStatusAsync(job.Id!, cancellationToken);

            if (status.IsFinished && status.Response != null)
            {
                return new ScrapeResponse
                {
                    Success = true,
                    Content = status.Response.Body,
                    StatusCode = status.Response.StatusCode,
                    Headers = status.Response.Headers,
                    Url = request.Url
                };
            }

            if (status.IsFailed)
            {
                return new ScrapeResponse { Success = false, Error = "Async job failed", Url = request.Url };
            }
        }

        return new ScrapeResponse { Success = false, Error = "Async job timed out", Url = request.Url };
    }

    #endregion

    #region Account

    /// <summary>
    /// Gets account information including remaining credits.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Account information.</returns>
    public async Task<AccountInfo> GetAccountInfoAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/account?api_key={_options.ApiKey}";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(
#if NET5_0_OR_GREATER
            cancellationToken
#endif
        );

        return JsonSerializer.Deserialize<AccountInfo>(content) ?? new AccountInfo();
    }

    #endregion

    #region Helpers

    private string BuildQueryParams(ScrapeRequest request)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["api_key"] = _options.ApiKey;
        query["url"] = request.Url;

        var render = request.RenderJavaScript || request.Screenshot || _options.DefaultRenderJavaScript;
        if (render) query["render"] = "true";
        if (request.Screenshot) query["screenshot"] = "true";

        var countryCode = request.CountryCode ?? _options.DefaultCountryCode;
        if (!string.IsNullOrEmpty(countryCode)) query["country_code"] = countryCode;

        var premium = request.Premium || _options.DefaultPremium;
        if (premium) query["premium"] = "true";
        if (request.UltraPremium) query["ultra_premium"] = "true";

        if (request.SessionNumber.HasValue) query["session_number"] = request.SessionNumber.Value.ToString();

        var deviceType = request.DeviceType ?? _options.DefaultDeviceType;
        if (!string.IsNullOrEmpty(deviceType)) query["device_type"] = deviceType;

        if (request.AutoParse) query["autoparse"] = "true";
        if (request.KeepHeaders) query["keep_headers"] = "true";
        if (!request.FollowRedirect) query["follow_redirect"] = "false";
        if (!string.IsNullOrEmpty(request.OutputFormat)) query["output_format"] = request.OutputFormat;

        return query.ToString() ?? string.Empty;
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
/// Exception thrown when a ScraperAPI request fails.
/// </summary>
public class ScraperApiException : Exception
{
    /// <summary>
    /// HTTP status code from the failed request.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Creates a new ScraperApiException.
    /// </summary>
    public ScraperApiException(string message, int statusCode = 0) : base(message)
    {
        StatusCode = statusCode;
    }
}
