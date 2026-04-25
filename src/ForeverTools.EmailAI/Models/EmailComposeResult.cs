namespace ForeverTools.EmailAI.Models;

/// <summary>
/// Result of an email composition request.
/// </summary>
public class EmailComposeResult
{
    /// <summary>
    /// The composed email body text.
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// The subject line (may be refined by the model).
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// The tone applied in the composed email.
    /// </summary>
    public string Tone { get; set; } = string.Empty;

    /// <summary>
    /// Optional notes from the model (e.g. suggestions for personalisation).
    /// </summary>
    public string? Notes { get; set; }
}
