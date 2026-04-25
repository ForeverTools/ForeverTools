using System.Net;
using System.Text;
using System.Text.Json;
using ForeverTools.EmailAI;
using ForeverTools.EmailAI.Extensions;
using ForeverTools.EmailAI.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace ForeverTools.EmailAI.Tests;

/// <summary>
/// Unit tests for EmailAIClient. All HTTP calls are mocked — no real API calls are made.
/// </summary>
public class EmailAIClientTests
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

    private static EmailAIClient BuildClient(string contentJson, HttpStatusCode code = HttpStatusCode.OK)
    {
        var http = BuildMockedHttpClient(contentJson, code);
        var opts = new EmailAIOptions { ApiKey = "test-key" };
        return new EmailAIClient(opts, http);
    }

    private static string MakeComposeJson(
        string body = "Dear Team, Please find the report attached. Best regards",
        string subject = "Q3 Report",
        string tone = "professional",
        string? notes = null)
    {
        var notesVal = notes != null ? $"\"{notes}\"" : "null";
        return $"{{\"body\":\"{body}\",\"subject\":\"{subject}\",\"tone\":\"{tone}\",\"notes\":{notesVal}}}";
    }

    private static string MakeReplyJson(
        string body = "Thank you for your email. I will look into this.",
        string tone = "professional",
        string? notes = null)
    {
        var notesVal = notes != null ? $"\"{notes}\"" : "null";
        return $"{{\"body\":\"{body}\",\"tone\":\"{tone}\",\"notes\":{notesVal}}}";
    }

    private static string MakeSummarizeJson(
        string summary = "Project status update covering Q3 milestones.",
        string[]? actionItems = null,
        string topic = "Project Update")
    {
        var items = actionItems ?? new[] { "Review attached report", "Schedule follow-up" };
        var itemsJson = "[" + string.Join(",", items.Select(i => $"\"{i}\"")) + "]";
        return $"{{\"summary\":\"{summary}\",\"action_items\":{itemsJson},\"topic\":\"{topic}\"}}";
    }

    private static string MakeClassifyJson(
        string category = "normal",
        string priority = "medium",
        string sentiment = "neutral",
        string rationale = "Standard business communication with no urgency indicators.")
    {
        return $"{{\"category\":\"{category}\",\"priority\":\"{priority}\",\"sentiment\":\"{sentiment}\",\"rationale\":\"{rationale}\"}}";
    }

    private static EmailComposeRequest MakeComposeRequest(
        string subject = "Meeting Tomorrow",
        string context = "Remind the team about the 10am standup",
        string tone = "friendly") =>
        new() { Subject = subject, Context = context, Tone = tone, Recipients = new List<string> { "team@example.com" } };

    private static EmailReplyRequest MakeReplyRequest(
        string original = "Hi, can you confirm the meeting time?",
        string replyContext = "Confirm 10am works fine",
        string tone = "professional") =>
        new() { OriginalEmail = original, ReplyContext = replyContext, Tone = tone };

    // -------------------------------------------------------------------------
    // 1. Constructor validation
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_NullOptions_Throws()
    {
        Action act = () => new EmailAIClient(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_EmptyApiKey_Throws()
    {
        Action act = () => new EmailAIClient(new EmailAIOptions { ApiKey = "" });
        act.Should().Throw<ArgumentException>().WithMessage("*API key*");
    }

    [Fact]
    public void Constructor_WhitespaceApiKey_Throws()
    {
        Action act = () => new EmailAIClient(new EmailAIOptions { ApiKey = "   " });
        act.Should().Throw<ArgumentException>().WithMessage("*API key*");
    }

    [Fact]
    public void Constructor_EmptyModel_Throws()
    {
        Action act = () => new EmailAIClient(new EmailAIOptions { ApiKey = "key", Model = "" });
        act.Should().Throw<ArgumentException>().WithMessage("*Model*");
    }

    [Fact]
    public void Constructor_InvalidBaseUrl_Throws()
    {
        Action act = () => new EmailAIClient(new EmailAIOptions { ApiKey = "key", BaseUrl = "not-a-url" });
        act.Should().Throw<ArgumentException>().WithMessage("*BaseUrl*");
    }

    [Fact]
    public void Constructor_ValidOptions_CreatesInstance()
    {
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" });
        client.Should().NotBeNull();
        client.Dispose();
    }

    [Fact]
    public void Constructor_WithExternalHttpClient_DoesNotOwnIt()
    {
        var http = new HttpClient();
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        client.Should().NotBeNull();
        client.Dispose();
        http.Dispose(); // should still be usable
    }

    // -------------------------------------------------------------------------
    // 2. EmailAIOptions
    // -------------------------------------------------------------------------

    [Fact]
    public void EmailAIOptions_Defaults_AreCorrect()
    {
        var opts = new EmailAIOptions();
        opts.BaseUrl.Should().Be(EmailAIOptions.DefaultBaseUrl);
        opts.Model.Should().Be(EmailAIOptions.DefaultModel);
        opts.Timeout.Should().Be(TimeSpan.FromSeconds(60));
        opts.ApiKey.Should().BeEmpty();
    }

    [Fact]
    public void EmailAIOptions_DefaultBaseUrl_IsCorrect()
    {
        EmailAIOptions.DefaultBaseUrl.Should().Be("https://api.aimlapi.com");
    }

    [Fact]
    public void EmailAIOptions_DefaultModel_IsGpt4oMini()
    {
        EmailAIOptions.DefaultModel.Should().Be("gpt-4o-mini");
    }

    [Fact]
    public void EmailAIOptions_FromEnvironment_ReadsEnvVar()
    {
        Environment.SetEnvironmentVariable("AIML_API_KEY", "env-test-key");
        try
        {
            var opts = EmailAIOptions.FromEnvironment();
            opts.ApiKey.Should().Be("env-test-key");
        }
        finally
        {
            Environment.SetEnvironmentVariable("AIML_API_KEY", null);
        }
    }

    [Fact]
    public void EmailAIOptions_FromEnvironment_MissingVar_Throws()
    {
        Environment.SetEnvironmentVariable("AIML_API_KEY", null);
        Action act = () => EmailAIOptions.FromEnvironment();
        act.Should().Throw<InvalidOperationException>().WithMessage("*AIML_API_KEY*");
    }

    [Fact]
    public void EmailAIOptions_FromEnvironment_CustomEnvVar()
    {
        Environment.SetEnvironmentVariable("MY_EMAIL_KEY", "custom-val");
        try
        {
            var opts = EmailAIOptions.FromEnvironment("MY_EMAIL_KEY");
            opts.ApiKey.Should().Be("custom-val");
        }
        finally
        {
            Environment.SetEnvironmentVariable("MY_EMAIL_KEY", null);
        }
    }

    // -------------------------------------------------------------------------
    // 3. ComposeAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ComposeAsync_ValidRequest_ReturnsParsedResult()
    {
        var json = MakeComposeJson("Dear Team Standup at 10am. Best", "Meeting Tomorrow", "friendly");
        var client = BuildClient(json);

        var result = await client.ComposeAsync(MakeComposeRequest());

        result.Body.Should().Contain("Standup");
        result.Subject.Should().Be("Meeting Tomorrow");
        result.Tone.Should().Be("friendly");
        result.Notes.Should().BeNull();
    }

    [Fact]
    public async Task ComposeAsync_WithNotes_ParsesNotes()
    {
        var json = MakeComposeJson(notes: "Consider adding sender name for personalisation.");
        var client = BuildClient(json);
        var result = await client.ComposeAsync(MakeComposeRequest());
        result.Notes.Should().Contain("personalisation");
    }

    [Fact]
    public async Task ComposeAsync_WithNoRecipients_StillWorks()
    {
        var json = MakeComposeJson();
        var client = BuildClient(json);
        var req = new EmailComposeRequest { Subject = "Hello", Context = "Just saying hi", Tone = "casual" };
        var result = await client.ComposeAsync(req);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ComposeAsync_ModelFenceStripped()
    {
        var inner = MakeComposeJson();
        var withFence = $"```json\n{inner}\n```";
        var http = BuildMockedHttpClient(withFence);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        var result = await client.ComposeAsync(MakeComposeRequest());
        result.Body.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ComposeAsync_MultipleRecipients_Works()
    {
        var json = MakeComposeJson();
        var client = BuildClient(json);
        var req = new EmailComposeRequest
        {
            Subject = "Announcement",
            Context = "New product launch next week",
            Tone = "professional",
            Recipients = new List<string> { "alice@example.com", "bob@example.com", "carol@example.com" }
        };
        var result = await client.ComposeAsync(req);
        result.Should().NotBeNull();
    }

    // -------------------------------------------------------------------------
    // 4. ComposeAsync — validation errors
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ComposeAsync_NullRequest_Throws()
    {
        var client = BuildClient(MakeComposeJson());
        Func<Task> act = () => client.ComposeAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ComposeAsync_EmptySubject_Throws()
    {
        var client = BuildClient(MakeComposeJson());
        var req = MakeComposeRequest(subject: "");
        Func<Task> act = () => client.ComposeAsync(req);
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Subject*");
    }

    [Fact]
    public async Task ComposeAsync_WhitespaceSubject_Throws()
    {
        var client = BuildClient(MakeComposeJson());
        var req = MakeComposeRequest(subject: "   ");
        Func<Task> act = () => client.ComposeAsync(req);
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Subject*");
    }

    [Fact]
    public async Task ComposeAsync_EmptyContext_Throws()
    {
        var client = BuildClient(MakeComposeJson());
        var req = MakeComposeRequest(context: "");
        Func<Task> act = () => client.ComposeAsync(req);
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Context*");
    }

    [Fact]
    public async Task ComposeAsync_EmptyTone_Throws()
    {
        var client = BuildClient(MakeComposeJson());
        var req = MakeComposeRequest(tone: "");
        Func<Task> act = () => client.ComposeAsync(req);
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Tone*");
    }

    // -------------------------------------------------------------------------
    // 5. ReplyAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ReplyAsync_ValidRequest_ReturnsParsedResult()
    {
        var json = MakeReplyJson("Thank you for reaching out. The 10am slot works perfectly.", "professional");
        var client = BuildClient(json);

        var result = await client.ReplyAsync(MakeReplyRequest());

        result.Body.Should().Contain("10am");
        result.Tone.Should().Be("professional");
        result.Notes.Should().BeNull();
    }

    [Fact]
    public async Task ReplyAsync_WithNotes_ParsesNotes()
    {
        var json = MakeReplyJson(notes: "Kept the reply concise as requested.");
        var client = BuildClient(json);
        var result = await client.ReplyAsync(MakeReplyRequest());
        result.Notes.Should().Contain("concise");
    }

    [Fact]
    public async Task ReplyAsync_FriendlyTone_Works()
    {
        var json = MakeReplyJson(tone: "friendly");
        var client = BuildClient(json);
        var req = MakeReplyRequest(tone: "friendly");
        var result = await client.ReplyAsync(req);
        result.Tone.Should().Be("friendly");
    }

    [Fact]
    public async Task ReplyAsync_ModelFenceStripped()
    {
        var inner = MakeReplyJson();
        var withFence = $"```json\n{inner}\n```";
        var http = BuildMockedHttpClient(withFence);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        var result = await client.ReplyAsync(MakeReplyRequest());
        result.Body.Should().NotBeEmpty();
    }

    // -------------------------------------------------------------------------
    // 6. ReplyAsync — validation errors
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ReplyAsync_NullRequest_Throws()
    {
        var client = BuildClient(MakeReplyJson());
        Func<Task> act = () => client.ReplyAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ReplyAsync_EmptyOriginalEmail_Throws()
    {
        var client = BuildClient(MakeReplyJson());
        var req = MakeReplyRequest(original: "");
        Func<Task> act = () => client.ReplyAsync(req);
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*OriginalEmail*");
    }

    [Fact]
    public async Task ReplyAsync_EmptyReplyContext_Throws()
    {
        var client = BuildClient(MakeReplyJson());
        var req = MakeReplyRequest(replyContext: "");
        Func<Task> act = () => client.ReplyAsync(req);
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*ReplyContext*");
    }

    [Fact]
    public async Task ReplyAsync_EmptyTone_Throws()
    {
        var client = BuildClient(MakeReplyJson());
        var req = MakeReplyRequest(tone: "");
        Func<Task> act = () => client.ReplyAsync(req);
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Tone*");
    }

    // -------------------------------------------------------------------------
    // 7. SummarizeAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SummarizeAsync_ValidBody_ReturnsParsedResult()
    {
        var json = MakeSummarizeJson(
            "Project Q3 report is attached. Key milestones were met.",
            new[] { "Review Q3 report", "Approve budget" },
            "Q3 Project Update");
        var client = BuildClient(json);

        var result = await client.SummarizeAsync("Please find the Q3 report attached for your review.");

        result.Summary.Should().Contain("Q3");
        result.ActionItems.Should().HaveCount(2);
        result.ActionItems[0].Should().Be("Review Q3 report");
        result.Topic.Should().Be("Q3 Project Update");
    }

    [Fact]
    public async Task SummarizeAsync_NoActionItems_ReturnsEmptyList()
    {
        var json = MakeSummarizeJson(actionItems: Array.Empty<string>());
        var client = BuildClient(json);
        var result = await client.SummarizeAsync("Just a friendly hello.");
        result.ActionItems.Should().BeEmpty();
    }

    [Fact]
    public async Task SummarizeAsync_ManyActionItems_ParsedCorrectly()
    {
        var json = MakeSummarizeJson(actionItems: new[] { "a1", "a2", "a3", "a4", "a5" });
        var client = BuildClient(json);
        var result = await client.SummarizeAsync("Long email with many tasks.");
        result.ActionItems.Should().HaveCount(5);
    }

    [Fact]
    public async Task SummarizeAsync_ModelFenceStripped()
    {
        var inner = MakeSummarizeJson();
        var withFence = $"```json\n{inner}\n```";
        var http = BuildMockedHttpClient(withFence);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        var result = await client.SummarizeAsync("Some email body.");
        result.Summary.Should().NotBeEmpty();
    }

    // -------------------------------------------------------------------------
    // 8. SummarizeAsync — validation errors
    // -------------------------------------------------------------------------

    [Fact]
    public async Task SummarizeAsync_EmptyBody_Throws()
    {
        var client = BuildClient(MakeSummarizeJson());
        Func<Task> act = () => client.SummarizeAsync("");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*EmailBody*");
    }

    [Fact]
    public async Task SummarizeAsync_WhitespaceBody_Throws()
    {
        var client = BuildClient(MakeSummarizeJson());
        Func<Task> act = () => client.SummarizeAsync("   ");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*EmailBody*");
    }

    // -------------------------------------------------------------------------
    // 9. ClassifyAsync — happy path
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ClassifyAsync_ValidBody_ReturnsParsedResult()
    {
        var json = MakeClassifyJson("urgent", "high", "negative", "Customer threatening to cancel subscription.");
        var client = BuildClient(json);

        var result = await client.ClassifyAsync("I am very disappointed and will cancel if this isn't fixed today.");

        result.Category.Should().Be("urgent");
        result.Priority.Should().Be("high");
        result.Sentiment.Should().Be("negative");
        result.Rationale.Should().Contain("cancel");
    }

    [Fact]
    public async Task ClassifyAsync_SpamEmail_ReturnsSpamCategory()
    {
        var json = MakeClassifyJson("spam", "low", "neutral", "Promotional bulk email.");
        var client = BuildClient(json);
        var result = await client.ClassifyAsync("You have won a million dollars! Click here now!");
        result.Category.Should().Be("spam");
        result.Priority.Should().Be("low");
    }

    [Fact]
    public async Task ClassifyAsync_NewsletterEmail_ReturnsNewsletterCategory()
    {
        var json = MakeClassifyJson("newsletter", "low", "positive", "Regular newsletter.");
        var client = BuildClient(json);
        var result = await client.ClassifyAsync("This week in .NET: new features and community news.");
        result.Category.Should().Be("newsletter");
    }

    [Fact]
    public async Task ClassifyAsync_SupportEmail_ReturnsSupportCategory()
    {
        var json = MakeClassifyJson("support", "medium", "neutral", "User requesting technical help.");
        var client = BuildClient(json);
        var result = await client.ClassifyAsync("I cannot log into my account, please help.");
        result.Category.Should().Be("support");
    }

    [Fact]
    public async Task ClassifyAsync_SalesEmail_ReturnsSalesCategory()
    {
        var json = MakeClassifyJson("sales", "medium", "positive", "Sales outreach email.");
        var client = BuildClient(json);
        var result = await client.ClassifyAsync("Hi, I'd like to discuss how our software can help your team.");
        result.Category.Should().Be("sales");
    }

    [Fact]
    public async Task ClassifyAsync_NormalEmail_ReturnsNormalCategory()
    {
        var json = MakeClassifyJson("normal", "medium", "neutral", "Routine business communication.");
        var client = BuildClient(json);
        var result = await client.ClassifyAsync("Please see the attached agenda for tomorrow's meeting.");
        result.Category.Should().Be("normal");
    }

    [Fact]
    public async Task ClassifyAsync_PositiveSentiment_Works()
    {
        var json = MakeClassifyJson(sentiment: "positive");
        var client = BuildClient(json);
        var result = await client.ClassifyAsync("Great news! The project was approved.");
        result.Sentiment.Should().Be("positive");
    }

    [Fact]
    public async Task ClassifyAsync_ModelFenceStripped()
    {
        var inner = MakeClassifyJson();
        var withFence = $"```json\n{inner}\n```";
        var http = BuildMockedHttpClient(withFence);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        var result = await client.ClassifyAsync("Any email body here.");
        result.Category.Should().NotBeEmpty();
    }

    // -------------------------------------------------------------------------
    // 10. ClassifyAsync — validation errors
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ClassifyAsync_EmptyBody_Throws()
    {
        var client = BuildClient(MakeClassifyJson());
        Func<Task> act = () => client.ClassifyAsync("");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*EmailBody*");
    }

    [Fact]
    public async Task ClassifyAsync_WhitespaceBody_Throws()
    {
        var client = BuildClient(MakeClassifyJson());
        Func<Task> act = () => client.ClassifyAsync("   ");
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*EmailBody*");
    }

    // -------------------------------------------------------------------------
    // 11. API error handling
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ComposeAsync_ApiReturns401_ThrowsEmailAIException()
    {
        var http = BuildMockedHttpClient("{\"error\":\"unauthorized\"}", HttpStatusCode.Unauthorized);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "bad-key" }, http);
        Func<Task> act = () => client.ComposeAsync(MakeComposeRequest());
        await act.Should().ThrowAsync<EmailAIException>().WithMessage("*401*");
    }

    [Fact]
    public async Task ComposeAsync_ApiReturns500_ThrowsEmailAIException()
    {
        var http = BuildMockedHttpClient("{\"error\":\"server error\"}", HttpStatusCode.InternalServerError);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.ComposeAsync(MakeComposeRequest());
        await act.Should().ThrowAsync<EmailAIException>().WithMessage("*500*");
    }

    [Fact]
    public async Task ComposeAsync_ApiReturns429_ThrowsEmailAIException()
    {
        var http = BuildMockedHttpClient("{\"error\":\"rate limit\"}", HttpStatusCode.TooManyRequests);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.ComposeAsync(MakeComposeRequest());
        await act.Should().ThrowAsync<EmailAIException>().WithMessage("*429*");
    }

    [Fact]
    public async Task ReplyAsync_ApiReturns429_ThrowsEmailAIException()
    {
        var http = BuildMockedHttpClient("{\"error\":\"rate limit\"}", HttpStatusCode.TooManyRequests);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.ReplyAsync(MakeReplyRequest());
        await act.Should().ThrowAsync<EmailAIException>().WithMessage("*429*");
    }

    [Fact]
    public async Task SummarizeAsync_ApiReturns429_ThrowsEmailAIException()
    {
        var http = BuildMockedHttpClient("{\"error\":\"rate limit\"}", HttpStatusCode.TooManyRequests);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.SummarizeAsync("some email");
        await act.Should().ThrowAsync<EmailAIException>().WithMessage("*429*");
    }

    [Fact]
    public async Task ClassifyAsync_ApiReturns429_ThrowsEmailAIException()
    {
        var http = BuildMockedHttpClient("{\"error\":\"rate limit\"}", HttpStatusCode.TooManyRequests);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.ClassifyAsync("some email");
        await act.Should().ThrowAsync<EmailAIException>().WithMessage("*429*");
    }

    [Fact]
    public async Task ComposeAsync_InvalidJson_ThrowsEmailAIException()
    {
        var brokenResponse = new
        {
            choices = new[] { new { message = new { role = "assistant", content = "not-json-at-all" } } }
        };
        var http = BuildRawMockedHttpClient(JsonSerializer.Serialize(brokenResponse), HttpStatusCode.OK);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.ComposeAsync(MakeComposeRequest());
        await act.Should().ThrowAsync<EmailAIException>();
    }

    [Fact]
    public async Task ComposeAsync_MalformedApiBody_ThrowsEmailAIException()
    {
        var http = BuildRawMockedHttpClient("this is not json", HttpStatusCode.OK);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.ComposeAsync(MakeComposeRequest());
        await act.Should().ThrowAsync<EmailAIException>();
    }

    [Fact]
    public async Task ComposeAsync_EmptyContentField_ThrowsEmailAIException()
    {
        var noContent = new { choices = new[] { new { message = new { role = "assistant", content = "" } } } };
        var http = BuildRawMockedHttpClient(JsonSerializer.Serialize(noContent), HttpStatusCode.OK);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);
        Func<Task> act = () => client.ComposeAsync(MakeComposeRequest());
        await act.Should().ThrowAsync<EmailAIException>();
    }

    // -------------------------------------------------------------------------
    // 12. BatchComposeAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task BatchComposeAsync_MultipleRequests_ReturnsAllResults()
    {
        var json = MakeComposeJson();
        var http = BuildMockedHttpClient(json);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);

        var requests = new[]
        {
            MakeComposeRequest(subject: "Email 1", context: "First context"),
            MakeComposeRequest(subject: "Email 2", context: "Second context"),
            MakeComposeRequest(subject: "Email 3", context: "Third context")
        };

        var batch = await client.BatchComposeAsync(requests);

        batch.Total.Should().Be(3);
        batch.Results.Should().HaveCount(3);
    }

    [Fact]
    public async Task BatchComposeAsync_SingleRequest_Works()
    {
        var json = MakeComposeJson();
        var client = BuildClient(json);
        var batch = await client.BatchComposeAsync(new[] { MakeComposeRequest() });
        batch.Total.Should().Be(1);
    }

    [Fact]
    public async Task BatchComposeAsync_NullRequests_Throws()
    {
        var client = BuildClient(MakeComposeJson());
        Func<Task> act = () => client.BatchComposeAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task BatchComposeAsync_EmptyList_Throws()
    {
        var client = BuildClient(MakeComposeJson());
        Func<Task> act = () => client.BatchComposeAsync(Array.Empty<EmailComposeRequest>());
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*empty*");
    }

    [Fact]
    public void BatchEmailComposeResult_Total_MatchesResultsCount()
    {
        var batch = new BatchEmailComposeResult
        {
            Results = new List<EmailComposeResult>
            {
                new() { Body = "a", Subject = "s1", Tone = "professional" },
                new() { Body = "b", Subject = "s2", Tone = "friendly" }
            }
        };
        batch.Total.Should().Be(2);
    }

    // -------------------------------------------------------------------------
    // 13. BatchReplyAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task BatchReplyAsync_MultipleRequests_ReturnsAllResults()
    {
        var json = MakeReplyJson();
        var http = BuildMockedHttpClient(json);
        var client = new EmailAIClient(new EmailAIOptions { ApiKey = "key" }, http);

        var requests = new[]
        {
            MakeReplyRequest(original: "Email 1 body", replyContext: "Reply 1"),
            MakeReplyRequest(original: "Email 2 body", replyContext: "Reply 2")
        };

        var batch = await client.BatchReplyAsync(requests);

        batch.Total.Should().Be(2);
        batch.Results.Should().HaveCount(2);
    }

    [Fact]
    public async Task BatchReplyAsync_NullRequests_Throws()
    {
        var client = BuildClient(MakeReplyJson());
        Func<Task> act = () => client.BatchReplyAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task BatchReplyAsync_EmptyList_Throws()
    {
        var client = BuildClient(MakeReplyJson());
        Func<Task> act = () => client.BatchReplyAsync(Array.Empty<EmailReplyRequest>());
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*empty*");
    }

    [Fact]
    public void BatchEmailReplyResult_Total_MatchesResultsCount()
    {
        var batch = new BatchEmailReplyResult
        {
            Results = new List<EmailReplyResult>
            {
                new() { Body = "reply1", Tone = "professional" },
                new() { Body = "reply2", Tone = "friendly" },
                new() { Body = "reply3", Tone = "formal" }
            }
        };
        batch.Total.Should().Be(3);
    }

    // -------------------------------------------------------------------------
    // 14. Model default values
    // -------------------------------------------------------------------------

    [Fact]
    public void EmailComposeResult_DefaultValues_AreCorrect()
    {
        var r = new EmailComposeResult();
        r.Body.Should().BeEmpty();
        r.Subject.Should().BeEmpty();
        r.Tone.Should().BeEmpty();
        r.Notes.Should().BeNull();
    }

    [Fact]
    public void EmailReplyResult_DefaultValues_AreCorrect()
    {
        var r = new EmailReplyResult();
        r.Body.Should().BeEmpty();
        r.Tone.Should().BeEmpty();
        r.Notes.Should().BeNull();
    }

    [Fact]
    public void EmailSummarizeResult_DefaultValues_AreCorrect()
    {
        var r = new EmailSummarizeResult();
        r.Summary.Should().BeEmpty();
        r.ActionItems.Should().NotBeNull();
        r.ActionItems.Should().BeEmpty();
        r.Topic.Should().BeEmpty();
    }

    [Fact]
    public void EmailClassifyResult_DefaultValues_AreCorrect()
    {
        var r = new EmailClassifyResult();
        r.Category.Should().BeEmpty();
        r.Priority.Should().BeEmpty();
        r.Sentiment.Should().BeEmpty();
        r.Rationale.Should().BeEmpty();
    }

    [Fact]
    public void EmailComposeRequest_DefaultTone_IsProfessional()
    {
        var r = new EmailComposeRequest();
        r.Tone.Should().Be("professional");
    }

    [Fact]
    public void EmailReplyRequest_DefaultTone_IsProfessional()
    {
        var r = new EmailReplyRequest();
        r.Tone.Should().Be("professional");
    }

    [Fact]
    public void EmailComposeRequest_DefaultRecipients_IsEmpty()
    {
        var r = new EmailComposeRequest();
        r.Recipients.Should().NotBeNull();
        r.Recipients.Should().BeEmpty();
    }

    // -------------------------------------------------------------------------
    // 15. EmailAIException
    // -------------------------------------------------------------------------

    [Fact]
    public void EmailAIException_MessageOnly_Works()
    {
        var ex = new EmailAIException("test message");
        ex.Message.Should().Be("test message");
        ex.InnerException.Should().BeNull();
    }

    [Fact]
    public void EmailAIException_WithInner_Works()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new EmailAIException("outer", inner);
        ex.Message.Should().Be("outer");
        ex.InnerException.Should().Be(inner);
    }

    [Fact]
    public void EmailAIException_IsException()
    {
        var ex = new EmailAIException("msg");
        ex.Should().BeAssignableTo<Exception>();
    }

    // -------------------------------------------------------------------------
    // 16. Cancellation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ComposeAsync_CancelledToken_ThrowsOperationCancelled()
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

        var client = new EmailAIClient(
            new EmailAIOptions { ApiKey = "key" },
            new HttpClient(handlerMock.Object));

        Func<Task> act = () => client.ComposeAsync(MakeComposeRequest(), cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task SummarizeAsync_CancelledToken_ThrowsOperationCancelled()
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

        var client = new EmailAIClient(
            new EmailAIOptions { ApiKey = "key" },
            new HttpClient(handlerMock.Object));

        Func<Task> act = () => client.SummarizeAsync("email body", cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ClassifyAsync_CancelledToken_ThrowsOperationCancelled()
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

        var client = new EmailAIClient(
            new EmailAIOptions { ApiKey = "key" },
            new HttpClient(handlerMock.Object));

        Func<Task> act = () => client.ClassifyAsync("email body", cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    // -------------------------------------------------------------------------
    // 17. Dependency Injection
    // -------------------------------------------------------------------------

    [Fact]
    public void AddForeverToolsEmailAI_WithApiKey_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsEmailAI("test-key");
        var provider = services.BuildServiceProvider();
        var client = provider.GetService<EmailAIClient>();
        client.Should().NotBeNull();
        client!.Dispose();
    }

    [Fact]
    public void AddForeverToolsEmailAI_WithConfiguration_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddForeverToolsEmailAI(opts =>
        {
            opts.ApiKey = "test-key";
            opts.Model = "gpt-4o";
        });
        var provider = services.BuildServiceProvider();
        var client = provider.GetService<EmailAIClient>();
        client.Should().NotBeNull();
        client!.Dispose();
    }

    [Fact]
    public void AddForeverToolsEmailAI_ReturnsServiceCollection_ForChaining()
    {
        var services = new ServiceCollection();
        var result = services.AddForeverToolsEmailAI("test-key");
        result.Should().BeSameAs(services);
    }
}
