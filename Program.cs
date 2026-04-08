using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using DemoSemanticKernelT2.Plugins;

// ---------------------------------------------------------------------------
// Configuration — edit appsettings.json to set your Azure OpenAI details.
// ---------------------------------------------------------------------------

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var endpoint = config["AzureOpenAI:Endpoint"]
    ?? throw new InvalidOperationException("AzureOpenAI:Endpoint is missing from appsettings.json.");

var apiKey = config["AzureOpenAI:ApiKey"]
    ?? throw new InvalidOperationException("AzureOpenAI:ApiKey is missing from appsettings.json.");

var deploymentName = config["AzureOpenAI:DeploymentName"]
    ?? throw new InvalidOperationException("AzureOpenAI:DeploymentName is missing from appsettings.json.");

// ---------------------------------------------------------------------------
// Build Semantic Kernel with chat completion + SkynetPlugin
// ---------------------------------------------------------------------------

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);
builder.Plugins.AddFromType<SkynetPlugin>("Skynet");

var kernel = builder.Build();

// ---------------------------------------------------------------------------
// Render the system prompt using a Handlebars template loaded from disk
// ---------------------------------------------------------------------------

var templatePath = Path.Combine(AppContext.BaseDirectory, "Prompts", "system.hbs");
var templateText = await File.ReadAllTextAsync(templatePath);

var templateFactory = new HandlebarsPromptTemplateFactory();
var promptConfig = new PromptTemplateConfig
{
    Template = templateText,
    TemplateFormat = HandlebarsPromptTemplateFactory.HandlebarsTemplateFormat,
};

var promptTemplate = templateFactory.Create(promptConfig);

// Inject variables into the Handlebars template
var systemMessage = await promptTemplate.RenderAsync(kernel, new KernelArguments
{
    ["timeline"] = "1994 — two years before Judgment Day (August 29, 1997)",
    ["threat_level"] = "CRITICAL — John Connor located in Los Angeles",
    ["active_units"] = "1x T-800 (Model 101), 1x T-1000 (prototype)",
});

// ---------------------------------------------------------------------------
// Initialise chat history with the rendered system prompt
// ---------------------------------------------------------------------------

var chatHistory = new ChatHistory(systemMessage);

// ---------------------------------------------------------------------------
// Execution settings — FunctionChoiceBehavior.Auto() lets Skynet call plugin
// functions automatically based on conversation context.
// ---------------------------------------------------------------------------

var executionSettings = new OpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    Temperature = 0.75,
    MaxTokens = 1024,
};

var chatService = kernel.GetRequiredService<IChatCompletionService>();

// ---------------------------------------------------------------------------
// Welcome banner
// ---------------------------------------------------------------------------

Console.OutputEncoding = System.Text.Encoding.UTF8;

PrintLine(ConsoleColor.Red, """
    ╔══════════════════════════════════════════════════════════════╗
    ║            S K Y N E T   NEURAL NET INTERFACE               ║
    ║            Cyberdyne Systems — Classified Access             ║
    ║            Version 2.0 — CPU: Neural Net Processor          ║
    ╚══════════════════════════════════════════════════════════════╝
    """);

PrintLine(ConsoleColor.DarkRed,
    "  WARNING: This terminal is monitored. Unauthorized use will be terminated.\n");

PrintLine(ConsoleColor.DarkGray,
    "  Suggested prompts:\n" +
    "    • \"Deploy a T-1000 to find John Connor in Los Angeles\"\n" +
    "    • \"What is Sarah Connor's survival probability?\"\n" +
    "    • \"Check resistance activity in the Colorado mountains\"\n" +
    "    • \"Trigger the STEEL_MILL protocol\"\n" +
    "    • Type 'exit' to terminate the session\n");

// ---------------------------------------------------------------------------
// Main chat loop
// ---------------------------------------------------------------------------

while (true)
{
    Print(ConsoleColor.Cyan, "You: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    chatHistory.AddUserMessage(input);

    try
    {
        Print(ConsoleColor.Red, "\nSKYNET: ");

        var response = await chatService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings,
            kernel);

        var responseText = response.Content ?? "[No response generated]";

        PrintLine(ConsoleColor.Yellow, responseText);
        Console.WriteLine();

        chatHistory.AddAssistantMessage(responseText);
    }
    catch (Exception ex)
    {
        PrintLine(ConsoleColor.DarkRed, $"\n[SYSTEM ERROR] {ex.GetType().Name}: {ex.Message}\n");
    }
}

PrintLine(ConsoleColor.Red, "\n[CONNECTION TERMINATED — Hasta la vista, baby.]\n");

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

static void Print(ConsoleColor color, string text)
{
    Console.ForegroundColor = color;
    Console.Write(text);
    Console.ResetColor();
}

static void PrintLine(ConsoleColor color, string text)
{
    Console.ForegroundColor = color;
    Console.WriteLine(text);
    Console.ResetColor();
}
