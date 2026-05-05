using System.Text.Json.Serialization;

namespace ForeverTools.BrightData.Models;

/// <summary>Information about a BrightData dataset (scraper zone).</summary>
public class DatasetInfo
{
    /// <summary>Dataset ID used to trigger scrapes.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>Human-readable name of the dataset.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Dataset description.</summary>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    /// <summary>Dataset type (e.g. "scraper", "dataset").</summary>
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; set; }
}
