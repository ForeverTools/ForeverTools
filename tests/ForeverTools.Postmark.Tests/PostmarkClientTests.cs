using ForeverTools.Postmark;
using ForeverTools.Postmark.Models;
using Xunit;

namespace ForeverTools.Postmark.Tests;

public class PostmarkClientTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidToken_CreatesClient()
    {
        var client = new PostmarkClient("test-server-token");

        Assert.NotNull(client);
        Assert.Equal("test-server-token", client.Options.ServerToken);
    }

    [Fact]
    public void Constructor_WithOptions_UsesProvidedOptions()
    {
        var options = new PostmarkOptions
        {
            ServerToken = "test-token",
            DefaultFrom = "sender@example.com",
            TimeoutSeconds = 60
        };

        var client = new PostmarkClient(options);

        Assert.Equal("test-token", client.Options.ServerToken);
        Assert.Equal("sender@example.com", client.Options.DefaultFrom);
        Assert.Equal(60, client.Options.TimeoutSeconds);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidToken_ThrowsArgumentException(string? invalidToken)
    {
        var options = new PostmarkOptions { ServerToken = invalidToken };

        Assert.Throws<ArgumentException>(() => new PostmarkClient(options));
    }

    [Fact]
    public void Constructor_WithNullToken_ThrowsArgumentException()
    {
        // String constructor creates new PostmarkOptions with null token
        Assert.Throws<ArgumentException>(() => new PostmarkClient((string)null!));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PostmarkClient((PostmarkOptions)null!));
    }

    #endregion

    #region PostmarkOptions Tests

    [Fact]
    public void PostmarkOptions_HasCorrectDefaults()
    {
        var options = new PostmarkOptions();

        Assert.Null(options.ServerToken);
        Assert.Null(options.AccountToken);
        Assert.Equal("https://api.postmarkapp.com", options.BaseUrl);
        Assert.Null(options.DefaultFrom);
        Assert.Null(options.DefaultReplyTo);
        Assert.Null(options.DefaultMessageStream);
        Assert.Null(options.TrackOpens);
        Assert.Null(options.TrackLinks);
        Assert.Equal(30, options.TimeoutSeconds);
    }

    [Fact]
    public void PostmarkOptions_SectionName_IsCorrect()
    {
        Assert.Equal("Postmark", PostmarkOptions.SectionName);
    }

    [Fact]
    public void PostmarkOptions_HasServerToken_ReturnsTrueWhenSet()
    {
        var options = new PostmarkOptions { ServerToken = "token" };
        Assert.True(options.HasServerToken);
    }

    [Fact]
    public void PostmarkOptions_HasServerToken_ReturnsFalseWhenEmpty()
    {
        var options = new PostmarkOptions();
        Assert.False(options.HasServerToken);
    }

    [Fact]
    public void PostmarkOptions_HasAccountToken_ReturnsTrueWhenSet()
    {
        var options = new PostmarkOptions { AccountToken = "token" };
        Assert.True(options.HasAccountToken);
    }

    [Fact]
    public void PostmarkOptions_HasAccountToken_ReturnsFalseWhenEmpty()
    {
        var options = new PostmarkOptions();
        Assert.False(options.HasAccountToken);
    }

    #endregion

    #region PostmarkEmail Tests

    [Fact]
    public void PostmarkEmail_DefaultValues_AreCorrect()
    {
        var email = new PostmarkEmail();

        Assert.Equal(string.Empty, email.From);
        Assert.Equal(string.Empty, email.To);
        Assert.Null(email.Cc);
        Assert.Null(email.Bcc);
        Assert.Null(email.Subject);
        Assert.Null(email.HtmlBody);
        Assert.Null(email.TextBody);
        Assert.Null(email.ReplyTo);
        Assert.Null(email.Tag);
        Assert.Null(email.TrackOpens);
        Assert.Null(email.TrackLinks);
        Assert.Null(email.Metadata);
        Assert.Null(email.MessageStream);
        Assert.Null(email.Attachments);
        Assert.Null(email.Headers);
    }

    [Fact]
    public void PostmarkEmail_CanSetAllProperties()
    {
        var email = new PostmarkEmail
        {
            From = "sender@example.com",
            To = "recipient@example.com",
            Cc = "cc@example.com",
            Bcc = "bcc@example.com",
            Subject = "Test Subject",
            HtmlBody = "<p>Hello</p>",
            TextBody = "Hello",
            ReplyTo = "reply@example.com",
            Tag = "test-tag",
            TrackOpens = true,
            TrackLinks = LinkTrackingOptions.HtmlAndText,
            MessageStream = MessageStreams.Outbound,
            Metadata = new Dictionary<string, string> { ["key"] = "value" },
            Attachments = new List<PostmarkAttachment>(),
            Headers = new List<PostmarkHeader>()
        };

        Assert.Equal("sender@example.com", email.From);
        Assert.Equal("recipient@example.com", email.To);
        Assert.Equal("cc@example.com", email.Cc);
        Assert.Equal("bcc@example.com", email.Bcc);
        Assert.Equal("Test Subject", email.Subject);
        Assert.Equal("<p>Hello</p>", email.HtmlBody);
        Assert.Equal("Hello", email.TextBody);
        Assert.Equal("reply@example.com", email.ReplyTo);
        Assert.Equal("test-tag", email.Tag);
        Assert.True(email.TrackOpens);
        Assert.Equal("HtmlAndText", email.TrackLinks);
        Assert.Equal("outbound", email.MessageStream);
        Assert.NotNull(email.Metadata);
        Assert.NotNull(email.Attachments);
        Assert.NotNull(email.Headers);
    }

    #endregion

    #region PostmarkTemplateEmail Tests

    [Fact]
    public void PostmarkTemplateEmail_DefaultValues_AreCorrect()
    {
        var email = new PostmarkTemplateEmail();

        Assert.Null(email.TemplateId);
        Assert.Null(email.TemplateAlias);
        Assert.NotNull(email.TemplateModel);
        Assert.Empty(email.TemplateModel);
        Assert.Equal(string.Empty, email.From);
        Assert.Equal(string.Empty, email.To);
    }

    [Fact]
    public void PostmarkTemplateEmail_CanSetTemplateId()
    {
        var email = new PostmarkTemplateEmail { TemplateId = 12345 };
        Assert.Equal(12345, email.TemplateId);
    }

    [Fact]
    public void PostmarkTemplateEmail_CanSetTemplateAlias()
    {
        var email = new PostmarkTemplateEmail { TemplateAlias = "welcome-email" };
        Assert.Equal("welcome-email", email.TemplateAlias);
    }

    [Fact]
    public void PostmarkTemplateEmail_CanSetTemplateModel()
    {
        var email = new PostmarkTemplateEmail
        {
            TemplateModel = new Dictionary<string, object>
            {
                ["name"] = "John",
                ["count"] = 42,
                ["active"] = true
            }
        };

        Assert.Equal("John", email.TemplateModel["name"]);
        Assert.Equal(42, email.TemplateModel["count"]);
        Assert.Equal(true, email.TemplateModel["active"]);
    }

    #endregion

    #region PostmarkAttachment Tests

    [Fact]
    public void PostmarkAttachment_DefaultValues_AreCorrect()
    {
        var attachment = new PostmarkAttachment();

        Assert.Equal(string.Empty, attachment.Name);
        Assert.Equal(string.Empty, attachment.Content);
        Assert.Equal(string.Empty, attachment.ContentType);
        Assert.Null(attachment.ContentId);
    }

    [Fact]
    public void PostmarkAttachment_FromBytes_CreatesCorrectAttachment()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        var attachment = PostmarkAttachment.FromBytes("test.pdf", bytes);

        Assert.Equal("test.pdf", attachment.Name);
        Assert.Equal(Convert.ToBase64String(bytes), attachment.Content);
        Assert.Equal("application/pdf", attachment.ContentType);
    }

    [Fact]
    public void PostmarkAttachment_FromBytes_WithCustomContentType_UsesCustomType()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var attachment = PostmarkAttachment.FromBytes("data.bin", bytes, "application/custom");

        Assert.Equal("application/custom", attachment.ContentType);
    }

    [Theory]
    [InlineData("file.pdf", "application/pdf")]
    [InlineData("image.png", "image/png")]
    [InlineData("image.jpg", "image/jpeg")]
    [InlineData("image.jpeg", "image/jpeg")]
    [InlineData("image.gif", "image/gif")]
    [InlineData("document.txt", "text/plain")]
    [InlineData("page.html", "text/html")]
    [InlineData("data.csv", "text/csv")]
    [InlineData("data.xml", "application/xml")]
    [InlineData("data.json", "application/json")]
    [InlineData("archive.zip", "application/zip")]
    [InlineData("document.doc", "application/msword")]
    [InlineData("document.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [InlineData("spreadsheet.xls", "application/vnd.ms-excel")]
    [InlineData("spreadsheet.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("unknown.xyz", "application/octet-stream")]
    public void PostmarkAttachment_FromBytes_DetectsContentType(string fileName, string expectedContentType)
    {
        var attachment = PostmarkAttachment.FromBytes(fileName, new byte[] { 1 });
        Assert.Equal(expectedContentType, attachment.ContentType);
    }

    #endregion

    #region PostmarkHeader Tests

    [Fact]
    public void PostmarkHeader_DefaultValues_AreCorrect()
    {
        var header = new PostmarkHeader();

        Assert.Equal(string.Empty, header.Name);
        Assert.Equal(string.Empty, header.Value);
    }

    [Fact]
    public void PostmarkHeader_CanSetProperties()
    {
        var header = new PostmarkHeader
        {
            Name = "X-Custom-Header",
            Value = "custom-value"
        };

        Assert.Equal("X-Custom-Header", header.Name);
        Assert.Equal("custom-value", header.Value);
    }

    #endregion

    #region PostmarkResponse Tests

    [Fact]
    public void PostmarkResponse_Success_IsTrueWhenErrorCodeIsZero()
    {
        var response = new PostmarkResponse { ErrorCode = 0 };
        Assert.True(response.Success);
    }

    [Fact]
    public void PostmarkResponse_Success_IsFalseWhenErrorCodeIsNonZero()
    {
        var response = new PostmarkResponse { ErrorCode = 300 };
        Assert.False(response.Success);
    }

    [Fact]
    public void PostmarkResponse_DefaultValues_AreCorrect()
    {
        var response = new PostmarkResponse();

        Assert.Null(response.To);
        Assert.Null(response.SubmittedAt);
        Assert.Null(response.MessageId);
        Assert.Equal(0, response.ErrorCode);
        Assert.Null(response.Message);
    }

    #endregion

    #region LinkTrackingOptions Tests

    [Fact]
    public void LinkTrackingOptions_HasCorrectValues()
    {
        Assert.Equal("None", LinkTrackingOptions.None);
        Assert.Equal("HtmlAndText", LinkTrackingOptions.HtmlAndText);
        Assert.Equal("HtmlOnly", LinkTrackingOptions.HtmlOnly);
        Assert.Equal("TextOnly", LinkTrackingOptions.TextOnly);
    }

    #endregion

    #region MessageStreams Tests

    [Fact]
    public void MessageStreams_HasCorrectValues()
    {
        Assert.Equal("outbound", MessageStreams.Outbound);
        Assert.Equal("broadcast", MessageStreams.Broadcast);
    }

    #endregion

    #region PostmarkDeliveryStats Tests

    [Fact]
    public void PostmarkDeliveryStats_DefaultValues_AreCorrect()
    {
        var stats = new PostmarkDeliveryStats();

        Assert.Equal(0, stats.InactiveMails);
        Assert.Null(stats.Bounces);
    }

    #endregion

    #region PostmarkBounce Tests

    [Fact]
    public void PostmarkBounce_DefaultValues_AreCorrect()
    {
        var bounce = new PostmarkBounce();

        Assert.Equal(0, bounce.Id);
        Assert.Null(bounce.Type);
        Assert.Equal(0, bounce.TypeCode);
        Assert.Null(bounce.Email);
        Assert.False(bounce.CanActivate);
        Assert.False(bounce.Inactive);
    }

    #endregion

    #region PostmarkOutboundStats Tests

    [Fact]
    public void PostmarkOutboundStats_DefaultValues_AreCorrect()
    {
        var stats = new PostmarkOutboundStats();

        Assert.Equal(0, stats.Sent);
        Assert.Equal(0, stats.Bounced);
        Assert.Equal(0, stats.SpamComplaints);
        Assert.Equal(0, stats.BounceRate);
        Assert.Equal(0, stats.UniqueOpens);
        Assert.Equal(0, stats.TotalOpens);
        Assert.Equal(0, stats.UniqueClicks);
        Assert.Equal(0, stats.TotalClicks);
    }

    #endregion

    #region PostmarkServer Tests

    [Fact]
    public void PostmarkServer_DefaultValues_AreCorrect()
    {
        var server = new PostmarkServer();

        Assert.Equal(0, server.Id);
        Assert.Null(server.Name);
        Assert.False(server.SmtpApiActivated);
        Assert.False(server.RawEmailEnabled);
        Assert.False(server.TrackOpens);
    }

    #endregion

    #region FromEnvironment Tests

    [Fact]
    public void FromEnvironment_WithMissingVariable_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("POSTMARK_TEST_MISSING", null);

        Assert.Throws<InvalidOperationException>(
            () => PostmarkClient.FromEnvironment("POSTMARK_TEST_MISSING"));
    }

    [Fact]
    public void FromEnvironment_WithSetVariable_CreatesClient()
    {
        const string envVar = "POSTMARK_TEST_TOKEN";
        Environment.SetEnvironmentVariable(envVar, "test-server-token");

        try
        {
            var client = PostmarkClient.FromEnvironment(envVar);

            Assert.NotNull(client);
            Assert.Equal("test-server-token", client.Options.ServerToken);
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    #endregion

    #region BounceStats Tests

    [Fact]
    public void BounceStats_DefaultValues_AreCorrect()
    {
        var stats = new BounceStats();

        Assert.Null(stats.Name);
        Assert.Equal(0, stats.Count);
        Assert.Null(stats.Type);
    }

    #endregion

    #region PostmarkBouncesResponse Tests

    [Fact]
    public void PostmarkBouncesResponse_DefaultValues_AreCorrect()
    {
        var response = new PostmarkBouncesResponse();

        Assert.Equal(0, response.TotalCount);
        Assert.Null(response.Bounces);
    }

    #endregion

    #region PostmarkBounceActivation Tests

    [Fact]
    public void PostmarkBounceActivation_DefaultValues_AreCorrect()
    {
        var activation = new PostmarkBounceActivation();

        Assert.Null(activation.Message);
        Assert.Null(activation.Bounce);
    }

    #endregion
}
