using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;

namespace ForeverTools.Mcp.Tools;

[McpServerToolType]
public sealed class SummarizeTool(IHttpClientFactory httpFactory)
{
    private const string ApiBase = "https://api.aimlapi.com/v1";

    [McpServerTool, Description(
        "Summarize a block of text using AI. Returns a concise summary in the requested style. " +
        "Requires AIMLAPI_KEY environment variable (get one free at aimlapi.com/?via=forevertools).")]
    public async Task<string> SummarizeText(
        [Description("The text to summarize (up to 10,000 characters)")] string text,
        [Description("Summary style: 'tldr' (1-2 sentences), 'bullets' (key bullet points), 'executive' (structured executive summary), 'keypoints' (numbered key takeaways)")] string style = "tldr",
        [Description("Target length in words (50-500, default 150)")] int maxWords = 150)
    {
        var apiKey = Environment.GetEnvironmentVariable("AIMLAPI_KEY")
            ?? throw new InvalidOperationException("AIMLAPI_KEY environment variable not set. Get a free key at https://aimlapi.com/?via=forevertools");

        if (text.Length > 10_000)
            text = text[..10_000];

        var styleInstruction = style.ToLower() switch
        {
            "bullets" => $"Summarize as concise bullet points (•). Use {maxWords} words or fewer.",
            "executive" => $"Write an executive summary with: 1) Overview, 2) Key Points, 3) Conclusion. Use {maxWords} words or fewer.",
            "keypoints" => $"Extract the top key takeaways as a numbered list. Use {maxWords} words or fewer.",
            _ => $"Write a TL;DR summary in {maxWords} words or fewer."
        };

        var payload = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = styleInstruction },
                new { role = "user", content = text }
            },
            max_tokens = maxWords * 2
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
}
