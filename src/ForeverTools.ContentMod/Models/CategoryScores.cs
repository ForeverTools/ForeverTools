namespace ForeverTools.ContentMod.Models;

/// <summary>
/// Confidence scores (0.0–1.0) for each moderation category.
/// Higher scores indicate a stronger signal for that category.
/// </summary>
public class CategoryScores
{
    /// <summary>Toxicity score in the range [0, 1].</summary>
    public double Toxic { get; set; }

    /// <summary>NSFW content score in the range [0, 1].</summary>
    public double Nsfw { get; set; }

    /// <summary>Spam likelihood score in the range [0, 1].</summary>
    public double Spam { get; set; }

    /// <summary>Hate speech score in the range [0, 1].</summary>
    public double Hate { get; set; }

    /// <summary>
    /// Returns the name of the highest-scoring category.
    /// </summary>
    public string Dominant
    {
        get
        {
            var max = Math.Max(Math.Max(Toxic, Nsfw), Math.Max(Spam, Hate));
            if (max == Toxic) return "toxic";
            if (max == Nsfw)  return "nsfw";
            if (max == Spam)  return "spam";
            return "hate";
        }
    }

    /// <summary>
    /// Returns the highest score across all categories.
    /// </summary>
    public double MaxScore => Math.Max(Math.Max(Toxic, Nsfw), Math.Max(Spam, Hate));
}
