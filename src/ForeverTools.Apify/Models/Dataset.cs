using System.Text.Json.Serialization;

namespace ForeverTools.Apify.Models;

/// <summary>
/// Represents an Apify dataset for storing structured data.
/// </summary>
public class Dataset
{
    /// <summary>
    /// Unique identifier of the dataset.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Name of the dataset.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// User ID of the dataset owner.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    /// Number of items in the dataset.
    /// </summary>
    [JsonPropertyName("itemCount")]
    public int ItemCount { get; set; }

    /// <summary>
    /// When the dataset was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// When the dataset was last modified.
    /// </summary>
    [JsonPropertyName("modifiedAt")]
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// When the dataset was last accessed.
    /// </summary>
    [JsonPropertyName("accessedAt")]
    public DateTime? AccessedAt { get; set; }

    /// <summary>
    /// Total size of clean items in bytes.
    /// </summary>
    [JsonPropertyName("cleanItemCount")]
    public int CleanItemCount { get; set; }

    /// <summary>
    /// ID of the associated actor task, if any.
    /// </summary>
    [JsonPropertyName("actId")]
    public string? ActorId { get; set; }

    /// <summary>
    /// ID of the associated actor run, if any.
    /// </summary>
    [JsonPropertyName("actRunId")]
    public string? ActorRunId { get; set; }
}

/// <summary>
/// Response containing dataset items.
/// </summary>
/// <typeparam name="T">Type of items in the dataset.</typeparam>
public class DatasetItemsResponse<T>
{
    /// <summary>
    /// The dataset items.
    /// </summary>
    public List<T>? Items { get; set; }

    /// <summary>
    /// Total number of items in the dataset.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Pagination offset.
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Number of items returned.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Maximum items per page.
    /// </summary>
    public int Limit { get; set; }
}

/// <summary>
/// List of datasets response.
/// </summary>
public class DatasetListResponse
{
    /// <summary>
    /// List of datasets.
    /// </summary>
    [JsonPropertyName("items")]
    public List<Dataset>? Items { get; set; }

    /// <summary>
    /// Total count of datasets.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// Pagination offset.
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>
    /// Number of items returned.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// Maximum items per page.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

/// <summary>
/// Options for retrieving dataset items.
/// </summary>
public class DatasetItemsOptions
{
    /// <summary>
    /// Number of items to skip.
    /// </summary>
    public int? Offset { get; set; }

    /// <summary>
    /// Maximum number of items to return.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// If true, returns only clean items (without internal fields).
    /// </summary>
    public bool? Clean { get; set; }

    /// <summary>
    /// Output format: json, jsonl, csv, xlsx, xml, html, rss.
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Fields to include in the response.
    /// </summary>
    public List<string>? Fields { get; set; }

    /// <summary>
    /// Fields to omit from the response.
    /// </summary>
    public List<string>? OmitFields { get; set; }

    /// <summary>
    /// Unwind the given array field.
    /// </summary>
    public string? Unwind { get; set; }

    /// <summary>
    /// Flatten nested objects.
    /// </summary>
    public bool? Flatten { get; set; }
}
