using ForeverTools.Captcha.Providers;

namespace ForeverTools.Captcha;

/// <summary>
/// Unified captcha solving client supporting multiple providers.
/// Supports 2Captcha, CapSolver, and Anti-Captcha.
/// </summary>
public class CaptchaClient
{
    private readonly CaptchaOptions _options;
    private readonly Dictionary<CaptchaProvider, ICaptchaSolver> _providers;

    /// <summary>
    /// Gets the configured options.
    /// </summary>
    public CaptchaOptions Options => _options;

    /// <summary>
    /// Creates a new CaptchaClient with the specified API key for the default provider.
    /// </summary>
    /// <param name="apiKey">API key for the default provider (2Captcha).</param>
    public CaptchaClient(string apiKey) : this(new CaptchaOptions { TwoCaptchaApiKey = apiKey })
    {
    }

    /// <summary>
    /// Creates a new CaptchaClient with the specified options.
    /// </summary>
    /// <param name="options">Configuration options.</param>
    /// <param name="httpClient">Optional HTTP client for all providers.</param>
    public CaptchaClient(CaptchaOptions options, HttpClient? httpClient = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _providers = new Dictionary<CaptchaProvider, ICaptchaSolver>();

        // Initialize configured providers
        if (!string.IsNullOrWhiteSpace(options.TwoCaptchaApiKey))
            _providers[CaptchaProvider.TwoCaptcha] = new TwoCaptchaProvider(options.TwoCaptchaApiKey, options, httpClient);

        if (!string.IsNullOrWhiteSpace(options.CapSolverApiKey))
            _providers[CaptchaProvider.CapSolver] = new CapSolverProvider(options.CapSolverApiKey, options, httpClient);

        if (!string.IsNullOrWhiteSpace(options.AntiCaptchaApiKey))
            _providers[CaptchaProvider.AntiCaptcha] = new AntiCaptchaProvider(options.AntiCaptchaApiKey, options, httpClient);

        if (_providers.Count == 0)
            throw new ArgumentException("At least one provider API key is required.", nameof(options));
    }

    /// <summary>
    /// Gets a specific provider solver.
    /// </summary>
    /// <param name="provider">The provider to get.</param>
    /// <returns>The provider's solver.</returns>
    public ICaptchaSolver GetProvider(CaptchaProvider provider)
    {
        if (!_providers.TryGetValue(provider, out var solver))
            throw new InvalidOperationException($"Provider {provider} is not configured. Set the API key in options.");
        return solver;
    }

    /// <summary>
    /// Gets the default provider solver.
    /// </summary>
    public ICaptchaSolver DefaultProvider => GetProvider(_options.DefaultProvider);

    #region Image Captcha

    /// <summary>
    /// Solves an image captcha from base64 data.
    /// </summary>
    /// <param name="imageBase64">Base64-encoded image.</param>
    /// <param name="provider">Optional provider to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<CaptchaResult> SolveImageAsync(string imageBase64, CaptchaProvider? provider = null, CancellationToken cancellationToken = default)
    {
        var task = ImageCaptchaTask.FromBase64(imageBase64);
        return SolveAsync(task, provider, cancellationToken);
    }

    /// <summary>
    /// Solves an image captcha from raw bytes.
    /// </summary>
    /// <param name="imageBytes">Image bytes.</param>
    /// <param name="provider">Optional provider to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<CaptchaResult> SolveImageAsync(byte[] imageBytes, CaptchaProvider? provider = null, CancellationToken cancellationToken = default)
    {
        var task = ImageCaptchaTask.FromBytes(imageBytes);
        return SolveAsync(task, provider, cancellationToken);
    }

    #endregion

    #region reCAPTCHA

    /// <summary>
    /// Solves a reCAPTCHA v2.
    /// </summary>
    /// <param name="websiteUrl">The website URL.</param>
    /// <param name="siteKey">The reCAPTCHA site key.</param>
    /// <param name="invisible">Whether this is invisible reCAPTCHA.</param>
    /// <param name="provider">Optional provider to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<CaptchaResult> SolveReCaptchaV2Async(string websiteUrl, string siteKey, bool invisible = false, CaptchaProvider? provider = null, CancellationToken cancellationToken = default)
    {
        var task = ReCaptchaV2Task.Create(websiteUrl, siteKey, invisible);
        return SolveAsync(task, provider, cancellationToken);
    }

    /// <summary>
    /// Solves a reCAPTCHA v3.
    /// </summary>
    /// <param name="websiteUrl">The website URL.</param>
    /// <param name="siteKey">The reCAPTCHA site key.</param>
    /// <param name="action">The action parameter.</param>
    /// <param name="minScore">Minimum required score.</param>
    /// <param name="provider">Optional provider to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<CaptchaResult> SolveReCaptchaV3Async(string websiteUrl, string siteKey, string action = "verify", double minScore = 0.3, CaptchaProvider? provider = null, CancellationToken cancellationToken = default)
    {
        var task = ReCaptchaV3Task.Create(websiteUrl, siteKey, action, minScore);
        return SolveAsync(task, provider, cancellationToken);
    }

    #endregion

    #region hCaptcha

    /// <summary>
    /// Solves an hCaptcha.
    /// </summary>
    /// <param name="websiteUrl">The website URL.</param>
    /// <param name="siteKey">The hCaptcha site key.</param>
    /// <param name="provider">Optional provider to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<CaptchaResult> SolveHCaptchaAsync(string websiteUrl, string siteKey, CaptchaProvider? provider = null, CancellationToken cancellationToken = default)
    {
        var task = HCaptchaTask.Create(websiteUrl, siteKey);
        return SolveAsync(task, provider, cancellationToken);
    }

    #endregion

    #region Turnstile

    /// <summary>
    /// Solves a Cloudflare Turnstile.
    /// </summary>
    /// <param name="websiteUrl">The website URL.</param>
    /// <param name="siteKey">The Turnstile site key.</param>
    /// <param name="provider">Optional provider to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<CaptchaResult> SolveTurnstileAsync(string websiteUrl, string siteKey, CaptchaProvider? provider = null, CancellationToken cancellationToken = default)
    {
        var task = TurnstileTask.Create(websiteUrl, siteKey);
        return SolveAsync(task, provider, cancellationToken);
    }

    #endregion

    #region FunCaptcha

    /// <summary>
    /// Solves a FunCaptcha / Arkose Labs captcha.
    /// </summary>
    /// <param name="websiteUrl">The website URL.</param>
    /// <param name="publicKey">The FunCaptcha public key.</param>
    /// <param name="provider">Optional provider to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<CaptchaResult> SolveFunCaptchaAsync(string websiteUrl, string publicKey, CaptchaProvider? provider = null, CancellationToken cancellationToken = default)
    {
        var task = FunCaptchaTask.Create(websiteUrl, publicKey);
        return SolveAsync(task, provider, cancellationToken);
    }

    #endregion

    #region Generic

    /// <summary>
    /// Solves any captcha task.
    /// </summary>
    /// <param name="task">The captcha task.</param>
    /// <param name="provider">Optional provider to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<CaptchaResult> SolveAsync(CaptchaTask task, CaptchaProvider? provider = null, CancellationToken cancellationToken = default)
    {
        var solver = provider.HasValue ? GetProvider(provider.Value) : DefaultProvider;
        return solver.SolveAsync(task, cancellationToken);
    }

    /// <summary>
    /// Gets the balance for all configured providers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IReadOnlyList<CaptchaBalance>> GetAllBalancesAsync(CancellationToken cancellationToken = default)
    {
        var tasks = _providers.Values.Select(p => p.GetBalanceAsync(cancellationToken));
        var results = await Task.WhenAll(tasks);
        return results;
    }

    /// <summary>
    /// Gets the balance for a specific provider.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<CaptchaBalance> GetBalanceAsync(CaptchaProvider? provider = null, CancellationToken cancellationToken = default)
    {
        var solver = provider.HasValue ? GetProvider(provider.Value) : DefaultProvider;
        return solver.GetBalanceAsync(cancellationToken);
    }

    /// <summary>
    /// Reports an incorrect solution for refund.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <param name="provider">The provider that solved the captcha.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<bool> ReportIncorrectAsync(string taskId, CaptchaProvider provider, CancellationToken cancellationToken = default)
    {
        var solver = GetProvider(provider);
        return solver.ReportIncorrectAsync(taskId, cancellationToken);
    }

    #endregion

    #region Static Factory Methods

    /// <summary>
    /// Creates a client configured for 2Captcha.
    /// </summary>
    /// <param name="apiKey">2Captcha API key.</param>
    public static CaptchaClient For2Captcha(string apiKey) =>
        new(new CaptchaOptions { TwoCaptchaApiKey = apiKey, DefaultProvider = CaptchaProvider.TwoCaptcha });

    /// <summary>
    /// Creates a client configured for CapSolver.
    /// </summary>
    /// <param name="apiKey">CapSolver API key.</param>
    public static CaptchaClient ForCapSolver(string apiKey) =>
        new(new CaptchaOptions { CapSolverApiKey = apiKey, DefaultProvider = CaptchaProvider.CapSolver });

    /// <summary>
    /// Creates a client configured for Anti-Captcha.
    /// </summary>
    /// <param name="apiKey">Anti-Captcha API key.</param>
    public static CaptchaClient ForAntiCaptcha(string apiKey) =>
        new(new CaptchaOptions { AntiCaptchaApiKey = apiKey, DefaultProvider = CaptchaProvider.AntiCaptcha });

    /// <summary>
    /// Creates a client from an environment variable.
    /// </summary>
    /// <param name="envVarName">Environment variable name.</param>
    /// <param name="provider">The provider the key is for.</param>
    public static CaptchaClient FromEnvironment(string envVarName = "CAPTCHA_API_KEY", CaptchaProvider provider = CaptchaProvider.TwoCaptcha)
    {
        var apiKey = Environment.GetEnvironmentVariable(envVarName);
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException($"Environment variable '{envVarName}' not set.");

        return provider switch
        {
            CaptchaProvider.TwoCaptcha => For2Captcha(apiKey),
            CaptchaProvider.CapSolver => ForCapSolver(apiKey),
            CaptchaProvider.AntiCaptcha => ForAntiCaptcha(apiKey),
            _ => throw new ArgumentException($"Unknown provider: {provider}")
        };
    }

    #endregion
}
