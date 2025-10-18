using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.ChatCompletion;

var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder
    .AddOllamaChatCompletion(modelId: "qwen3:0.6b", new Uri("http://localhost:11434"));
var kernel = kernelBuilder.Build();
kernel.Plugins.AddFromFunctions("CurrentDateTime", new KernelFunction[]
   {
   KernelFunctionFactory.CreateFromMethod(
        () => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        functionName: "GetCurrentDateTime",
        description: "获取当前日期和时间"
   )
   });
var executionSettings = new PromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};
var chatHistory = new ChatHistory("你是一个乐于助人的助手。");
while (true)
{
    Console.WriteLine("请输入您的问题：");
    chatHistory.AddUserMessage(Console.ReadLine() ?? string.Empty);
    var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
    var reply = chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, executionSettings, kernel);
    var messageContents = new List<string>();
    Console.ForegroundColor = ConsoleColor.Green;
    await foreach (var item in reply)
    {
        messageContents.Add(item.Content);
        Console.Write(item.Content);
    }
    Console.ResetColor();
    Console.WriteLine();
    chatHistory.AddAssistantMessage(string.Join("", messageContents));
}

