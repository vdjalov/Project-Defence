using static ClercSystem.Common.ApplicationConstants.Department.DepartmentConstants;

using System.ComponentModel.DataAnnotations;


namespace ClercSystem.Data.Models
{
    public class Department
    {
        [Key]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public Guid DepartmentId { get; set; }

        [Required]
        []
        public string Name { get; set; } = null!;

        public ICollection<Document> Documents { get; set; } = new HashSet<Document>();

    }
}