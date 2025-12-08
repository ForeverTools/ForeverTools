using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ForeverTools.Captcha.Providers;

/// <summary>
/// 2Captcha.com captcha solving provider.
/// Get your API key at: https://2captcha.com
/// </summary>
public class TwoCaptchaProvider : ICaptchaSolver
{
    private const string BaseUrl = "https://api.2captcha.com";
    private const int SoftId = 2482; // ForeverTools affiliate ID

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly CaptchaOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <inheritdoc />
    public CaptchaProvider Provider => CaptchaProvider.TwoCaptcha;

    /// <summary>
    /// Creates a new 2Captcha provider instance.
    /// </summary>
    /// <param name="apiKey">Your 2Captcha API key.</param>
    /// <param name="options">Optional configuration options.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    public TwoCaptchaProvider(string apiKey, CaptchaOptions? options = null, HttpClient? httpClient = null)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key is required. Get one at https://2captcha.com", nameof(apiKey));

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
            var taskId = await SubmitTaskAsync(task, cancellationToken);
            if (string.IsNullOrEmpty(taskId))
                return CaptchaResult.Failed("Failed to submit captcha task", provider: Provider);

            // Poll for result
            var timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
            var pollInterval = TimeSpan.FromMilliseconds(_options.PollingIntervalMs);

            while (DateTime.UtcNow - startTime < timeout)
            {
                await Task.Delay(pollInterval, cancellationToken);

                var result = await GetResultAsync(taskId, cancellationToken);
                if (result.Success || result.ErrorCode == "ERROR_CAPTCHA_UNSOLVABLE")
                {
                    result.SolveTime = DateTime.UtcNow - startTime;
                    return result;
                }

                if (!string.IsNullOrEmpty(result.ErrorCode) && result.ErrorCode != "CAPCHA_NOT_READY")
                    return result;
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
        var url = $"{BaseUrl}/getBalance?key={_apiKey}&json=1";
        var response = await _httpClient.GetFromJsonAsync<TwoCaptchaResponse>(url, cancellationToken);

        if (response?.Status == 1 && decimal.TryParse(response.Request, out var balance))
        {
            return new CaptchaBalance { Provider = Provider, Balance = balance };
        }

        throw new InvalidOperationException($"Failed to get balance: {response?.Request ?? "Unknown error"}");
    }

    /// <inheritdoc />
    public async Task<bool> ReportIncorrectAsync(string taskId, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/reportbad?key={_apiKey}&id={taskId}&json=1";
        var response = await _httpClient.GetFromJsonAsync<TwoCaptchaResponse>(url, cancellationToken);
        return response?.Status == 1;
    }

    private async Task<string?> SubmitTaskAsync(CaptchaTask task, CancellationToken cancellationToken)
    {
        var request = BuildRequest(task);
        var content = new FormUrlEncodedContent(request);

        var response = await _httpClient.PostAsync($"{BaseUrl}/in.php", content, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<TwoCaptchaResponse>(cancellationToken);

        if (result?.Status == 1)
            return result.Request;

        return null;
    }

    private async Task<CaptchaResult> GetResultAsync(string taskId, CancellationToken cancellationToken)
    {
        var url = $"{BaseUrl}/res.php?key={_apiKey}&action=get&id={taskId}&json=1";
        var response = await _httpClient.GetFromJsonAsync<TwoCaptchaResponse>(url, cancellationToken);

        if (response == null)
            return CaptchaResult.Failed("No response from server", provider: Provider);

        if (response.Status == 1)
            return CaptchaResult.Solved(response.Request ?? "", taskId, Provider);

        if (response.Request == "CAPCHA_NOT_READY")
            return new CaptchaResult { Success = false, TaskId = taskId, Provider = Provider };

        return CaptchaResult.Failed(response.Request ?? "Unknown error", response.Request, Provider);
    }

    private Dictionary<string, string> BuildRequest(CaptchaTask task)
    {
        var request = new Dictionary<string, string>
        {
            ["key"] = _apiKey,
            ["soft_id"] = SoftId.ToString(),
            ["json"] = "1"
        };

        switch (task)
        {
            case ImageCaptchaTask image:
                request["method"] = "base64";
                request["body"] = image.ImageBase64;
                if (image.CaseSensitive) request["regsense"] = "1";
                if (image.MinLength.HasValue) request["min_len"] = image.MinLength.Value.ToString();
                if (image.MaxLength.HasValue) request["max_len"] = image.MaxLength.Value.ToString();
                break;

            case ReCaptchaV2Task recaptcha:
                request["method"] = "userrecaptcha";
                request["googlekey"] = recaptcha.SiteKey;
                request["pageurl"] = recaptcha.WebsiteUrl;
                if (recaptcha.IsInvisible) request["invisible"] = "1";
                if (!string.IsNullOrEmpty(recaptcha.DataS)) request["data-s"] = recaptcha.DataS;
                break;

            case ReCaptchaV3Task recaptcha3:
                request["method"] = "userrecaptcha";
                request["googlekey"] = recaptcha3.SiteKey;
                request["pageurl"] = recaptcha3.WebsiteUrl;
                request["version"] = "v3";
                request["action"] = recaptcha3.Action;
                request["min_score"] = recaptcha3.MinScore.ToString("F1");
                break;

            case HCaptchaTask hcaptcha:
                request["method"] = "hcaptcha";
                request["sitekey"] = hcaptcha.SiteKey;
                request["pageurl"] = hcaptcha.WebsiteUrl;
                break;

            case TurnstileTask turnstile:
                request["method"] = "turnstile";
                request["sitekey"] = turnstile.SiteKey;
                request["pageurl"] = turnstile.WebsiteUrl;
                if (!string.IsNullOrEmpty(turnstile.Action)) request["action"] = turnstile.Action;
                if (!string.IsNullOrEmpty(turnstile.CData)) request["data"] = turnstile.CData;
                break;

            case FunCaptchaTask funcaptcha:
                request["method"] = "funcaptcha";
                request["publickey"] = funcaptcha.PublicKey;
                request["pageurl"] = funcaptcha.WebsiteUrl;
                if (!string.IsNullOrEmpty(funcaptcha.ServiceUrl)) request["surl"] = funcaptcha.ServiceUrl;
                if (!string.IsNullOrEmpty(funcaptcha.Data)) request["data[blob]"] = funcaptcha.Data;
                break;

            default:
                throw new NotSupportedException($"Captcha type {task.Type} is not supported by 2Captcha");
        }

        return request;
    }

    private class TwoCaptchaResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("request")]
        public string? Request { get; set; }
    }
}
