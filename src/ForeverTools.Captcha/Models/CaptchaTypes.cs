namespace ForeverTools.Captcha;

/// <summary>
/// Supported captcha types.
/// </summary>
public enum CaptchaType
{
    /// <summary>Image captcha with text to recognize.</summary>
    Image,

    /// <summary>Google reCAPTCHA v2 checkbox.</summary>
    ReCaptchaV2,

    /// <summary>Google reCAPTCHA v2 invisible.</summary>
    ReCaptchaV2Invisible,

    /// <summary>Google reCAPTCHA v3 score-based.</summary>
    ReCaptchaV3,

    /// <summary>Google reCAPTCHA Enterprise.</summary>
    ReCaptchaEnterprise,

    /// <summary>hCaptcha checkbox.</summary>
    HCaptcha,

    /// <summary>Cloudflare Turnstile.</summary>
    Turnstile,

    /// <summary>FunCaptcha / Arkose Labs.</summary>
    FunCaptcha,

    /// <summary>GeeTest v3.</summary>
    GeeTestV3,

    /// <summary>GeeTest v4.</summary>
    GeeTestV4
}

/// <summary>
/// Supported captcha solving providers.
/// </summary>
public enum CaptchaProvider
{
    /// <summary>2Captcha.com - https://2captcha.com</summary>
    TwoCaptcha,

    /// <summary>CapSolver.com - https://capsolver.com</summary>
    CapSolver,

    /// <summary>Anti-Captcha.com - https://anti-captcha.com</summary>
    AntiCaptcha
}

/// <summary>
/// Status of a captcha solving task.
/// </summary>
public enum CaptchaTaskStatus
{
    /// <summary>Task is being processed.</summary>
    Processing,

    /// <summary>Task completed successfully.</summary>
    Ready,

    /// <summary>Task failed.</summary>
    Failed
}
