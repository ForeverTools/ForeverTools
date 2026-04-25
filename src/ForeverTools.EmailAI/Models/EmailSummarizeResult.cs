using System.Text.Json.Serialization;

namespace ForeverTools.EmailAI.Models;

/// <summary>
/// Result of an email summarisation request.
/// </summary>
public class EmailSummarizeResult
{
    /// <summary>
    /// A concise summary of the email content.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Key action items or follow-ups identified in the email.
    /// </summary>
    [JsonPropertyName("action_items")]
    public List<string> ActionItems { get; set; } = new();

    /// <summary>
    /// The main topic or subject inferred from the email body.
    /// </summary>
    public string Topic { get; set; } = string.Empty;
}
