namespace ForeverTools.EmailAI.Models;

/// <summary>
/// Result of an email classification request.
/// </summary>
public class EmailClassifyResult
{
    /// <summary>
    /// The email category. One of: urgent, normal, spam, newsletter, support, sales, other.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Priority level: high, medium, or low.
    /// </summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>
    /// Sentiment of the email: positive, neutral, or negative.
    /// </summary>
    public string Sentiment { get; set; } = string.Empty;

    /// <summary>
    /// A brief rationale explaining the classification decision.
    /// </summary>
    public string Rationale { get; set; } = string.Empty;
}
