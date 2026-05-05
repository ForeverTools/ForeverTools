using System.Text.Json.Serialization;

namespace ForeverTools.BrightData.Models;

/// <summary>Request to trigger a BrightData dataset scrape job.</summary>
public class TriggerRequest
{
    /// <summary>List of inputs for the scrape (e.g. URLs, search terms, product IDs).</summary>
    [JsonPropertyName("inputs")]
    public List<Dictionary<string, string>> Inputs { get; set; } = new();

    /// <summary>Optional: notify via webhook when complete.</summary>
    [JsonPropertyName("notify")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? NotifyUrl { get; set; }

    /// <summary>Optional: custom output format. Values: "json" (default), "ndjson", "csv".</summary>
    [JsonPropertyName("format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Format { get; set; }

    /// <summary>Optional: include errors in output.</summary>
    [JsonPropertyName("include_errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IncludeErrors { get; set; }
}
