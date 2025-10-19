
using CustomModeDemo.Models;

var builder = WebApplication.CreateBuilder(args);

// 注册依赖注入
builder.Services.AddSingleton<IBlogRepository, BlogRepository>();
builder.Services.AddSingleton<IBlogService, BlogService>();

var app = builder.Build();

// Blog API 映射
app.MapGet("/blogs", async (IBlogService service) => await service.GetAllAsync());
app.MapGet("/blogs/{id}", async (IBlogService service, Guid id) => await service.GetByIdAsync(id));
app.MapPost("/blogs", async (IBlogService service, Blog blog) => { await service.AddAsync(blog); return Results.Created($"/blogs/{blog.Id}", blog); });
app.MapPut("/blogs/{id}", async (IBlogService service, Guid id, Blog blog) => { blog.Id = id; await service.UpdateAsync(blog); return Results.NoContent(); });
app.MapDelete("/blogs/{id}", async (IBlogService service, Guid id) => { await service.DeleteAsync(id); return Results.NoContent(); });

app.Run();

