using AutoMapper;
using just_agi_api.IRepositories;
using just_agi_api.IServices;
using just_agi_api.Models;
using just_agi_api.Repositories;
using just_agi_api.Services;

namespace just_agi_api.Routing
{
    public static class PromptItemRouting
    {
        public static void AddPromptItem(this IServiceCollection services)
        {
            services.AddTransient<IPromptItemService, PromptItemService>();
            services.AddTransient<IPromptItemRepository, PromptItemRepository>();

        }
        public static void MapPromptItemRoutes(this WebApplication app)
        {
            app.MapGet("/promptitem/{id}", async (IPromptItemService promptItemService, long id) =>
            {
                app.Logger.LogInformation("按照 {0} 查询PromptItem");
                var promptItem = await promptItemService.GetPromptItemByIdAsync(id);
                return promptItem;
            });

            app.MapGet("/promptitems", async (IPromptItemService promptItemService, long promptID) =>
            {
                app.Logger.LogInformation("查询PromptItem列表");
                return await promptItemService.GetPromptItemListAsync(promptID);
            });

            app.MapPost("/promptitem", async (IPromptItemService promptItemService, PromptItem promptItem) =>
            {
                app.Logger.LogInformation("添加PromptItem");
                return await promptItemService.AddPromptItemAsync(promptItem);
            });

            app.MapPut("/promptitem", async (IPromptItemService promptItemService, PromptItem promptItem) =>
            {
                app.Logger.LogInformation("修改PromptItem");
                return await promptItemService.UpdatePromptItemAsync(promptItem);
            });

            app.MapDelete("/promptitem/{id}", async (IPromptItemService promptItemService, long id) =>
            {
                app.Logger.LogInformation("失效PromptItem");
                return await promptItemService.InvalidatePromptItemAsync(id);
            });
        }
    }
}
