using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.IServices
{
    public interface IPromptItemService
    {
        /// <summary>
        /// 添加提示词项
        /// </summary>
        /// <param name="promptItem">要添加的提示词项</param>
        /// <returns>添加后的提示词项</returns>
        Task<PromptItem> AddPromptItemAsync(PromptItem promptItem);

        /// <summary>
        /// 更新提示词项
        /// </summary>
        /// <param name="promptItem">要更新的提示词项</param>
        /// <returns>更新后的提示词项</returns>
        Task<PromptItem> UpdatePromptItemAsync(PromptItem promptItem);

        /// <summary>
        /// 使提示词项失效
        /// </summary>
        /// <param name="id">要使失效的提示词项的ID</param>
        /// <returns>操作是否成功</returns>
        Task<bool> InvalidatePromptItemAsync(long id);

        /// <summary>
        /// 根据ID获取提示词项
        /// </summary>
        /// <param name="id">要获取的提示词项的ID</param>
        /// <returns>获取到的提示词项</returns>
        Task<PromptItem> GetPromptItemByIdAsync(long id);

        /// <summary>
        /// 获取提示词项列表
        /// </summary>
        /// <param name="id">要获取的提示词项的PromptID</param>
        /// <returns>提示词项列表</returns>
        Task<IEnumerable<PromptItem>> GetPromptItemListAsync(long id);
    }
}
