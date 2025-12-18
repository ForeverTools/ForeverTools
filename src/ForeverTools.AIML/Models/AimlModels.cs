namespace ForeverTools.AIML;

/// <summary>
/// Constants for available AI/ML API models.
/// Full list at: https://docs.aimlapi.com/api-references/model-database
/// Get your API key at: https://aimlapi.com?via=forevertools
/// </summary>
public static class AimlModels
{
    /// <summary>
    /// Chat and text completion models.
    /// </summary>
    public static class Chat
    {
        // OpenAI Models
        public const string Gpt4o = "gpt-4o";
        public const string Gpt4oMini = "gpt-4o-mini";
        public const string Gpt4Turbo = "gpt-4-turbo";
        public const string Gpt4 = "gpt-4";
        public const string Gpt35Turbo = "gpt-3.5-turbo";
        public const string O1Preview = "o1-preview";
        public const string O1Mini = "o1-mini";

        // Anthropic Claude Models
        public const string Claude3Opus = "claude-3-opus-20240229";
        public const string Claude3Sonnet = "claude-3-sonnet-20240229";
        public const string Claude3Haiku = "claude-3-haiku-20240307";
        public const string Claude35Sonnet = "claude-3-5-sonnet-20241022";

        // Google Gemini Models
        public const string GeminiPro = "gemini-pro";
        public const string Gemini15Pro = "gemini-1.5-pro";
        public const string Gemini15Flash = "gemini-1.5-flash";

        // Meta Llama Models
        public const string Llama3170B = "meta-llama/Meta-Llama-3.1-70B-Instruct-Turbo";
        public const string Llama318B = "meta-llama/Meta-Llama-3.1-8B-Instruct-Turbo";
        public const string Llama31405B = "meta-llama/Meta-Llama-3.1-405B-Instruct-Turbo";
        public const string Llama370B = "meta-llama/Llama-3-70b-chat-hf";

        // Mistral Models
        public const string MistralLarge = "mistral-large-latest";
        public const string MistralMedium = "mistral-medium-latest";
        public const string MistralSmall = "mistral-small-latest";
        public const string Mixtral8x7B = "mistralai/Mixtral-8x7B-Instruct-v0.1";

        // DeepSeek Models
        public const string DeepSeekChat = "deepseek-chat";
        public const string DeepSeekCoder = "deepseek-coder";
        public const string DeepSeekR1 = "deepseek-r1";

        // Qwen Models
        public const string Qwen72B = "Qwen/Qwen2-72B-Instruct";
        public const string Qwen7B = "Qwen/Qwen2-7B-Instruct";

        // Other Notable Models
        public const string Yi34B = "zero-one-ai/Yi-34B-Chat";
        public const string Dolphin26 = "cognitivecomputations/dolphin-2.6-mixtral-8x7b";
    }

    /// <summary>
    /// Image generation models.
    /// </summary>
    public static class Image
    {
        // OpenAI DALL-E
        public const string DallE3 = "dall-e-3";
        public const string DallE2 = "dall-e-2";

        // Stability AI
        public const string StableDiffusionXL = "stable-diffusion-xl-1024-v1-0";
        public const string StableDiffusion3 = "stable-diffusion-v3-medium";

        // Flux Models
        public const string FluxPro = "flux-pro";
        public const string FluxDev = "flux-dev";
        public const string FluxSchnell = "flux-schnell";

        // Midjourney-style
        public const string Playground25 = "playground-v2.5-1024px-aesthetic";
    }

    /// <summary>
    /// Embedding models for vector representations.
    /// </summary>
    public static class Embedding
    {
        public const string TextEmbedding3Large = "text-embedding-3-large";
        public const string TextEmbedding3Small = "text-embedding-3-small";
        public const string TextEmbeddingAda002 = "text-embedding-ada-002";
    }

    /// <summary>
    /// Audio and speech models.
    /// </summary>
    public static class Audio
    {
        // Text-to-Speech
        public const string Tts1 = "tts-1";
        public const string Tts1Hd = "tts-1-hd";
        public const string ElevenLabsMultilingual = "eleven_multilingual_v2";

        // Speech-to-Text
        public const string Whisper1 = "whisper-1";
    }

    /// <summary>
    /// Video generation models.
    /// </summary>
    public static class Video
    {
        public const string Sora = "sora";
        public const string RunwayGen3 = "runway-gen3";
        public const string KlingAi = "kling-ai";
    }

    /// <summary>
    /// Code generation models.
    /// </summary>
    public static class Code
    {
        public const string CodeLlama70B = "codellama/CodeLlama-70b-Instruct-hf";
        public const string CodeLlama34B = "codellama/CodeLlama-34b-Instruct-hf";
        public const string DeepSeekCoder33B = "deepseek-ai/deepseek-coder-33b-instruct";
        public const string StarCoder2 = "bigcode/starcoder2-15b";
    }
}
