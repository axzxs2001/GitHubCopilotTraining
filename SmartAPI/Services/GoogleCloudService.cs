using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Cloud.Functions.V2;
using Google.Cloud.Iam.V1;
using Google.Cloud.Run.V2;
using Google.Cloud.Storage.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using Grpc.Auth;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Function = Google.Cloud.Functions.V2.Function;

namespace SmartAPI.Services
{
    public class GoogleCloudService : IGoogleCloudService
    {
        private readonly string _projectId;
        private readonly string _location;
        private readonly string _bucket;
        private readonly string _directory;
        private readonly string _connector;
        private readonly ILogger<GoogleCloudService> _logger;
        public GoogleCloudService(IConfiguration configuration, ILogger<GoogleCloudService> logger)
        {
            _projectId = configuration["CloudSetting:FunctionSetting:ProjectId"];
            _location = configuration["CloudSetting:FunctionSetting:Location"];
            _connector = configuration["CloudSetting:FunctionSetting:Connector"];
            _bucket = configuration["CloudSetting:StorageSetting:Bucket"];
            _directory = configuration["CloudSetting:StorageSetting:Directory"];
            _logger = logger;
        }
        public async Task<bool> StoreCodeAsync(Stream stream, string zipFile)
        {
            try
            {
                //var credential = GoogleCredential.FromFile("secrets/bard-386905-1d685b130108.json");
                var storageClient = await StorageClient.CreateAsync();
                var result = await storageClient.UploadObjectAsync(_bucket, $"{_directory}/{zipFile}", null, source: stream);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"上传代码时发生错误：{ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteCodeAsync(string zipFile)
        {
            try
            {
                //var credential = GoogleCredential.FromFile("secrets/bard-386905-1d685b130108.json");
                var storageClient = await StorageClient.CreateAsync();
                await storageClient.DeleteObjectAsync(_bucket, $"{_directory}/{zipFile}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"删除代码时发生错误：{ex.Message}");
                return false;
            }
        }
        public async Task<bool> SetIamPolicyAsync(string functionName)
        {
            try
            {
                var servicesClient = ServicesClient.Create();
                var serviceFullName = $"projects/{_projectId}/locations/{_location}/services/{functionName.ToLower()}";
                var request = new GetIamPolicyRequest();
                request.Resource = serviceFullName;
                var policy = await servicesClient.GetIamPolicyAsync(request);
                var binding = new Binding
                {
                    Role = "roles/run.invoker",
                    Members = { "allUsers" }
                };
                policy.Bindings.Add(binding);
                var setRequest = new SetIamPolicyRequest();
                setRequest.Resource = serviceFullName;
                setRequest.Policy = policy;
                await servicesClient.SetIamPolicyAsync(setRequest);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"设置IAM策略时发生错误：{ex.Message}");
                return false;
            }
        }
        public async Task<(bool, string)> CreateFunctionAsync(string runTime, string entryPoint, string functionName)
        {
            try
            {
                var requestUrl = $"https://{_location}-{_projectId}.cloudfunctions.net/{functionName}";
                var functionServiceClient = new FunctionServiceClientBuilder().Build();

                var parent = $"projects/{_projectId}/locations/{_location}";
                var fullFunctionName = $"projects/{_projectId}/locations/{_location}/functions/{functionName}";
                var request = new CreateFunctionRequest()
                {
                    Parent = parent,
                    Function = new Function
                    {
                        Name = fullFunctionName,
                        BuildConfig = new Google.Cloud.Functions.V2.BuildConfig
                        {
                            Runtime = runTime,
                            EntryPoint = entryPoint,
                            Source = new Source
                            {
                                StorageSource = new Google.Cloud.Functions.V2.StorageSource
                                {
                                    Bucket = _bucket,
                                    Object = $"{_directory}/{functionName}.zip",
                                    Generation = 1
                                }
                            },

                        },
                        ServiceConfig = new ServiceConfig
                        {
                            IngressSettings = ServiceConfig.Types.IngressSettings.AllowAll,
                            Uri = requestUrl,
                            VpcConnector = $"projects/{_projectId}/locations/{_location}/connectors/{_connector}",
                            VpcConnectorEgressSettings = ServiceConfig.Types.VpcConnectorEgressSettings.AllTraffic

                        }
                    }
                };
                //var response = await functionServiceClient.CreateFunctionAsync(request);
                var response = await functionServiceClient.CreateFunctionAsync(parent, request.Function, functionName);

                response = await response.PollUntilCompletedAsync();
                if (response.IsCompleted)
                {
                    return (true, requestUrl);
                }
                else
                {
                    _logger.LogInformation("函数创建失败！");
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"函数创建成功发生错误：{ex.Message}");
                return (false, null);
            }
        }
        public async Task<bool> DeleteFunctionAsync(string functionName)
        {
            try
            {
                var functionServiceClient = await new FunctionServiceClientBuilder().BuildAsync();
                var fullFunctionName = $"projects/{_projectId}/locations/{_location}/functions/{functionName}";
                var request = new DeleteFunctionRequest
                {
                    Name = fullFunctionName
                };

                var response = await functionServiceClient.DeleteFunctionAsync(request);

                response = response.PollUntilCompleted();

                if (response.IsCompleted)
                {
                    _logger.LogInformation("函数删除成功！");
                    return true;
                }
                else
                {
                    _logger.LogInformation("函数删除失败！");
                    return false;

                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"删除函数时发生错误：{ex.Message}");
                return false;
            }
        }
    }
}
