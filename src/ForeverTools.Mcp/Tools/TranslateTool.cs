using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;

namespace ForeverTools.Mcp.Tools;

[McpServerToolType]
public sealed class TranslateTool(IHttpClientFactory httpFactory)
{
    private const string ApiBase = "https://api.aimlapi.com/v1";

    [McpServerTool, Description(
        "Translate text into any language using AI. Supports 100+ languages. " +
        "Requires AIMLAPI_KEY environment variable (get one free at aimlapi.com/?via=forevertools).")]
    public async Task<string> TranslateText(
        [Description("The text to translate")] string text,
        [Description("Target language (e.g. 'Spanish', 'French', 'Japanese', 'Arabic', 'zh-CN')")] string targetLanguage,
        [Description("Source language (optional, auto-detected if omitted)")] string? sourceLanguage = null,
        [Description("Translation style: 'formal', 'casual', 'technical' (default: 'formal')")] string style = "formal")
    {
        var apiKey = Environment.GetEnvironmentVariable("AIMLAPI_KEY")
            ?? throw new InvalidOperationException("AIMLAPI_KEY environment variable not set. Get a free key at https://aimlapi.com/?via=forevertools");

        var from = sourceLanguage != null ? $"from {sourceLanguage} " : "";
        var systemPrompt = $"You are a professional translator. Translate the following text {from}to {targetLanguage}. " +
            $"Use a {style} register. Return ONLY the translated text, no explanations.";

        var payload = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = text }
            },
            max_tokens = text.Length * 3
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
            .GetString() ?? "(empty response)";
    }

    [McpServerTool, Description("List all supported languages for translation.")]
    public Task<string> ListSupportedLanguages()
    {
        return Task.FromResult("""
            Supported languages include (but are not limited to):

            European: English, Spanish, French, German, Italian, Portuguese, Dutch, Polish,
            Russian, Swedish, Norwegian, Danish, Finnish, Greek, Czech, Romanian, Hungarian

            Asian: Japanese, Chinese (Simplified/Traditional), Korean, Arabic, Hindi, Bengali,
            Thai, Vietnamese, Indonesian, Malay, Tagalog, Urdu, Persian (Farsi)

            African: Swahili, Amharic, Yoruba, Zulu, Afrikaans, Hausa, Igbo

            Other: Hebrew, Turkish, Ukrainian, Catalan, Croatian, Slovak, Slovenian,
            Bulgarian, Serbian, Lithuanian, Latvian, Estonian

            For best results, use the full language name (e.g. 'Spanish') or ISO 639-1 code
            (e.g. 'es'). Chinese variants: 'zh-CN' (Simplified) or 'zh-TW' (Traditional).
            """);
    }
}
