using System.Text.Json.Serialization;

namespace ForeverTools.Apify.Models;

/// <summary>
/// Represents an Apify schedule for automated actor runs.
/// </summary>
public class Schedule
{
    /// <summary>
    /// Unique identifier of the schedule.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// User ID of the schedule owner.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    /// Name of the schedule.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Cron expression defining when to run.
    /// </summary>
    [JsonPropertyName("cronExpression")]
    public string? CronExpression { get; set; }

    /// <summary>
    /// Timezone for the cron expression (e.g., "America/New_York").
    /// </summary>
    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    /// <summary>
    /// Whether the schedule is enabled.
    /// </summary>
    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Whether the schedule is exclusive (only one run at a time).
    /// </summary>
    [JsonPropertyName("isExclusive")]
    public bool IsExclusive { get; set; }

    /// <summary>
    /// Description of the schedule.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// When the schedule was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// When the schedule was last modified.
    /// </summary>
    [JsonPropertyName("modifiedAt")]
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// When the schedule last ran.
    /// </summary>
    [JsonPropertyName("lastRunAt")]
    public DateTime? LastRunAt { get; set; }

    /// <summary>
    /// When the schedule will next run.
    /// </summary>
    [JsonPropertyName("nextRunAt")]
    public DateTime? NextRunAt { get; set; }

    /// <summary>
    /// Actions to perform when the schedule triggers.
    /// </summary>
    [JsonPropertyName("actions")]
    public List<ScheduleAction>? Actions { get; set; }
}

/// <summary>
/// Action to perform when a schedule triggers.
/// </summary>
public class ScheduleAction
{
    /// <summary>
    /// Type of action (RUN_ACTOR, RUN_ACTOR_TASK).
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Actor ID to run (for RUN_ACTOR type).
    /// </summary>
    [JsonPropertyName("actorId")]
    public string? ActorId { get; set; }

    /// <summary>
    /// Actor task ID to run (for RUN_ACTOR_TASK type).
    /// </summary>
    [JsonPropertyName("actorTaskId")]
    public string? ActorTaskId { get; set; }

    /// <summary>
    /// Input for the actor run.
    /// </summary>
    [JsonPropertyName("runInput")]
    public ScheduleRunInput? RunInput { get; set; }

    /// <summary>
    /// Run options.
    /// </summary>
    [JsonPropertyName("runOptions")]
    public ActorRunOptions? RunOptions { get; set; }
}

/// <summary>
/// Input configuration for scheduled runs.
/// </summary>
public class ScheduleRunInput
{
    /// <summary>
    /// Content type of the body.
    /// </summary>
    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }

    /// <summary>
    /// The input body (JSON string or other format).
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; set; }
}

/// <summary>
/// Request to create or update a schedule.
/// </summary>
public class ScheduleRequest
{
    /// <summary>
    /// Name of the schedule.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Cron expression defining when to run.
    /// </summary>
    [JsonPropertyName("cronExpression")]
    public string? CronExpression { get; set; }

    /// <summary>
    /// Timezone for the cron expression.
    /// </summary>
    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    /// <summary>
    /// Whether the schedule is enabled.
    /// </summary>
    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Whether the schedule is exclusive.
    /// </summary>
    [JsonPropertyName("isExclusive")]
    public bool IsExclusive { get; set; }

    /// <summary>
    /// Description of the schedule.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Actions to perform when the schedule triggers.
    /// </summary>
    [JsonPropertyName("actions")]
    public List<ScheduleAction>? Actions { get; set; }
}

/// <summary>
/// List of schedules response.
/// </summary>
public class ScheduleListResponse
{
    /// <summary>
    /// List of schedules.
    /// </summary>
    [JsonPropertyName("items")]
    public List<Schedule>? Items { get; set; }

    /// <summary>
    /// Total count of schedules.
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
/// Schedule action type constants.
/// </summary>
public static class ScheduleActionTypes
{
    /// <summary>Run an actor.</summary>
    public const string RunActor = "RUN_ACTOR";

    /// <summary>Run an actor task.</summary>
    public const string RunActorTask = "RUN_ACTOR_TASK";
}
