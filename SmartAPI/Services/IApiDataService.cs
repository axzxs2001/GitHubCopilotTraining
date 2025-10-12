using SmartAPI.Models;
using SmartAPI.Respositories;

namespace SmartAPI.Services
{
    public interface IApiDataService
    {
        Task<IEnumerable<ApiData>> GetApiDatasAsync(int productID, CancellationToken cancellationToke);
        Task<IEnumerable<ApiData>> GetApiDatasByCategoryAsync(int productID, CancellationToken cancellationToken);
        Task<ApiData> GetApiDataAsync(int id, CancellationToken cancellationToke);
        Task<ApiData> AddApiDataAsync(ApiData apiData, CancellationToken cancellationToke);
        Task<bool> ModifyApiDataAsync(ApiData apiData, CancellationToken cancellationToke);
        Task<bool> RemoveApiDataAsync(int id, CancellationToken cancellationToke);
        Task<bool> CopyAPIsAsync(int oldProductID, int newProductID, CancellationToken cancellationToken);
        IAsyncEnumerable<RunResult> RunCodeAsync(RunParmeter runParmeter, CancellationToken cancellationToken);
        IAsyncEnumerable<string> GenerateCode(string requestID, APIQuest apiQuestion, CancellationToken cancellationToken);
        Task<string> CallBackAsync(string body, Dictionary<string, Dictionary<string, string>> requestParmeters, CancellationToken cancellationToken);

        Task<string> GetCallBackResponse(string connectionID, string requestJson, CancellationToken cancellationToken);
    }
}
