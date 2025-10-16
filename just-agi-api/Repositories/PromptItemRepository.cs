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
        /// �����ʾ����
        /// </summary>
        /// <param name="promptItem">��ʾ����ʵ��</param>
        /// <returns>��Ӻ����ʾ����ʵ��</returns>
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
        /// �޸���ʾ����
        /// </summary>
        /// <param name="promptItem">��ʾ����ʵ��</param>
        /// <returns>�޸ĺ����ʾ����ʵ��</returns>
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
        /// ʧЧ��ʾ����
        /// </summary>
        /// <param name="id">��ʾ������</param>
        /// <returns>�Ƿ�ɹ�ʧЧ</returns>
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
        /// ��ID��ѯ��ʾ����
        /// </summary>
        /// <param name="id">��ʾ������</param>
        /// <returns>��ѯ������ʾ����ʵ��</returns>
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
        /// ��ѯ��ʾ�����б�
        /// </summary>
        /// <param name="id">��ʾ�ʱ��</param>
        /// <returns>��ʾ�����б�</returns>
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
