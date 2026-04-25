namespace ForeverTools.EmailAI.Models;

/// <summary>
/// Result of a batch email reply generation request.
/// </summary>
public class BatchEmailReplyResult
{
    /// <summary>
    /// Individual reply results, in the same order as the input requests.
    /// </summary>
    public List<EmailReplyResult> Results { get; set; } = new();

    /// <summary>
    /// Total number of replies generated.
    /// </summary>
    public int Total => Results.Count;
}
