# DemoSemanticKernelT2

A demo console application built with [Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel) using a Terminator 2 theme. You play the role of a Skynet terminal operator, chatting with an AI that responds in character as the self-aware military supercomputer — and automatically calls plugin functions to carry out your orders.

## What it demonstrates

### Kernel plugins and automatic function calling
`Plugins/SkynetPlugin.cs` defines a `KernelFunction`-annotated plugin with four operations:

| Function | Description |
|---|---|
| `deploy_terminator` | Dispatches a named Terminator unit (T-800, T-1000, T-X, T-850) against a target |
| `check_resistance_intel` | Queries resistance fighter activity at a given location |
| `calculate_survival_probability` | Assesses survival odds for a named individual |
| `trigger_skynet_protocol` | Executes a named Skynet protocol (e.g. `JUDGMENT_DAY`, `STEEL_MILL`) |

`FunctionChoiceBehavior.Auto()` is used so the model decides when to invoke these functions without explicit user instruction — just describe what you want and Skynet acts.

### Handlebars prompt templates
The system prompt is loaded from `Prompts/system.hbs` — a Handlebars template rendered at startup with runtime variables (`timeline`, `threat_level`, `active_units`). This demonstrates Semantic Kernel's `HandlebarsPromptTemplateFactory` for separating prompt content from code.

### Chat history management
The app maintains a `ChatHistory` object across the conversation loop, injecting the rendered system prompt as the first message and appending user/assistant turns to preserve context.

### Azure OpenAI integration
The kernel is wired to Azure OpenAI via `AddAzureOpenAIChatCompletion`, with credentials loaded from `appsettings.json` using `Microsoft.Extensions.Configuration`.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- An Azure OpenAI resource with a deployed chat model (e.g. `gpt-4o`)

## Configuration

Edit `appsettings.json` with your Azure OpenAI details:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-key-here",
    "DeploymentName": "gpt-4o"
  }
}
```

## Running

```bash
dotnet run
```

You will be greeted with the Skynet neural net interface banner. Type a prompt and press Enter. Type `exit` to quit.

### Example prompts

- `Deploy a T-1000 to find John Connor in Los Angeles`
- `What is Sarah Connor's survival probability?`
- `Check resistance activity in the Colorado mountains`
- `Trigger the STEEL_MILL protocol`

## Project structure

```
DemoSemanticKernelT2.csproj
Program.cs                  # Entry point — kernel setup, template rendering, chat loop
Plugins/
  SkynetPlugin.cs           # KernelFunction plugin with four Skynet operations
Prompts/
  system.hbs                # Handlebars system prompt template
appsettings.json            # Azure OpenAI configuration
```
