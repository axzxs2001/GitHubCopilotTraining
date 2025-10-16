using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.IServices
{
    /// <summary>
    /// �ͻ�����ӿ�
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// �첽��ӿͻ�
        /// </summary>
        Task<Customer> AddCustomerAsync(Customer customer);

        /// <summary>
        /// �첽���¿ͻ�
        /// </summary>
        Task<Customer> UpdateCustomerAsync(Customer customer);

        /// <summary>
        /// �첽ʹ�ͻ���Ч
        /// </summary>
        Task<bool> InvalidateCustomerAsync(long id);

        /// <summary>
        /// �첽ͨ��ID��ȡ�ͻ�
        /// </summary>
        Task<Customer> GetCustomerByIdAsync(long id);

        /// <summary>
        /// �첽��ȡ�ͻ��б�
        /// </summary>
        Task<IEnumerable<Customer>> GetCustomerListAsync();
    }
}
