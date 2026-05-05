namespace ForeverTools.BrightData;

/// <summary>Exception thrown when a BrightData API call fails.</summary>
public class BrightDataException : Exception
{
    /// <inheritdoc/>
    public BrightDataException(string message) : base(message) { }

    /// <inheritdoc/>
    public BrightDataException(string message, Exception innerException) : base(message, innerException) { }
}
