using just_agi_api.Models;
using Dapper;
using Npgsql;
using just_agi_api.IRepositories;

namespace just_agi_api.Repositories
{
    public class SmartFillRepository: ISmartFillRepository
    {      
        private readonly ILogger<SmartFillRepository> _logger;
        private readonly string? _connectionString;
        public SmartFillRepository( ILogger<SmartFillRepository> logger, IConfiguration configuration)
        {            
            _logger = logger;
            _connectionString = configuration?.GetConnectionString("DefaultConnection");
        }     

        public async Task<SmartFillUser> GetSmartFillUserAsync(string userUrl)
        {
           using(var connection = new NpgsqlConnection(_connectionString))
            {
               var sql = "SELECT * FROM SmartFillUsers WHERE validate=true and userurl = @userurl";
               return await connection.QueryFirstOrDefaultAsync<SmartFillUser>(sql, new { userurl = userUrl });               
           }
        }
        //生成增，删，改，查的方法
        public async Task<SmartFillUser> AddSmartFillUserAsync(SmartFillUser smartFillUser)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"INSERT INTO SmartFillUser (UserUrl, UserData, Validate, CreateTime, CreateUser, ModifyTime, ModifyUser)
                                  VALUES (@UserUrl, @UserData, @Validate, @CreateTime, @CreateUser, @ModifyTime, @ModifyUser)
                                  RETURNING *";

                return await connection.QuerySingleOrDefaultAsync<SmartFillUser>(query, smartFillUser);
            }
        }
        public async Task<SmartFillUser> UpdateSmartFillUserAsync(SmartFillUser smartFillUser)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"UPDATE SmartFillUser
                                  SET UserUrl = @UserUrl, UserData = @UserData, Validate = @Validate, CreateTime = @CreateTime, CreateUser = @CreateUser, ModifyTime = @ModifyTime, ModifyUser = @ModifyUser
                                  WHERE ID = @ID
                                  RETURNING *";

                return await connection.QuerySingleOrDefaultAsync<SmartFillUser>(query, smartFillUser);
            }
        }
        public async Task<bool> InvalidateSmartFillUserAsync(long id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"UPDATE SmartFillUser
                                  SET Validate = false
                                  WHERE ID = @ID";

                var affectedRows = await connection.ExecuteAsync(query, new { ID = id });

                return affectedRows > 0;
            }
        }
        public async Task<SmartFillUser> GetSmartFillUserByIdAsync(long id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM SmartFillUser
                                  WHERE ID = @ID";

                return await connection.QuerySingleOrDefaultAsync<SmartFillUser>(query, new { ID = id });
            }
        }
        public async Task<IEnumerable<SmartFillUser>> GetSmartFillUserListAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM SmartFillUser";
                return await connection.QueryAsync<SmartFillUser>(query);
            }
        }
        public async Task<IEnumerable<SmartFillUser>> GetSmartFillUserListAsync(int page, int pageSize)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM SmartFillUser ORDER BY ID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                return await connection.QueryAsync<SmartFillUser>(query, new { Offset = (page - 1) * pageSize, PageSize = pageSize });
            }
        }
        public async Task<int> GetSmartFillUserCountAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"SELECT COUNT(*) FROM SmartFillUser";
                return await connection.ExecuteScalarAsync<int>(query);
            }
        }

    }


}
