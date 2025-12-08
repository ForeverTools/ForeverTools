namespace ForeverTools.Captcha;

/// <summary>
/// Base class for all captcha tasks.
/// </summary>
public abstract class CaptchaTask
{
    /// <summary>
    /// The type of captcha to solve.
    /// </summary>
    public abstract CaptchaType Type { get; }
}

/// <summary>
/// Image captcha task - recognize text from an image.
/// </summary>
public class ImageCaptchaTask : CaptchaTask
{
    /// <inheritdoc />
    public override CaptchaType Type => CaptchaType.Image;

    /// <summary>
    /// Base64-encoded image data.
    /// </summary>
    public string ImageBase64 { get; set; } = string.Empty;

    /// <summary>
    /// Whether the captcha is case-sensitive.
    /// </summary>
    public bool CaseSensitive { get; set; }

    /// <summary>
    /// Minimum length of the answer.
    /// </summary>
    public int? MinLength { get; set; }

    /// <summary>
    /// Maximum length of the answer.
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Creates an image captcha task from base64 data.
    /// </summary>
    public static ImageCaptchaTask FromBase64(string base64) => new() { ImageBase64 = base64 };

    /// <summary>
    /// Creates an image captcha task from raw image bytes.
    /// </summary>
    public static ImageCaptchaTask FromBytes(byte[] imageBytes) =>
        new() { ImageBase64 = Convert.ToBase64String(imageBytes) };
}

/// <summary>
/// reCAPTCHA v2 task.
/// </summary>
public class ReCaptchaV2Task : CaptchaTask
{
    /// <inheritdoc />
    public override CaptchaType Type => IsInvisible ? CaptchaType.ReCaptchaV2Invisible : CaptchaType.ReCaptchaV2;

    /// <summary>
    /// The website URL where the captcha is located.
    /// </summary>
    public string WebsiteUrl { get; set; } = string.Empty;

    /// <summary>
    /// The reCAPTCHA site key (data-sitekey attribute).
    /// </summary>
    public string SiteKey { get; set; } = string.Empty;

    /// <summary>
    /// Whether this is an invisible reCAPTCHA.
    /// </summary>
    public bool IsInvisible { get; set; }

    /// <summary>
    /// Optional data-s parameter for some reCAPTCHAs.
    /// </summary>
    public string? DataS { get; set; }

    /// <summary>
    /// Creates a reCAPTCHA v2 task.
    /// </summary>
    public static ReCaptchaV2Task Create(string websiteUrl, string siteKey, bool invisible = false) =>
        new() { WebsiteUrl = websiteUrl, SiteKey = siteKey, IsInvisible = invisible };
}

/// <summary>
/// reCAPTCHA v3 task.
/// </summary>
public class ReCaptchaV3Task : CaptchaTask
{
    /// <inheritdoc />
    public override CaptchaType Type => CaptchaType.ReCaptchaV3;

    /// <summary>
    /// The website URL where the captcha is located.
    /// </summary>
    public string WebsiteUrl { get; set; } = string.Empty;

    /// <summary>
    /// The reCAPTCHA site key.
    /// </summary>
    public string SiteKey { get; set; } = string.Empty;

    /// <summary>
    /// The action parameter (e.g., "login", "submit").
    /// </summary>
    public string Action { get; set; } = "verify";

    /// <summary>
    /// Minimum required score (0.1 to 0.9).
    /// </summary>
    public double MinScore { get; set; } = 0.3;

    /// <summary>
    /// Creates a reCAPTCHA v3 task.
    /// </summary>
    public static ReCaptchaV3Task Create(string websiteUrl, string siteKey, string action = "verify", double minScore = 0.3) =>
        new() { WebsiteUrl = websiteUrl, SiteKey = siteKey, Action = action, MinScore = minScore };
}

/// <summary>
/// hCaptcha task.
/// </summary>
public class HCaptchaTask : CaptchaTask
{
    /// <inheritdoc />
    public override CaptchaType Type => CaptchaType.HCaptcha;

    /// <summary>
    /// The website URL where the captcha is located.
    /// </summary>
    public string WebsiteUrl { get; set; } = string.Empty;

    /// <summary>
    /// The hCaptcha site key.
    /// </summary>
    public string SiteKey { get; set; } = string.Empty;

    /// <summary>
    /// Optional enterprise payload data.
    /// </summary>
    public string? EnterprisePayload { get; set; }

    /// <summary>
    /// Creates an hCaptcha task.
    /// </summary>
    public static HCaptchaTask Create(string websiteUrl, string siteKey) =>
        new() { WebsiteUrl = websiteUrl, SiteKey = siteKey };
}

/// <summary>
/// Cloudflare Turnstile task.
/// </summary>
public class TurnstileTask : CaptchaTask
{
    /// <inheritdoc />
    public override CaptchaType Type => CaptchaType.Turnstile;

    /// <summary>
    /// The website URL where the captcha is located.
    /// </summary>
    public string WebsiteUrl { get; set; } = string.Empty;

    /// <summary>
    /// The Turnstile site key.
    /// </summary>
    public string SiteKey { get; set; } = string.Empty;

    /// <summary>
    /// Optional action parameter.
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// Optional cData parameter.
    /// </summary>
    public string? CData { get; set; }

    /// <summary>
    /// Creates a Turnstile task.
    /// </summary>
    public static TurnstileTask Create(string websiteUrl, string siteKey) =>
        new() { WebsiteUrl = websiteUrl, SiteKey = siteKey };
}

/// <summary>
/// FunCaptcha / Arkose Labs task.
/// </summary>
public class FunCaptchaTask : CaptchaTask
{
    /// <inheritdoc />
    public override CaptchaType Type => CaptchaType.FunCaptcha;

    /// <summary>
    /// The website URL where the captcha is located.
    /// </summary>
    public string WebsiteUrl { get; set; } = string.Empty;

    /// <summary>
    /// The FunCaptcha public key.
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Optional service URL (subdomain).
    /// </summary>
    public string? ServiceUrl { get; set; }

    /// <summary>
    /// Optional data blob.
    /// </summary>
    public string? Data { get; set; }

    /// <summary>
    /// Creates a FunCaptcha task.
    /// </summary>
    public static FunCaptchaTask Create(string websiteUrl, string publicKey) =>
        new() { WebsiteUrl = websiteUrl, PublicKey = publicKey };
}
