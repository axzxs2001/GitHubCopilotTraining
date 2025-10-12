using Dapper;
using MySql.Data.MySqlClient;
using SmartAPI.Models;

namespace SmartAPI.Respositories
{
    public class ApiDataRepository : IApiDataRepository
    {
        readonly string _connectionString;
        public ApiDataRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<IEnumerable<ApiData>> GetApiDatasAsync(int productID, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    SELECT 
                        id AS Id,
                        name AS Name,
                        content AS Content,
                        category_name AS CategoryName,
                        product_id AS ProductId,
                        api_type AS ApiType,
                        serial_number AS SerialNumber,
                        validate AS Validate,
                        link_api_id as LinkAPIID,
                        result_api_id as ResultAPIID,
                        parent_id as ParentID,
                        create_time AS CreateTime,
                        create_user AS CreateUser,
                        modify_time AS ModifyTime,
                        modify_user AS ModifyUser
                    FROM api_data where product_id=@productid and validate=true order by parent_id,serial_number
                    """;
                var command = new CommandDefinition(
                  sql, new { productID }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<ApiData>(command);
            }
        }

        public async Task<IEnumerable<ApiData>> GetApiDatasAsync(int productID, int parentID, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    SELECT 
                        id AS Id,
                        name AS Name,
                        content AS Content,
                        category_name AS CategoryName,
                        product_id AS ProductId,
                        api_type AS ApiType,
                        serial_number AS SerialNumber,
                        validate AS Validate,
                        link_api_id as LinkAPIID,
                        result_api_id as ResultAPIID,
                        parent_id as ParentID,
                        create_time AS CreateTime,
                        create_user AS CreateUser,
                        modify_time AS ModifyTime,
                        modify_user AS ModifyUser
                    FROM api_data where product_id=@productid and parent_id=@parentID order by serial_number
                    """;
                var command = new CommandDefinition(
                  sql, new { productID, parentID }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<ApiData>(command);
            }
        }

        public async Task<IEnumerable<ApiData>> GetApiDatasByCategoryAsync(int productID,string[] categoryNames, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    SELECT 
                        id AS Id,
                        name AS Name,
                        content AS Content,
                        category_name AS CategoryName,
                        product_id AS ProductId,
                        api_type AS ApiType,
                        serial_number AS SerialNumber,
                        validate AS Validate,
                        parent_id as ParentID,
                        link_api_id as LinkAPIID,
                        result_api_id as ResultAPIID,
                        create_time AS CreateTime,
                        create_user AS CreateUser,
                        modify_time AS ModifyTime,
                        modify_user AS ModifyUser
                    FROM api_data where product_id=@productid and category_name not in @categoryNames order by serial_number
                    """;
                var command = new CommandDefinition(
                  sql, new { productID, categoryNames }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<ApiData>(command);
            }
        }
        public async Task<ApiData?> GetApiDataAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition("""
                    SELECT
                    id AS Id,
                        name AS Name,
                        content AS Content,
                        category_name AS CategoryName,
                        product_id AS ProductId,
                        api_type AS ApiType,
                        serial_number AS SerialNumber,
                        validate AS Validate,
                        parent_id as ParentID,
                        link_api_id as LinkAPIID,
                        result_api_id as ResultAPIID,
                        create_time AS CreateTime,
                        create_user AS CreateUser,
                        modify_time AS ModifyTime,
                        modify_user AS ModifyUser
                    FROM api_data WHERE id = @id                    
                    """, new { id }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApiData>(command);
            }
        }
        public async Task<ApiData?> GetApiDataByCategoryAsync(int productID,string apiCategoryType, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition(@"SELECT 
                    id AS Id,
    name AS Name,
    content AS Content,
    category_name AS CategoryName,
    product_id AS ProductId,
    api_type AS ApiType,
    serial_number AS SerialNumber,
    validate AS Validate,
    parent_id as ParentID,
    link_api_id as LinkAPIID,
    result_api_id as ResultAPIID,
    create_time AS CreateTime,
    create_user AS CreateUser,
    modify_time AS ModifyTime,
    modify_user AS ModifyUser
                    FROM api_data WHERE product_id = @productID and category_name=@apiCategoryType", new { productID, apiCategoryType }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApiData>(command);
            }
        }
        public async Task<ApiData> CreateApiDataAsync(ApiData apiData, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {         
                await connection.OpenAsync(cancellationToken);            
                var sql = @"INSERT INTO api_data 
                    (name, content, category_name, product_id,parent_id, api_type, serial_number, validate,link_api_id,result_api_id,create_time, create_user, modify_time, modify_user) 
                    VALUES 
                    (@Name, @Content, @CategoryName, @ProductId,@ParentID, @ApiType, @SerialNumber, @Validate, @LinkAPIID,@ResultAPIID ,@CreateTime, @CreateUser, @ModifyTime, @ModifyUser);
                    SELECT LAST_INSERT_ID();";
        
                using (var transaction = await connection.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {                  
                        var newId = await connection.ExecuteScalarAsync<int>(
                            new CommandDefinition(sql, apiData, transaction, cancellationToken: cancellationToken)
                        );             
                        await transaction.CommitAsync(cancellationToken);                     
                        apiData.Id = newId;
                        return apiData;
                    }
                    catch
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                }
            }
        }
        public async Task<int> UpdateApiDataAsync(ApiData apiData, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition("UPDATE api_data SET name = @Name, content = @Content, category_name = @CategoryName, product_id = @ProductId, api_type = @ApiType, serial_number = @SerialNumber, validate = @Validate, link_api_id=@LinkAPIID, result_api_id=@ResultAPIID, modify_time = @ModifyTime, modify_user = @ModifyUser WHERE id = @Id", apiData, cancellationToken: cancellationToken);
                return await connection.ExecuteAsync(command);
            }
        }
        public async Task<int> DeleteApiDataAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition("UPDATE api_data SET validate = false WHERE id = @Id", new { id }, cancellationToken: cancellationToken);
                return await connection.ExecuteAsync(command);
            }
        }
    }
}
