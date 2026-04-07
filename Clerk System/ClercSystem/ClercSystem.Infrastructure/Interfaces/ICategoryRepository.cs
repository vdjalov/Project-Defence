using ClercSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Infrastructure.Interfaces
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetAll();
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category?> GetByNameAsync(string name);

        Task AddAndSaveAsync(Category category);
        Task<bool> UpdateAndSaveAsync(Category category);
        Task<bool> DeleteAndSaveAsync(Category category);

        Task SaveChangesAsync();
        
    }
}
