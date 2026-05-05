using System.Text.Json.Serialization;

namespace ForeverTools.BrightData.Models;

/// <summary>Status of a BrightData snapshot (scrape job).</summary>
public class SnapshotStatus
{
    /// <summary>Snapshot ID.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Current status. Common values:
    /// "running" — still in progress
    /// "ready" — results available for download
    /// "failed" — scrape failed
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>Number of records collected so far.</summary>
    [JsonPropertyName("records")]
    public int Records { get; set; }

    /// <summary>Error message if status is "failed".</summary>
    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Error { get; set; }

    /// <summary>Whether the snapshot has completed successfully.</summary>
    [JsonIgnore]
    public bool IsReady => Status == "ready";

    /// <summary>Whether the snapshot failed.</summary>
    [JsonIgnore]
    public bool IsFailed => Status == "failed";

    /// <summary>Whether the snapshot is still running.</summary>
    [JsonIgnore]
    public bool IsRunning => Status == "running";
}
