namespace ForeverTools.TTS.Models;

/// <summary>
/// Supported audio output formats for text-to-speech synthesis.
/// </summary>
public enum TtsFormat
{
    /// <summary>MPEG audio (MP3). Good balance of quality and file size.</summary>
    Mp3,

    /// <summary>Opus codec — low latency, good for streaming.</summary>
    Opus,

    /// <summary>Advanced Audio Coding — efficient lossy compression.</summary>
    Aac,

    /// <summary>Free Lossless Audio Codec — maximum quality, larger files.</summary>
    Flac,

    /// <summary>Waveform audio — uncompressed PCM in a RIFF container.</summary>
    Wav,

    /// <summary>Raw Pulse-Code Modulation audio data without a container.</summary>
    Pcm
}

/// <summary>
/// Extension methods for <see cref="TtsFormat"/>.
/// </summary>
public static class TtsFormatExtensions
{
    /// <summary>
    /// Returns the MIME content type for the given <see cref="TtsFormat"/>.
    /// </summary>
    /// <param name="format">The audio format.</param>
    /// <returns>The MIME type string.</returns>
    public static string ToContentType(this TtsFormat format) => format switch
    {
        TtsFormat.Mp3  => "audio/mpeg",
        TtsFormat.Opus => "audio/opus",
        TtsFormat.Aac  => "audio/aac",
        TtsFormat.Flac => "audio/flac",
        TtsFormat.Wav  => "audio/wav",
        TtsFormat.Pcm  => "audio/pcm",
        _              => "audio/mpeg"
    };

    /// <summary>
    /// Returns the file extension (without leading dot) for the format.
    /// </summary>
    public static string ToFileExtension(this TtsFormat format) => format switch
    {
        TtsFormat.Mp3  => "mp3",
        TtsFormat.Opus => "opus",
        TtsFormat.Aac  => "aac",
        TtsFormat.Flac => "flac",
        TtsFormat.Wav  => "wav",
        TtsFormat.Pcm  => "pcm",
        _              => "mp3"
    };

    /// <summary>
    /// Returns the format identifier used by the AI/ML API.
    /// </summary>
    internal static string ToApiString(this TtsFormat format) => format.ToString().ToLowerInvariant();
}
