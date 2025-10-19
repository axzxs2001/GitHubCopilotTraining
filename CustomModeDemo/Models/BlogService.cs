using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomModeDemo.Models
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _repository;

        public BlogService(IBlogRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Blog>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Blog?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);
        public Task AddAsync(Blog blog) => _repository.AddAsync(blog);
        public Task UpdateAsync(Blog blog) => _repository.UpdateAsync(blog);
        public Task DeleteAsync(Guid id) => _repository.DeleteAsync(id);
    }
}
