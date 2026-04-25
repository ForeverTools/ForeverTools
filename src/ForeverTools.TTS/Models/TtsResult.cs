namespace ForeverTools.TTS.Models;

/// <summary>
/// The result of a text-to-speech synthesis operation, including the audio bytes and metadata.
/// </summary>
public class TtsResult
{
    /// <summary>The synthesised audio data.</summary>
    public byte[] AudioBytes { get; set; } = Array.Empty<byte>();

    /// <summary>The audio format of the returned data.</summary>
    public TtsFormat Format { get; set; }

    /// <summary>How long the API round-trip took in milliseconds.</summary>
    public long DurationMs { get; set; }

    /// <summary>The voice used for synthesis.</summary>
    public string Voice { get; set; } = string.Empty;

    /// <summary>The model used for synthesis.</summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>The number of characters in the input text.</summary>
    public int CharacterCount { get; set; }
}
