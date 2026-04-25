namespace ForeverTools.CodeGen.Models;

/// <summary>
/// Result of a code refactor request.
/// </summary>
public class RefactorResult
{
    /// <summary>
    /// The refactored code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// The programming language of the code.
    /// </summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Summary of changes made during refactoring.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// List of specific improvements applied.
    /// </summary>
    public List<string> Improvements { get; set; } = new();
}
