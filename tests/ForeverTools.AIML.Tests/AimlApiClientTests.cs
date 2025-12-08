using ForeverTools.AIML;
using Xunit;

namespace ForeverTools.AIML.Tests;

public class AimlApiClientTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidApiKey_CreatesClient()
    {
        // Arrange & Act
        var client = new AimlApiClient("test-api-key");

        // Assert
        Assert.NotNull(client);
        Assert.NotNull(client.UnderlyingClient);
        Assert.Equal("test-api-key", client.Options.ApiKey);
    }

    [Fact]
    public void Constructor_WithOptions_UsesProvidedOptions()
    {
        // Arrange
        var options = new AimlApiOptions
        {
            ApiKey = "test-key",
            DefaultChatModel = AimlModels.Chat.Claude35Sonnet,
            DefaultImageModel = AimlModels.Image.FluxPro
        };

        // Act
        var client = new AimlApiClient(options);

        // Assert
        Assert.Equal("test-key", client.Options.ApiKey);
        Assert.Equal(AimlModels.Chat.Claude35Sonnet, client.Options.DefaultChatModel);
        Assert.Equal(AimlModels.Image.FluxPro, client.Options.DefaultImageModel);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidApiKey_ThrowsArgumentException(string? invalidKey)
    {
        // Arrange
        var options = new AimlApiOptions { ApiKey = invalidKey! };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new AimlApiClient(options));
        Assert.Contains("API key is required", exception.Message);
        Assert.Contains("aimlapi.com", exception.Message); // Should include signup link
    }

    #endregion

    #region Options Tests

    [Fact]
    public void AimlApiOptions_HasCorrectDefaults()
    {
        // Arrange & Act
        var options = new AimlApiOptions();

        // Assert
        Assert.Equal(AimlApiOptions.DefaultBaseUrl, options.BaseUrl);
        Assert.Equal("https://api.aimlapi.com/v1", options.BaseUrl);
        Assert.Equal(AimlModels.Chat.Gpt4o, options.DefaultChatModel);
        Assert.Equal(AimlModels.Image.DallE3, options.DefaultImageModel);
        Assert.Equal(AimlModels.Embedding.TextEmbedding3Small, options.DefaultEmbeddingModel);
        Assert.Equal(string.Empty, options.ApiKey);
    }

    [Fact]
    public void AimlApiOptions_SectionName_IsCorrect()
    {
        Assert.Equal("AimlApi", AimlApiOptions.SectionName);
    }

    #endregion

    #region Model Constants Tests

    [Fact]
    public void AimlModels_Chat_ContainsExpectedModels()
    {
        // Verify key models are present and have correct values
        Assert.Equal("gpt-4o", AimlModels.Chat.Gpt4o);
        Assert.Equal("gpt-4o-mini", AimlModels.Chat.Gpt4oMini);
        Assert.Equal("claude-3-5-sonnet-20241022", AimlModels.Chat.Claude35Sonnet);
        Assert.Equal("gemini-1.5-pro", AimlModels.Chat.Gemini15Pro);
        Assert.Equal("deepseek-r1", AimlModels.Chat.DeepSeekR1);
    }

    [Fact]
    public void AimlModels_Image_ContainsExpectedModels()
    {
        Assert.Equal("dall-e-3", AimlModels.Image.DallE3);
        Assert.Equal("flux-pro", AimlModels.Image.FluxPro);
        Assert.Equal("stable-diffusion-xl-1024-v1-0", AimlModels.Image.StableDiffusionXL);
    }

    [Fact]
    public void AimlModels_Embedding_ContainsExpectedModels()
    {
        Assert.Equal("text-embedding-3-large", AimlModels.Embedding.TextEmbedding3Large);
        Assert.Equal("text-embedding-3-small", AimlModels.Embedding.TextEmbedding3Small);
        Assert.Equal("text-embedding-ada-002", AimlModels.Embedding.TextEmbeddingAda002);
    }

    [Fact]
    public void AimlModels_Audio_ContainsExpectedModels()
    {
        Assert.Equal("tts-1", AimlModels.Audio.Tts1);
        Assert.Equal("whisper-1", AimlModels.Audio.Whisper1);
    }

    #endregion

    #region Client Factory Tests

    [Fact]
    public void GetChatClient_ReturnsClientWithDefaultModel()
    {
        // Arrange
        var client = new AimlApiClient("test-key");

        // Act
        var chatClient = client.GetChatClient();

        // Assert
        Assert.NotNull(chatClient);
    }

    [Fact]
    public void GetChatClient_WithSpecificModel_ReturnsClient()
    {
        // Arrange
        var client = new AimlApiClient("test-key");

        // Act
        var chatClient = client.GetChatClient(AimlModels.Chat.Claude35Sonnet);

        // Assert
        Assert.NotNull(chatClient);
    }

    [Fact]
    public void GetEmbeddingClient_ReturnsClient()
    {
        // Arrange
        var client = new AimlApiClient("test-key");

        // Act
        var embeddingClient = client.GetEmbeddingClient();

        // Assert
        Assert.NotNull(embeddingClient);
    }

    [Fact]
    public void GetImageClient_ReturnsClient()
    {
        // Arrange
        var client = new AimlApiClient("test-key");

        // Act
        var imageClient = client.GetImageClient();

        // Assert
        Assert.NotNull(imageClient);
    }

    #endregion

    #region Static Factory Tests

    [Fact]
    public void CreateChatClient_ReturnsClient()
    {
        // Act
        var chatClient = AimlApiClient.CreateChatClient("test-key");

        // Assert
        Assert.NotNull(chatClient);
    }

    [Fact]
    public void CreateChatClient_WithModel_ReturnsClient()
    {
        // Act
        var chatClient = AimlApiClient.CreateChatClient("test-key", AimlModels.Chat.Gpt4oMini);

        // Assert
        Assert.NotNull(chatClient);
    }

    [Fact]
    public void CreateEmbeddingClient_ReturnsClient()
    {
        // Act
        var embeddingClient = AimlApiClient.CreateEmbeddingClient("test-key");

        // Assert
        Assert.NotNull(embeddingClient);
    }

    [Fact]
    public void CreateImageClient_ReturnsClient()
    {
        // Act
        var imageClient = AimlApiClient.CreateImageClient("test-key");

        // Assert
        Assert.NotNull(imageClient);
    }

    [Fact]
    public void FromEnvironment_WithMissingVariable_ThrowsInvalidOperationException()
    {
        // Arrange - ensure variable doesn't exist
        Environment.SetEnvironmentVariable("AIML_API_KEY_TEST_MISSING", null);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(
            () => AimlApiClient.FromEnvironment("AIML_API_KEY_TEST_MISSING"));
        Assert.Contains("not set", exception.Message);
        Assert.Contains("aimlapi.com", exception.Message); // Should include signup link
    }

    [Fact]
    public void FromEnvironment_WithSetVariable_CreatesClient()
    {
        // Arrange
        const string testKey = "test-api-key-from-env";
        const string envVar = "AIML_API_KEY_TEST";
        Environment.SetEnvironmentVariable(envVar, testKey);

        try
        {
            // Act
            var client = AimlApiClient.FromEnvironment(envVar);

            // Assert
            Assert.NotNull(client);
            Assert.Equal(testKey, client.Options.ApiKey);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    #endregion
}
