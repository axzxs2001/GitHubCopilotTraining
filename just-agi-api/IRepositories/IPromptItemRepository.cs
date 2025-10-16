using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.IRepositories
{
    public interface IPromptItemRepository
    {
        /// <summary>
        /// 添加提示词项
        /// </summary>
        /// <param name="promptItem">提示词项实体</param>
        /// <returns>添加后的提示词项实体</returns>
        Task<PromptItem> AddPromptItemAsync(PromptItem promptItem);

        /// <summary>
        /// 修改提示词项
        /// </summary>
        /// <param name="promptItem">提示词项实体</param>
        /// <returns>修改后的提示词项实体</returns>
        Task<PromptItem> UpdatePromptItemAsync(PromptItem promptItem);

        /// <summary>
        /// 失效提示词项
        /// </summary>
        /// <param name="id">提示词项编号</param>
        /// <returns>是否成功失效</returns>
        Task<bool> InvalidatePromptItemAsync(long id);

        /// <summary>
        /// 按ID查询提示词项
        /// </summary>
        /// <param name="id">提示词项编号</param>
        /// <returns>查询到的提示词项实体</returns>
        Task<PromptItem> GetPromptItemByIdAsync(long id);

        /// <summary>
        /// 查询提示词项列表
        /// </summary>
        /// <param name="id">提示词编号</param>
        /// <returns>提示词项列表</returns>
        Task<IEnumerable<PromptItem>> GetPromptItemListAsync(long id);
    }
}
