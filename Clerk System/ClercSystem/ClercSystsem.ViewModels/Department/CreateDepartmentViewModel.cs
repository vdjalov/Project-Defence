using static ClercSystem.Common.ApplicationConstants.Department.DepartmentConstants;


using System.ComponentModel.DataAnnotations;

namespace ClercSystem.ViewModels.Department
{
    public class CreateDepartmentViewModel
    {
        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(LocationMinLength)]
        [MaxLength(LocationMaxLength)]
        public string Location { get; set; } = null!;

    }
}
