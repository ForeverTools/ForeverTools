using System.Text.Json.Serialization;

namespace ForeverTools.ScraperAPI.Models;

/// <summary>
/// Response from a scrape request.
/// </summary>
public class ScrapeResponse
{
    /// <summary>
    /// Whether the scrape was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The scraped content (HTML, JSON, text, etc.).
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Screenshot data as base64 (when screenshot=true).
    /// </summary>
    public string? ScreenshotBase64 { get; set; }

    /// <summary>
    /// HTTP status code from the target website.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Error message if the request failed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// The original URL that was scraped.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Response headers from the target website.
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }
}

/// <summary>
/// Response from an async scrape job submission.
/// </summary>
public class AsyncJobResponse
{
    /// <summary>
    /// The job ID for checking status.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Current status of the job.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// URL to check job status.
    /// </summary>
    [JsonPropertyName("statusUrl")]
    public string? StatusUrl { get; set; }

    /// <summary>
    /// Whether the job was submitted successfully.
    /// </summary>
    public bool Success => !string.IsNullOrEmpty(Id);
}

/// <summary>
/// Status of an async scrape job.
/// </summary>
public class AsyncJobStatus
{
    /// <summary>
    /// The job ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Current status: "running", "finished", or "failed".
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// The scraped response body (when finished).
    /// </summary>
    [JsonPropertyName("response")]
    public AsyncJobResponseBody? Response { get; set; }

    /// <summary>
    /// Whether the job is still running.
    /// </summary>
    public bool IsRunning => Status == "running";

    /// <summary>
    /// Whether the job completed successfully.
    /// </summary>
    public bool IsFinished => Status == "finished";

    /// <summary>
    /// Whether the job failed.
    /// </summary>
    public bool IsFailed => Status == "failed";
}

/// <summary>
/// Response body from a completed async job.
/// </summary>
public class AsyncJobResponseBody
{
    /// <summary>
    /// HTTP status code from the target.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    /// <summary>
    /// The scraped content.
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; set; }

    /// <summary>
    /// Response headers.
    /// </summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; set; }
}

/// <summary>
/// Account information and credits.
/// </summary>
public class AccountInfo
{
    /// <summary>
    /// Number of API credits remaining.
    /// </summary>
    [JsonPropertyName("requestCount")]
    public int RequestCount { get; set; }

    /// <summary>
    /// Maximum requests allowed in the billing period.
    /// </summary>
    [JsonPropertyName("requestLimit")]
    public int RequestLimit { get; set; }

    /// <summary>
    /// Number of concurrent requests allowed.
    /// </summary>
    [JsonPropertyName("concurrencyLimit")]
    public int ConcurrencyLimit { get; set; }

    /// <summary>
    /// Remaining credits (requestLimit - requestCount).
    /// </summary>
    public int RemainingCredits => RequestLimit - RequestCount;
}
