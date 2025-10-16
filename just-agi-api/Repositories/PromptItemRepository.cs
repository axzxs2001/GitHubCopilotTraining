using Dapper;
using just_agi_api.IRepositories;
using just_agi_api.Models;
using Npgsql;

namespace just_agi_api.Repositories
{
    public class PromptItemRepository : IPromptItemRepository
    {
        private readonly string _connectionString;

        public PromptItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// 添加提示词项
        /// </summary>
        /// <param name="promptItem">提示词项实体</param>
        /// <returns>添加后的提示词项实体</returns>
        public async Task<PromptItem> AddPromptItemAsync(PromptItem promptItem)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"INSERT INTO PromptItems (Version, Prompt, PromptID, Valid, CreateTime, CreateUser, ModifyTime, ModifyUser)
                                  VALUES (@Version, @Prompt, @PromptID, @Valid, @CreateTime, @CreateUser, @ModifyTime, @ModifyUser)
                                  RETURNING *";

                return await connection.QuerySingleOrDefaultAsync<PromptItem>(query, promptItem);
            }
        }

        /// <summary>
        /// 修改提示词项
        /// </summary>
        /// <param name="promptItem">提示词项实体</param>
        /// <returns>修改后的提示词项实体</returns>
        public async Task<PromptItem> UpdatePromptItemAsync(PromptItem promptItem)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"UPDATE PromptItems
                                  SET Version = @Version, Prompt = @Prompt, PromptID = @PromptID, Valid = @Valid, CreateTime = @CreateTime, CreateUser = @CreateUser, ModifyTime = @ModifyTime, ModifyUser = @ModifyUser
                                  WHERE ID = @ID
                                  RETURNING *";

                return await connection.QuerySingleOrDefaultAsync<PromptItem>(query, promptItem);
            }
        }

        /// <summary>
        /// 失效提示词项
        /// </summary>
        /// <param name="id">提示词项编号</param>
        /// <returns>是否成功失效</returns>
        public async Task<bool> InvalidatePromptItemAsync(long id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"UPDATE PromptItems
                                  SET Valid = false
                                  WHERE ID = @ID";

                var affectedRows = await connection.ExecuteAsync(query, new { ID = id });

                return affectedRows > 0;
            }
        }

        /// <summary>
        /// 按ID查询提示词项
        /// </summary>
        /// <param name="id">提示词项编号</param>
        /// <returns>查询到的提示词项实体</returns>
        public async Task<PromptItem> GetPromptItemByIdAsync(long id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM PromptItems
                                  WHERE ID = @ID";

                return await connection.QuerySingleOrDefaultAsync<PromptItem>(query, new { ID = id });
            }
        }

        /// <summary>
        /// 查询提示词项列表
        /// </summary>
        /// <param name="id">提示词编号</param>
        /// <returns>提示词项列表</returns>
        public async Task<IEnumerable<PromptItem>> GetPromptItemListAsync(long id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM PromptItems where PromptID=@PromptID";

                return await connection.QueryAsync<PromptItem>(query, new { PromptID = id });
            }
        }
    }
}
