using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ModelContextProtocol.Server;

namespace ForeverTools.Mcp.Tools;

[McpServerToolType]
public sealed class WebTool(IHttpClientFactory httpFactory)
{
    private const string ApiBase = "https://api.aimlapi.com/v1";

    [McpServerTool, Description(
        "Fetch the text content of any public web page. Strips HTML tags, scripts, and styles, " +
        "returning clean readable text. No API key required. " +
        "Ideal for reading documentation, articles, or any public URL.")]
    public async Task<string> FetchUrlText(
        [Description("The https:// URL of the web page to fetch")] string url,
        [Description("Maximum characters to return (500–20000, default 8000)")] int maxChars = 8000)
    {
        if (!url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("URL must start with http:// or https://");

        maxChars = Math.Clamp(maxChars, 500, 20_000);

        using var client = httpFactory.CreateClient();
        client.DefaultRequestHeaders.Add("User-Agent", "ForeverTools-Mcp/1.2 (+https://github.com/ForeverTools/ForeverTools)");
        client.Timeout = TimeSpan.FromSeconds(15);

        var html = await client.GetStringAsync(url);
        var text = StripHtml(html);

        return text.Length > maxChars ? text[..maxChars] + $"\n\n[Truncated at {maxChars} chars. Use maxChars to increase.]" : text;
    }

    [McpServerTool, Description(
        "Fetch a web page and summarize its content using AI. Combines URL fetching with AI summarization. " +
        "Requires AIMLAPI_KEY environment variable (get one free at aimlapi.com/?via=forevertools).")]
    public async Task<string> SummarizeUrl(
        [Description("The https:// URL of the web page to summarize")] string url,
        [Description("Summary style: 'tldr' (1-2 sentences), 'bullets' (key points), 'executive' (structured), 'keypoints' (numbered takeaways)")] string style = "tldr",
        [Description("Target length in words (50-500, default 150)")] int maxWords = 150)
    {
        var apiKey = Environment.GetEnvironmentVariable("AIMLAPI_KEY")
            ?? throw new InvalidOperationException("AIMLAPI_KEY environment variable not set. Get a free key at https://aimlapi.com/?via=forevertools");

        var text = await FetchUrlText(url, maxChars: 8000);

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

    [McpServerTool, Description(
        "Extract the most important keywords and key phrases from a block of text using AI. " +
        "Returns a ranked list of terms that capture the core topics and concepts. " +
        "Requires AIMLAPI_KEY environment variable (get one free at aimlapi.com/?via=forevertools).")]
    public async Task<string> ExtractKeywords(
        [Description("The text to extract keywords from (up to 10,000 characters)")] string text,
        [Description("Number of keywords/phrases to extract (5–50, default 15)")] int count = 15,
        [Description("Output format: 'list' (one per line), 'json' (structured with relevance scores), 'csv' (comma-separated)")] string format = "list")
    {
        var apiKey = Environment.GetEnvironmentVariable("AIMLAPI_KEY")
            ?? throw new InvalidOperationException("AIMLAPI_KEY environment variable not set. Get a free key at https://aimlapi.com/?via=forevertools");

        count = Math.Clamp(count, 5, 50);
        if (text.Length > 10_000)
            text = text[..10_000];

        var formatInstruction = format.ToLower() switch
        {
            "json" => $"Return a JSON array of objects with 'keyword' and 'relevance' (0.0-1.0) fields. Example: [{{\"keyword\":\"machine learning\",\"relevance\":0.95}}]. Return exactly {count} items.",
            "csv" => $"Return keywords as a single comma-separated line, most relevant first. Return exactly {count} keywords.",
            _ => $"Return one keyword or key phrase per line, most relevant first. Return exactly {count} keywords."
        };

        var systemPrompt = $"You are a keyword extraction specialist. Extract the {count} most important and relevant keywords and key phrases from the provided text. {formatInstruction} Return ONLY the keywords in the requested format, no preamble or explanation.";

        var payload = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = text }
            },
            max_tokens = count * 20
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

    private static string StripHtml(string html)
    {
        // Remove scripts and styles completely
        html = Regex.Replace(html, @"<script[^>]*>[\s\S]*?</script>", " ", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<style[^>]*>[\s\S]*?</style>", " ", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<!--[\s\S]*?-->", " ");

        // Block-level elements → newline
        html = Regex.Replace(html, @"<(p|div|br|h[1-6]|li|tr|blockquote)[^>]*>", "\n", RegexOptions.IgnoreCase);

        // Strip remaining tags
        html = Regex.Replace(html, @"<[^>]+>", " ");

        // Decode common HTML entities
        html = html.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">")
                   .Replace("&quot;", "\"").Replace("&#39;", "'").Replace("&nbsp;", " ")
                   .Replace("&ndash;", "–").Replace("&mdash;", "—").Replace("&hellip;", "…");
        html = Regex.Replace(html, @"&#\d+;", " ");

        // Collapse whitespace
        html = Regex.Replace(html, @"[ \t]+", " ");
        html = Regex.Replace(html, @"\n{3,}", "\n\n");

        return html.Trim();
    }
}
