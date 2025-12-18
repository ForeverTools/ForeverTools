namespace ForeverTools.STT.Models;

/// <summary>
/// Result of an audio transcription.
/// </summary>
public class TranscriptionResult
{
    /// <summary>
    /// The transcribed text.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Detected or specified language (ISO 639-1 code).
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Duration of the audio in seconds.
    /// </summary>
    public double? DurationSeconds { get; set; }

    /// <summary>
    /// Duration of the audio as TimeSpan.
    /// </summary>
    public TimeSpan? Duration => DurationSeconds.HasValue
        ? TimeSpan.FromSeconds(DurationSeconds.Value)
        : null;

    /// <summary>
    /// Individual segments with timestamps.
    /// </summary>
    public IReadOnlyList<TranscriptionSegment> Segments { get; set; } = Array.Empty<TranscriptionSegment>();

    /// <summary>
    /// Word-level timestamps (if available).
    /// </summary>
    public IReadOnlyList<TranscriptionWord>? Words { get; set; }

    /// <summary>
    /// The model used for transcription.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Whether the language was auto-detected.
    /// </summary>
    public bool WasLanguageDetected { get; set; }
}

/// <summary>
/// A segment of transcription with timestamps.
/// </summary>
public class TranscriptionSegment
{
    /// <summary>
    /// Segment index.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Start time in seconds.
    /// </summary>
    public double StartSeconds { get; set; }

    /// <summary>
    /// End time in seconds.
    /// </summary>
    public double EndSeconds { get; set; }

    /// <summary>
    /// Start time as TimeSpan.
    /// </summary>
    public TimeSpan Start => TimeSpan.FromSeconds(StartSeconds);

    /// <summary>
    /// End time as TimeSpan.
    /// </summary>
    public TimeSpan End => TimeSpan.FromSeconds(EndSeconds);

    /// <summary>
    /// The transcribed text for this segment.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Average log probability (confidence indicator).
    /// </summary>
    public double? AvgLogProb { get; set; }

    /// <summary>
    /// Compression ratio.
    /// </summary>
    public double? CompressionRatio { get; set; }

    /// <summary>
    /// No speech probability.
    /// </summary>
    public double? NoSpeechProb { get; set; }

    /// <summary>
    /// Temperature used for this segment.
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// Token IDs for this segment.
    /// </summary>
    public IReadOnlyList<int>? Tokens { get; set; }
}

/// <summary>
/// Word-level timestamp information.
/// </summary>
public class TranscriptionWord
{
    /// <summary>
    /// The word.
    /// </summary>
    public string Word { get; set; } = string.Empty;

    /// <summary>
    /// Start time in seconds.
    /// </summary>
    public double StartSeconds { get; set; }

    /// <summary>
    /// End time in seconds.
    /// </summary>
    public double EndSeconds { get; set; }

    /// <summary>
    /// Start time as TimeSpan.
    /// </summary>
    public TimeSpan Start => TimeSpan.FromSeconds(StartSeconds);

    /// <summary>
    /// End time as TimeSpan.
    /// </summary>
    public TimeSpan End => TimeSpan.FromSeconds(EndSeconds);
}

/// <summary>
/// Request for transcription with full options.
/// </summary>
public class TranscriptionRequest
{
    /// <summary>
    /// Path to the audio file.
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Audio data as bytes.
    /// </summary>
    public byte[]? AudioData { get; set; }

    /// <summary>
    /// Audio stream.
    /// </summary>
    public Stream? AudioStream { get; set; }

    /// <summary>
    /// File name (required when using AudioData or AudioStream).
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// URL to audio file (will be downloaded).
    /// </summary>
    public string? AudioUrl { get; set; }

    /// <summary>
    /// Model to use. Defaults to options setting.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Language of the audio (ISO 639-1). Null for auto-detect.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Prompt to guide transcription style/vocabulary.
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// Temperature (0-1). Lower is more deterministic.
    /// </summary>
    public float? Temperature { get; set; }

    /// <summary>
    /// Response format.
    /// </summary>
    public string? ResponseFormat { get; set; }

    /// <summary>
    /// Whether to include word-level timestamps.
    /// </summary>
    public bool IncludeWordTimestamps { get; set; }
}

/// <summary>
/// Result of language detection.
/// </summary>
public class LanguageDetectionResult
{
    /// <summary>
    /// Detected language code (ISO 639-1).
    /// </summary>
    public string LanguageCode { get; set; } = string.Empty;

    /// <summary>
    /// English name of the detected language.
    /// </summary>
    public string LanguageName { get; set; } = string.Empty;

    /// <summary>
    /// Confidence score (0-1).
    /// </summary>
    public double Confidence { get; set; }
}

/// <summary>
/// Subtitle entry for SRT/VTT output.
/// </summary>
public class SubtitleEntry
{
    /// <summary>
    /// Sequence number.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Start time.
    /// </summary>
    public TimeSpan Start { get; set; }

    /// <summary>
    /// End time.
    /// </summary>
    public TimeSpan End { get; set; }

    /// <summary>
    /// Subtitle text.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Converts to SRT format line.
    /// </summary>
    public string ToSrt()
    {
        return $"{Index}\n{FormatSrtTime(Start)} --> {FormatSrtTime(End)}\n{Text}\n";
    }

    /// <summary>
    /// Converts to VTT format line.
    /// </summary>
    public string ToVtt()
    {
        return $"{FormatVttTime(Start)} --> {FormatVttTime(End)}\n{Text}\n";
    }

    private static string FormatSrtTime(TimeSpan ts)
    {
        return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00},{ts.Milliseconds:000}";
    }

    private static string FormatVttTime(TimeSpan ts)
    {
        return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
    }
}
