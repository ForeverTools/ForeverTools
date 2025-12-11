namespace ForeverTools.ImageGen.Models;

/// <summary>
/// Represents a generated image result.
/// </summary>
public class GeneratedImage
{
    /// <summary>
    /// The URL of the generated image (if returned as URL).
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// The base64-encoded image data (if returned as base64).
    /// </summary>
    public string? Base64Data { get; set; }

    /// <summary>
    /// The revised prompt used by the model (if available).
    /// Some models like DALL-E 3 may revise the prompt for better results.
    /// </summary>
    public string? RevisedPrompt { get; set; }

    /// <summary>
    /// The original prompt that was used.
    /// </summary>
    public string? OriginalPrompt { get; set; }

    /// <summary>
    /// The model that generated this image.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// The size of the generated image.
    /// </summary>
    public string? Size { get; set; }

    /// <summary>
    /// The style that was applied (if any).
    /// </summary>
    public ImageStyle? Style { get; set; }

    /// <summary>
    /// The index of this image in a batch generation.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Whether this image has data available.
    /// </summary>
    public bool HasData => !string.IsNullOrEmpty(Url) || !string.IsNullOrEmpty(Base64Data);

    /// <summary>
    /// Whether this image was returned as a URL.
    /// </summary>
    public bool IsUrl => !string.IsNullOrEmpty(Url);

    /// <summary>
    /// Whether this image was returned as base64 data.
    /// </summary>
    public bool IsBase64 => !string.IsNullOrEmpty(Base64Data);

    /// <summary>
    /// Gets the image data as bytes. Downloads from URL if necessary.
    /// </summary>
    public async Task<byte[]> GetBytesAsync(HttpClient? httpClient = null, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(Base64Data))
        {
            return Convert.FromBase64String(Base64Data);
        }

        if (!string.IsNullOrEmpty(Url))
        {
            var client = httpClient ?? new HttpClient();
            try
            {
                return await client.GetByteArrayAsync(Url
#if !NETSTANDARD2_0
                    , cancellationToken
#endif
                );
            }
            finally
            {
                if (httpClient == null)
                {
                    client.Dispose();
                }
            }
        }

        throw new InvalidOperationException("No image data available.");
    }

    /// <summary>
    /// Gets the image data as a base64 string. Downloads from URL if necessary.
    /// </summary>
    public async Task<string> GetBase64Async(HttpClient? httpClient = null, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(Base64Data))
        {
            return Base64Data!;
        }

        var bytes = await GetBytesAsync(httpClient, cancellationToken);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Saves the image to a file.
    /// </summary>
    public async Task SaveToFileAsync(string filePath, HttpClient? httpClient = null, CancellationToken cancellationToken = default)
    {
        var bytes = await GetBytesAsync(httpClient, cancellationToken);
#if NETSTANDARD2_0
        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await fs.WriteAsync(bytes, 0, bytes.Length);
#else
        await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);
#endif
    }
}

/// <summary>
/// Result of a batch image generation.
/// </summary>
public class BatchGenerationResult
{
    /// <summary>
    /// The generated images.
    /// </summary>
    public IReadOnlyList<GeneratedImage> Images { get; set; } = Array.Empty<GeneratedImage>();

    /// <summary>
    /// The original prompt used.
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// The model used.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Number of images generated.
    /// </summary>
    public int Count => Images.Count;

    /// <summary>
    /// Gets the first image in the batch.
    /// </summary>
    public GeneratedImage? First => Images.FirstOrDefault();
}

/// <summary>
/// Request options for image generation.
/// </summary>
public class ImageGenerationRequest
{
    /// <summary>
    /// The prompt describing the image to generate.
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// The model to use for generation.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// The size of the image to generate.
    /// </summary>
    public string? Size { get; set; }

    /// <summary>
    /// The style preset to apply.
    /// </summary>
    public ImageStyle Style { get; set; } = ImageStyle.None;

    /// <summary>
    /// The quality setting.
    /// </summary>
    public ImageQuality Quality { get; set; } = ImageQuality.Standard;

    /// <summary>
    /// Number of images to generate.
    /// </summary>
    public int Count { get; set; } = 1;

    /// <summary>
    /// Negative prompt - things to avoid in the image.
    /// </summary>
    public string? NegativePrompt { get; set; }

    /// <summary>
    /// Seed for reproducible generation (model-dependent).
    /// </summary>
    public int? Seed { get; set; }

    /// <summary>
    /// Guidance scale / CFG scale (model-dependent).
    /// Higher values = more prompt adherence.
    /// </summary>
    public double? GuidanceScale { get; set; }

    /// <summary>
    /// Number of inference steps (model-dependent).
    /// </summary>
    public int? Steps { get; set; }
}

/// <summary>
/// Request for image editing/inpainting.
/// </summary>
public class ImageEditRequest
{
    /// <summary>
    /// The original image to edit (as bytes).
    /// </summary>
    public byte[]? ImageBytes { get; set; }

    /// <summary>
    /// The original image URL.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// The prompt describing the desired edit.
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// The mask indicating areas to edit (for inpainting).
    /// White areas will be edited, black areas preserved.
    /// </summary>
    public byte[]? MaskBytes { get; set; }

    /// <summary>
    /// The model to use.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// The output size.
    /// </summary>
    public string? Size { get; set; }
}

/// <summary>
/// Request for creating image variations.
/// </summary>
public class ImageVariationRequest
{
    /// <summary>
    /// The source image (as bytes).
    /// </summary>
    public byte[]? ImageBytes { get; set; }

    /// <summary>
    /// The source image URL.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Optional prompt to guide the variation.
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// Number of variations to generate.
    /// </summary>
    public int Count { get; set; } = 1;

    /// <summary>
    /// The model to use.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// The output size.
    /// </summary>
    public string? Size { get; set; }

    /// <summary>
    /// Strength of variation (0.0-1.0). Lower = more similar to original.
    /// </summary>
    public double? Strength { get; set; }
}

/// <summary>
/// Request for image upscaling.
/// </summary>
public class ImageUpscaleRequest
{
    /// <summary>
    /// The image to upscale (as bytes).
    /// </summary>
    public byte[]? ImageBytes { get; set; }

    /// <summary>
    /// The image URL to upscale.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// The upscale factor (2x, 4x, etc.).
    /// </summary>
    public int Scale { get; set; } = 2;

    /// <summary>
    /// The model to use for upscaling.
    /// </summary>
    public string? Model { get; set; }
}
