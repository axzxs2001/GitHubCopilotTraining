using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.IServices
{
    /// <summary>
    /// 客户服务接口
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// 异步添加客户
        /// </summary>
        Task<Customer> AddCustomerAsync(Customer customer);

        /// <summary>
        /// 异步更新客户
        /// </summary>
        Task<Customer> UpdateCustomerAsync(Customer customer);

        /// <summary>
        /// 异步使客户无效
        /// </summary>
        Task<bool> InvalidateCustomerAsync(long id);

        /// <summary>
        /// 异步通过ID获取客户
        /// </summary>
        Task<Customer> GetCustomerByIdAsync(long id);

        /// <summary>
        /// 异步获取客户列表
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomerListAsync();
    }
}
