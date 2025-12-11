namespace ForeverTools.OCR;

/// <summary>
/// Available vision models for OCR text extraction.
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public static class OcrModels
{
    /// <summary>
    /// OpenAI GPT-4o - Best balance of speed and accuracy.
    /// Excellent for general OCR, documents, screenshots, receipts.
    /// </summary>
    public const string Gpt4o = "gpt-4o";

    /// <summary>
    /// OpenAI GPT-4o Mini - Faster and cheaper, good for simple text.
    /// Best for clear printed text, screenshots, simple documents.
    /// </summary>
    public const string Gpt4oMini = "gpt-4o-mini";

    /// <summary>
    /// OpenAI GPT-4 Turbo - High accuracy vision model.
    /// Excellent for complex documents, handwriting, degraded images.
    /// </summary>
    public const string Gpt4Turbo = "gpt-4-turbo";

    /// <summary>
    /// Anthropic Claude 3.5 Sonnet - Excellent reasoning and accuracy.
    /// Best for complex layouts, forms, structured documents.
    /// </summary>
    public const string Claude35Sonnet = "claude-3-5-sonnet-20241022";

    /// <summary>
    /// Anthropic Claude 3 Opus - Highest accuracy Claude model.
    /// Best for critical documents, legal text, handwriting.
    /// </summary>
    public const string Claude3Opus = "claude-3-opus-20240229";

    /// <summary>
    /// Anthropic Claude 3 Sonnet - Good balance for Claude.
    /// Good for general documents and forms.
    /// </summary>
    public const string Claude3Sonnet = "claude-3-sonnet-20240229";

    /// <summary>
    /// Anthropic Claude 3 Haiku - Fast Claude model.
    /// Best for simple, clear text extraction.
    /// </summary>
    public const string Claude3Haiku = "claude-3-haiku-20240307";

    /// <summary>
    /// Google Gemini 1.5 Pro - Strong multilingual support.
    /// Excellent for non-English text and complex layouts.
    /// </summary>
    public const string Gemini15Pro = "gemini-1.5-pro";

    /// <summary>
    /// Google Gemini 1.5 Flash - Fast Gemini model.
    /// Good for quick OCR tasks with clear text.
    /// </summary>
    public const string Gemini15Flash = "gemini-1.5-flash";

    /// <summary>
    /// Model recommendations by use case.
    /// </summary>
    public static class Recommendations
    {
        /// <summary>Best for general purpose OCR.</summary>
        public const string GeneralPurpose = Gpt4o;

        /// <summary>Best for handwriting recognition.</summary>
        public const string Handwriting = Claude3Opus;

        /// <summary>Best for scanned documents.</summary>
        public const string ScannedDocuments = Gpt4Turbo;

        /// <summary>Best for receipts and invoices.</summary>
        public const string Receipts = Gpt4o;

        /// <summary>Best for screenshots and digital text.</summary>
        public const string Screenshots = Gpt4oMini;

        /// <summary>Best for forms and structured documents.</summary>
        public const string Forms = Claude35Sonnet;

        /// <summary>Best for non-English text.</summary>
        public const string Multilingual = Gemini15Pro;

        /// <summary>Best for speed/cost when quality is less critical.</summary>
        public const string Fast = Gpt4oMini;

        /// <summary>Best for maximum accuracy.</summary>
        public const string HighAccuracy = Claude3Opus;
    }
}
