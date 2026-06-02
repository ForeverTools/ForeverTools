using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace ForeverTools.Mcp.Tools;

[McpServerToolType]
public sealed class ImageGenTool(IHttpClientFactory httpFactory)
{
    private const string ApiBase = "https://api.aimlapi.com/v1";

    [McpServerTool, Description(
        "Generate an image from a text prompt using AI (DALL-E 3, Flux, Stable Diffusion). " +
        "Returns the URL(s) of the generated image(s). " +
        "Requires AIMLAPI_KEY environment variable (get one free at aimlapi.com/?via=forevertools).")]
    public async Task<string> GenerateImage(
        [Description("Detailed description of the image to generate. More detail = better results.")] string prompt,
        [Description("Model to use: 'dall-e-3' (highest quality, photorealistic), 'flux-schnell' (fast, good quality), 'flux-pro' (best Flux quality), 'stable-diffusion-xl' (artistic)")] string model = "dall-e-3",
        [Description("Image size: '1024x1024' (square), '1792x1024' (landscape), '1024x1792' (portrait). DALL-E 3 supports all; Flux/SD default to 1024x1024.")] string size = "1024x1024",
        [Description("Style hint for DALL-E 3: 'vivid' (hyper-real, dramatic) or 'natural' (more subtle, natural look)")] string style = "vivid",
        [Description("Number of images to generate (1-4; DALL-E 3 only supports 1)")] int count = 1)
    {
        var apiKey = Environment.GetEnvironmentVariable("AIMLAPI_KEY")
            ?? throw new InvalidOperationException("AIMLAPI_KEY environment variable not set. Get a free key at https://aimlapi.com/?via=forevertools");

        count = Math.Clamp(count, 1, 4);
        var modelId = model.ToLower() switch
        {
            "flux-schnell" => "flux-schnell",
            "flux-pro" => "flux-pro",
            "stable-diffusion-xl" or "sdxl" => "stable-diffusion-xl-1024-v1-0",
            _ => "dall-e-3"
        };

        // DALL-E 3 only supports n=1
        if (modelId == "dall-e-3") count = 1;

        object payload = modelId == "dall-e-3"
            ? new { model = modelId, prompt, n = count, size, style, response_format = "url" }
            : new { model = modelId, prompt, n = count, size };

        using var client = httpFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var response = await client.PostAsJsonAsync($"{ApiBase}/images/generations", payload);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var dataArray = doc.RootElement.GetProperty("data");

        var urls = new List<string>();
        foreach (var item in dataArray.EnumerateArray())
        {
            if (item.TryGetProperty("url", out var urlProp))
                urls.Add(urlProp.GetString() ?? "");
            else if (item.TryGetProperty("b64_json", out _))
                urls.Add("[base64 image data — use response_format=url for URLs]");
        }

        if (urls.Count == 0)
            return "No images returned from API.";

        return urls.Count == 1
            ? $"Generated image URL:\n{urls[0]}"
            : $"Generated {urls.Count} images:\n" + string.Join("\n", urls.Select((u, i) => $"{i + 1}. {u}"));
    }
}
