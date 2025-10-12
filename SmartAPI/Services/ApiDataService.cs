using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using MySqlX.XDevAPI.Common;
using SmartAPI.Models;
using SmartAPI.Respositories;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SmartAPI.Services
{
    public class ApiDataService : IApiDataService
    {
        readonly IApiDataRepository _apiDataRepository;
        readonly IGoogleCloudService _googleCloudService;
        readonly IAIService _aiService;
        readonly ICodeSettingService _codeSettingService;
        readonly IHttpClientFactory _httpClientFactory;
        readonly IStringLocalizer<SharedResource> _localizer;
        readonly ILogger<ApiDataService> _logger;
        readonly IDistributedCache _distributedCache;
        public ApiDataService(ILogger<ApiDataService> logger, IApiDataRepository apiDataRepository, IGoogleCloudService googleCloudService, IAIService aiService, ICodeSettingService codeSettingService, IHttpClientFactory httpClientFactory, IDistributedCache distributedCache, IStringLocalizer<SharedResource> localizer)
        {
            _googleCloudService = googleCloudService;
            _apiDataRepository = apiDataRepository;
            _httpClientFactory = httpClientFactory;
            _aiService = aiService;
            _codeSettingService = codeSettingService;
            _logger = logger;
            _localizer = localizer;
            _distributedCache = distributedCache;
        }
        public async Task<IEnumerable<ApiData>> GetApiDatasAsync(int productID, CancellationToken cancellationToken)
        {
            return await _apiDataRepository.GetApiDatasAsync(productID, cancellationToken);
        }
        public async Task<IEnumerable<ApiData>> GetApiDatasByCategoryAsync(int productID, CancellationToken cancellationToken)
        {
            var noAPICategories = typeof(CategoryType)
              .GetFields(BindingFlags.Public | BindingFlags.Static)
              .Where(field => field.GetCustomAttribute<IsAPIAttribute>() == null)
              .Select(field => field.GetValue(null)) // 获取字段值
              .Cast<string>() // 转换为字符串类型
              .ToList();

            return await _apiDataRepository.GetApiDatasByCategoryAsync(productID, noAPICategories.ToArray(), cancellationToken);
        }
        public async Task<ApiData> GetApiDataAsync(int id, CancellationToken cancellationToken)
        {
            return await _apiDataRepository.GetApiDataAsync(id, cancellationToken);
        }
        public async Task<ApiData> AddApiDataAsync(ApiData apiData, CancellationToken cancellationToke)
        {
            apiData.CreateTime = DateTime.Now;
            apiData.CreateUser = "sys";
            apiData.ModifyUser = "sys";
            apiData.ModifyTime = DateTime.Now;
            apiData.Validate = true;
            return await _apiDataRepository.CreateApiDataAsync(apiData, cancellationToke);
        }
        public async Task<bool> ModifyApiDataAsync(ApiData apiData, CancellationToken cancellationToke)
        {
            apiData.ModifyTime = DateTime.Now;
            var result = await _apiDataRepository.UpdateApiDataAsync(apiData, cancellationToke);
            return result > 0;
        }
        public async Task<bool> RemoveApiDataAsync(int id, CancellationToken cancellationToke)
        {
            var result = await _apiDataRepository.DeleteApiDataAsync(id, cancellationToke);
            return result > 0;
        }
        public async Task<bool> CopyAPIsAsync(int oldProductID, int newProductID, CancellationToken cancellationToken)
        {
            try
            {
                await CopyChildApiDataAsync(oldProductID, newProductID, 0, 0, cancellationToken);
                return true;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, exc.Message);
                return false;
            }
        }
        async Task CopyChildApiDataAsync(int oldProductID, int newProductID, int oldParentID, int newParentID, CancellationToken cancellationToken)
        {
            var parentAPIDatas = await _apiDataRepository.GetApiDatasAsync(oldProductID, oldParentID, cancellationToken);
            foreach (var apiData in parentAPIDatas)
            {
                var oldID = apiData.Id;
                apiData.Id = 0;
                apiData.ProductId = newProductID;
                apiData.ParentID = newParentID;
                var newAPIData = await _apiDataRepository.CreateApiDataAsync(apiData, cancellationToken);
                await CopyChildApiDataAsync(oldProductID, newProductID, oldID, newAPIData.Id, cancellationToken);
            }
        }

        #region runcode
        public async IAsyncEnumerable<RunResult> RunCodeAsync(RunParmeter runParmeter, CancellationToken cancellationToken)
        {
            var codeSetting = await _codeSettingService.GetCodeSettingAsync(runParmeter.CodeSettingID, cancellationToken);
            yield return new RunResult { Result = true, Message = _localizer["run_00"].Value };
            await foreach (var item in RunDotNet8(runParmeter, codeSetting))
            {
                yield return item;
            }
            yield return new RunResult { Result = true, Message = _localizer["run_99"].Value };
        }
        async IAsyncEnumerable<RunResult> RunDotNet8(RunParmeter runParmeter, CodeSetting codeSetting)
        {
            var functionName = "SmartAPIFunction" + DateTime.Now.ToString("MMddHHmmssfff");
            var zipStream = CreateZipStream((codeSetting.CodeFilename, runParmeter.SourceCode), (codeSetting.AdditionalFilename, codeSetting.AdditionalTemplates));

            //第一步：创建一个云存储，并上传源代码
            var clearFlag = false;
            var createFunction = false;
            yield return new RunResult { Result = true, Message = _localizer["run_01"].Value };
            var functionCodeFile = $"{functionName}.zip";
            var result = await _googleCloudService.StoreCodeAsync(zipStream, functionCodeFile);
            if (!result)
            {
                _logger.LogError(_localizer["run_err_01"].Value);
                yield return new RunResult { Result = false, Message = _localizer["run_err_01"].Value };
            }
            else
            {
                clearFlag = true;
                _logger.LogInformation(_localizer["run_02"].Value);
                yield return new RunResult { Result = true, Message = _localizer["run_02"].Value };
                //第二步：创建一个云函数
                yield return new RunResult { Result = true, Message = _localizer["run_03"].Value };

                var functionResult = await _googleCloudService.CreateFunctionAsync(runParmeter.RunTime, codeSetting.EntryPoint, functionName);
                if (!functionResult.result)
                {
                    clearFlag = false;
                    _logger.LogError(_localizer["run_err_02"].Value, functionName);
                    yield return new RunResult { Result = false, Message = string.Format(_localizer["run_err_02"].Value, functionName) };
                }
                else
                {
                    createFunction = true;
                    _logger.LogInformation(_localizer["run_04"].Value, functionName);
                    yield return new RunResult { Result = true, Message = string.Format(_localizer["run_04"].Value, functionName) };

                    //第三步：设置云函数的权限
                    await Task.Delay(15000);
                    yield return new RunResult { Result = true, Message = _localizer["run_05"].Value };
                    var setPolicyResult = await _googleCloudService.SetIamPolicyAsync(functionName);
                    if (!setPolicyResult)
                    {
                        clearFlag = false;
                        _logger.LogError(_localizer["run_err_03"].Value, functionName);
                        yield return new RunResult { Result = false, Message = string.Format(_localizer["run_err_03"].Value, functionName) };
                    }
                    else
                    {
                        _logger.LogInformation(_localizer["run_06"].Value, functionName);
                        yield return new RunResult { Result = true, Message = string.Format(_localizer["run_06"].Value, functionName) };
                        //第四步：请求执行云函数
                        yield return new RunResult { Result = true, Message = _localizer["run_07"].Value };
                        var content = await RequestFunctionAsync(functionResult.url);
                        if (string.IsNullOrEmpty(content))
                        {
                            clearFlag = false;
                            _logger.LogError(_localizer["run_err_04"].Value, functionResult.url);
                            yield return new RunResult { Result = false, Message = string.Format(_localizer["run_err_04"].Value, functionResult.url) };
                        }
                        else
                        {
                            _logger.LogInformation(_localizer["run_08"].Value, functionResult.url);
                            yield return new RunResult { Result = true, Message = string.Format(_localizer["run_08"].Value, functionResult.url) };
                            yield return new RunResult { Result = true, Message = _localizer["run_09"].Value };
                            yield return new RunResult { Result = true, DataType = "json", Message = content };

                            await _distributedCache.SetStringAsync($"{runParmeter.APIDataID}_{runParmeter.UserEmail}", content);
                        }
                    }
                }
            }

            //第六步：删除云存储
            if (clearFlag)
            {
                Task.Run(async () =>
                {
                    var deleteResult = await _googleCloudService.DeleteCodeAsync(functionCodeFile);
                    if (!deleteResult)
                    {
                        _logger.LogError("删除代码{0}失败！", functionCodeFile);
                    }
                    else
                    {
                        _logger.LogInformation("删除代码{0}成功！", functionCodeFile);

                    }
                });
            }

            //第五步：删除云函数
            if (createFunction)
            {
                Task.Run(async () =>
                {
                    var deleteResult = await _googleCloudService.DeleteFunctionAsync(functionName);
                    if (!deleteResult)
                    {
                        _logger.LogError("删除函数{0}失败", functionName);
                    }
                    else
                    {
                        _logger.LogInformation("删除函数{0}成功！", functionName);
                    }
                });
            }
        }
        async Task<string> RequestFunctionAsync(string url)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                return await client.GetStringAsync(url);
            }
            catch (Exception exc)
            {
                _logger.LogWarning($"RequestFunctionAsync Error:{exc.Message}");
                return "";
            }
        }

        MemoryStream CreateZipStream(params (string FileName, string Content)[] files)
        {
            var memoryStream = new MemoryStream();
            using (ZipOutputStream zipStream = new ZipOutputStream(memoryStream))
            {
                zipStream.SetLevel(9); // 压缩级别
                foreach (var (fileName, content) in files)
                {
                    ZipEntry entry = new ZipEntry(fileName)
                    {
                        DateTime = DateTime.Now, // 文件时间戳
                        //Size = content.Length // 文件大小
                    };
                    zipStream.PutNextEntry(entry);
                    using (StreamWriter writer = new StreamWriter(zipStream, leaveOpen: true))
                    {
                        writer.Write(content);
                    }
                    zipStream.CloseEntry();
                }
                zipStream.IsStreamOwner = false;
                zipStream.Finish();
                zipStream.Close();
            }
            memoryStream.Position = 0;
            return memoryStream;
        }
        #endregion

        #region GenerateCode
        public async IAsyncEnumerable<string> GenerateCode(string requestID, APIQuest apiQuestion, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"culture:{_localizer["culture"].Value}");
            var codeSetting = await _codeSettingService.GetCodeSettingAsync(apiQuestion.CodeSettingID, cancellationToken);

            var api = await _apiDataRepository.GetApiDataAsync(apiQuestion.APIID, cancellationToken);
            var signInAPIContent = api.Content;
            if (api.CategoryName == CategoryType.NeedCallBack && !string.IsNullOrWhiteSpace(apiQuestion.ConnectionID))
            {
                if (!string.IsNullOrWhiteSpace(apiQuestion.ConnectionID))
                {
                    signInAPIContent = signInAPIContent.Replace("${{connectionId}}", apiQuestion.ConnectionID);
                }
                _distributedCache.SetStringAsync(apiQuestion.ConnectionID, apiQuestion.APIID.ToString(), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(5) });
            }
      

            var codeTemplate = codeSetting.CodeTemplate;

            #region 处理关联配置信息
            var otherSettings = new Dictionary<string, string>();
            if (api.LinkAPIID != null)
            {
                foreach (var linkID in api.LinkAPIID)
                {
                    var linkResult = await _apiDataRepository.GetApiDataAsync(linkID, cancellationToken);
                    otherSettings.Add(linkResult.Name, linkResult.Content);
                }
            }
            #endregion

            #region 处理其他API返回结果
            var otherAPIResults = new Dictionary<string, string>();
            if (api.ResultAPIID != null)
            {
                foreach (var resultAPIID in api.ResultAPIID)
                {
                    var resultAPI = await _apiDataRepository.GetApiDataAsync(resultAPIID, cancellationToken);
                    var cacheResponse = await _distributedCache.GetStringAsync($"{resultAPIID}_{apiQuestion.UserEmail}");
                    otherAPIResults.Add($"{resultAPI.Name}返回结果", cacheResponse);
                }
            }
            #endregion

            var systemPrompt = @$"""
    # Role: 高级软件工程师

    ## Goals: 根据user提供的信息，按照Workflows step by step生成{codeSetting.Language}的{codeSetting.LanguageRuntime}的代码。

    ## Constrains:
    1. step by step执行Workflows
    2. 特别强调，要求只返回代码，要求只返回代码,要求只返回代码,禁止返回说明信息和markdown格式的代码
    3. 除掉代码的开始的“```{codeSetting.Language}”，除掉代码结尾的“```”
    4. 生成请求API代码时，要使用请求API的参数
    5. 使用{_localizer["culture"].Value}语言在关键部分添加注释，务必使用{_localizer["culture"].Value}给出注释
    6、一定要按【代码模板】生成代码

    ## Skills:
    1. 出色的分析能力
    2. 能准确提炼出【请求API】中的请求参数
    3. 精通{codeSetting.Language}
    4. 请求参数如果有随机码，能够生成随机码

    ## Workflows: 
    ${{{{Workflows}}}}

    ## Initialization
    作为角色 <Role>，你拥有 <Skills>，严格遵守 <Constrains>，按照 <Workflows>输出结果代码。    
    """;
            var Workflows = new List<string>
            {
                "1. step1 整理出【请求API】中全部请求参数，形成一个“API请求参列表” ",
                "2. step2 根据【请求API】请求要求，结合【代码模板】，生成请求这个api的请求代码，需要注意的时，请求的参数是“API请求参列表”"
            };

            foreach (var item in otherSettings)
            {
                Workflows.Add($"{Workflows.Count + 1}. step{Workflows.Count + 1}结合【{item.Key}】来完成代码生成");
            }
            foreach (var item in otherAPIResults)
            {
                Workflows.Add($"{Workflows.Count + 1}. step{Workflows.Count + 1} 结合【{item.Key}】来完成代码生成");
            }
            if (!string.IsNullOrWhiteSpace(apiQuestion.Prompt))
            {
                Workflows.Add($"{Workflows.Count + 1}. step{Workflows.Count + 1} 结合用户提的【用户要求】来完成代码生成");
            }
            systemPrompt = systemPrompt.Replace("${{Workflows}}", string.Join('\n', Workflows.ToArray()));
            var referenceAnswers = new Dictionary<string, string>
            {
                { "【代码模板】", codeTemplate },
                { "【请求API】", signInAPIContent },
            };
            foreach (var item in otherSettings)
            {
                referenceAnswers.Add($"【{item.Key}】", item.Value);
            }
            foreach (var item in otherAPIResults)
            {
                referenceAnswers.Add($"【{item.Key}】", item.Value);
            }
            if (!string.IsNullOrWhiteSpace(apiQuestion.Prompt))
            {
                referenceAnswers.Add("【用户要求】", apiQuestion.Prompt);
            }
            var question = "生码请求代码";
            await foreach (var item in _aiService.StreamingChatAsync(requestID, systemPrompt, question, referenceAnswers: referenceAnswers, cancellationToken))
            {
                yield return item;
            }
        }
        #endregion

        #region CallBack
        public async Task<string> GetCallBackResponse(string connectionID, string requestJson, CancellationToken cancellationToken)
        {
            var appID = await _distributedCache.GetStringAsync(connectionID);
            if (string.IsNullOrWhiteSpace(appID))
            {
                return "";
            }
            else
            {
                var api = await _apiDataRepository.GetApiDataAsync(int.Parse(appID), cancellationToken);
                if (api != null && api.CategoryName == CategoryType.NeedCallBack && api.LinkAPIID != null)
                {
                    var linkContents = new StringBuilder();
                    foreach (var linkAPIID in api.LinkAPIID)
                    {
                        var linkAPI = await _apiDataRepository.GetApiDataAsync(linkAPIID, cancellationToken);
                        linkContents.AppendLine(linkAPI.Content);
                    }
                    if (!string.IsNullOrWhiteSpace(linkContents.ToString()))
                    {
                        var prompt = $"""
                        参考API文档如下：
                        {linkContents.ToString()}
                        Request参数如下：
                        {requestJson}
                        请根据API文档，和Request参数，给出正确的response的json
                        要求只返回json格式
                        """;
                        var result = await _aiService.ChatAsync("你会根据API文档，和Request参数，给出正确的response的json", prompt, cancellationToken);
                        return result.Replace("```json", "").TrimEnd('`');
                    }
                    return "";
                }
                else
                {
                    _logger.LogWarning("没有找到对应的API");
                    return "";
                }
            }
        }



        public async Task<string> CallBackAsync(string body, Dictionary<string, Dictionary<string, string>> requestParmeters, CancellationToken cancellationToken)
        {
            var json = "[";
            foreach (var item in requestParmeters)
            {
                json += "{";
                json += $"\"{item.Key}\":{System.Text.Json.JsonSerializer.Serialize(item.Value)}";
                json += "},";
            }
            json += $"\"body\":{body}";
            json += "]";
            return json;
        }

        #endregion
    }

}
