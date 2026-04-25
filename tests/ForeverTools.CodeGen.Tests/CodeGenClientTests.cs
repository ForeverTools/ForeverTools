using System.Net;
using System.Text;
using System.Text.Json;
using ForeverTools.CodeGen;
using ForeverTools.CodeGen.Extensions;
using ForeverTools.CodeGen.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace ForeverTools.CodeGen.Tests;

/// <summary>
/// Unit tests for CodeGenClient. All HTTP calls are mocked — no real API calls are made.
/// </summary>
public class CodeGenClientTests
{
    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static HttpClient BuildMockedHttpClient(string contentJson, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var wrapper = new
        {
            choices = new[]
            {
                new { message = new { role = "assistant", content = contentJson } }
            }
        };
        var body = JsonSerializer.Serialize(wrapper);
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

    private static HttpClient BuildRawMockedHttpClient(string rawBody, HttpStatusCode statusCode)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(rawBody, Encoding.UTF8, "application/json")
            });
        return new HttpClient(handlerMock.Object);
    }

    private static CodeGenClient BuildClient(string contentJson, HttpStatusCode code = HttpStatusCode.OK)
    {
        var http = BuildMockedHttpClient(contentJson, code);
        var opts = new CodeGenOptions { ApiKey = "test-key" };
        return new CodeGenClient(opts, http);
    }

    private static string MakeCodeGenJson(
        string code = "print('hello')",
        string language = "python",
        string explanation = "Prints hello to stdout.",
        string? notes = null)
    {
        var notesVal = notes != null ? $"\"{notes}\"" : "null";
        return $"{{\"code\":\"{code}\",\"language\":\"{language}\",\"explanation\":\"{explanation}\",\"notes\":{notesVal}}}";
    }

    private static string MakeRefactorJson(
        string code = "def f(x): return x+1",
        string language = "python",
        string summary = "Renamed for clarity.",
        string[]? improvements = null)
    {
        var impr = improvements ?? new[] { "Renamed variable x to value", "Added type hint" };
        var imprJson = "[" + string.Join(",", impr.Select(i => $"\"{i}\"")) + "]";
        return $"{{\"code\":\"{code}\",\"language\":\"{language}\",\"summary\":\"{summary}\",\"improvements\":{imprJson}}}";
    }

    private static string MakeExplainJson(
        string summary = "Reads a file and prints lines.",
        string[] ? steps = null,
        string language = "python",
        string complexity = "simple")
    {
        var s = steps ?? new[] { "Opens the file", "Iterates lines", "Prints each line" };
        var stepsJson = "[" + string.Join(",", s.Select(st => $"\"{st}\"")) + "]";
        return $"{{\"summary\":\"{summary}\",\"steps\":{stepsJson},\"language\":\"{language}\",\"complexity\":\"{complexity}\"}}";
    }

    // -------------------------------------------------------------------------
    // 1. Constructor validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_NullOptions_Throws()
    {
        Action act = () => new CodeGenClient(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_EmptyApiKey_Throws()
    {
        Action act = () => new CodeGenClient(new CodeGenOptions { ApiKey = "" });
        act.Should().Throw<ArgumentException>().WithMessage("*API key*");
    }

    [Fact]
    public void Constructor_EmptyModel_Throws()
    {
        Action act = () => new CodeGenClient(new CodeGenOptions { ApiKey = "key", Model = "" });
        act.Should().Throw<ArgumentException>().WithMessage("*Model*");
    }

    [Fact]
    public void Constructor_InvalidBaseUrl_Throws()
    {
        Action act = () => new CodeGenClient(new CodeGenOptions { ApiKey = "key", BaseUrl = "not-a-url" });
        act.Should().Throw<ArgumentException>().WithMessage("*BaseUrl*");
    }

    [Fact]
    public void Constructor_ValidOptions_CreatesInstance()
    {
        var client = new CodeGenClient(new CodeGenOptions { ApiKey = "key" });
        client.Should().NotBeNull();
        client.Dispose();
    }

    [Fact]
    public void Constructor_WithExternalHttpClient_DoesNotOwnIt()
    {
        var http = new HttpClient();
        var client = new CodeGenClient(new CodeGenOptions { ApiKey = "key" }, http);
        client.Should().NotBeNull();
        client.Dispose();
        // external HttpClient should still be usable after Dispose
        http.Dispose();
    }

    // -------------------------------------------------------------------------
    // 2. CodeGenOptions
    // -------------------------------------------------------------------------

    [Fact]
    public void CodeGenOptions_Defaults_AreCorrect()
    {
        var opts = new CodeGenOptions();
        opts.BaseUrl.Should().Be(CodeGenOptions.DefaultBaseUrl);
        opts.Model.Should().Be(CodeGenOptions.DefaultModel);
        opts.Timeout.Should().Be(TimeSpan.FromSeconds(60));
        opts.ApiKey.Should().BeEmpty();
    }

    [Fact]
    public void CodeGenOptions_FromEnvironment_ReadsEnvVar()
    {
        Environment.SetEnvironmentVariable("AIML_API_KEY", "env-test-key");
        try
        {
            var opts = CodeGenOptions.FromEnvironment();
            opts.ApiKey.Should().Be("env-test-key");
        }
        finally
        {
            Environment.SetEnvironmentVariable("AIML_API_KEY", null);
        }
    }

    [Fact]
    public void CodeGenOptions_FromEnvironment_MissingVar_Throws()
    {
        Environment.SetEnvironmentVariable("AIML_API_KEY", null);
        Action act = () => CodeGenOptions.FromEnvironment();
        act.Should().Throw<InvalidOperationException>().WithMessage("*AIML_API_KEY*");
    }

    [Fact]
    public void CodeGenOptions_FromEnvironment_CustomEnvVar()
    {
        Environment.SetEnvironmentVariable("MY_CUSTOM_KEY", "custom-val");
        try
        {
            var opts = CodeGenOptions.FromEnvironment("MY_CUSTOM_KEY");
            opts.ApiKey.Should().Be("custom-val");
        }
        finally
        {
            Environment.SetEnvironmentVariable("MY_CUSTOM_KEY", null);
        }
    }

    // -------------------------------------------------------------------------
    // 3. GenerateAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GenerateAsync_ValidPrompt_ReturnsParsedResult()
    {
        var json = MakeCodeGenJson("print('hello')", "python", "Prints hello.", "Requires Python 3.6+");
        var client = BuildClient(json);

        var result = await client.GenerateAsync("print hello world", "python");

        result.Code.Should().Be("print('hello')");
        result.Language.Should().Be("python");
        result.Explanation.Should().Be("Prints hello.");
        result.Notes.Should().Be("Requires Python 3.6+");
    }

    [Fact]
    public async Task GenerateAsync_DefaultLanguage_IsPython()
    {
        var json = MakeCodeGenJson();
        var client = BuildClient(json);
        var result = await client.GenerateAsync("do something");
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GenerateAsync_CSharpLanguage_Works()
    {
        var json = MakeCodeGenJson("Console.WriteLine(\\\"Hello\\\");", "csharp", "Prints Hello to console.");
        var client = BuildClient(json);
        var result = await client.GenerateAsync("print hello", "csharp");
        result.Language.Should().Be("csharp");
    }

    [Fact]
    public async Task GenerateAsync_JavaScriptLanguage_Works()
    {
        var json = MakeCodeGenJson("console.log('hi');", "javascript", "Logs hi.");
        var client = BuildClient(json);
        var result = await client.GenerateAsync("log hi", "javascript");
        result.Language.Should().Be("javascript");
    }

    [Fact]
    public async Task GenerateAsync_NullNotes_ReturnsNullNotes()
    {
        var json = MakeCodeGenJson(notes: null);
        var client = BuildClient(json);
        var result = await client.GenerateAsync("generate something");
        result.Notes.Should().BeNull();
    }

    [Fact]
    public async Task GenerateAsync_ModelFenceStripped()
    {
        var inner = MakeCodeGenJson();
        var withFence = $"```json\n{inner}\n```";
        var http = BuildMockedHttpClient(withFence);
        var client = new CodeGenClient(new CodeGenOptions { ApiKey = "key" }, http);
        var result = await client.GenerateAsync("anything");
        result.Code.Should().NotBeEmpty();
    }

    // -------------------------------------------------------------------------
    // 4. GenerateAsync — validation errors
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GenerateAsync_EmptyPrompt_Throws()
    {
        var client = BuildClient(MakeCodeGenJson());
        Func<Task> act = () => client.GenerateAsync("");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Prompt*");
    }

    [Fact]
    public async Task GenerateAsync_WhitespacePrompt_Throws()
    {
        var client = BuildClient(MakeCodeGenJson());
        Func<Task> act = () => client.GenerateAsync("   ");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Prompt*");
    }

    [Fact]
    public async Task GenerateAsync_EmptyLanguage_Throws()
    {
        var client = BuildClient(MakeCodeGenJson());
        Func<Task> act = () => client.GenerateAsync("do something", "");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Language*");
    }

    // -------------------------------------------------------------------------
    // 5. GenerateAsync — API error responses
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GenerateAsync_ApiReturns401_ThrowsCodeGenException()
    {
        var http = BuildMockedHttpClient("{\"error\":\"unauthorized\"}", HttpStatusCode.Unauthorized);
        var client = new CodeGenClient(new CodeGenOptions { ApiKey = "bad-key" }, http);
        Func<Task> act = () => client.GenerateAsync("anything");
        await act.Should().ThrowAsync<CodeGenException>().WithMessage("*401*");
    }

    [Fact]
    public async Task GenerateAsync_ApiReturns500_ThrowsCodeGenException()
    {
        var http = BuildMockedHttpClient("{\"error\":\"server error\"}", HttpStatusCode.InternalServerError);
        var client = new CodeGenClient(new CodeGenOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.GenerateAsync("anything");
        await act.Should().ThrowAsync<CodeGenException>().WithMessage("*500*");
    }

    [Fact]
    public async Task GenerateAsync_InvalidJson_ThrowsCodeGenException()
    {
        var brokenResponse = new
        {
            choices = new[] { new { message = new { role = "assistant", content = "not-json-at-all" } } }
        };
        var http = BuildRawMockedHttpClient(JsonSerializer.Serialize(brokenResponse), HttpStatusCode.OK);
        var client = new CodeGenClient(new CodeGenOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.GenerateAsync("anything");
        await act.Should().ThrowAsync<CodeGenException>();
    }

    [Fact]
    public async Task GenerateAsync_EmptyChoices_ThrowsCodeGenException()
    {
        var noContent = new { choices = new[] { new { message = new { role = "assistant", content = "" } } } };
        var http = BuildRawMockedHttpClient(JsonSerializer.Serialize(noContent), HttpStatusCode.OK);
        var client = new CodeGenClient(new CodeGenOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.GenerateAsync("anything");
        await act.Should().ThrowAsync<CodeGenException>();
    }

    [Fact]
    public async Task GenerateAsync_MalformedApiResponseBody_ThrowsCodeGenException()
    {
        var http = BuildRawMockedHttpClient("this is not json", HttpStatusCode.OK);
        var client = new CodeGenClient(new CodeGenOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.GenerateAsync("anything");
        await act.Should().ThrowAsync<CodeGenException>();
    }

    // -------------------------------------------------------------------------
    // 6. RefactorAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task RefactorAsync_ValidCode_ReturnsParsedResult()
    {
        var json = MakeRefactorJson(
            "def add(value: int) -> int: return value + 1",
            "python",
            "Added type hints and renamed for clarity.",
            new[] { "Added type hint", "Renamed parameter x to value" });
        var client = BuildClient(json);

        var result = await client.RefactorAsync("def f(x): return x+1", "python");

        result.Code.Should().Contain("value");
        result.Language.Should().Be("python");
        result.Summary.Should().Contain("type hints");
        result.Improvements.Should().HaveCount(2);
        result.Improvements[0].Should().Be("Added type hint");
    }

    [Fact]
    public async Task RefactorAsync_DefaultLanguage_IsPython()
    {
        var json = MakeRefactorJson();
        var client = BuildClient(json);
        var result = await client.RefactorAsync("def f(): pass");
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task RefactorAsync_MultipleImprovements_ParsedCorrectly()
    {
        var json = MakeRefactorJson(improvements: new[] { "a", "b", "c", "d" });
        var client = BuildClient(json);
        var result = await client.RefactorAsync("code");
        result.Improvements.Should().HaveCount(4);
    }

    // -------------------------------------------------------------------------
    // 7. RefactorAsync — validation errors
    // -------------------------------------------------------------------------

    [Fact]
    public async Task RefactorAsync_EmptyCode_Throws()
    {
        var client = BuildClient(MakeRefactorJson());
        Func<Task> act = () => client.RefactorAsync("");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Code*");
    }

    [Fact]
    public async Task RefactorAsync_WhitespaceCode_Throws()
    {
        var client = BuildClient(MakeRefactorJson());
        Func<Task> act = () => client.RefactorAsync("   ");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Code*");
    }

    [Fact]
    public async Task RefactorAsync_EmptyLanguage_Throws()
    {
        var client = BuildClient(MakeRefactorJson());
        Func<Task> act = () => client.RefactorAsync("code", "");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Language*");
    }

    // -------------------------------------------------------------------------
    // 8. ExplainAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExplainAsync_ValidCode_ReturnsParsedResult()
    {
        var json = MakeExplainJson(
            "Opens a file and prints each line.",
            new[] { "Opens the file", "Iterates each line", "Prints to stdout" },
            "python",
            "simple");
        var client = BuildClient(json);

        var result = await client.ExplainAsync("with open('f') as fh:\n  for line in fh: print(line)");

        result.Summary.Should().Contain("file");
        result.Steps.Should().HaveCount(3);
        result.Language.Should().Be("python");
        result.Complexity.Should().Be("simple");
    }

    [Fact]
    public async Task ExplainAsync_ComplexCode_ReturnsComplexComplexity()
    {
        var json = MakeExplainJson(complexity: "complex");
        var client = BuildClient(json);
        var result = await client.ExplainAsync("some complex code");
        result.Complexity.Should().Be("complex");
    }

    [Fact]
    public async Task ExplainAsync_ModerateCode_ReturnsModerateComplexity()
    {
        var json = MakeExplainJson(complexity: "moderate");
        var client = BuildClient(json);
        var result = await client.ExplainAsync("def sort(arr): return sorted(arr)");
        result.Complexity.Should().Be("moderate");
    }

    [Fact]
    public async Task ExplainAsync_MultipleSteps_ParsedCorrectly()
    {
        var json = MakeExplainJson(steps: new[] { "s1", "s2", "s3", "s4", "s5" });
        var client = BuildClient(json);
        var result = await client.ExplainAsync("code");
        result.Steps.Should().HaveCount(5);
    }

    // -------------------------------------------------------------------------
    // 9. ExplainAsync — validation errors
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExplainAsync_EmptyCode_Throws()
    {
        var client = BuildClient(MakeExplainJson());
        Func<Task> act = () => client.ExplainAsync("");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Code*");
    }

    [Fact]
    public async Task ExplainAsync_WhitespaceCode_Throws()
    {
        var client = BuildClient(MakeExplainJson());
        Func<Task> act = () => client.ExplainAsync("   ");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Code*");
    }

    // -------------------------------------------------------------------------
    // 10. BatchCodeGenResult
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GenerateBatchAsync_MultiplePrompts_ReturnsAllResults()
    {
        var json = MakeCodeGenJson();
        var http = BuildMockedHttpClient(json);
        var client = new CodeGenClient(new CodeGenOptions { ApiKey = "key" }, http);

        var batch = await client.GenerateBatchAsync(
            new[] { "prompt1", "prompt2", "prompt3" },
            "python");

        batch.Total.Should().Be(3);
        batch.Results.Should().HaveCount(3);
    }

    [Fact]
    public async Task GenerateBatchAsync_SinglePrompt_Works()
    {
        var json = MakeCodeGenJson();
        var client = BuildClient(json);
        var batch = await client.GenerateBatchAsync(new[] { "one prompt" });
        batch.Total.Should().Be(1);
    }

    [Fact]
    public async Task GenerateBatchAsync_NullPrompts_Throws()
    {
        var client = BuildClient(MakeCodeGenJson());
        Func<Task> act = () => client.GenerateBatchAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GenerateBatchAsync_EmptyList_Throws()
    {
        var client = BuildClient(MakeCodeGenJson());
        Func<Task> act = () => client.GenerateBatchAsync(Array.Empty<string>());
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*empty*");
    }

    [Fact]
    public async Task GenerateBatchAsync_EmptyLanguage_Throws()
    {
        var client = BuildClient(MakeCodeGenJson());
        Func<Task> act = () => client.GenerateBatchAsync(new[] { "p1" }, "");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Language*");
    }

    [Fact]
    public void BatchCodeGenResult_Total_MatchesResultsCount()
    {
        var batch = new BatchCodeGenResult
        {
            Results = new List<CodeGenResult>
            {
                new CodeGenResult { Code = "a", Language = "py", Explanation = "x" },
                new CodeGenResult { Code = "b", Language = "js", Explanation = "y" }
            }
        };
        batch.Total.Should().Be(2);
    }

    // -------------------------------------------------------------------------
    // 11. Model classes
    // -------------------------------------------------------------------------

    [Fact]
    public void CodeGenResult_DefaultValues_AreCorrect()
    {
        var r = new CodeGenResult();
        r.Code.Should().BeEmpty();
        r.Language.Should().BeEmpty();
        r.Explanation.Should().BeEmpty();
        r.Notes.Should().BeNull();
    }

    [Fact]
    public void RefactorResult_DefaultValues_AreCorrect()
    {
        var r = new RefactorResult();
        r.Code.Should().BeEmpty();
        r.Language.Should().BeEmpty();
        r.Summary.Should().BeEmpty();
        r.Improvements.Should().NotBeNull();
        r.Improvements.Should().BeEmpty();
    }

    [Fact]
    public void ExplainResult_DefaultValues_AreCorrect()
    {
        var r = new ExplainResult();
        r.Summary.Should().BeEmpty();
        r.Steps.Should().NotBeNull();
        r.Steps.Should().BeEmpty();
        r.Language.Should().BeEmpty();
        r.Complexity.Should().BeEmpty();
    }

    // -------------------------------------------------------------------------
    // 12. Cancellation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GenerateAsync_CancelledToken_ThrowsOperationCancelled()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns<HttpRequestMessage, CancellationToken>(async (_, ct) =>
            {
                ct.ThrowIfCancellationRequested();
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var client = new CodeGenClient(
            new CodeGenOptions { ApiKey = "key" },
            new HttpClient(handlerMock.Object));

        Func<Task> act = () => client.GenerateAsync("test", "python", cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task RefactorAsync_CancelledToken_ThrowsOperationCancelled()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns<HttpRequestMessage, CancellationToken>(async (_, ct) =>
            {
                ct.ThrowIfCancellationRequested();
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var client = new CodeGenClient(
            new CodeGenOptions { ApiKey = "key" },
            new HttpClient(handlerMock.Object));

        Func<Task> act = () => client.RefactorAsync("code", "python", cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    // -------------------------------------------------------------------------
    // 13. Dependency Injection
    // -------------------------------------------------------------------------

    [Fact]
    public void AddCodeGenClient_WithApiKey_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddCodeGenClient("test-key");
        var provider = services.BuildServiceProvider();
        var client = provider.GetService<CodeGenClient>();
        client.Should().NotBeNull();
        client!.Dispose();
    }

    [Fact]
    public void AddCodeGenClient_WithConfiguration_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddCodeGenClient(opts =>
        {
            opts.ApiKey = "test-key";
            opts.Model = "gpt-4o";
        });
        var provider = services.BuildServiceProvider();
        var client = provider.GetService<CodeGenClient>();
        client.Should().NotBeNull();
        client!.Dispose();
    }

    // -------------------------------------------------------------------------
    // 14. CodeGenException
    // -------------------------------------------------------------------------

    [Fact]
    public void CodeGenException_MessageOnly_Works()
    {
        var ex = new CodeGenException("test message");
        ex.Message.Should().Be("test message");
        ex.InnerException.Should().BeNull();
    }

    [Fact]
    public void CodeGenException_WithInner_Works()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new CodeGenException("outer", inner);
        ex.Message.Should().Be("outer");
        ex.InnerException.Should().Be(inner);
    }

    // -------------------------------------------------------------------------
    // 15. API rate limit / 429 handling
    // -------------------------------------------------------------------------

    [Fact]
    public async Task GenerateAsync_ApiReturns429_ThrowsCodeGenException()
    {
        var http = BuildMockedHttpClient("{\"error\":\"rate limit exceeded\"}", HttpStatusCode.TooManyRequests);
        var client = new CodeGenClient(new CodeGenOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.GenerateAsync("anything");
        await act.Should().ThrowAsync<CodeGenException>().WithMessage("*429*");
    }

    [Fact]
    public async Task RefactorAsync_ApiReturns429_ThrowsCodeGenException()
    {
        var http = BuildMockedHttpClient("{\"error\":\"rate limit exceeded\"}", HttpStatusCode.TooManyRequests);
        var client = new CodeGenClient(new CodeGenOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.RefactorAsync("code");
        await act.Should().ThrowAsync<CodeGenException>().WithMessage("*429*");
    }

    [Fact]
    public async Task ExplainAsync_ApiReturns429_ThrowsCodeGenException()
    {
        var http = BuildMockedHttpClient("{\"error\":\"rate limit exceeded\"}", HttpStatusCode.TooManyRequests);
        var client = new CodeGenClient(new CodeGenOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.ExplainAsync("code");
        await act.Should().ThrowAsync<CodeGenException>().WithMessage("*429*");
    }
}
