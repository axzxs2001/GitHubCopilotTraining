/* Copilot Prompt
#file:'repos\\just-agi\\just-agi-api\\Models\\Prompt.cs' ORM是dapper，数据库postgres，生成Prompt和PromptItem的添加，修改，失效，按ID查询，查询列表的仓储异步方法，并且给方法和参数添加中文注释。
 */

using Dapper;
using just_agi_api.IRepositories;
using just_agi_api.Models;
using Npgsql;

namespace just_agi_api.Repositories
{
    public class PromptRepository : IPromptRepository
    {
        private readonly string _connectionString;

        public PromptRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// 添加提示词
        /// </summary>
        /// <param name="prompt">提示词实体</param>
        /// <returns>添加后的提示词实体</returns>
        public async Task<Prompt> AddPromptAsync(Prompt prompt)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"INSERT INTO Prompts (Tags, Name, LLMUrl, EnterpriseID, RoleID, Status, Valid, CreateTime, CreateUser, ModifyTime, ModifyUser)
                                  VALUES (@Tags::jsonb, @Name, @LLMUrl, @EnterpriseID, @RoleID, @Status, @Valid, @CreateTime, @CreateUser, @ModifyTime, @ModifyUser)
                                  RETURNING *";

                return await connection.QuerySingleOrDefaultAsync<Prompt>(query, prompt);
            }
        }

        /// <summary>
        /// 修改提示词
        /// </summary>
        /// <param name="prompt">提示词实体</param>
        /// <returns>修改后的提示词实体</returns>
        public async Task<Prompt> UpdatePromptAsync(Prompt prompt)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"UPDATE Prompts
                                  SET Tags = @Tags::jsonb, Name = @Name, LLMUrl = @LLMUrl, EnterpriseID = @EnterpriseID, RoleID = @RoleID, Status = @Status, Valid = @Valid, CreateTime = @CreateTime, CreateUser = @CreateUser, ModifyTime = @ModifyTime, ModifyUser = @ModifyUser
                                  WHERE ID = @ID
                                  RETURNING *";

                return await connection.QuerySingleOrDefaultAsync<Prompt>(query, prompt);
            }
        }

        /// <summary>
        /// 失效提示词
        /// </summary>
        /// <param name="id">提示词编号</param>
        /// <returns>是否成功失效</returns>
        public async Task<bool> InvalidatePromptAsync(long id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"UPDATE Prompts
                                  SET Valid = false
                                  WHERE ID = @ID";

                var affectedRows = await connection.ExecuteAsync(query, new { ID = id });

                return affectedRows > 0;
            }
        }

        /// <summary>
        /// 按ID查询提示词
        /// </summary>
        /// <param name="id">提示词编号</param>
        /// <returns>查询到的提示词实体</returns>
        public async Task<Prompt> GetPromptByIdAsync(long id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM Prompts
                                  WHERE ID = @ID";

                return await connection.QuerySingleOrDefaultAsync<Prompt>(query, new { ID = id });
            }
        }

        /// <summary>
        /// 查询提示词列表
        /// </summary>
        /// <returns>提示词列表</returns>
        public async Task<IEnumerable<Prompt>> GetPromptListAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM Prompts";

                return await connection.QueryAsync<Prompt>(query);
            }
        }
    }
}
