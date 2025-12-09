using ForeverTools.ScraperAPI;
using ForeverTools.ScraperAPI.Models;
using Xunit;

namespace ForeverTools.ScraperAPI.Tests;

public class ScraperApiClientTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidApiKey_CreatesClient()
    {
        var client = new ScraperApiClient("test-api-key");

        Assert.NotNull(client);
        Assert.Equal("test-api-key", client.Options.ApiKey);
    }

    [Fact]
    public void Constructor_WithOptions_UsesProvidedOptions()
    {
        var options = new ScraperApiOptions
        {
            ApiKey = "test-key",
            DefaultCountryCode = "us",
            TimeoutSeconds = 90,
            DefaultRenderJavaScript = true
        };

        var client = new ScraperApiClient(options);

        Assert.Equal("test-key", client.Options.ApiKey);
        Assert.Equal("us", client.Options.DefaultCountryCode);
        Assert.Equal(90, client.Options.TimeoutSeconds);
        Assert.True(client.Options.DefaultRenderJavaScript);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidApiKey_ThrowsArgumentException(string? invalidKey)
    {
        var options = new ScraperApiOptions { ApiKey = invalidKey };

        Assert.Throws<ArgumentException>(() => new ScraperApiClient(options));
    }

    [Fact]
    public void Constructor_WithNullApiKey_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ScraperApiClient((string)null!));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ScraperApiClient((ScraperApiOptions)null!));
    }

    #endregion

    #region ScraperApiOptions Tests

    [Fact]
    public void ScraperApiOptions_HasCorrectDefaults()
    {
        var options = new ScraperApiOptions();

        Assert.Null(options.ApiKey);
        Assert.Equal("https://api.scraperapi.com", options.BaseUrl);
        Assert.Equal("https://async.scraperapi.com", options.AsyncBaseUrl);
        Assert.Equal(70, options.TimeoutSeconds);
        Assert.Null(options.DefaultCountryCode);
        Assert.False(options.DefaultRenderJavaScript);
        Assert.False(options.DefaultPremium);
        Assert.Null(options.DefaultDeviceType);
    }

    [Fact]
    public void ScraperApiOptions_SectionName_IsCorrect()
    {
        Assert.Equal("ScraperAPI", ScraperApiOptions.SectionName);
    }

    [Fact]
    public void ScraperApiOptions_HasApiKey_ReturnsTrueWhenSet()
    {
        var options = new ScraperApiOptions { ApiKey = "key" };
        Assert.True(options.HasApiKey);
    }

    [Fact]
    public void ScraperApiOptions_HasApiKey_ReturnsFalseWhenEmpty()
    {
        var options = new ScraperApiOptions();
        Assert.False(options.HasApiKey);
    }

    #endregion

    #region ScrapeRequest Tests

    [Fact]
    public void ScrapeRequest_DefaultValues_AreCorrect()
    {
        var request = new ScrapeRequest();

        Assert.Equal(string.Empty, request.Url);
        Assert.False(request.RenderJavaScript);
        Assert.False(request.Screenshot);
        Assert.Null(request.CountryCode);
        Assert.False(request.Premium);
        Assert.False(request.UltraPremium);
        Assert.Null(request.SessionNumber);
        Assert.Null(request.DeviceType);
        Assert.False(request.AutoParse);
        Assert.False(request.KeepHeaders);
        Assert.True(request.FollowRedirect);
        Assert.Null(request.Headers);
        Assert.Null(request.OutputFormat);
    }

    [Fact]
    public void ScrapeRequest_CanSetAllProperties()
    {
        var request = new ScrapeRequest
        {
            Url = "https://example.com",
            RenderJavaScript = true,
            Screenshot = true,
            CountryCode = "us",
            Premium = true,
            UltraPremium = false,
            SessionNumber = 12345,
            DeviceType = DeviceTypes.Mobile,
            AutoParse = true,
            KeepHeaders = true,
            FollowRedirect = false,
            OutputFormat = OutputFormats.Json,
            Headers = new Dictionary<string, string> { ["X-Custom"] = "value" }
        };

        Assert.Equal("https://example.com", request.Url);
        Assert.True(request.RenderJavaScript);
        Assert.True(request.Screenshot);
        Assert.Equal("us", request.CountryCode);
        Assert.True(request.Premium);
        Assert.False(request.UltraPremium);
        Assert.Equal(12345, request.SessionNumber);
        Assert.Equal("mobile", request.DeviceType);
        Assert.True(request.AutoParse);
        Assert.True(request.KeepHeaders);
        Assert.False(request.FollowRedirect);
        Assert.Equal("json", request.OutputFormat);
        Assert.NotNull(request.Headers);
        Assert.Equal("value", request.Headers["X-Custom"]);
    }

    #endregion

    #region ScrapeResponse Tests

    [Fact]
    public void ScrapeResponse_DefaultValues_AreCorrect()
    {
        var response = new ScrapeResponse();

        Assert.False(response.Success);
        Assert.Null(response.Content);
        Assert.Null(response.ScreenshotBase64);
        Assert.Equal(0, response.StatusCode);
        Assert.Null(response.Error);
        Assert.Null(response.Url);
        Assert.Null(response.Headers);
    }

    [Fact]
    public void ScrapeResponse_CanSetAllProperties()
    {
        var response = new ScrapeResponse
        {
            Success = true,
            Content = "<html></html>",
            ScreenshotBase64 = "base64data",
            StatusCode = 200,
            Url = "https://example.com",
            Headers = new Dictionary<string, string> { ["Content-Type"] = "text/html" }
        };

        Assert.True(response.Success);
        Assert.Equal("<html></html>", response.Content);
        Assert.Equal("base64data", response.ScreenshotBase64);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal("https://example.com", response.Url);
        Assert.NotNull(response.Headers);
    }

    #endregion

    #region AsyncJobResponse Tests

    [Fact]
    public void AsyncJobResponse_Success_IsTrueWhenIdSet()
    {
        var response = new AsyncJobResponse { Id = "job-123" };
        Assert.True(response.Success);
    }

    [Fact]
    public void AsyncJobResponse_Success_IsFalseWhenIdEmpty()
    {
        var response = new AsyncJobResponse();
        Assert.False(response.Success);
    }

    #endregion

    #region AsyncJobStatus Tests

    [Fact]
    public void AsyncJobStatus_IsRunning_WhenStatusRunning()
    {
        var status = new AsyncJobStatus { Status = "running" };

        Assert.True(status.IsRunning);
        Assert.False(status.IsFinished);
        Assert.False(status.IsFailed);
    }

    [Fact]
    public void AsyncJobStatus_IsFinished_WhenStatusFinished()
    {
        var status = new AsyncJobStatus { Status = "finished" };

        Assert.False(status.IsRunning);
        Assert.True(status.IsFinished);
        Assert.False(status.IsFailed);
    }

    [Fact]
    public void AsyncJobStatus_IsFailed_WhenStatusFailed()
    {
        var status = new AsyncJobStatus { Status = "failed" };

        Assert.False(status.IsRunning);
        Assert.False(status.IsFinished);
        Assert.True(status.IsFailed);
    }

    #endregion

    #region AccountInfo Tests

    [Fact]
    public void AccountInfo_RemainingCredits_CalculatedCorrectly()
    {
        var info = new AccountInfo
        {
            RequestLimit = 10000,
            RequestCount = 3500
        };

        Assert.Equal(6500, info.RemainingCredits);
    }

    [Fact]
    public void AccountInfo_DefaultValues_AreCorrect()
    {
        var info = new AccountInfo();

        Assert.Equal(0, info.RequestCount);
        Assert.Equal(0, info.RequestLimit);
        Assert.Equal(0, info.ConcurrencyLimit);
        Assert.Equal(0, info.RemainingCredits);
    }

    #endregion

    #region DeviceTypes Tests

    [Fact]
    public void DeviceTypes_HasCorrectValues()
    {
        Assert.Equal("desktop", DeviceTypes.Desktop);
        Assert.Equal("mobile", DeviceTypes.Mobile);
    }

    #endregion

    #region OutputFormats Tests

    [Fact]
    public void OutputFormats_HasCorrectValues()
    {
        Assert.Equal("html", OutputFormats.Html);
        Assert.Equal("markdown", OutputFormats.Markdown);
        Assert.Equal("text", OutputFormats.Text);
        Assert.Equal("json", OutputFormats.Json);
        Assert.Equal("csv", OutputFormats.Csv);
    }

    #endregion

    #region ScraperApiException Tests

    [Fact]
    public void ScraperApiException_StoresMessage()
    {
        var ex = new ScraperApiException("Test error", 500);

        Assert.Equal("Test error", ex.Message);
        Assert.Equal(500, ex.StatusCode);
    }

    [Fact]
    public void ScraperApiException_DefaultStatusCode_IsZero()
    {
        var ex = new ScraperApiException("Test error");

        Assert.Equal("Test error", ex.Message);
        Assert.Equal(0, ex.StatusCode);
    }

    #endregion

    #region FromEnvironment Tests

    [Fact]
    public void FromEnvironment_WithMissingVariable_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("SCRAPERAPI_TEST_MISSING", null);

        Assert.Throws<InvalidOperationException>(
            () => ScraperApiClient.FromEnvironment("SCRAPERAPI_TEST_MISSING"));
    }

    [Fact]
    public void FromEnvironment_WithSetVariable_CreatesClient()
    {
        const string envVar = "SCRAPERAPI_TEST_KEY";
        Environment.SetEnvironmentVariable(envVar, "test-api-key");

        try
        {
            var client = ScraperApiClient.FromEnvironment(envVar);

            Assert.NotNull(client);
            Assert.Equal("test-api-key", client.Options.ApiKey);
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    #endregion

    #region Validation Tests

    [Fact]
    public async Task ScrapeWithResponseAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        var client = new ScraperApiClient("test-key");

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => client.ScrapeWithResponseAsync(null!));
    }

    [Fact]
    public async Task ScrapeWithResponseAsync_WithEmptyUrl_ThrowsArgumentException()
    {
        var client = new ScraperApiClient("test-key");
        var request = new ScrapeRequest { Url = "" };

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.ScrapeWithResponseAsync(request));
    }

    [Fact]
    public async Task SubmitAsyncJobAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        var client = new ScraperApiClient("test-key");

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => client.SubmitAsyncJobAsync(null!));
    }

    [Fact]
    public async Task GetAsyncJobStatusAsync_WithEmptyJobId_ThrowsArgumentException()
    {
        var client = new ScraperApiClient("test-key");

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.GetAsyncJobStatusAsync(""));
    }

    #endregion
}
