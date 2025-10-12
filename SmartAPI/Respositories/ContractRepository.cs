using Dapper;
using MySql.Data.MySqlClient;
using SmartAPI.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAPI.Respositories
{
    public class ContractRepository : IContractRepository
    {
        readonly string _connectionString;
        
        public ContractRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        
        public async Task<IEnumerable<Contract>> GetContractsAsync(CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    SELECT 
                        id AS Id,
                        title AS Title,
                        content AS Content,
                        version AS Version,
                        create_time AS CreateTime,
                        modify_time AS ModifyTime,
                        create_user AS CreateUser,
                        modify_user AS ModifyUser
                    FROM contracts
                    ORDER BY id DESC
                    """;
                    
                var command = new CommandDefinition(
                    sql, cancellationToken: cancellationToken);
                    
                return await connection.QueryAsync<Contract>(command);
            }
        }
        
        public async Task<Contract> GetContractAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    SELECT 
                        id AS Id,
                        title AS Title,
                        content AS Content,
                        version AS Version,
                        create_time AS CreateTime,
                        modify_time AS ModifyTime,
                        create_user AS CreateUser,
                        modify_user AS ModifyUser
                    FROM contracts
                    WHERE id = @id
                    """;
                    
                var command = new CommandDefinition(
                    sql, new { id }, cancellationToken: cancellationToken);
                    
                return await connection.QuerySingleOrDefaultAsync<Contract>(command);
            }
        }
        
        public async Task<int> CreateContractAsync(Contract contract, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    INSERT INTO contracts
                    (
                        title, 
                        content, 
                        version, 
                        create_time, 
                        modify_time,
                        create_user,
                        modify_user
                    )
                    VALUES
                    (
                        @Title,
                        @Content,
                        @Version,
                        @CreateTime,
                        @ModifyTime,
                        @CreateUser,
                        @ModifyUser
                    );
                    SELECT LAST_INSERT_ID();
                    """;
                    
                var id = await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(sql, contract, cancellationToken: cancellationToken));
                    
                return id;
            }
        }
        
        public async Task<int> UpdateContractAsync(Contract contract, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    UPDATE contracts
                    SET 
                        title = @Title,
                        content = @Content,
                        version = @Version,
                        modify_time = @ModifyTime,
                        modify_user = @ModifyUser
                    WHERE id = @Id
                    """;
                    
                return await connection.ExecuteAsync(
                    new CommandDefinition(sql, contract, cancellationToken: cancellationToken));
            }
        }
        
        public async Task<int> DeleteContractAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "DELETE FROM contracts WHERE id = @id";
                
                return await connection.ExecuteAsync(
                    new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
            }
        }        
    }
}