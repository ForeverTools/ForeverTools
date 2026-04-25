using System.Net;
using System.Text;
using System.Text.Json;
using ForeverTools.InvoiceParser;
using ForeverTools.InvoiceParser.Extensions;
using ForeverTools.InvoiceParser.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace ForeverTools.InvoiceParser.Tests;

/// <summary>
/// Unit tests for InvoiceParserClient. All HTTP calls are mocked — no real API calls are made.
/// </summary>
public class InvoiceParserClientTests
{
    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static readonly InvoiceResult SampleInvoice = new()
    {
        InvoiceNumber = "INV-2025-001",
        Date = "2025-01-15",
        DueDate = "2025-02-15",
        Vendor = "Acme Corp",
        Customer = "Widget Ltd",
        LineItems = new List<InvoiceLineItem>
        {
            new() { Description = "Widget A", Quantity = 10, UnitPrice = 9.99m, Total = 99.90m },
            new() { Description = "Widget B", Quantity = 5,  UnitPrice = 19.99m, Total = 99.95m }
        },
        Subtotal = 199.85m,
        Tax = 20.0m,
        Total = 219.85m,
        Currency = "USD"
    };

    private static string SerializeInvoice(InvoiceResult invoice) =>
        JsonSerializer.Serialize(invoice, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

    private static HttpClient BuildMockedHttpClient(
        string? responseJson = null,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var body = responseJson ?? SerializeInvoice(SampleInvoice);
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
        return new HttpClient(handlerMock.Object);
    }

    private static InvoiceParserClient BuildClient(
        string? responseJson = null,
        HttpStatusCode code = HttpStatusCode.OK,
        string apiKey = "test-key")
    {
        var http = BuildMockedHttpClient(responseJson, code);
        var opts = new InvoiceParserOptions { ApiKey = apiKey };
        return new InvoiceParserClient(opts, http);
    }

    private static string CreateTempInvoiceFile(string? content = null)
    {
        var path = Path.Combine(Path.GetTempPath(), $"invoice_{Guid.NewGuid()}.pdf");
        File.WriteAllText(path, content ?? "%PDF-1.4 fake invoice content");
        return path;
    }

    // -------------------------------------------------------------------------
    // 1. Constructor validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_WithValidApiKey_CreatesClient()
    {
        var client = new InvoiceParserClient("test-key");
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithOptions_CreatesClient()
    {
        var opts = new InvoiceParserOptions { ApiKey = "test-key" };
        var client = new InvoiceParserClient(opts);
        client.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_NullOrEmptyApiKey_ThrowsArgumentException(string? key)
    {
        var opts = new InvoiceParserOptions { ApiKey = key! };
        var act = () => new InvoiceParserClient(opts);
        act.Should().Throw<ArgumentException>().WithMessage("*API key*");
    }

    [Fact]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        var act = () => new InvoiceParserClient((InvoiceParserOptions)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithHttpClient_DoesNotOwnIt()
    {
        var http = new HttpClient();
        var opts = new InvoiceParserOptions { ApiKey = "test-key" };
        var client = new InvoiceParserClient(opts, http);
        client.Should().NotBeNull();
        // Disposing the client should not dispose the external http client
        client.Dispose();
        // http is still usable (no ObjectDisposedException)
        http.Timeout.Should().BeGreaterThan(TimeSpan.Zero);
        http.Dispose();
    }

    // -------------------------------------------------------------------------
    // 2. ParseAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseAsync_ValidFile_ReturnsInvoiceResult()
    {
        var client = BuildClient();
        var path = CreateTempInvoiceFile();
        try
        {
            var result = await client.ParseAsync(path);
            result.Should().NotBeNull();
            result.InvoiceNumber.Should().Be("INV-2025-001");
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task ParseAsync_ValidFile_PopulatesVendor()
    {
        var client = BuildClient();
        var path = CreateTempInvoiceFile();
        try
        {
            var result = await client.ParseAsync(path);
            result.Vendor.Should().Be("Acme Corp");
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task ParseAsync_ValidFile_PopulatesCustomer()
    {
        var client = BuildClient();
        var path = CreateTempInvoiceFile();
        try
        {
            var result = await client.ParseAsync(path);
            result.Customer.Should().Be("Widget Ltd");
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task ParseAsync_ValidFile_PopulatesTotal()
    {
        var client = BuildClient();
        var path = CreateTempInvoiceFile();
        try
        {
            var result = await client.ParseAsync(path);
            result.Total.Should().Be(219.85m);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task ParseAsync_ValidFile_PopulatesCurrency()
    {
        var client = BuildClient();
        var path = CreateTempInvoiceFile();
        try
        {
            var result = await client.ParseAsync(path);
            result.Currency.Should().Be("USD");
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task ParseAsync_ValidFile_PopulatesLineItems()
    {
        var client = BuildClient();
        var path = CreateTempInvoiceFile();
        try
        {
            var result = await client.ParseAsync(path);
            result.LineItems.Should().HaveCount(2);
            result.LineItems[0].Description.Should().Be("Widget A");
            result.LineItems[0].Quantity.Should().Be(10);
            result.LineItems[0].UnitPrice.Should().Be(9.99m);
            result.LineItems[0].Total.Should().Be(99.90m);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task ParseAsync_ValidFile_PopulatesDates()
    {
        var client = BuildClient();
        var path = CreateTempInvoiceFile();
        try
        {
            var result = await client.ParseAsync(path);
            result.Date.Should().Be("2025-01-15");
            result.DueDate.Should().Be("2025-02-15");
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task ParseAsync_ValidFile_PopulatesSubtotalAndTax()
    {
        var client = BuildClient();
        var path = CreateTempInvoiceFile();
        try
        {
            var result = await client.ParseAsync(path);
            result.Subtotal.Should().Be(199.85m);
            result.Tax.Should().Be(20.0m);
        }
        finally { File.Delete(path); }
    }

    // -------------------------------------------------------------------------
    // 3. ParseAsync — input validation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseAsync_NullPath_ThrowsArgumentNullException()
    {
        var client = BuildClient();
        var act = async () => await client.ParseAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ParseAsync_EmptyOrWhitespacePath_ThrowsArgumentException(string path)
    {
        var client = BuildClient();
        var act = async () => await client.ParseAsync(path);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ParseAsync_NonExistentFile_ThrowsFileNotFoundException()
    {
        var client = BuildClient();
        var act = async () => await client.ParseAsync("/tmp/does_not_exist_xyz.pdf");
        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    // -------------------------------------------------------------------------
    // 4. ParseFromUrlAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseFromUrlAsync_ValidUrl_ReturnsInvoiceResult()
    {
        var client = BuildClient();
        var result = await client.ParseFromUrlAsync("https://example.com/invoice.pdf");
        result.Should().NotBeNull();
        result.InvoiceNumber.Should().Be("INV-2025-001");
    }

    [Fact]
    public async Task ParseFromUrlAsync_ValidUrl_SendsUrlInForm()
    {
        string? capturedContent = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedContent = await req.Content!.ReadAsStringAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(SerializeInvoice(SampleInvoice), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new InvoiceParserOptions { ApiKey = "test-key" };
        var client = new InvoiceParserClient(opts, http);

        await client.ParseFromUrlAsync("https://example.com/invoice.pdf");

        capturedContent.Should().Contain("example.com/invoice.pdf");
    }

    // -------------------------------------------------------------------------
    // 5. ParseFromUrlAsync — validation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseFromUrlAsync_NullUrl_ThrowsArgumentNullException()
    {
        var client = BuildClient();
        var act = async () => await client.ParseFromUrlAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ParseFromUrlAsync_EmptyOrWhitespaceUrl_ThrowsArgumentException(string url)
    {
        var client = BuildClient();
        var act = async () => await client.ParseFromUrlAsync(url);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ParseFromUrlAsync_RelativeUrl_ThrowsArgumentException()
    {
        var client = BuildClient();
        var act = async () => await client.ParseFromUrlAsync("not-a-url");
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ParseFromUrlAsync_HttpUrl_IsAccepted()
    {
        var client = BuildClient();
        var result = await client.ParseFromUrlAsync("http://example.com/invoice.pdf");
        result.Should().NotBeNull();
    }

    // -------------------------------------------------------------------------
    // 6. ParseFromStreamAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseFromStreamAsync_ValidStream_ReturnsInvoiceResult()
    {
        var client = BuildClient();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("fake pdf content"));
        var result = await client.ParseFromStreamAsync(stream, "invoice.pdf");
        result.Should().NotBeNull();
        result.InvoiceNumber.Should().Be("INV-2025-001");
    }

    [Fact]
    public async Task ParseFromStreamAsync_NullFileName_UsesDefault()
    {
        var client = BuildClient();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));
        // Should not throw even with null fileName
        var result = await client.ParseFromStreamAsync(stream, null);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ParseFromStreamAsync_EmptyFileName_UsesDefault()
    {
        var client = BuildClient();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("content"));
        var result = await client.ParseFromStreamAsync(stream, "");
        result.Should().NotBeNull();
    }

    // -------------------------------------------------------------------------
    // 7. ParseFromStreamAsync — validation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseFromStreamAsync_NullStream_ThrowsArgumentNullException()
    {
        var client = BuildClient();
        var act = async () => await client.ParseFromStreamAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    // -------------------------------------------------------------------------
    // 8. HTTP error handling
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseAsync_Http401_ThrowsUnauthorizedAccessException()
    {
        var client = BuildClient(code: HttpStatusCode.Unauthorized);
        var path = CreateTempInvoiceFile();
        try
        {
            var act = async () => await client.ParseAsync(path);
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task ParseAsync_Http429_ThrowsRateLimitException()
    {
        var client = BuildClient(code: (HttpStatusCode)429);
        var path = CreateTempInvoiceFile();
        try
        {
            var act = async () => await client.ParseAsync(path);
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Rate limit*");
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task ParseAsync_Http500_ThrowsHttpRequestException()
    {
        var client = BuildClient(code: HttpStatusCode.InternalServerError);
        var path = CreateTempInvoiceFile();
        try
        {
            var act = async () => await client.ParseAsync(path);
            await act.Should().ThrowAsync<HttpRequestException>();
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task ParseFromUrlAsync_Http401_ThrowsUnauthorizedAccessException()
    {
        var client = BuildClient(code: HttpStatusCode.Unauthorized);
        var act = async () => await client.ParseFromUrlAsync("https://example.com/inv.pdf");
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task ParseFromUrlAsync_Http429_ThrowsRateLimitException()
    {
        var client = BuildClient(code: (HttpStatusCode)429);
        var act = async () => await client.ParseFromUrlAsync("https://example.com/inv.pdf");
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Rate limit*");
    }

    // -------------------------------------------------------------------------
    // 9. Authorization header
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseFromUrlAsync_SendsAuthorizationHeader()
    {
        HttpRequestMessage? captured = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                captured = req;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(SerializeInvoice(SampleInvoice), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new InvoiceParserOptions { ApiKey = "secret-api-key" };
        var client = new InvoiceParserClient(opts, http);

        await client.ParseFromUrlAsync("https://example.com/invoice.pdf");

        captured!.Headers.Authorization.Should().NotBeNull();
        captured.Headers.Authorization!.Scheme.Should().Be("Bearer");
        captured.Headers.Authorization.Parameter.Should().Be("secret-api-key");
    }

    // -------------------------------------------------------------------------
    // 10. Endpoint path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseFromUrlAsync_PostsToCorrectEndpoint()
    {
        Uri? capturedUri = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedUri = req.RequestUri;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(SerializeInvoice(SampleInvoice), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new InvoiceParserOptions { ApiKey = "test-key" };
        var client = new InvoiceParserClient(opts, http);

        await client.ParseFromUrlAsync("https://example.com/invoice.pdf");

        capturedUri!.AbsolutePath.Should().Contain("invoice/parse");
    }

    [Fact]
    public async Task ParseFromUrlAsync_UsesPostMethod()
    {
        HttpMethod? capturedMethod = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedMethod = req.Method;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(SerializeInvoice(SampleInvoice), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new InvoiceParserOptions { ApiKey = "test-key" };
        var client = new InvoiceParserClient(opts, http);

        await client.ParseFromUrlAsync("https://example.com/invoice.pdf");

        capturedMethod.Should().Be(HttpMethod.Post);
    }

    // -------------------------------------------------------------------------
    // 11. Cancellation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseFromUrlAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        var http = new HttpClient(handlerMock.Object);
        var opts = new InvoiceParserOptions { ApiKey = "test-key" };
        var client = new InvoiceParserClient(opts, http);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await client.ParseFromUrlAsync("https://example.com/invoice.pdf", cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ParseFromStreamAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        var http = new HttpClient(handlerMock.Object);
        var opts = new InvoiceParserOptions { ApiKey = "test-key" };
        var client = new InvoiceParserClient(opts, http);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var act = async () => await client.ParseFromStreamAsync(stream, "test.pdf", cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    // -------------------------------------------------------------------------
    // 12. FromEnvironment factory
    // -------------------------------------------------------------------------

    [Fact]
    public void FromEnvironment_MissingEnvVar_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("INVOICE_TEST_MISSING_KEY", null);
        var act = () => InvoiceParserClient.FromEnvironment("INVOICE_TEST_MISSING_KEY");
        act.Should().Throw<InvalidOperationException>().WithMessage("*not set*");
    }

    [Fact]
    public void FromEnvironment_SetEnvVar_CreatesClient()
    {
        const string envVar = "INVOICE_TEST_KEY_SET";
        Environment.SetEnvironmentVariable(envVar, "fake-api-key");
        try
        {
            var client = InvoiceParserClient.FromEnvironment(envVar);
            client.Should().NotBeNull();
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    // -------------------------------------------------------------------------
    // 13. InvoiceParserOptions
    // -------------------------------------------------------------------------

    [Fact]
    public void InvoiceParserOptions_HasCorrectDefaults()
    {
        var opts = new InvoiceParserOptions();
        opts.BaseUrl.Should().Be("https://api.aimlapi.com");
        opts.Timeout.Should().Be(TimeSpan.FromSeconds(120));
        opts.ApiKey.Should().BeEmpty();
    }

    [Fact]
    public void InvoiceParserOptions_DefaultBaseUrl_Constant_IsCorrect()
    {
        InvoiceParserOptions.DefaultBaseUrl.Should().Be("https://api.aimlapi.com");
    }

    [Fact]
    public void InvoiceParserOptions_CanSetCustomTimeout()
    {
        var opts = new InvoiceParserOptions { ApiKey = "key", Timeout = TimeSpan.FromSeconds(30) };
        opts.Timeout.TotalSeconds.Should().Be(30);
    }

    [Fact]
    public void InvoiceParserOptions_CanSetCustomBaseUrl()
    {
        var opts = new InvoiceParserOptions { ApiKey = "key", BaseUrl = "https://custom.api.com" };
        opts.BaseUrl.Should().Be("https://custom.api.com");
    }

    [Fact]
    public void InvoiceParserOptions_FromEnvironment_MissingVar_Throws()
    {
        Environment.SetEnvironmentVariable("INVOICE_OPTS_MISSING", null);
        var act = () => InvoiceParserOptions.FromEnvironment("INVOICE_OPTS_MISSING");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void InvoiceParserOptions_FromEnvironment_ReadsApiKey()
    {
        const string envVar = "INVOICE_OPTS_KEY";
        Environment.SetEnvironmentVariable(envVar, "my-invoice-key");
        try
        {
            var opts = InvoiceParserOptions.FromEnvironment(envVar);
            opts.ApiKey.Should().Be("my-invoice-key");
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    // -------------------------------------------------------------------------
    // 14. InvoiceResult model
    // -------------------------------------------------------------------------

    [Fact]
    public void InvoiceResult_DefaultLineItems_IsEmpty()
    {
        var result = new InvoiceResult();
        result.LineItems.Should().NotBeNull();
        result.LineItems.Should().BeEmpty();
    }

    [Fact]
    public void InvoiceResult_NullableFields_DefaultToNull()
    {
        var result = new InvoiceResult();
        result.InvoiceNumber.Should().BeNull();
        result.Date.Should().BeNull();
        result.DueDate.Should().BeNull();
        result.Vendor.Should().BeNull();
        result.Customer.Should().BeNull();
        result.Subtotal.Should().BeNull();
        result.Tax.Should().BeNull();
        result.Total.Should().BeNull();
        result.Currency.Should().BeNull();
    }

    // -------------------------------------------------------------------------
    // 15. InvoiceLineItem model
    // -------------------------------------------------------------------------

    [Fact]
    public void InvoiceLineItem_DefaultDescription_IsEmpty()
    {
        var item = new InvoiceLineItem();
        item.Description.Should().BeEmpty();
    }

    [Fact]
    public void InvoiceLineItem_NullableDecimalFields_DefaultToNull()
    {
        var item = new InvoiceLineItem();
        item.Quantity.Should().BeNull();
        item.UnitPrice.Should().BeNull();
        item.Total.Should().BeNull();
    }

    [Fact]
    public void InvoiceLineItem_CanSetAllFields()
    {
        var item = new InvoiceLineItem
        {
            Description = "Test Item",
            Quantity = 3,
            UnitPrice = 12.50m,
            Total = 37.50m
        };
        item.Description.Should().Be("Test Item");
        item.Quantity.Should().Be(3);
        item.UnitPrice.Should().Be(12.50m);
        item.Total.Should().Be(37.50m);
    }

    // -------------------------------------------------------------------------
    // 16. DI / ServiceCollection extension
    // -------------------------------------------------------------------------

    [Fact]
    public void AddInvoiceParser_WithApiKey_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddInvoiceParser("test-key");
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<InvoiceParserClient>();
        client.Should().NotBeNull();
    }

    [Fact]
    public void AddInvoiceParser_WithOptions_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddInvoiceParser(opts =>
        {
            opts.ApiKey = "test-key";
            opts.Timeout = TimeSpan.FromSeconds(30);
        });
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<InvoiceParserClient>();
        client.Should().NotBeNull();
    }

    // -------------------------------------------------------------------------
    // 17. Dispose
    // -------------------------------------------------------------------------

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var client = new InvoiceParserClient("test-key");
        client.Dispose();
        var act = () => client.Dispose();
        act.Should().NotThrow();
    }

    // -------------------------------------------------------------------------
    // 18. BaseUrl trailing slash handling
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseFromUrlAsync_BaseUrlWithTrailingSlash_PostsToCorrectEndpoint()
    {
        Uri? capturedUri = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                capturedUri = req.RequestUri;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(SerializeInvoice(SampleInvoice), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new InvoiceParserOptions
        {
            ApiKey = "test-key",
            BaseUrl = "https://api.aimlapi.com/"  // trailing slash
        };
        var client = new InvoiceParserClient(opts, http);

        await client.ParseFromUrlAsync("https://example.com/invoice.pdf");

        capturedUri!.AbsolutePath.Should().NotContain("//");
        capturedUri.AbsolutePath.Should().Contain("invoice/parse");
    }

    // -------------------------------------------------------------------------
    // 19. Multipart content type
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseFromStreamAsync_SendsMultipartFormData()
    {
        string? contentType = null;
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage req, CancellationToken _) =>
            {
                contentType = req.Content?.Headers.ContentType?.MediaType;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(SerializeInvoice(SampleInvoice), Encoding.UTF8, "application/json")
                };
            });

        var http = new HttpClient(handlerMock.Object);
        var opts = new InvoiceParserOptions { ApiKey = "test-key" };
        var client = new InvoiceParserClient(opts, http);

        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        await client.ParseFromStreamAsync(stream, "invoice.pdf");

        contentType.Should().Be("multipart/form-data");
    }

    // -------------------------------------------------------------------------
    // 20. JSON deserialization
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ParseFromUrlAsync_DeserializesMultipleLineItems()
    {
        var invoice = new InvoiceResult
        {
            LineItems = new List<InvoiceLineItem>
            {
                new() { Description = "Item 1", Quantity = 1, UnitPrice = 10m, Total = 10m },
                new() { Description = "Item 2", Quantity = 2, UnitPrice = 5m,  Total = 10m },
                new() { Description = "Item 3", Quantity = 3, UnitPrice = 3m,  Total = 9m  }
            },
            Total = 29m,
            Currency = "EUR"
        };

        var client = BuildClient(SerializeInvoice(invoice));
        var result = await client.ParseFromUrlAsync("https://example.com/invoice.pdf");

        result.LineItems.Should().HaveCount(3);
        result.LineItems[2].Description.Should().Be("Item 3");
        result.Currency.Should().Be("EUR");
    }

    [Fact]
    public async Task ParseFromUrlAsync_HandlesInvoiceWithNoLineItems()
    {
        var invoice = new InvoiceResult
        {
            InvoiceNumber = "SIMPLE-001",
            Total = 500m,
            Currency = "GBP",
            LineItems = new List<InvoiceLineItem>()
        };

        var client = BuildClient(SerializeInvoice(invoice));
        var result = await client.ParseFromUrlAsync("https://example.com/invoice.pdf");

        result.InvoiceNumber.Should().Be("SIMPLE-001");
        result.LineItems.Should().BeEmpty();
        result.Total.Should().Be(500m);
    }
}
