using Dapper;
using MySql.Data.MySqlClient;
using SmartAPI.Models;

namespace SmartAPI.Respositories
{
    public class CustomerInfoRepository : ICustomerInfoRepository
    {
        readonly string _connectionString;
        public CustomerInfoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<CustomerInfo>> GetCustomerInfosAsync(string companyName, string responsiblePerson, int pageSize, int pageNumber, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = $"""
                    SELECT 
                        id AS Id,
                        company_name AS CompanyName,
                        responsible_person AS ResponsiblePerson,
                        responsible_email AS ResponsibleEmail,
                        salesperson_email AS SalespersonEmail,
                        developer_email AS DeveloperEmail,
                        nss_salesperson_email AS NssSalespersonEmail,
                        scene AS Scene,
                        ip_whitelist AS IpWhitelist,
                        wallet AS Wallet,
                        is_sub_pay AS IsSubPay,
                        is_use_datatransfer AS IsUseDataTransfer,
                        is_use_sms AS IsUseSms,
                        is_use_email AS IsUseEmail,
                        is_wallet_logo AS IsWalletLogo,
                        create_time AS CreateTime,
                        is_user_active as IsUserActive
                    FROM customer_info
                    Where 1=1
                    {(string.IsNullOrWhiteSpace(companyName) ? "" : " and company_name like @companyName")}
                    {(string.IsNullOrWhiteSpace(responsiblePerson) ? "" : " and responsible_person like @responsiblePerson")}
                     ORDER BY create_time desc
                    LIMIT @PageSize OFFSET @PageNumber;
                    """;
                var command = new CommandDefinition(sql, parameters: new { companyName = $"%{companyName}%", responsiblePerson = $"%{responsiblePerson}%", pageSize, pageNumber = (pageNumber - 1) * pageSize }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<CustomerInfo>(command);
            }
        }

        public async Task<int> GetCustomerInfoCountAsync(string companyName, string responsiblePerson, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = $"""
                    SELECT count(1)  FROM customer_info
                    Where 1=1
                    {(string.IsNullOrWhiteSpace(companyName) ? "" : " and company_name like @companyName")}
                    {(string.IsNullOrWhiteSpace(responsiblePerson) ? "" : " and responsible_person like @responsiblePerson")}
                    """;
                var command = new CommandDefinition(sql, parameters: new { companyName = $"%{companyName}%", responsiblePerson = $"%{responsiblePerson}%" }, cancellationToken: cancellationToken);
                return await connection.ExecuteScalarAsync<int>(command);
            }
        }

        public async Task<CustomerInfo?> GetCustomerInfoAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition(@"SELECT  id AS Id,
    company_name AS CompanyName,
    responsible_person AS ResponsiblePerson,
    responsible_email AS ResponsibleEmail,
    salesperson_email AS SalespersonEmail,
    developer_email AS DeveloperEmail,
    nss_salesperson_email AS NssSalespersonEmail,
    scene AS Scene,
    ip_whitelist AS IpWhitelist,
    wallet AS Wallet,
    is_sub_pay AS IsSubPay,
    is_use_datatransfer AS IsUseDataTransfer,
    is_use_sms AS IsUseSms,
    is_use_email AS IsUseEmail,
    is_wallet_logo AS IsWalletLogo,
    create_time AS CreateTime,
    is_user_active as IsUserActive
FROM customer_info WHERE id = @id", new { id }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<CustomerInfo>(command);
            }
        }

        public async Task<bool> CreateCustomerInfoAsync(CustomerInfo customerInfo, CancellationToken cancellationToken)
        {

            using (var connection = new MySqlConnection(_connectionString))
            {

                var command = new CommandDefinition("INSERT INTO customer_info (company_name, responsible_person, responsible_email, salesperson_email, developer_email, nss_salesperson_email, scene, ip_whitelist, wallet, is_sub_pay, is_use_datatransfer, is_use_sms, is_use_email, is_wallet_logo) VALUES (@CompanyName, @ResponsiblePerson, @ResponsibleEmail, @SalespersonEmail, @DeveloperEmail, @NssSalespersonEmail, @Scene, @IpWhitelist, @Wallet, @IsSubPay, @IsUseDataTransfer, @IsUseSms, @IsUseEmail, @IsWalletLogo)", customerInfo, cancellationToken: cancellationToken);
                return (await connection.ExecuteAsync(command)) > 0;
            }
        }

        public async Task<bool> UpdateCustomerInfoAsync(CustomerInfo customerInfo, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition("UPDATE customer_info SET company_name = @CompanyName, responsible_person = @ResponsiblePerson, responsible_email = @ResponsibleEmail, salesperson_email = @SalespersonEmail, developer_email = @DeveloperEmail, nss_salesperson_email = @NssSalespersonEmail, scene = @Scene, ip_whitelist = @IpWhitelist, wallet = @Wallet, is_sub_pay = @IsSubPay, is_use_datatransfer = @IsUseDataTransfer, is_use_sms = @IsUseSms, is_use_email = @IsUseEmail, is_wallet_logo = @IsWalletLogo WHERE id = @Id", customerInfo, cancellationToken: cancellationToken);
                return (await connection.ExecuteAsync(command)) > 0;
            }
        }

        public async Task<bool> ActiveUserAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition("UPDATE customer_info SET  is_user_active=true WHERE id = @Id", parameters: new { id }, cancellationToken: cancellationToken);
                return (await connection.ExecuteAsync(command)) > 0;
            }
        }

        public async Task<bool> DeleteCustomerInfoAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition("DELETE FROM customer_info WHERE id = @Id", new { id }, cancellationToken: cancellationToken);
                return (await connection.ExecuteAsync(command)) > 0;
            }
        }

        public async Task<int> GetCountAsync(string companyName, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition("SELECT count(1) from customer_info WHERE company_name= @CompanyName", new { CompanyName = companyName }, cancellationToken: cancellationToken);
                return await connection.ExecuteScalarAsync<int>(command);
            }
        }
    }
}

