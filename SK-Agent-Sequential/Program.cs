using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.Sequential;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;
using System.ComponentModel;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.Agents.AzureAI;
using System.ClientModel;
using OpenAI.Files;
using Microsoft.SemanticKernel.TextToImage;
using Microsoft.SemanticKernel.Connectors.OpenAI;


#pragma warning disable

// var test = """
//  Display this data using a bar-chart (not stacked):

// [{"Name":"Strawberry","Price":11,"Quantity":20},{"Name":"Dragon Fruit","Price":11,"Quantity":100},{"Name":"Lychee","Price":12,"Quantity":100},{"Name":"Coconut","Price":13,"Quantity":100}]
// """;
// var c=new ChartGenerator();
// var r= await c.GenerateChart(test);
// Console.WriteLine(r);
// return;

var arr = File.ReadLines("C://gpt/azure_key.txt").ToArray();
var endpoint = arr[1];
var deploymentName = arr[0];
var key = arr[2];

var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(deploymentName, endpoint, key)
            .Build();
kernel.Plugins.AddFromType<StoreSystem>("StoreSystem");
kernel.Plugins.AddFromType<ImageShower>("ImageShower");
kernel.Plugins.AddFromType<ChartGenerator>("ChartGenerator");


ChatCompletionAgent dataQuerier = new ChatCompletionAgent()
{
    Name = "DataQuerier",
    Instructions =
            """
            你是一名数据查询专家。根据用户要求，查询到准确的数据：
            - 要求只返回json格式的数据
            - 确保数据的完整性和准确性
            - 如果有中文名称，翻译成英文名称
            - 数据格式示例: [{"Name":"Apple","Price":5,"Quantity":100},{"Name":"Banana","Price":3,"Quantity":200}]
            """,
    Description = "一个能从数据中提取关键信息的智能代理（Agent）。",
    Kernel = kernel,
    Arguments = new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }),
};

ChatCompletionAgent charter = new ChatCompletionAgent()
{
    Name = "Charter",
    Instructions =
            """        
            你是图表生成代理。根据数据json，生成专业图表：  
            - 要求只返回图表的url或本地路径          
            """,
    Description = "一个根据数据生成专业图表的智能代理。",
    Kernel = kernel,
    Arguments = new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }),
};

ChatCompletionAgent imageViewer = new ChatCompletionAgent()
{
    Name = "ImageShower",
    Instructions = """
                你是一名图像展示专家。判断图片的路径不为空，再根据提供的图像url或本地路径，调用图像展示工具显示                
                """,
    Description = "一个展示图像的智能代理。",
    Kernel = kernel,
    Arguments = new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() }),
};

var orchestration = new SequentialOrchestration(dataQuerier, charter, imageViewer)
{
    ResponseCallback = (res) =>
    {
        Console.WriteLine(res.AuthorName + ":" + res.Content);
        return ValueTask.CompletedTask;
    },
    LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole()),
};

InProcessRuntime runtime = new();
await runtime.StartAsync();

while (true)
{
    Console.WriteLine("请输入你的需求：");
    string? input = "价格大于10块的水果有什么?";
    Console.ReadLine();

    OrchestrationResult<string> result = await orchestration.InvokeAsync(input, runtime);

    string text = await result.GetValueAsync(TimeSpan.FromSeconds(3000));
    Console.WriteLine($"\n# RESULT: {text}");
    await runtime.RunUntilIdleAsync();

}

public class ChartGenerator
{
    [KernelFunction]
    [Description("生成图表图片")]
    public async Task<string> GenerateChart([Description("图表描述")] string chartDescription)
    {
        var key = File.ReadAllText("C:\\gpt\\key.txt");
        var builder = Kernel.CreateBuilder()
                   .AddOpenAITextToImage(
                       apiKey: key,
                       modelId: "dall-e-3");

        var kernel = builder.Build();
        var service = kernel.GetRequiredService<ITextToImageService>();

        var generatedImages = await service.GetImageContentsAsync(
            new TextContent($"根据下面内容生成适全的图表：\n{chartDescription}"),
            new OpenAITextToImageExecutionSettings { Size = (Width: 1792, Height: 1024) });

        return generatedImages[0].Uri!.ToString();
    }


    // [KernelFunction]
    // [Description("生成图表图片")]
    // public async Task<string> GenerateChart([Description("生成图表的数据，只接收json格式的数据")] string chartJson)
    // {
    //     var arr = File.ReadLines("C://gpt/azure_key.txt").ToArray();
    //     var endpoint = arr[1];
    //     var deploymentName = arr[0];
    //     var key = arr[2];

    //     var Client = OpenAIAssistantAgent.CreateAzureOpenAIClient(
    //         new ApiKeyCredential(key), new Uri(endpoint));

    //     var AssistantClient = Client.GetAssistantClient();        
    //     var assistant =
    //         await AssistantClient.CreateAssistantAsync("gpt-4.1",
    //            "ChartMaker",
    //             instructions: "Create charts as requested without explanation.",
    //                     enableCodeInterpreter: true);
  
    //     OpenAIAssistantAgent agent = new(assistant, AssistantClient);
    //     AgentThread? agentThread = null;
    //     try
    //     {
    //         var result = await InvokeAgentAsync($""" 
    //         Display this data using a bar-chart (not stacked):
    //         {chartJson}
    //         """);
    //         return "生成图表图片成功，图片路径：" + result;
    //     }
    //     finally
    //     {
    //         if (agentThread is not null)
    //         {
    //             await agentThread.DeleteAsync();
    //         }

    //         await AssistantClient.DeleteAssistantAsync(agent.Id);
    //     }

    //     async Task<string> InvokeAgentAsync(string input)
    //     {
    //         ChatMessageContent message = new(AuthorRole.User, input);
    //         await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(message))
    //         {
    //             var imagePath = await DownloadResponseImageAsync(response);
    //             if (!string.IsNullOrEmpty(imagePath))
    //             {
    //                 return imagePath;
    //             }
    //             agentThread = response.Thread;
    //         }
    //         return string.Empty;
    //     }
    //     async Task<string> DownloadResponseImageAsync(ChatMessageContent message)
    //     {
    //         var fileClient = Client.GetOpenAIFileClient();
    //         foreach (KernelContent item in message.Items)
    //         {
    //             if (item is FileReferenceContent fileReference)
    //             {
    //                 var imagePath = await DownloadFileContentAsync(fileClient, fileReference.FileId);
    //                 if (!string.IsNullOrEmpty(imagePath))
    //                 {
    //                     return imagePath;
    //                 }
    //             }
    //         }
    //         return string.Empty;
    //     }
    //     async Task<string> DownloadFileContentAsync(OpenAIFileClient fileClient, string fileId)
    //     {
    //         OpenAIFile fileInfo = fileClient.GetFile(fileId);
    //         if (fileInfo.Purpose == FilePurpose.AssistantsOutput)
    //         {
    //             string filePath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(fileInfo.Filename));              
    //             BinaryData content = await fileClient.DownloadFileAsync(fileId);
    //             File.WriteAllBytes(filePath, content.ToArray());
    //             Console.WriteLine($"图表已生成，保存在：{filePath}");
    //             return filePath;
    //         }
    //         return string.Empty;
    //     }
    // }
}

public class ImageShower
{
    [KernelFunction]
    [Description("显示网络图片")]
    public string ShowImage([Description("图片URL")] string imageUrl)
    {
        //调用本地图片显示程序显示图片
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = imageUrl,
            UseShellExecute = true
        });

        return "图片显示成功";
    }

    [KernelFunction]
    [Description("显示本地图片")]
    public string ShowImageLocal([Description("本地图片路径")] string imagePath)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/C start {imagePath}"
        });
        return "图片显示成功";
    }
}

public class StoreSystem
{
    public List<Goods> GoodsList { get; set; } = new List<Goods>
        {
            new Goods("苹果",5,100),
            new Goods("香蕉",3,200),
            new Goods("橙子",4,150),
            new Goods("桃子",6,120),
            new Goods("梨",5,100),
            new Goods("葡萄",7,80),
            new Goods("西瓜",8,60),
            new Goods("菠萝",9,40),
            new Goods("芒果",10,30),
            new Goods("草莓",11,20),
            new Goods("柠檬",4,100),
            new Goods("橘子",3,100),
            new Goods("蓝莓",6,100),
            new Goods("樱桃",7,100),
            new Goods("葡萄柚",8,100),
            new Goods("柚子",9,100),
            new Goods("榴莲",10,100),
            new Goods("火龙果",11,50),
            new Goods("荔枝",12,30),
            new Goods("椰子",13,60),
            new Goods("桑葚",5,100),
            new Goods("杨梅",4,100),
            new Goods("树梅",6,100),
            new Goods("莓子",7,100),
            new Goods("石榴",8,100),
            new Goods("蜜桃",9,100),
        };
    public decimal Total { get; set; } = 0;
    [KernelFunction]
    [Description("按照水果名称(Name)查询水果")]
    public string GetGoodsByName([Description("水果名称")] string name)
    {
        return GoodsList.FirstOrDefault(g => g.Name == name)?.ToString() ?? "未找到水果";
    }
    [KernelFunction]
    [Description("查询单价(Price)少于等于参数的所有水果")]
    public string GetGoodsLessEqualsPrice([Description("水果单价")] decimal price)
    {
        var goodses = GoodsList.Where(g => g.Price <= price);
        if (goodses == null || goodses.Any() == false)
        {
            return "未找到水果";
        }
        else
        {
            return string.Join("\n", goodses);
        }
    }
    [Description("查询单价(Price)少于参数的所有水果")]
    public string GetGoodsLessPrice([Description("水果单价")] decimal price)
    {
        var goodses = GoodsList.Where(g => g.Price < price);
        if (goodses == null || goodses.Any() == false)
        {
            return "未找到水果";
        }
        else
        {
            return string.Join("\n", goodses);
        }
    }
    [KernelFunction]
    [Description("查询单价(Price)大于等于参数的所有水果")]
    public string GetGoodsGreaterEqualsPrice([Description("水果单价")] decimal price)
    {
        var goodses = GoodsList.Where(g => g.Price >= price);
        if (goodses == null || goodses.Any() == false)
        {
            return "未找到水果";
        }
        else
        {
            return string.Join("\n", goodses);
        }
    }
    [KernelFunction]
    [Description("查询单价(Price)大于参数的所有水果")]
    public string GetGoodsGreaterPrice([Description("水果单价")] decimal price)
    {
        var goodses = GoodsList.Where(g => g.Price > price);
        if (goodses == null || goodses.Any() == false)
        {
            return "未找到水果";
        }
        else
        {
            return string.Join("\n", goodses);
        }
    }

    [KernelFunction]
    [Description("查询库存数量(Quantity)大于等于参数的所有水果")]
    public string GetGoodsGreaterEqualsQuantity([Description("水果库存数量")] int quantity)
    {
        var goodses = GoodsList.Where(g => g.Quantity >= quantity);
        if (goodses == null || goodses.Any() == false)
        {
            return "未找到水果";
        }
        else
        {
            return string.Join("\n", goodses);
        }
    }

    [KernelFunction]
    [Description("查询库存数量(Quantity)大于参数的所有水果")]
    public string GetGoodsGreaterQuantity([Description("水果库存数量")] int quantity)
    {
        var goodses = GoodsList.Where(g => g.Quantity > quantity);
        if (goodses == null || goodses.Any() == false)
        {
            return "未找到水果";
        }
        else
        {
            return string.Join("\n", goodses);
        }
    }
    [KernelFunction]
    [Description("查询库存数量(Quantity)少于等于参数的所有水果")]
    public string GetGoodsLessEqualsQuantity([Description("水果数量")] int quantity)
    {
        var goodses = GoodsList.Where(g => g.Quantity <= quantity);
        if (goodses == null || goodses.Any() == false)
        {
            return "未找到水果";
        }
        else
        {
            return string.Join("\n", goodses);
        }
    }
    [KernelFunction]
    [Description("查询库存数量(Quantity)少于参数的所有水果")]
    public string GetGoodsLessQuantity([Description("水果数量")] int quantity)
    {
        var goodses = GoodsList.Where(g => g.Quantity < quantity);
        if (goodses == null || goodses.Any() == false)
        {
            return "未找到水果";
        }
        else
        {
            return string.Join("\n", goodses);
        }
    }
    [KernelFunction]
    [Description("购买水果")]
    public string BuyGoods([Description("水果名称")] string name, [Description("购买数量")] int quantity)
    {
        var goods = GoodsList.FirstOrDefault(g => g.Name == name);
        if (goods != null)
        {
            var newQuantity = goods.Quantity - quantity;
            if (newQuantity < 0)
            {
                return "库存不足";
            }
            else
            {
                goods.Quantity = newQuantity;
                goods.BuyQuantity += quantity;
                Total += goods.Price * quantity;
                return "购买成功！";
            }
        }
        else
        {
            return "未找到水果";
        }
    }
}
public class Goods
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int BuyQuantity { get; set; } = 0;
    public Goods(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }
    public override string ToString()
    {
        return $"名称:{Name},单价:{Price},库存数量:{Quantity},已购数量:{BuyQuantity}";
    }
}