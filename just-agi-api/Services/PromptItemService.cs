using just_agi_api.IRepositories;
using just_agi_api.IServices;
using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.Services
{
    public class PromptItemService : IPromptItemService
    {
        private readonly IPromptItemRepository _promptItemRepository;

        public PromptItemService(IPromptItemRepository promptItemRepository)
        {
            _promptItemRepository = promptItemRepository;
        }

        /// <summary>
        /// 添加提示词项
        /// </summary>
        /// <param name="promptItem">要添加的提示词项</param>
        /// <returns>添加后的提示词项</returns>
        public async Task<PromptItem> AddPromptItemAsync(PromptItem promptItem)
        {
            return await _promptItemRepository.AddPromptItemAsync(promptItem);
        }

        /// <summary>
        /// 更新提示词项
        /// </summary>
        /// <param name="promptItem">要更新的提示词项</param>
        /// <returns>更新后的提示词项</returns>
        public async Task<PromptItem> UpdatePromptItemAsync(PromptItem promptItem)
        {
            return await _promptItemRepository.UpdatePromptItemAsync(promptItem);
        }

        /// <summary>
        /// 使提示词项失效
        /// </summary>
        /// <param name="id">要使失效的提示词项的ID</param>
        /// <returns>操作是否成功</returns>
        public async Task<bool> InvalidatePromptItemAsync(long id)
        {
            return await _promptItemRepository.InvalidatePromptItemAsync(id);
        }

        /// <summary>
        /// 根据ID获取提示词项
        /// </summary>
        /// <param name="id">要获取的提示词项的ID</param>
        /// <returns>获取到的提示词项</returns>
        public async Task<PromptItem> GetPromptItemByIdAsync(long id)
        {
            return await _promptItemRepository.GetPromptItemByIdAsync(id);
        }

        /// <summary>
        /// 获取提示词项列表
        /// </summary>
        /// <param name="id">要获取的提示词项的PromptID</param>
        /// <returns>提示词项列表</returns>
        public async Task<IEnumerable<PromptItem>> GetPromptItemListAsync(long id)
        {
            return await _promptItemRepository.GetPromptItemListAsync(id);
        }
    }
}
