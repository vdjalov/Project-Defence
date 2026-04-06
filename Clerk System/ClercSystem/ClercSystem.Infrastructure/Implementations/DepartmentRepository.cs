using ClercSystem.Data;
using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Infrastructure.Implementations
{
    public class DepartmentRepository : IDepartmentRepository
    {

        private readonly AppDbContext context;

        public DepartmentRepository(AppDbContext _context)
        {
            this.context = _context;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await this.context.Departments.ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(Guid id)
        {
            return await this.context.Departments.FindAsync(id);
        }

        public async Task<Department?> GetByNameAsync(string name)
        {
            return await this.context.Departments
                .FirstOrDefaultAsync(d => d.Name == name);
        }

        public async Task AddAndSaveAsync(Department department)
        {
            await this.context.Departments.AddAsync(department);
            await this.context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAndSaveAsync(Department department)
        {
            Department? existingDepartment = this.GetByIdAsync(department.DepartmentId).Result;

            if (existingDepartment == null)
            {
                return false;
            }

            this.context.Departments.Update(department);
            await this.SaveChangesAsync();

            return true; 
        }

        public async Task<bool> DeleteAndSaveAsync(Department department)
        {
            Department? existingDepartment = this.GetByIdAsync(department.DepartmentId).Result;
            if(existingDepartment == null)
            {
                return false;
            }

            this.context.Departments.Remove(department);
            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await this.context.SaveChangesAsync();
        }

     
        
    }
}
