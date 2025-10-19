# GitHub Copilot Training - 智能AI代理演示项目# GitHub Copilot Training Codebase Instructions



这是一个多语言（中文/日文）C# AI开发训练项目，展示Microsoft Semantic Kernel框架的各种使用模式。## Project Overview

This is a **Microsoft Semantic Kernel and AI Agent training repository** demonstrating various patterns for building AI agents, chat systems, and orchestrations using Azure OpenAI and .NET 8+. The codebase contains ~20+ demo projects showcasing different architectural patterns.

## 项目架构模式

## Core Architecture Patterns

### AI服务配置标准

- **外部密钥文件**: 所有项目使用 `C://gpt/azure_key.txt` 存储Azure OpenAI配置### 1. **AI Configuration Strategy**

  - 文件格式: `[0]部署名称`, `[1]端点`, `[2]API密钥`- **Azure OpenAI Keys**: External file `C://gpt/azure_key.txt` with format:

  - 示例加载: `var arr = File.ReadLines("C://gpt/azure_key.txt").ToArray();`  ```

- **配置模式**: 使用 `AIConfig` 类结构化AI服务配置（见 `just-agi-api/Models/AIConfig.cs`）  [0] = deploymentName (e.g., "gpt-4.1")

  [1] = endpoint URL

### Semantic Kernel代理模式  [2] = API key

项目演示了4种SK Agent编排模式：  ```

1. **Concurrent**: 并发执行多个代理（`SK-Agent-Concurrent`）- **Alternative**: `appsettings.json` with `AIConfig` section for OpenAI/Azure configs

2. **Sequential**: 顺序执行工作流（`SK-Agent-Sequential`） - **Pattern**: Always read credentials from external sources, never hardcode

3. **GroupChat**: 群聊模式代理交互（`SK-Agent-GroupChat`）

4. **Handoff**: 代理间任务传递（`SK-Agent-Handoff`）### 2. **Semantic Kernel Agent Orchestrations**



### Web API标准#### Sequential Orchestration (`SK-Agent-Sequential/`)

- **最小API**: 使用 `app.MapPost/Get` 模式，不使用控制器```csharp

- **依赖注入**: Repository-Service模式（见 `CustomModeDemo`）var orchestration = new SequentialOrchestration(agent1, agent2, agent3)

- **接口设计**: 使用 `I{Service}` 接口约定（如 `IBlogService`, `IBlogRepository`）{

    ResponseCallback = (res) => { /* handle response */ },

## 开发约定    LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole())

};

### 多语言注释要求```

- **C#函数**: 必须包含中文和日文注释- Agents execute in order, each building on previous results

- **格式**: `/// <summary>中文描述 / 日本語説明</summary>`- Example: DataQuerier → Charter → ImageViewer pipeline

- **参数**: `/// <param name="x">中文说明 / 日本語説明</param>`

- **返回值**: `/// <returns>中文说明 / 日本語説明</returns>`#### Concurrent Orchestration (`SK-Agent-Concurrent/`)

- All agents process input simultaneously

### 代理创建模式- Used for parallel information extraction (e.g., resume parsing)

```csharp

ChatCompletionAgent CreateAgent(string instructions, string? description = null, string? name = null, Kernel? kernel = null)#### Handoff Orchestration (`SK-Agent-Handoff/`)

{- Triage agent routes to specialized agents (OrderStatus, Return, Refund)

    return new ChatCompletionAgent- Customer service pattern with role-based delegation

    {

        Name = name,#### Group Chat Orchestration (`SK-Agent-GroupChat/`)

        Description = description,- Multi-agent collaboration with selection strategy

        Instructions = instructions,- Termination via `KernelFunctionTerminationStrategy`

        Kernel = kernel,

    };#### Magentic Orchestration (`SK-Agent-Magentic/`)

}- Manager-led orchestration delegating to researcher/writer agents

```- Uses `StandardMagenticManager` for intelligent task routing



### 数据模型约定### 3. **ChatCompletionAgent Pattern**

- 使用 `required` 关键字标记必需属性Standard agent creation follows this structure:

- GUID主键命名: `{Entity}Id` (如 `Id`)```csharp

- 数组属性使用明确类型 (如 `string[] Tags`)ChatCompletionAgent agent = new ChatCompletionAgent()

{

## 核心技术栈    Name = "AgentName",

    Instructions = """系统提示词""",

### AI功能模块    Description = "代理功能描述",

- **聊天完成**: `IChatCompletionService` - 基础对话    Kernel = kernel,

- **多模态**: 图像转文本、文本转图像、音频处理    Arguments = new KernelArguments(new PromptExecutionSettings() 

- **函数调用**: 使用 `FunctionChoiceBehavior.Auto()`    { 

- **嵌入向量**: 文本语义搜索        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() 

    })

### 数据库支持};

- **主数据库**: PostgreSQL（见 `TokyoAI_Database_Documentation.md`）```

- **连接字符串**: 标准格式 `Server=127.0.0.1;Port=5432;...`

- **缓存**: Redis支持### 4. **Kernel Plugin System**

Two primary methods for adding tools:

## 快速启动```csharp

// Method 1: Type-based plugins

### 运行聊天服务kernel.Plugins.AddFromType<StoreSystem>("StoreSystem");

```bash

cd SK-Chat// Method 2: Function-based plugins

dotnet runkernel.Plugins.Add(KernelPluginFactory.CreateFromObject(new OrderPlugin()));

# POST http://localhost:5000/chat {"Input": "你好"}```

```

**Plugin Conventions**:

### 运行代理演示- Use `[KernelFunction]` attribute

```bash- Use `[Description("...")]` for function and parameter descriptions

cd SK-Agent-Concurrent- Descriptions are crucial - they guide the AI in function selection

dotnet run  # 并发简历分析演示- Example from `StoreSystem`: Query, Create, Update, Delete operations

```

### 5. **Model Context Protocol (MCP) Integration**

### API服务For external service integration (see `OfficeAgentDemo/`):

```bash```csharp

cd just-agi-apiawait kernel.Plugins.AddMcpFunctionsFromSseServerAsync(

dotnet run    "ServiceName", 

# Swagger: http://localhost:5000/swagger    new Uri("http://localhost:3001/sse")

```);

```

## 常见模式- Used for connecting to Node.js/TypeScript MCP servers

- Enables agent access to external APIs and data sources

### Kernel构建

```csharp### 6. **Streaming Patterns**

var kernel = Kernel.CreateBuilder()All streaming follows this pattern:

    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, key)```csharp

    .Build();public async IAsyncEnumerable<string?> StreamingChatAsync(...)

```{

    await foreach (var item in chatCompletionService.GetStreamingChatMessageContentsAsync(...))

### 代理编排    {

```csharp        yield return item.Content;

var orchestration = new ConcurrentOrchestration(agent1, agent2, agent3)    }

{}

    ResponseCallback = (response) => { /* 处理响应 */ },```

    LoggerFactory = NullLoggerFactory.Instance

};## Project Structure Conventions

```

### Demo Project Naming

### Web API端点- `SK-*`: Semantic Kernel demos (Agent, Chat, Embedding, Audio, Image)

```csharp- `*Demo`: Web API demos (AgentModeDemo, OfficeAgentDemo, CustomModeDemo)

app.MapPost("/endpoint", async (ServiceType service, RequestModel request) => - `*API`: Production-style APIs (just-agi-api, SmartAPI)

{

    var result = await service.ProcessAsync(request);### Typical Project Structure

    return Results.Ok(result);```

});ProjectName/

```├── Program.cs              # Entry point, orchestration setup
├── appsettings.json        # Configuration (DB, AI configs)
├── Models/                 # DTOs and domain models
├── Services/               # AIService, business logic
├── Repositories/           # Data access layer (Dapper-based)
└── wwwroot/                # Static files for web demos
```

## Critical Development Workflows

### Running Demos Locally
1. **Set up Azure OpenAI credentials**:
   - Create `C:\gpt\azure_key.txt` with 3 lines (deployment/endpoint/key)
   - OR configure `appsettings.json` AIConfig section

2. **For console demos** (SK-* projects):
   ```powershell
   cd SK-Agent-Sequential
   dotnet run
   ```

3. **For web demos** (*Demo projects):
   ```powershell
   cd AgentModeDemo
   dotnet run
   # Opens on http://localhost:5000
   ```

4. **Database-dependent projects** (just-agi-api, SmartAPI):
   - PostgreSQL required (see `ConnectionStrings:DefaultConnection`)
   - Schema: `TokyoAI_Database_Schema.sql`
   - Docs: `TokyoAI_Database_Documentation.md`

### Testing Agent Orchestrations
- Use `InProcessRuntime` for all orchestrations:
  ```csharp
  InProcessRuntime runtime = new();
  await runtime.StartAsync();
  var result = await orchestration.InvokeAsync(input, runtime);
  await runtime.RunUntilIdleAsync();
  ```

## Language and Documentation Standards

### Multilingual Comments (Chinese + Japanese)
All C# functions MUST include bilingual XML comments:
```csharp
/// <summary>
/// 两个整数相加 / 2つの整数を加算する
/// </summary>
/// <param name="a">第一个加数 / 最初の加数</param>
/// <returns>返回两数之和 / 2つの数の合計を返す</returns>
```
This is enforced by `.github/instructions/gsw.instructions.md`.

### Agent Instructions Language
- Agent `Instructions` fields: **Chinese only**
- Agent `Description` fields: **Chinese only**
- Console output: **Chinese**
- Code comments: **Chinese + Japanese**

## Key Technical Decisions

### Why Multiple Orchestration Patterns?
Each pattern solves different problems:
- **Sequential**: Workflow pipelines (ETL, data → visualization)
- **Concurrent**: Parallel data extraction
- **Handoff**: Customer service routing
- **GroupChat**: Collaborative problem-solving
- **Magentic**: Complex research tasks needing delegation

### Why External Credential Files?
- Demo/training environment convenience
- Never committed to Git (see `.gitignore`)
- Production projects (SmartAPI, just-agi-api) use `appsettings.json`

### Database Strategy
- **PostgreSQL** for all database demos
- **Dapper** for data access (micro-ORM)
- Schema includes departments, positions, employees (Tokyo AI org structure)

## Common Patterns to Follow

### 1. Creating New Agent Demos
```csharp
// 1. Load credentials
var arr = File.ReadLines("C://gpt/azure_key.txt").ToArray();
var endpoint = arr[1];
var deploymentName = arr[0];
var key = arr[2];

// 2. Build kernel with plugins
var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, key)
    .Build();
kernel.Plugins.AddFromType<YourPlugin>();

// 3. Create agents with Chinese instructions
ChatCompletionAgent agent = new() { /* ... */ };

// 4. Set up orchestration
var orchestration = new XOrchestration(agent1, agent2);

// 5. Run with InProcessRuntime
InProcessRuntime runtime = new();
await runtime.StartAsync();
var result = await orchestration.InvokeAsync(input, runtime);
```

### 2. Creating Web API Endpoints
Follow `just-agi-api/Program.cs` pattern:
- Register services: `AddSingleton<AIConfig>()`, `AddMemoryCache()`
- Use minimal APIs: `app.MapGet/MapPost`
- Streaming: Set `Response.ContentType = "text/event-stream"`

### 3. Plugin Development
```csharp
public class MyPlugin
{
    [KernelFunction]
    [Description("明确描述功能，供AI理解")]
    public async Task<string> DoSomething(
        [Description("参数说明")] string param)
    {
        // Implementation
        return result;
    }
}
```

## Important Files

- **Database**: `TokyoAI_Database_Schema.sql`, `TokyoAI_Database_Documentation.md`
- **Instructions**: `.github/instructions/gsw.instructions.md` (C# comment rules)
- **Chat Mode**: `.github/chatmodes/MyGPT4.1.chatmode.md` (custom agent mode)
- **Solution**: `GitHubCopilotTraining.sln` (all projects)

## Debugging Tips

1. **Orchestration not responding**: Check `ResponseCallback` is set
2. **Function calling not working**: Verify `FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()`
3. **Streaming breaks**: Ensure `text/event-stream` content type
4. **Agent credentials fail**: Verify `azure_key.txt` path and format
5. **Database errors**: Check PostgreSQL connection and schema exists

## External Dependencies

- **Microsoft.SemanticKernel** (core)
- **Microsoft.SemanticKernel.Agents.*** (orchestration types)
- **Microsoft.SemanticKernel.Connectors.OpenAI** (Azure OpenAI)
- **Dapper** (database access)
- **ModelContextProtocol.SemanticKernel.Extensions** (MCP integration)

When adding new demos, maintain consistency with these patterns and ensure multilingual documentation.
