namespace ForeverTools.CodeGen.Models;

/// <summary>
/// Result of a code explanation request.
/// </summary>
public class ExplainResult
{
    /// <summary>
    /// High-level summary of what the code does.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Step-by-step walkthrough of the code logic.
    /// </summary>
    public List<string> Steps { get; set; } = new();

    /// <summary>
    /// The detected or inferred programming language.
    /// </summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Complexity estimate: simple, moderate, or complex.
    /// </summary>
    public string Complexity { get; set; } = string.Empty;
}
