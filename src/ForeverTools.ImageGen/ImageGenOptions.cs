namespace ForeverTools.ImageGen;

/// <summary>
/// Configuration options for the ImageGen client.
/// </summary>
public class ImageGenOptions
{
    /// <summary>
    /// The API key for AI/ML API. Get one at https://aimlapi.com
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The default model to use for image generation.
    /// Defaults to DALL-E 3.
    /// </summary>
    public string DefaultModel { get; set; } = ImageGenModels.DallE3;

    /// <summary>
    /// The default image size.
    /// Defaults to 1024x1024 square.
    /// </summary>
    public string DefaultSize { get; set; } = ImageSize.Square1024;

    /// <summary>
    /// The default image quality.
    /// Defaults to Standard.
    /// </summary>
    public ImageQuality DefaultQuality { get; set; } = ImageQuality.Standard;

    /// <summary>
    /// The default output format.
    /// Defaults to PNG.
    /// </summary>
    public OutputFormat DefaultFormat { get; set; } = OutputFormat.Png;

    /// <summary>
    /// Request timeout in seconds.
    /// Defaults to 120 seconds (image generation can be slow).
    /// </summary>
    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// The base URL for the API.
    /// Defaults to AI/ML API endpoint.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.aimlapi.com/v1";
}

/// <summary>
/// Image quality settings.
/// </summary>
public enum ImageQuality
{
    /// <summary>
    /// Standard quality - faster, lower cost.
    /// </summary>
    Standard,

    /// <summary>
    /// HD quality - more detail, higher cost.
    /// </summary>
    HD
}

/// <summary>
/// Output format for generated images.
/// </summary>
public enum OutputFormat
{
    /// <summary>
    /// PNG format - lossless, supports transparency.
    /// </summary>
    Png,

    /// <summary>
    /// JPEG format - smaller file size.
    /// </summary>
    Jpeg,

    /// <summary>
    /// WebP format - modern, efficient compression.
    /// </summary>
    WebP
}
