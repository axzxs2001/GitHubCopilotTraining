using just_agi_api.Models;
using just_agi_api.Routing;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.Configure<AIConfig>(
//   builder.Configuration.GetSection("AIConfig"));
var aiConfig = new AIConfig();
builder.Configuration.Bind("AIConfig", aiConfig);
builder.Services.AddSingleton(aiConfig);


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseStaticFiles();

app.MapGet("/test", () =>
{

    return "";
})
.WithName("test")

.WithOpenApi();

app.Run();

