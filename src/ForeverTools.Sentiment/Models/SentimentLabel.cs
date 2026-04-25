namespace ForeverTools.Sentiment.Models;

/// <summary>
/// The overall sentiment label for a piece of text.
/// </summary>
public enum SentimentLabel
{
    /// <summary>Positive sentiment detected.</summary>
    Positive,

    /// <summary>Negative sentiment detected.</summary>
    Negative,

    /// <summary>Neutral sentiment — neither positive nor negative.</summary>
    Neutral,

    /// <summary>Mixed sentiment — both positive and negative signals present.</summary>
    Mixed
}
