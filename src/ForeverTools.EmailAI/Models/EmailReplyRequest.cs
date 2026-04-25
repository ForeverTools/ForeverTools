namespace ForeverTools.EmailAI.Models;

/// <summary>
/// Request parameters for generating a reply to an existing email.
/// </summary>
public class EmailReplyRequest
{
    /// <summary>
    /// The original email body to which you are replying.
    /// </summary>
    public string OriginalEmail { get; set; } = string.Empty;

    /// <summary>
    /// Context describing what the reply should address or say.
    /// </summary>
    public string ReplyContext { get; set; } = string.Empty;

    /// <summary>
    /// Desired tone of the reply (e.g. "professional", "friendly", "formal", "apologetic").
    /// Defaults to "professional".
    /// </summary>
    public string Tone { get; set; } = "professional";
}
