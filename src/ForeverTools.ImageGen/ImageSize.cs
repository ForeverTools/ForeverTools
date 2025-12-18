namespace ForeverTools.ImageGen;

/// <summary>
/// Pre-defined image size presets including social media dimensions.
/// Format: "widthxheight"
/// </summary>
public static class ImageSize
{
    // ============================================
    // Standard Sizes
    // ============================================

    /// <summary>
    /// 512x512 - Small square. Fast generation.
    /// </summary>
    public const string Square512 = "512x512";

    /// <summary>
    /// 1024x1024 - Standard square. Default size.
    /// </summary>
    public const string Square1024 = "1024x1024";

    /// <summary>
    /// 2048x2048 - Large square. High detail.
    /// </summary>
    public const string Square2048 = "2048x2048";

    // ============================================
    // Landscape Sizes
    // ============================================

    /// <summary>
    /// 1792x1024 - Wide landscape (DALL-E 3 native).
    /// </summary>
    public const string Landscape1792x1024 = "1792x1024";

    /// <summary>
    /// 1536x1024 - Landscape 3:2 ratio.
    /// </summary>
    public const string Landscape1536x1024 = "1536x1024";

    /// <summary>
    /// 1920x1080 - Full HD landscape.
    /// </summary>
    public const string Landscape1920x1080 = "1920x1080";

    /// <summary>
    /// 1280x720 - HD landscape.
    /// </summary>
    public const string Landscape1280x720 = "1280x720";

    // ============================================
    // Portrait Sizes
    // ============================================

    /// <summary>
    /// 1024x1792 - Tall portrait (DALL-E 3 native).
    /// </summary>
    public const string Portrait1024x1792 = "1024x1792";

    /// <summary>
    /// 1024x1536 - Portrait 2:3 ratio.
    /// </summary>
    public const string Portrait1024x1536 = "1024x1536";

    /// <summary>
    /// 768x1024 - Standard portrait.
    /// </summary>
    public const string Portrait768x1024 = "768x1024";

    // ============================================
    // Instagram
    // ============================================

    /// <summary>
    /// 1080x1080 - Instagram square post.
    /// </summary>
    public const string InstagramSquare = "1080x1080";

    /// <summary>
    /// 1080x1350 - Instagram portrait post (4:5 ratio, max engagement).
    /// </summary>
    public const string InstagramPortrait = "1080x1350";

    /// <summary>
    /// 1080x566 - Instagram landscape post.
    /// </summary>
    public const string InstagramLandscape = "1080x566";

    /// <summary>
    /// 1080x1920 - Instagram Story / Reel (9:16 ratio).
    /// </summary>
    public const string InstagramStory = "1080x1920";

    /// <summary>
    /// 1080x1920 - Instagram Reel (same as Story).
    /// </summary>
    public const string InstagramReel = "1080x1920";

    // ============================================
    // Twitter / X
    // ============================================

    /// <summary>
    /// 1200x675 - Twitter single image post (16:9 ratio).
    /// </summary>
    public const string TwitterPost = "1200x675";

    /// <summary>
    /// 1500x500 - Twitter header/banner image.
    /// </summary>
    public const string TwitterHeader = "1500x500";

    /// <summary>
    /// 400x400 - Twitter profile picture.
    /// </summary>
    public const string TwitterProfile = "400x400";

    /// <summary>
    /// 1200x628 - Twitter card image.
    /// </summary>
    public const string TwitterCard = "1200x628";

    // ============================================
    // Facebook
    // ============================================

    /// <summary>
    /// 1200x630 - Facebook shared image post.
    /// </summary>
    public const string FacebookPost = "1200x630";

    /// <summary>
    /// 1640x856 - Facebook cover photo (personal).
    /// </summary>
    public const string FacebookCover = "1640x856";

    /// <summary>
    /// 1200x628 - Facebook ad / link preview.
    /// </summary>
    public const string FacebookAd = "1200x628";

    /// <summary>
    /// 1080x1920 - Facebook Story.
    /// </summary>
    public const string FacebookStory = "1080x1920";

    // ============================================
    // YouTube
    // ============================================

    /// <summary>
    /// 1280x720 - YouTube thumbnail (recommended).
    /// </summary>
    public const string YouTubeThumbnail = "1280x720";

    /// <summary>
    /// 2560x1440 - YouTube channel banner.
    /// </summary>
    public const string YouTubeBanner = "2560x1440";

    /// <summary>
    /// 800x800 - YouTube channel profile picture.
    /// </summary>
    public const string YouTubeProfile = "800x800";

    // ============================================
    // LinkedIn
    // ============================================

    /// <summary>
    /// 1200x627 - LinkedIn post image.
    /// </summary>
    public const string LinkedInPost = "1200x627";

    /// <summary>
    /// 1584x396 - LinkedIn personal background.
    /// </summary>
    public const string LinkedInBanner = "1584x396";

    /// <summary>
    /// 400x400 - LinkedIn profile picture.
    /// </summary>
    public const string LinkedInProfile = "400x400";

    /// <summary>
    /// 1128x191 - LinkedIn company cover.
    /// </summary>
    public const string LinkedInCompanyCover = "1128x191";

    // ============================================
    // TikTok
    // ============================================

    /// <summary>
    /// 1080x1920 - TikTok video cover / content (9:16).
    /// </summary>
    public const string TikTok = "1080x1920";

    // ============================================
    // Pinterest
    // ============================================

    /// <summary>
    /// 1000x1500 - Pinterest pin (2:3 ratio, optimal).
    /// </summary>
    public const string PinterestPin = "1000x1500";

    /// <summary>
    /// 600x900 - Pinterest standard pin.
    /// </summary>
    public const string PinterestStandard = "600x900";

    // ============================================
    // Blog / Web
    // ============================================

    /// <summary>
    /// 1200x630 - Open Graph / social share image.
    /// </summary>
    public const string OpenGraph = "1200x630";

    /// <summary>
    /// 1200x600 - Blog post featured image.
    /// </summary>
    public const string BlogFeatured = "1200x600";

    /// <summary>
    /// 1920x1080 - Full HD wallpaper / hero image.
    /// </summary>
    public const string FullHD = "1920x1080";

    /// <summary>
    /// 3840x2160 - 4K wallpaper / high-res hero.
    /// </summary>
    public const string UltraHD4K = "3840x2160";

    // ============================================
    // Email
    // ============================================

    /// <summary>
    /// 600x300 - Email header image.
    /// </summary>
    public const string EmailHeader = "600x300";

    /// <summary>
    /// 600x400 - Email content image.
    /// </summary>
    public const string EmailContent = "600x400";

    // ============================================
    // Ads
    // ============================================

    /// <summary>
    /// 300x250 - Medium rectangle (Google Ads).
    /// </summary>
    public const string AdMediumRectangle = "300x250";

    /// <summary>
    /// 728x90 - Leaderboard (Google Ads).
    /// </summary>
    public const string AdLeaderboard = "728x90";

    /// <summary>
    /// 160x600 - Wide skyscraper (Google Ads).
    /// </summary>
    public const string AdSkyscraper = "160x600";

    /// <summary>
    /// 320x50 - Mobile banner (Google Ads).
    /// </summary>
    public const string AdMobileBanner = "320x50";

    // ============================================
    // Platform Presets (Aliases)
    // ============================================

    /// <summary>
    /// Platform-specific preset collections.
    /// </summary>
    public static class Platforms
    {
        /// <summary>
        /// Instagram size presets.
        /// </summary>
        public static class Instagram
        {
            public const string Square = InstagramSquare;
            public const string Portrait = InstagramPortrait;
            public const string Landscape = InstagramLandscape;
            public const string Story = InstagramStory;
            public const string Reel = InstagramReel;
        }

        /// <summary>
        /// Twitter/X size presets.
        /// </summary>
        public static class Twitter
        {
            public const string Post = TwitterPost;
            public const string Header = TwitterHeader;
            public const string Profile = TwitterProfile;
            public const string Card = TwitterCard;
        }

        /// <summary>
        /// Facebook size presets.
        /// </summary>
        public static class Facebook
        {
            public const string Post = FacebookPost;
            public const string Cover = FacebookCover;
            public const string Ad = FacebookAd;
            public const string Story = FacebookStory;
        }

        /// <summary>
        /// YouTube size presets.
        /// </summary>
        public static class YouTube
        {
            public const string Thumbnail = YouTubeThumbnail;
            public const string Banner = YouTubeBanner;
            public const string Profile = YouTubeProfile;
        }

        /// <summary>
        /// LinkedIn size presets.
        /// </summary>
        public static class LinkedIn
        {
            public const string Post = LinkedInPost;
            public const string Banner = LinkedInBanner;
            public const string Profile = LinkedInProfile;
        }
    }

    /// <summary>
    /// Parse a size string into width and height.
    /// </summary>
    public static (int Width, int Height) Parse(string size)
    {
        var parts = size.Split('x');
        if (parts.Length != 2 ||
            !int.TryParse(parts[0], out var width) ||
            !int.TryParse(parts[1], out var height))
        {
            throw new ArgumentException($"Invalid size format: {size}. Expected format: 'widthxheight'", nameof(size));
        }
        return (width, height);
    }

    /// <summary>
    /// Create a custom size string.
    /// </summary>
    public static string Custom(int width, int height) => $"{width}x{height}";
}
