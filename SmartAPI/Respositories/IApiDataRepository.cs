using Dapper;
using MySql.Data.MySqlClient;
using SmartAPI.Models;

namespace SmartAPI.Respositories
{
    public interface IApiDataRepository
    {
        Task<IEnumerable<ApiData>> GetApiDatasAsync(int productID, CancellationToken cancellationToken);
        Task<IEnumerable<ApiData>> GetApiDatasAsync(int productID, int parentID, CancellationToken cancellationToken);
        Task<IEnumerable<ApiData>> GetApiDatasByCategoryAsync(int productID, string[] categoryNames, CancellationToken cancellationToken);
        Task<ApiData> GetApiDataAsync(int id, CancellationToken cancellationToken);
        Task<ApiData> CreateApiDataAsync(ApiData apiData, CancellationToken cancellationToken);
        Task<int> UpdateApiDataAsync(ApiData apiData, CancellationToken cancellationToken);
        Task<int> DeleteApiDataAsync(int id, CancellationToken cancellationToken);
        Task<ApiData> GetApiDataByCategoryAsync(int productID, string apiCategoryType, CancellationToken cancellationToken);
    }
}
