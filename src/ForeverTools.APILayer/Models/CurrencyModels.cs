using System.Text.Json.Serialization;

namespace ForeverTools.APILayer.Models;

/// <summary>
/// Exchange rates response.
/// </summary>
public class ExchangeRatesResult
{
    /// <summary>
    /// Whether the request was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Unix timestamp of the rates.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// Base currency code.
    /// </summary>
    [JsonPropertyName("base")]
    public string? Base { get; set; }

    /// <summary>
    /// Date of the rates (YYYY-MM-DD).
    /// </summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>
    /// Exchange rates keyed by currency code.
    /// </summary>
    [JsonPropertyName("rates")]
    public Dictionary<string, decimal>? Rates { get; set; }

    /// <summary>
    /// Gets the rate for a specific currency.
    /// </summary>
    public decimal? GetRate(string currencyCode)
    {
        if (Rates != null && Rates.TryGetValue(currencyCode.ToUpperInvariant(), out var rate))
        {
            return rate;
        }
        return null;
    }

    /// <summary>
    /// DateTime representation of the timestamp.
    /// </summary>
    public DateTime TimestampUtc => DateTimeOffset.FromUnixTimeSeconds(Timestamp).UtcDateTime;
}

/// <summary>
/// Currency conversion result.
/// </summary>
public class CurrencyConversionResult
{
    /// <summary>
    /// Whether the request was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Query parameters that were used.
    /// </summary>
    [JsonPropertyName("query")]
    public ConversionQuery? Query { get; set; }

    /// <summary>
    /// Information about the conversion.
    /// </summary>
    [JsonPropertyName("info")]
    public ConversionInfo? Info { get; set; }

    /// <summary>
    /// Historical date if specified.
    /// </summary>
    [JsonPropertyName("historical")]
    public bool Historical { get; set; }

    /// <summary>
    /// Date of the conversion rate.
    /// </summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>
    /// The converted result amount.
    /// </summary>
    [JsonPropertyName("result")]
    public decimal Result { get; set; }
}

/// <summary>
/// Conversion query parameters.
/// </summary>
public class ConversionQuery
{
    /// <summary>
    /// Source currency code.
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>
    /// Target currency code.
    /// </summary>
    [JsonPropertyName("to")]
    public string? To { get; set; }

    /// <summary>
    /// Amount to convert.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}

/// <summary>
/// Conversion rate information.
/// </summary>
public class ConversionInfo
{
    /// <summary>
    /// Unix timestamp of the rate.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// Exchange rate used for conversion.
    /// </summary>
    [JsonPropertyName("rate")]
    public decimal Rate { get; set; }

    /// <summary>
    /// DateTime representation of the timestamp.
    /// </summary>
    public DateTime TimestampUtc => DateTimeOffset.FromUnixTimeSeconds(Timestamp).UtcDateTime;
}

/// <summary>
/// Available currencies/symbols.
/// </summary>
public class CurrencySymbolsResult
{
    /// <summary>
    /// Whether the request was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Currency symbols keyed by code.
    /// </summary>
    [JsonPropertyName("symbols")]
    public Dictionary<string, string>? Symbols { get; set; }
}

/// <summary>
/// Time series exchange rates result.
/// </summary>
public class TimeSeriesResult
{
    /// <summary>
    /// Whether the request was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Whether this is a time series response.
    /// </summary>
    [JsonPropertyName("timeseries")]
    public bool TimeSeries { get; set; }

    /// <summary>
    /// Start date of the series.
    /// </summary>
    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    /// <summary>
    /// End date of the series.
    /// </summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    /// <summary>
    /// Base currency.
    /// </summary>
    [JsonPropertyName("base")]
    public string? Base { get; set; }

    /// <summary>
    /// Rates by date, then by currency.
    /// </summary>
    [JsonPropertyName("rates")]
    public Dictionary<string, Dictionary<string, decimal>>? Rates { get; set; }

    /// <summary>
    /// Gets rates for a specific date.
    /// </summary>
    public Dictionary<string, decimal>? GetRatesForDate(string date)
    {
        if (Rates != null && Rates.TryGetValue(date, out var dayRates))
        {
            return dayRates;
        }
        return null;
    }
}

/// <summary>
/// Fluctuation data result.
/// </summary>
public class FluctuationResult
{
    /// <summary>
    /// Whether the request was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Whether this is a fluctuation response.
    /// </summary>
    [JsonPropertyName("fluctuation")]
    public bool Fluctuation { get; set; }

    /// <summary>
    /// Start date of the period.
    /// </summary>
    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    /// <summary>
    /// End date of the period.
    /// </summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    /// <summary>
    /// Base currency.
    /// </summary>
    [JsonPropertyName("base")]
    public string? Base { get; set; }

    /// <summary>
    /// Fluctuation data by currency.
    /// </summary>
    [JsonPropertyName("rates")]
    public Dictionary<string, FluctuationData>? Rates { get; set; }
}

/// <summary>
/// Fluctuation data for a currency.
/// </summary>
public class FluctuationData
{
    /// <summary>
    /// Rate at the start of the period.
    /// </summary>
    [JsonPropertyName("start_rate")]
    public decimal StartRate { get; set; }

    /// <summary>
    /// Rate at the end of the period.
    /// </summary>
    [JsonPropertyName("end_rate")]
    public decimal EndRate { get; set; }

    /// <summary>
    /// Absolute change in rate.
    /// </summary>
    [JsonPropertyName("change")]
    public decimal Change { get; set; }

    /// <summary>
    /// Percentage change in rate.
    /// </summary>
    [JsonPropertyName("change_pct")]
    public decimal ChangePercentage { get; set; }
}

/// <summary>
/// Common currency codes.
/// </summary>
public static class CurrencyCodes
{
    /// <summary>US Dollar</summary>
    public const string USD = "USD";
    /// <summary>Euro</summary>
    public const string EUR = "EUR";
    /// <summary>British Pound</summary>
    public const string GBP = "GBP";
    /// <summary>Japanese Yen</summary>
    public const string JPY = "JPY";
    /// <summary>Swiss Franc</summary>
    public const string CHF = "CHF";
    /// <summary>Canadian Dollar</summary>
    public const string CAD = "CAD";
    /// <summary>Australian Dollar</summary>
    public const string AUD = "AUD";
    /// <summary>Chinese Yuan</summary>
    public const string CNY = "CNY";
    /// <summary>Indian Rupee</summary>
    public const string INR = "INR";
    /// <summary>Mexican Peso</summary>
    public const string MXN = "MXN";
    /// <summary>Brazilian Real</summary>
    public const string BRL = "BRL";
    /// <summary>South Korean Won</summary>
    public const string KRW = "KRW";
    /// <summary>Singapore Dollar</summary>
    public const string SGD = "SGD";
    /// <summary>Hong Kong Dollar</summary>
    public const string HKD = "HKD";
    /// <summary>New Zealand Dollar</summary>
    public const string NZD = "NZD";
    /// <summary>Bitcoin</summary>
    public const string BTC = "BTC";
    /// <summary>Gold (troy ounce)</summary>
    public const string XAU = "XAU";
    /// <summary>Silver (troy ounce)</summary>
    public const string XAG = "XAG";
}
