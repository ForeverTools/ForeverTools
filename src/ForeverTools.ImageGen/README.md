# ForeverTools.ImageGen

AI-powered image generation for .NET with **social media presets**. Generate perfectly-sized images for Instagram, Twitter, YouTube, Facebook, LinkedIn, TikTok, and Pinterest using DALL-E 3, Stable Diffusion, Flux, and more.

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.ImageGen.svg)](https://www.nuget.org/packages/ForeverTools.ImageGen)

## Features

- **Social Media Presets** - One-click generation for Instagram, Twitter, YouTube, Facebook, LinkedIn, TikTok, Pinterest
- **Style Presets** - 20+ styles: Photorealistic, Anime, Cinematic, Minimalist, Corporate, and more
- **Multiple Models** - DALL-E 3, Stable Diffusion, Flux Pro, and 10+ other models
- **Image Editing** - Variations, inpainting, upscaling
- **Batch Generation** - Generate multiple images at once
- **Download Helpers** - Save images directly to files
- **ASP.NET Core Ready** - Built-in dependency injection
- **Multi-Target** - .NET 8, .NET 6, .NET Standard 2.0

## Installation

```bash
dotnet add package ForeverTools.ImageGen
```

## Quick Start

Get your API key at [aimlapi.com](https://aimlapi.com?via=forevertools) (400+ AI models, one API).

```csharp
using ForeverTools.ImageGen;

var client = new ImageGenClient("your-api-key");

// Generate an image
var image = await client.GenerateAsync("A sunset over mountains");
await client.SaveAsync(image, "sunset.png");
```

## Social Media Generation

Generate images perfectly sized for any platform:

```csharp
// Instagram post (1080x1080)
var insta = await client.GenerateForInstagramAsync("Coffee shop aesthetic");
await client.SaveAsync(insta, "instagram-post.png");

// Instagram Story/Reel (1080x1920)
var story = await client.GenerateForInstagramStoryAsync("Behind the scenes");

// YouTube thumbnail (1280x720)
var thumbnail = await client.GenerateForYouTubeAsync("SHOCKING Discovery!");

// Twitter/X post (1200x675)
var tweet = await client.GenerateForTwitterAsync("Breaking news graphic");

// LinkedIn post (1200x627)
var linkedin = await client.GenerateForLinkedInAsync("Professional headshot");

// Facebook post (1200x630)
var fb = await client.GenerateForFacebookAsync("Event announcement");

// Pinterest pin (1000x1500)
var pin = await client.GenerateForPinterestAsync("DIY home decor");

// TikTok cover (1080x1920)
var tiktok = await client.GenerateForTikTokAsync("Dance challenge thumbnail");

// Blog featured image (1200x600)
var blog = await client.GenerateForBlogAsync("Tech article header");

// Open Graph / social share (1200x630)
var og = await client.GenerateOpenGraphAsync("Website preview");
```

## Style Presets

Apply consistent styles to your images:

```csharp
// Photorealistic (product photos, portraits)
var photo = await client.GenerateAsync("Product on white background", ImageStyle.Photorealistic);

// Cinematic (YouTube thumbnails, dramatic content)
var cinematic = await client.GenerateAsync("Epic battle scene", ImageStyle.Cinematic);

// Anime/Manga style
var anime = await client.GenerateAsync("Warrior princess", ImageStyle.Anime);

// Corporate/Professional (LinkedIn, business)
var corporate = await client.GenerateAsync("Team meeting", ImageStyle.Corporate);

// Instagram Aesthetic (lifestyle, influencer)
var aesthetic = await client.GenerateAsync("Morning coffee routine", ImageStyle.InstagramAesthetic);

// Minimalist (clean, modern)
var minimal = await client.GenerateAsync("Logo concept", ImageStyle.Minimalist);

// More styles: DigitalArt, OilPainting, Watercolor, Sketch, Fantasy,
// SciFi, Neon, Vintage, Comic, PixelArt, FlatDesign, Isometric...
```

## Size Presets

All social media sizes built-in:

```csharp
// Using size constants directly
var image = await client.GenerateAsync("Mountain landscape", size: ImageSize.YouTubeThumbnail);

// Platform-specific presets
var size = ImageSize.Platforms.Instagram.Story;  // 1080x1920
var size = ImageSize.Platforms.Twitter.Post;     // 1200x675
var size = ImageSize.Platforms.YouTube.Thumbnail; // 1280x720
var size = ImageSize.Platforms.LinkedIn.Banner;   // 1584x396

// Ad sizes
var ad = await client.GenerateAsync("Product ad", size: ImageSize.AdMediumRectangle); // 300x250
var banner = await client.GenerateAsync("Sale banner", size: ImageSize.AdLeaderboard); // 728x90

// Custom sizes
var custom = await client.GenerateAsync("Custom image", size: ImageSize.Custom(800, 600));
```

## Available Models

```csharp
// OpenAI DALL-E
ImageGenModels.DallE3          // Best balanced quality
ImageGenModels.DallE2          // Faster, cheaper

// Flux (Black Forest Labs)
ImageGenModels.FluxPro         // Best photorealistic
ImageGenModels.FluxSchnell     // Fastest generation
ImageGenModels.FluxRealism     // Optimized for realism

// Stable Diffusion
ImageGenModels.StableDiffusion3      // Best artistic
ImageGenModels.StableDiffusionXL     // Great for anime

// Recommendations by use case
ImageGenModels.Recommended.Photorealistic  // FluxPro
ImageGenModels.Recommended.SocialMedia     // DallE3
ImageGenModels.Recommended.Thumbnails      // DallE3
ImageGenModels.Recommended.Marketing       // FluxPro
ImageGenModels.Recommended.Anime           // SDXL
ImageGenModels.Recommended.Fast            // FluxSchnell
ImageGenModels.Recommended.Budget          // SDXL
```

## Batch Generation

Generate multiple variations at once:

```csharp
// Generate 4 variations
var batch = await client.GenerateBatchAsync("Product photo", count: 4);

foreach (var image in batch.Images)
{
    await client.SaveAsync(image, $"variation-{image.Index}.png");
}

// With style
var animeBatch = await client.GenerateBatchAsync(
    "Character design",
    count: 4,
    ImageStyle.Anime
);
```

## Image Editing

### Create Variations

```csharp
// From existing image
var sourceBytes = File.ReadAllBytes("original.png");
var variation = await client.CreateVariationAsync(sourceBytes);

// With prompt guidance
var guided = await client.CreateVariationAsync(
    sourceBytes,
    prompt: "Make it more colorful"
);
```

### Inpainting (Edit Parts of Images)

```csharp
var imageBytes = File.ReadAllBytes("photo.png");
var maskBytes = File.ReadAllBytes("mask.png"); // White = edit, Black = keep

var edited = await client.InpaintAsync(
    imageBytes,
    maskBytes,
    prompt: "Replace with a cat"
);
```

### Upscaling

```csharp
var small = File.ReadAllBytes("small-image.png");
var upscaled = await client.UpscaleAsync(small, scale: 2);
```

## ASP.NET Core Integration

```csharp
// Program.cs
builder.Services.AddForeverToolsImageGen("your-api-key");

// Or with full configuration
builder.Services.AddForeverToolsImageGen(options =>
{
    options.ApiKey = "your-api-key";
    options.DefaultModel = ImageGenModels.DallE3;
    options.DefaultSize = ImageSize.Square1024;
    options.DefaultQuality = ImageQuality.HD;
    options.TimeoutSeconds = 120;
});

// Or from appsettings.json
builder.Services.AddForeverToolsImageGen(builder.Configuration);
```

```json
// appsettings.json
{
  "ImageGen": {
    "ApiKey": "your-api-key",
    "DefaultModel": "dall-e-3",
    "DefaultSize": "1024x1024",
    "DefaultQuality": "HD"
  }
}
```

```csharp
// Inject and use
public class ContentService
{
    private readonly ImageGenClient _imageGen;

    public ContentService(ImageGenClient imageGen)
    {
        _imageGen = imageGen;
    }

    public async Task<byte[]> GenerateThumbnailAsync(string title)
    {
        var image = await _imageGen.GenerateForYouTubeAsync(title, ImageStyle.Cinematic);
        return await _imageGen.ToBytesAsync(image);
    }
}
```

## Environment Variables

```csharp
// Uses AIML_API_KEY by default
var client = ImageGenClient.FromEnvironment();

// Or specify custom variable
var client = ImageGenClient.FromEnvironment("MY_IMAGE_API_KEY");
```

## Model Comparison

| Model | Best For | Speed | Quality | Cost |
|-------|----------|-------|---------|------|
| DALL-E 3 | General purpose, social media | Medium | High | $$ |
| Flux Pro | Photorealistic, marketing | Slow | Highest | $$$ |
| Flux Schnell | Fast iteration, prototyping | Fast | Good | $ |
| SD3 | Artistic, creative | Medium | High | $$ |
| SDXL | Anime, budget generation | Medium | Good | $ |

## Platform Size Reference

| Platform | Type | Size | Constant |
|----------|------|------|----------|
| Instagram | Square Post | 1080x1080 | `ImageSize.InstagramSquare` |
| Instagram | Portrait Post | 1080x1350 | `ImageSize.InstagramPortrait` |
| Instagram | Story/Reel | 1080x1920 | `ImageSize.InstagramStory` |
| Twitter/X | Post | 1200x675 | `ImageSize.TwitterPost` |
| Twitter/X | Header | 1500x500 | `ImageSize.TwitterHeader` |
| YouTube | Thumbnail | 1280x720 | `ImageSize.YouTubeThumbnail` |
| YouTube | Banner | 2560x1440 | `ImageSize.YouTubeBanner` |
| Facebook | Post | 1200x630 | `ImageSize.FacebookPost` |
| Facebook | Cover | 1640x856 | `ImageSize.FacebookCover` |
| LinkedIn | Post | 1200x627 | `ImageSize.LinkedInPost` |
| LinkedIn | Banner | 1584x396 | `ImageSize.LinkedInBanner` |
| TikTok | Cover | 1080x1920 | `ImageSize.TikTok` |
| Pinterest | Pin | 1000x1500 | `ImageSize.PinterestPin` |

## Use Cases

### Social Media Marketing
```csharp
// Generate a week's worth of Instagram content
var prompts = new[] { "Monday motivation", "Product highlight", "Behind the scenes" };
foreach (var prompt in prompts)
{
    var image = await client.GenerateForInstagramAsync(prompt, ImageStyle.InstagramAesthetic);
    await client.SaveAsync(image, $"{prompt.Replace(" ", "-")}.png");
}
```

### YouTube Content Creation
```csharp
// Generate eye-catching thumbnails
var thumbnail = await client.GenerateForYouTubeAsync(
    "SHOCKED face discovering hidden feature",
    ImageStyle.Cinematic,
    ImageGenModels.Recommended.Thumbnails
);
```

### E-commerce Product Images
```csharp
// Generate product mockups
var product = await client.GenerateAsync(
    "White sneakers on marble surface, studio lighting",
    ImageStyle.Photorealistic,
    ImageSize.Square1024,
    ImageGenModels.FluxPro
);
```

### Blog & Website Content
```csharp
// Generate featured images
var featured = await client.GenerateForBlogAsync(
    "Abstract technology concept with blue tones",
    ImageStyle.DigitalArt
);

// Generate Open Graph images for social sharing
var og = await client.GenerateOpenGraphAsync(
    "Article preview with title overlay space"
);
```

## Why AI/ML API?

[AI/ML API](https://aimlapi.com?via=forevertools) provides access to 400+ AI models through a single API:

- **One API key** for DALL-E, Stable Diffusion, Flux, and more
- **Competitive pricing** - Pay only for what you use
- **High availability** - 99.9% uptime
- **Fast inference** - Optimized infrastructure
- **No vendor lock-in** - Switch models anytime

## Other ForeverTools Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.APILayer** | IP geolocation, currency exchange, phone & email validation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.APILayer.svg)](https://www.nuget.org/packages/ForeverTools.APILayer) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.OCR** | AI-powered OCR using GPT-4 Vision, Claude 3, and Gemini | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.OCR.svg)](https://www.nuget.org/packages/ForeverTools.OCR) |
| **ForeverTools.Postmark** | Transactional email sending with templates and tracking | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation and CAPTCHA bypass | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |

## Need Proxies?

For standalone proxy access, check out [BrightData](https://get.brightdata.com/ForeverToolsResidentialProxies) - the industry leader:
- [Residential Proxies](https://get.brightdata.com/ForeverToolsResidentialProxies) - General scraping, geo-targeting
- [Social Media Proxies](https://get.brightdata.com/ForeverToolsSocialProxies) - Instagram, Facebook, TikTok automation
- [ISP Proxies](https://get.brightdata.com/ForeverToolsISP) - High-speed, stable connections
- [SERP API](https://get.brightdata.com/ForeverToolsSerp) - Search engine scraping

## Requirements

- .NET 8.0, .NET 6.0, or .NET Standard 2.0 compatible framework
- AI/ML API key ([get one here](https://aimlapi.com?via=forevertools))

## License

MIT License - see [LICENSE](LICENSE) for details.

## Links

- [Get API Key](https://aimlapi.com?via=forevertools)
- [GitHub Repository](https://github.com/ForeverTools/ForeverTools)
- [NuGet Package](https://www.nuget.org/packages/ForeverTools.ImageGen)
