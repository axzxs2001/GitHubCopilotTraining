#pragma warning disable 
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var arr = File.ReadLines("C:\\gpt\\azure_tts_key.txt").ToArray();
var endpoint = arr[1];
var deploymentName = arr[0];
var key = arr[2];
var builder = Kernel.CreateBuilder()
           .AddAzureOpenAITextToImage(
               deploymentName: "dall-e-3",
               endpoint: endpoint,
               apiKey: key,
               modelId: "dall-e-3");

var kernel = builder.Build();
var service = kernel.GetRequiredService<ITextToImageService>();

var generatedImages = await service.GetImageContentsAsync(
    new TextContent("雄伟的大雪山下，一只可爱的柴犬戴着红色围巾，阳光明媚，高清摄影"),
    new OpenAITextToImageExecutionSettings { Size = (Width: 1792, Height: 1024) });

Console.WriteLine(generatedImages[0].Uri!.ToString());
System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
{
    FileName = generatedImages[0].Uri!.ToString(),
    UseShellExecute = true
});

Console.ReadLine();