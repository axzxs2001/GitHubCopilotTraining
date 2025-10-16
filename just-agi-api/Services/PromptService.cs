
/*CopilotPrompt
  #PromptRepository.cs 根据文件中的类生成PromptService，并给类和方法添加中文注释，并调用PromptRepository
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
        /// 添加提示
        /// </summary>
        /// <param name="prompt">要添加的提示</param>
        /// <returns>添加后的提示</returns>
        public async Task<Prompt> AddPromptAsync(Prompt prompt)
        {
            return await _promptRepository.AddPromptAsync(prompt);
        }

        /// <summary>
        /// 更新提示
        /// </summary>
        /// <param name="prompt">要更新的提示</param>
        /// <returns>更新后的提示</returns>
        public async Task<Prompt> UpdatePromptAsync(Prompt prompt)
        {
            return await _promptRepository.UpdatePromptAsync(prompt);
        }

        /// <summary>
        /// 使提示无效
        /// </summary>
        /// <param name="id">要使无效的提示的ID</param>
        /// <returns>操作是否成功</returns>
        public async Task<bool> InvalidatePromptAsync(long id)
        {
            return await _promptRepository.InvalidatePromptAsync(id);
        }

        /// <summary>
        /// 根据ID获取提示
        /// </summary>
        /// <param name="id">要获取的提示的ID</param>
        /// <returns>获取到的提示</returns>
        public async Task<Prompt> GetPromptByIdAsync(long id)
        {
            return await _promptRepository.GetPromptByIdAsync(id);
        }

        /// <summary>
        /// 获取提示列表
        /// </summary>
        /// <returns>提示列表</returns>
        public async Task<IEnumerable<Prompt>> GetPromptListAsync()
        {
            return await _promptRepository.GetPromptListAsync();
        }
    }
}
