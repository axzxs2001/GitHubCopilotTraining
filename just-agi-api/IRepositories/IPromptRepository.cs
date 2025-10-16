using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.IRepositories
{
    public interface IPromptRepository
    {
        /// <summary>
        /// �����ʾ��
        /// </summary>
        /// <param name="prompt">��ʾ��ʵ��</param>
        /// <returns>��Ӻ����ʾ��ʵ��</returns>
        Task<Prompt> AddPromptAsync(Prompt prompt);

        /// <summary>
        /// �޸���ʾ��
        /// </summary>
        /// <param name="prompt">��ʾ��ʵ��</param>
        /// <returns>�޸ĺ����ʾ��ʵ��</returns>
        Task<Prompt> UpdatePromptAsync(Prompt prompt);

        /// <summary>
        /// ʧЧ��ʾ��
        /// </summary>
        /// <param name="id">��ʾ�ʱ��</param>
        /// <returns>�Ƿ�ɹ�ʧЧ</returns>
        Task<bool> InvalidatePromptAsync(long id);

        /// <summary>
        /// ��ID��ѯ��ʾ��
        /// </summary>
        /// <param name="id">��ʾ�ʱ��</param>
        /// <returns>��ѯ������ʾ��ʵ��</returns>
        Task<Prompt> GetPromptByIdAsync(long id);

        /// <summary>
        /// ��ѯ��ʾ���б�
        /// </summary>
        /// <returns>��ʾ���б�</returns>
        Task<IEnumerable<Prompt>> GetPromptListAsync();
    }
}
