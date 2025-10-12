using SmartAPI.Models;
using SmartAPI.Respositories;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAPI.Services
{
    public class CodeSettingService : ICodeSettingService
    {
        readonly ICodeSettingRepository _codeSettingRepository;
        readonly ILogger<CodeSettingService> _logger;

        public CodeSettingService(ILogger<CodeSettingService> logger, ICodeSettingRepository codeSettingRepository)
        {
            _codeSettingRepository = codeSettingRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CodeSetting>> GetCodeSettingsAsync(CancellationToken cancellationToken)
        {
            return await _codeSettingRepository.GetCodeSettingsAsync(cancellationToken);
        }

        public async Task<CodeSetting?> GetCodeSettingAsync(int id, CancellationToken cancellationToken)
        {
            return await _codeSettingRepository.GetCodeSettingAsync(id, cancellationToken);
        }

        public async Task<int> AddCodeSettingAsync(CodeSetting codeSetting, CancellationToken cancellationToken)
        {
            codeSetting.Validate = true;
            codeSetting.CreateTime = DateTime.Now;
            codeSetting.ModifyTime = DateTime.Now;
            codeSetting.CreateUser = "sys";
            codeSetting.ModifyUser = "sys";
            return await _codeSettingRepository.CreateCodeSettingAsync(codeSetting, cancellationToken);
        }

        public async Task<int> ModifyCodeSettingAsync(CodeSetting codeSetting, CancellationToken cancellationToken)
        {
            codeSetting.ModifyTime = DateTime.Now;        
            codeSetting.ModifyUser = "sys";
            return await _codeSettingRepository.UpdateCodeSettingAsync(codeSetting, cancellationToken);
        }

        public async Task<int> RemoveCodeSettingAsync(int id, CancellationToken cancellationToken)
        {
            return await _codeSettingRepository.DeleteCodeSettingAsync(id, cancellationToken);
        }
    }
}
