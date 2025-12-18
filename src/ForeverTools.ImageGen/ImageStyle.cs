namespace ForeverTools.ImageGen;

/// <summary>
/// Pre-defined style presets for image generation.
/// These modify the prompt to achieve consistent styles.
/// </summary>
public enum ImageStyle
{
    /// <summary>
    /// No style modification - use prompt as-is.
    /// </summary>
    None,

    /// <summary>
    /// Natural, realistic style without heavy stylization.
    /// </summary>
    Natural,

    /// <summary>
    /// Vivid, colorful, dramatic style.
    /// </summary>
    Vivid,

    /// <summary>
    /// Photorealistic style - looks like a real photograph.
    /// Best for: Product photos, portraits, marketing materials.
    /// </summary>
    Photorealistic,

    /// <summary>
    /// Digital art illustration style.
    /// Best for: Blog headers, social media graphics.
    /// </summary>
    DigitalArt,

    /// <summary>
    /// Japanese anime/manga style.
    /// Best for: Character art, gaming content.
    /// </summary>
    Anime,

    /// <summary>
    /// 3D rendered style.
    /// Best for: Product mockups, tech visuals.
    /// </summary>
    ThreeD,

    /// <summary>
    /// Oil painting artistic style.
    /// Best for: Artistic content, elegant visuals.
    /// </summary>
    OilPainting,

    /// <summary>
    /// Watercolor painting style.
    /// Best for: Soft, artistic content.
    /// </summary>
    Watercolor,

    /// <summary>
    /// Pencil sketch style.
    /// Best for: Concept art, drafts.
    /// </summary>
    Sketch,

    /// <summary>
    /// Cinematic movie-like style with dramatic lighting.
    /// Best for: YouTube thumbnails, dramatic content.
    /// </summary>
    Cinematic,

    /// <summary>
    /// Fantasy art style.
    /// Best for: Gaming, fantasy content.
    /// </summary>
    Fantasy,

    /// <summary>
    /// Science fiction style.
    /// Best for: Tech content, futuristic themes.
    /// </summary>
    SciFi,

    /// <summary>
    /// Clean, minimalist style.
    /// Best for: Professional content, UI mockups.
    /// </summary>
    Minimalist,

    /// <summary>
    /// Retro/vintage style.
    /// Best for: Nostalgic content, retro branding.
    /// </summary>
    Vintage,

    /// <summary>
    /// Neon/cyberpunk style with glowing colors.
    /// Best for: Tech content, gaming, nightlife.
    /// </summary>
    Neon,

    /// <summary>
    /// Abstract art style.
    /// Best for: Backgrounds, artistic content.
    /// </summary>
    Abstract,

    /// <summary>
    /// Comic book style.
    /// Best for: Fun content, storytelling.
    /// </summary>
    Comic,

    /// <summary>
    /// Pixel art retro gaming style.
    /// Best for: Gaming content, retro themes.
    /// </summary>
    PixelArt,

    /// <summary>
    /// Professional corporate style.
    /// Best for: Business content, LinkedIn, presentations.
    /// </summary>
    Corporate,

    /// <summary>
    /// Cartoon style.
    /// Best for: Kids content, fun marketing.
    /// </summary>
    Cartoon,

    /// <summary>
    /// Instagram-ready aesthetic style.
    /// Best for: Social media content, lifestyle brands.
    /// </summary>
    InstagramAesthetic,

    /// <summary>
    /// Flat design style.
    /// Best for: Icons, infographics, modern UI.
    /// </summary>
    FlatDesign,

    /// <summary>
    /// Isometric illustration style.
    /// Best for: Tech content, explainers, infographics.
    /// </summary>
    Isometric
}

/// <summary>
/// Helper methods for ImageStyle.
/// </summary>
public static class ImageStyleExtensions
{
    /// <summary>
    /// Gets the style prompt modifier for the given style.
    /// </summary>
    public static string GetStylePrompt(this ImageStyle style) => style switch
    {
        ImageStyle.None => "",
        ImageStyle.Natural => ", natural lighting, realistic",
        ImageStyle.Vivid => ", vivid colors, dramatic, high contrast",
        ImageStyle.Photorealistic => ", photorealistic, ultra realistic, 8k, professional photography, detailed",
        ImageStyle.DigitalArt => ", digital art, illustration, vibrant colors, detailed",
        ImageStyle.Anime => ", anime style, manga art, Japanese animation, cel shaded",
        ImageStyle.ThreeD => ", 3D render, octane render, cinema 4d, blender, realistic lighting",
        ImageStyle.OilPainting => ", oil painting, classical art, brushstrokes, canvas texture",
        ImageStyle.Watercolor => ", watercolor painting, soft edges, artistic, delicate",
        ImageStyle.Sketch => ", pencil sketch, hand drawn, black and white, detailed lines",
        ImageStyle.Cinematic => ", cinematic, movie scene, dramatic lighting, film grain, 35mm",
        ImageStyle.Fantasy => ", fantasy art, magical, ethereal, epic, detailed",
        ImageStyle.SciFi => ", science fiction, futuristic, high tech, cyberpunk",
        ImageStyle.Minimalist => ", minimalist, clean, simple, modern, white space",
        ImageStyle.Vintage => ", vintage, retro, nostalgic, film photography, warm tones",
        ImageStyle.Neon => ", neon lights, cyberpunk, glowing, vibrant colors, night scene",
        ImageStyle.Abstract => ", abstract art, modern art, geometric, artistic",
        ImageStyle.Comic => ", comic book style, bold lines, halftone dots, dynamic",
        ImageStyle.PixelArt => ", pixel art, 8-bit, retro gaming, pixelated",
        ImageStyle.Corporate => ", professional, corporate, clean, business, modern",
        ImageStyle.Cartoon => ", cartoon style, animated, colorful, fun, whimsical",
        ImageStyle.InstagramAesthetic => ", instagram aesthetic, trendy, lifestyle, warm tones, golden hour",
        ImageStyle.FlatDesign => ", flat design, vector art, simple shapes, bold colors",
        ImageStyle.Isometric => ", isometric illustration, 3d isometric, technical, detailed",
        _ => ""
    };

    /// <summary>
    /// Gets the recommended model for this style.
    /// </summary>
    public static string GetRecommendedModel(this ImageStyle style) => style switch
    {
        ImageStyle.Photorealistic => ImageGenModels.FluxPro,
        ImageStyle.Anime => ImageGenModels.StableDiffusionXL,
        ImageStyle.DigitalArt => ImageGenModels.StableDiffusion3,
        ImageStyle.ThreeD => ImageGenModels.FluxPro,
        ImageStyle.Cinematic => ImageGenModels.FluxPro,
        ImageStyle.Fantasy => ImageGenModels.StableDiffusion3,
        ImageStyle.PixelArt => ImageGenModels.StableDiffusionXL,
        ImageStyle.Comic => ImageGenModels.StableDiffusionXL,
        _ => ImageGenModels.DallE3
    };
}
