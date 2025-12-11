using ForeverTools.OCR;
using ForeverTools.OCR.Models;
using ForeverTools.OCR.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ForeverTools.OCR.Tests;

public class OcrClientTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithApiKey_CreatesClient()
    {
        var client = new OcrClient("test-api-key");
        Assert.NotNull(client);
        Assert.Equal("test-api-key", client.Options.ApiKey);
    }

    [Fact]
    public void Constructor_WithOptions_CreatesClient()
    {
        var options = new OcrOptions
        {
            ApiKey = "test-key",
            DefaultModel = OcrModels.Claude35Sonnet,
            TimeoutSeconds = 120,
            MaxTokens = 8192,
            ImageDetail = "high"
        };

        var client = new OcrClient(options);

        Assert.NotNull(client);
        Assert.Equal("test-key", client.Options.ApiKey);
        Assert.Equal(OcrModels.Claude35Sonnet, client.Options.DefaultModel);
        Assert.Equal(120, client.Options.TimeoutSeconds);
        Assert.Equal(8192, client.Options.MaxTokens);
        Assert.Equal("high", client.Options.ImageDetail);
    }

    [Fact]
    public void Constructor_WithEmptyApiKey_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new OcrClient(""));
    }

    [Fact]
    public void Constructor_WithNullApiKey_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new OcrClient((string)null!));
    }

    [Fact]
    public void Constructor_WithWhitespaceApiKey_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new OcrClient("   "));
    }

    #endregion

    #region OcrOptions Tests

    [Fact]
    public void OcrOptions_DefaultValues_AreCorrect()
    {
        var options = new OcrOptions();

        Assert.Equal(string.Empty, options.ApiKey);
        Assert.Equal("https://api.aimlapi.com/v1", options.BaseUrl);
        Assert.Equal(OcrModels.Gpt4o, options.DefaultModel);
        Assert.Equal(60, options.TimeoutSeconds);
        Assert.Equal(4096, options.MaxTokens);
        Assert.Equal("auto", options.ImageDetail);
    }

    [Fact]
    public void OcrOptions_CanSetAllProperties()
    {
        var options = new OcrOptions
        {
            ApiKey = "my-key",
            BaseUrl = "https://custom.api.com/v1",
            DefaultModel = OcrModels.Gemini15Pro,
            TimeoutSeconds = 90,
            MaxTokens = 2048,
            ImageDetail = "low"
        };

        Assert.Equal("my-key", options.ApiKey);
        Assert.Equal("https://custom.api.com/v1", options.BaseUrl);
        Assert.Equal(OcrModels.Gemini15Pro, options.DefaultModel);
        Assert.Equal(90, options.TimeoutSeconds);
        Assert.Equal(2048, options.MaxTokens);
        Assert.Equal("low", options.ImageDetail);
    }

    #endregion

    #region OcrModels Tests

    [Fact]
    public void OcrModels_Gpt4o_IsCorrect()
    {
        Assert.Equal("gpt-4o", OcrModels.Gpt4o);
    }

    [Fact]
    public void OcrModels_Gpt4oMini_IsCorrect()
    {
        Assert.Equal("gpt-4o-mini", OcrModels.Gpt4oMini);
    }

    [Fact]
    public void OcrModels_Gpt4Turbo_IsCorrect()
    {
        Assert.Equal("gpt-4-turbo", OcrModels.Gpt4Turbo);
    }

    [Fact]
    public void OcrModels_Claude35Sonnet_IsCorrect()
    {
        Assert.Equal("claude-3-5-sonnet-20241022", OcrModels.Claude35Sonnet);
    }

    [Fact]
    public void OcrModels_Claude3Opus_IsCorrect()
    {
        Assert.Equal("claude-3-opus-20240229", OcrModels.Claude3Opus);
    }

    [Fact]
    public void OcrModels_Claude3Sonnet_IsCorrect()
    {
        Assert.Equal("claude-3-sonnet-20240229", OcrModels.Claude3Sonnet);
    }

    [Fact]
    public void OcrModels_Claude3Haiku_IsCorrect()
    {
        Assert.Equal("claude-3-haiku-20240307", OcrModels.Claude3Haiku);
    }

    [Fact]
    public void OcrModels_Gemini15Pro_IsCorrect()
    {
        Assert.Equal("gemini-1.5-pro", OcrModels.Gemini15Pro);
    }

    [Fact]
    public void OcrModels_Gemini15Flash_IsCorrect()
    {
        Assert.Equal("gemini-1.5-flash", OcrModels.Gemini15Flash);
    }

    [Fact]
    public void OcrModels_Recommendations_AreCorrect()
    {
        Assert.Equal(OcrModels.Gpt4o, OcrModels.Recommendations.GeneralPurpose);
        Assert.Equal(OcrModels.Claude3Opus, OcrModels.Recommendations.Handwriting);
        Assert.Equal(OcrModels.Gpt4Turbo, OcrModels.Recommendations.ScannedDocuments);
        Assert.Equal(OcrModels.Gpt4o, OcrModels.Recommendations.Receipts);
        Assert.Equal(OcrModels.Gpt4oMini, OcrModels.Recommendations.Screenshots);
        Assert.Equal(OcrModels.Claude35Sonnet, OcrModels.Recommendations.Forms);
        Assert.Equal(OcrModels.Gemini15Pro, OcrModels.Recommendations.Multilingual);
        Assert.Equal(OcrModels.Gpt4oMini, OcrModels.Recommendations.Fast);
        Assert.Equal(OcrModels.Claude3Opus, OcrModels.Recommendations.HighAccuracy);
    }

    #endregion

    #region OcrResult Tests

    [Fact]
    public void OcrResult_Successful_CreatesCorrectResult()
    {
        var result = OcrResult.Successful("Hello World", "gpt-4o", 100, 500);

        Assert.True(result.Success);
        Assert.Equal("Hello World", result.Text);
        Assert.Equal("gpt-4o", result.Model);
        Assert.Equal(100, result.TokensUsed);
        Assert.Equal(500, result.ProcessingTimeMs);
        Assert.Null(result.Error);
    }

    [Fact]
    public void OcrResult_Failed_CreatesCorrectResult()
    {
        var result = OcrResult.Failed("Something went wrong", "gpt-4o");

        Assert.False(result.Success);
        Assert.Equal("Something went wrong", result.Error);
        Assert.Equal("gpt-4o", result.Model);
        Assert.Equal(string.Empty, result.Text);
    }

    [Fact]
    public void OcrResult_DefaultValues_AreCorrect()
    {
        var result = new OcrResult();

        Assert.False(result.Success);
        Assert.Equal(string.Empty, result.Text);
        Assert.Equal(string.Empty, result.Model);
        Assert.Equal(0, result.TokensUsed);
        Assert.Equal(0, result.ProcessingTimeMs);
        Assert.Null(result.Error);
        Assert.Null(result.Confidence);
    }

    #endregion

    #region StructuredOcrResult Tests

    [Fact]
    public void StructuredOcrResult_DefaultValues_AreEmpty()
    {
        var result = new StructuredOcrResult();

        Assert.NotNull(result.Blocks);
        Assert.Empty(result.Blocks);
        Assert.NotNull(result.Paragraphs);
        Assert.Empty(result.Paragraphs);
        Assert.NotNull(result.Lines);
        Assert.Empty(result.Lines);
    }

    [Fact]
    public void StructuredOcrResult_CanAddBlocks()
    {
        var result = new StructuredOcrResult();
        result.Blocks.Add(new TextBlock
        {
            Text = "Hello",
            BlockType = "heading",
            Order = 0
        });
        result.Blocks.Add(new TextBlock
        {
            Text = "World",
            BlockType = "paragraph",
            Order = 1
        });

        Assert.Equal(2, result.Blocks.Count);
        Assert.Equal("heading", result.Blocks[0].BlockType);
        Assert.Equal("paragraph", result.Blocks[1].BlockType);
    }

    #endregion

    #region TextBlock Tests

    [Fact]
    public void TextBlock_DefaultValues_AreCorrect()
    {
        var block = new TextBlock();

        Assert.Equal(string.Empty, block.Text);
        Assert.Equal("paragraph", block.BlockType);
        Assert.Null(block.Confidence);
        Assert.Equal(0, block.Order);
    }

    [Fact]
    public void TextBlock_CanSetAllProperties()
    {
        var block = new TextBlock
        {
            Text = "Test text",
            BlockType = "heading",
            Confidence = 0.95,
            Order = 5
        };

        Assert.Equal("Test text", block.Text);
        Assert.Equal("heading", block.BlockType);
        Assert.Equal(0.95, block.Confidence);
        Assert.Equal(5, block.Order);
    }

    #endregion

    #region ExtractedTable Tests

    [Fact]
    public void ExtractedTable_DefaultValues_AreEmpty()
    {
        var table = new ExtractedTable();

        Assert.NotNull(table.Headers);
        Assert.Empty(table.Headers);
        Assert.NotNull(table.Rows);
        Assert.Empty(table.Rows);
        Assert.Equal(0, table.ColumnCount);
        Assert.Equal(0, table.RowCount);
    }

    [Fact]
    public void ExtractedTable_ColumnCount_FromHeaders()
    {
        var table = new ExtractedTable();
        table.Headers.Add("Name");
        table.Headers.Add("Age");
        table.Headers.Add("City");

        Assert.Equal(3, table.ColumnCount);
    }

    [Fact]
    public void ExtractedTable_ColumnCount_FromRows_WhenNoHeaders()
    {
        var table = new ExtractedTable();
        table.Rows.Add(new List<string> { "A", "B", "C", "D" });

        Assert.Equal(4, table.ColumnCount);
    }

    [Fact]
    public void ExtractedTable_RowCount_IsCorrect()
    {
        var table = new ExtractedTable();
        table.Rows.Add(new List<string> { "1", "2" });
        table.Rows.Add(new List<string> { "3", "4" });
        table.Rows.Add(new List<string> { "5", "6" });

        Assert.Equal(3, table.RowCount);
    }

    [Fact]
    public void ExtractedTable_ToCsv_WithHeaders()
    {
        var table = new ExtractedTable();
        table.Headers.AddRange(new[] { "Name", "Age" });
        table.Rows.Add(new List<string> { "John", "30" });
        table.Rows.Add(new List<string> { "Jane", "25" });

        var csv = table.ToCsv();

        Assert.Contains("Name,Age", csv);
        Assert.Contains("John,30", csv);
        Assert.Contains("Jane,25", csv);
    }

    [Fact]
    public void ExtractedTable_ToCsv_EscapesCommas()
    {
        var table = new ExtractedTable();
        table.Headers.Add("Description");
        table.Rows.Add(new List<string> { "Hello, World" });

        var csv = table.ToCsv();

        Assert.Contains("\"Hello, World\"", csv);
    }

    [Fact]
    public void ExtractedTable_ToCsv_EscapesQuotes()
    {
        var table = new ExtractedTable();
        table.Headers.Add("Quote");
        table.Rows.Add(new List<string> { "He said \"Hello\"" });

        var csv = table.ToCsv();

        Assert.Contains("\"He said \"\"Hello\"\"\"", csv);
    }

    #endregion

    #region FormOcrResult Tests

    [Fact]
    public void FormOcrResult_DefaultValues_AreEmpty()
    {
        var result = new FormOcrResult();

        Assert.NotNull(result.Fields);
        Assert.Empty(result.Fields);
    }

    [Fact]
    public void FormOcrResult_GetField_ReturnsValue()
    {
        var result = new FormOcrResult();
        result.Fields["Name"] = "John Doe";
        result.Fields["Email"] = "john@example.com";

        Assert.Equal("John Doe", result.GetField("Name"));
        Assert.Equal("john@example.com", result.GetField("Email"));
    }

    [Fact]
    public void FormOcrResult_GetField_IsCaseInsensitive()
    {
        var result = new FormOcrResult();
        result.Fields["Name"] = "John Doe";

        Assert.Equal("John Doe", result.GetField("name"));
        Assert.Equal("John Doe", result.GetField("NAME"));
        Assert.Equal("John Doe", result.GetField("NaMe"));
    }

    [Fact]
    public void FormOcrResult_GetField_ReturnsNullForMissingField()
    {
        var result = new FormOcrResult();
        result.Fields["Name"] = "John";

        Assert.Null(result.GetField("Address"));
    }

    #endregion

    #region ReceiptOcrResult Tests

    [Fact]
    public void ReceiptOcrResult_DefaultValues_AreNull()
    {
        var result = new ReceiptOcrResult();

        Assert.Null(result.MerchantName);
        Assert.Null(result.Date);
        Assert.Null(result.Total);
        Assert.Null(result.Subtotal);
        Assert.Null(result.Tax);
        Assert.Null(result.Currency);
        Assert.Null(result.PaymentMethod);
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
    }

    [Fact]
    public void ReceiptOcrResult_CanSetAllProperties()
    {
        var result = new ReceiptOcrResult
        {
            MerchantName = "Test Store",
            Date = "2024-01-15",
            Total = "$50.00",
            Subtotal = "$45.00",
            Tax = "$5.00",
            Currency = "USD",
            PaymentMethod = "VISA"
        };

        Assert.Equal("Test Store", result.MerchantName);
        Assert.Equal("2024-01-15", result.Date);
        Assert.Equal("$50.00", result.Total);
        Assert.Equal("$45.00", result.Subtotal);
        Assert.Equal("$5.00", result.Tax);
        Assert.Equal("USD", result.Currency);
        Assert.Equal("VISA", result.PaymentMethod);
    }

    [Fact]
    public void ReceiptLineItem_DefaultValues_AreCorrect()
    {
        var item = new ReceiptLineItem();

        Assert.Equal(string.Empty, item.Description);
        Assert.Null(item.Quantity);
        Assert.Null(item.UnitPrice);
        Assert.Null(item.TotalPrice);
    }

    [Fact]
    public void ReceiptOcrResult_CanAddItems()
    {
        var result = new ReceiptOcrResult();
        result.Items.Add(new ReceiptLineItem
        {
            Description = "Coffee",
            Quantity = "2",
            UnitPrice = "$3.00",
            TotalPrice = "$6.00"
        });

        Assert.Single(result.Items);
        Assert.Equal("Coffee", result.Items[0].Description);
    }

    #endregion

    #region TableOcrResult Tests

    [Fact]
    public void TableOcrResult_DefaultValues_AreEmpty()
    {
        var result = new TableOcrResult();

        Assert.NotNull(result.Tables);
        Assert.Empty(result.Tables);
    }

    [Fact]
    public void TableOcrResult_CanAddTables()
    {
        var result = new TableOcrResult();
        result.Tables.Add(new ExtractedTable
        {
            Headers = new List<string> { "A", "B" },
            Rows = new List<List<string>> { new List<string> { "1", "2" } }
        });

        Assert.Single(result.Tables);
        Assert.Equal(2, result.Tables[0].ColumnCount);
    }

    #endregion

    #region Dependency Injection Tests

    [Fact]
    public void AddForeverToolsOcr_WithApiKey_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsOcr("test-api-key");

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<OcrClient>();

        Assert.NotNull(client);
        Assert.Equal("test-api-key", client.Options.ApiKey);
    }

    [Fact]
    public void AddForeverToolsOcr_WithOptions_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsOcr(options =>
        {
            options.ApiKey = "custom-key";
            options.DefaultModel = OcrModels.Claude3Opus;
            options.TimeoutSeconds = 120;
        });

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<OcrClient>();

        Assert.NotNull(client);
        Assert.Equal("custom-key", client.Options.ApiKey);
        Assert.Equal(OcrModels.Claude3Opus, client.Options.DefaultModel);
        Assert.Equal(120, client.Options.TimeoutSeconds);
    }

    [Fact]
    public void AddForeverToolsOcr_WithConfiguration_RegistersClient()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OCR:ApiKey"] = "config-key",
                ["OCR:DefaultModel"] = "gemini-1.5-pro",
                ["OCR:TimeoutSeconds"] = "90",
                ["OCR:MaxTokens"] = "2048",
                ["OCR:ImageDetail"] = "high"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddForeverToolsOcr(config);

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<OcrClient>();

        Assert.NotNull(client);
        Assert.Equal("config-key", client.Options.ApiKey);
        Assert.Equal("gemini-1.5-pro", client.Options.DefaultModel);
        Assert.Equal(90, client.Options.TimeoutSeconds);
        Assert.Equal(2048, client.Options.MaxTokens);
        Assert.Equal("high", client.Options.ImageDetail);
    }

    [Fact]
    public void AddForeverToolsOcr_ReturnsSameInstance()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsOcr("test-key");

        var provider = services.BuildServiceProvider();
        var client1 = provider.GetRequiredService<OcrClient>();
        var client2 = provider.GetRequiredService<OcrClient>();

        Assert.Same(client1, client2);
    }

    #endregion

    #region FromEnvironment Tests

    [Fact]
    public void FromEnvironment_WithMissingVariable_ThrowsException()
    {
        Environment.SetEnvironmentVariable("TEST_OCR_KEY_MISSING", null);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            OcrClient.FromEnvironment("TEST_OCR_KEY_MISSING"));

        Assert.Contains("not set", ex.Message);
        Assert.Contains("aimlapi.com", ex.Message);
    }

    [Fact]
    public void FromEnvironment_WithSetVariable_CreatesClient()
    {
        Environment.SetEnvironmentVariable("TEST_OCR_KEY_SET", "env-test-key");

        try
        {
            var client = OcrClient.FromEnvironment("TEST_OCR_KEY_SET");
            Assert.NotNull(client);
            Assert.Equal("env-test-key", client.Options.ApiKey);
        }
        finally
        {
            Environment.SetEnvironmentVariable("TEST_OCR_KEY_SET", null);
        }
    }

    #endregion

    #region ExtractTextFromFile Validation Tests

    [Fact]
    public async Task ExtractTextFromFileAsync_WithNonExistentFile_ReturnsFailed()
    {
        var client = new OcrClient("test-key");

        var result = await client.ExtractTextFromFileAsync("/nonexistent/path/file.png");

        Assert.False(result.Success);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var client = new OcrClient("test-key");

        client.Dispose();
        client.Dispose(); // Should not throw

        Assert.True(true);
    }

    #endregion
}
