using SmartAPI.Models;

namespace SmartAPI.Services
{
    public interface ICodeSettingService
    {
        Task<IEnumerable<CodeSetting>> GetCodeSettingsAsync(CancellationToken cancellationToken);
        Task<CodeSetting?> GetCodeSettingAsync(int id, CancellationToken cancellationToken);
        Task<int> AddCodeSettingAsync(CodeSetting codeSetting, CancellationToken cancellationToken);
        Task<int> ModifyCodeSettingAsync(CodeSetting codeSetting, CancellationToken cancellationToken);
        Task<int> RemoveCodeSettingAsync(int id, CancellationToken cancellationToken);
    }
}
