namespace ForeverTools.ContentMod.Models;

/// <summary>
/// The result of a content moderation check on a single piece of text.
/// </summary>
public class ModerationResult
{
    /// <summary>
    /// Whether the text was flagged for any category.
    /// </summary>
    public bool Flagged { get; set; }

    /// <summary>
    /// Boolean flags indicating whether the text matched each moderation category.
    /// </summary>
    public CategoryFlags Categories { get; set; } = new();

    /// <summary>
    /// Confidence scores (0–1) for each moderation category.
    /// </summary>
    public CategoryScores Scores { get; set; } = new();

    /// <summary>
    /// Convenience property: <c>true</c> if the text is considered safe for work.
    /// Equivalent to <c>!Flagged</c> but derived from the API's own assessment.
    /// </summary>
    public bool SafeForWork { get; set; }

    /// <summary>
    /// The original text that was moderated.
    /// </summary>
    public string InputText { get; set; } = string.Empty;

    /// <summary>
    /// How long the API call took in milliseconds.
    /// </summary>
    public long ProcessingMs { get; set; }
}
