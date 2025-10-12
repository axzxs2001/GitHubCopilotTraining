using Dapper;
using MySql.Data.MySqlClient;
using SmartAPI.Models;

namespace SmartAPI.Respositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Product>> GetProductsAsync(string culture, CancellationToken cancellationToken);
        Task<Product> GetProductAsync(int id, CancellationToken cancellationToken);

        Task<int> CreateProductAsync(Product product, CancellationToken cancellationToken);
        Task<int> UpdateProductAsync(Product product, CancellationToken cancellationToken);
        Task<int> DeleteProductAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<Scene>> GetScensAsync(CancellationToken cancellationToke);

        Task<IEnumerable<Product>> GetProductsByUserAsync(string userName, CancellationToken cancellationToken);
        Task<IEnumerable<Product>> GetProductsByUserAsync(string userName, string culture, CancellationToken cancellationToken);
    }
}
