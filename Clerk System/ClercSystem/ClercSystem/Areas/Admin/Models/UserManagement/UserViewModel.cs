using System.ComponentModel.DataAnnotations;

namespace ClercSystem.Areas.Admin.Models.UserManagement
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public bool IsManager { get; set; } = false;

        public string Email { get; set; } = string.Empty;

        public Guid? DepartmentId { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

    }
}
