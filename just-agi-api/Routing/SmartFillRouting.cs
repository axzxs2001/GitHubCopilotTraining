
using Microsoft.Extensions.Caching.Memory;
using just_agi_api.Models;
using just_agi_api.Services;
using just_agi_api.IServices;
using just_agi_api.Repositories;
using just_agi_api.IRepositories;
using Dapper;
using just_agi_api.Common;
using System.Diagnostics;

// todos: 这部分代码需要增加日志记录和错误处理机制，以提高系统的健壮性和可维护性。
namespace just_agi_api.Routing
{
    public static class SmartFillRouting
    {
        public static void AddSmartFill(this IServiceCollection services)
        {
            SqlMapper.AddTypeHandler(new JsonUserUIDataArrayHandler());
            services.AddScoped<ISmartFillRepository, SmartFillRepository>();
            services.AddScoped<ISmartFillService, SmartFillService>();

        }
        public static void MapSmartFillRoutes(this WebApplication app)
        {

            app.MapPost("/starrecord", async (AudioEntity audio, ISmartFillService smartFillService) =>
            {
                if (string.IsNullOrWhiteSpace(audio.UserUrl))
                {
                    app.Logger.LogWarning("UserUrl不能为空！");
                    return "UserUrl is empty";
                }
                return await smartFillService.StarRecordAsync(audio);
            });
            app.MapPost("/stoprecord", async (IMemoryCache cache, ISmartFillService smartFillService, HttpContext context, AudioEntity audio) =>
            {
                if (string.IsNullOrWhiteSpace(audio.UserUrl))
                {
                    app.Logger.LogWarning("UserUrl不能为空！");
                    return "UserUrl is empty";
                }
                context.Response.ContentType = "application/json";
                return await smartFillService.StopRecordAsync(audio);
            });

            app.MapPost("/contenttojson", async (HttpContext context, ISmartFillService smartFillService, AnswerEntity answerEntity) =>
            {
                if (string.IsNullOrWhiteSpace(answerEntity.UserUrl))
                {
                    app.Logger.LogWarning("UserUrl不能为空！");
                    await context.Response.WriteAsJsonAsync("{}");

                }
                else
                {

                    context.Response.ContentType = "application/json";
                    var json = await smartFillService.ContentToJsonAsync(answerEntity);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        app.Logger.LogError("没有获取到网页内容！");
                        await context.Response.WriteAsJsonAsync("{}");
                    }
                    else
                    {
                        app.Logger.LogInformation("成功返回：{0}", json);
                        await context.Response.WriteAsJsonAsync(json);
                    }
                }
            });
            app.MapPost("/userinputjson", async (ISmartFillService smartFillService, SmartFillUser smartFillUser) =>
            {
                if (string.IsNullOrWhiteSpace(smartFillUser.UserUrl))
                {
                    app.Logger.LogWarning("UserUrl不能为空！");
                    return "UserUrl is empty";
                }
                return await smartFillService.GetJsonByUrl(smartFillUser.UserUrl);
            });

            app.MapPost("/addsmartfill", async (ISmartFillService smartFillService, SmartFillUser smartFillUser) =>
            {
                if (string.IsNullOrWhiteSpace(smartFillUser.UserUrl))
                {
                    app.Logger.LogWarning("UserUrl不能为空！");
                    return new { Result = false, Message = "UserUrl is empty", Data = new SmartFillUser() };
                }
                if (smartFillUser.UserData == null || smartFillUser.UserData.Length == 0)
                {
                    app.Logger.LogWarning("UserData不能为空！");
                    return new { Result = false, Message = "UserUrl is empty", Data = new SmartFillUser() };
                }
                return new { Result = true, Message = "", Data = await smartFillService.AddSmartFillUserAsync(smartFillUser) };
            });

        }
    }
}
