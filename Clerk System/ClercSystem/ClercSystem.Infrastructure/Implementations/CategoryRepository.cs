using ClercSystem.Data;
using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Infrastructure.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext context;

        public CategoryRepository(AppDbContext _context)
        {
            this.context = _context;
        }

        public async Task AddAndSaveAsync(Category category)
        {
            await this.context.Categories.AddAsync(category);
            await this.context.SaveChangesAsync();
        }

        public Task<bool> DeleteAndSaveAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await this.context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await this.context.Categories.FindAsync(id);
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await this.context.Categories
               .FirstOrDefaultAsync(d => d.CategoryName == name);
        }

        public async Task SaveChangesAsync()
        {
            await this.context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAndSaveAsync(Category category)
        {
            Category? existingCategory = await this.GetByIdAsync(category.Id);

            if (existingCategory == null)
            {
                return false;
            }

            this.context.Categories.Update(category);
            await this.SaveChangesAsync();

            return true;
        }
    }
}
