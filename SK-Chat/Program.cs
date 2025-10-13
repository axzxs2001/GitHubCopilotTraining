using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextToImage;
using Microsoft.SemanticKernel.TextToAudio;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.Connectors.Google;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var arr = File.ReadLines("C://gpt/azure_key.txt").ToArray();
var endpoint = arr[1];
var deploymentName = arr[0];
var key = arr[2];

app.MapPost("/chat", async (PromptMessage chatPrompt) =>
{
    var kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: deploymentName,
        endpoint: endpoint,
        apiKey: key)
        // .AddOpenAIChatCompletion(apiKey: key, modelId: "gpt-3.5-turbo")
        // .AddGoogleAIGeminiChatCompletion(modelId:"gemini-1", apiKey: key)       
        ;
    var kernel = kernelBuilder.Build();
    var chatHistory = new ChatHistory("你是一个乐于助人的助手。");
    chatHistory.AddUserMessage(chatPrompt.Input);
    var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
    var reply = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
    return Results.Ok(reply.Content);
})
.WithName("chat");

app.Run();
public class PromptMessage
{
    public string Input { get; set; }
}
