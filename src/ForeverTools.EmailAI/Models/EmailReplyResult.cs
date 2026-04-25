namespace ForeverTools.EmailAI.Models;

/// <summary>
/// Result of an email reply generation request.
/// </summary>
public class EmailReplyResult
{
    /// <summary>
    /// The composed reply body text.
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// The tone applied in the reply.
    /// </summary>
    public string Tone { get; set; } = string.Empty;

    /// <summary>
    /// Optional notes or suggestions from the model.
    /// </summary>
    public string? Notes { get; set; }
}
