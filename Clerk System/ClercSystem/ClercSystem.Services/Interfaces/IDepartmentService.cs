using ClercSystem.Infrastructure.Interfaces;
using ClercSystem.ViewModels.Department;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Services.Interfaces
{
    public interface IDepartmentService
    {

        public Task<List<AllDepartmentsViewModel>> GetAllDepartmentsAsync();
        CreateDepartmentViewModel GetCreateModel();
        Task CreateDepartmentAsync(CreateDepartmentViewModel model);
        Task<bool> DepartmentExistsAsync(string departmentName, string departmentLocation);
        Task <bool> DepartmentExistsByIdAsync(Guid departmentId);
        Task<EditDepartmentViewModel> GetEditModelAsync(Guid departmentId);
        Task<bool> EditDepartmentAsync(EditDepartmentViewModel model);

        //Task<DepartmentMoreViewModel?> GetDepartmentDetailsAsync(Guid id);
        //Task<(bool Success, string ErrorMessage)> DeleteDepartmentAsync(Guid id);

    }
}
