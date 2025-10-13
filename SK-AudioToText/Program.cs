
#pragma warning disable 
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var arr = File.ReadLines("C:\\gpt\\azure_key.txt").ToArray();
var endpoint = arr[1];
var deploymentName = arr[0];
var key = arr[2];

var kernel = Kernel.CreateBuilder()
         .AddAzureOpenAIAudioToText(
             deploymentName: "whisper",
             endpoint: endpoint,
             apiKey: key)
         .Build();
var audioToTextService = kernel.GetRequiredService<IAudioToTextService>();
var AudioFilename = "audio.mp3";
OpenAIAudioToTextExecutionSettings executionSettings = new  OpenAIAudioToTextExecutionSettings(AudioFilename)
{
    Language = "zh", 
    Prompt = "提取出文字内容",   
    Temperature = 0.3f, 
};
await using var audioFileStream = File.OpenRead(AudioFilename);
var audioFileBinaryData = await BinaryData.FromStreamAsync(audioFileStream);
AudioContent audioContent = new(audioFileBinaryData, mimeType: null);
var textContent = await audioToTextService.GetTextContentAsync(audioContent,executionSettings);

Console.WriteLine(textContent.Text);
Console.ReadLine();