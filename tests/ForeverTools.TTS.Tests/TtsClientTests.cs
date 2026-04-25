using System.Net;
using System.Text;
using System.Text.Json;
using ForeverTools.TTS;
using ForeverTools.TTS.Extensions;
using ForeverTools.TTS.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace ForeverTools.TTS.Tests;

/// <summary>
/// Unit tests for TtsClient. All HTTP calls are mocked — no real API calls are made.
/// </summary>
public class TtsClientTests
{
    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static readonly byte[] FakeAudioBytes = Encoding.UTF8.GetBytes("FAKE_AUDIO_DATA");

    private static HttpClient BuildMockedHttpClient(
        byte[]? responseBytes = null,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var bytes = responseBytes ?? FakeAudioBytes;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = new ByteArrayContent(bytes)
            });
        return new HttpClient(handlerMock.Object);
    }

    private static TtsClient BuildClient(
        byte[]? bytes = null,
        HttpStatusCode code = HttpStatusCode.OK,
        string? apiKey = "test-key")
    {
        var http = BuildMockedHttpClient(bytes, code);
        var opts = new TtsOptions { ApiKey = apiKey! };
        return new TtsClient(opts, http);
    }

    // -------------------------------------------------------------------------
    // 1. Constructor validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_WithValidApiKey_CreatesClient()
    {
        var client = new TtsClient("test-key");
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithOptions_CreatesClient()
    {
        var opts = new TtsOptions { ApiKey = "test-key" };
        var client = new TtsClient(opts);
        client.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_NullOrEmptyApiKey_ThrowsArgumentException(string? key)
    {
        var opts = new TtsOptions { ApiKey = key! };
        var act = () => new TtsClient(opts);
        act.Should().Throw<ArgumentException>().WithMessage("*API key*");
    }

    [Fact]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        var act = () => new TtsClient((TtsOptions)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // -------------------------------------------------------------------------
    // 2. SynthesizeAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SynthesizeAsync_ValidText_ReturnsByteArray()
    {
        var client = BuildClient();
        var bytes = await client.SynthesizeAsync("Hello world");
        bytes.Should().NotBeNull();
        bytes.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SynthesizeAsync_DefaultOptions_UsesAlloyVoice()
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
                    Content = new ByteArrayContent(FakeAudioBytes)
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new TtsOptions { ApiKey = "test-key" };
        var client = new TtsClient(opts, http);

        await client.SynthesizeAsync("Test");

        capturedBody.Should().Contain("\"voice\":\"alloy\"");
    }

    [Fact]
    public async Task SynthesizeAsync_DefaultOptions_UsesTts1Model()
    {
        string? capturedBody = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedBody = await req.Content!.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(FakeAudioBytes) };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new TtsOptions { ApiKey = "test-key" };
        var client = new TtsClient(opts, http);

        await client.SynthesizeAsync("Test");
        capturedBody.Should().Contain("\"model\":\"tts-1\"");
    }

    [Fact]
    public async Task SynthesizeAsync_DefaultOptions_UsesMp3Format()
    {
        string? capturedBody = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedBody = await req.Content!.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(FakeAudioBytes) };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new TtsOptions { ApiKey = "test-key" };
        var client = new TtsClient(opts, http);

        await client.SynthesizeAsync("Test");
        capturedBody.Should().Contain("\"response_format\":\"mp3\"");
    }

    [Fact]
    public async Task SynthesizeAsync_CustomVoice_SendsCorrectVoice()
    {
        string? capturedBody = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedBody = await req.Content!.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(FakeAudioBytes) };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new TtsOptions { ApiKey = "test-key", Voice = TtsVoices.Nova };
        var client = new TtsClient(opts, http);

        await client.SynthesizeAsync("Test");
        capturedBody.Should().Contain("\"voice\":\"nova\"");
    }

    [Fact]
    public async Task SynthesizeAsync_HdModel_SendsCorrectModel()
    {
        string? capturedBody = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedBody = await req.Content!.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(FakeAudioBytes) };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new TtsOptions { ApiKey = "test-key", Model = TtsModels.Tts1Hd };
        var client = new TtsClient(opts, http);

        await client.SynthesizeAsync("Test");
        capturedBody.Should().Contain("\"model\":\"tts-1-hd\"");
    }

    // -------------------------------------------------------------------------
    // 3. TtsFormat content types
    // -------------------------------------------------------------------------

    [Theory]
    [InlineData(TtsFormat.Mp3,  "audio/mpeg")]
    [InlineData(TtsFormat.Opus, "audio/opus")]
    [InlineData(TtsFormat.Aac,  "audio/aac")]
    [InlineData(TtsFormat.Flac, "audio/flac")]
    [InlineData(TtsFormat.Wav,  "audio/wav")]
    [InlineData(TtsFormat.Pcm,  "audio/pcm")]
    public void TtsFormat_ToContentType_ReturnsCorrectMimeType(TtsFormat format, string expectedMime)
    {
        format.ToContentType().Should().Be(expectedMime);
    }

    // -------------------------------------------------------------------------
    // 4. TtsVoices and TtsModels constants
    // -------------------------------------------------------------------------

    [Fact]
    public void TtsVoices_Constants_HaveCorrectValues()
    {
        TtsVoices.Alloy.Should().Be("alloy");
        TtsVoices.Echo.Should().Be("echo");
        TtsVoices.Fable.Should().Be("fable");
        TtsVoices.Onyx.Should().Be("onyx");
        TtsVoices.Nova.Should().Be("nova");
        TtsVoices.Shimmer.Should().Be("shimmer");
    }

    [Fact]
    public void TtsModels_Constants_HaveCorrectValues()
    {
        TtsModels.Tts1.Should().Be("tts-1");
        TtsModels.Tts1Hd.Should().Be("tts-1-hd");
    }

    // -------------------------------------------------------------------------
    // 5. Speed validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_SpeedBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var opts = new TtsOptions { ApiKey = "key", Speed = 0.1f };
        var act = () => new TtsClient(opts);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_SpeedAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var opts = new TtsOptions { ApiKey = "key", Speed = 5.0f };
        var act = () => new TtsClient(opts);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_SpeedAt1_IsValid()
    {
        var opts = new TtsOptions { ApiKey = "key", Speed = 1.0f };
        var act = () => new TtsClient(opts);
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_SpeedAtMin_IsValid()
    {
        var opts = new TtsOptions { ApiKey = "key", Speed = 0.25f };
        var act = () => new TtsClient(opts);
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_SpeedAtMax_IsValid()
    {
        var opts = new TtsOptions { ApiKey = "key", Speed = 4.0f };
        var act = () => new TtsClient(opts);
        act.Should().NotThrow();
    }

    // -------------------------------------------------------------------------
    // 6. Input validation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SynthesizeAsync_NullText_ThrowsArgumentNullException()
    {
        var client = BuildClient();
        var act = async () => await client.SynthesizeAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SynthesizeAsync_EmptyOrWhitespaceText_ThrowsArgumentException(string text)
    {
        var client = BuildClient();
        var act = async () => await client.SynthesizeAsync(text);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    // -------------------------------------------------------------------------
    // 7. SaveToFileAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SaveToFileAsync_CreatesFileAtPath()
    {
        var client = BuildClient();
        var path = Path.Combine(Path.GetTempPath(), $"tts_test_{Guid.NewGuid()}.mp3");
        try
        {
            await client.SaveToFileAsync("Hello", path);
            File.Exists(path).Should().BeTrue();
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public async Task SaveToFileAsync_CreatesParentDirectoryIfNeeded()
    {
        var client = BuildClient();
        var dir = Path.Combine(Path.GetTempPath(), $"tts_subdir_{Guid.NewGuid()}");
        var path = Path.Combine(dir, "audio.mp3");
        try
        {
            await client.SaveToFileAsync("Hello", path);
            File.Exists(path).Should().BeTrue();
        }
        finally
        {
            if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task SaveToFileAsync_FileContainsByteData()
    {
        var client = BuildClient(FakeAudioBytes);
        var path = Path.Combine(Path.GetTempPath(), $"tts_data_{Guid.NewGuid()}.mp3");
        try
        {
            await client.SaveToFileAsync("Hello", path);
            var written = File.ReadAllBytes(path);
            written.Should().Equal(FakeAudioBytes);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    // -------------------------------------------------------------------------
    // 8. GetAudioStreamAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GetAudioStreamAsync_ReturnsReadableStream()
    {
        var client = BuildClient(FakeAudioBytes);
        await using var stream = await client.GetAudioStreamAsync("Hello");
        stream.Should().NotBeNull();
        stream.CanRead.Should().BeTrue();
        stream.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetAudioStreamAsync_StreamPositionedAtStart()
    {
        var client = BuildClient(FakeAudioBytes);
        await using var stream = await client.GetAudioStreamAsync("Hello");
        stream.Position.Should().Be(0);
    }

    // -------------------------------------------------------------------------
    // 9. SynthesizeWithMetadataAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SynthesizeWithMetadataAsync_PopulatesAllFields()
    {
        var client = BuildClient(FakeAudioBytes);
        var result = await client.SynthesizeWithMetadataAsync("Hello world");
        result.AudioBytes.Should().NotBeEmpty();
        result.Format.Should().Be(TtsFormat.Mp3);
        result.Voice.Should().Be(TtsVoices.Alloy);
        result.Model.Should().Be(TtsModels.Tts1);
        result.CharacterCount.Should().Be("Hello world".Length);
        result.DurationMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SynthesizeWithMetadataAsync_CharacterCountMatchesInput()
    {
        var text = "A short sentence.";
        var client = BuildClient(FakeAudioBytes);
        var result = await client.SynthesizeWithMetadataAsync(text);
        result.CharacterCount.Should().Be(text.Length);
    }

    // -------------------------------------------------------------------------
    // 10. HTTP error handling
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SynthesizeAsync_Http401_ThrowsUnauthorizedAccessException()
    {
        var client = BuildClient(code: HttpStatusCode.Unauthorized);
        var act = async () => await client.SynthesizeAsync("Test");
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task SynthesizeAsync_Http429_ThrowsRateLimitException()
    {
        var client = BuildClient(code: (HttpStatusCode)429);
        var act = async () => await client.SynthesizeAsync("Test");
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Rate limit*");
    }

    // -------------------------------------------------------------------------
    // 11. Cancellation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SynthesizeAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        var http = new HttpClient(handlerMock.Object);
        var opts = new TtsOptions { ApiKey = "test-key" };
        var client = new TtsClient(opts, http);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await client.SynthesizeAsync("Test", ct: cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task SynthesizeAsync_Timeout_ThrowsTaskCanceledException()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Timeout"));

        var http = new HttpClient(handlerMock.Object);
        var opts = new TtsOptions { ApiKey = "test-key" };
        var client = new TtsClient(opts, http);

        var act = async () => await client.SynthesizeAsync("Test");
        await act.Should().ThrowAsync<TaskCanceledException>();
    }

    // -------------------------------------------------------------------------
    // 12. Large text
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SynthesizeAsync_LargeText_HandledWithoutError()
    {
        var largeText = new string('x', 4096);
        var client = BuildClient(FakeAudioBytes);
        var bytes = await client.SynthesizeAsync(largeText);
        bytes.Should().NotBeEmpty();
    }

    // -------------------------------------------------------------------------
    // 13. FromEnvironment
    // -------------------------------------------------------------------------

    [Fact]
    public void FromEnvironment_MissingEnvVar_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("TTS_TEST_MISSING_KEY", null);
        var act = () => TtsClient.FromEnvironment("TTS_TEST_MISSING_KEY");
        act.Should().Throw<InvalidOperationException>().WithMessage("*not set*");
    }

    [Fact]
    public void FromEnvironment_SetEnvVar_CreatesClient()
    {
        const string envVar = "TTS_TEST_KEY_SET";
        Environment.SetEnvironmentVariable(envVar, "fake-api-key");
        try
        {
            var client = TtsClient.FromEnvironment(envVar);
            client.Should().NotBeNull();
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    [Fact]
    public void TtsOptions_FromEnvironment_MissingVar_Throws()
    {
        Environment.SetEnvironmentVariable("TTS_OPTS_MISSING", null);
        var act = () => TtsOptions.FromEnvironment("TTS_OPTS_MISSING");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void TtsOptions_FromEnvironment_ReadsApiKey()
    {
        const string envVar = "TTS_OPTS_KEY";
        Environment.SetEnvironmentVariable(envVar, "my-tts-key");
        try
        {
            var opts = TtsOptions.FromEnvironment(envVar);
            opts.ApiKey.Should().Be("my-tts-key");
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    // -------------------------------------------------------------------------
    // 14. DI / ServiceCollection extension
    // -------------------------------------------------------------------------

    [Fact]
    public void AddTtsClient_WithApiKey_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddTtsClient("test-key");
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<TtsClient>();
        client.Should().NotBeNull();
    }

    [Fact]
    public void AddTtsClient_WithOptions_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddTtsClient(opts =>
        {
            opts.ApiKey = "test-key";
            opts.Voice = TtsVoices.Echo;
        });
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<TtsClient>();
        client.Should().NotBeNull();
    }

    // -------------------------------------------------------------------------
    // 15. TtsOptions defaults
    // -------------------------------------------------------------------------

    [Fact]
    public void TtsOptions_HasCorrectDefaults()
    {
        var opts = new TtsOptions();
        opts.Voice.Should().Be("alloy");
        opts.Model.Should().Be("tts-1");
        opts.Format.Should().Be(TtsFormat.Mp3);
        opts.Speed.Should().BeApproximately(1.0f, 0.001f);
        opts.BaseUrl.Should().Be("https://api.aimlapi.com");
        opts.Timeout.Should().Be(TimeSpan.FromSeconds(60));
        opts.ApiKey.Should().BeEmpty();
    }

    // -------------------------------------------------------------------------
    // 16. Authorization header is sent
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SynthesizeAsync_SendsAuthorizationHeader()
    {
        HttpRequestMessage? captured = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                captured = req;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(FakeAudioBytes) };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new TtsOptions { ApiKey = "secret-key-123" };
        var client = new TtsClient(opts, http);

        await client.SynthesizeAsync("Test");

        captured!.Headers.Authorization.Should().NotBeNull();
        captured.Headers.Authorization!.Scheme.Should().Be("Bearer");
        captured.Headers.Authorization.Parameter.Should().Be("secret-key-123");
    }

    // -------------------------------------------------------------------------
    // 17. Endpoint path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SynthesizeAsync_PostsToCorrectEndpoint()
    {
        Uri? capturedUri = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedUri = req.RequestUri;
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(FakeAudioBytes) };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new TtsOptions { ApiKey = "test-key" };
        var client = new TtsClient(opts, http);

        await client.SynthesizeAsync("Test");
        capturedUri!.AbsolutePath.Should().Contain("audio/speech");
    }

    // -------------------------------------------------------------------------
    // 18. Dispose
    // -------------------------------------------------------------------------

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var client = new TtsClient("test-key");
        client.Dispose();
        var act = () => client.Dispose();
        act.Should().NotThrow();
    }

    // -------------------------------------------------------------------------
    // 19. Timeout option respected
    // -------------------------------------------------------------------------

    [Fact]
    public void TtsOptions_CustomTimeout_IsRespected()
    {
        var opts = new TtsOptions { ApiKey = "key", Timeout = TimeSpan.FromSeconds(120) };
        opts.Timeout.TotalSeconds.Should().Be(120);
    }
}
