using System.Text.Json.Serialization;

namespace ForeverTools.APILayer.Models;

/// <summary>
/// Base response wrapper for APILayer responses.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public class ApiLayerResponse<T>
{
    /// <summary>
    /// Whether the request was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The result data.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error information if the request failed.
    /// </summary>
    public ApiLayerError? Error { get; set; }

    /// <summary>
    /// Creates a successful response.
    /// </summary>
    public static ApiLayerResponse<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    /// <summary>
    /// Creates a failed response.
    /// </summary>
    public static ApiLayerResponse<T> Fail(string message, int code = 0) => new()
    {
        Success = false,
        Error = new ApiLayerError { Code = code, Message = message }
    };
}

/// <summary>
/// APILayer error response.
/// </summary>
public class ApiLayerError
{
    /// <summary>
    /// Error code.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// Error type.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Additional error info.
    /// </summary>
    [JsonPropertyName("info")]
    public string? Info { get; set; }
}

/// <summary>
/// Raw APILayer response with error handling.
/// </summary>
internal class RawApiResponse
{
    [JsonPropertyName("success")]
    public bool? Success { get; set; }

    [JsonPropertyName("error")]
    public ApiLayerError? Error { get; set; }
}
