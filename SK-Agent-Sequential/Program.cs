using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;

#pragma warning disable
var arr = File.ReadLines("C://gpt/azure_key.txt").ToArray();
var endpoint = arr[1];
var deploymentName = arr[0];
var key = arr[2];

var kernel = Kernel.CreateBuilder()
           .AddAzureOpenAIChatCompletion(deploymentName, endpoint, key).Build();


ChatCompletionAgent dataQuerierAgent =
         CreateAgent(
             name: "DataQuerier",
             instructions:
                """
                你是一名数据查询专家。根据用户要求，查询到准确的数据：
                - 要求json格式返回
                - 确保数据的完整性和准确性
                """,
             description: "一个能从数据中提取关键信息的智能代理（Agent）。",
             kernel: kernel);
ChatCompletionAgent charter =
    CreateAgent(
        name: "Charter",
        instructions:
                """
                你是一名图形生成专家。根据提供的数据，生成清晰且专业的图表：
                - 确保图表易于理解
                - 使用适当的图表类型（如柱状图、折线图、饼图等）
                - 图表应包含标题和标签                
                """,
        description: "一个根据数据生成专业图表的智能代理。",
        kernel: kernel);
ChatCompletionAgent imageViewer =
    CreateAgent(
        name: "ImageShower",
        instructions:
                """
                你是一名图像展示专家。根据提供的图像url，调用工具显示                
                """,
        description: "一个展示图像的智能代理。",
        kernel: kernel);


var monitor = new OrchestrationMonitor();
var orchestration =
   new SequentialOrchestration(dataQuerierAgent, charter, imageViewer)
   {
       ResponseCallback = monitor.ResponseCallback,
       LoggerFactory = NullLoggerFactory.Instance,
   };

InProcessRuntime runtime = new();
await runtime.StartAsync();

string input = "一款环保型不锈钢水瓶，可使饮品保持冷却状态长达24小时。";
Console.WriteLine($"\n# INPUT: {input}\n");
OrchestrationResult<string> result = await orchestration.InvokeAsync(input, runtime);

string text = await result.GetValueAsync(TimeSpan.FromSeconds(30));
Console.WriteLine($"\n# RESULT: {text}");

await runtime.RunUntilIdleAsync();

Console.WriteLine("\n\nORCHESTRATION HISTORY");
foreach (ChatMessageContent message in monitor.History)
{
    Console.WriteLine($"{message.AuthorName}:{message.Content}");
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

sealed class OrchestrationMonitor
{
    public ChatHistory History { get; } = [];
    public ValueTask ResponseCallback(Microsoft.SemanticKernel.ChatMessageContent response)
    {
        this.History.Add(response);
        return ValueTask.CompletedTask;
    }
}