using SmartAPI.Models;

namespace SmartAPI.Services
{
    public interface ICustomerInfoService
    {
        Task<IEnumerable<CustomerInfo>> GetCustomerInfosAsync(string companyName, string responsiblePerson, int pageSize, int pageNumber, CancellationToken cancellationToken);
        Task<CustomerInfo?> GetCustomerInfoAsync(int id, CancellationToken cancellationToken);
        Task<bool> AddCustomerInfoAsync(CustomerInfo customerInfo, CancellationToken cancellationToken);
        Task<bool> ModifyCustomerInfoAsync(CustomerInfo customerInfo, CancellationToken cancellationToken);
        Task<bool> RemoveCustomerInfoAsync(int id, CancellationToken cancellationToken);
        Task<int> GetCustomerInfoCountAsync(string companyName, string responsiblePerson, CancellationToken cancellationToken);
        Task<bool> ActiveUserAsync(int id, CancellationToken cancellationToken);
        Task<int> GetCountAsync(string companyName, CancellationToken cancellationToken);
    }
}
