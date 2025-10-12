using SmartAPI.Models;
using SmartAPI.Respositories;

namespace SmartAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToke);
        Task<Product> GetProductAsync(int id, CancellationToken cancellationToke);
        Task<IEnumerable<Product>> GetProductsAsync(string culture, CancellationToken cancellationToken);
        Task<int> AddProductAsync(Product product, CancellationToken cancellationToke);
        Task<bool> ModifyProductAsync(Product product, CancellationToken cancellationToke);
        Task<bool> RemoveProductAsync(int id, CancellationToken cancellationToke);

        Task<IEnumerable<Scene>> GetScensAsync(CancellationToken cancellationToke);

        Task<IEnumerable<Product>> GetProductsByUserAsync(string userName, CancellationToken cancellationToken);
        Task<IEnumerable<Product>> GetProductsByUserAsync(string userName, string culture, CancellationToken cancellationToken);
    }
}
