using System.Text.Json.Serialization;

namespace ForeverTools.Postmark.Models;

/// <summary>
/// Delivery statistics overview.
/// </summary>
public class PostmarkDeliveryStats
{
    /// <summary>
    /// Number of inactive recipients (bounces, spam complaints).
    /// </summary>
    [JsonPropertyName("InactiveMails")]
    public int InactiveMails { get; set; }

    /// <summary>
    /// Bounce statistics breakdown.
    /// </summary>
    [JsonPropertyName("Bounces")]
    public List<BounceStats>? Bounces { get; set; }
}

/// <summary>
/// Bounce statistics by type.
/// </summary>
public class BounceStats
{
    /// <summary>
    /// The bounce type name.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    /// <summary>
    /// The number of bounces of this type.
    /// </summary>
    [JsonPropertyName("Count")]
    public int Count { get; set; }

    /// <summary>
    /// The bounce type code.
    /// </summary>
    [JsonPropertyName("Type")]
    public string? Type { get; set; }
}

/// <summary>
/// Represents a bounced email.
/// </summary>
public class PostmarkBounce
{
    /// <summary>
    /// The bounce ID.
    /// </summary>
    [JsonPropertyName("ID")]
    public long Id { get; set; }

    /// <summary>
    /// The bounce type.
    /// </summary>
    [JsonPropertyName("Type")]
    public string? Type { get; set; }

    /// <summary>
    /// The bounce type code.
    /// </summary>
    [JsonPropertyName("TypeCode")]
    public int TypeCode { get; set; }

    /// <summary>
    /// The bounce name/description.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    /// <summary>
    /// The associated tag.
    /// </summary>
    [JsonPropertyName("Tag")]
    public string? Tag { get; set; }

    /// <summary>
    /// The original message ID.
    /// </summary>
    [JsonPropertyName("MessageID")]
    public string? MessageId { get; set; }

    /// <summary>
    /// The server ID.
    /// </summary>
    [JsonPropertyName("ServerID")]
    public long ServerId { get; set; }

    /// <summary>
    /// The message stream.
    /// </summary>
    [JsonPropertyName("MessageStream")]
    public string? MessageStream { get; set; }

    /// <summary>
    /// The bounce description.
    /// </summary>
    [JsonPropertyName("Description")]
    public string? Description { get; set; }

    /// <summary>
    /// Details about the bounce.
    /// </summary>
    [JsonPropertyName("Details")]
    public string? Details { get; set; }

    /// <summary>
    /// The bounced email address.
    /// </summary>
    [JsonPropertyName("Email")]
    public string? Email { get; set; }

    /// <summary>
    /// The sender (from) address.
    /// </summary>
    [JsonPropertyName("From")]
    public string? From { get; set; }

    /// <summary>
    /// When the bounce occurred.
    /// </summary>
    [JsonPropertyName("BouncedAt")]
    public DateTime? BouncedAt { get; set; }

    /// <summary>
    /// Whether the bounce can be reactivated.
    /// </summary>
    [JsonPropertyName("CanActivate")]
    public bool CanActivate { get; set; }

    /// <summary>
    /// The email subject.
    /// </summary>
    [JsonPropertyName("Subject")]
    public string? Subject { get; set; }

    /// <summary>
    /// The bounce content/body.
    /// </summary>
    [JsonPropertyName("Content")]
    public string? Content { get; set; }

    /// <summary>
    /// Whether the recipient is inactive.
    /// </summary>
    [JsonPropertyName("Inactive")]
    public bool Inactive { get; set; }
}

/// <summary>
/// List of bounces response.
/// </summary>
public class PostmarkBouncesResponse
{
    /// <summary>
    /// Total count of bounces.
    /// </summary>
    [JsonPropertyName("TotalCount")]
    public int TotalCount { get; set; }

    /// <summary>
    /// List of bounces.
    /// </summary>
    [JsonPropertyName("Bounces")]
    public List<PostmarkBounce>? Bounces { get; set; }
}

/// <summary>
/// Outbound message statistics.
/// </summary>
public class PostmarkOutboundStats
{
    /// <summary>
    /// Total emails sent.
    /// </summary>
    [JsonPropertyName("Sent")]
    public int Sent { get; set; }

    /// <summary>
    /// Total emails bounced.
    /// </summary>
    [JsonPropertyName("Bounced")]
    public int Bounced { get; set; }

    /// <summary>
    /// Total spam complaints.
    /// </summary>
    [JsonPropertyName("SpamComplaints")]
    public int SpamComplaints { get; set; }

    /// <summary>
    /// Bounce rate percentage.
    /// </summary>
    [JsonPropertyName("BounceRate")]
    public double BounceRate { get; set; }

    /// <summary>
    /// Spam complaint rate percentage.
    /// </summary>
    [JsonPropertyName("SpamComplaintsRate")]
    public double SpamComplaintsRate { get; set; }

    /// <summary>
    /// Unique opens count.
    /// </summary>
    [JsonPropertyName("UniqueOpens")]
    public int UniqueOpens { get; set; }

    /// <summary>
    /// Total opens count.
    /// </summary>
    [JsonPropertyName("TotalOpens")]
    public int TotalOpens { get; set; }

    /// <summary>
    /// Unique clicks count.
    /// </summary>
    [JsonPropertyName("UniqueClicks")]
    public int UniqueClicks { get; set; }

    /// <summary>
    /// Total clicks count.
    /// </summary>
    [JsonPropertyName("TotalClicks")]
    public int TotalClicks { get; set; }

    /// <summary>
    /// Tracked emails with open tracking enabled.
    /// </summary>
    [JsonPropertyName("WithOpenTracking")]
    public int WithOpenTracking { get; set; }

    /// <summary>
    /// Tracked emails with link tracking enabled.
    /// </summary>
    [JsonPropertyName("WithLinkTracking")]
    public int WithLinkTracking { get; set; }

    /// <summary>
    /// Total emails tracked.
    /// </summary>
    [JsonPropertyName("TotalTrackedLinksSent")]
    public int TotalTrackedLinksSent { get; set; }
}

/// <summary>
/// Server information.
/// </summary>
public class PostmarkServer
{
    /// <summary>
    /// The server ID.
    /// </summary>
    [JsonPropertyName("ID")]
    public long Id { get; set; }

    /// <summary>
    /// The server name.
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    /// <summary>
    /// The server API tokens (masked).
    /// </summary>
    [JsonPropertyName("ApiTokens")]
    public List<string>? ApiTokens { get; set; }

    /// <summary>
    /// The server color in the UI.
    /// </summary>
    [JsonPropertyName("Color")]
    public string? Color { get; set; }

    /// <summary>
    /// Whether SMTP is enabled.
    /// </summary>
    [JsonPropertyName("SmtpApiActivated")]
    public bool SmtpApiActivated { get; set; }

    /// <summary>
    /// The raw email enabled status.
    /// </summary>
    [JsonPropertyName("RawEmailEnabled")]
    public bool RawEmailEnabled { get; set; }

    /// <summary>
    /// The delivery type.
    /// </summary>
    [JsonPropertyName("DeliveryType")]
    public string? DeliveryType { get; set; }

    /// <summary>
    /// The inbound address.
    /// </summary>
    [JsonPropertyName("InboundAddress")]
    public string? InboundAddress { get; set; }

    /// <summary>
    /// The inbound hook URL.
    /// </summary>
    [JsonPropertyName("InboundHookUrl")]
    public string? InboundHookUrl { get; set; }

    /// <summary>
    /// The bounce hook URL.
    /// </summary>
    [JsonPropertyName("BounceHookUrl")]
    public string? BounceHookUrl { get; set; }

    /// <summary>
    /// The open hook URL.
    /// </summary>
    [JsonPropertyName("OpenHookUrl")]
    public string? OpenHookUrl { get; set; }

    /// <summary>
    /// Whether open tracking is enabled.
    /// </summary>
    [JsonPropertyName("TrackOpens")]
    public bool TrackOpens { get; set; }

    /// <summary>
    /// The link tracking mode.
    /// </summary>
    [JsonPropertyName("TrackLinks")]
    public string? TrackLinks { get; set; }

    /// <summary>
    /// The inbound domain.
    /// </summary>
    [JsonPropertyName("InboundDomain")]
    public string? InboundDomain { get; set; }

    /// <summary>
    /// The inbound spam threshold.
    /// </summary>
    [JsonPropertyName("InboundSpamThreshold")]
    public int InboundSpamThreshold { get; set; }
}

/// <summary>
/// Bounce activation result.
/// </summary>
public class PostmarkBounceActivation
{
    /// <summary>
    /// Status message.
    /// </summary>
    [JsonPropertyName("Message")]
    public string? Message { get; set; }

    /// <summary>
    /// The reactivated bounce.
    /// </summary>
    [JsonPropertyName("Bounce")]
    public PostmarkBounce? Bounce { get; set; }
}
