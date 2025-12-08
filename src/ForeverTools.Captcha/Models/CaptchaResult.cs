namespace ForeverTools.Captcha;

/// <summary>
/// Result of a captcha solving operation.
/// </summary>
public class CaptchaResult
{
    /// <summary>
    /// Whether the captcha was solved successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The solution token or text.
    /// </summary>
    public string Solution { get; set; } = string.Empty;

    /// <summary>
    /// The task ID from the provider.
    /// </summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// Error message if the solve failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Error code from the provider.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Cost of solving this captcha (in USD).
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Time taken to solve the captcha.
    /// </summary>
    public TimeSpan? SolveTime { get; set; }

    /// <summary>
    /// The provider that solved this captcha.
    /// </summary>
    public CaptchaProvider Provider { get; set; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static CaptchaResult Solved(string solution, string taskId, CaptchaProvider provider) =>
        new() { Success = true, Solution = solution, TaskId = taskId, Provider = provider };

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static CaptchaResult Failed(string errorMessage, string? errorCode = null, CaptchaProvider provider = default) =>
        new() { Success = false, ErrorMessage = errorMessage, ErrorCode = errorCode, Provider = provider };
}

/// <summary>
/// Balance information for a captcha provider account.
/// </summary>
public class CaptchaBalance
{
    /// <summary>
    /// The provider this balance is for.
    /// </summary>
    public CaptchaProvider Provider { get; set; }

    /// <summary>
    /// Current balance in USD.
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Currency of the balance (usually USD).
    /// </summary>
    public string Currency { get; set; } = "USD";
}
