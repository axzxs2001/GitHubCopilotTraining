
/*CopilotPrompt
  #PromptRepository.cs �����ļ��е�������PromptService��������ͷ����������ע�ͣ�������PromptRepository
 */
using just_agi_api.IRepositories;
using just_agi_api.IServices;
using just_agi_api.Models;


namespace just_agi_api.Services
{

    public class PromptService : IPromptService
    {
        private readonly IPromptRepository _promptRepository;

        public PromptService(IPromptRepository promptRepository)
        {
            _promptRepository = promptRepository;
        }

        /// <summary>
        /// �����ʾ
        /// </summary>
        /// <param name="prompt">Ҫ��ӵ���ʾ</param>
        /// <returns>��Ӻ����ʾ</returns>
        public async Task<Prompt> AddPromptAsync(Prompt prompt)
        {
            return await _promptRepository.AddPromptAsync(prompt);
        }

        /// <summary>
        /// ������ʾ
        /// </summary>
        /// <param name="prompt">Ҫ���µ���ʾ</param>
        /// <returns>���º����ʾ</returns>
        public async Task<Prompt> UpdatePromptAsync(Prompt prompt)
        {
            return await _promptRepository.UpdatePromptAsync(prompt);
        }

        /// <summary>
        /// ʹ��ʾ��Ч
        /// </summary>
        /// <param name="id">Ҫʹ��Ч����ʾ��ID</param>
        /// <returns>�����Ƿ�ɹ�</returns>
        public async Task<bool> InvalidatePromptAsync(long id)
        {
            return await _promptRepository.InvalidatePromptAsync(id);
        }

        /// <summary>
        /// ����ID��ȡ��ʾ
        /// </summary>
        /// <param name="id">Ҫ��ȡ����ʾ��ID</param>
        /// <returns>��ȡ������ʾ</returns>
        public async Task<Prompt> GetPromptByIdAsync(long id)
        {
            return await _promptRepository.GetPromptByIdAsync(id);
        }

        /// <summary>
        /// ��ȡ��ʾ�б�
        /// </summary>
        /// <returns>��ʾ�б�</returns>
        public async Task<IEnumerable<Prompt>> GetPromptListAsync()
        {
            return await _promptRepository.GetPromptListAsync();
        }
    }
}
