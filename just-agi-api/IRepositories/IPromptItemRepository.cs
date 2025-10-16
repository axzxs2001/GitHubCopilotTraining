using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.IRepositories
{
    public interface IPromptItemRepository
    {
        /// <summary>
        /// �����ʾ����
        /// </summary>
        /// <param name="promptItem">��ʾ����ʵ��</param>
        /// <returns>��Ӻ����ʾ����ʵ��</returns>
        Task<PromptItem> AddPromptItemAsync(PromptItem promptItem);

        /// <summary>
        /// �޸���ʾ����
        /// </summary>
        /// <param name="promptItem">��ʾ����ʵ��</param>
        /// <returns>�޸ĺ����ʾ����ʵ��</returns>
        Task<PromptItem> UpdatePromptItemAsync(PromptItem promptItem);

        /// <summary>
        /// ʧЧ��ʾ����
        /// </summary>
        /// <param name="id">��ʾ������</param>
        /// <returns>�Ƿ�ɹ�ʧЧ</returns>
        Task<bool> InvalidatePromptItemAsync(long id);

        /// <summary>
        /// ��ID��ѯ��ʾ����
        /// </summary>
        /// <param name="id">��ʾ������</param>
        /// <returns>��ѯ������ʾ����ʵ��</returns>
        Task<PromptItem> GetPromptItemByIdAsync(long id);

        /// <summary>
        /// ��ѯ��ʾ�����б�
        /// </summary>
        /// <param name="id">��ʾ�ʱ��</param>
        /// <returns>��ʾ�����б�</returns>
        Task<IEnumerable<PromptItem>> GetPromptItemListAsync(long id);
    }
}
