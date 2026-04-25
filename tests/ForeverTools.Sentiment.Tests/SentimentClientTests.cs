using System.Net;
using System.Text;
using System.Text.Json;
using ForeverTools.Sentiment;
using ForeverTools.Sentiment.Extensions;
using ForeverTools.Sentiment.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace ForeverTools.Sentiment.Tests;

/// <summary>
/// Unit tests for SentimentClient. All HTTP calls are mocked — no real API calls are made.
/// </summary>
public class SentimentClientTests
{
    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static HttpClient BuildMockedHttpClient(
        string sentimentJson,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var wrapper = new
        {
            choices = new[]
            {
                new
                {
                    message = new { role = "assistant", content = sentimentJson }
                }
            }
        };

        var responseBody = JsonSerializer.Serialize(wrapper);

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

    private static HttpClient BuildRawMockedHttpClient(
        string rawBody,
        HttpStatusCode statusCode)
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
                Content = new StringContent(rawBody, Encoding.UTF8, "application/json")
            });

        return new HttpClient(handlerMock.Object);
    }

    private static SentimentClient BuildClient(string sentimentJson, HttpStatusCode code = HttpStatusCode.OK)
    {
        var http = BuildMockedHttpClient(sentimentJson, code);
        var opts = new SentimentOptions { ApiKey = "test-key" };
        return new SentimentClient(opts, http);
    }

    private static string MakeSentimentJson(
        string label = "positive",
        double confidence = 0.92,
        double joy = 0.78,
        double anger = 0.02,
        double sadness = 0.05,
        double fear = 0.03,
        double surprise = 0.12,
        double disgust = 0.00,
        string summary = "Enthusiastic and optimistic tone")
    {
        return $"{{\"label\":\"{label}\",\"confidence\":{confidence}," +
               $"\"emotions\":{{\"joy\":{joy},\"anger\":{anger},\"sadness\":{sadness}," +
               $"\"fear\":{fear},\"surprise\":{surprise},\"disgust\":{disgust}}}," +
               $"\"summary\":\"{summary}\"}}";
    }

    // -------------------------------------------------------------------------
    // 1. Constructor validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_WithValidApiKey_CreatesClient()
    {
        var client = new SentimentClient("test-key");
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithOptions_CreatesClient()
    {
        var opts = new SentimentOptions { ApiKey = "test-key", Model = "gpt-4o-mini" };
        var client = new SentimentClient(opts);
        client.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_NullOrEmptyApiKey_ThrowsArgumentException(string? key)
    {
        var opts = new SentimentOptions { ApiKey = key! };
        var act = () => new SentimentClient(opts);
        act.Should().Throw<ArgumentException>().WithMessage("*API key*");
    }

    [Fact]
    public void Constructor_EmptyModel_ThrowsArgumentException()
    {
        var opts = new SentimentOptions { ApiKey = "key", Model = "" };
        var act = () => new SentimentClient(opts);
        act.Should().Throw<ArgumentException>().WithMessage("*Model*");
    }

    [Fact]
    public void Constructor_InvalidUrl_ThrowsArgumentException()
    {
        var opts = new SentimentOptions { ApiKey = "key", BaseUrl = "not-a-url" };
        var act = () => new SentimentClient(opts);
        act.Should().Throw<ArgumentException>().WithMessage("*BaseUrl*");
    }

    // -------------------------------------------------------------------------
    // 2. AnalyzeAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AnalyzeAsync_ValidText_ReturnsResult()
    {
        var client = BuildClient(MakeSentimentJson());
        var result = await client.AnalyzeAsync("I love this!");
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AnalyzeAsync_PopulatesInputText()
    {
        const string text = "Hello world";
        var client = BuildClient(MakeSentimentJson());
        var result = await client.AnalyzeAsync(text);
        result.InputText.Should().Be(text);
    }

    [Fact]
    public async Task AnalyzeAsync_PopulatesProcessingMs()
    {
        var client = BuildClient(MakeSentimentJson());
        var result = await client.AnalyzeAsync("Test");
        result.ProcessingMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task AnalyzeAsync_PositiveLabel_MapsCorrectly()
    {
        var client = BuildClient(MakeSentimentJson("positive"));
        var result = await client.AnalyzeAsync("Great!");
        result.Label.Should().Be(SentimentLabel.Positive);
    }

    [Fact]
    public async Task AnalyzeAsync_NegativeLabel_MapsCorrectly()
    {
        var client = BuildClient(MakeSentimentJson("negative"));
        var result = await client.AnalyzeAsync("Terrible!");
        result.Label.Should().Be(SentimentLabel.Negative);
    }

    [Fact]
    public async Task AnalyzeAsync_NeutralLabel_MapsCorrectly()
    {
        var client = BuildClient(MakeSentimentJson("neutral"));
        var result = await client.AnalyzeAsync("It was okay.");
        result.Label.Should().Be(SentimentLabel.Neutral);
    }

    [Fact]
    public async Task AnalyzeAsync_MixedLabel_MapsCorrectly()
    {
        var client = BuildClient(MakeSentimentJson("mixed"));
        var result = await client.AnalyzeAsync("Good product, bad service.");
        result.Label.Should().Be(SentimentLabel.Mixed);
    }

    [Fact]
    public async Task AnalyzeAsync_ConfidenceIsBetween0And1()
    {
        var client = BuildClient(MakeSentimentJson(confidence: 0.87));
        var result = await client.AnalyzeAsync("Test");
        result.Confidence.Should().BeInRange(0, 1);
    }

    [Fact]
    public async Task AnalyzeAsync_AllEmotionScoresReturnedCorrectly()
    {
        var client = BuildClient(MakeSentimentJson(joy: 0.8, anger: 0.1, sadness: 0.05, fear: 0.02, surprise: 0.03, disgust: 0.0));
        var result = await client.AnalyzeAsync("Test");
        result.Emotions.Joy.Should().BeApproximately(0.8, 0.001);
        result.Emotions.Anger.Should().BeApproximately(0.1, 0.001);
        result.Emotions.Sadness.Should().BeApproximately(0.05, 0.001);
        result.Emotions.Fear.Should().BeApproximately(0.02, 0.001);
        result.Emotions.Surprise.Should().BeApproximately(0.03, 0.001);
        result.Emotions.Disgust.Should().BeApproximately(0.0, 0.001);
    }

    [Fact]
    public async Task AnalyzeAsync_AllEmotionScoresBetween0And1()
    {
        var client = BuildClient(MakeSentimentJson());
        var result = await client.AnalyzeAsync("Test");
        result.Emotions.Joy.Should().BeInRange(0, 1);
        result.Emotions.Anger.Should().BeInRange(0, 1);
        result.Emotions.Sadness.Should().BeInRange(0, 1);
        result.Emotions.Fear.Should().BeInRange(0, 1);
        result.Emotions.Surprise.Should().BeInRange(0, 1);
        result.Emotions.Disgust.Should().BeInRange(0, 1);
    }

    [Fact]
    public async Task AnalyzeAsync_SummaryIsPopulated()
    {
        var client = BuildClient(MakeSentimentJson(summary: "Very enthusiastic"));
        var result = await client.AnalyzeAsync("Test");
        result.Summary.Should().Be("Very enthusiastic");
    }

    // -------------------------------------------------------------------------
    // 3. EmotionScores.Dominant
    // -------------------------------------------------------------------------

    [Fact]
    public void EmotionScores_Dominant_ReturnsHighestScoringEmotion()
    {
        var emotions = new EmotionScores { Joy = 0.8, Anger = 0.1, Sadness = 0.05, Fear = 0.02, Surprise = 0.03, Disgust = 0.0 };
        emotions.Dominant.Should().Be("Joy");
    }

    [Fact]
    public void EmotionScores_Dominant_HandlesAngerBest()
    {
        var emotions = new EmotionScores { Joy = 0.1, Anger = 0.9, Sadness = 0.05, Fear = 0.02, Surprise = 0.03, Disgust = 0.0 };
        emotions.Dominant.Should().Be("Anger");
    }

    [Fact]
    public void EmotionScores_Dominant_HandlesDisgust()
    {
        var emotions = new EmotionScores { Joy = 0.1, Anger = 0.1, Sadness = 0.1, Fear = 0.1, Surprise = 0.1, Disgust = 0.9 };
        emotions.Dominant.Should().Be("Disgust");
    }

    // -------------------------------------------------------------------------
    // 4. AnalyzeBatchAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AnalyzeBatchAsync_ReturnsCorrectCount()
    {
        var http = BuildMockedHttpClient(MakeSentimentJson());
        var opts = new SentimentOptions { ApiKey = "test-key" };
        var client = new SentimentClient(opts, http);

        var texts = new[] { "Text 1", "Text 2", "Text 3" };
        var results = await client.AnalyzeBatchAsync(texts);
        results.Should().HaveCount(3);
    }

    [Fact]
    public async Task AnalyzeBatchAsync_NullTexts_ThrowsArgumentNullException()
    {
        var client = BuildClient(MakeSentimentJson());
        var act = async () => await client.AnalyzeBatchAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AnalyzeBatchAsync_AllPositive_OverallLabelIsPositive()
    {
        var http = BuildMockedHttpClient(MakeSentimentJson("positive", 0.9));
        var opts = new SentimentOptions { ApiKey = "test-key" };
        var sentimentClient = new SentimentClient(opts, http);

        var results = await sentimentClient.AnalyzeBatchAsync(new[] { "Great!", "Awesome!", "Love it!" });

        // Manually build BatchSentimentResult (the client returns IReadOnlyList, not BatchSentimentResult)
        var labels = results.Select(r => r.Label).Distinct().ToList();
        var overallLabel = labels.Count == 1 ? labels[0] : SentimentLabel.Mixed;
        overallLabel.Should().Be(SentimentLabel.Positive);
    }

    [Fact]
    public async Task AnalyzeBatchAsync_AverageConfidence_IsCorrect()
    {
        var http = BuildMockedHttpClient(MakeSentimentJson("positive", 0.8));
        var opts = new SentimentOptions { ApiKey = "test-key" };
        var sentimentClient = new SentimentClient(opts, http);

        var results = await sentimentClient.AnalyzeBatchAsync(new[] { "A", "B" });
        var avg = results.Average(r => r.Confidence);
        avg.Should().BeApproximately(0.8, 0.001);
    }

    // -------------------------------------------------------------------------
    // 5. Input validation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AnalyzeAsync_NullText_ThrowsArgumentNullException()
    {
        var client = BuildClient(MakeSentimentJson());
        var act = async () => await client.AnalyzeAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AnalyzeAsync_EmptyOrWhitespaceText_ThrowsArgumentException(string text)
    {
        var client = BuildClient(MakeSentimentJson());
        var act = async () => await client.AnalyzeAsync(text);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    // -------------------------------------------------------------------------
    // 6. HTTP error handling
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AnalyzeAsync_Http401_ThrowsUnauthorizedAccessException()
    {
        var http = BuildRawMockedHttpClient("{\"error\":\"Unauthorized\"}", HttpStatusCode.Unauthorized);
        var opts = new SentimentOptions { ApiKey = "bad-key" };
        var client = new SentimentClient(opts, http);

        var act = async () => await client.AnalyzeAsync("Test");
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task AnalyzeAsync_Http429_ThrowsRateLimitException()
    {
        var http = BuildRawMockedHttpClient("{\"error\":\"Too Many Requests\"}", (HttpStatusCode)429);
        var opts = new SentimentOptions { ApiKey = "test-key" };
        var client = new SentimentClient(opts, http);

        var act = async () => await client.AnalyzeAsync("Test");
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Rate limit*");
    }

    [Fact]
    public async Task AnalyzeAsync_InvalidJsonInContent_ThrowsFormatException()
    {
        // The API wrapper returns valid JSON but the inner content is malformed.
        var wrapper = new
        {
            choices = new[]
            {
                new { message = new { role = "assistant", content = "not-json-at-all{{{" } }
            }
        };
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(wrapper), Encoding.UTF8, "application/json")
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new SentimentOptions { ApiKey = "test-key" };
        var client = new SentimentClient(opts, http);

        var act = async () => await client.AnalyzeAsync("Test");
        await act.Should().ThrowAsync<FormatException>();
    }

    [Fact]
    public async Task AnalyzeAsync_InvalidOuterJson_ThrowsFormatException()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("DEFINITELY NOT JSON", Encoding.UTF8, "application/json")
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new SentimentOptions { ApiKey = "test-key" };
        var client = new SentimentClient(opts, http);

        var act = async () => await client.AnalyzeAsync("Test");
        await act.Should().ThrowAsync<FormatException>();
    }

    // -------------------------------------------------------------------------
    // 7. Cancellation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AnalyzeAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        var http = new HttpClient(handlerMock.Object);
        var opts = new SentimentOptions { ApiKey = "test-key" };
        var client = new SentimentClient(opts, http);

        var act = async () => await client.AnalyzeAsync("Test", cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task AnalyzeAsync_Timeout_ThrowsTaskCanceledException()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Timeout"));

        var http = new HttpClient(handlerMock.Object);
        var opts = new SentimentOptions { ApiKey = "test-key" };
        var client = new SentimentClient(opts, http);

        var act = async () => await client.AnalyzeAsync("Test");
        await act.Should().ThrowAsync<TaskCanceledException>();
    }

    // -------------------------------------------------------------------------
    // 8. AnalyzeWithContextAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AnalyzeWithContextAsync_ValidInputs_ReturnsResult()
    {
        var client = BuildClient(MakeSentimentJson());
        var result = await client.AnalyzeWithContextAsync("I love it!", "This is a product review.");
        result.Should().NotBeNull();
        result.Label.Should().Be(SentimentLabel.Positive);
    }

    [Fact]
    public async Task AnalyzeWithContextAsync_PopulatesInputText()
    {
        const string text = "Not bad at all";
        var client = BuildClient(MakeSentimentJson("neutral", 0.65));
        var result = await client.AnalyzeWithContextAsync(text, "customer feedback");
        result.InputText.Should().Be(text);
    }

    [Fact]
    public async Task AnalyzeWithContextAsync_VerifiesContextSentToApi()
    {
        string? capturedBody = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedBody = await req.Content!.ReadAsStringAsync();
                var wrapper = new { choices = new[] { new { message = new { role = "assistant", content = MakeSentimentJson() } } } };
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(wrapper), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new SentimentOptions { ApiKey = "test-key" };
        var client = new SentimentClient(opts, http);

        await client.AnalyzeWithContextAsync("Great product", "product review context");

        capturedBody.Should().NotBeNull();
        capturedBody!.Should().Contain("product review context");
    }

    // -------------------------------------------------------------------------
    // 9. Large text
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AnalyzeAsync_LargeText_HandledWithoutError()
    {
        var largeText = new string('a', 5000);
        var client = BuildClient(MakeSentimentJson("neutral", 0.5));
        var result = await client.AnalyzeAsync(largeText);
        result.Should().NotBeNull();
        result.InputText.Length.Should().Be(5000);
    }

    // -------------------------------------------------------------------------
    // 10. FromEnvironment
    // -------------------------------------------------------------------------

    [Fact]
    public void FromEnvironment_MissingEnvVar_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("SENTIMENT_TEST_MISSING_KEY", null);
        var act = () => SentimentClient.FromEnvironment("SENTIMENT_TEST_MISSING_KEY");
        act.Should().Throw<InvalidOperationException>().WithMessage("*not set*");
    }

    [Fact]
    public void FromEnvironment_SetEnvVar_CreatesClient()
    {
        const string envVar = "SENTIMENT_TEST_KEY_SET";
        Environment.SetEnvironmentVariable(envVar, "fake-api-key");
        try
        {
            var client = SentimentClient.FromEnvironment(envVar);
            client.Should().NotBeNull();
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    [Fact]
    public void SentimentOptions_FromEnvironment_MissingVar_Throws()
    {
        Environment.SetEnvironmentVariable("SENTIMENT_OPTS_MISSING", null);
        var act = () => SentimentOptions.FromEnvironment("SENTIMENT_OPTS_MISSING");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SentimentOptions_FromEnvironment_ReadsApiKey()
    {
        const string envVar = "SENTIMENT_OPTS_KEY";
        Environment.SetEnvironmentVariable(envVar, "my-key");
        try
        {
            var opts = SentimentOptions.FromEnvironment(envVar);
            opts.ApiKey.Should().Be("my-key");
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    // -------------------------------------------------------------------------
    // 11. SentimentOptions defaults
    // -------------------------------------------------------------------------

    [Fact]
    public void SentimentOptions_HasCorrectDefaults()
    {
        var opts = new SentimentOptions();
        opts.Model.Should().Be("gpt-4o-mini");
        opts.BaseUrl.Should().Be("https://api.aimlapi.com");
        opts.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        opts.ApiKey.Should().BeEmpty();
    }

    // -------------------------------------------------------------------------
    // 12. DI / ServiceCollection extension
    // -------------------------------------------------------------------------

    [Fact]
    public void AddSentimentClient_WithApiKey_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddSentimentClient("test-key");
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SentimentClient>();
        client.Should().NotBeNull();
    }

    [Fact]
    public void AddSentimentClient_WithOptions_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddSentimentClient(opts =>
        {
            opts.ApiKey = "test-key";
            opts.Model = "gpt-4o-mini";
        });
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SentimentClient>();
        client.Should().NotBeNull();
    }

    // -------------------------------------------------------------------------
    // 13. HTTP request shape verification
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AnalyzeAsync_SendsAuthorizationHeader()
    {
        HttpRequestMessage? capturedRequest = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedRequest = req;
                var w = new { choices = new[] { new { message = new { role = "assistant", content = MakeSentimentJson() } } } };
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(w), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new SentimentOptions { ApiKey = "my-secret-key" };
        var client = new SentimentClient(opts, http);

        await client.AnalyzeAsync("Test");

        capturedRequest.Should().NotBeNull();
        capturedRequest!.Headers.Authorization.Should().NotBeNull();
        capturedRequest.Headers.Authorization!.Scheme.Should().Be("Bearer");
        capturedRequest.Headers.Authorization.Parameter.Should().Be("my-secret-key");
    }

    [Fact]
    public async Task AnalyzeAsync_SendsToCorrectEndpoint()
    {
        Uri? capturedUri = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedUri = req.RequestUri;
                var w = new { choices = new[] { new { message = new { role = "assistant", content = MakeSentimentJson() } } } };
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(w), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new SentimentOptions { ApiKey = "test-key" };
        var client = new SentimentClient(opts, http);

        await client.AnalyzeAsync("Test");

        capturedUri.Should().NotBeNull();
        capturedUri!.AbsolutePath.Should().Contain("chat/completions");
    }

    // -------------------------------------------------------------------------
    // 14. BatchSentimentResult helper test
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AnalyzeBatchAsync_EmptyList_ReturnsEmptyList()
    {
        var client = BuildClient(MakeSentimentJson());
        var results = await client.AnalyzeBatchAsync(Array.Empty<string>());
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task AnalyzeBatchAsync_SingleItem_ReturnsSingleResult()
    {
        var http = BuildMockedHttpClient(MakeSentimentJson("negative", 0.75));
        var opts = new SentimentOptions { ApiKey = "test-key" };
        var client = new SentimentClient(opts, http);
        var results = await client.AnalyzeBatchAsync(new[] { "Terrible." });
        results.Should().HaveCount(1);
        results[0].Label.Should().Be(SentimentLabel.Negative);
    }

    // -------------------------------------------------------------------------
    // 15. Dispose
    // -------------------------------------------------------------------------

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var client = new SentimentClient("test-key");
        client.Dispose();
        var act = () => client.Dispose();
        act.Should().NotThrow();
    }

    // -------------------------------------------------------------------------
    // 16. HttpClient timeout option
    // -------------------------------------------------------------------------

    [Fact]
    public void SentimentOptions_CustomTimeout_IsRespected()
    {
        var opts = new SentimentOptions { ApiKey = "key", Timeout = TimeSpan.FromSeconds(5) };
        opts.Timeout.TotalSeconds.Should().Be(5);
    }

    // -------------------------------------------------------------------------
    // 17. Unknown label falls back to Neutral
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AnalyzeAsync_UnknownLabel_FallsBackToNeutral()
    {
        var client = BuildClient(MakeSentimentJson("unknown_label", 0.5));
        var result = await client.AnalyzeAsync("Some text");
        result.Label.Should().Be(SentimentLabel.Neutral);
    }

    // -------------------------------------------------------------------------
    // 18. HttpClientTimeout throws TaskCanceledException
    // -------------------------------------------------------------------------

    [Fact]
    public async Task AnalyzeAsync_HttpClientTimeout_ThrowsTaskCanceled()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Simulated timeout"));

        var http = new HttpClient(handlerMock.Object) { Timeout = TimeSpan.FromMilliseconds(1) };
        var opts = new SentimentOptions { ApiKey = "key" };
        var client = new SentimentClient(opts, http);

        var act = async () => await client.AnalyzeAsync("test");
        await act.Should().ThrowAsync<TaskCanceledException>();
    }
}
