namespace ForeverTools.Captcha;

/// <summary>
/// Configuration options for the captcha solving client.
/// </summary>
public class CaptchaOptions
{
    /// <summary>
    /// The configuration section name for binding from appsettings.json.
    /// </summary>
    public const string SectionName = "Captcha";

    /// <summary>
    /// The default provider to use for solving captchas.
    /// </summary>
    public CaptchaProvider DefaultProvider { get; set; } = CaptchaProvider.TwoCaptcha;

    /// <summary>
    /// API key for 2Captcha. Get one at https://2captcha.com
    /// </summary>
    public string? TwoCaptchaApiKey { get; set; }

    /// <summary>
    /// API key for CapSolver. Get one at https://capsolver.com
    /// </summary>
    public string? CapSolverApiKey { get; set; }

    /// <summary>
    /// API key for Anti-Captcha. Get one at https://anti-captcha.com
    /// </summary>
    public string? AntiCaptchaApiKey { get; set; }

    /// <summary>
    /// Default timeout for solving captchas (in seconds). Default is 120.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// Polling interval when checking for results (in milliseconds). Default is 3000.
    /// </summary>
    public int PollingIntervalMs { get; set; } = 3000;

    /// <summary>
    /// Whether to use proxy for token-based captchas. Default is false (proxyless).
    /// </summary>
    public bool UseProxy { get; set; }

    /// <summary>
    /// Proxy configuration if UseProxy is true.
    /// </summary>
    public ProxyConfig? Proxy { get; set; }

    /// <summary>
    /// Gets the API key for the specified provider.
    /// </summary>
    public string? GetApiKey(CaptchaProvider provider) => provider switch
    {
        CaptchaProvider.TwoCaptcha => TwoCaptchaApiKey,
        CaptchaProvider.CapSolver => CapSolverApiKey,
        CaptchaProvider.AntiCaptcha => AntiCaptchaApiKey,
        _ => null
    };

    /// <summary>
    /// Validates that at least one provider is configured.
    /// </summary>
    public bool HasValidProvider =>
        !string.IsNullOrWhiteSpace(TwoCaptchaApiKey) ||
        !string.IsNullOrWhiteSpace(CapSolverApiKey) ||
        !string.IsNullOrWhiteSpace(AntiCaptchaApiKey);
}

/// <summary>
/// Proxy configuration for captcha solving.
/// </summary>
public class ProxyConfig
{
    /// <summary>
    /// Proxy type (http, https, socks4, socks5).
    /// </summary>
    public string Type { get; set; } = "http";

    /// <summary>
    /// Proxy host/IP address.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Proxy port.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Proxy username (if authentication required).
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Proxy password (if authentication required).
    /// </summary>
    public string? Password { get; set; }
}
