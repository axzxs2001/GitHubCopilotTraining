using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.IServices
{
    public interface IPromptService
    {
        /// <summary>
        /// 添加提示
        /// </summary>
        /// <param name="prompt">要添加的提示</param>
        /// <returns>添加后的提示</returns>
        Task<Prompt> AddPromptAsync(Prompt prompt);
        /// <summary>
        /// 更新提示
        /// </summary>
        /// <param name="prompt">要更新的提示</param>
        /// <returns>更新后的提示</returns>
        Task<Prompt> UpdatePromptAsync(Prompt prompt);
        /// <summary>
        /// 使提示无效
        /// </summary>
        /// <param name="id">要使无效的提示的ID</param>
        /// <returns>操作是否成功</returns>
        Task<bool> InvalidatePromptAsync(long id);

        /// <summary>
        /// 根据ID获取提示
        /// </summary>
        /// <param name="id">要获取的提示的ID</param>
        /// <returns>获取到的提示</returns>
        Task<Prompt> GetPromptByIdAsync(long id);

        /// <summary>
        /// 获取提示列表
        /// </summary>
        /// <returns>提示列表</returns>
        Task<IEnumerable<Prompt>> GetPromptListAsync();
    }
}
