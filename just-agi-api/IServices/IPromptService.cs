using just_agi_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace just_agi_api.IServices
{
    public interface IPromptService
    {
        /// <summary>
        /// �����ʾ
        /// </summary>
        /// <param name="prompt">Ҫ��ӵ���ʾ</param>
        /// <returns>��Ӻ����ʾ</returns>
        Task<Prompt> AddPromptAsync(Prompt prompt);
        /// <summary>
        /// ������ʾ
        /// </summary>
        /// <param name="prompt">Ҫ���µ���ʾ</param>
        /// <returns>���º����ʾ</returns>
        Task<Prompt> UpdatePromptAsync(Prompt prompt);
        /// <summary>
        /// ʹ��ʾ��Ч
        /// </summary>
        /// <param name="id">Ҫʹ��Ч����ʾ��ID</param>
        /// <returns>�����Ƿ�ɹ�</returns>
        Task<bool> InvalidatePromptAsync(long id);

        /// <summary>
        /// ����ID��ȡ��ʾ
        /// </summary>
        /// <param name="id">Ҫ��ȡ����ʾ��ID</param>
        /// <returns>��ȡ������ʾ</returns>
        Task<Prompt> GetPromptByIdAsync(long id);

        /// <summary>
        /// ��ȡ��ʾ�б�
        /// </summary>
        /// <returns>��ʾ�б�</returns>
        Task<IEnumerable<Prompt>> GetPromptListAsync();
    }
}
