using System.Text.Json.Serialization;

namespace ForeverTools.Apify.Models;

/// <summary>
/// Represents an Apify key-value store.
/// </summary>
public class KeyValueStore
{
    /// <summary>
    /// Unique identifier of the store.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Name of the store.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// User ID of the store owner.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    /// When the store was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// When the store was last modified.
    /// </summary>
    [JsonPropertyName("modifiedAt")]
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// When the store was last accessed.
    /// </summary>
    [JsonPropertyName("accessedAt")]
    public DateTime? AccessedAt { get; set; }

    /// <summary>
    /// ID of the associated actor, if any.
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
/// Record in a key-value store.
/// </summary>
public class KeyValueRecord
{
    /// <summary>
    /// The record key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// The record value.
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Content type of the value.
    /// </summary>
    public string? ContentType { get; set; }
}

/// <summary>
/// Typed record in a key-value store.
/// </summary>
/// <typeparam name="T">Type of the record value.</typeparam>
public class KeyValueRecord<T>
{
    /// <summary>
    /// The record key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// The record value.
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// Content type of the value.
    /// </summary>
    public string? ContentType { get; set; }
}

/// <summary>
/// List of key-value store keys.
/// </summary>
public class KeyValueStoreKeysResponse
{
    /// <summary>
    /// List of keys.
    /// </summary>
    [JsonPropertyName("items")]
    public List<KeyValueStoreKey>? Items { get; set; }

    /// <summary>
    /// Total count of keys.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// Maximum items per page.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>
    /// Whether there are more items.
    /// </summary>
    [JsonPropertyName("isTruncated")]
    public bool IsTruncated { get; set; }

    /// <summary>
    /// Key to use for getting next page.
    /// </summary>
    [JsonPropertyName("nextExclusiveStartKey")]
    public string? NextExclusiveStartKey { get; set; }
}

/// <summary>
/// Key in a key-value store.
/// </summary>
public class KeyValueStoreKey
{
    /// <summary>
    /// The key name.
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>
    /// Size of the value in bytes.
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }
}

/// <summary>
/// List of key-value stores response.
/// </summary>
public class KeyValueStoreListResponse
{
    /// <summary>
    /// List of stores.
    /// </summary>
    [JsonPropertyName("items")]
    public List<KeyValueStore>? Items { get; set; }

    /// <summary>
    /// Total count of stores.
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
