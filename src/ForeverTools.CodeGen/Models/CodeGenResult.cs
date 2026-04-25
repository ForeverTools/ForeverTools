namespace ForeverTools.CodeGen.Models;

/// <summary>
/// Result of a code generation request.
/// </summary>
public class CodeGenResult
{
    /// <summary>
    /// The generated code snippet.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// The programming language of the generated code.
    /// </summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// A brief explanation of what the code does.
    /// </summary>
    public string Explanation { get; set; } = string.Empty;

    /// <summary>
    /// Usage notes or caveats (e.g. dependencies required, known limitations).
    /// </summary>
    public string? Notes { get; set; }
}
