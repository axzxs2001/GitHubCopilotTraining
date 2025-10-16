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
        /// �����ʾ����
        /// </summary>
        /// <param name="promptItem">Ҫ��ӵ���ʾ����</param>
        /// <returns>��Ӻ����ʾ����</returns>
        public async Task<PromptItem> AddPromptItemAsync(PromptItem promptItem)
        {
            return await _promptItemRepository.AddPromptItemAsync(promptItem);
        }

        /// <summary>
        /// ������ʾ����
        /// </summary>
        /// <param name="promptItem">Ҫ���µ���ʾ����</param>
        /// <returns>���º����ʾ����</returns>
        public async Task<PromptItem> UpdatePromptItemAsync(PromptItem promptItem)
        {
            return await _promptItemRepository.UpdatePromptItemAsync(promptItem);
        }

        /// <summary>
        /// ʹ��ʾ����ʧЧ
        /// </summary>
        /// <param name="id">ҪʹʧЧ����ʾ�����ID</param>
        /// <returns>�����Ƿ�ɹ�</returns>
        public async Task<bool> InvalidatePromptItemAsync(long id)
        {
            return await _promptItemRepository.InvalidatePromptItemAsync(id);
        }

        /// <summary>
        /// ����ID��ȡ��ʾ����
        /// </summary>
        /// <param name="id">Ҫ��ȡ����ʾ�����ID</param>
        /// <returns>��ȡ������ʾ����</returns>
        public async Task<PromptItem> GetPromptItemByIdAsync(long id)
        {
            return await _promptItemRepository.GetPromptItemByIdAsync(id);
        }

        /// <summary>
        /// ��ȡ��ʾ�����б�
        /// </summary>
        /// <param name="id">Ҫ��ȡ����ʾ�����PromptID</param>
        /// <returns>��ʾ�����б�</returns>
        public async Task<IEnumerable<PromptItem>> GetPromptItemListAsync(long id)
        {
            return await _promptItemRepository.GetPromptItemListAsync(id);
        }
    }
}
