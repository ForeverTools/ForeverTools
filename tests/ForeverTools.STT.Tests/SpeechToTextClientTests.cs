using ForeverTools.STT;
using ForeverTools.STT.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ForeverTools.STT.Tests;

public class SpeechToTextClientTests
{
    private const string TestApiKey = "test-api-key";

    #region Constructor Tests

    [Fact]
    public void Constructor_WithApiKey_CreatesClient()
    {
        var client = new SpeechToTextClient(TestApiKey);
        Assert.NotNull(client);
        Assert.Equal(TestApiKey, client.Options.ApiKey);
    }

    [Fact]
    public void Constructor_WithOptions_CreatesClient()
    {
        var options = new SpeechToTextOptions
        {
            ApiKey = TestApiKey,
            DefaultModel = SttModels.WhisperLargeV3
        };

        var client = new SpeechToTextClient(options);
        Assert.NotNull(client);
        Assert.Equal(SttModels.WhisperLargeV3, client.Options.DefaultModel);
    }

    [Fact]
    public void Constructor_WithNullApiKey_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new SpeechToTextClient(""));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new SpeechToTextClient((SpeechToTextOptions)null!));
    }

    [Fact]
    public void Constructor_WithEmptyApiKeyInOptions_ThrowsException()
    {
        var options = new SpeechToTextOptions { ApiKey = "" };
        Assert.Throws<ArgumentException>(() => new SpeechToTextClient(options));
    }

    #endregion

    #region Options Tests

    [Fact]
    public void Options_DefaultValues_AreCorrect()
    {
        var options = new SpeechToTextOptions();

        Assert.Equal("https://api.aimlapi.com/v1", options.BaseUrl);
        Assert.Equal(SttModels.Whisper1, options.DefaultModel);
        Assert.Null(options.DefaultLanguage);
        Assert.Equal(0f, options.Temperature);
        Assert.Null(options.DefaultPrompt);
        Assert.Equal(ResponseFormats.VerboseJson, options.DefaultResponseFormat);
    }

    [Fact]
    public void Options_CanBeCustomized()
    {
        var options = new SpeechToTextOptions
        {
            ApiKey = TestApiKey,
            BaseUrl = "https://custom.api.com/v1",
            DefaultModel = SttModels.WhisperLargeV3,
            DefaultLanguage = "es",
            Temperature = 0.5f,
            DefaultPrompt = "Meeting about sales",
            DefaultResponseFormat = ResponseFormats.Srt
        };

        Assert.Equal("https://custom.api.com/v1", options.BaseUrl);
        Assert.Equal(SttModels.WhisperLargeV3, options.DefaultModel);
        Assert.Equal("es", options.DefaultLanguage);
        Assert.Equal(0.5f, options.Temperature);
        Assert.Equal("Meeting about sales", options.DefaultPrompt);
        Assert.Equal(ResponseFormats.Srt, options.DefaultResponseFormat);
    }

    #endregion

    #region Model Constants Tests

    [Fact]
    public void SttModels_HasExpectedValues()
    {
        Assert.Equal("whisper-1", SttModels.Whisper1);
        Assert.Equal("whisper-large-v3", SttModels.WhisperLargeV3);
        Assert.Equal("whisper-large-v3-turbo", SttModels.WhisperLargeV3Turbo);
    }

    [Fact]
    public void ResponseFormats_HasExpectedValues()
    {
        Assert.Equal("text", ResponseFormats.Text);
        Assert.Equal("json", ResponseFormats.Json);
        Assert.Equal("verbose_json", ResponseFormats.VerboseJson);
        Assert.Equal("srt", ResponseFormats.Srt);
        Assert.Equal("vtt", ResponseFormats.Vtt);
    }

    [Fact]
    public void TranscriptionLanguages_HasExpectedValues()
    {
        Assert.Equal("en", TranscriptionLanguages.English);
        Assert.Equal("es", TranscriptionLanguages.Spanish);
        Assert.Equal("fr", TranscriptionLanguages.French);
        Assert.Equal("de", TranscriptionLanguages.German);
        Assert.Equal("ja", TranscriptionLanguages.Japanese);
        Assert.Equal("zh", TranscriptionLanguages.Chinese);
    }

    #endregion

    #region AudioFormats Tests

    [Fact]
    public void AudioFormats_HasExpectedValues()
    {
        Assert.Equal("mp3", AudioFormats.Mp3);
        Assert.Equal("wav", AudioFormats.Wav);
        Assert.Equal("m4a", AudioFormats.M4a);
        Assert.Equal("webm", AudioFormats.Webm);
        Assert.Equal("flac", AudioFormats.Flac);
        Assert.Equal("ogg", AudioFormats.Ogg);
    }

    [Fact]
    public void AudioFormats_Supported_ContainsAllFormats()
    {
        Assert.Contains("mp3", AudioFormats.Supported);
        Assert.Contains("wav", AudioFormats.Supported);
        Assert.Contains("m4a", AudioFormats.Supported);
        Assert.Contains("webm", AudioFormats.Supported);
        Assert.Contains("flac", AudioFormats.Supported);
        Assert.Contains("ogg", AudioFormats.Supported);
    }

    [Theory]
    [InlineData("mp3", "audio/mpeg")]
    [InlineData("wav", "audio/wav")]
    [InlineData("m4a", "audio/mp4")]
    [InlineData("webm", "audio/webm")]
    [InlineData("flac", "audio/flac")]
    [InlineData("ogg", "audio/ogg")]
    public void AudioFormats_GetMimeType_ReturnsCorrectType(string format, string expectedMime)
    {
        Assert.Equal(expectedMime, AudioFormats.GetMimeType(format));
    }

    [Fact]
    public void AudioFormats_GetMimeType_UnknownFormat_ReturnsOctetStream()
    {
        Assert.Equal("application/octet-stream", AudioFormats.GetMimeType("unknown"));
    }

    [Theory]
    [InlineData(".mp3", "mp3")]
    [InlineData("mp3", "mp3")]
    [InlineData(".wav", "wav")]
    [InlineData(".m4a", "m4a")]
    public void AudioFormats_FromExtension_ReturnsFormat(string extension, string expected)
    {
        Assert.Equal(expected, AudioFormats.FromExtension(extension));
    }

    [Fact]
    public void AudioFormats_FromExtension_UnknownExtension_ReturnsNull()
    {
        Assert.Null(AudioFormats.FromExtension(".xyz"));
    }

    #endregion

    #region TranscriptionResult Model Tests

    [Fact]
    public void TranscriptionResult_Duration_ConvertsFromSeconds()
    {
        var result = new TranscriptionResult
        {
            DurationSeconds = 65.5
        };

        Assert.NotNull(result.Duration);
        Assert.Equal(TimeSpan.FromSeconds(65.5), result.Duration);
    }

    [Fact]
    public void TranscriptionResult_Duration_NullWhenNoSeconds()
    {
        var result = new TranscriptionResult();
        Assert.Null(result.Duration);
    }

    [Fact]
    public void TranscriptionSegment_TimeSpan_ConvertsCorrectly()
    {
        var segment = new TranscriptionSegment
        {
            StartSeconds = 10.5,
            EndSeconds = 15.75
        };

        Assert.Equal(TimeSpan.FromSeconds(10.5), segment.Start);
        Assert.Equal(TimeSpan.FromSeconds(15.75), segment.End);
    }

    [Fact]
    public void TranscriptionWord_TimeSpan_ConvertsCorrectly()
    {
        var word = new TranscriptionWord
        {
            Word = "hello",
            StartSeconds = 1.2,
            EndSeconds = 1.5
        };

        Assert.Equal(TimeSpan.FromSeconds(1.2), word.Start);
        Assert.Equal(TimeSpan.FromSeconds(1.5), word.End);
    }

    #endregion

    #region SubtitleEntry Tests

    [Fact]
    public void SubtitleEntry_ToSrt_FormatsCorrectly()
    {
        var entry = new SubtitleEntry
        {
            Index = 1,
            Start = TimeSpan.FromSeconds(10.5),
            End = TimeSpan.FromSeconds(15.75),
            Text = "Hello world"
        };

        var srt = entry.ToSrt();
        Assert.Contains("1", srt);
        Assert.Contains("00:00:10,500 --> 00:00:15,750", srt);
        Assert.Contains("Hello world", srt);
    }

    [Fact]
    public void SubtitleEntry_ToVtt_FormatsCorrectly()
    {
        var entry = new SubtitleEntry
        {
            Index = 1,
            Start = TimeSpan.FromSeconds(10.5),
            End = TimeSpan.FromSeconds(15.75),
            Text = "Hello world"
        };

        var vtt = entry.ToVtt();
        Assert.Contains("00:00:10.500 --> 00:00:15.750", vtt);
        Assert.Contains("Hello world", vtt);
    }

    [Fact]
    public void SubtitleEntry_ToSrt_HandlesLongDurations()
    {
        var entry = new SubtitleEntry
        {
            Index = 1,
            Start = TimeSpan.FromHours(1) + TimeSpan.FromMinutes(30) + TimeSpan.FromSeconds(45),
            End = TimeSpan.FromHours(1) + TimeSpan.FromMinutes(30) + TimeSpan.FromSeconds(50),
            Text = "Test"
        };

        var srt = entry.ToSrt();
        Assert.Contains("01:30:45,000 --> 01:30:50,000", srt);
    }

    #endregion

    #region GenerateSrt/GenerateVtt Tests

    [Fact]
    public void GenerateSrt_WithSegments_FormatsCorrectly()
    {
        var segments = new List<TranscriptionSegment>
        {
            new TranscriptionSegment { Id = 0, StartSeconds = 0, EndSeconds = 5, Text = "First line" },
            new TranscriptionSegment { Id = 1, StartSeconds = 5, EndSeconds = 10, Text = "Second line" }
        };

        var srt = SpeechToTextClient.GenerateSrt(segments);

        Assert.Contains("1\n", srt);
        Assert.Contains("2\n", srt);
        Assert.Contains("First line", srt);
        Assert.Contains("Second line", srt);
    }

    [Fact]
    public void GenerateVtt_WithSegments_StartsWithHeader()
    {
        var segments = new List<TranscriptionSegment>
        {
            new TranscriptionSegment { Id = 0, StartSeconds = 0, EndSeconds = 5, Text = "Hello" }
        };

        var vtt = SpeechToTextClient.GenerateVtt(segments);

        Assert.StartsWith("WEBVTT", vtt);
        Assert.Contains("Hello", vtt);
    }

    [Fact]
    public void GenerateSrt_EmptySegments_ReturnsEmptyString()
    {
        var srt = SpeechToTextClient.GenerateSrt(new List<TranscriptionSegment>());
        Assert.Empty(srt.Trim());
    }

    [Fact]
    public void GenerateVtt_EmptySegments_ReturnsHeaderOnly()
    {
        var vtt = SpeechToTextClient.GenerateVtt(new List<TranscriptionSegment>());
        Assert.StartsWith("WEBVTT", vtt.Trim());
    }

    #endregion

    #region TranscriptionRequest Validation Tests

    [Fact]
    public void TranscriptionRequest_CanSetFilePath()
    {
        var request = new TranscriptionRequest
        {
            FilePath = "test.mp3"
        };
        Assert.Equal("test.mp3", request.FilePath);
    }

    [Fact]
    public void TranscriptionRequest_CanSetAudioData()
    {
        var data = new byte[] { 1, 2, 3 };
        var request = new TranscriptionRequest
        {
            AudioData = data,
            FileName = "test.mp3"
        };
        Assert.Equal(data, request.AudioData);
        Assert.Equal("test.mp3", request.FileName);
    }

    [Fact]
    public void TranscriptionRequest_CanSetAllOptions()
    {
        var request = new TranscriptionRequest
        {
            FilePath = "test.mp3",
            Model = SttModels.WhisperLargeV3,
            Language = "en",
            Prompt = "Tech meeting",
            Temperature = 0.3f,
            ResponseFormat = ResponseFormats.Srt,
            IncludeWordTimestamps = true
        };

        Assert.Equal(SttModels.WhisperLargeV3, request.Model);
        Assert.Equal("en", request.Language);
        Assert.Equal("Tech meeting", request.Prompt);
        Assert.Equal(0.3f, request.Temperature);
        Assert.Equal(ResponseFormats.Srt, request.ResponseFormat);
        Assert.True(request.IncludeWordTimestamps);
    }

    #endregion

    #region LanguageDetectionResult Tests

    [Fact]
    public void LanguageDetectionResult_CanSetProperties()
    {
        var result = new LanguageDetectionResult
        {
            LanguageCode = "en",
            LanguageName = "English",
            Confidence = 0.95
        };

        Assert.Equal("en", result.LanguageCode);
        Assert.Equal("English", result.LanguageName);
        Assert.Equal(0.95, result.Confidence);
    }

    #endregion

    #region Dependency Injection Tests

    [Fact]
    public void AddForeverToolsSTT_WithApiKey_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsSTT(TestApiKey);

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SpeechToTextClient>();

        Assert.NotNull(client);
        Assert.Equal(TestApiKey, client.Options.ApiKey);
    }

    [Fact]
    public void AddForeverToolsSTT_WithAction_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsSTT(options =>
        {
            options.ApiKey = TestApiKey;
            options.DefaultModel = SttModels.WhisperLargeV3;
        });

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SpeechToTextClient>();

        Assert.NotNull(client);
        Assert.Equal(SttModels.WhisperLargeV3, client.Options.DefaultModel);
    }

    [Fact]
    public void AddForeverToolsSTT_WithOptions_RegistersClient()
    {
        var services = new ServiceCollection();
        var options = new SpeechToTextOptions
        {
            ApiKey = TestApiKey,
            DefaultLanguage = "es"
        };
        services.AddForeverToolsSTT(options);

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SpeechToTextClient>();

        Assert.NotNull(client);
        Assert.Equal("es", client.Options.DefaultLanguage);
    }

    [Fact]
    public void AddForeverToolsSTT_RegistersAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsSTT(TestApiKey);

        var provider = services.BuildServiceProvider();
        var client1 = provider.GetRequiredService<SpeechToTextClient>();
        var client2 = provider.GetRequiredService<SpeechToTextClient>();

        Assert.Same(client1, client2);
    }

    #endregion

    #region File Not Found Tests

    [Fact]
    public async Task TranscribeAsync_FileNotFound_ThrowsException()
    {
        var client = new SpeechToTextClient(TestApiKey);

        await Assert.ThrowsAsync<FileNotFoundException>(
            () => client.TranscribeAsync("nonexistent.mp3"));
    }

    [Fact]
    public async Task TranscribeWithDetailsAsync_FileNotFound_ThrowsException()
    {
        var client = new SpeechToTextClient(TestApiKey);

        await Assert.ThrowsAsync<FileNotFoundException>(
            () => client.TranscribeWithDetailsAsync("nonexistent.mp3"));
    }

    #endregion

    #region Request Validation Tests

    [Fact]
    public async Task TranscribeWithDetailsAsync_NoSource_ThrowsException()
    {
        var client = new SpeechToTextClient(TestApiKey);
        var request = new TranscriptionRequest();

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => client.TranscribeWithDetailsAsync(request));

        Assert.Contains("source", ex.Message.ToLower());
    }

    [Fact]
    public async Task TranscribeWithDetailsAsync_AudioDataWithoutFileName_ThrowsException()
    {
        var client = new SpeechToTextClient(TestApiKey);
        var request = new TranscriptionRequest
        {
            AudioData = new byte[] { 1, 2, 3 }
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => client.TranscribeWithDetailsAsync(request));

        Assert.Contains("FileName", ex.Message);
    }

    [Fact]
    public async Task TranscribeWithDetailsAsync_AudioStreamWithoutFileName_ThrowsException()
    {
        var client = new SpeechToTextClient(TestApiKey);
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var request = new TranscriptionRequest
        {
            AudioStream = stream
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => client.TranscribeWithDetailsAsync(request));

        Assert.Contains("FileName", ex.Message);
    }

    #endregion
}
