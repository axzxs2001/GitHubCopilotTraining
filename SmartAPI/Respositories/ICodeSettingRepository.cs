using SmartAPI.Models;

namespace SmartAPI.Respositories
{
    public interface ICodeSettingRepository
    {
        Task<IEnumerable<CodeSetting>> GetCodeSettingsAsync(CancellationToken cancellationToken);
        Task<CodeSetting?> GetCodeSettingAsync(int id, CancellationToken cancellationToken);
        Task<int> CreateCodeSettingAsync(CodeSetting codeSetting, CancellationToken cancellationToken);
        Task<int> UpdateCodeSettingAsync(CodeSetting codeSetting, CancellationToken cancellationToken);
        Task<int> DeleteCodeSettingAsync(int id, CancellationToken cancellationToken);
    }
}
