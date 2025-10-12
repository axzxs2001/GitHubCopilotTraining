using Microsoft.Extensions.Localization;
using SmartAPI.Models;
using SmartAPI.Respositories;
using System.Threading;

namespace SmartAPI.Services
{
    public class ProductService : IProductService
    {
        readonly IProductRepository _productRepository;
        readonly IStringLocalizer<SharedResource> _localizer;
        public ProductService(IProductRepository productRepository, IStringLocalizer<SharedResource> localizer)
        {
            _productRepository = productRepository;
            _localizer = localizer;
        }
        public async Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToke)
        {
            return await _productRepository.GetProductsAsync(cancellationToke);
        }
        public async Task<IEnumerable<Product>> GetProductsAsync(string culture, CancellationToken cancellationToke)
        {
            return await _productRepository.GetProductsAsync(cancellationToke);
        }
        public async Task<Product> GetProductAsync(int id, CancellationToken cancellationToke)
        {
            return await _productRepository.GetProductAsync(id, cancellationToke);
        }
        public async Task<int> AddProductAsync(Product product, CancellationToken cancellationToke)
        {
            product.Validate = true;
            product.CreateTime = DateTime.Now;
            product.CreateUser = "sys";
            product.ModifyTime = DateTime.Now;
            product.ModifyUser = "sys";
            var scenes = await GetScensAsync(cancellationToke);
            var scene = scenes.SingleOrDefault(s => s.Id == product.SceneId);
            if (scene == null || scene.ParentId == -1)
            {
                throw new Exception(_localizer["scene_error"].Value);
            }
            return await _productRepository.CreateProductAsync(product, cancellationToke);
        }
        public async Task<bool> ModifyProductAsync(Product product, CancellationToken cancellationToke)
        {
            product.ModifyTime = DateTime.Now;
            var result = await _productRepository.UpdateProductAsync(product, cancellationToke);
            return result > 0;
        }
        public async Task<bool> RemoveProductAsync(int id, CancellationToken cancellationToke)
        {
            var result = await _productRepository.DeleteProductAsync(id, cancellationToke);
            return result > 0;
        }
        public async Task<IEnumerable<Scene>> GetScensAsync(CancellationToken cancellationToke)
        {
            return await _productRepository.GetScensAsync(cancellationToke);
        }

        public async Task<IEnumerable<Product>> GetProductsByUserAsync(string userName, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductsByUserAsync(userName, cancellationToken);
        }
        public async Task<IEnumerable<Product>> GetProductsByUserAsync(string userName, string culture, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductsByUserAsync(userName, culture, cancellationToken);
        }

    }
}
