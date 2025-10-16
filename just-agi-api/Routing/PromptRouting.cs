using AutoMapper;
using Dapper;
using just_agi_api.Common;
using just_agi_api.IRepositories;
using just_agi_api.IServices;
using just_agi_api.Models;
using just_agi_api.Repositories;
using just_agi_api.Services;

namespace just_agi_api.Routing
{
    public static class PromptRouting
    {
        public static void AddPrompt(this IServiceCollection services)
        {
            SqlMapper.AddTypeHandler(new JsonStringArrayHandler());
            services.AddTransient<IPromptService, PromptService>();
            services.AddTransient<IPromptRepository, PromptRepository>();

        }
        public static void MapPromptRoutes(this WebApplication app)
        {

            app.MapGet("/prompt/{id}", async (IPromptService promptService, IPromptItemService promptItemService, long id) =>
            {
                try
                {
                    app.Logger.LogInformation("按照 {0} 查询Prompt");
                    var prompt = await promptService.GetPromptByIdAsync(id);
                    var promptItems = await promptItemService.GetPromptItemListAsync(id);
                    var config = new MapperConfiguration(cfg =>
      cfg.CreateMap<Prompt, PromptViewModel>());
                    var mapper = new Mapper(config);
                    var result = mapper.Map<PromptViewModel>(prompt);
                    result.PromptItems = promptItems.ToList();
                    return result;
                }
                catch (Exception exc)
                {
                    app.Logger.LogError(exc.Message, exc);
                    throw;
                }
            });

            app.MapGet("/prompts", async (IPromptService promptService) =>
            {
                app.Logger.LogInformation("查询Prompt列表");
                return await promptService.GetPromptListAsync();
            });

            app.MapPost("/prompt", async (IPromptService promptService, Prompt prompt) =>
            {
                app.Logger.LogInformation("添加Prompt");
                return await promptService.AddPromptAsync(prompt);
            });

            app.MapPut("/prompt", async (IPromptService promptService, Prompt prompt) =>
            {
                app.Logger.LogInformation("修改Prompt");
                return await promptService.UpdatePromptAsync(prompt);
            });

            app.MapDelete("/prompt/{id}", async (IPromptService promptService, long id) =>
            {
                app.Logger.LogInformation("失效Prompt");
                return await promptService.InvalidatePromptAsync(id);
            });

        }
    }
}
