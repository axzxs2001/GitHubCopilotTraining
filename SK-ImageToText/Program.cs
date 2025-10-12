using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.ChatCompletion;

var arr = File.ReadLines("C://gpt/azure_key.txt").ToArray();
var endpoint = arr[1];
var deploymentName = arr[0];
var key = arr[2];


var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddAzureOpenAIChatCompletion(
    deploymentName: deploymentName,
    endpoint: endpoint,
    apiKey: key);
var kernel = kernelBuilder.Build();
var chatHistory = new ChatHistory("你是一个图像解释助手。");


var imageFilePath = "mcp-dev-use.jpg"; 
var imageBytes = await File.ReadAllBytesAsync(imageFilePath);
var imageMemory = new ReadOnlyMemory<byte>(imageBytes);

chatHistory.AddUserMessage(new ChatMessageContentItemCollection
{
    new TextContent("请说明漫画的寓意是什么"),
    new ImageContent(imageMemory, "image/jpeg") 
});
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
Console.WriteLine("AI 响应:");
await foreach (var message in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory))
{
    Console.Write(message.Content);
}
Console.ReadLine();