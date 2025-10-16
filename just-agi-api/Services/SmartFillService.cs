using HtmlAgilityPack;
using just_agi_api.Models;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Text;
using just_agi_api.IServices;
using Microsoft.Extensions.Caching.Memory;
using just_agi_api.Repositories;
using just_agi_api.IRepositories;
using static System.Net.Mime.MediaTypeNames;


#pragma warning disable SKEXP0001
namespace just_agi_api.Services
{
    public class SmartFillService : ISmartFillService
    {
        private readonly AIConfig _aiConfig;
        private readonly ILogger<SmartFillService> _logger;
        private readonly IMemoryCache _cache;
        private readonly ISmartFillRepository _smartFillRepository;
        public SmartFillService(AIConfig aiConfig, ILogger<SmartFillService> logger, IMemoryCache cache, ISmartFillRepository smartFillRepository)
        {
            _aiConfig = aiConfig;
            _logger = logger;
            _cache = cache;
            _smartFillRepository = smartFillRepository;
        }
        async Task<string?> AudioToTextAsync(string audioFilePath)
        {
            try
            {
                var kernel = Kernel.CreateBuilder()
                    .AddOpenAIAudioToText(
                        modelId: "whisper-1",
                        apiKey: _aiConfig.OpenAIConfig.Key)
                    .Build();

                var audioToTextService = kernel.GetRequiredService<IAudioToTextService>();
                var executionSettings = new OpenAIAudioToTextExecutionSettings(audioFilePath)
                {
                    Language = "zh",
                    Prompt = "给出简体中文的文本",
                    ResponseFormat = "json",
                    Temperature = 0.3f,
                    Filename = "audio.wav"
                };


                ReadOnlyMemory<byte> audioData = await File.ReadAllBytesAsync(audioFilePath);
                var audioContent = new AudioContent(new BinaryData(audioData));
                var textContent = await audioToTextService.GetTextContentAsync(audioContent, executionSettings);
                _logger.LogInformation(textContent.Text);
                File.Delete(audioFilePath);
                return textContent?.Text;


            }
            catch (Exception exc)
            {
                return exc.Message;
            }
        }
        public async Task<string?> ContentToJsonAsync(AnswerEntity answer)
        {
            var smartFillUser = await _smartFillRepository.GetSmartFillUserAsync(answer.UserUrl);
            if (smartFillUser == null)
            {
                return null;
            }
            var content = await GetUrlContentAsync(answer.Answer);
            _logger.LogInformation(content);
            if (string.IsNullOrWhiteSpace(content.Trim()))
            {
                _logger.LogError("没有获取到网页内容！");

            }
            return await GetJsonAsync(content, smartFillUser);
        }
        async Task<string> GetUrlContentAsync(string question)
        {
            using (var client = new HttpClient())
            {
                var pattern = @"(https?://)([\da-z\.-]+)\.([a-z\.]{2,6})([/\w \.-]*)*/?";
                var reg = new RegularExpressionAttribute(pattern);
                var regex = new Regex(pattern);

                var matches = regex.Matches(question);
                var contentBuilder = new StringBuilder();
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        var content = await GetContentByUrlAsync(match.Value);
                        contentBuilder.Append(content);
                    }
                    return contentBuilder.ToString();

                }
                else
                {
                    return question;
                }
            }
        }
        async Task<string> GetContentByUrlAsync(string url, bool isHtml = false)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var client = new HttpClient())
            {
                // 发送 GET 请求
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // 读取响应内容为字符串
                var responseBody = await response.Content.ReadAsStringAsync();

                // 创建 HtmlDocument 对象
                var doc = new HtmlDocument();
                doc.LoadHtml(responseBody);

                // 移除不需要的标签内容，如 JavaScript 或 CSS
                RemoveTag(doc, "script");
                RemoveTag(doc, "style");
                RemoveTag(doc, "noscript");

                // 使用 XPath 查询找到所有的文本节点，忽略样式和脚本标签
                
                var nodes = doc.DocumentNode.SelectNodes(isHtml?"//body":"//body//text()[normalize-space()]");
                var content = new StringBuilder();
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        if (isHtml)
                        {
                            content.AppendLine(node.InnerHtml.Trim().Replace(" ","").Replace("/r","").Replace("/n",""));
                        }
                        else
                        {
                            content.AppendLine(node.InnerText.Trim());
                        }
                    }
                }
                return content.ToString();
            }
        }
        void RemoveTag(HtmlDocument document, string tagName)
        {
            var nodes = document.DocumentNode.SelectNodes($"//{tagName}");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    node.Remove();
                }
            }
        }
        async Task<string?> GetJsonAsync(string text, SmartFillUser smartFillUser)
        {
            var subPrompt = new StringBuilder();
            foreach (var userUIData in smartFillUser.UserData)
            {
                subPrompt.AppendLine($"{userUIData.AttrTitle}({userUIData.AttrValue}),");
            }

            var prompt = @$"
角色：
你是一位高级内容分析专家，能理解内容的含意，并且你有很强的理解能力，有优秀的数据报取能力和分类能力。
任务：
1、你能从User输入的内容中，提取以下数据项：{subPrompt.ToString().TrimEnd(',')}等信息。
2、把提取的数据项，组装成一个Json字符串，如果信息中没有对应数据项，请不要组装在Json。
格式：
直接输出的结果是纯Json字符串，不含```json这样的信息。";
            _logger.LogInformation("Prompt:{0}", prompt);

            var chatCompletionService = new OpenAIChatCompletionService(_aiConfig.OpenAIConfig.ModelID, _aiConfig.OpenAIConfig.Key);
            var chatHistory = new ChatHistory(prompt);
            chatHistory.AddUserMessage(text);
            var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
            return result.Content;

        }
        public async Task<string> StarRecordAsync(AudioEntity audio)
        {


            var guid = "";
            if ((string.IsNullOrWhiteSpace(audio.ID)))
            {
                guid = Guid.NewGuid().ToString("N");
                _cache.Set<byte[]>(guid, Convert.FromBase64String(audio.Data));
            }
            else
            {
                guid = audio.ID;
                var oldBytes = new List<byte>(_cache.Get<byte[]>(guid));
                var newBytes = Convert.FromBase64String(audio.Data);
                oldBytes.AddRange(newBytes);
                _cache.Set<byte[]>(guid, oldBytes.ToArray());
            }
            return await Task.FromResult(guid);
        }
        public async Task<string?> StopRecordAsync(AudioEntity audio)
        {
            var oldBytes = _cache.Get<byte[]>(audio.ID);
            var fielPath = Directory.GetCurrentDirectory() + $"/wwwroot/audios/{audio.ID}.mp3";
            using var file = new FileStream(fielPath, FileMode.Create);
            await file.WriteAsync(oldBytes);
            await file.FlushAsync();
            file.Close();
            await file.DisposeAsync();
            return await AudioToTextAsync(fielPath);

        }

        public async Task<string?> GetJsonByUrl(string url)
        {
            var html = await GetContentByUrlAsync(url,true);
            var prompt = @$"你是一个html专家，为了汇总输入标签特征，请分析user给出的html代码，找出输入类型的标签，如果是input标签，不包含type=hide，type=button，type=submit类型的标签：标签的类型作为tagType的值；如果是input标签，把type属性的值为inputType的值；为了确定标签的唯一性，查看标签是否有id，name，class属性，从这三个中选择一个作为attrName的值，如attrName=""id""；id或name或class属性的值作为attrValue的值；输入类型标签的描述或功能名称作为attrTitle的值。要求只输出标签集合的Json，不需要其他说明信息。";
            _logger.LogInformation("Prompt:{0}", prompt);

            var chatCompletionService = new OpenAIChatCompletionService(_aiConfig.OpenAIConfig.ModelID, _aiConfig.OpenAIConfig.Key);
            var chatHistory = new ChatHistory(prompt);
            chatHistory.AddUserMessage(html);
            var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
            return result.Content;

        }

        public async Task<SmartFillUser> AddSmartFillUserAsync(SmartFillUser smartFillUser)
        {
            return await _smartFillRepository.AddSmartFillUserAsync(smartFillUser);
        }
    }
}

