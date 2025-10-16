using just_agi_api.Models;

namespace just_agi_api.IRepositories
{
    public interface ISmartFillRepository
    {
        Task<SmartFillUser> GetSmartFillUserAsync(string url);
        Task<SmartFillUser> AddSmartFillUserAsync(SmartFillUser smartFillUser);
        Task<SmartFillUser> UpdateSmartFillUserAsync(SmartFillUser smartFillUser);
        Task<bool> InvalidateSmartFillUserAsync(long id);
        Task<SmartFillUser> GetSmartFillUserByIdAsync(long id);
    }
}
