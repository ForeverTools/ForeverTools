namespace ForeverTools.EmailAI;

/// <summary>
/// Exception thrown when an email AI operation fails.
/// </summary>
public class EmailAIException : Exception
{
    /// <inheritdoc />
    public EmailAIException(string message) : base(message) { }

    /// <inheritdoc />
    public EmailAIException(string message, Exception inner) : base(message, inner) { }
}
