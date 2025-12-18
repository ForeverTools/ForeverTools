namespace ForeverTools.STT;

/// <summary>
/// Configuration options for the Speech-to-Text client.
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public class SpeechToTextOptions
{
    /// <summary>
    /// AI/ML API key. Get yours at: https://aimlapi.com?via=forevertools
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Base URL for the API. Default is AI/ML API endpoint.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.aimlapi.com/v1";

    /// <summary>
    /// Default model to use for transcription.
    /// </summary>
    public string DefaultModel { get; set; } = SttModels.Whisper1;

    /// <summary>
    /// Default language for transcription. Null means auto-detect.
    /// Use ISO 639-1 codes (e.g., "en", "es", "fr").
    /// </summary>
    public string? DefaultLanguage { get; set; }

    /// <summary>
    /// Temperature for transcription (0-1). Lower is more deterministic.
    /// </summary>
    public float Temperature { get; set; } = 0f;

    /// <summary>
    /// Optional prompt to guide transcription style or vocabulary.
    /// </summary>
    public string? DefaultPrompt { get; set; }

    /// <summary>
    /// Default response format for transcriptions.
    /// </summary>
    public string DefaultResponseFormat { get; set; } = ResponseFormats.VerboseJson;
}

/// <summary>
/// Available speech-to-text models.
/// </summary>
public static class SttModels
{
    /// <summary>
    /// OpenAI Whisper model - fast and accurate.
    /// </summary>
    public const string Whisper1 = "whisper-1";

    /// <summary>
    /// Whisper Large V3 - highest accuracy, slower.
    /// </summary>
    public const string WhisperLargeV3 = "whisper-large-v3";

    /// <summary>
    /// Whisper Large V3 Turbo - balanced speed and accuracy.
    /// </summary>
    public const string WhisperLargeV3Turbo = "whisper-large-v3-turbo";
}

/// <summary>
/// Supported audio file formats.
/// </summary>
public static class AudioFormats
{
    public const string Mp3 = "mp3";
    public const string Mp4 = "mp4";
    public const string Mpeg = "mpeg";
    public const string Mpga = "mpga";
    public const string M4a = "m4a";
    public const string Wav = "wav";
    public const string Webm = "webm";
    public const string Flac = "flac";
    public const string Ogg = "ogg";

    /// <summary>
    /// All supported audio formats.
    /// </summary>
    public static readonly string[] Supported = { Mp3, Mp4, Mpeg, Mpga, M4a, Wav, Webm, Flac, Ogg };

    /// <summary>
    /// Gets the MIME type for an audio format.
    /// </summary>
    public static string GetMimeType(string format)
    {
        return format.ToLowerInvariant() switch
        {
            Mp3 or Mpga => "audio/mpeg",
            Mp4 or M4a => "audio/mp4",
            Wav => "audio/wav",
            Webm => "audio/webm",
            Flac => "audio/flac",
            Ogg => "audio/ogg",
            Mpeg => "audio/mpeg",
            _ => "application/octet-stream"
        };
    }

    /// <summary>
    /// Gets the format from a file extension.
    /// </summary>
    public static string? FromExtension(string extension)
    {
        var ext = extension.TrimStart('.').ToLowerInvariant();
        return Supported.FirstOrDefault(f => f == ext);
    }
}

/// <summary>
/// Available response formats for transcription.
/// </summary>
public static class ResponseFormats
{
    /// <summary>
    /// Plain text output.
    /// </summary>
    public const string Text = "text";

    /// <summary>
    /// JSON with text field only.
    /// </summary>
    public const string Json = "json";

    /// <summary>
    /// JSON with timestamps and segments.
    /// </summary>
    public const string VerboseJson = "verbose_json";

    /// <summary>
    /// SubRip subtitle format.
    /// </summary>
    public const string Srt = "srt";

    /// <summary>
    /// WebVTT subtitle format.
    /// </summary>
    public const string Vtt = "vtt";
}

/// <summary>
/// Common languages for transcription (ISO 639-1 codes).
/// </summary>
public static class TranscriptionLanguages
{
    public const string English = "en";
    public const string Spanish = "es";
    public const string French = "fr";
    public const string German = "de";
    public const string Italian = "it";
    public const string Portuguese = "pt";
    public const string Russian = "ru";
    public const string Japanese = "ja";
    public const string Korean = "ko";
    public const string Chinese = "zh";
    public const string Arabic = "ar";
    public const string Hindi = "hi";
    public const string Dutch = "nl";
    public const string Polish = "pl";
    public const string Turkish = "tr";
    public const string Vietnamese = "vi";
    public const string Thai = "th";
    public const string Indonesian = "id";
    public const string Swedish = "sv";
    public const string Danish = "da";
    public const string Norwegian = "no";
    public const string Finnish = "fi";
    public const string Greek = "el";
    public const string Czech = "cs";
    public const string Romanian = "ro";
    public const string Hungarian = "hu";
    public const string Ukrainian = "uk";
    public const string Hebrew = "he";
}
