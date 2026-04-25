namespace ForeverTools.EmailAI.Models;

/// <summary>
/// Request parameters for summarising an email.
/// </summary>
public class EmailSummarizeRequest
{
    /// <summary>
    /// The full email body to summarise.
    /// </summary>
    public string EmailBody { get; set; } = string.Empty;
}
