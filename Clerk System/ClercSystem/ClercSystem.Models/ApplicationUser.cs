
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClercSystem.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public bool IsManager { get; set; } = false;

        public Guid DepartmentId { get; set; }

        // shows whitch department the employee belongs to 
        [Required]
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;

        // Navigation properties for the user documents 
        public ICollection<DocumentUser> DocumentsUsers { get; set; } = new HashSet<DocumentUser>();

    }
}