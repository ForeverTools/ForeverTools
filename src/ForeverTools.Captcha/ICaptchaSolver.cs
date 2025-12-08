namespace ForeverTools.Captcha;

/// <summary>
/// Interface for captcha solving providers.
/// </summary>
public interface ICaptchaSolver
{
    /// <summary>
    /// The provider type.
    /// </summary>
    CaptchaProvider Provider { get; }

    /// <summary>
    /// Solves a captcha and returns the solution.
    /// </summary>
    /// <param name="task">The captcha task to solve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The captcha solution.</returns>
    Task<CaptchaResult> SolveAsync(CaptchaTask task, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current account balance.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account balance.</returns>
    Task<CaptchaBalance> GetBalanceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reports an incorrect captcha solution for refund.
    /// </summary>
    /// <param name="taskId">The task ID to report.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether the report was successful.</returns>
    Task<bool> ReportIncorrectAsync(string taskId, CancellationToken cancellationToken = default);
}
