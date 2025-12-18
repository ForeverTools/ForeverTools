using System.ClientModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using ForeverTools.Translate.Models;
using OpenAI;
using OpenAI.Chat;

namespace ForeverTools.Translate;

/// <summary>
/// AI-powered translation client using GPT-4, Claude, and other models.
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public class TranslationClient
{
    private readonly OpenAIClient _client;
    private readonly TranslationOptions _options;

    /// <summary>
    /// Creates a new TranslationClient with the specified API key.
    /// Get your API key at: https://aimlapi.com?via=forevertools
    /// </summary>
    public TranslationClient(string apiKey) : this(new TranslationOptions { ApiKey = apiKey })
    {
    }

    /// <summary>
    /// Creates a new TranslationClient with full options.
    /// </summary>
    public TranslationClient(TranslationOptions options)
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
    }

    /// <summary>
    /// Creates a client from environment variable.
    /// Uses AIML_API_KEY or TRANSLATION_API_KEY.
    /// </summary>
    public static TranslationClient FromEnvironment()
    {
        var apiKey = Environment.GetEnvironmentVariable("AIML_API_KEY")
            ?? Environment.GetEnvironmentVariable("TRANSLATION_API_KEY")
            ?? throw new InvalidOperationException(
                "Environment variable 'AIML_API_KEY' or 'TRANSLATION_API_KEY' not set. " +
                "Get your API key at https://aimlapi.com?via=forevertools");

        return new TranslationClient(apiKey);
    }

    /// <summary>
    /// Gets the configured options.
    /// </summary>
    public TranslationOptions Options => _options;

    #region Simple Translation

    /// <summary>
    /// Translates text to the specified target language.
    /// Source language is auto-detected.
    /// </summary>
    public async Task<string> TranslateAsync(
        string text,
        string targetLanguage,
        CancellationToken cancellationToken = default)
    {
        var result = await TranslateWithDetailsAsync(text, null, targetLanguage, cancellationToken);
        return result.TranslatedText;
    }

    /// <summary>
    /// Translates text from source language to target language.
    /// </summary>
    public async Task<string> TranslateAsync(
        string text,
        string sourceLanguage,
        string targetLanguage,
        CancellationToken cancellationToken = default)
    {
        var result = await TranslateWithDetailsAsync(text, sourceLanguage, targetLanguage, cancellationToken);
        return result.TranslatedText;
    }

    /// <summary>
    /// Translates text to English (auto-detecting source language).
    /// </summary>
    public async Task<string> TranslateToEnglishAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        return await TranslateAsync(text, "en", cancellationToken);
    }

    /// <summary>
    /// Translates text from English to the specified language.
    /// </summary>
    public async Task<string> TranslateFromEnglishAsync(
        string text,
        string targetLanguage,
        CancellationToken cancellationToken = default)
    {
        return await TranslateAsync(text, "en", targetLanguage, cancellationToken);
    }

    #endregion

    #region Detailed Translation

    /// <summary>
    /// Translates text and returns detailed results including detected language.
    /// </summary>
    public async Task<TranslationResult> TranslateWithDetailsAsync(
        string text,
        string? sourceLanguage,
        string targetLanguage,
        CancellationToken cancellationToken = default)
    {
        return await TranslateWithDetailsAsync(new TranslationRequest
        {
            Text = text,
            SourceLanguage = sourceLanguage,
            TargetLanguage = targetLanguage
        }, cancellationToken);
    }

    /// <summary>
    /// Translates text using a full request object with all options.
    /// </summary>
    public async Task<TranslationResult> TranslateWithDetailsAsync(
        TranslationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            throw new ArgumentException("Text to translate is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.TargetLanguage))
            throw new ArgumentException("Target language is required.", nameof(request));

        var model = request.Model ?? _options.DefaultModel;
        var style = request.Style ?? _options.Style;
        var sourceLanguage = request.SourceLanguage ?? _options.DefaultSourceLanguage;
        var targetLang = GetLanguageInfo(request.TargetLanguage);

        var prompt = BuildTranslationPrompt(request.Text, sourceLanguage, targetLang, style, request.Context, request.Glossary);

        var chatClient = _client.GetChatClient(model);
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(GetSystemPrompt(style)),
            new UserChatMessage(prompt)
        };

        var response = await chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        var translatedText = response.Value.Content[0].Text;

        // Parse the response to extract translation and metadata
        var (translation, detectedLanguage, confidence) = ParseTranslationResponse(translatedText, request.Text);

        return new TranslationResult
        {
            OriginalText = request.Text,
            TranslatedText = translation,
            SourceLanguage = detectedLanguage ?? sourceLanguage ?? "auto",
            TargetLanguage = request.TargetLanguage,
            Model = model,
            WasLanguageDetected = sourceLanguage == null,
            DetectionConfidence = confidence
        };
    }

    #endregion

    #region Language Detection

    /// <summary>
    /// Detects the language of the given text.
    /// </summary>
    public async Task<LanguageDetectionResult> DetectLanguageAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text is required for language detection.", nameof(text));

        var chatClient = _client.GetChatClient(_options.DefaultModel);

        var prompt = $@"Detect the language of the following text. Respond with a JSON object containing:
- ""code"": the ISO 639-1 language code (e.g., ""en"", ""es"", ""fr"")
- ""name"": the English name of the language
- ""confidence"": a number between 0 and 1 indicating confidence

Text to analyze:
{text}

Respond ONLY with the JSON object, no other text.";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("You are a language detection expert. Respond only with valid JSON."),
            new UserChatMessage(prompt)
        };

        var response = await chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        var jsonResponse = response.Value.Content[0].Text.Trim();

        // Clean up the response (remove markdown code blocks if present)
        jsonResponse = CleanJsonResponse(jsonResponse);

        try
        {
            var json = JsonDocument.Parse(jsonResponse);
            var root = json.RootElement;

            return new LanguageDetectionResult
            {
                LanguageCode = root.GetProperty("code").GetString() ?? "unknown",
                LanguageName = root.GetProperty("name").GetString() ?? "Unknown",
                Confidence = root.TryGetProperty("confidence", out var conf) ? conf.GetDouble() : 0.9,
                AnalyzedText = text
            };
        }
        catch (JsonException)
        {
            // Fallback: try to extract language from plain text response
            return new LanguageDetectionResult
            {
                LanguageCode = "unknown",
                LanguageName = jsonResponse,
                Confidence = 0.5,
                AnalyzedText = text
            };
        }
    }

    #endregion

    #region Batch Translation

    /// <summary>
    /// Translates multiple texts to the target language.
    /// </summary>
    public async Task<BatchTranslationResult> TranslateBatchAsync(
        IEnumerable<string> texts,
        string targetLanguage,
        CancellationToken cancellationToken = default)
    {
        return await TranslateBatchAsync(texts, null, targetLanguage, cancellationToken);
    }

    /// <summary>
    /// Translates multiple texts from source to target language.
    /// </summary>
    public async Task<BatchTranslationResult> TranslateBatchAsync(
        IEnumerable<string> texts,
        string? sourceLanguage,
        string targetLanguage,
        CancellationToken cancellationToken = default)
    {
        var textList = texts.ToList();
        var results = new List<TranslationResult>();
        var errors = new List<BatchTranslationError>();

        var semaphore = new SemaphoreSlim(_options.MaxParallelRequests);
        var tasks = textList.Select(async (text, index) =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var result = await TranslateWithDetailsAsync(text, sourceLanguage, targetLanguage, cancellationToken);
                return (Index: index, Result: result, Error: (string?)null);
            }
            catch (Exception ex)
            {
                return (Index: index, Result: (TranslationResult?)null, Error: ex.Message);
            }
            finally
            {
                semaphore.Release();
            }
        });

        var taskResults = await Task.WhenAll(tasks);

        foreach (var (index, result, error) in taskResults.OrderBy(r => r.Index))
        {
            if (result != null)
            {
                results.Add(result);
            }
            else
            {
                errors.Add(new BatchTranslationError
                {
                    Index = index,
                    OriginalText = textList[index],
                    ErrorMessage = error ?? "Unknown error"
                });
            }
        }

        return new BatchTranslationResult
        {
            Results = results,
            SuccessCount = results.Count,
            FailureCount = errors.Count,
            Errors = errors.Count > 0 ? errors : null
        };
    }

    /// <summary>
    /// Translates text to multiple target languages simultaneously.
    /// </summary>
    public async Task<IReadOnlyList<TranslationResult>> TranslateToMultipleLanguagesAsync(
        string text,
        IEnumerable<string> targetLanguages,
        CancellationToken cancellationToken = default)
    {
        var semaphore = new SemaphoreSlim(_options.MaxParallelRequests);
        var tasks = targetLanguages.Select(async lang =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                return await TranslateWithDetailsAsync(text, null, lang, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        });

        var results = await Task.WhenAll(tasks);
        return results;
    }

    #endregion

    #region Specialized Translation

    /// <summary>
    /// Translates text with a specific style/tone.
    /// </summary>
    public async Task<string> TranslateWithStyleAsync(
        string text,
        string targetLanguage,
        TranslationStyle style,
        CancellationToken cancellationToken = default)
    {
        var result = await TranslateWithDetailsAsync(new TranslationRequest
        {
            Text = text,
            TargetLanguage = targetLanguage,
            Style = style
        }, cancellationToken);

        return result.TranslatedText;
    }

    /// <summary>
    /// Translates text using a glossary for consistent terminology.
    /// </summary>
    public async Task<string> TranslateWithGlossaryAsync(
        string text,
        string targetLanguage,
        IDictionary<string, string> glossary,
        CancellationToken cancellationToken = default)
    {
        var result = await TranslateWithDetailsAsync(new TranslationRequest
        {
            Text = text,
            TargetLanguage = targetLanguage,
            Glossary = glossary
        }, cancellationToken);

        return result.TranslatedText;
    }

    /// <summary>
    /// Translates text with context for better accuracy.
    /// </summary>
    public async Task<string> TranslateWithContextAsync(
        string text,
        string targetLanguage,
        string context,
        CancellationToken cancellationToken = default)
    {
        var result = await TranslateWithDetailsAsync(new TranslationRequest
        {
            Text = text,
            TargetLanguage = targetLanguage,
            Context = context
        }, cancellationToken);

        return result.TranslatedText;
    }

    #endregion

    #region Private Helpers

    private string GetSystemPrompt(TranslationStyle style)
    {
        var basePrompt = "You are an expert translator with native-level fluency in all languages.";

        return style switch
        {
            TranslationStyle.Formal => $"{basePrompt} Use formal, professional language suitable for business documents.",
            TranslationStyle.Casual => $"{basePrompt} Use casual, conversational language that feels natural and friendly.",
            TranslationStyle.Technical => $"{basePrompt} Preserve technical terminology accurately while ensuring clarity.",
            TranslationStyle.Literal => $"{basePrompt} Translate as literally as possible while maintaining grammatical correctness.",
            TranslationStyle.Creative => $"{basePrompt} Adapt the translation creatively to sound natural in the target language, even if it differs from the literal meaning.",
            _ => $"{basePrompt} Produce natural, fluent translations that read well in the target language."
        };
    }

    private string BuildTranslationPrompt(
        string text,
        string? sourceLanguage,
        Language targetLanguage,
        TranslationStyle style,
        string? context,
        IDictionary<string, string>? glossary)
    {
        var prompt = new System.Text.StringBuilder();

        if (sourceLanguage != null)
        {
            var sourceLang = GetLanguageInfo(sourceLanguage);
            prompt.AppendLine($"Translate the following text from {sourceLang.Name} to {targetLanguage.Name}.");
        }
        else
        {
            prompt.AppendLine($"Detect the source language and translate the following text to {targetLanguage.Name}.");
        }

        if (context != null)
        {
            prompt.AppendLine($"\nContext: {context}");
        }

        if (glossary != null && glossary.Count > 0)
        {
            prompt.AppendLine("\nUse these specific translations for the following terms:");
            foreach (var kvp in glossary)
            {
                prompt.AppendLine($"- \"{kvp.Key}\" → \"{kvp.Value}\"");
            }
        }

        if (_options.PreserveFormatting)
        {
            prompt.AppendLine("\nPreserve the original formatting (paragraphs, bullet points, etc.).");
        }

        prompt.AppendLine($"\nText to translate:\n{text}");
        prompt.AppendLine("\nProvide ONLY the translation, no explanations or notes.");

        return prompt.ToString();
    }

    private static Language GetLanguageInfo(string code)
    {
        return Languages.All.TryGetValue(code.ToLowerInvariant(), out var lang)
            ? lang
            : new Language(code, code, code);
    }

    private static (string translation, string? detectedLanguage, double? confidence) ParseTranslationResponse(
        string response,
        string originalText)
    {
        // The response should be just the translation
        // Clean up any potential prefixes like "Translation:" or markdown
        var cleaned = response.Trim();

        // Remove common prefixes
        var prefixes = new[] { "Translation:", "Translated text:", "Here is the translation:" };
        foreach (var prefix in prefixes)
        {
            if (cleaned.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Substring(prefix.Length).Trim();
            }
        }

        // Remove markdown code blocks
        cleaned = CleanJsonResponse(cleaned);

        return (cleaned, null, null);
    }

    private static string CleanJsonResponse(string response)
    {
        // Remove markdown code blocks
        var cleaned = response.Trim();
        if (cleaned.StartsWith("```"))
        {
            var endIndex = cleaned.IndexOf('\n');
            if (endIndex > 0)
            {
                cleaned = cleaned.Substring(endIndex + 1);
            }
            if (cleaned.EndsWith("```"))
            {
                cleaned = cleaned.Substring(0, cleaned.Length - 3);
            }
        }
        return cleaned.Trim();
    }

    #endregion
}
