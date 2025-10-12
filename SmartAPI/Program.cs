using CNSSDK;
using Dapper;
using FluentValidation;
using Google.Api;
using Google.Protobuf;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI.Common;
using NATS.Client.Internals.SimpleJSON;
using SmartAPI;
using SmartAPI.Models;
using SmartAPI.Respositories;
using SmartAPI.Services;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
SqlMapper.AddTypeHandler(new JsonTypeHandler<List<int>>());
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "secrets/gcp/sre-common-test-379805-faeba55faab0.json");
var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy
            .AllowAnyOrigin()
            .AllowAnyHeader()       
            .AllowAnyMethod();
        });
});



builder.Services.AddSignalR();
builder.Services.AddDistributedMemoryCache();
//�����ļ�
builder.Configuration.AddJsonFile("configs/appsettings.json", true, true);
builder.Configuration.AddJsonFile("secrets/secrets.json", true, true);

//����AI
builder.Services.AddScoped<IApiDataRepository, ApiDataRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IApiDataService, ApiDataService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<ICodeSettingRepository, CodeSettingRepository>();
builder.Services.AddScoped<ICodeSettingService, CodeSettingService>();
builder.Services.AddScoped<ICustomerInfoRepository, CustomerInfoRepository>();
builder.Services.AddScoped<ICustomerInfoService, CustomerInfoService>();

builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractService, ContractService>();

builder.Services.AddScoped<IContractUserRepository, ContractUserRepository>();
builder.Services.AddScoped<IContractUserService, ContractUserService>();

builder.Services.AddValidatorsFromAssemblyContaining<ProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ApiDataValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CustomerInfoValidator>();

await builder.AddAIAsync();


var conf = new AppConfig()
{
    AppID = "email-smtp-smartapi",
    AppName = "smartapi",
    Tenant = "NSS",
    APIKey = "apiKey",
    Timezone = "GMT+9",
    Charset = "UTF-8",
};
var outpuConf = new OutputConfig()
{
    Output = CNSOutput.NATS,
    NatsConf = new NatsOption()
    {
        Address = "nats://qa-nats-interface-connect-nlb-996d5f0edade33c1.elb.ap-northeast-1.amazonaws.com:4222",
        Subject = "CNS-TOPIC"
    },
};
var cns = new CnsSDK(conf, outpuConf);
builder.Services.AddSingleton<ICnsSDK>(cns);

SqlMapper.AddTypeHandler(new JsonListStringHandler());
SqlMapper.AddTypeHandler(new JsonListSIntHandler());

// ���ӱ��ػ�֧��
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// ע�� DataAnnotations ���ػ�
builder.Services.AddControllers()
    .AddDataAnnotationsLocalization(options =>
    {
        var localizer = builder.Services.BuildServiceProvider().GetService<IStringLocalizer<SharedResource>>();
        options.DataAnnotationLocalizerProvider = (type, factory) => localizer;
    });


var app = builder.Build();
app.UseCors();

// ���ñ��ػ��м��
var supportedCultures = new[] { "en-US", "zh-CN", "ja-JP" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
//localizationOptions.AddInitialRequestCultureProvider(new QueryStringRequestCultureProvider());
//localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localizationOptions);


app.UseStaticFiles();
var smarapi = app.MapGroup("smartapi");

app.MapGet("/healthz", () => "ok");
#region AI
smarapi.MapPost("/apiquery", APIQuery);
async IAsyncEnumerable<string> APIQuery(HttpRequest request, IStringLocalizer<SharedResource> localizer, IApiDataService apiDataService, [FromBody] APIQuest apiQuestion, CancellationToken cancellationToken)
{
    app.Logger.LogInformation("http Query culture:" + request.Query["culture"]);
    app.Logger.LogInformation("localizer culture:" + localizer["culture"].Value);
    var requestID = request.Headers["Request-ID"];
    app.Logger.LogInformation("RequestID: {0}", requestID);
    await foreach (var item in apiDataService.GenerateCode(requestID, apiQuestion, cancellationToken))
    {
        yield return item;
    }
}

smarapi.MapPost("/run", RunCodeAsync);
async IAsyncEnumerable<RunResult> RunCodeAsync(RunParmeter runParmeter, IApiDataService apiDataService, CancellationToken cancellationToken)
{
    if (runParmeter == null)
    {
        yield return new RunResult { Result = false, Message = "��������" };
    }
    else
    {
        if (string.IsNullOrWhiteSpace(runParmeter.RunTime))
        {
            yield return new RunResult { Result = false, Message = "����ʱΪ��" };
        }
        else if (string.IsNullOrWhiteSpace(runParmeter.SourceCode))
        {
            yield return new RunResult { Result = false, Message = "Դ��Ϊ��" };
        }
        else
        {
            await foreach (var result in apiDataService.RunCodeAsync(runParmeter, cancellationToken))
            {
                yield return result;
            }
        }
    }
}
;
#region ע��
//app.MapPost("/query", QueryAsync);
//async IAsyncEnumerable<string> QueryAsync(IAIService aiService, HttpRequest request, AskEntity askEntity, CancellationToken cancellationToken)
//{
//    var requestID = request.Headers["Request-ID"];
//    app.Logger.LogInformation("�յ����⣺{0}", askEntity.Question);
//    var referenceAnswers = new List<string>();
//    await foreach (var content in aiService.VectorizedSearchAsync(askEntity.Question))
//    {
//        referenceAnswers.Add(content);
//    }
//    var systemMessage = "Ҫ�󷵻صĽ������markdown�ĸ�ʽչʾ,ע�⣬�ı��еĴ��벻��Ҫʹ��code��ǩ������֧��mermaid";
//    await foreach (var item in aiService.StreamingChatAsync(requestID, systemMessage, askEntity.Question, referenceAnswers, cancellationToken))
//    {
//        yield return item;
//    }
//}
//app.MapGet("/loadcache/{productID}", async (IAIService aiService, IApiDataService apiService, int productID, CancellationToken cancellationToken) =>
//{
//    app.Logger.LogInformation("���ػ��棺product id {0}", productID);
//    var apiDatas = await apiService.GetApiDatasAsync(productID, cancellationToken);
//    foreach (var apiData in apiDatas)
//    {
//        if (!string.IsNullOrWhiteSpace(apiData.Content))
//        {
//            var apiVectorRecord = new APIVectorRecord
//            {
//                Id = apiData.Id,
//                ProductId = apiData.ProductId,
//                ApiType = apiData.ApiType,
//                Content = apiData.Content,
//            };
//            await aiService.EmbeddingVectoryAsync(apiVectorRecord);
//        }
//    }
//    return Results.Ok();
//});
#endregion
#endregion

#region CallBack
smarapi.Map("/callback/{connectionId}", async (HttpContext context
    , string connectionId, IApiDataService apiDataService, IHubContext<ChatHub> hubContext, CancellationToken cancellationToken) =>
{
    try
    {
        var forms = new Dictionary<string, string>();
        if (context.Request.HasFormContentType && context.Request.Form != null)
        {
            foreach (var item in context.Request.Form)
            {
                forms.Add(item.Key, item.Value);
            }
        }
        var queris = new Dictionary<string, string>();
        if (context.Request.Query != null)
        {
            foreach (var item in context.Request.Query)
            {
                queris.Add(item.Key, item.Value);
            }
        }
        var headers = new Dictionary<string, string>();
        if (context.Request.Headers != null)
        {
            foreach (var item in context.Request.Headers)
            {
                headers.Add(item.Key, item.Value);
            }
        }
        var body = string.Empty;
        if (context.Request.Body != null)
        {
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
            body = await reader.ReadToEndAsync();
        }
        var parmeters = new Dictionary<string, Dictionary<string, string>>();
        if (forms.Count > 0)
        {
            parmeters.Add("forms", forms);
        }
        if (queris.Count > 0)
        {
            parmeters.Add("queris", queris);
        }
        if (headers.Count > 0)
        {
            parmeters.Add("headers", headers);
        }
        var content = await apiDataService.CallBackAsync(body, parmeters, cancellationToken);
        await hubContext?.Clients?.Clients(connectionId)?.SendAsync("ReceiveMessage", content, cancellationToken);
        var result = await apiDataService.GetCallBackResponse(connectionId, content, cancellationToken);
        context.Response.ContentType = "application/json";
        return Results.Json(System.Text.Json.JsonSerializer.Deserialize<dynamic>(result));
    }
    catch (Exception exc)
    {
        return Results.BadRequest(exc.Message);
    }
});
#endregion

#region Product
smarapi.MapGet("/scenes", async (IProductService productService, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await productService.GetScensAsync(cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result.Where(s=>s.Name!= "NSSInternalAPI") };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = Enumerable.Empty<Scene>() };
    }
});
smarapi.MapGet("/back/scenes", async (IProductService productService, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await productService.GetScensAsync(cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = Enumerable.Empty<Scene>() };
    }
});

smarapi.MapGet("/products", async (IProductService productService, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await productService.GetProductsAsync(cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = Enumerable.Empty<Product>() };
    }
});
smarapi.MapGet("/products/{culture}", async (IProductService productService, string culture, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await productService.GetProductsAsync(culture, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = Enumerable.Empty<Product>() };
    }
});
smarapi.MapGet("/product/{id}", async (IProductService productService, int id, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await productService.GetProductAsync(id, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = new Product() };
    }
});
//smarapi.MapGet("/products/{userName}", async (IProductService productService, string userName, CancellationToken cancellationToken) =>
//{
//    try
//    {
//        var result = await productService.GetProductsByUserAsync(userName, cancellationToken);
//        return new { code = 200, msg = "SUCCESS", data = result };
//    }
//    catch (Exception exc)
//    {
//        app.Logger.LogError(exc, exc.Message);
//        return new { code = 1, msg = exc.Message, data = Enumerable.Empty<Product>() };
//    }
//});
smarapi.MapGet("/products/{userName}/{culture}", async (IProductService productService, string culture, string userName, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await productService.GetProductsByUserAsync(userName, culture, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = Enumerable.Empty<Product>() };
    }
});
smarapi.MapPost("/product", async (IProductService productService, Product product, IValidator<Product> validator, CancellationToken cancellationToken) =>
{
    try
    {
        var validationResult = await validator.ValidateAsync(product);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            return new { code = 2, msg = System.Text.Json.JsonSerializer.Serialize(errors, options), data = 0 };
        }
        var id = await productService.AddProductAsync(product, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = id };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = 0 };
    }
});
smarapi.MapPut("/product", async (IProductService productService, Product product, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await productService.ModifyProductAsync(product, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        return new { code = 1, msg = exc.Message, data = false };
    }
});
smarapi.MapDelete("/product/{id}", async (IProductService productService, int id, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await productService.RemoveProductAsync(id, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        return new { code = 1, msg = exc.Message, data = false };
    }
});
#endregion
#region ApiData

smarapi.MapGet("nocategorytypes", () =>
{
    var noAPICategories = typeof(CategoryType)
              .GetFields(BindingFlags.Public | BindingFlags.Static)
              .Where(field => field.GetCustomAttribute<IsAPIAttribute>() == null)
              .Select(field => field.GetValue(null)) // ��ȡ�ֶ�ֵ
              .Cast<string>() // ת��Ϊ�ַ�������
              .ToList();
    return new { code = 200, data = noAPICategories, mgs = "SUCCESS" };
});

smarapi.MapGet("categorytypes", () =>
{
    var APICategories = typeof(CategoryType)
              .GetFields(BindingFlags.Public | BindingFlags.Static)
              .Select(field => field.GetValue(null)) // ��ȡ�ֶ�ֵ
              .ToList();
    return new { code = 200, data = APICategories, mgs = "SUCCESS" };
});

smarapi.MapGet("/apidatas/{productID}", async (IApiDataService apiDataService, IProductService productService, int productID, CancellationToken cancellationToken) =>
{
    try
    {
        var product = await productService.GetProductAsync(productID, cancellationToken);
        var apis = await apiDataService.GetApiDatasAsync(productID, cancellationToken);
        return new { code = 200, data = new { product, apis }, mgs = "SUCCESS" };
    }
    catch (Exception exc)
    {
        return new { code = 1, data = new { product = new Product(), apis = Enumerable.Empty<ApiData>() }, mgs = exc.Message };
    }
});
smarapi.MapGet("/miniapidatas/{productID}", async (IApiDataService apiDataService, IProductService productService, int productID, CancellationToken cancellationToken) =>
{
    try
    {
        var apis = await apiDataService.GetApiDatasByCategoryAsync(productID, cancellationToken);
        return new { code = 200, data = apis.Select(s => new ApiData { Id = s.Id, Name = s.Name,CategoryName=s.CategoryName }), msg = "SUCCESS" };
    }
    catch (Exception exc)
    {
        return new { code = 1, data = Enumerable.Empty<ApiData>(), msg = exc.Message };
    }
});
smarapi.MapGet("/apidata/{id}", async (IApiDataService apiDataService, int id, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await apiDataService.GetApiDataAsync(id, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        return new { code = 1, msg = exc.Message, data = new ApiData() };
    }
});
smarapi.MapPost("/apidata", async (IApiDataService apiDataService, ApiData apiData, IValidator<ApiData> validator, CancellationToken cancellationToken) =>
{
    try
    {
        var validationResult = await validator.ValidateAsync(apiData);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            return new { code = 2, msg = System.Text.Json.JsonSerializer.Serialize(errors, options), data = new ApiData() };
        }
        var result = await apiDataService.AddApiDataAsync(apiData, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        return new { code = 1, msg = exc.Message, data = new ApiData() };
    }
});
smarapi.MapPut("/apidata", async (IApiDataService apiDataService, ApiData apiData, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await apiDataService.ModifyApiDataAsync(apiData, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        return new { code = 1, msg = exc.Message, data = false };
    }
});
smarapi.MapDelete("/apidata/{id}", async (IApiDataService apiDataService, int id, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await apiDataService.RemoveApiDataAsync(id, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        return new { code = 1, msg = exc.Message, data = false };
    }
});
smarapi.MapPost("/copyapis", async (IApiDataService apiDataService, [FromBody] CopyAPIs copyAPIs, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await apiDataService.CopyAPIsAsync(copyAPIs.OldProductID, copyAPIs.NewProductID, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        return new { code = 1, msg = exc.Message, data = false };
    }
});
#endregion
#region CodeSetting
smarapi.MapGet("/codesettings", async (ICodeSettingService codeSettingService, CancellationToken cancellationToken) =>
{
    return await codeSettingService.GetCodeSettingsAsync(cancellationToken);
});
smarapi.MapGet("/codesetting/{id}", async (ICodeSettingService codeSettingService, int id, CancellationToken cancellationToken) =>
{
    return await codeSettingService.GetCodeSettingAsync(id, cancellationToken);
});
smarapi.MapPost("/codesetting", async (ICodeSettingService codeSettingService, [FromBody] CodeSetting codeSetting, CancellationToken cancellationToken) =>
{
    return await codeSettingService.AddCodeSettingAsync(codeSetting, cancellationToken);
});
smarapi.MapPut("/codesetting", async (ICodeSettingService codeSettingService, CodeSetting codeSetting, CancellationToken cancellationToken) =>
{
    return await codeSettingService.ModifyCodeSettingAsync(codeSetting, cancellationToken);
});
smarapi.MapDelete("/codesetting/{id}", async (ICodeSettingService codeSettingService, int id, CancellationToken cancellationToken) =>
{
    return await codeSettingService.RemoveCodeSettingAsync(id, cancellationToken);
});
#endregion

#region CustomerInfo

smarapi.MapPut("/activeuser", async (ICustomerInfoService customerInfoService, CustomerInfo customerInfo, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await customerInfoService.ActiveUserAsync(customerInfo.Id, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = false };
    }
});
smarapi.MapPut("/customerinfo", async (ICustomerInfoService customerInfoService, CustomerInfo customerInfo, IValidator<CustomerInfo> validator, CancellationToken cancellationToken) =>
{
    try
    {
        var validationResult = await validator.ValidateAsync(customerInfo);
        if (!validationResult.IsValid)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            return new { code = 2, msg = System.Text.Json.JsonSerializer.Serialize(errors, options) };
        }
        var result = await customerInfoService.ModifyCustomerInfoAsync(customerInfo, cancellationToken);
        return new { code = 200, msg = "SUCCESS" };
    }
    catch (Exception exc)
    {
        return new { code = 1, msg = exc.Message };
    }
});

smarapi.MapGet("/customerinfos", async (string companyName, string responsiblePerson, int pageSize, int pageNumber, ICustomerInfoService customerInfoService, CancellationToken cancellationToken) =>
{
    try
    {
        var list = await customerInfoService.GetCustomerInfosAsync(companyName, responsiblePerson, pageSize, pageNumber, cancellationToken);
        var count = await customerInfoService.GetCustomerInfoCountAsync(companyName, responsiblePerson, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = new { customerInfos = list, count } };

    }
    catch (Exception exc)
    {
        return new { code = 1, msg = exc.Message, data = new { customerInfos = Enumerable.Empty<CustomerInfo>(), count = 0 } };
    }
});
smarapi.MapGet("/customerinfo/{id}", async (ICustomerInfoService customerInfoService, int id, CancellationToken cancellationToken) =>
{
    try
    {
        var customerInfo = await customerInfoService.GetCustomerInfoAsync(id, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = customerInfo };
    }
    catch (Exception exc)
    {
        return new { code = 1, msg = exc.Message, data = new CustomerInfo() };
    }
});
smarapi.MapPost("/customerinfo", async (ICustomerInfoService customerInfoService, CustomerInfo customerInfo, IValidator<CustomerInfo> validator, CancellationToken cancellationToken) =>
{
    try
    {
        var validationResult = await validator.ValidateAsync(customerInfo);
        if (!validationResult.IsValid)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            return new { code = 2, msg = System.Text.Json.JsonSerializer.Serialize(errors, options) };
        }
        var count = await customerInfoService.GetCountAsync(customerInfo.CompanyName, cancellationToken);
        if (count > 0)
        {
            return new { code = 3, msg = "�������ϼȤ˴��ڤ��ޤ���" };
        }
        var result = await customerInfoService.AddCustomerInfoAsync(customerInfo, cancellationToken);
        return new { code = 200, msg = "SUCCESS" };
    }
    catch (Exception exc)
    {
        return new { code = 1, msg = exc.Message };
    }
});

smarapi.MapDelete("/customerinfo/{id}", async (ICustomerInfoService customerInfoService, int id, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await customerInfoService.RemoveCustomerInfoAsync(id, cancellationToken);
        return new { code = 200, msg = "SUCCESS" };
    }
    catch (Exception exc)
    {
        return new { code = 1, msg = exc.Message };

    }
});
#endregion


#region Contract
smarapi.MapGet("/contracts", async (IContractService contractService, CancellationToken cancellationToken) =>
{
    try
    {
        var contracts = await contractService.GetContractsAsync(cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = contracts };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = Enumerable.Empty<Contract>() };
    }
});

smarapi.MapGet("/contract/{id}", async (IContractService contractService, int id, CancellationToken cancellationToken) =>
{
    try
    {
        var contract = await contractService.GetContractAsync(id, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = contract };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = new Contract() };
    }
});

smarapi.MapPost("/contract", async (IContractService contractService, Contract contract, CancellationToken cancellationToken) =>
{
    try
    {
        var id = await contractService.AddContractAsync(contract, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = id };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = 0 };
    }
});

smarapi.MapPut("/contract", async (IContractService contractService, Contract contract, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await contractService.ModifyContractAsync(contract, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = false };
    }
});

smarapi.MapDelete("/contract/{id}", async (IContractService contractService, int id, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await contractService.RemoveContractAsync(id, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = false };
    }
});
#endregion

#region ContractUser
smarapi.MapGet("/contractusers", async (IContractUserService contractUserService, CancellationToken cancellationToken) =>
{
    try
    {
        var contractUsers = await contractUserService.GetContractUsersAsync(cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = contractUsers };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = Enumerable.Empty<ContractUser>() };
    }
});

smarapi.MapGet("/contractuser/{id}", async (IContractUserService contractUserService, int id, CancellationToken cancellationToken) =>
{
    try
    {
        var contractUser = await contractUserService.GetContractUserAsync(id, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = contractUser };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = new ContractUser() };
    }
});

smarapi.MapPost("/contractuser", async (IContractUserService contractUserService, ContractUser contractUser, CancellationToken cancellationToken) =>
{
    try
    {
        var id = await contractUserService.AddContractUserAsync(contractUser, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = id };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = 0 };
    }
});

smarapi.MapPut("/contractuser", async (IContractUserService contractUserService, ContractUser contractUser, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await contractUserService.ModifyContractUserAsync(contractUser, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = false };
    }
});

smarapi.MapDelete("/contractuser/{id}", async (IContractUserService contractUserService, int id, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await contractUserService.RemoveContractUserAsync(id, cancellationToken);
        return new { code = 200, msg = "SUCCESS", data = result };
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = false };
    }
});

smarapi.MapGet("/contractuser/signed/{user}", async (IContractUserService contractUserService, string user, CancellationToken cancellationToken) =>
{
    try
    {
        var contract = await contractUserService.HasUserSignedLatestContractAsync(user, cancellationToken);
        if (contract != null)
        {
            return new { code = 200, msg = "SUCCESS", data = new { hasSigned = false, contract = contract } };
        }
        else
        {
            return new { code = 200, msg = "SUCCESS", data = new { hasSigned = true, contract = new Contract() } };
        }
    }
    catch (Exception exc)
    {
        app.Logger.LogError(exc, exc.Message);
        return new { code = 1, msg = exc.Message, data = new { hasSigned = false, contract = new Contract() } };
    }
});




#endregion

app.MapHub<ChatHub>("/SmartAPI/chatHub");
app.Run();



public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;
    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }
    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("�½�ConnectionID:{0}", Context.ConnectionId);
        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("�Ͽ�ConnectionID:{0}", Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
    public async Task AddToGroup(string groupName)
    {
        _logger.LogInformation("Add GroupName:{0}", groupName);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }
    public async Task RemoveFromGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
