#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0052
#pragma warning disable SKEXP0020
using Amazon;
using Amazon.BedrockRuntime;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Amazon;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Redis;
using Microsoft.SemanticKernel.Memory;
using SmartAPI.Models;
using SmartAPI.Services;
using StackExchange.Redis;
using System.Configuration;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

#pragma warning disable

namespace SmartAPI
{

    //docker run -d --name redis-stack-server -p 6379:6379 -e REDIS_PASSWORD=root123 redis/redis-stack-server:latest

    public static class AIExtension
    {
        public static async Task AddAIAsync(this WebApplicationBuilder build)
        {
            var secretAccessKey = build.Configuration.GetSection("SecretAccessKey").Value;
            var openAIKey = build.Configuration.GetSection("OpenAIKey").Value;
            var geminiKey = build.Configuration.GetSection("GeminiKey").Value;
            var chatModelId = build.Configuration.GetSection("AIConfig:GPT:ChatModelId").Value;
            var embeddingId = build.Configuration.GetSection("AIConfig:GPT:EmbeddingId").Value;
            var userModel = build.Configuration.GetSection("AIConfig:UserModel").Value;
            var aiConfig = new AIConfig();
            build.Configuration.Bind("AIConfig", aiConfig);
            build.Services.AddSingleton(aiConfig);

            if (string.IsNullOrWhiteSpace(openAIKey) || string.IsNullOrWhiteSpace(chatModelId) || string.IsNullOrWhiteSpace(embeddingId))
            {
                throw new Exception("AI配置信息不完整");
            }
          //  IChatCompletionService chatCompletionService = null;
            Kernel kernel = null;
            switch (userModel)
            {
                case "GPT":
                   // chatCompletionService = new OpenAIChatCompletionService(chatModelId!, openAIKey!);
                    kernel = Kernel.CreateBuilder()
                        .AddOpenAIChatCompletion(chatModelId, openAIKey).Build();
                    break;
                case "Bedrock":
                    var runtime = new AmazonBedrockRuntimeClient(aiConfig.Bedrock.AccesskeyID, secretAccessKey, RegionEndpoint.USWest2);
                   // chatCompletionService = new BedrockChatCompletionService(aiConfig.Bedrock.ModelID, runtime);
                    kernel = Kernel.CreateBuilder()
                        .AddBedrockChatCompletionService(aiConfig.Bedrock.ModelID, runtime).Build();
                    break;
                case "Gemini":
                    kernel = Kernel.CreateBuilder()
                        .AddGoogleAIGeminiChatCompletion(aiConfig.Gemini.ChatModelId, geminiKey).Build();
                    break;
            }

            build.Services.AddSingleton<Kernel>(kernel);
           // build.Services.AddSingleton<IChatCompletionService>(chatCompletionService);
            build.Services.AddScoped<IGoogleCloudService, GoogleCloudService>();



            //   var redisConfig = new RedisConfig();
            //   build.Configuration.Bind("RedisConfig", redisConfig);
            //   var redisConfiguration = new ConfigurationOptions
            //   {
            //       EndPoints = { redisConfig.Host },
            //       User = redisConfig.User,
            //       Password = redisConfig.Password,
            //       ConnectTimeout = redisConfig.ConnectTimeout,
            //       ConnectRetry = 3
            //   };
            //   //添加redis配置
            //   build.Services.AddStackExchangeRedisCache(opt =>
            //   {
            //       opt.ConfigurationOptions = redisConfiguration;
            //   });
            //   var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(redisConfiguration);
            //   var database = connectionMultiplexer.GetDatabase();
            //   var vectorStore = new RedisVectorStore(database);
            //   IVectorStoreRecordCollection<string, APIVectorRecord> apiVectorRecords = vectorStore.GetCollection<string, APIVectorRecord>("apidata");
            //   await apiVectorRecords.CreateCollectionIfNotExistsAsync();
            //   build.Services.AddSingleton(apiVectorRecords);
            //   IEmbeddingGenerator<string, Embedding<float>> generator =
            //new OpenAIEmbeddingGenerator(new OpenAI.OpenAIClient(openAIKey), "text-embedding-3-small");
            //   build.Services.AddSingleton(generator);
        }
        static async Task<IMemoryStore> CreateRedisMemoryStoreAsync(ConfigurationOptions redisConfiguration, int vectorSize)
        {
            var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(redisConfiguration);
            var database = connectionMultiplexer.GetDatabase();
            var store = new RedisMemoryStore(database, vectorSize: vectorSize);
            return store;
        }
    }
}
