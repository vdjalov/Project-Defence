using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Department;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {

        private readonly IDepartmentRepository departmentRepository;

        public DepartmentService(IDepartmentRepository _departmentRepository)
        {
            this.departmentRepository = _departmentRepository;
        }

        public async Task<bool> CheckIfThereAreDocumentsAssociatedWithDepartmentAsync(string id)
        {
            Department? department = await this.departmentRepository.GetByIdAsync(Guid.Parse(id));

            if(department != null && department.Documents.Count > 0)
            {
                return true;
            }

            return false;
        }

        // create new department
        public async Task<bool> CreateDepartmentAsync(CreateDepartmentViewModel model)
        {
            Data.Models.Department department = new Data.Models.Department
            {
                Name = model.Name,
                Location = model.Location
            };
           
            await this.departmentRepository.AddAndSaveAsync(department);

            return true;
        }

        // delete department by id
        public async Task<bool> DeleteDepartmentAsync(string id)
        {
            Department? department = await this.departmentRepository.GetByIdAsync(Guid.Parse(id));

            if(department == null)
            {
                return false;
            }

            bool isDeleted = await departmentRepository.DeleteAndSaveAsync(department);

            return isDeleted;
        }

        // checking if department already exists by name and location
        public async Task<bool> DepartmentExistsAsync(string departmentName, string departmentLocation)
        {
            return  await this.departmentRepository.ExistsAsync(departmentName, departmentLocation);
        }

        // checking if department exists by id
        public async Task<bool> DepartmentExistsByIdAsync(Guid departmentId)
        {
            Department? department = await this.departmentRepository.GetByIdAsync(departmentId);
            if(department == null)
            {
                return false;
            }

            return true;
        }

        // edit department by id
        public async Task<bool> EditDepartmentAsync(EditDepartmentViewModel model)
        {
            if (model == null)
            {
                return false;
            }

            Department? department = await this.departmentRepository.GetByIdAsync(model.DepartmentId);

            if(department == null)
            {
                return false;
            }

            department.Name = model.Name;
            department.Location = model.Location;

            return await this.departmentRepository.UpdateAndSaveAsync(department);
        }

        // list all departments
        public async Task<List<AllDepartmentsViewModel>> GetAllDepartmentsAsync()
        {
            IQueryable<Department> departments = this.departmentRepository.GetAll();
            List<AllDepartmentsViewModel> departmentsViewModel  = await departments
              .Select(d => new AllDepartmentsViewModel
              {
                  DepartmentId = d.DepartmentId,
                  Name = d.Name,
                  Location = d.Location,
              }).ToListAsync();

            return departmentsViewModel;

        }

        // return department view model by id
        public CreateDepartmentViewModel GetCreateModel()
        {
            return new CreateDepartmentViewModel();
        }

        // return department details by id
        public async Task<DepartmentMoreViewModel?> GetDepartmentDetailsAsync(string id)
        {
            Department? department = await this.departmentRepository.GetByIdAsync(Guid.Parse(id));

            if(department == null)
            {
                return null;
            }

            DepartmentMoreViewModel? departmentDetails = new DepartmentMoreViewModel()
            {
                Name = department.Name,
                Location = department.Location
            };
            
            return departmentDetails;
        }


        // return department edit model by id
        public async Task<EditDepartmentViewModel> GetEditModelAsync(Guid departmentId)
        {
            Department? department = await this.departmentRepository.GetByIdAsync(departmentId);

            if(department == null)
            {
                return null;
            }

            EditDepartmentViewModel viewModel = new EditDepartmentViewModel
            {
                DepartmentId = department.DepartmentId,
                Name = department.Name,
                Location = department.Location
            };

            return viewModel;
            
        }
    }
}
