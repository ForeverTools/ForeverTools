using System.Text.Json.Serialization;

namespace ForeverTools.BrightData.Models;

/// <summary>Response from triggering a BrightData scrape job.</summary>
public class TriggerResponse
{
    /// <summary>Snapshot ID to use for polling status and downloading results.</summary>
    [JsonPropertyName("snapshot_id")]
    public string SnapshotId { get; set; } = string.Empty;

    /// <summary>Current status of the snapshot.</summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
