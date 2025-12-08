using ForeverTools.Captcha;
using ForeverTools.Captcha.Providers;
using Xunit;

namespace ForeverTools.Captcha.Tests;

public class CaptchaClientTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidApiKey_CreatesClient()
    {
        var client = new CaptchaClient("test-api-key");

        Assert.NotNull(client);
        Assert.Equal(CaptchaProvider.TwoCaptcha, client.Options.DefaultProvider);
    }

    [Fact]
    public void Constructor_WithOptions_UsesProvidedOptions()
    {
        var options = new CaptchaOptions
        {
            TwoCaptchaApiKey = "test-key",
            DefaultProvider = CaptchaProvider.TwoCaptcha,
            TimeoutSeconds = 60
        };

        var client = new CaptchaClient(options);

        Assert.Equal("test-key", client.Options.TwoCaptchaApiKey);
        Assert.Equal(60, client.Options.TimeoutSeconds);
    }

    [Fact]
    public void Constructor_WithMultipleProviders_ConfiguresAll()
    {
        var options = new CaptchaOptions
        {
            TwoCaptchaApiKey = "key1",
            CapSolverApiKey = "key2",
            AntiCaptchaApiKey = "key3"
        };

        var client = new CaptchaClient(options);

        Assert.NotNull(client.GetProvider(CaptchaProvider.TwoCaptcha));
        Assert.NotNull(client.GetProvider(CaptchaProvider.CapSolver));
        Assert.NotNull(client.GetProvider(CaptchaProvider.AntiCaptcha));
    }

    [Fact]
    public void Constructor_WithNoApiKeys_ThrowsArgumentException()
    {
        var options = new CaptchaOptions();

        Assert.Throws<ArgumentException>(() => new CaptchaClient(options));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidApiKey_ThrowsArgumentException(string? invalidKey)
    {
        Assert.Throws<ArgumentException>(() => new CaptchaClient(invalidKey!));
    }

    #endregion

    #region Options Tests

    [Fact]
    public void CaptchaOptions_HasCorrectDefaults()
    {
        var options = new CaptchaOptions();

        Assert.Equal(CaptchaProvider.TwoCaptcha, options.DefaultProvider);
        Assert.Equal(120, options.TimeoutSeconds);
        Assert.Equal(3000, options.PollingIntervalMs);
        Assert.False(options.UseProxy);
        Assert.Null(options.TwoCaptchaApiKey);
        Assert.Null(options.CapSolverApiKey);
        Assert.Null(options.AntiCaptchaApiKey);
    }

    [Fact]
    public void CaptchaOptions_SectionName_IsCorrect()
    {
        Assert.Equal("Captcha", CaptchaOptions.SectionName);
    }

    [Fact]
    public void CaptchaOptions_GetApiKey_ReturnsCorrectKey()
    {
        var options = new CaptchaOptions
        {
            TwoCaptchaApiKey = "key1",
            CapSolverApiKey = "key2",
            AntiCaptchaApiKey = "key3"
        };

        Assert.Equal("key1", options.GetApiKey(CaptchaProvider.TwoCaptcha));
        Assert.Equal("key2", options.GetApiKey(CaptchaProvider.CapSolver));
        Assert.Equal("key3", options.GetApiKey(CaptchaProvider.AntiCaptcha));
    }

    [Fact]
    public void CaptchaOptions_HasValidProvider_ReturnsTrueWhenConfigured()
    {
        var options = new CaptchaOptions { TwoCaptchaApiKey = "key" };
        Assert.True(options.HasValidProvider);
    }

    [Fact]
    public void CaptchaOptions_HasValidProvider_ReturnsFalseWhenEmpty()
    {
        var options = new CaptchaOptions();
        Assert.False(options.HasValidProvider);
    }

    #endregion

    #region CaptchaTask Tests

    [Fact]
    public void ImageCaptchaTask_FromBase64_CreatesTask()
    {
        var task = ImageCaptchaTask.FromBase64("dGVzdA==");

        Assert.Equal(CaptchaType.Image, task.Type);
        Assert.Equal("dGVzdA==", task.ImageBase64);
    }

    [Fact]
    public void ImageCaptchaTask_FromBytes_CreatesTask()
    {
        var bytes = new byte[] { 1, 2, 3, 4 };
        var task = ImageCaptchaTask.FromBytes(bytes);

        Assert.Equal(CaptchaType.Image, task.Type);
        Assert.Equal(Convert.ToBase64String(bytes), task.ImageBase64);
    }

    [Fact]
    public void ReCaptchaV2Task_Create_CreatesTask()
    {
        var task = ReCaptchaV2Task.Create("https://example.com", "sitekey123", false);

        Assert.Equal(CaptchaType.ReCaptchaV2, task.Type);
        Assert.Equal("https://example.com", task.WebsiteUrl);
        Assert.Equal("sitekey123", task.SiteKey);
        Assert.False(task.IsInvisible);
    }

    [Fact]
    public void ReCaptchaV2Task_Invisible_ReturnsCorrectType()
    {
        var task = ReCaptchaV2Task.Create("https://example.com", "sitekey123", invisible: true);

        Assert.Equal(CaptchaType.ReCaptchaV2Invisible, task.Type);
        Assert.True(task.IsInvisible);
    }

    [Fact]
    public void ReCaptchaV3Task_Create_CreatesTask()
    {
        var task = ReCaptchaV3Task.Create("https://example.com", "sitekey123", "login", 0.7);

        Assert.Equal(CaptchaType.ReCaptchaV3, task.Type);
        Assert.Equal("login", task.Action);
        Assert.Equal(0.7, task.MinScore);
    }

    [Fact]
    public void HCaptchaTask_Create_CreatesTask()
    {
        var task = HCaptchaTask.Create("https://example.com", "sitekey123");

        Assert.Equal(CaptchaType.HCaptcha, task.Type);
        Assert.Equal("https://example.com", task.WebsiteUrl);
        Assert.Equal("sitekey123", task.SiteKey);
    }

    [Fact]
    public void TurnstileTask_Create_CreatesTask()
    {
        var task = TurnstileTask.Create("https://example.com", "sitekey123");

        Assert.Equal(CaptchaType.Turnstile, task.Type);
        Assert.Equal("https://example.com", task.WebsiteUrl);
        Assert.Equal("sitekey123", task.SiteKey);
    }

    [Fact]
    public void FunCaptchaTask_Create_CreatesTask()
    {
        var task = FunCaptchaTask.Create("https://example.com", "publickey123");

        Assert.Equal(CaptchaType.FunCaptcha, task.Type);
        Assert.Equal("https://example.com", task.WebsiteUrl);
        Assert.Equal("publickey123", task.PublicKey);
    }

    #endregion

    #region CaptchaResult Tests

    [Fact]
    public void CaptchaResult_Solved_CreatesSuccessResult()
    {
        var result = CaptchaResult.Solved("token123", "task456", CaptchaProvider.TwoCaptcha);

        Assert.True(result.Success);
        Assert.Equal("token123", result.Solution);
        Assert.Equal("task456", result.TaskId);
        Assert.Equal(CaptchaProvider.TwoCaptcha, result.Provider);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void CaptchaResult_Failed_CreatesFailedResult()
    {
        var result = CaptchaResult.Failed("Something went wrong", "ERROR_CODE", CaptchaProvider.CapSolver);

        Assert.False(result.Success);
        Assert.Equal("Something went wrong", result.ErrorMessage);
        Assert.Equal("ERROR_CODE", result.ErrorCode);
        Assert.Equal(CaptchaProvider.CapSolver, result.Provider);
    }

    #endregion

    #region Static Factory Tests

    [Fact]
    public void For2Captcha_CreatesConfiguredClient()
    {
        var client = CaptchaClient.For2Captcha("test-key");

        Assert.Equal(CaptchaProvider.TwoCaptcha, client.Options.DefaultProvider);
        Assert.Equal("test-key", client.Options.TwoCaptchaApiKey);
    }

    [Fact]
    public void ForCapSolver_CreatesConfiguredClient()
    {
        var client = CaptchaClient.ForCapSolver("test-key");

        Assert.Equal(CaptchaProvider.CapSolver, client.Options.DefaultProvider);
        Assert.Equal("test-key", client.Options.CapSolverApiKey);
    }

    [Fact]
    public void ForAntiCaptcha_CreatesConfiguredClient()
    {
        var client = CaptchaClient.ForAntiCaptcha("test-key");

        Assert.Equal(CaptchaProvider.AntiCaptcha, client.Options.DefaultProvider);
        Assert.Equal("test-key", client.Options.AntiCaptchaApiKey);
    }

    [Fact]
    public void FromEnvironment_WithMissingVariable_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("CAPTCHA_TEST_MISSING", null);

        Assert.Throws<InvalidOperationException>(
            () => CaptchaClient.FromEnvironment("CAPTCHA_TEST_MISSING"));
    }

    [Fact]
    public void FromEnvironment_WithSetVariable_CreatesClient()
    {
        const string envVar = "CAPTCHA_TEST_KEY";
        Environment.SetEnvironmentVariable(envVar, "test-api-key");

        try
        {
            var client = CaptchaClient.FromEnvironment(envVar, CaptchaProvider.TwoCaptcha);

            Assert.NotNull(client);
            Assert.Equal("test-api-key", client.Options.TwoCaptchaApiKey);
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    #endregion

    #region Provider Tests

    [Fact]
    public void GetProvider_WithConfiguredProvider_ReturnsProvider()
    {
        var client = CaptchaClient.For2Captcha("test-key");

        var provider = client.GetProvider(CaptchaProvider.TwoCaptcha);

        Assert.NotNull(provider);
        Assert.Equal(CaptchaProvider.TwoCaptcha, provider.Provider);
    }

    [Fact]
    public void GetProvider_WithUnconfiguredProvider_ThrowsInvalidOperationException()
    {
        var client = CaptchaClient.For2Captcha("test-key");

        Assert.Throws<InvalidOperationException>(
            () => client.GetProvider(CaptchaProvider.CapSolver));
    }

    [Fact]
    public void DefaultProvider_ReturnsConfiguredDefault()
    {
        var options = new CaptchaOptions
        {
            TwoCaptchaApiKey = "key1",
            CapSolverApiKey = "key2",
            DefaultProvider = CaptchaProvider.CapSolver
        };
        var client = new CaptchaClient(options);

        var provider = client.DefaultProvider;

        Assert.Equal(CaptchaProvider.CapSolver, provider.Provider);
    }

    #endregion

    #region TwoCaptchaProvider Tests

    [Fact]
    public void TwoCaptchaProvider_Constructor_SetsProvider()
    {
        var provider = new TwoCaptchaProvider("test-key");

        Assert.Equal(CaptchaProvider.TwoCaptcha, provider.Provider);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TwoCaptchaProvider_Constructor_WithInvalidKey_Throws(string? key)
    {
        Assert.Throws<ArgumentException>(() => new TwoCaptchaProvider(key!));
    }

    #endregion

    #region CapSolverProvider Tests

    [Fact]
    public void CapSolverProvider_Constructor_SetsProvider()
    {
        var provider = new CapSolverProvider("test-key");

        Assert.Equal(CaptchaProvider.CapSolver, provider.Provider);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CapSolverProvider_Constructor_WithInvalidKey_Throws(string? key)
    {
        Assert.Throws<ArgumentException>(() => new CapSolverProvider(key!));
    }

    #endregion

    #region AntiCaptchaProvider Tests

    [Fact]
    public void AntiCaptchaProvider_Constructor_SetsProvider()
    {
        var provider = new AntiCaptchaProvider("test-key");

        Assert.Equal(CaptchaProvider.AntiCaptcha, provider.Provider);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AntiCaptchaProvider_Constructor_WithInvalidKey_Throws(string? key)
    {
        Assert.Throws<ArgumentException>(() => new AntiCaptchaProvider(key!));
    }

    #endregion

    #region CaptchaType Enum Tests

    [Fact]
    public void CaptchaType_HasExpectedValues()
    {
        Assert.Equal(0, (int)CaptchaType.Image);
        Assert.Equal(1, (int)CaptchaType.ReCaptchaV2);
        Assert.Equal(2, (int)CaptchaType.ReCaptchaV2Invisible);
        Assert.Equal(3, (int)CaptchaType.ReCaptchaV3);
    }

    [Fact]
    public void CaptchaProvider_HasExpectedValues()
    {
        Assert.Equal(0, (int)CaptchaProvider.TwoCaptcha);
        Assert.Equal(1, (int)CaptchaProvider.CapSolver);
        Assert.Equal(2, (int)CaptchaProvider.AntiCaptcha);
    }

    #endregion
}
