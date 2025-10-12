using Dapper;
using MySql.Data.MySqlClient;
using SmartAPI.Models;

namespace SmartAPI.Respositories
{
    public class ProductRepository : IProductRepository
    {
        readonly string _connectionString;
        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                     SELECT 
                        id AS Id,
                        name AS Name,
                        `describe` AS `Describe`,
                        image as Image,
                        version as Version,
                        culture as Culture,
                        scene_id as SceneId,
                        create_time AS CreateTime,
                        create_user AS CreateUser,
                        validate AS Validate,
                        modify_time AS ModifyTime,
                        modify_user AS ModifyUser
                    FROM 
                        product where validate=true order by name,version
                    """;
                var command = new CommandDefinition(sql, cancellationToken);
                return await connection.QueryAsync<Product>(command);
            }
        }
        public async Task<IEnumerable<Product>> GetProductsAsync(string culture, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                     SELECT 
                        id AS Id,
                        name AS Name,
                        `describe` AS `Describe`,
                        image as Image,
                        version as Version,
                        culture as Culture,
                        scene_id as SceneId,
                        create_time AS CreateTime,
                        create_user AS CreateUser,
                        validate AS Validate,
                        modify_time AS ModifyTime,
                        modify_user AS ModifyUser
                    FROM 
                        product where validate=true order by name,version
                    """;
                var command = new CommandDefinition(sql, parameters: new { culture }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<Product>(command);
            }
        }
        public async Task<IEnumerable<Product>> GetProductsByUserAsync(string userName, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                     SELECT DISTINCT
                        product.id AS Id,
                        product.name AS Name,
                        product.`describe` AS `Describe`,
                        product.image as Image,
                        product.version as Version,
                        product.culture as Culture,
                        product.scene_id as SceneId,
                        product.create_time AS CreateTime,
                        product.create_user AS CreateUser,
                        product.validate AS Validate,
                        product.modify_time AS ModifyTime,
                        product.modify_user AS ModifyUser
                    FROM 
                    customer_info 
                    join product on JSON_CONTAINS(customer_info.scene,CAST(product.scene_id as JSON),'$')
                    WHERE JSON_CONTAINS(customer_info.developer_email, @username) 
                    or JSON_CONTAINS(customer_info.salesperson_email, @username) 
                    """;
                var command = new CommandDefinition(sql, parameters: new { username = $"\"{userName}\"" }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<Product>(command);
            }
        }
        public async Task<IEnumerable<Product>> GetProductsByUserAsync(string userName, string culture, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                     SELECT DISTINCT
                        product.id AS Id,
                        product.name AS Name,
                        product.`describe` AS `Describe`,
                        product.image as Image,
                        product.version as Version,
                        product.culture as Culture,
                        product.scene_id as SceneId,
                        product.create_time AS CreateTime,
                        product.create_user AS CreateUser,
                        product.validate AS Validate,
                        product.modify_time AS ModifyTime,
                        product.modify_user AS ModifyUser
                    FROM 
                    customer_info 
                    join product on JSON_CONTAINS(customer_info.scene,CAST(product.scene_id as JSON),'$')
                    WHERE product.culture=@culture  and (JSON_CONTAINS(customer_info.developer_email, @username) 
                    or JSON_CONTAINS(customer_info.salesperson_email, @username))
                    """;
                var command = new CommandDefinition(sql, parameters: new { username = $"\"{userName}\"",culture }, cancellationToken: cancellationToken);
                return await connection.QueryAsync<Product>(command);
            }
        }
        public async Task<Product?> GetProductAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                     SELECT 
                        id AS Id,
                        name AS Name,
                        `describe` AS `Describe`,
                        image as Image,
                        version as Version,
                        culture as Culture,
                        scene_id as SceneId,
                        create_time AS CreateTime,
                        create_user AS CreateUser,
                        validate AS Validate,
                        modify_time AS ModifyTime,
                        modify_user AS ModifyUser
                    FROM 
                        product where id=@id
                    """;
                var command = new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken);
                return await connection.QueryFirstOrDefaultAsync<Product>(command);
            }
        }
        public async Task<int> CreateProductAsync(Product product, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var sql = "INSERT INTO product (name, `describe`,image, version,culture,scene_id,create_time, create_user, validate, modify_time, modify_user) VALUES (@Name, @Describe,@Image,@Version,@Culture,@SceneId, @CreateTime, @CreateUser, @Validate, @ModifyTime, @ModifyUser);SELECT LAST_INSERT_ID(); ";

                using (var transaction = await connection.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {
                        var newId = await connection.ExecuteScalarAsync<int>(
                            new CommandDefinition(sql, product, transaction, cancellationToken: cancellationToken)
                        );
                        await transaction.CommitAsync(cancellationToken);
                        return newId;
                    }
                    catch
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                }
            }
        }
        public async Task<int> UpdateProductAsync(Product product, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition("UPDATE product SET name = @Name,image=@Image,version=@Version, `describe` = @Describe, validate = @Validate,culture=@Culture,scene_id=@SceneId, modify_time = @ModifyTime, modify_user = @ModifyUser WHERE id = @Id", product, cancellationToken: cancellationToken);
                return await connection.ExecuteAsync(command);
            }
        }
        public async Task<int> DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition("DELETE FROM product WHERE id = @id", new { id }, cancellationToken: cancellationToken);
                return await connection.ExecuteAsync(command);
            }
        }

        public async Task<IEnumerable<Scene>> GetScensAsync(CancellationToken cancellationToke)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                     SELECT 
                        id AS Id,
                        name AS Name,
                        `describe` AS `Describe`,
                        parent_id as ParentID
                    FROM 
                        scene
                    """;
                var command = new CommandDefinition(sql, cancellationToke);
                return await connection.QueryAsync<Scene>(command);
            }
        }
    }
}
