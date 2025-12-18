using System.ClientModel;
using System.Text;
using ForeverTools.STT.Models;
using OpenAI;
using OpenAI.Audio;

namespace ForeverTools.STT;

/// <summary>
/// Speech-to-Text client using Whisper models via AI/ML API.
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public class SpeechToTextClient
{
    private readonly OpenAIClient _client;
    private readonly SpeechToTextOptions _options;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Creates a new SpeechToTextClient with the specified API key.
    /// Get your API key at: https://aimlapi.com?via=forevertools
    /// </summary>
    public SpeechToTextClient(string apiKey) : this(new SpeechToTextOptions { ApiKey = apiKey })
    {
    }

    /// <summary>
    /// Creates a new SpeechToTextClient with full options.
    /// </summary>
    public SpeechToTextClient(SpeechToTextOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new ArgumentException(
                "API key is required. Get your key at https://aimlapi.com?via=forevertools",
                nameof(options));
        }

        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(options.BaseUrl)
        };

        _client = new OpenAIClient(new ApiKeyCredential(options.ApiKey), clientOptions);
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Creates a client from environment variable.
    /// Uses AIML_API_KEY or STT_API_KEY.
    /// </summary>
    public static SpeechToTextClient FromEnvironment()
    {
        var apiKey = Environment.GetEnvironmentVariable("AIML_API_KEY")
            ?? Environment.GetEnvironmentVariable("STT_API_KEY")
            ?? throw new InvalidOperationException(
                "Environment variable 'AIML_API_KEY' or 'STT_API_KEY' not set. " +
                "Get your API key at https://aimlapi.com?via=forevertools");

        return new SpeechToTextClient(apiKey);
    }

    /// <summary>
    /// Gets the configured options.
    /// </summary>
    public SpeechToTextOptions Options => _options;

    #region Simple Transcription

    /// <summary>
    /// Transcribes an audio file to text.
    /// </summary>
    public async Task<string> TranscribeAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var result = await TranscribeWithDetailsAsync(filePath, cancellationToken);
        return result.Text;
    }

    /// <summary>
    /// Transcribes audio bytes to text.
    /// </summary>
    public async Task<string> TranscribeAsync(
        byte[] audioData,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var result = await TranscribeWithDetailsAsync(audioData, fileName, cancellationToken);
        return result.Text;
    }

    /// <summary>
    /// Transcribes an audio stream to text.
    /// </summary>
    public async Task<string> TranscribeAsync(
        Stream audioStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var result = await TranscribeWithDetailsAsync(audioStream, fileName, cancellationToken);
        return result.Text;
    }

    /// <summary>
    /// Transcribes audio from a URL to text.
    /// </summary>
    public async Task<string> TranscribeFromUrlAsync(
        string audioUrl,
        CancellationToken cancellationToken = default)
    {
        var result = await TranscribeFromUrlWithDetailsAsync(audioUrl, cancellationToken);
        return result.Text;
    }

    #endregion

    #region Detailed Transcription

    /// <summary>
    /// Transcribes an audio file with full details.
    /// </summary>
    public async Task<TranscriptionResult> TranscribeWithDetailsAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Audio file not found.", filePath);

        return await TranscribeWithDetailsAsync(new TranscriptionRequest
        {
            FilePath = filePath
        }, cancellationToken);
    }

    /// <summary>
    /// Transcribes audio bytes with full details.
    /// </summary>
    public async Task<TranscriptionResult> TranscribeWithDetailsAsync(
        byte[] audioData,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        return await TranscribeWithDetailsAsync(new TranscriptionRequest
        {
            AudioData = audioData,
            FileName = fileName
        }, cancellationToken);
    }

    /// <summary>
    /// Transcribes an audio stream with full details.
    /// </summary>
    public async Task<TranscriptionResult> TranscribeWithDetailsAsync(
        Stream audioStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        return await TranscribeWithDetailsAsync(new TranscriptionRequest
        {
            AudioStream = audioStream,
            FileName = fileName
        }, cancellationToken);
    }

    /// <summary>
    /// Transcribes audio from a URL with full details.
    /// </summary>
    public async Task<TranscriptionResult> TranscribeFromUrlWithDetailsAsync(
        string audioUrl,
        CancellationToken cancellationToken = default)
    {
        return await TranscribeWithDetailsAsync(new TranscriptionRequest
        {
            AudioUrl = audioUrl
        }, cancellationToken);
    }

    /// <summary>
    /// Transcribes audio using a full request object.
    /// </summary>
    public async Task<TranscriptionResult> TranscribeWithDetailsAsync(
        TranscriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var model = request.Model ?? _options.DefaultModel;
        var language = request.Language ?? _options.DefaultLanguage;
        var temperature = request.Temperature ?? _options.Temperature;
        var prompt = request.Prompt ?? _options.DefaultPrompt;
        var responseFormat = request.ResponseFormat ?? _options.DefaultResponseFormat;

        var audioClient = _client.GetAudioClient(model);

        // Build options
        var options = new AudioTranscriptionOptions
        {
            ResponseFormat = GetResponseFormat(responseFormat),
            Temperature = temperature
        };

        if (!string.IsNullOrEmpty(language))
        {
            options.Language = language;
        }

        if (!string.IsNullOrEmpty(prompt))
        {
            options.Prompt = prompt;
        }

        // Get audio data
        Stream audioStream;
        string fileName;
        bool disposeStream = false;

        if (!string.IsNullOrEmpty(request.FilePath))
        {
            audioStream = File.OpenRead(request.FilePath);
            fileName = Path.GetFileName(request.FilePath);
            disposeStream = true;
        }
        else if (request.AudioData != null)
        {
            audioStream = new MemoryStream(request.AudioData);
            fileName = request.FileName ?? "audio.mp3";
            disposeStream = true;
        }
        else if (request.AudioStream != null)
        {
            audioStream = request.AudioStream;
            fileName = request.FileName ?? "audio.mp3";
        }
        else if (!string.IsNullOrEmpty(request.AudioUrl))
        {
            var response = await _httpClient.GetAsync(request.AudioUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            audioStream = new MemoryStream(data);
            fileName = GetFileNameFromUrl(request.AudioUrl!);
            disposeStream = true;
        }
        else
        {
            throw new ArgumentException("No audio source provided.");
        }

        try
        {
            var transcription = await audioClient.TranscribeAudioAsync(
                audioStream,
                fileName,
                options,
                cancellationToken);

            return MapToResult(transcription.Value, model, language == null);
        }
        finally
        {
            if (disposeStream)
            {
                audioStream.Dispose();
            }
        }
    }

    #endregion

    #region Subtitle Generation

    /// <summary>
    /// Transcribes audio and returns SRT subtitle format.
    /// </summary>
    public async Task<string> TranscribeToSrtAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var result = await TranscribeWithDetailsAsync(filePath, cancellationToken);
        return GenerateSrt(result.Segments);
    }

    /// <summary>
    /// Transcribes audio and returns VTT subtitle format.
    /// </summary>
    public async Task<string> TranscribeToVttAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var result = await TranscribeWithDetailsAsync(filePath, cancellationToken);
        return GenerateVtt(result.Segments);
    }

    /// <summary>
    /// Generates SRT subtitles from transcription segments.
    /// </summary>
    public static string GenerateSrt(IReadOnlyList<TranscriptionSegment> segments)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < segments.Count; i++)
        {
            var seg = segments[i];
            var entry = new SubtitleEntry
            {
                Index = i + 1,
                Start = seg.Start,
                End = seg.End,
                Text = seg.Text.Trim()
            };
            sb.AppendLine(entry.ToSrt());
        }
        return sb.ToString();
    }

    /// <summary>
    /// Generates VTT subtitles from transcription segments.
    /// </summary>
    public static string GenerateVtt(IReadOnlyList<TranscriptionSegment> segments)
    {
        var sb = new StringBuilder();
        sb.AppendLine("WEBVTT");
        sb.AppendLine();

        for (int i = 0; i < segments.Count; i++)
        {
            var seg = segments[i];
            var entry = new SubtitleEntry
            {
                Index = i + 1,
                Start = seg.Start,
                End = seg.End,
                Text = seg.Text.Trim()
            };
            sb.AppendLine(entry.ToVtt());
        }
        return sb.ToString();
    }

    #endregion

    #region Language Detection

    /// <summary>
    /// Detects the language of an audio file.
    /// </summary>
    public async Task<LanguageDetectionResult> DetectLanguageAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var result = await TranscribeWithDetailsAsync(new TranscriptionRequest
        {
            FilePath = filePath,
            ResponseFormat = ResponseFormats.VerboseJson
        }, cancellationToken);

        return new LanguageDetectionResult
        {
            LanguageCode = result.Language ?? "unknown",
            LanguageName = GetLanguageName(result.Language),
            Confidence = 0.9 // Whisper doesn't provide confidence, assume high
        };
    }

    #endregion

    #region Batch Transcription

    /// <summary>
    /// Transcribes multiple audio files.
    /// </summary>
    public async Task<IReadOnlyList<TranscriptionResult>> TranscribeBatchAsync(
        IEnumerable<string> filePaths,
        CancellationToken cancellationToken = default)
    {
        var tasks = filePaths.Select(fp => TranscribeWithDetailsAsync(fp, cancellationToken));
        var results = await Task.WhenAll(tasks);
        return results;
    }

    #endregion

    #region Private Helpers

    private void ValidateRequest(TranscriptionRequest request)
    {
        var hasSource = !string.IsNullOrEmpty(request.FilePath)
            || request.AudioData != null
            || request.AudioStream != null
            || !string.IsNullOrEmpty(request.AudioUrl);

        if (!hasSource)
        {
            throw new ArgumentException(
                "Audio source is required. Provide FilePath, AudioData, AudioStream, or AudioUrl.");
        }

        if (request.AudioData != null && string.IsNullOrEmpty(request.FileName))
        {
            throw new ArgumentException("FileName is required when using AudioData.");
        }

        if (request.AudioStream != null && string.IsNullOrEmpty(request.FileName))
        {
            throw new ArgumentException("FileName is required when using AudioStream.");
        }
    }

    private static AudioTranscriptionFormat GetResponseFormat(string format)
    {
        return format switch
        {
            ResponseFormats.Text => AudioTranscriptionFormat.Text,
            ResponseFormats.Json => AudioTranscriptionFormat.Simple,
            ResponseFormats.VerboseJson => AudioTranscriptionFormat.Verbose,
            ResponseFormats.Srt => AudioTranscriptionFormat.Srt,
            ResponseFormats.Vtt => AudioTranscriptionFormat.Vtt,
            _ => AudioTranscriptionFormat.Verbose
        };
    }

    private static TranscriptionResult MapToResult(
        AudioTranscription transcription,
        string model,
        bool wasLanguageDetected)
    {
        var segments = new List<TranscriptionSegment>();

        if (transcription.Segments != null)
        {
            int id = 0;
            foreach (var seg in transcription.Segments)
            {
                segments.Add(new TranscriptionSegment
                {
                    Id = id++,
                    StartSeconds = seg.StartTime.TotalSeconds,
                    EndSeconds = seg.EndTime.TotalSeconds,
                    Text = seg.Text,
                    AvgLogProb = seg.AverageLogProbability,
                    CompressionRatio = seg.CompressionRatio,
                    NoSpeechProb = seg.NoSpeechProbability,
                    Temperature = seg.Temperature
                });
            }
        }

        return new TranscriptionResult
        {
            Text = transcription.Text,
            Language = transcription.Language,
            DurationSeconds = transcription.Duration?.TotalSeconds,
            Segments = segments,
            Model = model,
            WasLanguageDetected = wasLanguageDetected
        };
    }

    private static string GetFileNameFromUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            var fileName = Path.GetFileName(uri.LocalPath);
            if (!string.IsNullOrEmpty(fileName) && fileName.Contains('.'))
            {
                return fileName;
            }
        }
        catch
        {
            // Ignore URL parsing errors
        }
        return "audio.mp3";
    }

    private static string GetLanguageName(string? code)
    {
        if (string.IsNullOrEmpty(code)) return "Unknown";

        return code!.ToLowerInvariant() switch
        {
            "en" => "English",
            "es" => "Spanish",
            "fr" => "French",
            "de" => "German",
            "it" => "Italian",
            "pt" => "Portuguese",
            "ru" => "Russian",
            "ja" => "Japanese",
            "ko" => "Korean",
            "zh" => "Chinese",
            "ar" => "Arabic",
            "hi" => "Hindi",
            "nl" => "Dutch",
            "pl" => "Polish",
            "tr" => "Turkish",
            "vi" => "Vietnamese",
            "th" => "Thai",
            "id" => "Indonesian",
            "sv" => "Swedish",
            "da" => "Danish",
            "no" => "Norwegian",
            "fi" => "Finnish",
            "el" => "Greek",
            "cs" => "Czech",
            "ro" => "Romanian",
            "hu" => "Hungarian",
            "uk" => "Ukrainian",
            "he" => "Hebrew",
            _ => code.ToUpperInvariant()
        };
    }

    #endregion
}
