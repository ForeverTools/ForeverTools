using ForeverTools.Mcp.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(opts => opts.FormatterName = "simple")
               .SetMinimumLevel(LogLevel.Warning);

builder.Services.AddHttpClient();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<SummarizeTool>()
    .WithTools<TranslateTool>()
    .WithTools<SentimentTool>()
    .WithTools<OcrTool>();

await builder.Build().RunAsync();
