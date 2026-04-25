namespace ForeverTools.EmailAI.Models;

/// <summary>
/// Request parameters for classifying an email.
/// </summary>
public class EmailClassifyRequest
{
    /// <summary>
    /// The full email body to classify.
    /// </summary>
    public string EmailBody { get; set; } = string.Empty;
}
