using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.IRepositories
{
    public interface IPromptRepository
    {
        /// <summary>
        /// 添加提示词
        /// </summary>
        /// <param name="prompt">提示词实体</param>
        /// <returns>添加后的提示词实体</returns>
        Task<Prompt> AddPromptAsync(Prompt prompt);

        /// <summary>
        /// 修改提示词
        /// </summary>
        /// <param name="prompt">提示词实体</param>
        /// <returns>修改后的提示词实体</returns>
        Task<Prompt> UpdatePromptAsync(Prompt prompt);

        /// <summary>
        /// 失效提示词
        /// </summary>
        /// <param name="id">提示词编号</param>
        /// <returns>是否成功失效</returns>
        Task<bool> InvalidatePromptAsync(long id);

        /// <summary>
        /// 按ID查询提示词
        /// </summary>
        /// <param name="id">提示词编号</param>
        /// <returns>查询到的提示词实体</returns>
        Task<Prompt> GetPromptByIdAsync(long id);

        /// <summary>
        /// 查询提示词列表
        /// </summary>
        /// <returns>提示词列表</returns>
        Task<IEnumerable<Prompt>> GetPromptListAsync();
    }
}
