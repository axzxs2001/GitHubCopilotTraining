using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

#pragma warning disable 

var arr = File.ReadLines("C://gpt/azure_key.txt").ToArray();
var endpoint = arr[1];
var deploymentName = arr[0];
var key = arr[2];


IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(
    deploymentName: "text-embedding-ada-002",
    endpoint: endpoint,
    apiKey: key
);
Kernel kernel = kernelBuilder.Build();

var embeddingGenerator = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

string[] documents = new[]
{
    "苹果",
    "土豆",
    "apple",
    "苹果手表"
};

var vectorStore = new Dictionary<string, ReadOnlyMemory<float>>();
foreach (var doc in documents)
{
    var embedding = await embeddingGenerator.GenerateEmbeddingAsync(doc);
    vectorStore[doc] = embedding;
}

await QueryAsync("水果",embeddingGenerator, vectorStore);
await QueryAsync("蔬菜", embeddingGenerator, vectorStore);
await QueryAsync("电子设备", embeddingGenerator,vectorStore);
// 搜索查询
static async Task QueryAsync(string query,ITextEmbeddingGenerationService embeddingGenerator,Dictionary<string, ReadOnlyMemory<float>> vectorStore)
{
    var queryEmbedding = await embeddingGenerator.GenerateEmbeddingAsync(query);
    // 计算余弦相似度并排序
    var results = vectorStore
        .Select(kvp => new
        {
            Document = kvp.Key,
            Similarity = CosineSimilarity(queryEmbedding.Span, kvp.Value.Span)
        })
        .OrderByDescending(x => x.Similarity);

    Console.WriteLine($"查询: {query}\n");
    foreach (var result in results)
    {
        Console.WriteLine($"相似度: {result.Similarity:F4} - {result.Document}");
    }
    Console.WriteLine("--------------------------\n");
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


