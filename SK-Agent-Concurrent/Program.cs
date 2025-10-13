using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.Concurrent;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;


#pragma warning disable
var arr = File.ReadLines("C://gpt/azure_key.txt").ToArray();
var endpoint = arr[1];
var deploymentName = arr[0];
var key = arr[2];

//简历提取
var resume = File.ReadAllText("李明.md");
var kernel = Kernel.CreateBuilder()
           .AddAzureOpenAIChatCompletion(deploymentName, endpoint, key).Build();

ChatCompletionAgent infoMaster =
    CreateAgent(
        instructions: "从文本中识别并提取个人基本信息，包括姓名、职位、联系方式、邮箱、GitHub、LinkedIn等。",
        description: "负责提取候选人的基础信息，如姓名、联系方式、所在地及社交链接，确保信息完整准确。",
        name: "InfoMaster",
        kernel);
ChatCompletionAgent careerSeeker =
    CreateAgent(
        instructions: "从文本中识别并提取所有与工作经历相关的内容，明确每段经历的公司、职位、时间及关键业绩。",
        description: "专注提取工作经历信息，聚焦公司名称、职位、时间、地点及主要职责和成果。",
        name: "CareerSeeker",
        kernel);
ChatCompletionAgent projectHunter =
    CreateAgent(
        instructions: "从文本中提取项目名称、使用技术、项目类型（个人/团队）及主要成就或优化结果。",
        description: "负责提取项目经历信息，分析项目名称、类型、技术栈和成果亮点，展现技术能力与实践经验。",
        name: "ProjectHunter",
        kernel);
ChatCompletionAgent eduSage =
    CreateAgent(
        instructions: "从文本中识别教育经历，提取学校、专业、学历、就读时间及获奖或主修课程等内容。",
        description: "专注提取教育背景信息，包括学校名称、专业、学历、时间及相关荣誉或课程。",
        name: "EduSage",
        kernel);                

var orchestration = new ConcurrentOrchestration(infoMaster, careerSeeker, projectHunter, eduSage)
{
    ResponseCallback = (response) =>
    {
        Console.WriteLine($"代理名称: {response.AuthorName}\n*****返回内容****：\r{response.Content}");
        Console.WriteLine("=====================================");
        return ValueTask.CompletedTask;
    },
    LoggerFactory = NullLoggerFactory.Instance
};
var runtime = new InProcessRuntime();
await runtime.StartAsync();


OrchestrationResult<string[]> result = await orchestration.InvokeAsync(resume, runtime);

string[] output = await result.GetValueAsync(TimeSpan.FromSeconds(30));
//Console.WriteLine($"\n# 结果:\n{string.Join("\n\n", output.Select(text => $"{text}"))}");

await runtime.RunUntilIdleAsync();

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

