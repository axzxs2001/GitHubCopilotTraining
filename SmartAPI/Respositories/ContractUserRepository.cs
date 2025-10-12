using Dapper;
using MySql.Data.MySqlClient;
using SmartAPI.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAPI.Respositories
{
    public class ContractUserRepository : IContractUserRepository
    {
        readonly string _connectionString;

        public ContractUserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<ContractUser>> GetContractUsersAsync(CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    SELECT 
                        id AS Id,
                        user AS User,
                        contract_id AS ContractId,
                        create_time AS CreateTime
                    FROM contract_user
                    ORDER BY id DESC
                    """;

                var command = new CommandDefinition(
                    sql, cancellationToken: cancellationToken);

                return await connection.QueryAsync<ContractUser>(command);
            }
        }

        public async Task<ContractUser> GetContractUserAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    SELECT 
                        id AS Id,
                        user AS User,
                        contract_id AS ContractId,
                        create_time AS CreateTime
                    FROM contract_user
                    WHERE id = @id
                    """;

                var command = new CommandDefinition(
                    sql, new { id }, cancellationToken: cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<ContractUser>(command);
            }
        }

        public async Task<int> CreateContractUserAsync(ContractUser contractUser, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    INSERT INTO contract_user
                    (
                        user, 
                        contract_id, 
                        create_time
                    )
                    VALUES
                    (
                        @User,
                        @ContractId,
                        @CreateTime
                    );
                    SELECT LAST_INSERT_ID();
                    """;

                var id = await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(sql, contractUser, cancellationToken: cancellationToken));

                return id;
            }
        }

        public async Task<int> UpdateContractUserAsync(ContractUser contractUser, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    UPDATE contract_user
                    SET 
                        user = @User,
                        contract_id = @ContractId,
                        create_time = @CreateTime
                    WHERE id = @Id
                    """;

                return await connection.ExecuteAsync(
                    new CommandDefinition(sql, contractUser, cancellationToken: cancellationToken));
            }
        }

        public async Task<int> DeleteContractUserAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "DELETE FROM contract_user WHERE id = @id";

                return await connection.ExecuteAsync(
                    new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
            }
        }
        //生成一个方法：按用户ID查询是否签订了最新的合同
        /// <summary>
        /// Checks if a user has signed the latest contract
        /// </summary>
        /// <param name="user">The user ID or username</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The contract if the user has signed the latest contract, null otherwise</returns>
        public async Task<Contract> HasUserSignedLatestContractAsync(string user, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                // First get the latest contract
                var latestContractSql = """
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
            ORDER BY create_time DESC, id DESC
            LIMIT 1
            """;

                var latestContract = await connection.QuerySingleOrDefaultAsync<Contract>(
                    new CommandDefinition(latestContractSql, cancellationToken: cancellationToken));

                if (latestContract == null)
                {
                    return null; // No contracts exist yet
                }

                // Then check if the user has signed this contract
                var userSignatureSql = """
            SELECT COUNT(1) 
            FROM contract_user 
            WHERE user = @user AND contract_id = @contractId
            """;

                var hasSigned = await connection.ExecuteScalarAsync<int>(
                    new CommandDefinition(userSignatureSql,
                    new { user, contractId = latestContract.Id },
                    cancellationToken: cancellationToken));

                // Return the contract if signed, null otherwise
                return hasSigned == 0 ? latestContract : null;
            }
        }


    }
}