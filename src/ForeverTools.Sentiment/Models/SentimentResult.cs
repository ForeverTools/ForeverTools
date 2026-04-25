namespace ForeverTools.Sentiment.Models;

/// <summary>
/// The result of a sentiment analysis operation.
/// </summary>
public class SentimentResult
{
    /// <summary>The overall sentiment label (Positive, Negative, Neutral, or Mixed).</summary>
    public SentimentLabel Label { get; set; }

    /// <summary>Confidence score for the label, in the range [0, 1].</summary>
    public double Confidence { get; set; }

    /// <summary>Per-emotion intensity scores.</summary>
    public EmotionScores Emotions { get; set; } = new();

    /// <summary>A short human-readable summary of the detected tone.</summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>The original text that was analysed.</summary>
    public string InputText { get; set; } = string.Empty;

    /// <summary>How long the API call took in milliseconds.</summary>
    public long ProcessingMs { get; set; }
}
