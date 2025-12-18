using System.Text.Json.Serialization;

namespace ForeverTools.Apify.Models;

/// <summary>
/// Represents an Apify actor.
/// </summary>
public class Actor
{
    /// <summary>
    /// Unique identifier of the actor.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Name of the actor.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Username of the actor owner.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>
    /// Full actor identifier (username/actor-name).
    /// </summary>
    public string? FullName => Username != null && Name != null ? $"{Username}/{Name}" : null;

    /// <summary>
    /// Description of what the actor does.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Title displayed in the UI.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Whether the actor is public.
    /// </summary>
    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; set; }

    /// <summary>
    /// Whether the actor is deprecated.
    /// </summary>
    [JsonPropertyName("isDeprecated")]
    public bool IsDeprecated { get; set; }

    /// <summary>
    /// Actor version information.
    /// </summary>
    [JsonPropertyName("versions")]
    public List<ActorVersion>? Versions { get; set; }

    /// <summary>
    /// Default run configuration.
    /// </summary>
    [JsonPropertyName("defaultRunOptions")]
    public ActorRunOptions? DefaultRunOptions { get; set; }

    /// <summary>
    /// Actor statistics.
    /// </summary>
    [JsonPropertyName("stats")]
    public ActorStats? Stats { get; set; }

    /// <summary>
    /// When the actor was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// When the actor was last modified.
    /// </summary>
    [JsonPropertyName("modifiedAt")]
    public DateTime? ModifiedAt { get; set; }
}

/// <summary>
/// Actor version information.
/// </summary>
public class ActorVersion
{
    /// <summary>
    /// Version number (e.g., "0.1", "1.0").
    /// </summary>
    [JsonPropertyName("versionNumber")]
    public string? VersionNumber { get; set; }

    /// <summary>
    /// Build tag (e.g., "latest", "beta").
    /// </summary>
    [JsonPropertyName("buildTag")]
    public string? BuildTag { get; set; }

    /// <summary>
    /// Source type (e.g., "SOURCE_FILES", "GIT_REPO").
    /// </summary>
    [JsonPropertyName("sourceType")]
    public string? SourceType { get; set; }
}

/// <summary>
/// Actor statistics.
/// </summary>
public class ActorStats
{
    /// <summary>
    /// Total number of runs.
    /// </summary>
    [JsonPropertyName("totalRuns")]
    public int TotalRuns { get; set; }

    /// <summary>
    /// Total number of users.
    /// </summary>
    [JsonPropertyName("totalUsers")]
    public int TotalUsers { get; set; }

    /// <summary>
    /// Total number of builds.
    /// </summary>
    [JsonPropertyName("totalBuilds")]
    public int TotalBuilds { get; set; }

    /// <summary>
    /// Number of runs in the last 30 days.
    /// </summary>
    [JsonPropertyName("totalRuns30Days")]
    public int TotalRuns30Days { get; set; }
}

/// <summary>
/// List of actors response.
/// </summary>
public class ActorListResponse
{
    /// <summary>
    /// List of actors.
    /// </summary>
    [JsonPropertyName("items")]
    public List<Actor>? Items { get; set; }

    /// <summary>
    /// Total count of actors.
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
