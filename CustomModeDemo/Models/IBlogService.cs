using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomModeDemo.Models
{
    public interface IBlogService
    {
        Task<IEnumerable<Blog>> GetAllAsync();
        Task<Blog?> GetByIdAsync(Guid id);
        Task AddAsync(Blog blog);
        Task UpdateAsync(Blog blog);
        Task DeleteAsync(Guid id);
    }
}
