namespace ForeverTools.Summarize.Models;

/// <summary>
/// Result of a summarization operation.
/// </summary>
public class SummaryResult
{
    /// <summary>
    /// The generated summary.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// The original text that was summarized.
    /// </summary>
    public string OriginalText { get; set; } = string.Empty;

    /// <summary>
    /// The style used for the summary.
    /// </summary>
    public SummaryStyle Style { get; set; }

    /// <summary>
    /// The length setting used.
    /// </summary>
    public SummaryLength Length { get; set; }

    /// <summary>
    /// The AI model used for summarization.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Word count of the original text.
    /// </summary>
    public int OriginalWordCount { get; set; }

    /// <summary>
    /// Word count of the summary.
    /// </summary>
    public int SummaryWordCount { get; set; }

    /// <summary>
    /// Compression ratio (summary words / original words).
    /// </summary>
    public double CompressionRatio => OriginalWordCount > 0
        ? Math.Round((double)SummaryWordCount / OriginalWordCount, 3)
        : 0;

    /// <summary>
    /// Percentage of text reduced.
    /// </summary>
    public double ReductionPercentage => OriginalWordCount > 0
        ? Math.Round((1 - (double)SummaryWordCount / OriginalWordCount) * 100, 1)
        : 0;

    /// <summary>
    /// Key points extracted from the text (if requested).
    /// </summary>
    public IReadOnlyList<string>? KeyPoints { get; set; }

    /// <summary>
    /// Key statistics/numbers from the text (if requested).
    /// </summary>
    public IReadOnlyList<string>? Statistics { get; set; }

    /// <summary>
    /// Detected content domain.
    /// </summary>
    public ContentDomain? DetectedDomain { get; set; }

    /// <summary>
    /// Detected language of the original text.
    /// </summary>
    public string? DetectedLanguage { get; set; }
}

/// <summary>
/// Result of a key points extraction operation.
/// </summary>
public class KeyPointsResult
{
    /// <summary>
    /// The extracted key points.
    /// </summary>
    public IReadOnlyList<string> KeyPoints { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The original text that was analyzed.
    /// </summary>
    public string OriginalText { get; set; } = string.Empty;

    /// <summary>
    /// The AI model used.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Themes or topics identified in the text.
    /// </summary>
    public IReadOnlyList<string>? Themes { get; set; }
}

/// <summary>
/// Result of a batch summarization operation.
/// </summary>
public class BatchSummaryResult
{
    /// <summary>
    /// Individual summary results.
    /// </summary>
    public IReadOnlyList<SummaryResult> Results { get; set; } = Array.Empty<SummaryResult>();

    /// <summary>
    /// Number of successful summarizations.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed summarizations.
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Total texts processed.
    /// </summary>
    public int TotalCount => SuccessCount + FailureCount;

    /// <summary>
    /// Any errors that occurred during batch summarization.
    /// </summary>
    public IReadOnlyList<BatchSummaryError>? Errors { get; set; }

    /// <summary>
    /// Total word count of all original texts.
    /// </summary>
    public int TotalOriginalWords => Results.Sum(r => r.OriginalWordCount);

    /// <summary>
    /// Total word count of all summaries.
    /// </summary>
    public int TotalSummaryWords => Results.Sum(r => r.SummaryWordCount);
}

/// <summary>
/// Error information for a failed summarization in a batch.
/// </summary>
public class BatchSummaryError
{
    /// <summary>
    /// Index of the failed text in the original batch.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// The original text that failed to summarize.
    /// </summary>
    public string OriginalText { get; set; } = string.Empty;

    /// <summary>
    /// Error message.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Request for summarizing text with all options.
/// </summary>
public class SummaryRequest
{
    /// <summary>
    /// The text to summarize.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Summary output style.
    /// </summary>
    public SummaryStyle? Style { get; set; }

    /// <summary>
    /// Summary length.
    /// </summary>
    public SummaryLength? Length { get; set; }

    /// <summary>
    /// Custom word count (when Length is Custom).
    /// </summary>
    public int? CustomWordCount { get; set; }

    /// <summary>
    /// Custom percentage of original (when Length is Custom).
    /// </summary>
    public int? CustomPercentage { get; set; }

    /// <summary>
    /// Content domain hint for better summarization.
    /// </summary>
    public ContentDomain? Domain { get; set; }

    /// <summary>
    /// Specific AI model to use (overrides default).
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Additional context or instructions for the summarization.
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    /// Target audience for the summary.
    /// </summary>
    public string? TargetAudience { get; set; }

    /// <summary>
    /// Whether to extract key points alongside the summary.
    /// </summary>
    public bool ExtractKeyPoints { get; set; }

    /// <summary>
    /// Whether to extract statistics/numbers from the text.
    /// </summary>
    public bool ExtractStatistics { get; set; }

    /// <summary>
    /// Whether to preserve important quotes.
    /// </summary>
    public bool PreserveQuotes { get; set; }

    /// <summary>
    /// Focus areas - specific topics to emphasize in the summary.
    /// </summary>
    public IEnumerable<string>? FocusAreas { get; set; }

    /// <summary>
    /// Language for the output summary (defaults to same as input).
    /// </summary>
    public string? OutputLanguage { get; set; }
}

/// <summary>
/// Result of comparing multiple summaries.
/// </summary>
public class SummaryComparisonResult
{
    /// <summary>
    /// Combined summary synthesizing all inputs.
    /// </summary>
    public string CombinedSummary { get; set; } = string.Empty;

    /// <summary>
    /// Common themes across all texts.
    /// </summary>
    public IReadOnlyList<string> CommonThemes { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Key differences between the texts.
    /// </summary>
    public IReadOnlyList<string> KeyDifferences { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Individual summaries.
    /// </summary>
    public IReadOnlyList<SummaryResult> IndividualSummaries { get; set; } = Array.Empty<SummaryResult>();
}

/// <summary>
/// Result of extracting action items from text.
/// </summary>
public class ActionItemsResult
{
    /// <summary>
    /// Extracted action items.
    /// </summary>
    public IReadOnlyList<ActionItem> ActionItems { get; set; } = Array.Empty<ActionItem>();

    /// <summary>
    /// The original text analyzed.
    /// </summary>
    public string OriginalText { get; set; } = string.Empty;

    /// <summary>
    /// The AI model used.
    /// </summary>
    public string Model { get; set; } = string.Empty;
}

/// <summary>
/// Represents an action item extracted from text.
/// </summary>
public class ActionItem
{
    /// <summary>
    /// Description of the action item.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Assigned person/team (if identifiable).
    /// </summary>
    public string? Assignee { get; set; }

    /// <summary>
    /// Due date (if mentioned).
    /// </summary>
    public string? DueDate { get; set; }

    /// <summary>
    /// Priority level.
    /// </summary>
    public ActionPriority Priority { get; set; } = ActionPriority.Normal;
}

/// <summary>
/// Action item priority levels.
/// </summary>
public enum ActionPriority
{
    Low,
    Normal,
    High,
    Urgent
}
