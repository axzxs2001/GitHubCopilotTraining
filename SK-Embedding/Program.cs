using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.Extensions.AI;
#pragma warning disable 

//await AzureEmbedding();
await OllamaEmbedding();


static async Task OllamaEmbedding()
{
    var arr = File.ReadLines("C://gpt/azure_key.txt").ToArray();
    var endpoint = arr[1];
    var deploymentName = arr[0];
    var key = arr[2];
    IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder
    .AddOllamaEmbeddingGenerator("qwen3-embedding:0.6b", new Uri("http://localhost:11434/"))
    .AddAzureOpenAIChatCompletion(
        deploymentName: deploymentName,
        endpoint: endpoint,
        apiKey: key);
    Kernel kernel = kernelBuilder.Build();

    var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

    string[] documents = new[]
    {
    "净利润：¥1,250,000 本月公司整体盈利水平，扣除所有成本和税费后的最终利润。",
    "工资成本：¥2,860,000 本月员工工资、奖金及社保公积金等人力成本支出。",
    "广告与市场推广费：¥480,000 包括线上广告投放、线下宣传活动及品牌合作费用。",
    "办公租金：¥310,000 总部及分支办公室租赁费用。",
    "销售收入：¥6,750,000 本月来自产品销售及服务的总收入。",
    "物流与运输费用：¥210,000 用于货物配送、仓储和第三方物流服务的支出。",
    "生产原材料成本：¥1,970,000 生产产品所需的主要原料和半成品采购成本。",
    "研发支出：¥620,000 研发部门人员薪资、设备购置及实验材料费用。",
    "税费支出：¥380,000 包括增值税、企业所得税及附加税费。",
    "信息化系统维护费：¥95,000 ERP、财务系统、服务器及软件订阅等IT相关支出。"
};

    var vectorStore = new Dictionary<string, ReadOnlyMemory<float>>();
    foreach (var doc in documents)
    {
        var embedding = await embeddingGenerator.GenerateVectorAsync(doc);
        vectorStore[doc] = embedding;
    }

    Console.WriteLine("向量的维度：" + vectorStore.First().Value.Length);

    while (true)
    {
        Console.WriteLine("请输入您的问题：");
        var chatPrompt = Console.ReadLine() ?? string.Empty;
        var list = await OllamaQueryAsync(chatPrompt, embeddingGenerator, vectorStore);
        foreach (var item in list)
        {
            Console.WriteLine($"相似度：{item.Similarity:F4}，内容：{item.Document}");
        }
        var chatHistory = new ChatHistory("你是一个乐于助人的助手。只回答用户提出的问题，不多嘴");
        chatHistory.AddUserMessage(chatPrompt);
        chatHistory.AddUserMessage("根据以下内容作答：" +
            string.Join("\n", list.Take(2).Select(x => x.Document)));

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var reply = chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory);
        Console.ForegroundColor = ConsoleColor.Green;
        await foreach (var item in reply)
        {
            Console.Write(item.Content);
        }
        Console.ResetColor();
        Console.WriteLine();
    }
}

static async Task AzureEmbedding()
{
    var arr = File.ReadLines("C://gpt/azure_key.txt").ToArray();
    var endpoint = arr[1];
    var deploymentName = arr[0];
    var key = arr[2];

    IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder
    //.AddOllamaEmbeddingGenerator("qwen3-embedding:0.6b", new Uri("http://localhost:11434/"))
    .AddAzureOpenAITextEmbeddingGeneration(deploymentName: "text-embedding-ada-002", endpoint: endpoint, apiKey: key)
    .AddAzureOpenAIChatCompletion(
        deploymentName: deploymentName,
        endpoint: endpoint,
        apiKey: key);
    Kernel kernel = kernelBuilder.Build();

    var embeddingGenerator = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

    string[] documents = new[]
    {
    "净利润：¥1,250,000 本月公司整体盈利水平，扣除所有成本和税费后的最终利润。",
    "工资成本：¥2,860,000 本月员工工资、奖金及社保公积金等人力成本支出。",
    "广告与市场推广费：¥480,000 包括线上广告投放、线下宣传活动及品牌合作费用。",
    "办公租金：¥310,000 总部及分支办公室租赁费用。",
    "销售收入：¥6,750,000 本月来自产品销售及服务的总收入。",
    "物流与运输费用：¥210,000 用于货物配送、仓储和第三方物流服务的支出。",
    "生产原材料成本：¥1,970,000 生产产品所需的主要原料和半成品采购成本。",
    "研发支出：¥620,000 研发部门人员薪资、设备购置及实验材料费用。",
    "税费支出：¥380,000 包括增值税、企业所得税及附加税费。",
    "信息化系统维护费：¥95,000 ERP、财务系统、服务器及软件订阅等IT相关支出。"
};

    var vectorStore = new Dictionary<string, ReadOnlyMemory<float>>();
    foreach (var doc in documents)
    {
        var embedding = await embeddingGenerator.GenerateEmbeddingAsync(doc);
        vectorStore[doc] = embedding;
    }

    Console.WriteLine("向量的维度：" + vectorStore.First().Value.Length);

    while (true)
    {
        Console.WriteLine("请输入您的问题：");
        var chatPrompt = Console.ReadLine() ?? string.Empty;
        var list = await QueryAsync(chatPrompt, embeddingGenerator, vectorStore);
        foreach (var item in list)
        {
            Console.WriteLine($"相似度：{item.Similarity:F4}，内容：{item.Document}");
        }
        var chatHistory = new ChatHistory("你是一个乐于助人的助手。只回答用户提出的问题，不多嘴");
        chatHistory.AddUserMessage(chatPrompt);
        chatHistory.AddUserMessage("根据以下内容作答：" +
            string.Join("\n", list.Take(2).Select(x => x.Document)));

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var reply = chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory);
        Console.ForegroundColor = ConsoleColor.Green;
        await foreach (var item in reply)
        {
            Console.Write(item.Content);
        }
        Console.ResetColor();
        Console.WriteLine();
    }
}
// 搜索查询
static async Task<IEnumerable<dynamic>> QueryAsync(string query, ITextEmbeddingGenerationService embeddingGenerator, Dictionary<string, ReadOnlyMemory<float>> vectorStore)
{
    var queryEmbedding = await embeddingGenerator.GenerateEmbeddingAsync(query);
    var results = vectorStore
        .Select(kvp => new
        {
            Document = kvp.Key,
            Similarity = CosineSimilarity(queryEmbedding.Span, kvp.Value.Span)
        })
        .OrderByDescending(x => x.Similarity);
    return results;
}
static async Task<IEnumerable<dynamic>> OllamaQueryAsync(string query, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, Dictionary<string, ReadOnlyMemory<float>> vectorStore)
{
    var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);
    var results = vectorStore
        .Select(kvp => new
        {
            Document = kvp.Key,
            Similarity = CosineSimilarity(queryEmbedding.Span, kvp.Value.Span)
        })
        .OrderByDescending(x => x.Similarity);
    return results;
}
static float CosineSimilarity(ReadOnlySpan<float> x, ReadOnlySpan<float> y)
{
    float dot = 0, magX = 0, magY = 0;
    for (int i = 0; i < x.Length; i++)
    {
        dot += x[i] * y[i];
        magX += x[i] * x[i];
        magY += y[i] * y[i];
    }
    return dot / (MathF.Sqrt(magX) * MathF.Sqrt(magY));
}


