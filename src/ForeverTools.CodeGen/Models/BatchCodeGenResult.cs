namespace ForeverTools.CodeGen.Models;

/// <summary>
/// Result of a batch code generation request.
/// </summary>
public class BatchCodeGenResult
{
    /// <summary>
    /// Individual generation results, in the same order as the input prompts.
    /// </summary>
    public List<CodeGenResult> Results { get; set; } = new();

    /// <summary>
    /// Total number of prompts submitted.
    /// </summary>
    public int Total => Results.Count;
}
