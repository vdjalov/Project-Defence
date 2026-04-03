using static ClercSystem.Common.ApplicationConstants.Department.DepartmentConstants;

using System.ComponentModel.DataAnnotations;


namespace ClercSystem.Data.Models
{
    public class Department
    {
        [Key]
        public Guid DepartmentId { get; set; }

        public Department()
        {
            this.DepartmentId = Guid.NewGuid();
        }

        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        public string Location { get; set; } = null!;

        public ICollection<ApplicationUser> Users = new HashSet<ApplicationUser>();

        public ICollection<Document> Documents { get; set; } = new HashSet<Document>();

    }
}