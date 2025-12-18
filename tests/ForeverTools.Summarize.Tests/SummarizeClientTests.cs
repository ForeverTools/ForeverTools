using ForeverTools.Summarize;
using ForeverTools.Summarize.Extensions;
using ForeverTools.Summarize.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ForeverTools.Summarize.Tests;

public class SummarizeClientTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithApiKey_CreatesClient()
    {
        var client = new SummarizeClient("test-api-key");
        Assert.NotNull(client);
        Assert.NotNull(client.Options);
        Assert.Equal("test-api-key", client.Options.ApiKey);
    }

    [Fact]
    public void Constructor_WithOptions_CreatesClient()
    {
        var options = new SummarizeOptions
        {
            ApiKey = "test-api-key",
            DefaultModel = SummarizeModels.Claude35Sonnet,
            DefaultStyle = SummaryStyle.BulletPoints,
            DefaultLength = SummaryLength.Short
        };

        var client = new SummarizeClient(options);

        Assert.NotNull(client);
        Assert.Equal(SummarizeModels.Claude35Sonnet, client.Options.DefaultModel);
        Assert.Equal(SummaryStyle.BulletPoints, client.Options.DefaultStyle);
        Assert.Equal(SummaryLength.Short, client.Options.DefaultLength);
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SummarizeClient((SummarizeOptions)null!));
    }

    [Fact]
    public void Constructor_WithEmptyApiKey_ThrowsArgumentException()
    {
        var options = new SummarizeOptions { ApiKey = "" };
        Assert.Throws<ArgumentException>(() => new SummarizeClient(options));
    }

    [Fact]
    public void Constructor_WithWhitespaceApiKey_ThrowsArgumentException()
    {
        var options = new SummarizeOptions { ApiKey = "   " };
        Assert.Throws<ArgumentException>(() => new SummarizeClient(options));
    }

    #endregion

    #region Options Tests

    [Fact]
    public void SummarizeOptions_HasCorrectDefaults()
    {
        var options = new SummarizeOptions();

        Assert.Equal(string.Empty, options.ApiKey);
        Assert.Equal("https://api.aimlapi.com/v1", options.BaseUrl);
        Assert.Equal(SummarizeModels.Gpt4o, options.DefaultModel);
        Assert.Equal(SummaryStyle.Paragraph, options.DefaultStyle);
        Assert.Equal(SummaryLength.Medium, options.DefaultLength);
        Assert.Equal(5, options.MaxParallelRequests);
        Assert.Equal(120, options.TimeoutSeconds);
        Assert.False(options.PreserveQuotes);
        Assert.True(options.IncludeStatistics);
    }

    [Fact]
    public void SummarizeOptions_SectionName_IsCorrect()
    {
        Assert.Equal("Summarize", SummarizeOptions.SectionName);
    }

    #endregion

    #region SummaryStyle Tests

    [Theory]
    [InlineData(SummaryStyle.Paragraph)]
    [InlineData(SummaryStyle.BulletPoints)]
    [InlineData(SummaryStyle.NumberedList)]
    [InlineData(SummaryStyle.Executive)]
    [InlineData(SummaryStyle.Abstract)]
    [InlineData(SummaryStyle.TLDR)]
    [InlineData(SummaryStyle.Structured)]
    [InlineData(SummaryStyle.Headline)]
    [InlineData(SummaryStyle.QAndA)]
    public void SummaryStyle_AllValuesValid(SummaryStyle style)
    {
        Assert.True(Enum.IsDefined(typeof(SummaryStyle), style));
    }

    [Fact]
    public void SummaryStyle_HasExpectedCount()
    {
        var values = Enum.GetValues(typeof(SummaryStyle));
        Assert.Equal(9, values.Length);
    }

    #endregion

    #region SummaryLength Tests

    [Theory]
    [InlineData(SummaryLength.VeryShort)]
    [InlineData(SummaryLength.Short)]
    [InlineData(SummaryLength.Medium)]
    [InlineData(SummaryLength.Long)]
    [InlineData(SummaryLength.Detailed)]
    [InlineData(SummaryLength.Custom)]
    public void SummaryLength_AllValuesValid(SummaryLength length)
    {
        Assert.True(Enum.IsDefined(typeof(SummaryLength), length));
    }

    [Fact]
    public void SummaryLength_HasExpectedCount()
    {
        var values = Enum.GetValues(typeof(SummaryLength));
        Assert.Equal(6, values.Length);
    }

    #endregion

    #region ContentDomain Tests

    [Theory]
    [InlineData(ContentDomain.General)]
    [InlineData(ContentDomain.News)]
    [InlineData(ContentDomain.Academic)]
    [InlineData(ContentDomain.Legal)]
    [InlineData(ContentDomain.Technical)]
    [InlineData(ContentDomain.Business)]
    [InlineData(ContentDomain.Medical)]
    [InlineData(ContentDomain.Financial)]
    [InlineData(ContentDomain.Meeting)]
    [InlineData(ContentDomain.Book)]
    [InlineData(ContentDomain.Email)]
    [InlineData(ContentDomain.Social)]
    public void ContentDomain_AllValuesValid(ContentDomain domain)
    {
        Assert.True(Enum.IsDefined(typeof(ContentDomain), domain));
    }

    [Fact]
    public void ContentDomain_HasExpectedCount()
    {
        var values = Enum.GetValues(typeof(ContentDomain));
        Assert.Equal(12, values.Length);
    }

    #endregion

    #region Model Constants Tests

    [Fact]
    public void SummarizeModels_HasOpenAIModels()
    {
        Assert.Equal("gpt-4o", SummarizeModels.Gpt4o);
        Assert.Equal("gpt-4o-mini", SummarizeModels.Gpt4oMini);
        Assert.Equal("gpt-4-turbo", SummarizeModels.Gpt4Turbo);
        Assert.Equal("gpt-4", SummarizeModels.Gpt4);
        Assert.Equal("gpt-3.5-turbo", SummarizeModels.Gpt35Turbo);
    }

    [Fact]
    public void SummarizeModels_HasClaudeModels()
    {
        Assert.Equal("claude-3-5-sonnet-20241022", SummarizeModels.Claude35Sonnet);
        Assert.Equal("claude-3-opus-20240229", SummarizeModels.Claude3Opus);
        Assert.Equal("claude-3-sonnet-20240229", SummarizeModels.Claude3Sonnet);
        Assert.Equal("claude-3-haiku-20240307", SummarizeModels.Claude3Haiku);
    }

    [Fact]
    public void SummarizeModels_HasGeminiModels()
    {
        Assert.Equal("gemini-1.5-pro", SummarizeModels.Gemini15Pro);
        Assert.Equal("gemini-1.5-flash", SummarizeModels.Gemini15Flash);
    }

    [Fact]
    public void SummarizeModels_HasLlamaModels()
    {
        Assert.Equal("meta-llama/Meta-Llama-3.1-405B-Instruct-Turbo", SummarizeModels.Llama31405B);
        Assert.Equal("meta-llama/Meta-Llama-3.1-70B-Instruct-Turbo", SummarizeModels.Llama3170B);
        Assert.Equal("meta-llama/Meta-Llama-3.1-8B-Instruct-Turbo", SummarizeModels.Llama318B);
    }

    [Fact]
    public void SummarizeModels_HasMistralModels()
    {
        Assert.Equal("mistral-large-latest", SummarizeModels.MistralLarge);
        Assert.Equal("mistralai/Mixtral-8x7B-Instruct-v0.1", SummarizeModels.Mixtral8x7B);
    }

    [Fact]
    public void SummarizeModels_HasQwenModels()
    {
        Assert.Equal("Qwen/Qwen2-72B-Instruct", SummarizeModels.Qwen72B);
    }

    #endregion

    #region SummaryResult Tests

    [Fact]
    public void SummaryResult_HasCorrectDefaults()
    {
        var result = new SummaryResult();

        Assert.Equal(string.Empty, result.Summary);
        Assert.Equal(string.Empty, result.OriginalText);
        Assert.Equal(string.Empty, result.Model);
        Assert.Equal(0, result.OriginalWordCount);
        Assert.Equal(0, result.SummaryWordCount);
        Assert.Null(result.KeyPoints);
        Assert.Null(result.Statistics);
        Assert.Null(result.DetectedDomain);
        Assert.Null(result.DetectedLanguage);
    }

    [Fact]
    public void SummaryResult_CompressionRatio_CalculatesCorrectly()
    {
        var result = new SummaryResult
        {
            OriginalWordCount = 1000,
            SummaryWordCount = 100
        };

        Assert.Equal(0.1, result.CompressionRatio);
    }

    [Fact]
    public void SummaryResult_CompressionRatio_HandlesZeroOriginal()
    {
        var result = new SummaryResult
        {
            OriginalWordCount = 0,
            SummaryWordCount = 100
        };

        Assert.Equal(0, result.CompressionRatio);
    }

    [Fact]
    public void SummaryResult_ReductionPercentage_CalculatesCorrectly()
    {
        var result = new SummaryResult
        {
            OriginalWordCount = 1000,
            SummaryWordCount = 100
        };

        Assert.Equal(90.0, result.ReductionPercentage);
    }

    [Fact]
    public void SummaryResult_ReductionPercentage_HandlesZeroOriginal()
    {
        var result = new SummaryResult
        {
            OriginalWordCount = 0,
            SummaryWordCount = 100
        };

        Assert.Equal(0, result.ReductionPercentage);
    }

    #endregion

    #region KeyPointsResult Tests

    [Fact]
    public void KeyPointsResult_HasCorrectDefaults()
    {
        var result = new KeyPointsResult();

        Assert.NotNull(result.KeyPoints);
        Assert.Empty(result.KeyPoints);
        Assert.Equal(string.Empty, result.OriginalText);
        Assert.Equal(string.Empty, result.Model);
        Assert.Null(result.Themes);
    }

    #endregion

    #region BatchSummaryResult Tests

    [Fact]
    public void BatchSummaryResult_HasCorrectDefaults()
    {
        var result = new BatchSummaryResult();

        Assert.NotNull(result.Results);
        Assert.Empty(result.Results);
        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);
        Assert.Null(result.Errors);
    }

    [Fact]
    public void BatchSummaryResult_TotalCount_CalculatesCorrectly()
    {
        var result = new BatchSummaryResult
        {
            SuccessCount = 8,
            FailureCount = 2
        };

        Assert.Equal(10, result.TotalCount);
    }

    [Fact]
    public void BatchSummaryResult_TotalWords_CalculatesCorrectly()
    {
        var result = new BatchSummaryResult
        {
            Results = new List<SummaryResult>
            {
                new SummaryResult { OriginalWordCount = 100, SummaryWordCount = 20 },
                new SummaryResult { OriginalWordCount = 200, SummaryWordCount = 40 },
                new SummaryResult { OriginalWordCount = 300, SummaryWordCount = 60 }
            }
        };

        Assert.Equal(600, result.TotalOriginalWords);
        Assert.Equal(120, result.TotalSummaryWords);
    }

    #endregion

    #region SummaryRequest Tests

    [Fact]
    public void SummaryRequest_HasCorrectDefaults()
    {
        var request = new SummaryRequest();

        Assert.Equal(string.Empty, request.Text);
        Assert.Null(request.Style);
        Assert.Null(request.Length);
        Assert.Null(request.CustomWordCount);
        Assert.Null(request.CustomPercentage);
        Assert.Null(request.Domain);
        Assert.Null(request.Model);
        Assert.Null(request.Context);
        Assert.Null(request.TargetAudience);
        Assert.False(request.ExtractKeyPoints);
        Assert.False(request.ExtractStatistics);
        Assert.False(request.PreserveQuotes);
        Assert.Null(request.FocusAreas);
        Assert.Null(request.OutputLanguage);
    }

    #endregion

    #region ActionItem Tests

    [Fact]
    public void ActionItem_HasCorrectDefaults()
    {
        var item = new ActionItem();

        Assert.Equal(string.Empty, item.Description);
        Assert.Null(item.Assignee);
        Assert.Null(item.DueDate);
        Assert.Equal(ActionPriority.Normal, item.Priority);
    }

    [Theory]
    [InlineData(ActionPriority.Low)]
    [InlineData(ActionPriority.Normal)]
    [InlineData(ActionPriority.High)]
    [InlineData(ActionPriority.Urgent)]
    public void ActionPriority_AllValuesValid(ActionPriority priority)
    {
        Assert.True(Enum.IsDefined(typeof(ActionPriority), priority));
    }

    [Fact]
    public void ActionPriority_HasExpectedCount()
    {
        var values = Enum.GetValues(typeof(ActionPriority));
        Assert.Equal(4, values.Length);
    }

    #endregion

    #region ActionItemsResult Tests

    [Fact]
    public void ActionItemsResult_HasCorrectDefaults()
    {
        var result = new ActionItemsResult();

        Assert.NotNull(result.ActionItems);
        Assert.Empty(result.ActionItems);
        Assert.Equal(string.Empty, result.OriginalText);
        Assert.Equal(string.Empty, result.Model);
    }

    #endregion

    #region SummaryComparisonResult Tests

    [Fact]
    public void SummaryComparisonResult_HasCorrectDefaults()
    {
        var result = new SummaryComparisonResult();

        Assert.Equal(string.Empty, result.CombinedSummary);
        Assert.NotNull(result.CommonThemes);
        Assert.Empty(result.CommonThemes);
        Assert.NotNull(result.KeyDifferences);
        Assert.Empty(result.KeyDifferences);
        Assert.NotNull(result.IndividualSummaries);
        Assert.Empty(result.IndividualSummaries);
    }

    #endregion

    #region BatchSummaryError Tests

    [Fact]
    public void BatchSummaryError_HasCorrectDefaults()
    {
        var error = new BatchSummaryError();

        Assert.Equal(0, error.Index);
        Assert.Equal(string.Empty, error.OriginalText);
        Assert.Equal(string.Empty, error.ErrorMessage);
    }

    #endregion

    #region Dependency Injection Tests

    [Fact]
    public void AddForeverToolsSummarize_WithApiKey_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsSummarize("test-api-key");

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SummarizeClient>();

        Assert.NotNull(client);
        Assert.Equal("test-api-key", client.Options.ApiKey);
    }

    [Fact]
    public void AddForeverToolsSummarize_WithOptions_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsSummarize(options =>
        {
            options.ApiKey = "test-api-key";
            options.DefaultModel = SummarizeModels.Claude35Sonnet;
            options.DefaultStyle = SummaryStyle.Executive;
        });

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SummarizeClient>();

        Assert.NotNull(client);
        Assert.Equal(SummarizeModels.Claude35Sonnet, client.Options.DefaultModel);
        Assert.Equal(SummaryStyle.Executive, client.Options.DefaultStyle);
    }

    [Fact]
    public void AddForeverToolsSummarize_WithStyle_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsSummarize("test-api-key", SummaryStyle.BulletPoints);

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SummarizeClient>();

        Assert.NotNull(client);
        Assert.Equal(SummaryStyle.BulletPoints, client.Options.DefaultStyle);
    }

    [Fact]
    public void AddForeverToolsSummarize_WithStyleAndLength_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsSummarize("test-api-key", SummaryStyle.BulletPoints, SummaryLength.Short);

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SummarizeClient>();

        Assert.NotNull(client);
        Assert.Equal(SummaryStyle.BulletPoints, client.Options.DefaultStyle);
        Assert.Equal(SummaryLength.Short, client.Options.DefaultLength);
    }

    [Fact]
    public void AddForeverToolsSummarize_WithModel_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsSummarize("test-api-key", SummarizeModels.Gpt4oMini);

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SummarizeClient>();

        Assert.NotNull(client);
        Assert.Equal(SummarizeModels.Gpt4oMini, client.Options.DefaultModel);
    }

    [Fact]
    public void AddForeverToolsSummarize_FromConfiguration_RegistersClient()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Summarize:ApiKey"] = "config-api-key",
                ["Summarize:DefaultModel"] = "gpt-4o-mini",
                ["Summarize:DefaultStyle"] = "BulletPoints",
                ["Summarize:DefaultLength"] = "Short"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddForeverToolsSummarize(configuration);

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SummarizeClient>();

        Assert.NotNull(client);
        Assert.Equal("config-api-key", client.Options.ApiKey);
        Assert.Equal("gpt-4o-mini", client.Options.DefaultModel);
        Assert.Equal(SummaryStyle.BulletPoints, client.Options.DefaultStyle);
        Assert.Equal(SummaryLength.Short, client.Options.DefaultLength);
    }

    [Fact]
    public void AddForeverToolsSummarize_FromAimlApiSection_RegistersClient()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AimlApi:ApiKey"] = "aiml-api-key"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddForeverToolsSummarize(configuration);

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SummarizeClient>();

        Assert.NotNull(client);
        Assert.Equal("aiml-api-key", client.Options.ApiKey);
    }

    [Fact]
    public void AddForeverToolsSummarize_IsSingleton()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsSummarize("test-api-key");

        var provider = services.BuildServiceProvider();
        var client1 = provider.GetRequiredService<SummarizeClient>();
        var client2 = provider.GetRequiredService<SummarizeClient>();

        Assert.Same(client1, client2);
    }

    #endregion

    #region FromEnvironment Tests

    [Fact]
    public void FromEnvironment_WithoutEnvVar_ThrowsInvalidOperationException()
    {
        // Ensure env vars are not set
        Environment.SetEnvironmentVariable("AIML_API_KEY", null);
        Environment.SetEnvironmentVariable("SUMMARIZE_API_KEY", null);

        Assert.Throws<InvalidOperationException>(() => SummarizeClient.FromEnvironment());
    }

    [Fact]
    public void FromEnvironment_WithAimlApiKey_CreatesClient()
    {
        var originalValue = Environment.GetEnvironmentVariable("AIML_API_KEY");
        try
        {
            Environment.SetEnvironmentVariable("AIML_API_KEY", "env-api-key");
            var client = SummarizeClient.FromEnvironment();

            Assert.NotNull(client);
            Assert.Equal("env-api-key", client.Options.ApiKey);
        }
        finally
        {
            Environment.SetEnvironmentVariable("AIML_API_KEY", originalValue);
        }
    }

    [Fact]
    public void FromEnvironment_WithSummarizeApiKey_CreatesClient()
    {
        var originalAiml = Environment.GetEnvironmentVariable("AIML_API_KEY");
        var originalSummarize = Environment.GetEnvironmentVariable("SUMMARIZE_API_KEY");
        try
        {
            Environment.SetEnvironmentVariable("AIML_API_KEY", null);
            Environment.SetEnvironmentVariable("SUMMARIZE_API_KEY", "summarize-api-key");
            var client = SummarizeClient.FromEnvironment();

            Assert.NotNull(client);
            Assert.Equal("summarize-api-key", client.Options.ApiKey);
        }
        finally
        {
            Environment.SetEnvironmentVariable("AIML_API_KEY", originalAiml);
            Environment.SetEnvironmentVariable("SUMMARIZE_API_KEY", originalSummarize);
        }
    }

    #endregion

    #region Validation Tests

    [Fact]
    public async Task SummarizeAsync_WithEmptyText_ThrowsArgumentException()
    {
        var client = new SummarizeClient("test-api-key");

        await Assert.ThrowsAsync<ArgumentException>(() => client.SummarizeAsync(""));
    }

    [Fact]
    public async Task SummarizeAsync_WithWhitespaceText_ThrowsArgumentException()
    {
        var client = new SummarizeClient("test-api-key");

        await Assert.ThrowsAsync<ArgumentException>(() => client.SummarizeAsync("   "));
    }

    [Fact]
    public async Task ExtractKeyPointsAsync_WithEmptyText_ThrowsArgumentException()
    {
        var client = new SummarizeClient("test-api-key");

        await Assert.ThrowsAsync<ArgumentException>(() => client.ExtractKeyPointsAsync(""));
    }

    [Fact]
    public async Task ExtractActionItemsAsync_WithEmptyText_ThrowsArgumentException()
    {
        var client = new SummarizeClient("test-api-key");

        await Assert.ThrowsAsync<ArgumentException>(() => client.ExtractActionItemsAsync(""));
    }

    [Fact]
    public async Task CompareAndSummarizeAsync_WithSingleText_ThrowsArgumentException()
    {
        var client = new SummarizeClient("test-api-key");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.CompareAndSummarizeAsync(new[] { "single text" }));
    }

    [Fact]
    public async Task CompareAndSummarizeAsync_WithEmptyList_ThrowsArgumentException()
    {
        var client = new SummarizeClient("test-api-key");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.CompareAndSummarizeAsync(Array.Empty<string>()));
    }

    #endregion
}
