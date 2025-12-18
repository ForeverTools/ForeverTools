using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ForeverTools.Captcha.Providers;

/// <summary>
/// Anti-Captcha.com captcha solving provider.
/// Get your API key at: https://anti-captcha.com
/// </summary>
public class AntiCaptchaProvider : ICaptchaSolver
{
    private const string BaseUrl = "https://api.anti-captcha.com";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly CaptchaOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <inheritdoc />
    public CaptchaProvider Provider => CaptchaProvider.AntiCaptcha;

    /// <summary>
    /// Creates a new Anti-Captcha provider instance.
    /// </summary>
    /// <param name="apiKey">Your Anti-Captcha API key.</param>
    /// <param name="options">Optional configuration options.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    public AntiCaptchaProvider(string apiKey, CaptchaOptions? options = null, HttpClient? httpClient = null)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key is required. Get one at https://anti-captcha.com", nameof(apiKey));

        _apiKey = apiKey;
        _options = options ?? new CaptchaOptions();
        _httpClient = httpClient ?? new HttpClient();
    }

    /// <inheritdoc />
    public async Task<CaptchaResult> SolveAsync(CaptchaTask task, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;

        try
        {
            // Submit the task
            var createResponse = await CreateTaskAsync(task, cancellationToken);
            if (createResponse == null)
                return CaptchaResult.Failed("Failed to submit captcha task", provider: Provider);

            if (createResponse.ErrorId != 0)
                return CaptchaResult.Failed(createResponse.ErrorDescription ?? "Unknown error", createResponse.ErrorCode, Provider);

            if (!createResponse.TaskId.HasValue)
                return CaptchaResult.Failed("No task ID returned", provider: Provider);

            var taskId = createResponse.TaskId.Value.ToString();

            // Poll for result
            var timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
            var pollInterval = TimeSpan.FromMilliseconds(_options.PollingIntervalMs);

            while (DateTime.UtcNow - startTime < timeout)
            {
                await Task.Delay(pollInterval, cancellationToken);

                var result = await GetResultAsync(taskId, cancellationToken);
                if (result.Success || !string.IsNullOrEmpty(result.ErrorCode))
                {
                    result.SolveTime = DateTime.UtcNow - startTime;
                    return result;
                }
            }

            return CaptchaResult.Failed("Timeout waiting for captcha solution", "TIMEOUT", Provider);
        }
        catch (Exception ex)
        {
            return CaptchaResult.Failed(ex.Message, "EXCEPTION", Provider);
        }
    }

    /// <inheritdoc />
    public async Task<CaptchaBalance> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var request = new { clientKey = _apiKey };
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/getBalance", request, JsonOptions, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<AntiCaptchaResponse>(JsonOptions, cancellationToken);

        if (result?.ErrorId == 0 && result.Balance.HasValue)
        {
            return new CaptchaBalance { Provider = Provider, Balance = result.Balance.Value };
        }

        throw new InvalidOperationException($"Failed to get balance: {result?.ErrorDescription ?? "Unknown error"}");
    }

    /// <inheritdoc />
    public async Task<bool> ReportIncorrectAsync(string taskId, CancellationToken cancellationToken = default)
    {
        var request = new { clientKey = _apiKey, taskId = long.Parse(taskId) };
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/reportIncorrectImageCaptcha", request, JsonOptions, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<AntiCaptchaResponse>(JsonOptions, cancellationToken);
        return result?.ErrorId == 0;
    }

    private async Task<AntiCaptchaResponse?> CreateTaskAsync(CaptchaTask task, CancellationToken cancellationToken)
    {
        var request = new Dictionary<string, object>
        {
            ["clientKey"] = _apiKey,
            ["task"] = BuildTaskObject(task)
        };

        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/createTask", request, JsonOptions, cancellationToken);
        return await response.Content.ReadFromJsonAsync<AntiCaptchaResponse>(JsonOptions, cancellationToken);
    }

    private async Task<CaptchaResult> GetResultAsync(string taskId, CancellationToken cancellationToken)
    {
        var request = new { clientKey = _apiKey, taskId = long.Parse(taskId) };
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/getTaskResult", request, JsonOptions, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<AntiCaptchaResponse>(JsonOptions, cancellationToken);

        if (result == null)
            return CaptchaResult.Failed("No response from server", provider: Provider);

        if (result.ErrorId != 0)
            return CaptchaResult.Failed(result.ErrorDescription ?? "Unknown error", result.ErrorCode, Provider);

        if (result.Status == "ready" && result.Solution.HasValue)
        {
            var solution = ExtractSolution(result.Solution.Value);
            var captchaResult = CaptchaResult.Solved(solution, taskId, Provider);
            if (result.Cost != null && decimal.TryParse(result.Cost, out var cost))
                captchaResult.Cost = cost;
            return captchaResult;
        }

        // Still processing
        return new CaptchaResult { Success = false, TaskId = taskId, Provider = Provider };
    }

    private Dictionary<string, object> BuildTaskObject(CaptchaTask task)
    {
        var taskObj = new Dictionary<string, object>();

        switch (task)
        {
            case ImageCaptchaTask image:
                taskObj["type"] = "ImageToTextTask";
                taskObj["body"] = image.ImageBase64;
                if (image.CaseSensitive) taskObj["case"] = true;
                if (image.MinLength.HasValue) taskObj["minLength"] = image.MinLength.Value;
                if (image.MaxLength.HasValue) taskObj["maxLength"] = image.MaxLength.Value;
                break;

            case ReCaptchaV2Task recaptcha:
                taskObj["type"] = "RecaptchaV2TaskProxyless";
                taskObj["websiteURL"] = recaptcha.WebsiteUrl;
                taskObj["websiteKey"] = recaptcha.SiteKey;
                if (recaptcha.IsInvisible) taskObj["isInvisible"] = true;
                break;

            case ReCaptchaV3Task recaptcha3:
                taskObj["type"] = "RecaptchaV3TaskProxyless";
                taskObj["websiteURL"] = recaptcha3.WebsiteUrl;
                taskObj["websiteKey"] = recaptcha3.SiteKey;
                taskObj["pageAction"] = recaptcha3.Action;
                taskObj["minScore"] = recaptcha3.MinScore;
                break;

            case HCaptchaTask hcaptcha:
                taskObj["type"] = "HCaptchaTaskProxyless";
                taskObj["websiteURL"] = hcaptcha.WebsiteUrl;
                taskObj["websiteKey"] = hcaptcha.SiteKey;
                break;

            case TurnstileTask turnstile:
                taskObj["type"] = "TurnstileTaskProxyless";
                taskObj["websiteURL"] = turnstile.WebsiteUrl;
                taskObj["websiteKey"] = turnstile.SiteKey;
                break;

            case FunCaptchaTask funcaptcha:
                taskObj["type"] = "FunCaptchaTaskProxyless";
                taskObj["websiteURL"] = funcaptcha.WebsiteUrl;
                taskObj["websitePublicKey"] = funcaptcha.PublicKey;
                if (!string.IsNullOrEmpty(funcaptcha.ServiceUrl)) taskObj["funcaptchaApiJSSubdomain"] = funcaptcha.ServiceUrl;
                break;

            default:
                throw new NotSupportedException($"Captcha type {task.Type} is not supported by Anti-Captcha");
        }

        return taskObj;
    }

    private static string ExtractSolution(JsonElement solution)
    {
        if (solution.TryGetProperty("gRecaptchaResponse", out var gResponse))
            return gResponse.GetString() ?? "";
        if (solution.TryGetProperty("token", out var token))
            return token.GetString() ?? "";
        if (solution.TryGetProperty("text", out var text))
            return text.GetString() ?? "";

        return solution.ToString();
    }

    private class AntiCaptchaResponse
    {
        [JsonPropertyName("errorId")]
        public int ErrorId { get; set; }

        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }

        [JsonPropertyName("errorDescription")]
        public string? ErrorDescription { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("taskId")]
        public long? TaskId { get; set; }

        [JsonPropertyName("solution")]
        public JsonElement? Solution { get; set; }

        [JsonPropertyName("balance")]
        public decimal? Balance { get; set; }

        [JsonPropertyName("cost")]
        public string? Cost { get; set; }
    }
}
