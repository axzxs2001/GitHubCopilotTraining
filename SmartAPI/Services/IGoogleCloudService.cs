using Google.Cloud.Functions.V2;
using Google.Cloud.Iam.V1;
using Google.Cloud.Storage.V1;
using System.IO;

namespace SmartAPI.Services
{
    public interface IGoogleCloudService
    {
        Task<bool> StoreCodeAsync(Stream stream, string zipFile);
        Task<bool> DeleteCodeAsync(string zipFile);
        Task<bool> SetIamPolicyAsync(string functionName);
        Task<(bool result, string url)> CreateFunctionAsync(string runTime,string entryPoint, string functionName);
        Task<bool> DeleteFunctionAsync(string functionName);
    }
}
