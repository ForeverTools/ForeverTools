using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;

namespace ForeverTools.Mcp.Tools;

[McpServerToolType]
public sealed class OcrTool(IHttpClientFactory httpFactory)
{
    private const string ApiBase = "https://api.aimlapi.com/v1";

    [McpServerTool, Description(
        "Extract text from an image using AI-powered OCR (Optical Character Recognition). " +
        "Supports PNG, JPG, WEBP, GIF. Works on receipts, invoices, screenshots, scanned documents, handwriting. " +
        "Requires AIMLAPI_KEY environment variable (get one free at aimlapi.com/?via=forevertools).")]
    public async Task<string> ExtractTextFromImage(
        [Description("URL of the image to process (must be publicly accessible https:// URL)")] string imageUrl,
        [Description("Extraction mode: 'text' (raw text), 'structured' (preserve layout/tables), 'receipt' (parse receipt data), 'document' (formal document with sections)")] string mode = "text")
    {
        var apiKey = Environment.GetEnvironmentVariable("AIMLAPI_KEY")
            ?? throw new InvalidOperationException("AIMLAPI_KEY environment variable not set. Get a free key at https://aimlapi.com/?via=forevertools");

        if (!imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Image URL must start with https://");

        var modeInstruction = mode.ToLower() switch
        {
            "structured" => "Extract all text from this image, preserving the layout. Use spacing and line breaks to reflect the original structure. If there are tables, format them with | separators.",
            "receipt" => "This is a receipt or invoice. Extract: store name, date, items (name + price), subtotal, tax, total. Format as structured data.",
            "document" => "This is a formal document. Extract all text, preserving headings, paragraphs, and any structured sections. Use markdown formatting.",
            _ => "Extract all text from this image. Return only the text content, exactly as it appears."
        };

        var payload = new
        {
            model = "gpt-4o",
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = modeInstruction },
                        new { type = "image_url", image_url = new { url = imageUrl } }
                    }
                }
            },
            max_tokens = 2048
        };

        using var client = httpFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var response = await client.PostAsJsonAsync($"{ApiBase}/chat/completions", payload);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "(no text detected)";
    }
}
