using static ClercSystem.Common.ApplicationConstants.Department.DepartmentConstants;

using System.ComponentModel.DataAnnotations;


namespace ClercSystem.Data.Models
{
    public class Department
    {
        [Key]
        public Guid DepartmentId { get; set; }

        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public ICollection<Document> Documents { get; set; } = new HashSet<Document>();

    }
}