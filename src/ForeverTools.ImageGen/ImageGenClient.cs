using System.ClientModel;
using ForeverTools.ImageGen.Models;
using OpenAI;
using OpenAI.Images;

namespace ForeverTools.ImageGen;

/// <summary>
/// Client for AI-powered image generation with social media presets.
/// Supports DALL-E 3, Stable Diffusion, Flux, and more through AI/ML API.
/// </summary>
public class ImageGenClient : IDisposable
{
    private readonly ImageClient _imageClient;
    private readonly HttpClient _httpClient;
    private readonly ImageGenOptions _options;
    private readonly bool _disposeHttpClient;

    /// <summary>
    /// Creates a new ImageGenClient with the specified API key.
    /// Get your API key at https://aimlapi.com
    /// </summary>
    /// <param name="apiKey">Your AI/ML API key.</param>
    /// <param name="model">The default model to use. Defaults to DALL-E 3.</param>
    public ImageGenClient(string apiKey, string? model = null)
        : this(new ImageGenOptions { ApiKey = apiKey, DefaultModel = model ?? ImageGenModels.DallE3 })
    {
    }

    /// <summary>
    /// Creates a new ImageGenClient with the specified options.
    /// </summary>
    public ImageGenClient(ImageGenOptions options)
        : this(options, null)
    {
    }

    /// <summary>
    /// Creates a new ImageGenClient with the specified options and HttpClient.
    /// </summary>
    public ImageGenClient(ImageGenOptions options, HttpClient? httpClient)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new ArgumentException("API key is required.", nameof(options));
        }

        _httpClient = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds) };
        _disposeHttpClient = httpClient == null;

        var credential = new ApiKeyCredential(options.ApiKey);
        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(options.BaseUrl)
        };

        var openAiClient = new OpenAIClient(credential, clientOptions);
        _imageClient = openAiClient.GetImageClient(options.DefaultModel);
    }

    /// <summary>
    /// Creates a client from environment variable.
    /// Uses AIML_API_KEY by default.
    /// </summary>
    public static ImageGenClient FromEnvironment(string variableName = "AIML_API_KEY")
    {
        var apiKey = Environment.GetEnvironmentVariable(variableName)
            ?? throw new InvalidOperationException($"Environment variable '{variableName}' not found.");
        return new ImageGenClient(apiKey);
    }

    #region Basic Generation

    /// <summary>
    /// Generates an image from a text prompt.
    /// </summary>
    /// <param name="prompt">The text description of the image to generate.</param>
    /// <param name="size">The image size. Defaults to 1024x1024.</param>
    /// <param name="model">The model to use. Defaults to DALL-E 3.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated image.</returns>
    public async Task<Models.GeneratedImage> GenerateAsync(
        string prompt,
        string? size = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return await GenerateAsync(prompt, ImageStyle.None, size, model, cancellationToken);
    }

    /// <summary>
    /// Generates an image with a style preset applied.
    /// </summary>
    /// <param name="prompt">The text description of the image to generate.</param>
    /// <param name="style">The style preset to apply.</param>
    /// <param name="size">The image size. Defaults to 1024x1024.</param>
    /// <param name="model">The model to use. Uses style-recommended model if not specified.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated image.</returns>
    public async Task<Models.GeneratedImage> GenerateAsync(
        string prompt,
        ImageStyle style,
        string? size = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt is required.", nameof(prompt));
        }

        var effectiveModel = model ?? (style != ImageStyle.None ? style.GetRecommendedModel() : _options.DefaultModel);
        var effectiveSize = size ?? _options.DefaultSize;
        var effectivePrompt = ApplyStyle(prompt, style);

        var imageClient = GetClientForModel(effectiveModel);
        var options = CreateGenerationOptions(effectiveSize, 1);

        var result = await imageClient.GenerateImageAsync(effectivePrompt, options, cancellationToken);

        return CreateGeneratedImageResult(result.Value, effectivePrompt, prompt, effectiveModel, effectiveSize, style, 0);
    }

    /// <summary>
    /// Generates an image using a full request object with all options.
    /// </summary>
    public async Task<Models.GeneratedImage> GenerateAsync(
        ImageGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            throw new ArgumentException("Prompt is required.", nameof(request));
        }

        var images = await GenerateBatchAsync(request, cancellationToken);
        return images.Images.First();
    }

    #endregion

    #region Batch Generation

    /// <summary>
    /// Generates multiple images from a single prompt.
    /// </summary>
    /// <param name="prompt">The text description of the images to generate.</param>
    /// <param name="count">Number of images to generate (1-10).</param>
    /// <param name="size">The image size.</param>
    /// <param name="model">The model to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of generated images.</returns>
    public async Task<BatchGenerationResult> GenerateBatchAsync(
        string prompt,
        int count,
        string? size = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return await GenerateBatchAsync(prompt, count, ImageStyle.None, size, model, cancellationToken);
    }

    /// <summary>
    /// Generates multiple images with a style preset.
    /// </summary>
    public async Task<BatchGenerationResult> GenerateBatchAsync(
        string prompt,
        int count,
        ImageStyle style,
        string? size = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        var request = new ImageGenerationRequest
        {
            Prompt = prompt,
            Count = count,
            Style = style,
            Size = size,
            Model = model
        };
        return await GenerateBatchAsync(request, cancellationToken);
    }

    /// <summary>
    /// Generates multiple images using a full request object.
    /// </summary>
    public async Task<BatchGenerationResult> GenerateBatchAsync(
        ImageGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            throw new ArgumentException("Prompt is required.", nameof(request));
        }

        var count = Math.Max(1, Math.Min(10, request.Count));
        var effectiveModel = request.Model ?? (request.Style != ImageStyle.None ? request.Style.GetRecommendedModel() : _options.DefaultModel);
        var effectiveSize = request.Size ?? _options.DefaultSize;
        var effectivePrompt = ApplyStyle(request.Prompt, request.Style);

        var imageClient = GetClientForModel(effectiveModel);
        var options = CreateGenerationOptions(effectiveSize, count);

        var result = await imageClient.GenerateImagesAsync(effectivePrompt, count, options, cancellationToken);

        var images = new List<Models.GeneratedImage>();
        var index = 0;
        foreach (var image in result.Value)
        {
            images.Add(CreateGeneratedImageResult(image, effectivePrompt, request.Prompt, effectiveModel, effectiveSize, request.Style, index++));
        }

        return new BatchGenerationResult
        {
            Images = images,
            Prompt = request.Prompt,
            Model = effectiveModel
        };
    }

    #endregion

    #region Social Media Helpers

    /// <summary>
    /// Generates an Instagram square post image (1080x1080).
    /// </summary>
    public Task<Models.GeneratedImage> GenerateForInstagramAsync(
        string prompt,
        ImageStyle style = ImageStyle.InstagramAesthetic,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return GenerateAsync(prompt, style, ImageSize.InstagramSquare, model, cancellationToken);
    }

    /// <summary>
    /// Generates an Instagram Story/Reel image (1080x1920).
    /// </summary>
    public Task<Models.GeneratedImage> GenerateForInstagramStoryAsync(
        string prompt,
        ImageStyle style = ImageStyle.InstagramAesthetic,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return GenerateAsync(prompt, style, ImageSize.InstagramStory, model, cancellationToken);
    }

    /// <summary>
    /// Generates a Twitter/X post image (1200x675).
    /// </summary>
    public Task<Models.GeneratedImage> GenerateForTwitterAsync(
        string prompt,
        ImageStyle style = ImageStyle.None,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return GenerateAsync(prompt, style, ImageSize.TwitterPost, model, cancellationToken);
    }

    /// <summary>
    /// Generates a YouTube thumbnail (1280x720).
    /// </summary>
    public Task<Models.GeneratedImage> GenerateForYouTubeAsync(
        string prompt,
        ImageStyle style = ImageStyle.Cinematic,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return GenerateAsync(prompt, style, ImageSize.YouTubeThumbnail, model, cancellationToken);
    }

    /// <summary>
    /// Generates a Facebook post image (1200x630).
    /// </summary>
    public Task<Models.GeneratedImage> GenerateForFacebookAsync(
        string prompt,
        ImageStyle style = ImageStyle.None,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return GenerateAsync(prompt, style, ImageSize.FacebookPost, model, cancellationToken);
    }

    /// <summary>
    /// Generates a LinkedIn post image (1200x627).
    /// </summary>
    public Task<Models.GeneratedImage> GenerateForLinkedInAsync(
        string prompt,
        ImageStyle style = ImageStyle.Corporate,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return GenerateAsync(prompt, style, ImageSize.LinkedInPost, model, cancellationToken);
    }

    /// <summary>
    /// Generates a Pinterest pin image (1000x1500).
    /// </summary>
    public Task<Models.GeneratedImage> GenerateForPinterestAsync(
        string prompt,
        ImageStyle style = ImageStyle.None,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return GenerateAsync(prompt, style, ImageSize.PinterestPin, model, cancellationToken);
    }

    /// <summary>
    /// Generates a TikTok cover/content image (1080x1920).
    /// </summary>
    public Task<Models.GeneratedImage> GenerateForTikTokAsync(
        string prompt,
        ImageStyle style = ImageStyle.Vivid,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return GenerateAsync(prompt, style, ImageSize.TikTok, model, cancellationToken);
    }

    /// <summary>
    /// Generates an Open Graph / social share image (1200x630).
    /// </summary>
    public Task<Models.GeneratedImage> GenerateOpenGraphAsync(
        string prompt,
        ImageStyle style = ImageStyle.None,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return GenerateAsync(prompt, style, ImageSize.OpenGraph, model, cancellationToken);
    }

    /// <summary>
    /// Generates a blog featured image (1200x600).
    /// </summary>
    public Task<Models.GeneratedImage> GenerateForBlogAsync(
        string prompt,
        ImageStyle style = ImageStyle.None,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return GenerateAsync(prompt, style, ImageSize.BlogFeatured, model, cancellationToken);
    }

    #endregion

    #region Image Editing (Future Enhancement)

    /// <summary>
    /// Creates a variation of an existing image.
    /// Note: This feature depends on model support and may be enhanced in future versions.
    /// </summary>
    public Task<Models.GeneratedImage> CreateVariationAsync(
        byte[] imageBytes,
        string? prompt = null,
        string? size = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        if (imageBytes == null || imageBytes.Length == 0)
        {
            throw new ArgumentException("Image bytes are required.", nameof(imageBytes));
        }

        // For v1.0, return a placeholder indicating feature is limited
        // Full variation support will be added in future versions
        throw new NotSupportedException("Image variations require DALL-E 2 which has limited API support. Use text-to-image generation with a descriptive prompt instead.");
    }

    /// <summary>
    /// Creates a variation of an existing image from a stream.
    /// </summary>
    public Task<Models.GeneratedImage> CreateVariationAsync(
        Stream imageStream,
        string? prompt = null,
        string? size = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Image variations require DALL-E 2 which has limited API support. Use text-to-image generation with a descriptive prompt instead.");
    }

    /// <summary>
    /// Edits an image by replacing areas defined by a mask (inpainting).
    /// Note: This feature depends on model support and may be enhanced in future versions.
    /// </summary>
    public Task<Models.GeneratedImage> InpaintAsync(
        byte[] imageBytes,
        byte[] maskBytes,
        string prompt,
        string? size = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Inpainting requires specific model support. This feature will be enhanced in future versions.");
    }

    /// <summary>
    /// Upscales an image to a higher resolution.
    /// Note: This feature depends on model support and may be enhanced in future versions.
    /// </summary>
    public Task<Models.GeneratedImage> UpscaleAsync(
        byte[] imageBytes,
        int scale = 2,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Image upscaling requires specific model support. This feature will be enhanced in future versions.");
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Downloads an image from a URL to bytes.
    /// </summary>
    public async Task<byte[]> DownloadImageAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL is required.", nameof(url));
        }

        return await _httpClient.GetByteArrayAsync(url
#if !NETSTANDARD2_0
            , cancellationToken
#endif
        );
    }

    /// <summary>
    /// Saves a generated image to a file.
    /// </summary>
    public async Task SaveAsync(Models.GeneratedImage image, string filePath, CancellationToken cancellationToken = default)
    {
        if (image == null) throw new ArgumentNullException(nameof(image));
        await image.SaveToFileAsync(filePath, _httpClient, cancellationToken);
    }

    /// <summary>
    /// Converts a generated image to base64.
    /// </summary>
    public async Task<string> ToBase64Async(Models.GeneratedImage image, CancellationToken cancellationToken = default)
    {
        if (image == null) throw new ArgumentNullException(nameof(image));
        return await image.GetBase64Async(_httpClient, cancellationToken);
    }

    /// <summary>
    /// Gets the bytes of a generated image.
    /// </summary>
    public async Task<byte[]> ToBytesAsync(Models.GeneratedImage image, CancellationToken cancellationToken = default)
    {
        if (image == null) throw new ArgumentNullException(nameof(image));
        return await image.GetBytesAsync(_httpClient, cancellationToken);
    }

    #endregion

    #region Private Methods

    private ImageClient GetClientForModel(string model)
    {
        var credential = new ApiKeyCredential(_options.ApiKey);
        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(_options.BaseUrl)
        };
        var openAiClient = new OpenAIClient(credential, clientOptions);
        return openAiClient.GetImageClient(model);
    }

    private static string ApplyStyle(string prompt, ImageStyle style)
    {
        if (style == ImageStyle.None)
        {
            return prompt;
        }

        var stylePrompt = style.GetStylePrompt();
        return prompt + stylePrompt;
    }

    private ImageGenerationOptions CreateGenerationOptions(string size, int count)
    {
        var options = new ImageGenerationOptions
        {
            Size = ParseGeneratedImageSize(size),
            Quality = _options.DefaultQuality == ImageQuality.HD
                ? GeneratedImageQuality.High
                : GeneratedImageQuality.Standard,
            ResponseFormat = GeneratedImageFormat.Uri
        };

        return options;
    }

    private static GeneratedImageSize? ParseGeneratedImageSize(string size)
    {
        return size switch
        {
            "256x256" => GeneratedImageSize.W256xH256,
            "512x512" => GeneratedImageSize.W512xH512,
            "1024x1024" => GeneratedImageSize.W1024xH1024,
            "1792x1024" => GeneratedImageSize.W1792xH1024,
            "1024x1792" => GeneratedImageSize.W1024xH1792,
            _ => GeneratedImageSize.W1024xH1024 // Default to 1024x1024 for unsupported sizes
        };
    }

    private static Models.GeneratedImage CreateGeneratedImageResult(
        OpenAI.Images.GeneratedImage openAiImage,
        string effectivePrompt,
        string? originalPrompt,
        string model,
        string size,
        ImageStyle? style,
        int index)
    {
        return new Models.GeneratedImage
        {
            Url = openAiImage.ImageUri?.ToString(),
            Base64Data = openAiImage.ImageBytes != null ? Convert.ToBase64String(openAiImage.ImageBytes.ToArray()) : null,
            RevisedPrompt = openAiImage.RevisedPrompt,
            OriginalPrompt = originalPrompt,
            Model = model,
            Size = size,
            Style = style,
            Index = index
        };
    }

    /// <summary>
    /// Disposes the client and releases resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    #endregion
}
