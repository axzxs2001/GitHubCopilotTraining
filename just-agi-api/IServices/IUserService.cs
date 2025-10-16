using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.IServices
{
    /// <summary>
    /// �û�����ӿ�
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// �첽�����û�
        /// </summary>
        Task<User> AddUserAsync(User user);

        /// <summary>
        /// �첽�����û�
        /// </summary>
        Task<User> UpdateUserAsync(User user);

        /// <summary>
        /// �첽ʹ�û���Ч
        /// </summary>
        Task<bool> InvalidateUserAsync(long id);

        /// <summary>
        /// �첽ͨ��ID��ȡ�û�
        /// </summary>
        Task<User> GetUserByIdAsync(long id);

        /// <summary>
        /// �첽��ȡ�û��б�
        /// </summary>
        Task<IEnumerable<User>> GetUserListAsync();

        Task<IEnumerable<User>> GetUsersAsync();
    }
}
