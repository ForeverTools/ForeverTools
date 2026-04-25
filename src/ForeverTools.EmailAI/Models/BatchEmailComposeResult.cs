namespace ForeverTools.EmailAI.Models;

/// <summary>
/// Result of a batch email composition request.
/// </summary>
public class BatchEmailComposeResult
{
    /// <summary>
    /// Individual composition results, in the same order as the input requests.
    /// </summary>
    public List<EmailComposeResult> Results { get; set; } = new();

    /// <summary>
    /// Total number of emails composed.
    /// </summary>
    public int Total => Results.Count;
}
