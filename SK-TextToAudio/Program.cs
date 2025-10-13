#pragma warning disable 
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToAudio;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var arr = File.ReadLines("C:\\gpt\\azure_tts_key.txt").ToArray();
var endpoint = arr[1];
var deploymentName = arr[0];
var key = arr[2];

var kernel = Kernel.CreateBuilder()
         .AddAzureOpenAITextToAudio(
             deploymentName: deploymentName,
             endpoint: endpoint,
             apiKey: key)
         .Build();
var textToAudioService = kernel.GetRequiredService<ITextToAudioService>();

string sampleText = """
有一天，小明去庙里拜佛。
他点了三炷香，磕头念道：
“佛祖啊佛祖，求您让我中彩票吧！”

第二天，小明又来了，磕头说：
“佛祖啊佛祖，您怎么还没让我中呢？”

第三天，他又来，佛祖终于忍不住显灵，对他说：
“孩子，我可以保佑你，但你得先去——买——一——张——票！
""";
OpenAITextToAudioExecutionSettings executionSettings = new()
{
    Voice = "alloy", // 支持的声音有 alloy, echo, fable, onyx, nova, and shimmer.
    ResponseFormat = "mp3",
    Speed = 1.0f
};

AudioContent audioContent = await textToAudioService.GetAudioContentAsync(sampleText, executionSettings);
await File.WriteAllBytesAsync("output.mp3", audioContent.Data.Value.ToArray());
Console.ReadLine();