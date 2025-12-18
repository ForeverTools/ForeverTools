namespace ForeverTools.ImageGen;

/// <summary>
/// Available image generation models.
/// </summary>
public static class ImageGenModels
{
    // ============================================
    // OpenAI DALL-E Models
    // ============================================

    /// <summary>
    /// DALL-E 3 - OpenAI's latest model. Best for balanced quality and creativity.
    /// Supports: 1024x1024, 1792x1024, 1024x1792
    /// </summary>
    public const string DallE3 = "dall-e-3";

    /// <summary>
    /// DALL-E 2 - OpenAI's previous generation. Faster and cheaper.
    /// Supports: 256x256, 512x512, 1024x1024
    /// </summary>
    public const string DallE2 = "dall-e-2";

    // ============================================
    // Flux Models (Black Forest Labs)
    // ============================================

    /// <summary>
    /// Flux Pro - Highest quality Flux model. Best for photorealistic images.
    /// </summary>
    public const string FluxPro = "flux-pro";

    /// <summary>
    /// Flux Pro 1.1 - Latest Flux Pro version with improved quality.
    /// </summary>
    public const string FluxPro11 = "flux-pro-1.1";

    /// <summary>
    /// Flux Dev - Development version. Good balance of speed and quality.
    /// </summary>
    public const string FluxDev = "flux-dev";

    /// <summary>
    /// Flux Schnell - Fastest Flux model. Great for rapid iteration.
    /// </summary>
    public const string FluxSchnell = "flux-schnell";

    /// <summary>
    /// Flux Realism - Optimized for photorealistic output.
    /// </summary>
    public const string FluxRealism = "flux-realism";

    // ============================================
    // Stable Diffusion Models
    // ============================================

    /// <summary>
    /// Stable Diffusion XL - Popular open model. Good for artistic styles.
    /// </summary>
    public const string StableDiffusionXL = "stable-diffusion-xl-1024-v1-0";

    /// <summary>
    /// Stable Diffusion 3 Medium - Latest SD3 medium variant.
    /// </summary>
    public const string StableDiffusion3 = "stable-diffusion-v3-medium";

    /// <summary>
    /// Stable Diffusion 3 Large - Highest quality SD3 model.
    /// </summary>
    public const string StableDiffusion3Large = "sd3-large";

    /// <summary>
    /// Stable Diffusion 3 Large Turbo - Fast SD3 large variant.
    /// </summary>
    public const string StableDiffusion3LargeTurbo = "sd3-large-turbo";

    // ============================================
    // Other Models
    // ============================================

    /// <summary>
    /// Ideogram - Great for text rendering in images.
    /// </summary>
    public const string Ideogram = "ideogram";

    /// <summary>
    /// Playground v2.5 - Community favorite for artistic generation.
    /// </summary>
    public const string PlaygroundV25 = "playground-v2.5";

    // ============================================
    // Recommendations by Use Case
    // ============================================

    /// <summary>
    /// Model recommendations for specific use cases.
    /// </summary>
    public static class Recommended
    {
        /// <summary>
        /// Best for photorealistic images (product photos, portraits).
        /// </summary>
        public const string Photorealistic = FluxPro;

        /// <summary>
        /// Best for artistic and creative images.
        /// </summary>
        public const string Artistic = StableDiffusion3;

        /// <summary>
        /// Best for fast iteration and prototyping.
        /// </summary>
        public const string Fast = FluxSchnell;

        /// <summary>
        /// Best balance of quality, speed, and cost.
        /// </summary>
        public const string Balanced = DallE3;

        /// <summary>
        /// Best for budget-conscious generation.
        /// </summary>
        public const string Budget = StableDiffusionXL;

        /// <summary>
        /// Best for images containing text/logos.
        /// </summary>
        public const string TextInImage = Ideogram;

        /// <summary>
        /// Best for social media content.
        /// </summary>
        public const string SocialMedia = DallE3;

        /// <summary>
        /// Best for marketing materials.
        /// </summary>
        public const string Marketing = FluxPro;

        /// <summary>
        /// Best for anime/illustration style.
        /// </summary>
        public const string Anime = StableDiffusionXL;

        /// <summary>
        /// Best for thumbnails (YouTube, blog posts).
        /// </summary>
        public const string Thumbnails = DallE3;
    }

    /// <summary>
    /// Get all available model IDs.
    /// </summary>
    public static IReadOnlyList<string> All => new[]
    {
        DallE3,
        DallE2,
        FluxPro,
        FluxPro11,
        FluxDev,
        FluxSchnell,
        FluxRealism,
        StableDiffusionXL,
        StableDiffusion3,
        StableDiffusion3Large,
        StableDiffusion3LargeTurbo,
        Ideogram,
        PlaygroundV25
    };
}
