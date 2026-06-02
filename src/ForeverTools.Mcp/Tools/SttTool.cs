using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace ForeverTools.Mcp.Tools;

[McpServerToolType]
public sealed class SttTool(IHttpClientFactory httpFactory)
{
    private const string ApiBase = "https://api.aimlapi.com/v1";

    [McpServerTool, Description(
        "Transcribe speech from an audio file URL to text using Whisper AI. " +
        "Supports MP3, WAV, M4A, OGG, FLAC, WEBM (max 25 MB). " +
        "Requires AIMLAPI_KEY environment variable (get one free at aimlapi.com/?via=forevertools).")]
    public async Task<string> TranscribeAudio(
        [Description("Publicly accessible https:// URL of the audio file to transcribe")] string audioUrl,
        [Description("Output format: 'text' (plain transcript), 'json' (with timestamps), 'srt' (subtitle format), 'vtt' (WebVTT format)")] string format = "text",
        [Description("Language hint (ISO-639-1 code, e.g. 'en', 'es', 'fr'). Leave empty for auto-detection.")] string language = "")
    {
        var apiKey = Environment.GetEnvironmentVariable("AIMLAPI_KEY")
            ?? throw new InvalidOperationException("AIMLAPI_KEY environment variable not set. Get a free key at https://aimlapi.com/?via=forevertools");

        if (!audioUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Audio URL must start with https://");

        using var client = httpFactory.CreateClient();

        // Download the audio file
        var audioBytes = await client.GetByteArrayAsync(audioUrl);
        if (audioBytes.Length > 25 * 1024 * 1024)
            throw new InvalidOperationException("Audio file exceeds 25 MB limit");

        // Determine filename/content-type from URL
        var fileName = Path.GetFileName(new Uri(audioUrl).LocalPath);
        if (string.IsNullOrEmpty(fileName)) fileName = "audio.mp3";

        var responseFormat = format.ToLower() switch
        {
            "srt" => "srt",
            "vtt" => "vtt",
            "json" => "verbose_json",
            _ => "text"
        };

        using var content = new MultipartFormDataContent();
        var audioContent = new ByteArrayContent(audioBytes);
        audioContent.Headers.ContentType = new MediaTypeHeaderValue(GetMimeType(fileName));
        content.Add(audioContent, "file", fileName);
        content.Add(new StringContent("whisper-1"), "model");
        content.Add(new StringContent(responseFormat), "response_format");
        if (!string.IsNullOrEmpty(language))
            content.Add(new StringContent(language), "language");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        var response = await client.PostAsync($"{ApiBase}/audio/transcriptions", content);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        // For verbose_json, pretty-print; for others return as-is
        if (responseFormat == "verbose_json")
        {
            using var doc = JsonDocument.Parse(body);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }

        return body.Trim();
    }

    private static string GetMimeType(string fileName) =>
        Path.GetExtension(fileName).ToLower() switch
        {
            ".wav" => "audio/wav",
            ".m4a" => "audio/mp4",
            ".ogg" => "audio/ogg",
            ".flac" => "audio/flac",
            ".webm" => "audio/webm",
            _ => "audio/mpeg"
        };
}
