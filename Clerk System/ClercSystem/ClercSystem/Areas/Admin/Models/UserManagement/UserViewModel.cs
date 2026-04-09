using ClercSystem.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace ClercSystem.Areas.Admin.Models.UserManagement
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? IsManager { get; set; }

        public string Email { get; set; } = string.Empty;

        public List<Department> Departments{ get; set; } = new List<Department>();

        public List<string> Roles { get; set; } = new List<string>();

    }
}
