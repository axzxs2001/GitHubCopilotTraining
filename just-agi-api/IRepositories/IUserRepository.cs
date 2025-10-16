using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.IRepositories
{
    /// <summary>
    /// 用户仓库接口
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 异步添加用户
        /// </summary>
        Task<User> AddUserAsync(User user);

        /// <summary>
        /// 异步更新用户
        /// </summary>
        Task<User> UpdateUserAsync(User user);

        /// <summary>
        /// 异步使用户无效
        /// </summary>
        Task<bool> InvalidateUserAsync(long id);

        /// <summary>
        /// 异步通过ID获取用户
        /// </summary>
        Task<User> GetUserByIdAsync(long id);

        /// <summary>
        /// 异步获取用户列表
        /// </summary>
        Task<IEnumerable<User>> GetUserListAsync();
    }
}
