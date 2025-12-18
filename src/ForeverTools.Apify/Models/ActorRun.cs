using System.Text.Json.Serialization;

namespace ForeverTools.Apify.Models;

/// <summary>
/// Represents an Apify actor run.
/// </summary>
public class ActorRun
{
    /// <summary>
    /// Unique identifier of the run.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// ID of the actor that was run.
    /// </summary>
    [JsonPropertyName("actId")]
    public string? ActorId { get; set; }

    /// <summary>
    /// ID of the user who started the run.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    /// Current status of the run.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// When the run started.
    /// </summary>
    [JsonPropertyName("startedAt")]
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// When the run finished.
    /// </summary>
    [JsonPropertyName("finishedAt")]
    public DateTime? FinishedAt { get; set; }

    /// <summary>
    /// Build ID used for the run.
    /// </summary>
    [JsonPropertyName("buildId")]
    public string? BuildId { get; set; }

    /// <summary>
    /// Build number used for the run.
    /// </summary>
    [JsonPropertyName("buildNumber")]
    public string? BuildNumber { get; set; }

    /// <summary>
    /// Exit code of the run (0 = success).
    /// </summary>
    [JsonPropertyName("exitCode")]
    public int? ExitCode { get; set; }

    /// <summary>
    /// ID of the default key-value store for this run.
    /// </summary>
    [JsonPropertyName("defaultKeyValueStoreId")]
    public string? DefaultKeyValueStoreId { get; set; }

    /// <summary>
    /// ID of the default dataset for this run.
    /// </summary>
    [JsonPropertyName("defaultDatasetId")]
    public string? DefaultDatasetId { get; set; }

    /// <summary>
    /// ID of the default request queue for this run.
    /// </summary>
    [JsonPropertyName("defaultRequestQueueId")]
    public string? DefaultRequestQueueId { get; set; }

    /// <summary>
    /// Run options used.
    /// </summary>
    [JsonPropertyName("options")]
    public ActorRunOptions? Options { get; set; }

    /// <summary>
    /// Run statistics.
    /// </summary>
    [JsonPropertyName("stats")]
    public ActorRunStats? Stats { get; set; }

    /// <summary>
    /// Usage information for billing.
    /// </summary>
    [JsonPropertyName("usage")]
    public ActorRunUsage? Usage { get; set; }

    /// <summary>
    /// Whether the run is still executing.
    /// </summary>
    public bool IsRunning => Status == RunStatuses.Running || Status == RunStatuses.Ready;

    /// <summary>
    /// Whether the run completed successfully.
    /// </summary>
    public bool IsSucceeded => Status == RunStatuses.Succeeded;

    /// <summary>
    /// Whether the run failed.
    /// </summary>
    public bool IsFailed => Status == RunStatuses.Failed;

    /// <summary>
    /// Whether the run was aborted.
    /// </summary>
    public bool IsAborted => Status == RunStatuses.Aborted;

    /// <summary>
    /// Whether the run timed out.
    /// </summary>
    public bool IsTimedOut => Status == RunStatuses.TimedOut;

    /// <summary>
    /// Whether the run has finished (any terminal state).
    /// </summary>
    public bool IsFinished => !IsRunning;
}

/// <summary>
/// Options for running an actor.
/// </summary>
public class ActorRunOptions
{
    /// <summary>
    /// Memory limit in MB (128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768).
    /// </summary>
    [JsonPropertyName("memoryMbytes")]
    public int? MemoryMb { get; set; }

    /// <summary>
    /// Timeout for the run in seconds.
    /// </summary>
    [JsonPropertyName("timeoutSecs")]
    public int? TimeoutSeconds { get; set; }

    /// <summary>
    /// Build tag or number to use (e.g., "latest", "beta", "1.0").
    /// </summary>
    [JsonPropertyName("build")]
    public string? Build { get; set; }

    /// <summary>
    /// Whether to wait for the run to finish.
    /// </summary>
    [JsonPropertyName("waitForFinish")]
    public int? WaitForFinish { get; set; }

    /// <summary>
    /// Webhook URLs to call when run finishes.
    /// </summary>
    [JsonPropertyName("webhooks")]
    public List<Webhook>? Webhooks { get; set; }
}

/// <summary>
/// Statistics about an actor run.
/// </summary>
public class ActorRunStats
{
    /// <summary>
    /// Input size in bytes.
    /// </summary>
    [JsonPropertyName("inputBodyLen")]
    public long? InputBodyLen { get; set; }

    /// <summary>
    /// Number of restarts.
    /// </summary>
    [JsonPropertyName("restartCount")]
    public int RestartCount { get; set; }

    /// <summary>
    /// Duration of the run in milliseconds.
    /// </summary>
    [JsonPropertyName("durationMillis")]
    public long? DurationMillis { get; set; }

    /// <summary>
    /// Compute units consumed.
    /// </summary>
    [JsonPropertyName("computeUnits")]
    public double? ComputeUnits { get; set; }
}

/// <summary>
/// Usage information for billing.
/// </summary>
public class ActorRunUsage
{
    /// <summary>
    /// Actor compute units used.
    /// </summary>
    [JsonPropertyName("ACTOR_COMPUTE_UNITS")]
    public double? ActorComputeUnits { get; set; }

    /// <summary>
    /// Dataset reads.
    /// </summary>
    [JsonPropertyName("DATASET_READS")]
    public int? DatasetReads { get; set; }

    /// <summary>
    /// Dataset writes.
    /// </summary>
    [JsonPropertyName("DATASET_WRITES")]
    public int? DatasetWrites { get; set; }

    /// <summary>
    /// Key-value store reads.
    /// </summary>
    [JsonPropertyName("KEY_VALUE_STORE_READS")]
    public int? KeyValueStoreReads { get; set; }

    /// <summary>
    /// Key-value store writes.
    /// </summary>
    [JsonPropertyName("KEY_VALUE_STORE_WRITES")]
    public int? KeyValueStoreWrites { get; set; }
}

/// <summary>
/// Webhook configuration.
/// </summary>
public class Webhook
{
    /// <summary>
    /// Event types that trigger the webhook.
    /// </summary>
    [JsonPropertyName("eventTypes")]
    public List<string>? EventTypes { get; set; }

    /// <summary>
    /// URL to call.
    /// </summary>
    [JsonPropertyName("requestUrl")]
    public string? RequestUrl { get; set; }

    /// <summary>
    /// Payload template.
    /// </summary>
    [JsonPropertyName("payloadTemplate")]
    public string? PayloadTemplate { get; set; }
}

/// <summary>
/// Run status constants.
/// </summary>
public static class RunStatuses
{
    /// <summary>Run is ready to start.</summary>
    public const string Ready = "READY";

    /// <summary>Run is currently executing.</summary>
    public const string Running = "RUNNING";

    /// <summary>Run completed successfully.</summary>
    public const string Succeeded = "SUCCEEDED";

    /// <summary>Run failed with an error.</summary>
    public const string Failed = "FAILED";

    /// <summary>Run was aborted by user.</summary>
    public const string Aborted = "ABORTED";

    /// <summary>Run timed out.</summary>
    public const string TimedOut = "TIMED-OUT";
}
