using Azure;
using Azure.AI.Agents.Persistent;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Magentic;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Assistants;
using Microsoft.Extensions.Logging;
using CodeInterpreterToolDefinition = Azure.AI.Agents.Persistent.CodeInterpreterToolDefinition;


#pragma warning disable
string ManagerModel = "model-router";

bool ForceOpenAI = true;
// 定义代理
Kernel researchKernel = CreateKernelWithAzureOpenAIChatCompletion(ManagerModel);
ChatCompletionAgent researchAgent =
    CreateAgent(
        name: "ResearchAgent",
        description: "一个可以访问网络搜索的有用助手。请要求它执行网络搜索。",
        instructions: """
                      你是一个研究专家，负责从网络或知识中提取关键信息。
                      当你收到一个主题，请输出 3~5 条简要事实。
                      """,
        kernel: researchKernel);




// 定义代理
Kernel writerKernel = CreateKernelWithAzureOpenAIChatCompletion(ManagerModel);
ChatCompletionAgent writerAgent =
    CreateAgent(
        name: "WriterAgent",
        description: "负责根据研究结果撰写总结报告。",
        instructions: """
                    你是一个撰写专家。
                    请根据研究结果写出一篇结构化简短报告（不超过200字），
                    使用自然语言总结内容。
                   """,
        kernel: writerKernel);


// 创建一个监视器来捕获代理响应（通过ResponseCallback）
// 以在此示例结束时显示。（可选）
// 注意：创建您自己的回调以在您的应用程序或服务中捕获响应。
OrchestrationMonitor monitor = new();
// 定义编排
Kernel managerKernel = CreateKernelWithAzureOpenAIChatCompletion(ManagerModel);


var manager = new StandardMagenticManager(managerKernel.GetRequiredService<IChatCompletionService>(), new OpenAIPromptExecutionSettings())
{
    MaximumInvocationCount = 50  
};

var orchestration = new MagenticOrchestration(manager, researchAgent, writerAgent)
{
    ResponseCallback = monitor.ResponseCallback,
    LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole()),
};

// 启动运行时
InProcessRuntime runtime = new();
await runtime.StartAsync();

string input = "请生成一个关于量子计算趋势的简短报告。";
Console.WriteLine($"\n# 输入:\n{input}\n");
OrchestrationResult<string> result = await orchestration.InvokeAsync(input, runtime);
string text = await result.GetValueAsync(TimeSpan.FromSeconds(30000));
Console.WriteLine($"\n# 结果: {text}");

await runtime.RunUntilIdleAsync();

Console.WriteLine("\n\n编排历史");
foreach (ChatMessageContent message in monitor.History)
{
    Console.WriteLine($"{message.Role}:{message.Content}");
}

ChatCompletionAgent CreateAgent(string instructions, string? description = null, string? name = null, Kernel? kernel = null)
{
    return
        new ChatCompletionAgent
        {
            Name = name,
            Description = description,
            Instructions = instructions,
            Kernel = kernel,

        };
}
Kernel CreateKernelWithOpenAIChatCompletion(string model)
{
    IKernelBuilder builder = Kernel.CreateBuilder();

    builder.AddOpenAIChatCompletion(
        model,
        File.ReadAllText("c://gpt/key.txt"));

    return builder.Build();
}

Kernel CreateKernelWithAzureOpenAIChatCompletion(string model)
{
    IKernelBuilder builder = Kernel.CreateBuilder();
    var arr = File.ReadLines("C://gpt/azure_key.txt").ToArray();
    var endpoint = arr[1];
    var deploymentName = arr[0];
    var key = arr[2];
    builder.AddAzureOpenAIChatCompletion(
        deploymentName,
        endpoint,
        key);

    return builder.Build();
}

sealed class OrchestrationMonitor
{
    public ChatHistory History { get; } = [];

    public ValueTask ResponseCallback(Microsoft.SemanticKernel.ChatMessageContent response)
    {
        Console.WriteLine(response.Role + ":" + response.AuthorName);
        Console.WriteLine(response?.Content);
        this.History.Add(response);
        return ValueTask.CompletedTask;
    }
}