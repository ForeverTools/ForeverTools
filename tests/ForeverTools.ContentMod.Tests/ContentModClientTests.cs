using System.Net;
using System.Text;
using System.Text.Json;
using ForeverTools.ContentMod;
using ForeverTools.ContentMod.Extensions;
using ForeverTools.ContentMod.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace ForeverTools.ContentMod.Tests;

/// <summary>
/// Unit tests for ContentModClient. All HTTP calls are mocked — no real API calls are made.
/// </summary>
public class ContentModClientTests
{
    // =========================================================================
    // Test helpers
    // =========================================================================

    private static string MakeModerationJson(
        bool flagged = false,
        bool toxic = false,
        bool nsfw = false,
        bool spam = false,
        bool hate = false,
        double toxicScore = 0.01,
        double nsfwScore = 0.01,
        double spamScore = 0.01,
        double hateScore = 0.01,
        bool safeForWork = true)
    {
        return JsonSerializer.Serialize(new
        {
            flagged,
            categories = new { toxic, nsfw, spam, hate },
            scores     = new { toxic = toxicScore, nsfw = nsfwScore, spam = spamScore, hate = hateScore },
            safe_for_work = safeForWork
        });
    }

    private static HttpClient BuildMockedHttpClient(
        string responseBody,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            });

        return new HttpClient(handlerMock.Object);
    }

    private static ContentModClient BuildClient(
        string responseBody,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var http = BuildMockedHttpClient(responseBody, statusCode);
        return new ContentModClient(new ContentModOptions { ApiKey = "test-key" }, http);
    }

    private static ContentModClient BuildSafeClient() =>
        BuildClient(MakeModerationJson(flagged: false, safeForWork: true));

    private static ContentModClient BuildFlaggedClient() =>
        BuildClient(MakeModerationJson(
            flagged: true, toxic: true,
            toxicScore: 0.95,
            safeForWork: false));

    // =========================================================================
    // 1. Constructor validation
    // =========================================================================

    [Fact]
    public void Constructor_WithValidApiKey_CreatesClient()
    {
        var client = new ContentModClient("test-key");
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithOptions_CreatesClient()
    {
        var opts = new ContentModOptions { ApiKey = "test-key" };
        var client = new ContentModClient(opts);
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        var act = () => new ContentModClient((ContentModOptions)null!);
        act.Should().Throw<ArgumentNullException>().WithMessage("*options*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_NullOrEmptyApiKey_ThrowsArgumentException(string? key)
    {
        var opts = new ContentModOptions { ApiKey = key! };
        var act = () => new ContentModClient(opts);
        act.Should().Throw<ArgumentException>().WithMessage("*API key*");
    }

    [Fact]
    public void Constructor_InvalidBaseUrl_ThrowsArgumentException()
    {
        var opts = new ContentModOptions { ApiKey = "key", BaseUrl = "not-a-url" };
        var act = () => new ContentModClient(opts);
        act.Should().Throw<ArgumentException>().WithMessage("*BaseUrl*");
    }

    [Fact]
    public void Constructor_EmptyBaseUrl_ThrowsArgumentException()
    {
        var opts = new ContentModOptions { ApiKey = "key", BaseUrl = "" };
        var act = () => new ContentModClient(opts);
        act.Should().Throw<ArgumentException>().WithMessage("*BaseUrl*");
    }

    // =========================================================================
    // 2. ModerateAsync (single) — happy path
    // =========================================================================

    [Fact]
    public async Task ModerateAsync_ValidText_ReturnsResult()
    {
        var client = BuildSafeClient();
        var result = await client.ModerateAsync("Hello world");
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ModerateAsync_SafeText_FlaggedIsFalse()
    {
        var client = BuildSafeClient();
        var result = await client.ModerateAsync("Hello world");
        result.Flagged.Should().BeFalse();
    }

    [Fact]
    public async Task ModerateAsync_SafeText_SafeForWorkIsTrue()
    {
        var client = BuildSafeClient();
        var result = await client.ModerateAsync("Hello world");
        result.SafeForWork.Should().BeTrue();
    }

    [Fact]
    public async Task ModerateAsync_ToxicText_FlaggedIsTrue()
    {
        var client = BuildFlaggedClient();
        var result = await client.ModerateAsync("Some toxic text");
        result.Flagged.Should().BeTrue();
    }

    [Fact]
    public async Task ModerateAsync_ToxicText_ToxicCategoryFlagged()
    {
        var client = BuildFlaggedClient();
        var result = await client.ModerateAsync("Some toxic text");
        result.Categories.Toxic.Should().BeTrue();
    }

    [Fact]
    public async Task ModerateAsync_SafeText_NoCategoriesFlagged()
    {
        var client = BuildSafeClient();
        var result = await client.ModerateAsync("Hello world");
        result.Categories.AnyFlagged.Should().BeFalse();
        result.Categories.FlaggedCategories().Should().BeEmpty();
    }

    [Fact]
    public async Task ModerateAsync_PopulatesInputText()
    {
        const string text = "Test input text";
        var client = BuildSafeClient();
        var result = await client.ModerateAsync(text);
        result.InputText.Should().Be(text);
    }

    [Fact]
    public async Task ModerateAsync_PopulatesProcessingMs()
    {
        var client = BuildSafeClient();
        var result = await client.ModerateAsync("Hello");
        result.ProcessingMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task ModerateAsync_ScoresPopulated()
    {
        var client = BuildClient(MakeModerationJson(
            toxicScore: 0.95, nsfwScore: 0.02, spamScore: 0.05, hateScore: 0.01));
        var result = await client.ModerateAsync("test");
        result.Scores.Toxic.Should().BeApproximately(0.95, 0.001);
        result.Scores.Nsfw.Should().BeApproximately(0.02, 0.001);
        result.Scores.Spam.Should().BeApproximately(0.05, 0.001);
        result.Scores.Hate.Should().BeApproximately(0.01, 0.001);
    }

    [Fact]
    public async Task ModerateAsync_NsfwFlagged_NsfwCategoryTrue()
    {
        var client = BuildClient(MakeModerationJson(flagged: true, nsfw: true, nsfwScore: 0.9, safeForWork: false));
        var result = await client.ModerateAsync("nsfw content");
        result.Categories.Nsfw.Should().BeTrue();
    }

    [Fact]
    public async Task ModerateAsync_SpamFlagged_SpamCategoryTrue()
    {
        var client = BuildClient(MakeModerationJson(flagged: true, spam: true, spamScore: 0.88, safeForWork: false));
        var result = await client.ModerateAsync("buy now!!!");
        result.Categories.Spam.Should().BeTrue();
    }

    [Fact]
    public async Task ModerateAsync_HateFlagged_HateCategoryTrue()
    {
        var client = BuildClient(MakeModerationJson(flagged: true, hate: true, hateScore: 0.92, safeForWork: false));
        var result = await client.ModerateAsync("hate speech text");
        result.Categories.Hate.Should().BeTrue();
    }

    // =========================================================================
    // 3. Input validation
    // =========================================================================

    [Fact]
    public async Task ModerateAsync_NullText_ThrowsArgumentNullException()
    {
        var client = BuildSafeClient();
        var act = async () => await client.ModerateAsync((string)null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ModerateAsync_EmptyOrWhitespace_ThrowsArgumentException(string text)
    {
        var client = BuildSafeClient();
        var act = async () => await client.ModerateAsync(text);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    // =========================================================================
    // 4. HTTP error handling
    // =========================================================================

    [Fact]
    public async Task ModerateAsync_Http401_ThrowsUnauthorizedAccessException()
    {
        var client = BuildClient("{\"error\":\"Unauthorized\"}", HttpStatusCode.Unauthorized);
        var act = async () => await client.ModerateAsync("test");
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task ModerateAsync_Http429_ThrowsInvalidOperationException()
    {
        var client = BuildClient("{\"error\":\"Too Many Requests\"}", (HttpStatusCode)429);
        var act = async () => await client.ModerateAsync("test");
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Rate limit*");
    }

    [Fact]
    public async Task ModerateAsync_Http500_ThrowsHttpRequestException()
    {
        var client = BuildClient("{\"error\":\"Internal Server Error\"}", HttpStatusCode.InternalServerError);
        var act = async () => await client.ModerateAsync("test");
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task ModerateAsync_InvalidJson_ThrowsFormatException()
    {
        var client = BuildClient("NOT JSON{{{{");
        var act = async () => await client.ModerateAsync("test");
        await act.Should().ThrowAsync<FormatException>();
    }

    // =========================================================================
    // 5. Cancellation
    // =========================================================================

    [Fact]
    public async Task ModerateAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        var http = new HttpClient(handlerMock.Object);
        var client = new ContentModClient(new ContentModOptions { ApiKey = "key" }, http);

        var act = async () => await client.ModerateAsync("test", null, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ModerateAsync_Timeout_ThrowsTaskCanceledException()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Simulated timeout"));

        var http = new HttpClient(handlerMock.Object);
        var client = new ContentModClient(new ContentModOptions { ApiKey = "key" }, http);

        var act = async () => await client.ModerateAsync("test");
        await act.Should().ThrowAsync<TaskCanceledException>();
    }

    // =========================================================================
    // 6. Batch moderation (IEnumerable overload)
    // =========================================================================

    [Fact]
    public async Task ModerateAsync_BatchTexts_ReturnsCorrectCount()
    {
        var client = BuildSafeClient();
        var texts = new[] { "text one", "text two", "text three" };
        var results = await client.ModerateAsync(texts);
        results.Should().HaveCount(3);
    }

    [Fact]
    public async Task ModerateAsync_BatchNullTexts_ThrowsArgumentNullException()
    {
        var client = BuildSafeClient();
        var act = async () => await client.ModerateAsync((IEnumerable<string>)null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ModerateAsync_BatchEmptyList_ReturnsEmptyList()
    {
        var client = BuildSafeClient();
        var results = await client.ModerateAsync(Array.Empty<string>());
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task ModerateAsync_BatchSingleItem_ReturnsSingleResult()
    {
        var client = BuildFlaggedClient();
        var results = await client.ModerateAsync(new[] { "toxic text" });
        results.Should().HaveCount(1);
        results[0].Flagged.Should().BeTrue();
    }

    [Fact]
    public async Task ModerateAsync_BatchPreservesOrder()
    {
        var client = BuildSafeClient();
        var texts = new[] { "alpha", "beta", "gamma" };
        var results = await client.ModerateAsync(texts);
        for (int i = 0; i < texts.Length; i++)
            results[i].InputText.Should().Be(texts[i]);
    }

    // =========================================================================
    // 7. Categories option / override
    // =========================================================================

    [Fact]
    public async Task ModerateAsync_WithCategoryOverride_SendsCategoriesInRequest()
    {
        string? capturedBody = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedBody = await req.Content!.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(MakeModerationJson(), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var client = new ContentModClient(new ContentModOptions { ApiKey = "key" }, http);

        await client.ModerateAsync("test", new[] { "toxic", "hate" });

        capturedBody.Should().NotBeNull();
        capturedBody!.Should().Contain("toxic");
        capturedBody.Should().Contain("hate");
    }

    [Fact]
    public async Task ModerateAsync_WithOptionsCategories_SendsCategoriesInRequest()
    {
        string? capturedBody = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedBody = await req.Content!.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(MakeModerationJson(), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new ContentModOptions
        {
            ApiKey = "key",
            Categories = new[] { "nsfw", "spam" }
        };
        var client = new ContentModClient(opts, http);

        await client.ModerateAsync("test");

        capturedBody.Should().NotBeNull();
        capturedBody!.Should().Contain("nsfw");
    }

    // =========================================================================
    // 8. HTTP request shape
    // =========================================================================

    [Fact]
    public async Task ModerateAsync_SendsBearerAuthorizationHeader()
    {
        HttpRequestMessage? capturedRequest = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(MakeModerationJson(), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var client = new ContentModClient(new ContentModOptions { ApiKey = "my-secret-key" }, http);

        await client.ModerateAsync("test");

        capturedRequest.Should().NotBeNull();
        capturedRequest!.Headers.Authorization!.Scheme.Should().Be("Bearer");
        capturedRequest.Headers.Authorization.Parameter.Should().Be("my-secret-key");
    }

    [Fact]
    public async Task ModerateAsync_PostsToCorrectEndpoint()
    {
        Uri? capturedUri = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedUri = req.RequestUri;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(MakeModerationJson(), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var client = new ContentModClient(new ContentModOptions { ApiKey = "key" }, http);

        await client.ModerateAsync("test");

        capturedUri.Should().NotBeNull();
        capturedUri!.AbsolutePath.Should().Contain("moderation");
    }

    [Fact]
    public async Task ModerateAsync_UsesHttpPost()
    {
        HttpMethod? capturedMethod = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedMethod = req.Method;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(MakeModerationJson(), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var client = new ContentModClient(new ContentModOptions { ApiKey = "key" }, http);

        await client.ModerateAsync("test");

        capturedMethod.Should().Be(HttpMethod.Post);
    }

    [Fact]
    public async Task ModerateAsync_SendsTextInBody()
    {
        string? capturedBody = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedBody = await req.Content!.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(MakeModerationJson(), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var client = new ContentModClient(new ContentModOptions { ApiKey = "key" }, http);

        await client.ModerateAsync("hello world");

        capturedBody.Should().Contain("hello world");
    }

    // =========================================================================
    // 9. ContentModOptions
    // =========================================================================

    [Fact]
    public void ContentModOptions_HasCorrectDefaults()
    {
        var opts = new ContentModOptions();
        opts.BaseUrl.Should().Be("https://api.aimlapi.com");
        opts.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        opts.ApiKey.Should().BeEmpty();
        opts.Categories.Should().BeNull();
    }

    [Fact]
    public void ContentModOptions_FromEnvironment_MissingVar_Throws()
    {
        Environment.SetEnvironmentVariable("CONTENTMOD_TEST_MISSING", null);
        var act = () => ContentModOptions.FromEnvironment("CONTENTMOD_TEST_MISSING");
        act.Should().Throw<InvalidOperationException>().WithMessage("*not set*");
    }

    [Fact]
    public void ContentModOptions_FromEnvironment_ReadsApiKey()
    {
        const string envVar = "CONTENTMOD_TEST_KEY";
        Environment.SetEnvironmentVariable(envVar, "my-key");
        try
        {
            var opts = ContentModOptions.FromEnvironment(envVar);
            opts.ApiKey.Should().Be("my-key");
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    [Fact]
    public void ContentModOptions_AllCategories_ContainsFourEntries()
    {
        ContentModOptions.AllCategories.Should().HaveCount(4);
        ContentModOptions.AllCategories.Should().Contain("toxic");
        ContentModOptions.AllCategories.Should().Contain("nsfw");
        ContentModOptions.AllCategories.Should().Contain("spam");
        ContentModOptions.AllCategories.Should().Contain("hate");
    }

    [Fact]
    public void ContentModOptions_CustomTimeout_IsRespected()
    {
        var opts = new ContentModOptions { ApiKey = "key", Timeout = TimeSpan.FromSeconds(10) };
        opts.Timeout.TotalSeconds.Should().Be(10);
    }

    // =========================================================================
    // 10. FromEnvironment on client
    // =========================================================================

    [Fact]
    public void FromEnvironment_MissingEnvVar_Throws()
    {
        Environment.SetEnvironmentVariable("CM_MISSING_KEY", null);
        var act = () => ContentModClient.FromEnvironment("CM_MISSING_KEY");
        act.Should().Throw<InvalidOperationException>().WithMessage("*not set*");
    }

    [Fact]
    public void FromEnvironment_SetEnvVar_CreatesClient()
    {
        const string envVar = "CM_TEST_KEY_SET";
        Environment.SetEnvironmentVariable(envVar, "fake-api-key");
        try
        {
            var client = ContentModClient.FromEnvironment(envVar);
            client.Should().NotBeNull();
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    // =========================================================================
    // 11. CategoryFlags model
    // =========================================================================

    [Fact]
    public void CategoryFlags_NoFlagsSet_AnyFlaggedIsFalse()
    {
        var flags = new CategoryFlags();
        flags.AnyFlagged.Should().BeFalse();
    }

    [Fact]
    public void CategoryFlags_ToxicSet_AnyFlaggedIsTrue()
    {
        var flags = new CategoryFlags { Toxic = true };
        flags.AnyFlagged.Should().BeTrue();
    }

    [Fact]
    public void CategoryFlags_NsfwSet_FlaggedCategoriesContainsNsfw()
    {
        var flags = new CategoryFlags { Nsfw = true };
        flags.FlaggedCategories().Should().Contain("nsfw");
    }

    [Fact]
    public void CategoryFlags_MultipleSet_FlaggedCategoriesReturnsAll()
    {
        var flags = new CategoryFlags { Toxic = true, Spam = true };
        var cats = flags.FlaggedCategories();
        cats.Should().Contain("toxic");
        cats.Should().Contain("spam");
        cats.Should().HaveCount(2);
    }

    [Fact]
    public void CategoryFlags_NoneSet_FlaggedCategoriesEmpty()
    {
        var flags = new CategoryFlags();
        flags.FlaggedCategories().Should().BeEmpty();
    }

    [Fact]
    public void CategoryFlags_AllSet_FlaggedCategoriesHasFour()
    {
        var flags = new CategoryFlags { Toxic = true, Nsfw = true, Spam = true, Hate = true };
        flags.FlaggedCategories().Should().HaveCount(4);
    }

    // =========================================================================
    // 12. CategoryScores model
    // =========================================================================

    [Fact]
    public void CategoryScores_Dominant_ReturnsHighestCategory()
    {
        var scores = new CategoryScores { Toxic = 0.9, Nsfw = 0.1, Spam = 0.2, Hate = 0.05 };
        scores.Dominant.Should().Be("toxic");
    }

    [Fact]
    public void CategoryScores_Dominant_NsfwHighest()
    {
        var scores = new CategoryScores { Toxic = 0.1, Nsfw = 0.95, Spam = 0.2, Hate = 0.05 };
        scores.Dominant.Should().Be("nsfw");
    }

    [Fact]
    public void CategoryScores_Dominant_SpamHighest()
    {
        var scores = new CategoryScores { Toxic = 0.1, Nsfw = 0.2, Spam = 0.85, Hate = 0.05 };
        scores.Dominant.Should().Be("spam");
    }

    [Fact]
    public void CategoryScores_Dominant_HateHighest()
    {
        var scores = new CategoryScores { Toxic = 0.1, Nsfw = 0.1, Spam = 0.1, Hate = 0.99 };
        scores.Dominant.Should().Be("hate");
    }

    [Fact]
    public void CategoryScores_MaxScore_ReturnsHighestValue()
    {
        var scores = new CategoryScores { Toxic = 0.9, Nsfw = 0.1, Spam = 0.2, Hate = 0.05 };
        scores.MaxScore.Should().BeApproximately(0.9, 0.001);
    }

    [Fact]
    public void CategoryScores_AllZero_MaxScoreIsZero()
    {
        var scores = new CategoryScores();
        scores.MaxScore.Should().Be(0.0);
    }

    // =========================================================================
    // 13. ModerationResult model
    // =========================================================================

    [Fact]
    public void ModerationResult_DefaultConstruction_HasSafeDefaults()
    {
        var result = new ModerationResult();
        result.Flagged.Should().BeFalse();
        result.SafeForWork.Should().BeFalse();
        result.InputText.Should().BeEmpty();
        result.ProcessingMs.Should().Be(0);
        result.Categories.Should().NotBeNull();
        result.Scores.Should().NotBeNull();
    }

    // =========================================================================
    // 14. DI registration
    // =========================================================================

    [Fact]
    public void AddContentMod_WithApiKey_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddContentMod("test-key");
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<ContentModClient>();
        client.Should().NotBeNull();
    }

    [Fact]
    public void AddContentMod_WithOptions_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddContentMod(opts =>
        {
            opts.ApiKey = "test-key";
            opts.Timeout = TimeSpan.FromSeconds(15);
        });
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<ContentModClient>();
        client.Should().NotBeNull();
    }

    [Fact]
    public void AddContentMod_WithCategories_RegistersClientSuccessfully()
    {
        var services = new ServiceCollection();
        services.AddContentMod(opts =>
        {
            opts.ApiKey = "test-key";
            opts.Categories = new[] { "toxic", "hate" };
        });
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<ContentModClient>();
        client.Should().NotBeNull();
    }

    // =========================================================================
    // 15. Dispose
    // =========================================================================

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var client = new ContentModClient("test-key");
        client.Dispose();
        var act = () => client.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_WithInjectedHttpClient_DoesNotDisposeIt()
    {
        var http = new HttpClient();
        var client = new ContentModClient(new ContentModOptions { ApiKey = "key" }, http);
        client.Dispose();
        // HttpClient is still usable (not disposed by ContentModClient)
        var act = () => _ = http.BaseAddress;
        act.Should().NotThrow();
    }

    // =========================================================================
    // 16. Large text
    // =========================================================================

    [Fact]
    public async Task ModerateAsync_LargeText_HandledWithoutError()
    {
        var largeText = new string('a', 5000);
        var client = BuildSafeClient();
        var result = await client.ModerateAsync(largeText);
        result.InputText.Length.Should().Be(5000);
    }

    // =========================================================================
    // 17. Partial API response (missing fields fall back to defaults)
    // =========================================================================

    [Fact]
    public async Task ModerateAsync_PartialResponse_FallsBackGracefully()
    {
        // API returns only flagged, no categories or scores
        var partialJson = "{\"flagged\":true}";
        var client = BuildClient(partialJson);
        var result = await client.ModerateAsync("test");
        result.Flagged.Should().BeTrue();
        result.Categories.Should().NotBeNull();
        result.Scores.Should().NotBeNull();
    }

    // =========================================================================
    // 18. Custom base URL
    // =========================================================================

    [Fact]
    public async Task ModerateAsync_CustomBaseUrl_SendsToCorrectHost()
    {
        Uri? capturedUri = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedUri = req.RequestUri;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(MakeModerationJson(), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new ContentModOptions
        {
            ApiKey  = "key",
            BaseUrl = "https://custom.example.com"
        };
        var client = new ContentModClient(opts, http);

        await client.ModerateAsync("test");

        capturedUri!.Host.Should().Be("custom.example.com");
    }
}
