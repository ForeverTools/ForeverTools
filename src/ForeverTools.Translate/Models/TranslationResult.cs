namespace ForeverTools.Translate.Models;

/// <summary>
/// Result of a translation operation.
/// </summary>
public class TranslationResult
{
    /// <summary>
    /// The original text that was translated.
    /// </summary>
    public string OriginalText { get; set; } = string.Empty;

    /// <summary>
    /// The translated text.
    /// </summary>
    public string TranslatedText { get; set; } = string.Empty;

    /// <summary>
    /// The source language code (detected or specified).
    /// </summary>
    public string SourceLanguage { get; set; } = string.Empty;

    /// <summary>
    /// The target language code.
    /// </summary>
    public string TargetLanguage { get; set; } = string.Empty;

    /// <summary>
    /// The AI model used for the translation.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Whether the source language was auto-detected.
    /// </summary>
    public bool WasLanguageDetected { get; set; }

    /// <summary>
    /// Confidence score for language detection (0-1), if applicable.
    /// </summary>
    public double? DetectionConfidence { get; set; }

    /// <summary>
    /// Translation quality notes or warnings, if any.
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Result of a language detection operation.
/// </summary>
public class LanguageDetectionResult
{
    /// <summary>
    /// The detected language code.
    /// </summary>
    public string LanguageCode { get; set; } = string.Empty;

    /// <summary>
    /// The detected language name in English.
    /// </summary>
    public string LanguageName { get; set; } = string.Empty;

    /// <summary>
    /// Confidence score (0-1).
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// The text that was analyzed.
    /// </summary>
    public string AnalyzedText { get; set; } = string.Empty;

    /// <summary>
    /// Alternative language possibilities, if any.
    /// </summary>
    public IReadOnlyList<LanguageAlternative>? Alternatives { get; set; }
}

/// <summary>
/// An alternative language detection possibility.
/// </summary>
public class LanguageAlternative
{
    /// <summary>
    /// The language code.
    /// </summary>
    public string LanguageCode { get; set; } = string.Empty;

    /// <summary>
    /// The language name.
    /// </summary>
    public string LanguageName { get; set; } = string.Empty;

    /// <summary>
    /// Confidence score for this alternative.
    /// </summary>
    public double Confidence { get; set; }
}

/// <summary>
/// Result of a batch translation operation.
/// </summary>
public class BatchTranslationResult
{
    /// <summary>
    /// Individual translation results.
    /// </summary>
    public IReadOnlyList<TranslationResult> Results { get; set; } = Array.Empty<TranslationResult>();

    /// <summary>
    /// Number of successful translations.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed translations.
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Total texts processed.
    /// </summary>
    public int TotalCount => SuccessCount + FailureCount;

    /// <summary>
    /// Any errors that occurred during batch translation.
    /// </summary>
    public IReadOnlyList<BatchTranslationError>? Errors { get; set; }
}

/// <summary>
/// Error information for a failed translation in a batch.
/// </summary>
public class BatchTranslationError
{
    /// <summary>
    /// Index of the failed text in the original batch.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// The original text that failed to translate.
    /// </summary>
    public string OriginalText { get; set; } = string.Empty;

    /// <summary>
    /// Error message.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Request for translating text.
/// </summary>
public class TranslationRequest
{
    /// <summary>
    /// The text to translate.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Source language code (null for auto-detection).
    /// </summary>
    public string? SourceLanguage { get; set; }

    /// <summary>
    /// Target language code.
    /// </summary>
    public string TargetLanguage { get; set; } = string.Empty;

    /// <summary>
    /// Optional context to help with translation accuracy.
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    /// Translation style preference.
    /// </summary>
    public TranslationStyle? Style { get; set; }

    /// <summary>
    /// Specific AI model to use (overrides default).
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Glossary of terms that should be translated in a specific way.
    /// Key is source term, value is target translation.
    /// </summary>
    public IDictionary<string, string>? Glossary { get; set; }
}
