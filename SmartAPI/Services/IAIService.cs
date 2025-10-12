using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using SmartAPI.Models;
using SmartAPI.Respositories;
using System;
using System.Reflection.Emit;

namespace SmartAPI.Services
{
    public interface IAIService
    {
        Task<string?> ChatAsync(string systemMessage, string question, CancellationToken cancellationToken);

        IAsyncEnumerable<string?> StreamingChatAsync(string requestID, string systemMessage, string question, Dictionary<string, string> referenceAnswers, CancellationToken cancellationToken);

        IAsyncEnumerable<string?> StreamingChatAsync(string requestID, string systemMessage, string question, List<string> referenceAnswers, CancellationToken cancellationToken);

        //Task<bool> EmbeddingVectoryAsync(APIVectorRecord record);
        //IAsyncEnumerable<string> VectorizedSearchAsync(string searchContent);
    }
}
