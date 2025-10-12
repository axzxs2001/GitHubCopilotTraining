using Dapper;
using MySql.Data.MySqlClient;
using SmartAPI.Models;

namespace SmartAPI.Respositories
{
    public interface ICustomerInfoRepository
    {
        Task<IEnumerable<CustomerInfo>> GetCustomerInfosAsync(string companyName, string responsiblePerson, int pageSize, int pageNumber, CancellationToken cancellationToken);

        Task<CustomerInfo?> GetCustomerInfoAsync(int id, CancellationToken cancellationToken);

        Task<bool> CreateCustomerInfoAsync(CustomerInfo customerInfo, CancellationToken cancellationToken);

        Task<bool> UpdateCustomerInfoAsync(CustomerInfo customerInfo, CancellationToken cancellationToken);

        Task<bool> DeleteCustomerInfoAsync(int id, CancellationToken cancellationToken);
        Task<int> GetCustomerInfoCountAsync(string companyName, string responsiblePerson, CancellationToken cancellationToken);

        Task<bool> ActiveUserAsync(int id, CancellationToken cancellationToken);
        Task<int> GetCountAsync(string companyName, CancellationToken cancellationToken);
    }
}
