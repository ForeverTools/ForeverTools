using System.ClientModel;
using System.Text.Json;
using ForeverTools.Summarize.Models;
using OpenAI;
using OpenAI.Chat;

namespace ForeverTools.Summarize;

/// <summary>
/// AI-powered text summarization client using GPT-4, Claude, and other models.
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public class SummarizeClient
{
    private readonly OpenAIClient _client;
    private readonly SummarizeOptions _options;

    /// <summary>
    /// Creates a new SummarizeClient with the specified API key.
    /// Get your API key at: https://aimlapi.com?via=forevertools
    /// </summary>
    public SummarizeClient(string apiKey) : this(new SummarizeOptions { ApiKey = apiKey })
    {
    }

    /// <summary>
    /// Creates a new SummarizeClient with full options.
    /// </summary>
    public SummarizeClient(SummarizeOptions options)
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
    /// Uses AIML_API_KEY or SUMMARIZE_API_KEY.
    /// </summary>
    public static SummarizeClient FromEnvironment()
    {
        var apiKey = Environment.GetEnvironmentVariable("AIML_API_KEY")
            ?? Environment.GetEnvironmentVariable("SUMMARIZE_API_KEY")
            ?? throw new InvalidOperationException(
                "Environment variable 'AIML_API_KEY' or 'SUMMARIZE_API_KEY' not set. " +
                "Get your API key at https://aimlapi.com?via=forevertools");

        return new SummarizeClient(apiKey);
    }

    /// <summary>
    /// Gets the configured options.
    /// </summary>
    public SummarizeOptions Options => _options;

    #region Simple Summarization

    /// <summary>
    /// Summarizes text using default settings.
    /// </summary>
    public async Task<string> SummarizeAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var result = await SummarizeWithDetailsAsync(text, cancellationToken);
        return result.Summary;
    }

    /// <summary>
    /// Summarizes text with the specified style.
    /// </summary>
    public async Task<string> SummarizeAsync(
        string text,
        SummaryStyle style,
        CancellationToken cancellationToken = default)
    {
        var result = await SummarizeWithDetailsAsync(new SummaryRequest
        {
            Text = text,
            Style = style
        }, cancellationToken);
        return result.Summary;
    }

    /// <summary>
    /// Summarizes text with the specified style and length.
    /// </summary>
    public async Task<string> SummarizeAsync(
        string text,
        SummaryStyle style,
        SummaryLength length,
        CancellationToken cancellationToken = default)
    {
        var result = await SummarizeWithDetailsAsync(new SummaryRequest
        {
            Text = text,
            Style = style,
            Length = length
        }, cancellationToken);
        return result.Summary;
    }

    /// <summary>
    /// Gets a very brief TL;DR summary.
    /// </summary>
    public async Task<string> TldrAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var result = await SummarizeWithDetailsAsync(new SummaryRequest
        {
            Text = text,
            Style = SummaryStyle.TLDR,
            Length = SummaryLength.VeryShort
        }, cancellationToken);
        return result.Summary;
    }

    /// <summary>
    /// Gets a headline-style single sentence summary.
    /// </summary>
    public async Task<string> HeadlineAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var result = await SummarizeWithDetailsAsync(new SummaryRequest
        {
            Text = text,
            Style = SummaryStyle.Headline,
            Length = SummaryLength.VeryShort
        }, cancellationToken);
        return result.Summary;
    }

    /// <summary>
    /// Gets bullet point summary.
    /// </summary>
    public async Task<string> BulletPointsAsync(
        string text,
        SummaryLength length = SummaryLength.Medium,
        CancellationToken cancellationToken = default)
    {
        var result = await SummarizeWithDetailsAsync(new SummaryRequest
        {
            Text = text,
            Style = SummaryStyle.BulletPoints,
            Length = length
        }, cancellationToken);
        return result.Summary;
    }

    /// <summary>
    /// Gets an executive summary suitable for business documents.
    /// </summary>
    public async Task<string> ExecutiveSummaryAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var result = await SummarizeWithDetailsAsync(new SummaryRequest
        {
            Text = text,
            Style = SummaryStyle.Executive,
            Length = SummaryLength.Medium,
            Domain = ContentDomain.Business
        }, cancellationToken);
        return result.Summary;
    }

    /// <summary>
    /// Gets an academic abstract.
    /// </summary>
    public async Task<string> AbstractAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var result = await SummarizeWithDetailsAsync(new SummaryRequest
        {
            Text = text,
            Style = SummaryStyle.Abstract,
            Length = SummaryLength.Medium,
            Domain = ContentDomain.Academic
        }, cancellationToken);
        return result.Summary;
    }

    #endregion

    #region Detailed Summarization

    /// <summary>
    /// Summarizes text and returns detailed results.
    /// </summary>
    public async Task<SummaryResult> SummarizeWithDetailsAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        return await SummarizeWithDetailsAsync(new SummaryRequest { Text = text }, cancellationToken);
    }

    /// <summary>
    /// Summarizes text using a full request object with all options.
    /// </summary>
    public async Task<SummaryResult> SummarizeWithDetailsAsync(
        SummaryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            throw new ArgumentException("Text to summarize is required.", nameof(request));

        var model = request.Model ?? _options.DefaultModel;
        var style = request.Style ?? _options.DefaultStyle;
        var length = request.Length ?? _options.DefaultLength;

        var prompt = BuildSummarizationPrompt(request, style, length);

        var chatClient = _client.GetChatClient(model);
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(GetSystemPrompt(style, request.Domain)),
            new UserChatMessage(prompt)
        };

        var response = await chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        var summaryText = response.Value.Content[0].Text.Trim();

        // Parse additional data if requested
        IReadOnlyList<string>? keyPoints = null;
        IReadOnlyList<string>? statistics = null;

        if (request.ExtractKeyPoints || request.ExtractStatistics)
        {
            var parsed = ParseEnhancedResponse(summaryText, request.ExtractKeyPoints, request.ExtractStatistics);
            summaryText = parsed.summary;
            keyPoints = parsed.keyPoints;
            statistics = parsed.statistics;
        }

        var originalWordCount = CountWords(request.Text);
        var summaryWordCount = CountWords(summaryText);

        return new SummaryResult
        {
            Summary = summaryText,
            OriginalText = request.Text,
            Style = style,
            Length = length,
            Model = model,
            OriginalWordCount = originalWordCount,
            SummaryWordCount = summaryWordCount,
            KeyPoints = keyPoints,
            Statistics = statistics,
            DetectedDomain = request.Domain
        };
    }

    #endregion

    #region Key Points Extraction

    /// <summary>
    /// Extracts key points from text.
    /// </summary>
    public async Task<IReadOnlyList<string>> ExtractKeyPointsAsync(
        string text,
        int maxPoints = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await ExtractKeyPointsWithDetailsAsync(text, maxPoints, cancellationToken);
        return result.KeyPoints;
    }

    /// <summary>
    /// Extracts key points with detailed results.
    /// </summary>
    public async Task<KeyPointsResult> ExtractKeyPointsWithDetailsAsync(
        string text,
        int maxPoints = 10,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text is required for key point extraction.", nameof(text));

        var model = _options.DefaultModel;
        var chatClient = _client.GetChatClient(model);

        var prompt = $@"Extract the {maxPoints} most important key points from the following text.
Return them as a JSON object with:
- ""keyPoints"": array of strings (the key points)
- ""themes"": array of strings (main themes/topics identified)

Text to analyze:
{text}

Respond ONLY with the JSON object, no other text.";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("You are an expert at analyzing text and extracting key information. Respond only with valid JSON."),
            new UserChatMessage(prompt)
        };

        var response = await chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        var jsonResponse = CleanJsonResponse(response.Value.Content[0].Text.Trim());

        try
        {
            var json = JsonDocument.Parse(jsonResponse);
            var root = json.RootElement;

            var keyPoints = root.GetProperty("keyPoints")
                .EnumerateArray()
                .Select(e => e.GetString() ?? "")
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            var themes = root.TryGetProperty("themes", out var themesElement)
                ? themesElement.EnumerateArray()
                    .Select(e => e.GetString() ?? "")
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList()
                : null;

            return new KeyPointsResult
            {
                KeyPoints = keyPoints,
                OriginalText = text,
                Model = model,
                Themes = themes
            };
        }
        catch (JsonException)
        {
            // Fallback: split by lines if JSON parsing fails
            var lines = jsonResponse.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.TrimStart('-', '*', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', ' '))
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Take(maxPoints)
                .ToList();

            return new KeyPointsResult
            {
                KeyPoints = lines,
                OriginalText = text,
                Model = model
            };
        }
    }

    #endregion

    #region Action Items Extraction

    /// <summary>
    /// Extracts action items from meeting notes or similar text.
    /// </summary>
    public async Task<ActionItemsResult> ExtractActionItemsAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text is required for action item extraction.", nameof(text));

        var model = _options.DefaultModel;
        var chatClient = _client.GetChatClient(model);

        var prompt = @$"Extract all action items from the following text (meeting notes, email, etc.).
Return a JSON object with:
- ""actionItems"": array of objects, each with:
  - ""description"": the action item description
  - ""assignee"": person/team responsible (null if not specified)
  - ""dueDate"": deadline mentioned (null if not specified)
  - ""priority"": ""Low"", ""Normal"", ""High"", or ""Urgent""

Text to analyze:
{text}

Respond ONLY with the JSON object, no other text.";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("You are an expert at extracting action items from meeting notes and documents. Respond only with valid JSON."),
            new UserChatMessage(prompt)
        };

        var response = await chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        var jsonResponse = CleanJsonResponse(response.Value.Content[0].Text.Trim());

        try
        {
            var json = JsonDocument.Parse(jsonResponse);
            var root = json.RootElement;

            var actionItems = root.GetProperty("actionItems")
                .EnumerateArray()
                .Select(e => new ActionItem
                {
                    Description = e.GetProperty("description").GetString() ?? "",
                    Assignee = e.TryGetProperty("assignee", out var a) ? a.GetString() : null,
                    DueDate = e.TryGetProperty("dueDate", out var d) ? d.GetString() : null,
                    Priority = e.TryGetProperty("priority", out var p)
                        ? Enum.TryParse<ActionPriority>(p.GetString(), true, out var priority)
                            ? priority
                            : ActionPriority.Normal
                        : ActionPriority.Normal
                })
                .ToList();

            return new ActionItemsResult
            {
                ActionItems = actionItems,
                OriginalText = text,
                Model = model
            };
        }
        catch (JsonException)
        {
            return new ActionItemsResult
            {
                ActionItems = Array.Empty<ActionItem>(),
                OriginalText = text,
                Model = model
            };
        }
    }

    #endregion

    #region Batch Summarization

    /// <summary>
    /// Summarizes multiple texts.
    /// </summary>
    public async Task<BatchSummaryResult> SummarizeBatchAsync(
        IEnumerable<string> texts,
        CancellationToken cancellationToken = default)
    {
        return await SummarizeBatchAsync(texts, _options.DefaultStyle, _options.DefaultLength, cancellationToken);
    }

    /// <summary>
    /// Summarizes multiple texts with specified style and length.
    /// </summary>
    public async Task<BatchSummaryResult> SummarizeBatchAsync(
        IEnumerable<string> texts,
        SummaryStyle style,
        SummaryLength length,
        CancellationToken cancellationToken = default)
    {
        var textList = texts.ToList();
        var results = new List<SummaryResult>();
        var errors = new List<BatchSummaryError>();

        var semaphore = new SemaphoreSlim(_options.MaxParallelRequests);
        var tasks = textList.Select(async (text, index) =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var result = await SummarizeWithDetailsAsync(new SummaryRequest
                {
                    Text = text,
                    Style = style,
                    Length = length
                }, cancellationToken);
                return (Index: index, Result: result, Error: (string?)null);
            }
            catch (Exception ex)
            {
                return (Index: index, Result: (SummaryResult?)null, Error: ex.Message);
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
                errors.Add(new BatchSummaryError
                {
                    Index = index,
                    OriginalText = textList[index],
                    ErrorMessage = error ?? "Unknown error"
                });
            }
        }

        return new BatchSummaryResult
        {
            Results = results,
            SuccessCount = results.Count,
            FailureCount = errors.Count,
            Errors = errors.Count > 0 ? errors : null
        };
    }

    #endregion

    #region Specialized Summarization

    /// <summary>
    /// Summarizes text for a specific audience.
    /// </summary>
    public async Task<string> SummarizeForAudienceAsync(
        string text,
        string targetAudience,
        CancellationToken cancellationToken = default)
    {
        var result = await SummarizeWithDetailsAsync(new SummaryRequest
        {
            Text = text,
            TargetAudience = targetAudience
        }, cancellationToken);
        return result.Summary;
    }

    /// <summary>
    /// Summarizes text with focus on specific topics.
    /// </summary>
    public async Task<string> SummarizeWithFocusAsync(
        string text,
        IEnumerable<string> focusAreas,
        CancellationToken cancellationToken = default)
    {
        var result = await SummarizeWithDetailsAsync(new SummaryRequest
        {
            Text = text,
            FocusAreas = focusAreas
        }, cancellationToken);
        return result.Summary;
    }

    /// <summary>
    /// Summarizes a legal document.
    /// </summary>
    public async Task<string> SummarizeLegalAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var result = await SummarizeWithDetailsAsync(new SummaryRequest
        {
            Text = text,
            Domain = ContentDomain.Legal,
            Style = SummaryStyle.Structured,
            Length = SummaryLength.Long
        }, cancellationToken);
        return result.Summary;
    }

    /// <summary>
    /// Summarizes meeting notes with action items.
    /// </summary>
    public async Task<SummaryResult> SummarizeMeetingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        return await SummarizeWithDetailsAsync(new SummaryRequest
        {
            Text = text,
            Domain = ContentDomain.Meeting,
            Style = SummaryStyle.Structured,
            ExtractKeyPoints = true
        }, cancellationToken);
    }

    /// <summary>
    /// Compares and summarizes multiple texts together.
    /// </summary>
    public async Task<SummaryComparisonResult> CompareAndSummarizeAsync(
        IEnumerable<string> texts,
        CancellationToken cancellationToken = default)
    {
        var textList = texts.ToList();
        if (textList.Count < 2)
            throw new ArgumentException("At least 2 texts are required for comparison.", nameof(texts));

        // First, summarize each text individually
        var batchResult = await SummarizeBatchAsync(textList, cancellationToken);

        // Then compare them
        var model = _options.DefaultModel;
        var chatClient = _client.GetChatClient(model);

        var combinedTexts = string.Join("\n\n---\n\n", textList.Select((t, i) => $"Document {i + 1}:\n{t}"));

        var prompt = $@"Analyze the following {textList.Count} documents and provide:
1. A combined summary that synthesizes the key information from all documents
2. Common themes across all documents
3. Key differences between the documents

Return as JSON with:
- ""combinedSummary"": string
- ""commonThemes"": array of strings
- ""keyDifferences"": array of strings

Documents:
{combinedTexts}

Respond ONLY with the JSON object.";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("You are an expert at comparative analysis of documents. Respond only with valid JSON."),
            new UserChatMessage(prompt)
        };

        var response = await chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        var jsonResponse = CleanJsonResponse(response.Value.Content[0].Text.Trim());

        try
        {
            var json = JsonDocument.Parse(jsonResponse);
            var root = json.RootElement;

            return new SummaryComparisonResult
            {
                CombinedSummary = root.GetProperty("combinedSummary").GetString() ?? "",
                CommonThemes = root.GetProperty("commonThemes")
                    .EnumerateArray()
                    .Select(e => e.GetString() ?? "")
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList(),
                KeyDifferences = root.GetProperty("keyDifferences")
                    .EnumerateArray()
                    .Select(e => e.GetString() ?? "")
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList(),
                IndividualSummaries = batchResult.Results
            };
        }
        catch (JsonException)
        {
            return new SummaryComparisonResult
            {
                CombinedSummary = jsonResponse,
                CommonThemes = Array.Empty<string>(),
                KeyDifferences = Array.Empty<string>(),
                IndividualSummaries = batchResult.Results
            };
        }
    }

    #endregion

    #region Private Helpers

    private string GetSystemPrompt(SummaryStyle style, ContentDomain? domain)
    {
        var basePrompt = "You are an expert summarizer with the ability to distill complex information into clear, concise summaries.";

        var domainPrompt = domain switch
        {
            ContentDomain.Legal => " You have expertise in legal documents and contracts.",
            ContentDomain.Medical => " You have expertise in medical and healthcare content.",
            ContentDomain.Financial => " You have expertise in financial reports and analysis.",
            ContentDomain.Academic => " You have expertise in academic and research papers.",
            ContentDomain.Technical => " You have expertise in technical documentation.",
            ContentDomain.Business => " You have expertise in business reports and communications.",
            ContentDomain.Meeting => " You have expertise in meeting notes and extracting action items.",
            _ => ""
        };

        var stylePrompt = style switch
        {
            SummaryStyle.BulletPoints => " Present summaries as clear bullet points.",
            SummaryStyle.NumberedList => " Present summaries as numbered lists.",
            SummaryStyle.Executive => " Write executive-style summaries suitable for senior leadership.",
            SummaryStyle.Abstract => " Write academic abstract-style summaries.",
            SummaryStyle.TLDR => " Provide extremely brief TL;DR summaries.",
            SummaryStyle.Structured => " Provide structured summaries with clear sections.",
            SummaryStyle.Headline => " Provide single-sentence headline summaries.",
            SummaryStyle.QAndA => " Provide summaries in Q&A format.",
            _ => " Write clear, flowing paragraph summaries."
        };

        return basePrompt + domainPrompt + stylePrompt;
    }

    private string BuildSummarizationPrompt(SummaryRequest request, SummaryStyle style, SummaryLength length)
    {
        var prompt = new System.Text.StringBuilder();

        prompt.AppendLine("Summarize the following text.");

        // Length instructions
        var lengthInstruction = length switch
        {
            SummaryLength.VeryShort => "Keep it very brief - 1-2 sentences or 3-5 bullet points maximum.",
            SummaryLength.Short => "Keep it short - 2-3 sentences or 5-7 bullet points.",
            SummaryLength.Medium => "Provide a medium-length summary - about 1 paragraph or 7-10 bullet points.",
            SummaryLength.Long => "Provide a detailed summary - 2-3 paragraphs or 10-15 bullet points.",
            SummaryLength.Detailed => "Provide a comprehensive summary preserving important details.",
            SummaryLength.Custom when request.CustomWordCount.HasValue =>
                $"Keep the summary to approximately {request.CustomWordCount.Value} words.",
            SummaryLength.Custom when request.CustomPercentage.HasValue =>
                $"Reduce the text to approximately {request.CustomPercentage.Value}% of its original length.",
            _ => "Provide a concise but complete summary."
        };
        prompt.AppendLine(lengthInstruction);

        // Style instructions
        var styleInstruction = style switch
        {
            SummaryStyle.BulletPoints => "Format as bullet points (use - for each point).",
            SummaryStyle.NumberedList => "Format as a numbered list.",
            SummaryStyle.Executive => "Write as an executive summary with key findings and recommendations.",
            SummaryStyle.Abstract => "Write in academic abstract style with background, methods (if applicable), results, and conclusions.",
            SummaryStyle.TLDR => "Provide a single TL;DR statement.",
            SummaryStyle.Structured => "Use sections: Overview, Key Points, and Conclusion.",
            SummaryStyle.Headline => "Provide a single headline-style sentence.",
            SummaryStyle.QAndA => "Format as key questions and their answers based on the text.",
            _ => "Write as flowing paragraph(s)."
        };
        prompt.AppendLine(styleInstruction);

        // Domain context
        if (request.Domain.HasValue && request.Domain != ContentDomain.General)
        {
            prompt.AppendLine($"This is a {request.Domain.Value} document - preserve relevant terminology and context.");
        }

        // Target audience
        if (!string.IsNullOrEmpty(request.TargetAudience))
        {
            prompt.AppendLine($"Target audience: {request.TargetAudience}. Adjust language and detail level accordingly.");
        }

        // Focus areas
        if (request.FocusAreas?.Any() == true)
        {
            prompt.AppendLine($"Focus especially on these topics: {string.Join(", ", request.FocusAreas)}");
        }

        // Additional context
        if (!string.IsNullOrEmpty(request.Context))
        {
            prompt.AppendLine($"Additional context: {request.Context}");
        }

        // Key points extraction
        if (request.ExtractKeyPoints)
        {
            prompt.AppendLine("After the summary, list the key points under a 'KEY POINTS:' heading.");
        }

        // Statistics extraction
        if (request.ExtractStatistics)
        {
            prompt.AppendLine("If there are important numbers/statistics, list them under a 'KEY STATISTICS:' heading.");
        }

        // Preserve quotes
        if (request.PreserveQuotes)
        {
            prompt.AppendLine("Preserve any important direct quotes from the original text.");
        }

        // Output language
        if (!string.IsNullOrEmpty(request.OutputLanguage))
        {
            prompt.AppendLine($"Write the summary in {request.OutputLanguage}.");
        }

        prompt.AppendLine();
        prompt.AppendLine("Text to summarize:");
        prompt.AppendLine(request.Text);

        return prompt.ToString();
    }

    private static (string summary, IReadOnlyList<string>? keyPoints, IReadOnlyList<string>? statistics) ParseEnhancedResponse(
        string response, bool extractKeyPoints, bool extractStatistics)
    {
        var summary = response;
        IReadOnlyList<string>? keyPoints = null;
        IReadOnlyList<string>? statistics = null;

        // Extract key points section
        if (extractKeyPoints)
        {
            var keyPointsIndex = response.IndexOf("KEY POINTS:", StringComparison.OrdinalIgnoreCase);
            if (keyPointsIndex >= 0)
            {
                var keyPointsSection = response.Substring(keyPointsIndex + "KEY POINTS:".Length);
                var endIndex = keyPointsSection.IndexOf("KEY STATISTICS:", StringComparison.OrdinalIgnoreCase);
                if (endIndex >= 0)
                    keyPointsSection = keyPointsSection.Substring(0, endIndex);

                keyPoints = keyPointsSection
                    .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.TrimStart('-', '*', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', ' '))
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .ToList();

                summary = response.Substring(0, keyPointsIndex).Trim();
            }
        }

        // Extract statistics section
        if (extractStatistics)
        {
            var statsIndex = response.IndexOf("KEY STATISTICS:", StringComparison.OrdinalIgnoreCase);
            if (statsIndex >= 0)
            {
                var statsSection = response.Substring(statsIndex + "KEY STATISTICS:".Length);
                statistics = statsSection
                    .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.TrimStart('-', '*', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', ' '))
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .ToList();

                if (summary.Contains("KEY STATISTICS:"))
                    summary = summary.Substring(0, summary.IndexOf("KEY STATISTICS:", StringComparison.OrdinalIgnoreCase)).Trim();
            }
        }

        return (summary, keyPoints, statistics);
    }

    private static string CleanJsonResponse(string response)
    {
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

    private static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    #endregion
}
