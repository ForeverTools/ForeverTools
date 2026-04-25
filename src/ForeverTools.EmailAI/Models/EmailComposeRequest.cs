namespace ForeverTools.EmailAI.Models;

/// <summary>
/// Request parameters for composing a new email.
/// </summary>
public class EmailComposeRequest
{
    /// <summary>
    /// The email subject line.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Recipient names or addresses (used for personalisation context).
    /// </summary>
    public IList<string> Recipients { get; set; } = new List<string>();

    /// <summary>
    /// Background context or bullet points describing what the email should say.
    /// </summary>
    public string Context { get; set; } = string.Empty;

    /// <summary>
    /// Desired tone of the email (e.g. "professional", "friendly", "formal", "casual").
    /// Defaults to "professional".
    /// </summary>
    public string Tone { get; set; } = "professional";
}
