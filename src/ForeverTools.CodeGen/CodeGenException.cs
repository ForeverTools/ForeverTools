namespace ForeverTools.CodeGen;

/// <summary>
/// Exception thrown when a code generation operation fails.
/// </summary>
public class CodeGenException : Exception
{
    /// <inheritdoc />
    public CodeGenException(string message) : base(message) { }

    /// <inheritdoc />
    public CodeGenException(string message, Exception inner) : base(message, inner) { }
}
