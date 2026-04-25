namespace ForeverTools.Sentiment.Models;

/// <summary>
/// Emotion intensity scores detected in the analysed text. All values are in the range [0, 1].
/// </summary>
public class EmotionScores
{
    /// <summary>Joy / happiness intensity (0–1).</summary>
    public double Joy { get; set; }

    /// <summary>Anger / frustration intensity (0–1).</summary>
    public double Anger { get; set; }

    /// <summary>Sadness / disappointment intensity (0–1).</summary>
    public double Sadness { get; set; }

    /// <summary>Fear / anxiety intensity (0–1).</summary>
    public double Fear { get; set; }

    /// <summary>Surprise / unexpectedness intensity (0–1).</summary>
    public double Surprise { get; set; }

    /// <summary>Disgust / revulsion intensity (0–1).</summary>
    public double Disgust { get; set; }

    /// <summary>
    /// Returns the name of the emotion with the highest score.
    /// </summary>
    public string Dominant
    {
        get
        {
            var emotions = new[]
            {
                (nameof(Joy), Joy),
                (nameof(Anger), Anger),
                (nameof(Sadness), Sadness),
                (nameof(Fear), Fear),
                (nameof(Surprise), Surprise),
                (nameof(Disgust), Disgust)
            };

            string dominant = nameof(Joy);
            double highest = -1;
            foreach (var (name, score) in emotions)
            {
                if (score > highest)
                {
                    highest = score;
                    dominant = name;
                }
            }
            return dominant;
        }
    }
}
