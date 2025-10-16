using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.IServices
{
    public interface IPromptItemService
    {
        /// <summary>
        /// �����ʾ����
        /// </summary>
        /// <param name="promptItem">Ҫ��ӵ���ʾ����</param>
        /// <returns>��Ӻ����ʾ����</returns>
        Task<PromptItem> AddPromptItemAsync(PromptItem promptItem);

        /// <summary>
        /// ������ʾ����
        /// </summary>
        /// <param name="promptItem">Ҫ���µ���ʾ����</param>
        /// <returns>���º����ʾ����</returns>
        Task<PromptItem> UpdatePromptItemAsync(PromptItem promptItem);

        /// <summary>
        /// ʹ��ʾ����ʧЧ
        /// </summary>
        /// <param name="id">ҪʹʧЧ����ʾ�����ID</param>
        /// <returns>�����Ƿ�ɹ�</returns>
        Task<bool> InvalidatePromptItemAsync(long id);

        /// <summary>
        /// ����ID��ȡ��ʾ����
        /// </summary>
        /// <param name="id">Ҫ��ȡ����ʾ�����ID</param>
        /// <returns>��ȡ������ʾ����</returns>
        Task<PromptItem> GetPromptItemByIdAsync(long id);

        /// <summary>
        /// ��ȡ��ʾ�����б�
        /// </summary>
        /// <param name="id">Ҫ��ȡ����ʾ�����PromptID</param>
        /// <returns>��ʾ�����б�</returns>
        Task<IEnumerable<PromptItem>> GetPromptItemListAsync(long id);
    }
}
