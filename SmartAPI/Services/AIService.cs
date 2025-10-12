#pragma warning disable SKEXP0001

using Amazon.Runtime.Internal.Transform;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Amazon;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SmartAPI.Models;
using SmartAPI.Respositories;
using System.Text;

#pragma warning disable
namespace SmartAPI.Services
{
    public class AIService(ILogger<AIService> logger, IConfiguration configuration, AIConfig aiConfig, Kernel kernel, IProductRepository productRepository, IApiDataRepository apiDataRepository) : IAIService
    {

        ChatHistory _chatHistory;
        public async Task<string?> ChatAsync(string systemMessage, string question, CancellationToken cancellationToken)
        {
            _chatHistory = new ChatHistory(systemMessage);
            _chatHistory.AddUserMessage(question);

            PromptExecutionSettings settings = null;
            switch (aiConfig.UserModel)
            {
                case "GPT":
                    settings = new OpenAIPromptExecutionSettings
                    {
                        MaxTokens = 3500,
                        Temperature = 0.7,
                        TopP = 1.0,
                        PresencePenalty = 0.0,
                        FrequencyPenalty = 0.0,
                    };
                    break;
                case "Bedrock":
                    settings = new PromptExecutionSettings
                    {
                        ExtensionData = new Dictionary<string, object>()
                        {
                            { "temperature", 0.7f },
                            { "top_p", 1.0f },
                            { "max_tokens_to_sample", 3500 }
                        }
                    };
                    break;
                default:
                    settings = new PromptExecutionSettings
                    {
                        ExtensionData = new Dictionary<string, object>()
                        {
                            { "temperature", 0.7f },
                            { "top_p", 1.0f },
                            { "max_tokens_to_sample", 1510 }
                        }
                    };
                    break;
            }

            var replay = await kernel.GetRequiredService<IChatCompletionService>().GetChatMessageContentAsync(_chatHistory, settings);
            _chatHistory?.AddAssistantMessage(replay.Content);
            logger.LogInformation("查询完成：{0}", replay.Content);
            return replay.Content;
        }
        public async IAsyncEnumerable<string?> StreamingChatAsync(string requestID, string systemMessage, string question, Dictionary<string, string> referenceAnswers, CancellationToken cancellationToken)
        {
            _chatHistory = new ChatHistory(systemMessage);
            foreach (var referenceAnswer in referenceAnswers)
            {
                _chatHistory.AddUserMessage($"{referenceAnswer.Key}：\r\n{referenceAnswer.Value}");
            }
            if (referenceAnswers.Count > 0)
            {
                _chatHistory.AddUserMessage("-------------------");
            }
            _chatHistory.AddUserMessage(question);
            var subAnswer = new StringBuilder();

            PromptExecutionSettings settings = null;
            switch (aiConfig.UserModel)
            {
                case "GPT":
                    settings = new OpenAIPromptExecutionSettings
                    {
                        MaxTokens = 3500,
                        Temperature = 0.7,
                        TopP = 1.0,
                        PresencePenalty = 0.0,
                        FrequencyPenalty = 0.0,
                    };
                    break;
                case "Bedrock":
                    settings = new PromptExecutionSettings
                    {
                        ExtensionData = new Dictionary<string, object>()
                        {
                            { "temperature", 0.7f },
                            { "top_p", 1.0f },
                            { "max_tokens_to_sample", 3500 }
                        }
                    };
                    break;
                default:
                    settings = new PromptExecutionSettings
                    {
                        ExtensionData = new Dictionary<string, object>()
                        {
                            { "temperature", 0.7f },
                            { "top_p", 1.0f },
                            { "max_tokens_to_sample", 1510 }
                        }
                    };
                    break;
            }  
            await foreach (var item in kernel.GetRequiredService<IChatCompletionService>().GetStreamingChatMessageContentsAsync(_chatHistory, settings))
            {
                if (item == null)
                {
                    continue;
                }
                subAnswer.Append(item);
                yield return item.Content;
            }
            _chatHistory?.AddAssistantMessage(subAnswer.ToString());

            logger.LogInformation("查询完成：{0}", subAnswer.ToString());
        }
        public async IAsyncEnumerable<string?> StreamingChatAsync(string requestID, string systemMessage, string question, List<string> referenceAnswers, CancellationToken cancellationToken)
        {
            _chatHistory = new ChatHistory(systemMessage);
            foreach (var referenceAnswer in referenceAnswers)
            {
                _chatHistory.AddUserMessage(referenceAnswer);
            }
            if (referenceAnswers.Count > 0)
            {
                _chatHistory.AddUserMessage("-------------------");
            }
            _chatHistory.AddUserMessage(question);
            var subAnswer = new StringBuilder();
            PromptExecutionSettings settings = null;
            switch (aiConfig.UserModel)
            {
                case "GPT":
                    settings = new OpenAIPromptExecutionSettings
                    {
                        MaxTokens = 3500,
                        Temperature = 0.7,
                        TopP = 1.0,
                        PresencePenalty = 0.0,
                        FrequencyPenalty = 0.0,
                    };
                    break;
                case "Bedrock":
                    settings = new PromptExecutionSettings
                    {
                        ExtensionData = new Dictionary<string, object>()
                        {
                            { "temperature", 0.7f },
                            { "top_p", 1.0f },
                            { "max_tokens_to_sample", 1510 }
                        }
                    };
                    break;
                default:   
                    settings = new PromptExecutionSettings
                    {
                        ExtensionData = new Dictionary<string, object>()
                        {
                            { "temperature", 0.7f },
                            { "top_p", 1.0f },
                            { "max_tokens_to_sample", 1510 }
                        }
                    };
                    break;             
            }
            await foreach (var item in kernel.GetRequiredService<IChatCompletionService>().GetStreamingChatMessageContentsAsync(_chatHistory, settings))
            {
                if (item == null)
                {
                    continue;
                }
                subAnswer.Append(item);
                yield return item.Content;
            }
            _chatHistory?.AddAssistantMessage(subAnswer.ToString());
            logger.LogInformation("查询完成：{0}", subAnswer.ToString());
        }

    }

}
