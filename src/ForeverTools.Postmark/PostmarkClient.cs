using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ForeverTools.Postmark.Models;

namespace ForeverTools.Postmark;

/// <summary>
/// Client for sending emails via Postmark.
/// Get your API key at: https://www.postmarkapp.com?via=8ac781
/// </summary>
public class PostmarkClient
{
    private readonly HttpClient _httpClient;
    private readonly PostmarkOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = null, // Postmark uses PascalCase
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Gets the configured options.
    /// </summary>
    public PostmarkOptions Options => _options;

    /// <summary>
    /// Creates a new Postmark client with the specified server token.
    /// Get your token at: https://www.postmarkapp.com?via=8ac781
    /// </summary>
    /// <param name="serverToken">Your Postmark Server API Token.</param>
    public PostmarkClient(string serverToken) : this(new PostmarkOptions { ServerToken = serverToken })
    {
    }

    /// <summary>
    /// Creates a new Postmark client with the specified options.
    /// </summary>
    /// <param name="options">Configuration options.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    public PostmarkClient(PostmarkOptions options, HttpClient? httpClient = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        if (!options.HasServerToken)
            throw new ArgumentException("Server token is required. Get one at https://www.postmarkapp.com?via=8ac781", nameof(options));

        _httpClient = httpClient ?? new HttpClient();
        _httpClient.BaseAddress = new Uri(options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("X-Postmark-Server-Token", options.ServerToken);
        _httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
    }

    #region Send Email

    /// <summary>
    /// Sends a single email.
    /// </summary>
    /// <param name="email">The email to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The send result.</returns>
    public async Task<PostmarkResponse> SendEmailAsync(PostmarkEmail email, CancellationToken cancellationToken = default)
    {
        ApplyDefaults(email);

        var response = await _httpClient.PostAsJsonAsync("/email", email, JsonOptions, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<PostmarkResponse>(JsonOptions, cancellationToken);

        return result ?? new PostmarkResponse { ErrorCode = -1, Message = "No response from server" };
    }

    /// <summary>
    /// Sends a single email with simplified parameters.
    /// </summary>
    /// <param name="to">Recipient email address.</param>
    /// <param name="subject">Email subject.</param>
    /// <param name="htmlBody">HTML body content.</param>
    /// <param name="textBody">Plain text body content.</param>
    /// <param name="from">Sender email (uses default if not specified).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<PostmarkResponse> SendEmailAsync(
        string to,
        string subject,
        string? htmlBody = null,
        string? textBody = null,
        string? from = null,
        CancellationToken cancellationToken = default)
    {
        var email = new PostmarkEmail
        {
            To = to,
            Subject = subject,
            HtmlBody = htmlBody,
            TextBody = textBody,
            From = from ?? _options.DefaultFrom ?? throw new InvalidOperationException("From address is required")
        };

        return SendEmailAsync(email, cancellationToken);
    }

    /// <summary>
    /// Sends multiple emails in a single batch (up to 500).
    /// </summary>
    /// <param name="emails">The emails to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Results for each email.</returns>
    public async Task<IReadOnlyList<PostmarkResponse>> SendBatchAsync(IEnumerable<PostmarkEmail> emails, CancellationToken cancellationToken = default)
    {
        var emailList = emails.ToList();
        if (emailList.Count > 500)
            throw new ArgumentException("Batch size cannot exceed 500 emails", nameof(emails));

        foreach (var email in emailList)
        {
            ApplyDefaults(email);
        }

        var response = await _httpClient.PostAsJsonAsync("/email/batch", emailList, JsonOptions, cancellationToken);
        var results = await response.Content.ReadFromJsonAsync<List<PostmarkResponse>>(JsonOptions, cancellationToken);

        return results ?? new List<PostmarkResponse>();
    }

    #endregion

    #region Template Email

    /// <summary>
    /// Sends an email using a template.
    /// </summary>
    /// <param name="email">The template email to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<PostmarkResponse> SendTemplateEmailAsync(PostmarkTemplateEmail email, CancellationToken cancellationToken = default)
    {
        ApplyDefaults(email);

        var response = await _httpClient.PostAsJsonAsync("/email/withTemplate", email, JsonOptions, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<PostmarkResponse>(JsonOptions, cancellationToken);

        return result ?? new PostmarkResponse { ErrorCode = -1, Message = "No response from server" };
    }

    /// <summary>
    /// Sends an email using a template with simplified parameters.
    /// </summary>
    /// <param name="templateIdOrAlias">Template ID or alias.</param>
    /// <param name="to">Recipient email address.</param>
    /// <param name="templateModel">Variables to substitute in the template.</param>
    /// <param name="from">Sender email (uses default if not specified).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<PostmarkResponse> SendTemplateEmailAsync(
        string templateIdOrAlias,
        string to,
        Dictionary<string, object> templateModel,
        string? from = null,
        CancellationToken cancellationToken = default)
    {
        var email = new PostmarkTemplateEmail
        {
            To = to,
            From = from ?? _options.DefaultFrom ?? throw new InvalidOperationException("From address is required"),
            TemplateModel = templateModel
        };

        if (long.TryParse(templateIdOrAlias, out var templateId))
            email.TemplateId = templateId;
        else
            email.TemplateAlias = templateIdOrAlias;

        return SendTemplateEmailAsync(email, cancellationToken);
    }

    /// <summary>
    /// Sends multiple template emails in a single batch (up to 500).
    /// </summary>
    /// <param name="emails">The template emails to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IReadOnlyList<PostmarkResponse>> SendTemplateBatchAsync(IEnumerable<PostmarkTemplateEmail> emails, CancellationToken cancellationToken = default)
    {
        var emailList = emails.ToList();
        if (emailList.Count > 500)
            throw new ArgumentException("Batch size cannot exceed 500 emails", nameof(emails));

        foreach (var email in emailList)
        {
            ApplyDefaults(email);
        }

        var request = new { Messages = emailList };
        var response = await _httpClient.PostAsJsonAsync("/email/batchWithTemplates", request, JsonOptions, cancellationToken);
        var results = await response.Content.ReadFromJsonAsync<List<PostmarkResponse>>(JsonOptions, cancellationToken);

        return results ?? new List<PostmarkResponse>();
    }

    #endregion

    #region Statistics

    /// <summary>
    /// Gets delivery statistics overview.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<PostmarkDeliveryStats> GetDeliveryStatsAsync(CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<PostmarkDeliveryStats>("/deliverystats", JsonOptions, cancellationToken);
        return result ?? new PostmarkDeliveryStats();
    }

    /// <summary>
    /// Gets outbound message statistics.
    /// </summary>
    /// <param name="tag">Filter by tag.</param>
    /// <param name="fromDate">Start date.</param>
    /// <param name="toDate">End date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<PostmarkOutboundStats> GetOutboundStatsAsync(
        string? tag = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQueryString(new Dictionary<string, string?>
        {
            ["tag"] = tag,
            ["fromdate"] = fromDate?.ToString("yyyy-MM-dd"),
            ["todate"] = toDate?.ToString("yyyy-MM-dd")
        });

        var result = await _httpClient.GetFromJsonAsync<PostmarkOutboundStats>($"/stats/outbound{query}", JsonOptions, cancellationToken);
        return result ?? new PostmarkOutboundStats();
    }

    #endregion

    #region Bounces

    /// <summary>
    /// Gets a list of bounces.
    /// </summary>
    /// <param name="count">Number of bounces to return (max 500).</param>
    /// <param name="offset">Pagination offset.</param>
    /// <param name="type">Filter by bounce type.</param>
    /// <param name="inactive">Filter by inactive status.</param>
    /// <param name="emailFilter">Filter by email address.</param>
    /// <param name="tag">Filter by tag.</param>
    /// <param name="messageId">Filter by message ID.</param>
    /// <param name="fromDate">Start date.</param>
    /// <param name="toDate">End date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<PostmarkBouncesResponse> GetBouncesAsync(
        int count = 100,
        int offset = 0,
        string? type = null,
        bool? inactive = null,
        string? emailFilter = null,
        string? tag = null,
        string? messageId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQueryString(new Dictionary<string, string?>
        {
            ["count"] = count.ToString(),
            ["offset"] = offset.ToString(),
            ["type"] = type,
            ["inactive"] = inactive?.ToString().ToLower(),
            ["emailFilter"] = emailFilter,
            ["tag"] = tag,
            ["messageID"] = messageId,
            ["fromdate"] = fromDate?.ToString("yyyy-MM-dd"),
            ["todate"] = toDate?.ToString("yyyy-MM-dd")
        });

        var result = await _httpClient.GetFromJsonAsync<PostmarkBouncesResponse>($"/bounces{query}", JsonOptions, cancellationToken);
        return result ?? new PostmarkBouncesResponse();
    }

    /// <summary>
    /// Gets a single bounce by ID.
    /// </summary>
    /// <param name="bounceId">The bounce ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<PostmarkBounce?> GetBounceAsync(long bounceId, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<PostmarkBounce>($"/bounces/{bounceId}", JsonOptions, cancellationToken);
    }

    /// <summary>
    /// Activates a bounced email address (removes from suppression).
    /// </summary>
    /// <param name="bounceId">The bounce ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<PostmarkBounceActivation> ActivateBounceAsync(long bounceId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsync($"/bounces/{bounceId}/activate", null, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<PostmarkBounceActivation>(JsonOptions, cancellationToken);
        return result ?? new PostmarkBounceActivation();
    }

    #endregion

    #region Server

    /// <summary>
    /// Gets the current server information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<PostmarkServer?> GetServerAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<PostmarkServer>("/server", JsonOptions, cancellationToken);
    }

    #endregion

    #region Static Factory Methods

    /// <summary>
    /// Creates a client from an environment variable.
    /// </summary>
    /// <param name="envVarName">Environment variable name.</param>
    public static PostmarkClient FromEnvironment(string envVarName = "POSTMARK_SERVER_TOKEN")
    {
        var token = Environment.GetEnvironmentVariable(envVarName);
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException($"Environment variable '{envVarName}' not set. Get your token at https://www.postmarkapp.com?via=8ac781");

        return new PostmarkClient(token);
    }

    #endregion

    #region Private Helpers

    private void ApplyDefaults(PostmarkEmail email)
    {
        if (string.IsNullOrEmpty(email.From) && !string.IsNullOrEmpty(_options.DefaultFrom))
            email.From = _options.DefaultFrom;

        if (string.IsNullOrEmpty(email.ReplyTo) && !string.IsNullOrEmpty(_options.DefaultReplyTo))
            email.ReplyTo = _options.DefaultReplyTo;

        if (string.IsNullOrEmpty(email.MessageStream) && !string.IsNullOrEmpty(_options.DefaultMessageStream))
            email.MessageStream = _options.DefaultMessageStream;

        if (!email.TrackOpens.HasValue && _options.TrackOpens.HasValue)
            email.TrackOpens = _options.TrackOpens;

        if (string.IsNullOrEmpty(email.TrackLinks) && !string.IsNullOrEmpty(_options.TrackLinks))
            email.TrackLinks = _options.TrackLinks;
    }

    private void ApplyDefaults(PostmarkTemplateEmail email)
    {
        if (string.IsNullOrEmpty(email.From) && !string.IsNullOrEmpty(_options.DefaultFrom))
            email.From = _options.DefaultFrom;

        if (string.IsNullOrEmpty(email.ReplyTo) && !string.IsNullOrEmpty(_options.DefaultReplyTo))
            email.ReplyTo = _options.DefaultReplyTo;

        if (string.IsNullOrEmpty(email.MessageStream) && !string.IsNullOrEmpty(_options.DefaultMessageStream))
            email.MessageStream = _options.DefaultMessageStream;

        if (!email.TrackOpens.HasValue && _options.TrackOpens.HasValue)
            email.TrackOpens = _options.TrackOpens;

        if (string.IsNullOrEmpty(email.TrackLinks) && !string.IsNullOrEmpty(_options.TrackLinks))
            email.TrackLinks = _options.TrackLinks;
    }

    private static string BuildQueryString(Dictionary<string, string?> parameters)
    {
        var validParams = parameters.Where(p => !string.IsNullOrEmpty(p.Value)).ToList();
        if (validParams.Count == 0)
            return string.Empty;

        return "?" + string.Join("&", validParams.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value!)}"));
    }

    #endregion
}
