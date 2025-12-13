using ForeverTools.Translate;
using ForeverTools.Translate.Extensions;
using ForeverTools.Translate.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ForeverTools.Translate.Tests;

public class TranslationClientTests
{
    private const string TestApiKey = "test-api-key";

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidApiKey_CreatesClient()
    {
        var client = new TranslationClient(TestApiKey);
        Assert.NotNull(client);
        Assert.Equal(TestApiKey, client.Options.ApiKey);
    }

    [Fact]
    public void Constructor_WithOptions_CreatesClient()
    {
        var options = new TranslationOptions
        {
            ApiKey = TestApiKey,
            DefaultModel = TranslationModels.Gpt4oMini,
            DefaultTargetLanguage = "es"
        };

        var client = new TranslationClient(options);

        Assert.NotNull(client);
        Assert.Equal(TestApiKey, client.Options.ApiKey);
        Assert.Equal(TranslationModels.Gpt4oMini, client.Options.DefaultModel);
        Assert.Equal("es", client.Options.DefaultTargetLanguage);
    }

    [Fact]
    public void Constructor_WithNullApiKey_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TranslationClient(string.Empty));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TranslationClient((TranslationOptions)null!));
    }

    [Fact]
    public void Constructor_WithEmptyApiKeyInOptions_ThrowsArgumentException()
    {
        var options = new TranslationOptions { ApiKey = "" };
        Assert.Throws<ArgumentException>(() => new TranslationClient(options));
    }

    #endregion

    #region TranslationOptions Tests

    [Fact]
    public void TranslationOptions_DefaultValues_AreCorrect()
    {
        var options = new TranslationOptions();

        Assert.Equal(string.Empty, options.ApiKey);
        Assert.Equal("https://api.aimlapi.com/v1", options.BaseUrl);
        Assert.Equal(TranslationModels.Gpt4o, options.DefaultModel);
        Assert.Null(options.DefaultSourceLanguage);
        Assert.Equal("en", options.DefaultTargetLanguage);
        Assert.True(options.PreserveFormatting);
        Assert.Equal(TranslationStyle.Natural, options.Style);
        Assert.Equal(5, options.MaxParallelRequests);
        Assert.Equal(60, options.TimeoutSeconds);
    }

    [Fact]
    public void TranslationOptions_CanSetAllProperties()
    {
        var options = new TranslationOptions
        {
            ApiKey = "my-key",
            BaseUrl = "https://custom.api.com",
            DefaultModel = TranslationModels.Claude35Sonnet,
            DefaultSourceLanguage = "en",
            DefaultTargetLanguage = "fr",
            PreserveFormatting = false,
            Style = TranslationStyle.Formal,
            MaxParallelRequests = 10,
            TimeoutSeconds = 120
        };

        Assert.Equal("my-key", options.ApiKey);
        Assert.Equal("https://custom.api.com", options.BaseUrl);
        Assert.Equal(TranslationModels.Claude35Sonnet, options.DefaultModel);
        Assert.Equal("en", options.DefaultSourceLanguage);
        Assert.Equal("fr", options.DefaultTargetLanguage);
        Assert.False(options.PreserveFormatting);
        Assert.Equal(TranslationStyle.Formal, options.Style);
        Assert.Equal(10, options.MaxParallelRequests);
        Assert.Equal(120, options.TimeoutSeconds);
    }

    #endregion

    #region TranslationModels Tests

    [Fact]
    public void TranslationModels_ContainsExpectedOpenAIModels()
    {
        Assert.Equal("gpt-4o", TranslationModels.Gpt4o);
        Assert.Equal("gpt-4o-mini", TranslationModels.Gpt4oMini);
        Assert.Equal("gpt-4-turbo", TranslationModels.Gpt4Turbo);
        Assert.Equal("gpt-4", TranslationModels.Gpt4);
        Assert.Equal("gpt-3.5-turbo", TranslationModels.Gpt35Turbo);
    }

    [Fact]
    public void TranslationModels_ContainsExpectedClaudeModels()
    {
        Assert.Equal("claude-3-5-sonnet-20241022", TranslationModels.Claude35Sonnet);
        Assert.Equal("claude-3-opus-20240229", TranslationModels.Claude3Opus);
        Assert.Equal("claude-3-sonnet-20240229", TranslationModels.Claude3Sonnet);
        Assert.Equal("claude-3-haiku-20240307", TranslationModels.Claude3Haiku);
    }

    [Fact]
    public void TranslationModels_ContainsExpectedGeminiModels()
    {
        Assert.Equal("gemini-1.5-pro", TranslationModels.Gemini15Pro);
        Assert.Equal("gemini-1.5-flash", TranslationModels.Gemini15Flash);
    }

    [Fact]
    public void TranslationModels_ContainsExpectedLlamaModels()
    {
        Assert.Equal("meta-llama/Meta-Llama-3.1-405B-Instruct-Turbo", TranslationModels.Llama31405B);
        Assert.Equal("meta-llama/Meta-Llama-3.1-70B-Instruct-Turbo", TranslationModels.Llama3170B);
        Assert.Equal("meta-llama/Meta-Llama-3.1-8B-Instruct-Turbo", TranslationModels.Llama318B);
    }

    #endregion

    #region TranslationStyle Tests

    [Fact]
    public void TranslationStyle_HasAllExpectedValues()
    {
        var styles = Enum.GetValues<TranslationStyle>();
        Assert.Equal(6, styles.Length);
        Assert.Contains(TranslationStyle.Natural, styles);
        Assert.Contains(TranslationStyle.Formal, styles);
        Assert.Contains(TranslationStyle.Casual, styles);
        Assert.Contains(TranslationStyle.Technical, styles);
        Assert.Contains(TranslationStyle.Literal, styles);
        Assert.Contains(TranslationStyle.Creative, styles);
    }

    #endregion

    #region Language Tests

    [Fact]
    public void Language_Constructor_SetsProperties()
    {
        var lang = new Language("en", "English", "English");

        Assert.Equal("en", lang.Code);
        Assert.Equal("English", lang.Name);
        Assert.Equal("English", lang.NativeName);
    }

    [Fact]
    public void Language_ToString_ReturnsFormattedString()
    {
        var lang = new Language("es", "Spanish", "Español");
        Assert.Equal("Spanish (es)", lang.ToString());
    }

    [Fact]
    public void Language_Equality_WorksCorrectly()
    {
        var lang1 = new Language("en", "English", "English");
        var lang2 = new Language("EN", "English", "English");
        var lang3 = new Language("es", "Spanish", "Español");

        Assert.Equal(lang1, lang2);
        Assert.NotEqual(lang1, lang3);
    }

    [Fact]
    public void Language_FromCode_ReturnsKnownLanguage()
    {
        var lang = Language.FromCode("en");
        Assert.Equal("English", lang.Name);
    }

    [Fact]
    public void Language_FromCode_ReturnsGenericForUnknown()
    {
        var lang = Language.FromCode("xyz");
        Assert.Equal("xyz", lang.Code);
        Assert.Equal("xyz", lang.Name);
    }

    [Fact]
    public void Language_ImplicitConversion_Works()
    {
        Language lang = "en";
        Assert.Equal("en", lang.Code);
        Assert.Equal("English", lang.Name);
    }

    [Fact]
    public void Language_FromCode_ThrowsOnEmpty()
    {
        Assert.Throws<ArgumentException>(() => Language.FromCode(""));
        Assert.Throws<ArgumentException>(() => Language.FromCode("   "));
    }

    #endregion

    #region Languages Static Tests

    [Fact]
    public void Languages_ContainsMajorLanguages()
    {
        Assert.Equal("en", Languages.English.Code);
        Assert.Equal("es", Languages.Spanish.Code);
        Assert.Equal("fr", Languages.French.Code);
        Assert.Equal("de", Languages.German.Code);
        Assert.Equal("ja", Languages.Japanese.Code);
        Assert.Equal("zh", Languages.Chinese.Code);
        Assert.Equal("ar", Languages.Arabic.Code);
        Assert.Equal("ru", Languages.Russian.Code);
    }

    [Fact]
    public void Languages_All_ContainsAllLanguages()
    {
        Assert.True(Languages.All.Count >= 50);
        Assert.True(Languages.All.ContainsKey("en"));
        Assert.True(Languages.All.ContainsKey("es"));
        Assert.True(Languages.All.ContainsKey("zh-cn"));
    }

    [Fact]
    public void Languages_All_IsCaseInsensitive()
    {
        Assert.True(Languages.All.ContainsKey("EN"));
        Assert.True(Languages.All.ContainsKey("En"));
        Assert.True(Languages.All.ContainsKey("eN"));
    }

    [Fact]
    public void Languages_ChineseVariants_AreDistinct()
    {
        Assert.NotEqual(Languages.ChineseSimplified.Code, Languages.ChineseTraditional.Code);
        Assert.Equal("zh-CN", Languages.ChineseSimplified.Code);
        Assert.Equal("zh-TW", Languages.ChineseTraditional.Code);
    }

    #endregion

    #region TranslationResult Tests

    [Fact]
    public void TranslationResult_DefaultValues()
    {
        var result = new TranslationResult();

        Assert.Equal(string.Empty, result.OriginalText);
        Assert.Equal(string.Empty, result.TranslatedText);
        Assert.Equal(string.Empty, result.SourceLanguage);
        Assert.Equal(string.Empty, result.TargetLanguage);
        Assert.Equal(string.Empty, result.Model);
        Assert.False(result.WasLanguageDetected);
        Assert.Null(result.DetectionConfidence);
        Assert.Null(result.Notes);
    }

    [Fact]
    public void TranslationResult_CanSetAllProperties()
    {
        var result = new TranslationResult
        {
            OriginalText = "Hello",
            TranslatedText = "Hola",
            SourceLanguage = "en",
            TargetLanguage = "es",
            Model = TranslationModels.Gpt4o,
            WasLanguageDetected = true,
            DetectionConfidence = 0.95,
            Notes = "High confidence translation"
        };

        Assert.Equal("Hello", result.OriginalText);
        Assert.Equal("Hola", result.TranslatedText);
        Assert.Equal("en", result.SourceLanguage);
        Assert.Equal("es", result.TargetLanguage);
        Assert.Equal(TranslationModels.Gpt4o, result.Model);
        Assert.True(result.WasLanguageDetected);
        Assert.Equal(0.95, result.DetectionConfidence);
        Assert.Equal("High confidence translation", result.Notes);
    }

    #endregion

    #region LanguageDetectionResult Tests

    [Fact]
    public void LanguageDetectionResult_DefaultValues()
    {
        var result = new LanguageDetectionResult();

        Assert.Equal(string.Empty, result.LanguageCode);
        Assert.Equal(string.Empty, result.LanguageName);
        Assert.Equal(0, result.Confidence);
        Assert.Equal(string.Empty, result.AnalyzedText);
        Assert.Null(result.Alternatives);
    }

    [Fact]
    public void LanguageDetectionResult_CanSetAllProperties()
    {
        var alternatives = new List<LanguageAlternative>
        {
            new() { LanguageCode = "pt", LanguageName = "Portuguese", Confidence = 0.1 }
        };

        var result = new LanguageDetectionResult
        {
            LanguageCode = "es",
            LanguageName = "Spanish",
            Confidence = 0.9,
            AnalyzedText = "Hola mundo",
            Alternatives = alternatives
        };

        Assert.Equal("es", result.LanguageCode);
        Assert.Equal("Spanish", result.LanguageName);
        Assert.Equal(0.9, result.Confidence);
        Assert.Equal("Hola mundo", result.AnalyzedText);
        Assert.Single(result.Alternatives!);
    }

    #endregion

    #region BatchTranslationResult Tests

    [Fact]
    public void BatchTranslationResult_DefaultValues()
    {
        var result = new BatchTranslationResult();

        Assert.Empty(result.Results);
        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
        Assert.Equal(0, result.TotalCount);
        Assert.Null(result.Errors);
    }

    [Fact]
    public void BatchTranslationResult_TotalCount_CalculatesCorrectly()
    {
        var result = new BatchTranslationResult
        {
            SuccessCount = 8,
            FailureCount = 2
        };

        Assert.Equal(10, result.TotalCount);
    }

    #endregion

    #region TranslationRequest Tests

    [Fact]
    public void TranslationRequest_DefaultValues()
    {
        var request = new TranslationRequest();

        Assert.Equal(string.Empty, request.Text);
        Assert.Null(request.SourceLanguage);
        Assert.Equal(string.Empty, request.TargetLanguage);
        Assert.Null(request.Context);
        Assert.Null(request.Style);
        Assert.Null(request.Model);
        Assert.Null(request.Glossary);
    }

    [Fact]
    public void TranslationRequest_CanSetAllProperties()
    {
        var glossary = new Dictionary<string, string> { ["hello"] = "hola" };

        var request = new TranslationRequest
        {
            Text = "Hello world",
            SourceLanguage = "en",
            TargetLanguage = "es",
            Context = "Greeting context",
            Style = TranslationStyle.Casual,
            Model = TranslationModels.Claude35Sonnet,
            Glossary = glossary
        };

        Assert.Equal("Hello world", request.Text);
        Assert.Equal("en", request.SourceLanguage);
        Assert.Equal("es", request.TargetLanguage);
        Assert.Equal("Greeting context", request.Context);
        Assert.Equal(TranslationStyle.Casual, request.Style);
        Assert.Equal(TranslationModels.Claude35Sonnet, request.Model);
        Assert.Single(request.Glossary!);
    }

    #endregion

    #region Dependency Injection Tests

    [Fact]
    public void AddForeverToolsTranslation_WithApiKey_RegistersServices()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsTranslation(TestApiKey);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<TranslationOptions>();
        var client = provider.GetRequiredService<TranslationClient>();

        Assert.NotNull(options);
        Assert.NotNull(client);
        Assert.Equal(TestApiKey, options.ApiKey);
    }

    [Fact]
    public void AddForeverToolsTranslation_WithAction_RegistersServices()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsTranslation(options =>
        {
            options.ApiKey = TestApiKey;
            options.DefaultTargetLanguage = "fr";
            options.Style = TranslationStyle.Formal;
        });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<TranslationOptions>();

        Assert.Equal(TestApiKey, options.ApiKey);
        Assert.Equal("fr", options.DefaultTargetLanguage);
        Assert.Equal(TranslationStyle.Formal, options.Style);
    }

    [Fact]
    public void AddForeverToolsTranslation_WithApiKeyAndLanguage_RegistersServices()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsTranslation(TestApiKey, "de");

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<TranslationOptions>();

        Assert.Equal(TestApiKey, options.ApiKey);
        Assert.Equal("de", options.DefaultTargetLanguage);
    }

    [Fact]
    public void AddForeverToolsTranslation_WithApiKeyLanguageAndModel_RegistersServices()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsTranslation(TestApiKey, "ja", TranslationModels.Claude35Sonnet);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<TranslationOptions>();

        Assert.Equal(TestApiKey, options.ApiKey);
        Assert.Equal("ja", options.DefaultTargetLanguage);
        Assert.Equal(TranslationModels.Claude35Sonnet, options.DefaultModel);
    }

    [Fact]
    public void AddForeverToolsTranslation_WithConfiguration_RegistersServices()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Translation:ApiKey"] = TestApiKey,
                ["Translation:DefaultTargetLanguage"] = "ko",
                ["Translation:DefaultModel"] = TranslationModels.Gpt4oMini
            })
            .Build();

        var services = new ServiceCollection();
        services.AddForeverToolsTranslation(config);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<TranslationOptions>();

        Assert.Equal(TestApiKey, options.ApiKey);
        Assert.Equal("ko", options.DefaultTargetLanguage);
        Assert.Equal(TranslationModels.Gpt4oMini, options.DefaultModel);
    }

    [Fact]
    public void AddForeverToolsTranslation_FallsBackToAimlApiSection()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AimlApi:ApiKey"] = TestApiKey
            })
            .Build();

        var services = new ServiceCollection();
        services.AddForeverToolsTranslation(config);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<TranslationOptions>();

        Assert.Equal(TestApiKey, options.ApiKey);
    }

    #endregion

    #region FromEnvironment Tests

    [Fact]
    public void FromEnvironment_WithNoEnvVar_ThrowsInvalidOperationException()
    {
        // Clear any existing environment variables
        Environment.SetEnvironmentVariable("AIML_API_KEY", null);
        Environment.SetEnvironmentVariable("TRANSLATION_API_KEY", null);

        Assert.Throws<InvalidOperationException>(() => TranslationClient.FromEnvironment());
    }

    #endregion
}
