# ForeverTools.Postmark

Lightweight Postmark email API client for .NET. Send transactional emails, batch emails, and templated emails with delivery tracking.

[![NuGet](https://img.shields.io/nuget/v/ForeverTools.Postmark.svg)](https://www.nuget.org/packages/ForeverTools.Postmark)

## Features

- **Simple Email Sending** - Single and batch (up to 500) emails
- **Template Support** - Send emails using Postmark templates
- **Delivery Tracking** - Open and click tracking
- **Bounce Management** - Get, filter, and reactivate bounces
- **Statistics** - Delivery stats and outbound metrics
- **ASP.NET Core Ready** - Built-in dependency injection
- **Async/Await** - Fully asynchronous
- **Multi-Target** - .NET 8, .NET 6, .NET Standard 2.0

## Quick Start

### Install

```bash
dotnet add package ForeverTools.Postmark
```

### Get Your API Key

Sign up at [Postmark](https://www.postmarkapp.com?via=8ac781) to get your Server API Token.

### Send an Email

```csharp
using ForeverTools.Postmark;

var client = new PostmarkClient("your-server-token");

// Simple send
var result = await client.SendEmailAsync(
    to: "recipient@example.com",
    subject: "Hello from Postmark!",
    htmlBody: "<h1>Welcome!</h1><p>This is a test email.</p>",
    from: "sender@yourdomain.com"
);

if (result.Success)
{
    Console.WriteLine($"Sent! Message ID: {result.MessageId}");
}
```

## Email with Full Options

```csharp
var email = new PostmarkEmail
{
    From = "sender@yourdomain.com",
    To = "recipient@example.com",
    Subject = "Order Confirmation",
    HtmlBody = "<h1>Thank you for your order!</h1>",
    TextBody = "Thank you for your order!",
    Tag = "order-confirmation",
    TrackOpens = true,
    TrackLinks = LinkTrackingOptions.HtmlAndText,
    Metadata = new Dictionary<string, string>
    {
        ["order_id"] = "12345"
    }
};

var result = await client.SendEmailAsync(email);
```

## Batch Sending (up to 500)

```csharp
var emails = new List<PostmarkEmail>
{
    new() { From = "sender@yourdomain.com", To = "user1@example.com", Subject = "Hello 1", TextBody = "Hi!" },
    new() { From = "sender@yourdomain.com", To = "user2@example.com", Subject = "Hello 2", TextBody = "Hi!" },
    new() { From = "sender@yourdomain.com", To = "user3@example.com", Subject = "Hello 3", TextBody = "Hi!" }
};

var results = await client.SendBatchAsync(emails);

foreach (var result in results)
{
    Console.WriteLine($"{result.To}: {(result.Success ? "Sent" : result.Message)}");
}
```

## Template Emails

```csharp
// Using template alias
var result = await client.SendTemplateEmailAsync(
    templateIdOrAlias: "welcome-email",
    to: "user@example.com",
    templateModel: new Dictionary<string, object>
    {
        ["name"] = "John",
        ["product_name"] = "Awesome App",
        ["action_url"] = "https://example.com/activate"
    },
    from: "welcome@yourdomain.com"
);
```

## Attachments

```csharp
var email = new PostmarkEmail
{
    From = "sender@yourdomain.com",
    To = "recipient@example.com",
    Subject = "Your Invoice",
    TextBody = "Please find your invoice attached.",
    Attachments = new List<PostmarkAttachment>
    {
        PostmarkAttachment.FromFile("invoice.pdf"),
        PostmarkAttachment.FromBytes("data.csv", csvBytes, "text/csv")
    }
};

await client.SendEmailAsync(email);
```

## Delivery Statistics

```csharp
// Get delivery overview
var stats = await client.GetDeliveryStatsAsync();
Console.WriteLine($"Inactive emails: {stats.InactiveMails}");

// Get detailed outbound stats
var outbound = await client.GetOutboundStatsAsync(
    fromDate: DateTime.UtcNow.AddDays(-30),
    toDate: DateTime.UtcNow
);

Console.WriteLine($"Sent: {outbound.Sent}");
Console.WriteLine($"Bounced: {outbound.Bounced}");
Console.WriteLine($"Opens: {outbound.UniqueOpens}");
Console.WriteLine($"Clicks: {outbound.UniqueClicks}");
```

## Bounce Management

```csharp
// Get recent bounces
var bounces = await client.GetBouncesAsync(count: 50);

foreach (var bounce in bounces.Bounces)
{
    Console.WriteLine($"{bounce.Email}: {bounce.Type} - {bounce.Description}");
}

// Reactivate a bounced address
var activation = await client.ActivateBounceAsync(bounceId: 123456);
```

## ASP.NET Core Integration

```csharp
// Program.cs
builder.Services.AddForeverToolsPostmark("your-server-token");

// Or with full configuration
builder.Services.AddForeverToolsPostmark(options =>
{
    options.ServerToken = "your-server-token";
    options.DefaultFrom = "noreply@yourdomain.com";
    options.TrackOpens = true;
    options.TrackLinks = "HtmlAndText";
});

// Or from appsettings.json
builder.Services.AddForeverToolsPostmark(builder.Configuration);
```

```json
// appsettings.json
{
  "Postmark": {
    "ServerToken": "your-server-token",
    "DefaultFrom": "noreply@yourdomain.com",
    "TrackOpens": true
  }
}
```

```csharp
// Inject and use
public class EmailService
{
    private readonly PostmarkClient _postmark;

    public EmailService(PostmarkClient postmark)
    {
        _postmark = postmark;
    }

    public async Task SendWelcomeEmail(string email, string name)
    {
        await _postmark.SendTemplateEmailAsync(
            "welcome-template",
            email,
            new Dictionary<string, object> { ["name"] = name }
        );
    }
}
```

## Environment Variables

```csharp
// Uses POSTMARK_SERVER_TOKEN by default
var client = PostmarkClient.FromEnvironment();

// Or specify custom variable name
var client = PostmarkClient.FromEnvironment("MY_POSTMARK_TOKEN");
```

## Message Streams

Postmark separates transactional and marketing emails:

```csharp
// Transactional (default)
email.MessageStream = MessageStreams.Outbound;

// Marketing/newsletters
email.MessageStream = MessageStreams.Broadcast;
```

## Link Tracking Options

```csharp
email.TrackLinks = LinkTrackingOptions.None;        // No tracking
email.TrackLinks = LinkTrackingOptions.HtmlAndText; // Track all links
email.TrackLinks = LinkTrackingOptions.HtmlOnly;    // Track only HTML links
email.TrackLinks = LinkTrackingOptions.TextOnly;    // Track only text links
```

## Why Postmark?

[Postmark](https://www.postmarkapp.com?via=8ac781) is designed specifically for transactional email:

- **Fast delivery** - 99% of emails delivered in under 10 seconds
- **High deliverability** - Dedicated IP pools, DKIM/SPF support
- **Detailed analytics** - Opens, clicks, bounces, spam complaints
- **Template system** - Reusable email templates with variables
- **Excellent support** - Known for responsive customer service

## Other ForeverTools Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ForeverTools.AIML** | Access 400+ AI models (GPT-4, Claude, Llama, Gemini, DALL-E) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.AIML.svg)](https://www.nuget.org/packages/ForeverTools.AIML) |
| **ForeverTools.APILayer** | IP geolocation, currency exchange, phone & email validation | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.APILayer.svg)](https://www.nuget.org/packages/ForeverTools.APILayer) |
| **ForeverTools.Captcha** | Multi-provider captcha solving (2Captcha, CapSolver, Anti-Captcha) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.Captcha.svg)](https://www.nuget.org/packages/ForeverTools.Captcha) |
| **ForeverTools.ImageGen** | AI image generation with social media presets (DALL-E, Flux, SD) | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ImageGen.svg)](https://www.nuget.org/packages/ForeverTools.ImageGen) |
| **ForeverTools.OCR** | AI-powered OCR using GPT-4 Vision, Claude 3, and Gemini | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.OCR.svg)](https://www.nuget.org/packages/ForeverTools.OCR) |
| **ForeverTools.ScraperAPI** | Web scraping with proxy rotation and CAPTCHA bypass | [![NuGet](https://img.shields.io/nuget/v/ForeverTools.ScraperAPI.svg)](https://www.nuget.org/packages/ForeverTools.ScraperAPI) |

## Requirements

- .NET 8.0, .NET 6.0, or .NET Standard 2.0 compatible framework
- Postmark account with verified sender signature

## License

MIT License - see [LICENSE](LICENSE) for details.
