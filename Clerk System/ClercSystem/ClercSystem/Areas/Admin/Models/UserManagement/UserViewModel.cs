using ClercSystem.ViewModels.Department;

namespace ClercSystem.Areas.Admin.Models.UserManagement
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? IsManager { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? DepartmentId { get; set; }
        public List<AllDepartmentsViewModel> Departments{ get; set; } = new List<AllDepartmentsViewModel>();

        public List<string> Roles { get; set; } = new List<string>();

    }
}
