namespace ForeverTools.Sentiment.Models;

/// <summary>
/// The result of analysing a batch of texts.
/// </summary>
public class BatchSentimentResult
{
    /// <summary>Individual results for each input text, in the same order as the input.</summary>
    public IReadOnlyList<SentimentResult> Results { get; set; } = Array.Empty<SentimentResult>();

    /// <summary>
    /// The overall label derived from the majority of individual results.
    /// If all results share the same label, that label is returned; otherwise Mixed is used.
    /// </summary>
    public SentimentLabel OverallLabel { get; set; }

    /// <summary>The arithmetic mean of all individual confidence scores.</summary>
    public double AverageConfidence { get; set; }
}
