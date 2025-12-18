using System.Text.Json.Serialization;

namespace ForeverTools.Apify.Models;

/// <summary>
/// Represents the current Apify user.
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Username.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>
    /// Email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Profile information.
    /// </summary>
    [JsonPropertyName("profile")]
    public UserProfile? Profile { get; set; }

    /// <summary>
    /// Current subscription plan.
    /// </summary>
    [JsonPropertyName("plan")]
    public UserPlan? Plan { get; set; }

    /// <summary>
    /// Resource limits for the user.
    /// </summary>
    [JsonPropertyName("limits")]
    public UserLimits? Limits { get; set; }
}

/// <summary>
/// User profile information.
/// </summary>
public class UserProfile
{
    /// <summary>
    /// User's display name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// User's profile picture URL.
    /// </summary>
    [JsonPropertyName("pictureUrl")]
    public string? PictureUrl { get; set; }

    /// <summary>
    /// User's bio/description.
    /// </summary>
    [JsonPropertyName("bio")]
    public string? Bio { get; set; }
}

/// <summary>
/// User subscription plan.
/// </summary>
public class UserPlan
{
    /// <summary>
    /// Plan ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Plan name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Monthly price in USD.
    /// </summary>
    [JsonPropertyName("monthlyBasePriceUsd")]
    public decimal? MonthlyBasePriceUsd { get; set; }

    /// <summary>
    /// Monthly usage credits included.
    /// </summary>
    [JsonPropertyName("monthlyUsageCreditsUsd")]
    public decimal? MonthlyUsageCreditsUsd { get; set; }
}

/// <summary>
/// User resource limits.
/// </summary>
public class UserLimits
{
    /// <summary>
    /// Maximum monthly usage in USD.
    /// </summary>
    [JsonPropertyName("maxMonthlyUsageUsd")]
    public decimal? MaxMonthlyUsageUsd { get; set; }

    /// <summary>
    /// Maximum concurrent actor runs.
    /// </summary>
    [JsonPropertyName("maxConcurrentActorJobs")]
    public int MaxConcurrentActorJobs { get; set; }

    /// <summary>
    /// Maximum actors.
    /// </summary>
    [JsonPropertyName("maxActorCount")]
    public int MaxActorCount { get; set; }

    /// <summary>
    /// Maximum actor tasks.
    /// </summary>
    [JsonPropertyName("maxActorTaskCount")]
    public int MaxActorTaskCount { get; set; }

    /// <summary>
    /// Maximum schedules.
    /// </summary>
    [JsonPropertyName("maxScheduleCount")]
    public int MaxScheduleCount { get; set; }

    /// <summary>
    /// Maximum actor memory in MB.
    /// </summary>
    [JsonPropertyName("maxActorMemoryMbytes")]
    public int MaxActorMemoryMb { get; set; }
}

/// <summary>
/// User usage statistics.
/// </summary>
public class UserUsage
{
    /// <summary>
    /// Current monthly usage in USD.
    /// </summary>
    [JsonPropertyName("monthlyUsageUsd")]
    public decimal MonthlyUsageUsd { get; set; }

    /// <summary>
    /// Actor compute units used this month.
    /// </summary>
    [JsonPropertyName("actorComputeUnits")]
    public double ActorComputeUnits { get; set; }

    /// <summary>
    /// Data transfer in GB this month.
    /// </summary>
    [JsonPropertyName("dataTransferGb")]
    public double DataTransferGb { get; set; }

    /// <summary>
    /// Storage in GB used.
    /// </summary>
    [JsonPropertyName("storageGb")]
    public double StorageGb { get; set; }
}
