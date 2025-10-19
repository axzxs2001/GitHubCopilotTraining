using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomModeDemo.Models
{
    public class BlogRepository : IBlogRepository
    {
        private readonly List<Blog> _blogs = new();

        public Task<IEnumerable<Blog>> GetAllAsync()
        {
            return Task.FromResult(_blogs.AsEnumerable());
        }

        public Task<Blog?> GetByIdAsync(Guid id)
        {
            var blog = _blogs.FirstOrDefault(b => b.Id == id);
            return Task.FromResult(blog);
        }

        public Task AddAsync(Blog blog)
        {
            if (blog.Id == Guid.Empty)
                blog.Id = Guid.NewGuid();
            _blogs.Add(blog);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Blog blog)
        {
            var existing = _blogs.FirstOrDefault(b => b.Id == blog.Id);
            if (existing != null)
            {
                existing.Title = blog.Title;
                existing.Content = blog.Content;
                existing.Author = blog.Author;
                existing.Tags = blog.Tags;
                existing.PublishedDate = blog.PublishedDate;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            var blog = _blogs.FirstOrDefault(b => b.Id == id);
            if (blog != null)
            {
                _blogs.Remove(blog);
            }
            return Task.CompletedTask;
        }
    }
}
