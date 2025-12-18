using System.Text.Json.Serialization;

namespace ForeverTools.Postmark.Models;

/// <summary>
/// Represents an email to be sent via Postmark.
/// </summary>
public class PostmarkEmail
{
    /// <summary>
    /// The sender email address. Must be a registered Sender Signature.
    /// </summary>
    [JsonPropertyName("From")]
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// The recipient email address(es). Up to 50 comma-separated addresses.
    /// </summary>
    [JsonPropertyName("To")]
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// CC recipients. Up to 50 comma-separated addresses.
    /// </summary>
    [JsonPropertyName("Cc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Cc { get; set; }

    /// <summary>
    /// BCC recipients. Up to 50 comma-separated addresses.
    /// </summary>
    [JsonPropertyName("Bcc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Bcc { get; set; }

    /// <summary>
    /// The email subject line.
    /// </summary>
    [JsonPropertyName("Subject")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Subject { get; set; }

    /// <summary>
    /// The HTML body of the email.
    /// </summary>
    [JsonPropertyName("HtmlBody")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? HtmlBody { get; set; }

    /// <summary>
    /// The plain text body of the email.
    /// </summary>
    [JsonPropertyName("TextBody")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TextBody { get; set; }

    /// <summary>
    /// The reply-to email address.
    /// </summary>
    [JsonPropertyName("ReplyTo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ReplyTo { get; set; }

    /// <summary>
    /// A tag for categorizing the email. Max 1000 characters.
    /// </summary>
    [JsonPropertyName("Tag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Tag { get; set; }

    /// <summary>
    /// Whether to track email opens.
    /// </summary>
    [JsonPropertyName("TrackOpens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? TrackOpens { get; set; }

    /// <summary>
    /// Link tracking mode: None, HtmlAndText, HtmlOnly, TextOnly.
    /// </summary>
    [JsonPropertyName("TrackLinks")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TrackLinks { get; set; }

    /// <summary>
    /// Custom metadata key-value pairs.
    /// </summary>
    [JsonPropertyName("Metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// The message stream to use. Defaults to "outbound".
    /// </summary>
    [JsonPropertyName("MessageStream")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MessageStream { get; set; }

    /// <summary>
    /// File attachments.
    /// </summary>
    [JsonPropertyName("Attachments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<PostmarkAttachment>? Attachments { get; set; }

    /// <summary>
    /// Custom email headers.
    /// </summary>
    [JsonPropertyName("Headers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<PostmarkHeader>? Headers { get; set; }
}

/// <summary>
/// Represents an email attachment.
/// </summary>
public class PostmarkAttachment
{
    /// <summary>
    /// The filename of the attachment.
    /// </summary>
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Base64-encoded content of the attachment.
    /// </summary>
    [JsonPropertyName("Content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The MIME type of the attachment.
    /// </summary>
    [JsonPropertyName("ContentType")]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Optional Content-ID for inline images.
    /// </summary>
    [JsonPropertyName("ContentId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContentId { get; set; }

    /// <summary>
    /// Creates an attachment from a file path.
    /// </summary>
    public static PostmarkAttachment FromFile(string filePath, string? contentType = null)
    {
        var fileName = Path.GetFileName(filePath);
        var bytes = File.ReadAllBytes(filePath);
        return new PostmarkAttachment
        {
            Name = fileName,
            Content = Convert.ToBase64String(bytes),
            ContentType = contentType ?? GetContentType(fileName)
        };
    }

    /// <summary>
    /// Creates an attachment from bytes.
    /// </summary>
    public static PostmarkAttachment FromBytes(string fileName, byte[] bytes, string? contentType = null)
    {
        return new PostmarkAttachment
        {
            Name = fileName,
            Content = Convert.ToBase64String(bytes),
            ContentType = contentType ?? GetContentType(fileName)
        };
    }

    private static string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".txt" => "text/plain",
            ".html" => "text/html",
            ".csv" => "text/csv",
            ".xml" => "application/xml",
            ".json" => "application/json",
            ".zip" => "application/zip",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };
    }
}

/// <summary>
/// Represents a custom email header.
/// </summary>
public class PostmarkHeader
{
    /// <summary>
    /// The header name.
    /// </summary>
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The header value.
    /// </summary>
    [JsonPropertyName("Value")]
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Response from sending an email.
/// </summary>
public class PostmarkResponse
{
    /// <summary>
    /// The recipient email address.
    /// </summary>
    [JsonPropertyName("To")]
    public string? To { get; set; }

    /// <summary>
    /// The timestamp when the message was submitted.
    /// </summary>
    [JsonPropertyName("SubmittedAt")]
    public DateTime? SubmittedAt { get; set; }

    /// <summary>
    /// The unique message ID.
    /// </summary>
    [JsonPropertyName("MessageID")]
    public string? MessageId { get; set; }

    /// <summary>
    /// The error code. 0 means success.
    /// </summary>
    [JsonPropertyName("ErrorCode")]
    public int ErrorCode { get; set; }

    /// <summary>
    /// The status message.
    /// </summary>
    [JsonPropertyName("Message")]
    public string? Message { get; set; }

    /// <summary>
    /// Whether the email was sent successfully.
    /// </summary>
    [JsonIgnore]
    public bool Success => ErrorCode == 0;
}

/// <summary>
/// Represents an email to be sent using a template.
/// </summary>
public class PostmarkTemplateEmail
{
    /// <summary>
    /// The template ID to use.
    /// </summary>
    [JsonPropertyName("TemplateId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? TemplateId { get; set; }

    /// <summary>
    /// The template alias to use (alternative to TemplateId).
    /// </summary>
    [JsonPropertyName("TemplateAlias")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TemplateAlias { get; set; }

    /// <summary>
    /// The template model (variables to substitute).
    /// </summary>
    [JsonPropertyName("TemplateModel")]
    public Dictionary<string, object> TemplateModel { get; set; } = new();

    /// <summary>
    /// The sender email address.
    /// </summary>
    [JsonPropertyName("From")]
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// The recipient email address(es).
    /// </summary>
    [JsonPropertyName("To")]
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// CC recipients.
    /// </summary>
    [JsonPropertyName("Cc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Cc { get; set; }

    /// <summary>
    /// BCC recipients.
    /// </summary>
    [JsonPropertyName("Bcc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Bcc { get; set; }

    /// <summary>
    /// The reply-to email address.
    /// </summary>
    [JsonPropertyName("ReplyTo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ReplyTo { get; set; }

    /// <summary>
    /// A tag for categorizing the email.
    /// </summary>
    [JsonPropertyName("Tag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Tag { get; set; }

    /// <summary>
    /// Whether to track email opens.
    /// </summary>
    [JsonPropertyName("TrackOpens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? TrackOpens { get; set; }

    /// <summary>
    /// Link tracking mode.
    /// </summary>
    [JsonPropertyName("TrackLinks")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TrackLinks { get; set; }

    /// <summary>
    /// Custom metadata.
    /// </summary>
    [JsonPropertyName("Metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// The message stream to use.
    /// </summary>
    [JsonPropertyName("MessageStream")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MessageStream { get; set; }

    /// <summary>
    /// File attachments.
    /// </summary>
    [JsonPropertyName("Attachments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<PostmarkAttachment>? Attachments { get; set; }

    /// <summary>
    /// Custom email headers.
    /// </summary>
    [JsonPropertyName("Headers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<PostmarkHeader>? Headers { get; set; }

    /// <summary>
    /// Whether to inline CSS styles.
    /// </summary>
    [JsonPropertyName("InlineCss")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? InlineCss { get; set; }
}

/// <summary>
/// Link tracking options.
/// </summary>
public static class LinkTrackingOptions
{
    public const string None = "None";
    public const string HtmlAndText = "HtmlAndText";
    public const string HtmlOnly = "HtmlOnly";
    public const string TextOnly = "TextOnly";
}

/// <summary>
/// Message stream types.
/// </summary>
public static class MessageStreams
{
    public const string Outbound = "outbound";
    public const string Broadcast = "broadcast";
}
