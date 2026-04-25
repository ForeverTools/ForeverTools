namespace ForeverTools.ContentMod.Models;

/// <summary>
/// Boolean flags indicating whether text was flagged in each moderation category.
/// </summary>
public class CategoryFlags
{
    /// <summary>Whether the text contains toxic content.</summary>
    public bool Toxic { get; set; }

    /// <summary>Whether the text contains NSFW (not safe for work) content.</summary>
    public bool Nsfw { get; set; }

    /// <summary>Whether the text appears to be spam.</summary>
    public bool Spam { get; set; }

    /// <summary>Whether the text contains hate speech.</summary>
    public bool Hate { get; set; }

    /// <summary>
    /// Returns the names of all flagged categories as a read-only list.
    /// </summary>
    public IReadOnlyList<string> FlaggedCategories()
    {
        var list = new List<string>();
        if (Toxic) list.Add("toxic");
        if (Nsfw)  list.Add("nsfw");
        if (Spam)  list.Add("spam");
        if (Hate)  list.Add("hate");
        return list.AsReadOnly();
    }

    /// <summary>Returns <c>true</c> if any category is flagged.</summary>
    public bool AnyFlagged => Toxic || Nsfw || Spam || Hate;
}
