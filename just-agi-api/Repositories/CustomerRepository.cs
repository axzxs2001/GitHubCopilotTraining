using Dapper;
using just_agi_api.IRepositories;
using just_agi_api.Models;
using Npgsql;
using System.Data;

namespace just_agi_api.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"INSERT INTO Customer (Name, Sex, Age, Phone, Email, Address, Valid, EnterpriseID, RoleID, CreateTime, CreateUser, ModifyTime, ModifyUser)
                                  VALUES (@Name, @Sex, @Age, @Phone, @Email, @Address, @Valid, @EnterpriseID, @RoleID, @CreateTime, @CreateUser, @ModifyTime, @ModifyUser)
                                  RETURNING *";

                return await connection.QuerySingleOrDefaultAsync<Customer>(query, customer);
            }
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"UPDATE Customer
                                  SET Name = @Name, Sex = @Sex, Age = @Age, Phone = @Phone, Email = @Email, Address = @Address, Valid = @Valid, EnterpriseID = @EnterpriseID, RoleID = @RoleID, CreateTime = @CreateTime, CreateUser = @CreateUser, ModifyTime = @ModifyTime, ModifyUser = @ModifyUser
                                  WHERE ID = @ID
                                  RETURNING *";

                return await connection.QuerySingleOrDefaultAsync<Customer>(query, customer);
            }
        }

        public async Task<bool> InvalidateCustomerAsync(long id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"UPDATE Customer
                                  SET Valid = false
                                  WHERE ID = @ID";

                var affectedRows = await connection.ExecuteAsync(query, new { ID = id });

                return affectedRows > 0;
            }
        }

        public async Task<Customer> GetCustomerByIdAsync(long id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM Customer
                                  WHERE ID = @ID";

                return await connection.QuerySingleOrDefaultAsync<Customer>(query, new { ID = id });
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomerListAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM Customer";

                return await connection.QueryAsync<Customer>(query);
            }
        }
    }
}
