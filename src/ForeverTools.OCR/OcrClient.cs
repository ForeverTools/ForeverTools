using System.ClientModel;
using System.Diagnostics;
using System.Text.Json;
using OpenAI;
using OpenAI.Chat;
using ForeverTools.OCR.Models;

namespace ForeverTools.OCR;

/// <summary>
/// AI-powered OCR client for extracting text from images.
/// Uses GPT-4 Vision, Claude, and other vision models via AI/ML API.
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public class OcrClient : IDisposable
{
    private readonly OpenAIClient _client;
    private readonly OcrOptions _options;
    private readonly HttpClient _httpClient;
    private bool _disposed;

    /// <summary>
    /// Creates a new OCR client with the specified API key.
    /// </summary>
    /// <param name="apiKey">Your AI/ML API key. Get one at https://aimlapi.com?via=forevertools</param>
    public OcrClient(string apiKey) : this(new OcrOptions { ApiKey = apiKey })
    {
    }

    /// <summary>
    /// Creates a new OCR client with the specified options.
    /// </summary>
    /// <param name="options">Configuration options for the client.</param>
    public OcrClient(OcrOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new ArgumentException(
                "API key is required. Get your key at https://aimlapi.com?via=forevertools",
                nameof(options));
        }

        _options = options;
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds) };

        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(options.BaseUrl)
        };

        _client = new OpenAIClient(new ApiKeyCredential(options.ApiKey), clientOptions);
    }

    /// <summary>
    /// Gets the configured options.
    /// </summary>
    public OcrOptions Options => _options;

    #region Simple Text Extraction

    /// <summary>
    /// Extracts all text from an image file.
    /// </summary>
    /// <param name="filePath">Path to the image file.</param>
    /// <param name="model">Vision model to use, or null for default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted text result.</returns>
    public async Task<OcrResult> ExtractTextFromFileAsync(
        string filePath,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            return OcrResult.Failed($"File not found: {filePath}");
        }

        var imageBytes = await ReadAllBytesAsync(filePath, cancellationToken);
        var mimeType = GetMimeType(filePath);
        return await ExtractTextAsync(imageBytes, mimeType, model, cancellationToken);
    }

    /// <summary>
    /// Extracts all text from image bytes.
    /// </summary>
    /// <param name="imageBytes">The image data.</param>
    /// <param name="mimeType">MIME type (e.g., "image/png"). Auto-detected if null.</param>
    /// <param name="model">Vision model to use, or null for default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted text result.</returns>
    public async Task<OcrResult> ExtractTextAsync(
        byte[] imageBytes,
        string? mimeType = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        mimeType ??= DetectMimeType(imageBytes);
        var base64 = Convert.ToBase64String(imageBytes);
        return await ExtractTextFromBase64Async(base64, mimeType, model, cancellationToken);
    }

    /// <summary>
    /// Extracts all text from a base64-encoded image.
    /// </summary>
    /// <param name="base64Image">Base64-encoded image data.</param>
    /// <param name="mimeType">MIME type (e.g., "image/png").</param>
    /// <param name="model">Vision model to use, or null for default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted text result.</returns>
    public async Task<OcrResult> ExtractTextFromBase64Async(
        string base64Image,
        string mimeType = "image/png",
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        var dataUri = $"data:{mimeType};base64,{base64Image}";
        return await ExtractTextInternalAsync(dataUri, null, model, cancellationToken);
    }

    /// <summary>
    /// Extracts all text from an image URL.
    /// </summary>
    /// <param name="imageUrl">URL of the image.</param>
    /// <param name="model">Vision model to use, or null for default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted text result.</returns>
    public async Task<OcrResult> ExtractTextFromUrlAsync(
        string imageUrl,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        return await ExtractTextInternalAsync(null, imageUrl, model, cancellationToken);
    }

    /// <summary>
    /// Extracts all text from a stream.
    /// </summary>
    /// <param name="imageStream">The image stream.</param>
    /// <param name="mimeType">MIME type (e.g., "image/png").</param>
    /// <param name="model">Vision model to use, or null for default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted text result.</returns>
    public async Task<OcrResult> ExtractTextFromStreamAsync(
        Stream imageStream,
        string mimeType = "image/png",
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        using var ms = new MemoryStream();
#if NETSTANDARD2_0
        await imageStream.CopyToAsync(ms);
#else
        await imageStream.CopyToAsync(ms, cancellationToken);
#endif
        return await ExtractTextAsync(ms.ToArray(), mimeType, model, cancellationToken);
    }

    #endregion

    #region Structured Extraction

    /// <summary>
    /// Extracts text with structure information (paragraphs, lines, blocks).
    /// </summary>
    /// <param name="imageBytes">The image data.</param>
    /// <param name="mimeType">MIME type (e.g., "image/png").</param>
    /// <param name="model">Vision model to use, or null for default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Structured OCR result.</returns>
    public async Task<StructuredOcrResult> ExtractStructuredAsync(
        byte[] imageBytes,
        string? mimeType = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        mimeType ??= DetectMimeType(imageBytes);
        var base64 = Convert.ToBase64String(imageBytes);
        var dataUri = $"data:{mimeType};base64,{base64}";

        var prompt = @"Extract all text from this image and return it as structured JSON with this exact format:
{
  ""paragraphs"": [""paragraph 1 text"", ""paragraph 2 text""],
  ""lines"": [""line 1"", ""line 2"", ""line 3""],
  ""blocks"": [
    {""text"": ""block text"", ""blockType"": ""heading|paragraph|list|caption"", ""order"": 0}
  ]
}

Extract ALL visible text. Preserve the reading order. Identify block types where possible.
Return ONLY valid JSON, no other text.";

        var sw = Stopwatch.StartNew();
        var useModel = model ?? _options.DefaultModel;

        try
        {
            var chatClient = _client.GetChatClient(useModel);
            var messages = CreateVisionMessages(prompt, dataUri, null);

            var response = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                MaxOutputTokenCount = _options.MaxTokens
            }, cancellationToken);

            sw.Stop();
            var content = response.Value.Content[0].Text;
            var tokensUsed = response.Value.Usage?.TotalTokenCount ?? 0;

            // Parse JSON response
            var result = ParseStructuredResponse(content);
            result.Model = useModel;
            result.TokensUsed = tokensUsed;
            result.ProcessingTimeMs = sw.ElapsedMilliseconds;
            result.Success = true;

            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new StructuredOcrResult
            {
                Success = false,
                Error = ex.Message,
                Model = useModel,
                ProcessingTimeMs = sw.ElapsedMilliseconds
            };
        }
    }

    /// <summary>
    /// Extracts tables from an image.
    /// </summary>
    /// <param name="imageBytes">The image data.</param>
    /// <param name="mimeType">MIME type (e.g., "image/png").</param>
    /// <param name="model">Vision model to use, or null for default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Table extraction result.</returns>
    public async Task<TableOcrResult> ExtractTablesAsync(
        byte[] imageBytes,
        string? mimeType = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        mimeType ??= DetectMimeType(imageBytes);
        var base64 = Convert.ToBase64String(imageBytes);
        var dataUri = $"data:{mimeType};base64,{base64}";

        var prompt = @"Extract all tables from this image and return as JSON with this exact format:
{
  ""tables"": [
    {
      ""headers"": [""Column 1"", ""Column 2"", ""Column 3""],
      ""rows"": [
        [""row1col1"", ""row1col2"", ""row1col3""],
        [""row2col1"", ""row2col2"", ""row2col3""]
      ]
    }
  ]
}

If there are no tables, return: {""tables"": []}
Extract ALL tables visible. Preserve cell contents exactly.
Return ONLY valid JSON, no other text.";

        var sw = Stopwatch.StartNew();
        var useModel = model ?? _options.DefaultModel;

        try
        {
            var chatClient = _client.GetChatClient(useModel);
            var messages = CreateVisionMessages(prompt, dataUri, null);

            var response = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                MaxOutputTokenCount = _options.MaxTokens
            }, cancellationToken);

            sw.Stop();
            var content = response.Value.Content[0].Text;
            var tokensUsed = response.Value.Usage?.TotalTokenCount ?? 0;

            var result = ParseTableResponse(content);
            result.Model = useModel;
            result.TokensUsed = tokensUsed;
            result.ProcessingTimeMs = sw.ElapsedMilliseconds;
            result.Success = true;

            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new TableOcrResult
            {
                Success = false,
                Error = ex.Message,
                Model = useModel,
                ProcessingTimeMs = sw.ElapsedMilliseconds
            };
        }
    }

    /// <summary>
    /// Extracts form fields (labels and values) from a document image.
    /// </summary>
    /// <param name="imageBytes">The image data.</param>
    /// <param name="mimeType">MIME type (e.g., "image/png").</param>
    /// <param name="model">Vision model to use, or null for default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Form extraction result.</returns>
    public async Task<FormOcrResult> ExtractFormFieldsAsync(
        byte[] imageBytes,
        string? mimeType = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        mimeType ??= DetectMimeType(imageBytes);
        var base64 = Convert.ToBase64String(imageBytes);
        var dataUri = $"data:{mimeType};base64,{base64}";

        var prompt = @"Extract all form fields from this image. Return as JSON with field labels as keys and field values as values:
{
  ""fields"": {
    ""Name"": ""John Smith"",
    ""Date"": ""2024-01-15"",
    ""Address"": ""123 Main St"",
    ""Email"": ""john@example.com""
  }
}

Extract ALL form fields, labels, and their values. Include checkboxes (true/false), dates, signatures (""[signature present]""), etc.
Return ONLY valid JSON, no other text.";

        var sw = Stopwatch.StartNew();
        var useModel = model ?? _options.DefaultModel;

        try
        {
            var chatClient = _client.GetChatClient(useModel);
            var messages = CreateVisionMessages(prompt, dataUri, null);

            var response = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                MaxOutputTokenCount = _options.MaxTokens
            }, cancellationToken);

            sw.Stop();
            var content = response.Value.Content[0].Text;
            var tokensUsed = response.Value.Usage?.TotalTokenCount ?? 0;

            var result = ParseFormResponse(content);
            result.Model = useModel;
            result.TokensUsed = tokensUsed;
            result.ProcessingTimeMs = sw.ElapsedMilliseconds;
            result.Success = true;

            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new FormOcrResult
            {
                Success = false,
                Error = ex.Message,
                Model = useModel,
                ProcessingTimeMs = sw.ElapsedMilliseconds
            };
        }
    }

    /// <summary>
    /// Extracts receipt/invoice data from an image.
    /// </summary>
    /// <param name="imageBytes">The image data.</param>
    /// <param name="mimeType">MIME type (e.g., "image/png").</param>
    /// <param name="model">Vision model to use, or null for default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Receipt extraction result.</returns>
    public async Task<ReceiptOcrResult> ExtractReceiptAsync(
        byte[] imageBytes,
        string? mimeType = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        mimeType ??= DetectMimeType(imageBytes);
        var base64 = Convert.ToBase64String(imageBytes);
        var dataUri = $"data:{mimeType};base64,{base64}";

        var prompt = @"Extract receipt/invoice data from this image. Return as JSON:
{
  ""merchantName"": ""Store Name"",
  ""date"": ""2024-01-15"",
  ""total"": ""$25.99"",
  ""subtotal"": ""$23.50"",
  ""tax"": ""$2.49"",
  ""currency"": ""USD"",
  ""paymentMethod"": ""VISA *1234"",
  ""items"": [
    {""description"": ""Item 1"", ""quantity"": ""2"", ""unitPrice"": ""$5.00"", ""totalPrice"": ""$10.00""},
    {""description"": ""Item 2"", ""quantity"": ""1"", ""unitPrice"": ""$13.50"", ""totalPrice"": ""$13.50""}
  ]
}

Extract all available fields. Use null for fields not found.
Return ONLY valid JSON, no other text.";

        var sw = Stopwatch.StartNew();
        var useModel = model ?? _options.DefaultModel;

        try
        {
            var chatClient = _client.GetChatClient(useModel);
            var messages = CreateVisionMessages(prompt, dataUri, null);

            var response = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                MaxOutputTokenCount = _options.MaxTokens
            }, cancellationToken);

            sw.Stop();
            var content = response.Value.Content[0].Text;
            var tokensUsed = response.Value.Usage?.TotalTokenCount ?? 0;

            var result = ParseReceiptResponse(content);
            result.Model = useModel;
            result.TokensUsed = tokensUsed;
            result.ProcessingTimeMs = sw.ElapsedMilliseconds;
            result.Success = true;

            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new ReceiptOcrResult
            {
                Success = false,
                Error = ex.Message,
                Model = useModel,
                ProcessingTimeMs = sw.ElapsedMilliseconds
            };
        }
    }

    #endregion

    #region Custom Extraction

    /// <summary>
    /// Extracts text using a custom prompt for specialized use cases.
    /// </summary>
    /// <param name="imageBytes">The image data.</param>
    /// <param name="customPrompt">Custom extraction prompt.</param>
    /// <param name="mimeType">MIME type (e.g., "image/png").</param>
    /// <param name="model">Vision model to use, or null for default.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted text result.</returns>
    public async Task<OcrResult> ExtractWithPromptAsync(
        byte[] imageBytes,
        string customPrompt,
        string? mimeType = null,
        string? model = null,
        CancellationToken cancellationToken = default)
    {
        mimeType ??= DetectMimeType(imageBytes);
        var base64 = Convert.ToBase64String(imageBytes);
        var dataUri = $"data:{mimeType};base64,{base64}";

        return await ExtractTextInternalAsync(dataUri, null, model, cancellationToken, customPrompt);
    }

    #endregion

    #region Static Factory Methods

    /// <summary>
    /// Creates a new OCR client from an environment variable.
    /// </summary>
    /// <param name="envVarName">Environment variable name. Defaults to "AIML_API_KEY".</param>
    public static OcrClient FromEnvironment(string envVarName = "AIML_API_KEY")
    {
        var apiKey = Environment.GetEnvironmentVariable(envVarName);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                $"Environment variable '{envVarName}' not set. " +
                "Get your API key at https://aimlapi.com?via=forevertools");
        }
        return new OcrClient(apiKey);
    }

    #endregion

    #region Private Methods

    private static async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_0
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        var bytes = new byte[fs.Length];
        await fs.ReadAsync(bytes, 0, bytes.Length);
        return bytes;
#else
        return await File.ReadAllBytesAsync(path, cancellationToken);
#endif
    }

    private async Task<OcrResult> ExtractTextInternalAsync(
        string? dataUri,
        string? imageUrl,
        string? model,
        CancellationToken cancellationToken,
        string? customPrompt = null)
    {
        var sw = Stopwatch.StartNew();
        var useModel = model ?? _options.DefaultModel;

        var prompt = customPrompt ?? "Extract ALL text from this image. Return only the extracted text, preserving the original layout and formatting as much as possible. Do not add any explanations or commentary.";

        try
        {
            var chatClient = _client.GetChatClient(useModel);
            var messages = CreateVisionMessages(prompt, dataUri, imageUrl);

            var response = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                MaxOutputTokenCount = _options.MaxTokens
            }, cancellationToken);

            sw.Stop();
            var content = response.Value.Content[0].Text;
            var tokensUsed = response.Value.Usage?.TotalTokenCount ?? 0;

            return OcrResult.Successful(content, useModel, tokensUsed, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();
            return OcrResult.Failed(ex.Message, useModel);
        }
    }

    private List<ChatMessage> CreateVisionMessages(string prompt, string? dataUri, string? imageUrl)
    {
        var contentParts = new List<ChatMessageContentPart>
        {
            ChatMessageContentPart.CreateTextPart(prompt)
        };

        if (!string.IsNullOrEmpty(dataUri))
        {
            contentParts.Add(ChatMessageContentPart.CreateImagePart(new Uri(dataUri), _options.ImageDetail));
        }
        else if (!string.IsNullOrEmpty(imageUrl))
        {
            contentParts.Add(ChatMessageContentPart.CreateImagePart(new Uri(imageUrl), _options.ImageDetail));
        }

        return new List<ChatMessage>
        {
            new UserChatMessage(contentParts)
        };
    }

    private static string GetMimeType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".tiff" or ".tif" => "image/tiff",
            _ => "image/png"
        };
    }

    private static string DetectMimeType(byte[] imageBytes)
    {
        if (imageBytes.Length < 8) return "image/png";

        // PNG: 89 50 4E 47
        if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
            return "image/png";

        // JPEG: FF D8 FF
        if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8 && imageBytes[2] == 0xFF)
            return "image/jpeg";

        // GIF: 47 49 46 38
        if (imageBytes[0] == 0x47 && imageBytes[1] == 0x49 && imageBytes[2] == 0x46 && imageBytes[3] == 0x38)
            return "image/gif";

        // WebP: 52 49 46 46 ... 57 45 42 50
        if (imageBytes[0] == 0x52 && imageBytes[1] == 0x49 && imageBytes[2] == 0x46 && imageBytes[3] == 0x46)
            return "image/webp";

        // BMP: 42 4D
        if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
            return "image/bmp";

        return "image/png"; // Default
    }

    private static StructuredOcrResult ParseStructuredResponse(string content)
    {
        var result = new StructuredOcrResult();

        try
        {
            // Clean up the response (remove markdown code blocks if present)
            content = CleanJsonResponse(content);

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("paragraphs", out var paragraphs))
            {
                foreach (var p in paragraphs.EnumerateArray())
                {
                    result.Paragraphs.Add(p.GetString() ?? "");
                }
            }

            if (root.TryGetProperty("lines", out var lines))
            {
                foreach (var l in lines.EnumerateArray())
                {
                    result.Lines.Add(l.GetString() ?? "");
                }
            }

            if (root.TryGetProperty("blocks", out var blocks))
            {
                var order = 0;
                foreach (var b in blocks.EnumerateArray())
                {
                    result.Blocks.Add(new TextBlock
                    {
                        Text = b.GetProperty("text").GetString() ?? "",
                        BlockType = b.TryGetProperty("blockType", out var bt) ? bt.GetString() ?? "paragraph" : "paragraph",
                        Order = b.TryGetProperty("order", out var o) ? o.GetInt32() : order++
                    });
                }
            }

            // Combine for full text
            result.Text = string.Join("\n\n", result.Paragraphs.Count > 0 ? result.Paragraphs : result.Lines);
        }
        catch
        {
            // If JSON parsing fails, treat entire content as plain text
            result.Text = content;
            result.Paragraphs.Add(content);
        }

        return result;
    }

    private static TableOcrResult ParseTableResponse(string content)
    {
        var result = new TableOcrResult();

        try
        {
            content = CleanJsonResponse(content);

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("tables", out var tables))
            {
                foreach (var t in tables.EnumerateArray())
                {
                    var table = new ExtractedTable();

                    if (t.TryGetProperty("headers", out var headers))
                    {
                        foreach (var h in headers.EnumerateArray())
                        {
                            table.Headers.Add(h.GetString() ?? "");
                        }
                    }

                    if (t.TryGetProperty("rows", out var rows))
                    {
                        foreach (var r in rows.EnumerateArray())
                        {
                            var row = new List<string>();
                            foreach (var cell in r.EnumerateArray())
                            {
                                row.Add(cell.GetString() ?? "");
                            }
                            table.Rows.Add(row);
                        }
                    }

                    result.Tables.Add(table);
                }
            }

            result.Text = string.Join("\n\n", result.Tables.Select(t => t.ToCsv()));
        }
        catch
        {
            result.Text = content;
        }

        return result;
    }

    private static FormOcrResult ParseFormResponse(string content)
    {
        var result = new FormOcrResult();

        try
        {
            content = CleanJsonResponse(content);

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("fields", out var fields))
            {
                foreach (var prop in fields.EnumerateObject())
                {
                    result.Fields[prop.Name] = prop.Value.GetString() ?? "";
                }
            }

            result.Text = string.Join("\n", result.Fields.Select(f => $"{f.Key}: {f.Value}"));
        }
        catch
        {
            result.Text = content;
        }

        return result;
    }

    private static ReceiptOcrResult ParseReceiptResponse(string content)
    {
        var result = new ReceiptOcrResult();

        try
        {
            content = CleanJsonResponse(content);

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            result.MerchantName = GetStringOrNull(root, "merchantName");
            result.Date = GetStringOrNull(root, "date");
            result.Total = GetStringOrNull(root, "total");
            result.Subtotal = GetStringOrNull(root, "subtotal");
            result.Tax = GetStringOrNull(root, "tax");
            result.Currency = GetStringOrNull(root, "currency");
            result.PaymentMethod = GetStringOrNull(root, "paymentMethod");

            if (root.TryGetProperty("items", out var items))
            {
                foreach (var item in items.EnumerateArray())
                {
                    result.Items.Add(new ReceiptLineItem
                    {
                        Description = GetStringOrNull(item, "description") ?? "",
                        Quantity = GetStringOrNull(item, "quantity"),
                        UnitPrice = GetStringOrNull(item, "unitPrice"),
                        TotalPrice = GetStringOrNull(item, "totalPrice")
                    });
                }
            }

            var textParts = new List<string>();
            if (result.MerchantName != null) textParts.Add(result.MerchantName);
            if (result.Date != null) textParts.Add($"Date: {result.Date}");
            if (result.Total != null) textParts.Add($"Total: {result.Total}");
            result.Text = string.Join("\n", textParts);
        }
        catch
        {
            result.Text = content;
        }

        return result;
    }

    private static string? GetStringOrNull(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var prop))
        {
            if (prop.ValueKind == JsonValueKind.Null) return null;
            return prop.GetString();
        }
        return null;
    }

    private static string CleanJsonResponse(string content)
    {
        content = content.Trim();

        // Remove markdown code blocks
        if (content.StartsWith("```json"))
        {
            content = content.Substring(7);
        }
        else if (content.StartsWith("```"))
        {
            content = content.Substring(3);
        }

        if (content.EndsWith("```"))
        {
            content = content.Substring(0, content.Length - 3);
        }

        return content.Trim();
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Disposes the client and releases resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    #endregion
}
