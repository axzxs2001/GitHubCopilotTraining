using Dapper;
using just_agi_api.IRepositories;
using just_agi_api.Models;
using Npgsql;
using System.Data;

namespace just_agi_api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<User> AddUserAsync(User user)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"INSERT INTO User (UserName, Password, Salt, Valid, CreateTime, CreateUser, ModifyTime, ModifyUser)
                                  VALUES (@UserName, @Password, @Salt, @Valid, @CreateTime, @CreateUser, @ModifyTime, @ModifyUser)
                                  RETURNING *";

                return await connection.QuerySingleOrDefaultAsync<User>(query, user);
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"UPDATE User
                                  SET UserName = @UserName, Password = @Password, Salt = @Salt, Valid = @Valid, CreateTime = @CreateTime, CreateUser = @CreateUser, ModifyTime = @ModifyTime, ModifyUser = @ModifyUser
                                  WHERE ID = @ID
                                  RETURNING *";

                return await connection.QuerySingleOrDefaultAsync<User>(query, user);
            }
        }

        public async Task<bool> InvalidateUserAsync(long id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"UPDATE User
                                  SET Valid = false
                                  WHERE ID = @ID";

                var affectedRows = await connection.ExecuteAsync(query, new { ID = id });

                return affectedRows > 0;
            }
        }

        public async Task<User> GetUserByIdAsync(long id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM User
                                  WHERE ID = @ID";

                return await connection.QuerySingleOrDefaultAsync<User>(query, new { ID = id });
            }
        }

        public async Task<IEnumerable<User>> GetUserListAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM User";

                return await connection.QueryAsync<User>(query);
            }
        }
    }
}
