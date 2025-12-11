using ForeverTools.ImageGen;
using ForeverTools.ImageGen.Extensions;
using ForeverTools.ImageGen.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ForeverTools.ImageGen.Tests;

public class ImageGenClientTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithApiKey_CreatesClient()
    {
        var client = new ImageGenClient("test-api-key");
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithApiKeyAndModel_CreatesClient()
    {
        var client = new ImageGenClient("test-api-key", ImageGenModels.FluxPro);
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithOptions_CreatesClient()
    {
        var options = new ImageGenOptions
        {
            ApiKey = "test-api-key",
            DefaultModel = ImageGenModels.FluxPro,
            DefaultSize = ImageSize.Square1024,
            DefaultQuality = ImageQuality.HD
        };

        var client = new ImageGenClient(options);
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithNullApiKey_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new ImageGenClient(string.Empty));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new ImageGenClient((ImageGenOptions)null!));
    }

    [Fact]
    public void Constructor_WithEmptyApiKeyInOptions_ThrowsException()
    {
        var options = new ImageGenOptions { ApiKey = string.Empty };
        Assert.Throws<ArgumentException>(() => new ImageGenClient(options));
    }

    #endregion

    #region Options Tests

    [Fact]
    public void ImageGenOptions_HasCorrectDefaults()
    {
        var options = new ImageGenOptions();

        Assert.Equal(string.Empty, options.ApiKey);
        Assert.Equal(ImageGenModels.DallE3, options.DefaultModel);
        Assert.Equal(ImageSize.Square1024, options.DefaultSize);
        Assert.Equal(ImageQuality.Standard, options.DefaultQuality);
        Assert.Equal(OutputFormat.Png, options.DefaultFormat);
        Assert.Equal(120, options.TimeoutSeconds);
        Assert.Equal("https://api.aimlapi.com/v1", options.BaseUrl);
    }

    #endregion

    #region Model Constants Tests

    [Fact]
    public void ImageGenModels_ContainsExpectedModels()
    {
        Assert.Equal("dall-e-3", ImageGenModels.DallE3);
        Assert.Equal("dall-e-2", ImageGenModels.DallE2);
        Assert.Equal("flux-pro", ImageGenModels.FluxPro);
        Assert.Equal("flux-schnell", ImageGenModels.FluxSchnell);
        Assert.Equal("stable-diffusion-xl-1024-v1-0", ImageGenModels.StableDiffusionXL);
        Assert.Equal("stable-diffusion-v3-medium", ImageGenModels.StableDiffusion3);
    }

    [Fact]
    public void ImageGenModels_Recommended_ReturnsCorrectModels()
    {
        Assert.Equal(ImageGenModels.FluxPro, ImageGenModels.Recommended.Photorealistic);
        Assert.Equal(ImageGenModels.StableDiffusion3, ImageGenModels.Recommended.Artistic);
        Assert.Equal(ImageGenModels.FluxSchnell, ImageGenModels.Recommended.Fast);
        Assert.Equal(ImageGenModels.DallE3, ImageGenModels.Recommended.Balanced);
        Assert.Equal(ImageGenModels.StableDiffusionXL, ImageGenModels.Recommended.Budget);
        Assert.Equal(ImageGenModels.DallE3, ImageGenModels.Recommended.SocialMedia);
        Assert.Equal(ImageGenModels.DallE3, ImageGenModels.Recommended.Thumbnails);
    }

    [Fact]
    public void ImageGenModels_All_ContainsAllModels()
    {
        var all = ImageGenModels.All;

        Assert.Contains(ImageGenModels.DallE3, all);
        Assert.Contains(ImageGenModels.DallE2, all);
        Assert.Contains(ImageGenModels.FluxPro, all);
        Assert.Contains(ImageGenModels.FluxSchnell, all);
        Assert.Contains(ImageGenModels.StableDiffusionXL, all);
        Assert.True(all.Count >= 10);
    }

    #endregion

    #region Image Size Tests

    [Fact]
    public void ImageSize_StandardSizes_AreCorrect()
    {
        Assert.Equal("512x512", ImageSize.Square512);
        Assert.Equal("1024x1024", ImageSize.Square1024);
        Assert.Equal("2048x2048", ImageSize.Square2048);
        Assert.Equal("1792x1024", ImageSize.Landscape1792x1024);
        Assert.Equal("1024x1792", ImageSize.Portrait1024x1792);
    }

    [Fact]
    public void ImageSize_InstagramSizes_AreCorrect()
    {
        Assert.Equal("1080x1080", ImageSize.InstagramSquare);
        Assert.Equal("1080x1350", ImageSize.InstagramPortrait);
        Assert.Equal("1080x566", ImageSize.InstagramLandscape);
        Assert.Equal("1080x1920", ImageSize.InstagramStory);
        Assert.Equal("1080x1920", ImageSize.InstagramReel);
    }

    [Fact]
    public void ImageSize_TwitterSizes_AreCorrect()
    {
        Assert.Equal("1200x675", ImageSize.TwitterPost);
        Assert.Equal("1500x500", ImageSize.TwitterHeader);
        Assert.Equal("400x400", ImageSize.TwitterProfile);
        Assert.Equal("1200x628", ImageSize.TwitterCard);
    }

    [Fact]
    public void ImageSize_YouTubeSizes_AreCorrect()
    {
        Assert.Equal("1280x720", ImageSize.YouTubeThumbnail);
        Assert.Equal("2560x1440", ImageSize.YouTubeBanner);
        Assert.Equal("800x800", ImageSize.YouTubeProfile);
    }

    [Fact]
    public void ImageSize_FacebookSizes_AreCorrect()
    {
        Assert.Equal("1200x630", ImageSize.FacebookPost);
        Assert.Equal("1640x856", ImageSize.FacebookCover);
        Assert.Equal("1200x628", ImageSize.FacebookAd);
        Assert.Equal("1080x1920", ImageSize.FacebookStory);
    }

    [Fact]
    public void ImageSize_LinkedInSizes_AreCorrect()
    {
        Assert.Equal("1200x627", ImageSize.LinkedInPost);
        Assert.Equal("1584x396", ImageSize.LinkedInBanner);
        Assert.Equal("400x400", ImageSize.LinkedInProfile);
    }

    [Fact]
    public void ImageSize_OtherPlatformSizes_AreCorrect()
    {
        Assert.Equal("1080x1920", ImageSize.TikTok);
        Assert.Equal("1000x1500", ImageSize.PinterestPin);
        Assert.Equal("1200x630", ImageSize.OpenGraph);
        Assert.Equal("1200x600", ImageSize.BlogFeatured);
    }

    [Fact]
    public void ImageSize_AdSizes_AreCorrect()
    {
        Assert.Equal("300x250", ImageSize.AdMediumRectangle);
        Assert.Equal("728x90", ImageSize.AdLeaderboard);
        Assert.Equal("160x600", ImageSize.AdSkyscraper);
        Assert.Equal("320x50", ImageSize.AdMobileBanner);
    }

    [Fact]
    public void ImageSize_Platforms_MatchMainConstants()
    {
        Assert.Equal(ImageSize.InstagramSquare, ImageSize.Platforms.Instagram.Square);
        Assert.Equal(ImageSize.InstagramStory, ImageSize.Platforms.Instagram.Story);
        Assert.Equal(ImageSize.TwitterPost, ImageSize.Platforms.Twitter.Post);
        Assert.Equal(ImageSize.YouTubeThumbnail, ImageSize.Platforms.YouTube.Thumbnail);
        Assert.Equal(ImageSize.LinkedInPost, ImageSize.Platforms.LinkedIn.Post);
    }

    [Fact]
    public void ImageSize_Parse_ValidSize_ReturnsCorrectDimensions()
    {
        var (width, height) = ImageSize.Parse("1920x1080");
        Assert.Equal(1920, width);
        Assert.Equal(1080, height);
    }

    [Fact]
    public void ImageSize_Parse_InstagramSize_ReturnsCorrectDimensions()
    {
        var (width, height) = ImageSize.Parse(ImageSize.InstagramSquare);
        Assert.Equal(1080, width);
        Assert.Equal(1080, height);
    }

    [Fact]
    public void ImageSize_Parse_InvalidFormat_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => ImageSize.Parse("invalid"));
        Assert.Throws<ArgumentException>(() => ImageSize.Parse("100"));
        Assert.Throws<ArgumentException>(() => ImageSize.Parse("axb"));
    }

    [Fact]
    public void ImageSize_Custom_CreatesCorrectString()
    {
        var custom = ImageSize.Custom(800, 600);
        Assert.Equal("800x600", custom);
    }

    #endregion

    #region Image Style Tests

    [Fact]
    public void ImageStyle_GetStylePrompt_ReturnsCorrectPrompts()
    {
        Assert.Equal("", ImageStyle.None.GetStylePrompt());
        Assert.Contains("photorealistic", ImageStyle.Photorealistic.GetStylePrompt().ToLower());
        Assert.Contains("anime", ImageStyle.Anime.GetStylePrompt().ToLower());
        Assert.Contains("cinematic", ImageStyle.Cinematic.GetStylePrompt().ToLower());
        Assert.Contains("minimalist", ImageStyle.Minimalist.GetStylePrompt().ToLower());
        Assert.Contains("corporate", ImageStyle.Corporate.GetStylePrompt().ToLower());
        Assert.Contains("instagram", ImageStyle.InstagramAesthetic.GetStylePrompt().ToLower());
    }

    [Fact]
    public void ImageStyle_GetRecommendedModel_ReturnsCorrectModels()
    {
        Assert.Equal(ImageGenModels.FluxPro, ImageStyle.Photorealistic.GetRecommendedModel());
        Assert.Equal(ImageGenModels.StableDiffusionXL, ImageStyle.Anime.GetRecommendedModel());
        Assert.Equal(ImageGenModels.FluxPro, ImageStyle.Cinematic.GetRecommendedModel());
        Assert.Equal(ImageGenModels.StableDiffusion3, ImageStyle.Fantasy.GetRecommendedModel());
        Assert.Equal(ImageGenModels.DallE3, ImageStyle.None.GetRecommendedModel());
    }

    [Fact]
    public void ImageStyle_AllStyles_HavePrompts()
    {
        foreach (ImageStyle style in Enum.GetValues(typeof(ImageStyle)))
        {
            var prompt = style.GetStylePrompt();
            Assert.NotNull(prompt);

            if (style != ImageStyle.None)
            {
                Assert.NotEmpty(prompt);
            }
        }
    }

    #endregion

    #region GeneratedImage Model Tests

    [Fact]
    public void GeneratedImage_HasData_ReturnsTrueWhenUrlSet()
    {
        var image = new GeneratedImage { Url = "https://example.com/image.png" };
        Assert.True(image.HasData);
        Assert.True(image.IsUrl);
        Assert.False(image.IsBase64);
    }

    [Fact]
    public void GeneratedImage_HasData_ReturnsTrueWhenBase64Set()
    {
        var image = new GeneratedImage { Base64Data = "aGVsbG8=" };
        Assert.True(image.HasData);
        Assert.False(image.IsUrl);
        Assert.True(image.IsBase64);
    }

    [Fact]
    public void GeneratedImage_HasData_ReturnsFalseWhenEmpty()
    {
        var image = new GeneratedImage();
        Assert.False(image.HasData);
        Assert.False(image.IsUrl);
        Assert.False(image.IsBase64);
    }

    [Fact]
    public async Task GeneratedImage_GetBytesAsync_FromBase64_ReturnsBytes()
    {
        var originalBytes = new byte[] { 1, 2, 3, 4, 5 };
        var base64 = Convert.ToBase64String(originalBytes);
        var image = new GeneratedImage { Base64Data = base64 };

        var result = await image.GetBytesAsync();

        Assert.Equal(originalBytes, result);
    }

    [Fact]
    public async Task GeneratedImage_GetBase64Async_FromBase64_ReturnsSameBase64()
    {
        var base64 = "aGVsbG8gd29ybGQ=";
        var image = new GeneratedImage { Base64Data = base64 };

        var result = await image.GetBase64Async();

        Assert.Equal(base64, result);
    }

    [Fact]
    public async Task GeneratedImage_GetBytesAsync_NoData_ThrowsException()
    {
        var image = new GeneratedImage();
        await Assert.ThrowsAsync<InvalidOperationException>(() => image.GetBytesAsync());
    }

    #endregion

    #region Request Model Tests

    [Fact]
    public void ImageGenerationRequest_HasCorrectDefaults()
    {
        var request = new ImageGenerationRequest();

        Assert.Equal(string.Empty, request.Prompt);
        Assert.Null(request.Model);
        Assert.Null(request.Size);
        Assert.Equal(ImageStyle.None, request.Style);
        Assert.Equal(ImageQuality.Standard, request.Quality);
        Assert.Equal(1, request.Count);
        Assert.Null(request.NegativePrompt);
        Assert.Null(request.Seed);
    }

    [Fact]
    public void ImageEditRequest_CanBeCreated()
    {
        var request = new ImageEditRequest
        {
            ImageBytes = new byte[] { 1, 2, 3 },
            Prompt = "Edit this image",
            MaskBytes = new byte[] { 4, 5, 6 },
            Model = ImageGenModels.DallE2
        };

        Assert.NotNull(request.ImageBytes);
        Assert.Equal("Edit this image", request.Prompt);
        Assert.NotNull(request.MaskBytes);
    }

    [Fact]
    public void ImageVariationRequest_HasCorrectDefaults()
    {
        var request = new ImageVariationRequest();

        Assert.Null(request.ImageBytes);
        Assert.Null(request.ImageUrl);
        Assert.Null(request.Prompt);
        Assert.Equal(1, request.Count);
        Assert.Null(request.Strength);
    }

    [Fact]
    public void ImageUpscaleRequest_HasCorrectDefaults()
    {
        var request = new ImageUpscaleRequest();

        Assert.Null(request.ImageBytes);
        Assert.Null(request.ImageUrl);
        Assert.Equal(2, request.Scale);
        Assert.Null(request.Model);
    }

    [Fact]
    public void BatchGenerationResult_HasCorrectDefaults()
    {
        var result = new BatchGenerationResult();

        Assert.Empty(result.Images);
        Assert.Null(result.Prompt);
        Assert.Null(result.Model);
        Assert.Equal(0, result.Count);
        Assert.Null(result.First);
    }

    [Fact]
    public void BatchGenerationResult_First_ReturnsFirstImage()
    {
        var images = new List<GeneratedImage>
        {
            new() { Url = "https://example.com/1.png", Index = 0 },
            new() { Url = "https://example.com/2.png", Index = 1 }
        };

        var result = new BatchGenerationResult
        {
            Images = images,
            Prompt = "test",
            Model = ImageGenModels.DallE3
        };

        Assert.Equal(2, result.Count);
        Assert.NotNull(result.First);
        Assert.Equal(0, result.First!.Index);
    }

    #endregion

    #region Dependency Injection Tests

    [Fact]
    public void AddForeverToolsImageGen_WithApiKey_RegistersServices()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsImageGen("test-api-key");

        var provider = services.BuildServiceProvider();
        var client = provider.GetService<ImageGenClient>();
        var options = provider.GetService<ImageGenOptions>();

        Assert.NotNull(client);
        Assert.NotNull(options);
        Assert.Equal("test-api-key", options.ApiKey);
    }

    [Fact]
    public void AddForeverToolsImageGen_WithOptions_RegistersServices()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsImageGen(options =>
        {
            options.ApiKey = "test-api-key";
            options.DefaultModel = ImageGenModels.FluxPro;
            options.DefaultSize = ImageSize.YouTubeThumbnail;
            options.DefaultQuality = ImageQuality.HD;
        });

        var provider = services.BuildServiceProvider();
        var options = provider.GetService<ImageGenOptions>();

        Assert.NotNull(options);
        Assert.Equal("test-api-key", options.ApiKey);
        Assert.Equal(ImageGenModels.FluxPro, options.DefaultModel);
        Assert.Equal(ImageSize.YouTubeThumbnail, options.DefaultSize);
        Assert.Equal(ImageQuality.HD, options.DefaultQuality);
    }

    [Fact]
    public void AddForeverToolsImageGen_FromConfiguration_RegistersServices()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ImageGen:ApiKey"] = "config-api-key",
                ["ImageGen:DefaultModel"] = "flux-pro",
                ["ImageGen:DefaultSize"] = "1280x720",
                ["ImageGen:TimeoutSeconds"] = "180"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddForeverToolsImageGen(configuration);

        var provider = services.BuildServiceProvider();
        var options = provider.GetService<ImageGenOptions>();

        Assert.NotNull(options);
        Assert.Equal("config-api-key", options.ApiKey);
    }

    [Fact]
    public void AddForeverToolsImageGen_RegistersSingleton()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsImageGen("test-api-key");

        var provider = services.BuildServiceProvider();
        var client1 = provider.GetService<ImageGenClient>();
        var client2 = provider.GetService<ImageGenClient>();

        Assert.Same(client1, client2);
    }

    #endregion

    #region Input Validation Tests

    [Fact]
    public async Task GenerateAsync_NullPrompt_ThrowsException()
    {
        var client = new ImageGenClient("test-api-key");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GenerateAsync((string)null!));
    }

    [Fact]
    public async Task GenerateAsync_EmptyPrompt_ThrowsException()
    {
        var client = new ImageGenClient("test-api-key");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GenerateAsync(""));
    }

    [Fact]
    public async Task GenerateAsync_WhitespacePrompt_ThrowsException()
    {
        var client = new ImageGenClient("test-api-key");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GenerateAsync("   "));
    }

    [Fact]
    public async Task GenerateBatchAsync_NullRequest_ThrowsException()
    {
        var client = new ImageGenClient("test-api-key");
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.GenerateBatchAsync((ImageGenerationRequest)null!));
    }

    [Fact]
    public async Task CreateVariationAsync_EmptyBytes_ThrowsException()
    {
        var client = new ImageGenClient("test-api-key");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.CreateVariationAsync(Array.Empty<byte>()));
    }

    [Fact]
    public async Task InpaintAsync_ThrowsNotSupportedException()
    {
        var client = new ImageGenClient("test-api-key");
        await Assert.ThrowsAsync<NotSupportedException>(() =>
            client.InpaintAsync(new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 }, "test prompt"));
    }

    [Fact]
    public async Task DownloadImageAsync_EmptyUrl_ThrowsException()
    {
        var client = new ImageGenClient("test-api-key");
        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.DownloadImageAsync(""));
    }

    [Fact]
    public async Task SaveAsync_NullImage_ThrowsException()
    {
        var client = new ImageGenClient("test-api-key");
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.SaveAsync(null!, "test.png"));
    }

    #endregion

    #region Quality Enum Tests

    [Fact]
    public void ImageQuality_HasExpectedValues()
    {
        Assert.Equal(0, (int)ImageQuality.Standard);
        Assert.Equal(1, (int)ImageQuality.HD);
    }

    [Fact]
    public void OutputFormat_HasExpectedValues()
    {
        Assert.Equal(0, (int)OutputFormat.Png);
        Assert.Equal(1, (int)OutputFormat.Jpeg);
        Assert.Equal(2, (int)OutputFormat.WebP);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var client = new ImageGenClient("test-api-key");
        client.Dispose();
        client.Dispose(); // Should not throw
    }

    [Fact]
    public void Dispose_WithProvidedHttpClient_DoesNotDisposeIt()
    {
        var httpClient = new HttpClient();
        var options = new ImageGenOptions { ApiKey = "test-api-key" };
        var client = new ImageGenClient(options, httpClient);

        client.Dispose();

        // HttpClient should still be usable - if disposed, this would throw
        var _ = httpClient.Timeout;
    }

    #endregion
}
