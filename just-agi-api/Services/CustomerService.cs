using just_agi_api.IRepositories;
using just_agi_api.IServices;
using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.Services
{
    /// <summary>
    /// 客户服务
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            return await _customerRepository.AddCustomerAsync(customer);
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            return await _customerRepository.UpdateCustomerAsync(customer);
        }

        public async Task<bool> InvalidateCustomerAsync(long id)
        {
            return await _customerRepository.InvalidateCustomerAsync(id);
        }

        public async Task<Customer> GetCustomerByIdAsync(long id)
        {
            return await _customerRepository.GetCustomerByIdAsync(id);
        }

        public async Task<IEnumerable<Customer>> GetCustomerListAsync()
        {
            return await _customerRepository.GetCustomerListAsync();
        }
    }
}
