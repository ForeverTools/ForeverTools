using System.Net.Http.Headers;
using System.Text.Json;
using ForeverTools.APILayer.Models;

namespace ForeverTools.APILayer;

/// <summary>
/// Client for APILayer marketplace APIs.
/// Get your API key at: https://apilayer.com?fpr=chris72
/// </summary>
public class ApiLayerClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ApiLayerOptions _options;
    private readonly bool _disposeHttpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new APILayer client with the specified API key.
    /// Get your API key at: https://apilayer.com?fpr=chris72
    /// </summary>
    /// <param name="apiKey">Your APILayer API key.</param>
    public ApiLayerClient(string apiKey)
        : this(new ApiLayerOptions { ApiKey = apiKey })
    {
    }

    /// <summary>
    /// Creates a new APILayer client with the specified options.
    /// Get your API key at: https://apilayer.com?fpr=chris72
    /// </summary>
    /// <param name="options">Configuration options.</param>
    public ApiLayerClient(ApiLayerOptions options)
        : this(options, new HttpClient())
    {
        _disposeHttpClient = true;
    }

    /// <summary>
    /// Creates a new APILayer client with the specified options and HttpClient.
    /// Get your API key at: https://apilayer.com?fpr=chris72
    /// </summary>
    /// <param name="options">Configuration options.</param>
    /// <param name="httpClient">HttpClient instance to use.</param>
    public ApiLayerClient(ApiLayerOptions options, HttpClient httpClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        if (!_options.HasApiKey)
        {
            throw new ArgumentException(
                "API key is required. Get one at: https://apilayer.com?fpr=chris72",
                nameof(options));
        }

        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        _httpClient.DefaultRequestHeaders.Add("apikey", _options.ApiKey);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// The configured options.
    /// </summary>
    public ApiLayerOptions Options => _options;

    /// <summary>
    /// Creates a client from an environment variable.
    /// </summary>
    /// <param name="envVarName">Environment variable name. Default: APILAYER_KEY</param>
    /// <returns>A new ApiLayerClient.</returns>
    public static ApiLayerClient FromEnvironment(string envVarName = "APILAYER_KEY")
    {
        var apiKey = Environment.GetEnvironmentVariable(envVarName);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                $"Environment variable '{envVarName}' is not set. " +
                "Get your API key at: https://apilayer.com?fpr=chris72");
        }
        return new ApiLayerClient(apiKey);
    }

    #region IP Geolocation (ipstack)

    /// <summary>
    /// Looks up geolocation data for an IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address to look up.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Geolocation data for the IP address.</returns>
    public async Task<ApiLayerResponse<IpGeolocationResult>> GetIpGeolocationAsync(
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address is required", nameof(ipAddress));

        var url = $"{_options.BaseUrl}/ip_to_location?ip_address={Uri.EscapeDataString(ipAddress)}";
        return await GetAsync<IpGeolocationResult>(url, cancellationToken);
    }

    /// <summary>
    /// Looks up geolocation data for the caller's IP address.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Geolocation data for the current IP.</returns>
    public async Task<ApiLayerResponse<IpGeolocationResult>> GetMyIpGeolocationAsync(
        CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/ip_to_location";
        return await GetAsync<IpGeolocationResult>(url, cancellationToken);
    }

    #endregion

    #region Currency Exchange (exchangerates_data)

    /// <summary>
    /// Gets the latest exchange rates.
    /// </summary>
    /// <param name="baseCurrency">Base currency code (default: EUR).</param>
    /// <param name="symbols">Specific currencies to get (null for all).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Latest exchange rates.</returns>
    public async Task<ApiLayerResponse<ExchangeRatesResult>> GetExchangeRatesAsync(
        string baseCurrency = "EUR",
        string[]? symbols = null,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/exchangerates_data/latest?base={Uri.EscapeDataString(baseCurrency)}";
        if (symbols != null && symbols.Length > 0)
        {
            url += $"&symbols={string.Join(",", symbols)}";
        }
        return await GetAsync<ExchangeRatesResult>(url, cancellationToken);
    }

    /// <summary>
    /// Gets historical exchange rates for a specific date.
    /// </summary>
    /// <param name="date">The date (YYYY-MM-DD).</param>
    /// <param name="baseCurrency">Base currency code (default: EUR).</param>
    /// <param name="symbols">Specific currencies to get (null for all).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Historical exchange rates.</returns>
    public async Task<ApiLayerResponse<ExchangeRatesResult>> GetHistoricalRatesAsync(
        string date,
        string baseCurrency = "EUR",
        string[]? symbols = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(date))
            throw new ArgumentException("Date is required", nameof(date));

        var url = $"{_options.BaseUrl}/exchangerates_data/{date}?base={Uri.EscapeDataString(baseCurrency)}";
        if (symbols != null && symbols.Length > 0)
        {
            url += $"&symbols={string.Join(",", symbols)}";
        }
        return await GetAsync<ExchangeRatesResult>(url, cancellationToken);
    }

    /// <summary>
    /// Converts an amount between two currencies.
    /// </summary>
    /// <param name="from">Source currency code.</param>
    /// <param name="to">Target currency code.</param>
    /// <param name="amount">Amount to convert.</param>
    /// <param name="date">Optional historical date (YYYY-MM-DD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Conversion result.</returns>
    public async Task<ApiLayerResponse<CurrencyConversionResult>> ConvertCurrencyAsync(
        string from,
        string to,
        decimal amount,
        string? date = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(from))
            throw new ArgumentException("Source currency is required", nameof(from));
        if (string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("Target currency is required", nameof(to));

        var url = $"{_options.BaseUrl}/exchangerates_data/convert?from={from}&to={to}&amount={amount}";
        if (!string.IsNullOrEmpty(date))
        {
            url += $"&date={date}";
        }
        return await GetAsync<CurrencyConversionResult>(url, cancellationToken);
    }

    /// <summary>
    /// Gets all available currency symbols.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Available currency symbols.</returns>
    public async Task<ApiLayerResponse<CurrencySymbolsResult>> GetCurrencySymbolsAsync(
        CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}/exchangerates_data/symbols";
        return await GetAsync<CurrencySymbolsResult>(url, cancellationToken);
    }

    /// <summary>
    /// Gets exchange rates over a time period.
    /// </summary>
    /// <param name="startDate">Start date (YYYY-MM-DD).</param>
    /// <param name="endDate">End date (YYYY-MM-DD).</param>
    /// <param name="baseCurrency">Base currency code.</param>
    /// <param name="symbols">Specific currencies to get.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Time series exchange rates.</returns>
    public async Task<ApiLayerResponse<TimeSeriesResult>> GetTimeSeriesAsync(
        string startDate,
        string endDate,
        string baseCurrency = "EUR",
        string[]? symbols = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(startDate))
            throw new ArgumentException("Start date is required", nameof(startDate));
        if (string.IsNullOrWhiteSpace(endDate))
            throw new ArgumentException("End date is required", nameof(endDate));

        var url = $"{_options.BaseUrl}/exchangerates_data/timeseries?start_date={startDate}&end_date={endDate}&base={baseCurrency}";
        if (symbols != null && symbols.Length > 0)
        {
            url += $"&symbols={string.Join(",", symbols)}";
        }
        return await GetAsync<TimeSeriesResult>(url, cancellationToken);
    }

    /// <summary>
    /// Gets rate fluctuation data between two dates.
    /// </summary>
    /// <param name="startDate">Start date (YYYY-MM-DD).</param>
    /// <param name="endDate">End date (YYYY-MM-DD).</param>
    /// <param name="baseCurrency">Base currency code.</param>
    /// <param name="symbols">Specific currencies to get.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Fluctuation data.</returns>
    public async Task<ApiLayerResponse<FluctuationResult>> GetFluctuationAsync(
        string startDate,
        string endDate,
        string baseCurrency = "EUR",
        string[]? symbols = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(startDate))
            throw new ArgumentException("Start date is required", nameof(startDate));
        if (string.IsNullOrWhiteSpace(endDate))
            throw new ArgumentException("End date is required", nameof(endDate));

        var url = $"{_options.BaseUrl}/exchangerates_data/fluctuation?start_date={startDate}&end_date={endDate}&base={baseCurrency}";
        if (symbols != null && symbols.Length > 0)
        {
            url += $"&symbols={string.Join(",", symbols)}";
        }
        return await GetAsync<FluctuationResult>(url, cancellationToken);
    }

    #endregion

    #region Phone Validation (number_verification)

    /// <summary>
    /// Validates a phone number.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate.</param>
    /// <param name="countryCode">Optional country code hint (ISO 3166-1 alpha-2).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Phone validation result.</returns>
    public async Task<ApiLayerResponse<PhoneValidationResult>> ValidatePhoneAsync(
        string phoneNumber,
        string? countryCode = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required", nameof(phoneNumber));

        var url = $"{_options.BaseUrl}/number_verification/validate?number={Uri.EscapeDataString(phoneNumber)}";
        if (!string.IsNullOrEmpty(countryCode))
        {
            url += $"&country_code={countryCode}";
        }
        return await GetAsync<PhoneValidationResult>(url, cancellationToken);
    }

    #endregion

    #region Email Validation (email_verification)

    /// <summary>
    /// Validates an email address.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <param name="smtpCheck">Whether to perform SMTP check (default: true).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Email validation result.</returns>
    public async Task<ApiLayerResponse<EmailValidationResult>> ValidateEmailAsync(
        string email,
        bool smtpCheck = true,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        var url = $"{_options.BaseUrl}/email_verification/check?email={Uri.EscapeDataString(email)}&smtp_check={smtpCheck.ToString().ToLower()}";
        return await GetAsync<EmailValidationResult>(url, cancellationToken);
    }

    #endregion

    #region Helpers

    private async Task<ApiLayerResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(
#if NET5_0_OR_GREATER
                cancellationToken
#endif
            );

            if (!response.IsSuccessStatusCode)
            {
                // Try to parse error response
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<RawApiResponse>(content, _jsonOptions);
                    if (errorResponse?.Error != null)
                    {
                        return ApiLayerResponse<T>.Fail(
                            errorResponse.Error.Message ?? "Request failed",
                            errorResponse.Error.Code);
                    }
                }
                catch
                {
                    // Ignore JSON parsing errors for error response
                }

                return ApiLayerResponse<T>.Fail($"Request failed with status {response.StatusCode}", (int)response.StatusCode);
            }

            var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            if (result == null)
            {
                return ApiLayerResponse<T>.Fail("Failed to parse response");
            }

            return ApiLayerResponse<T>.Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return ApiLayerResponse<T>.Fail($"HTTP error: {ex.Message}");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            return ApiLayerResponse<T>.Fail("Request timed out");
        }
        catch (JsonException ex)
        {
            return ApiLayerResponse<T>.Fail($"JSON parsing error: {ex.Message}");
        }
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
/// Exception thrown when an APILayer request fails.
/// </summary>
public class ApiLayerException : Exception
{
    /// <summary>
    /// Error code from APILayer.
    /// </summary>
    public int ErrorCode { get; }

    /// <summary>
    /// Creates a new ApiLayerException.
    /// </summary>
    public ApiLayerException(string message, int errorCode = 0) : base(message)
    {
        ErrorCode = errorCode;
    }
}
