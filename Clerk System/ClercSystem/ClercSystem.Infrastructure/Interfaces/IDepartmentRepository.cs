using ClercSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Infrastructure.Interfaces
{
    public interface IDepartmentRepository
    {
        IQueryable<Department> GetAll();
        Task<Department?> GetByIdAsync(Guid id);
        Task<Department?> GetByNameAsync(string name);

        Task AddAndSaveAsync(Department department);
        Task<bool> UpdateAndSaveAsync(Department department);
        Task<bool> DeleteAndSaveAsync(Department department);

        Task SaveChangesAsync();
        Task<bool> ExistsAsync(string departmentName, string departmentLocation);
    }
}
