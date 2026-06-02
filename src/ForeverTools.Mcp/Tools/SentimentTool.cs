using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;

namespace ForeverTools.Mcp.Tools;

[McpServerToolType]
public sealed class SentimentTool(IHttpClientFactory httpFactory)
{
    private const string ApiBase = "https://api.aimlapi.com/v1";

    [McpServerTool, Description(
        "Analyze the sentiment of text. Returns sentiment (positive/negative/neutral), " +
        "confidence score (0-1), detected emotions, and a brief explanation. " +
        "Requires AIMLAPI_KEY environment variable (get one free at aimlapi.com/?via=forevertools).")]
    public async Task<string> AnalyzeSentiment(
        [Description("The text to analyze (reviews, comments, feedback, social posts, etc.)")] string text,
        [Description("If true, include emotion breakdown (joy, anger, sadness, fear, surprise, disgust)")] bool includeEmotions = true)
    {
        var apiKey = Environment.GetEnvironmentVariable("AIMLAPI_KEY")
            ?? throw new InvalidOperationException("AIMLAPI_KEY environment variable not set. Get a free key at https://aimlapi.com/?via=forevertools");

        var emotionInstructions = includeEmotions
            ? "Also detect the top 2-3 emotions present (from: joy, anger, sadness, fear, surprise, disgust, neutral) with confidence scores."
            : "";

        var systemPrompt = $"""
            Analyze the sentiment of the text. Return a JSON object with:
            - "sentiment": "positive", "negative", or "neutral"
            - "confidence": number 0.0-1.0
            - "explanation": one sentence explaining why
            {(includeEmotions ? "- \"emotions\": array of {{\"name\": string, \"score\": number}} for top 2-3 emotions detected" : "")}
            Return ONLY valid JSON, no markdown.
            """;

        var payload = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = text }
            },
            max_tokens = 200,
            response_format = new { type = "json_object" }
        };

        using var client = httpFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var response = await client.PostAsJsonAsync($"{ApiBase}/chat/completions", payload);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var raw = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "{}";

        // Pretty-print the JSON result
        using var parsed = JsonDocument.Parse(raw);
        return JsonSerializer.Serialize(parsed, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description(
        "Analyze sentiment for multiple texts in batch. Returns a summary table with sentiment and confidence for each item. " +
        "Requires AIMLAPI_KEY environment variable.")]
    public async Task<string> AnalyzeSentimentBatch(
        [Description("List of texts to analyze (max 20)")] IEnumerable<string> texts)
    {
        var list = texts.Take(20).ToList();
        var results = new List<string>();

        for (int i = 0; i < list.Count; i++)
        {
            var result = await AnalyzeSentiment(list[i], false);
            using var doc = JsonDocument.Parse(result);
            var sentiment = doc.RootElement.GetProperty("sentiment").GetString() ?? "unknown";
            var confidence = doc.RootElement.GetProperty("confidence").GetDouble();
            results.Add($"{i + 1}. [{sentiment.ToUpper()} {confidence:P0}] {list[i][..Math.Min(60, list[i].Length)]}...");
        }

        return string.Join("\n", results);
    }
}
