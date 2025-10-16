using just_agi_api.Models;


namespace just_agi_api.IServices
{
    public interface ISmartFillService
    {
        Task<string> StarRecordAsync(AudioEntity audio);
        Task<string?> StopRecordAsync(AudioEntity audio);
        Task<string?> ContentToJsonAsync(AnswerEntity answer);
        Task<string?> GetJsonByUrl(string url);
        Task<SmartFillUser> AddSmartFillUserAsync(SmartFillUser smartFillUser);
    }
}
