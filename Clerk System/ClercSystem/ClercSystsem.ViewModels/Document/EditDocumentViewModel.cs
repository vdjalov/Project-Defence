using ClercSystem.ViewModels.Category;
using ClercSystem.ViewModels.Department;
using static ClercSystem.Common.ApplicationConstants.Document.DocumentConstants;

using System.ComponentModel.DataAnnotations;

namespace ClercSystem.ViewModels.Document
{
    public class EditDocumentViewModel
    {
        [Required]
        [MinLength(TitleMinLength)]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        public string? FilePath { get; set; }

        [Required]
        public string CreatedOn { get; set; } = null!;

        [Required]
        public string TimeToAnswer { get; set; } = null!;


        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        public string DepartmentId { get; set; } = null!;

        public string CategoryId { get; set; } = null!;

        public List<AllDepartmentsViewModel> Departments { get; set; } = new List<AllDepartmentsViewModel>();

        public List<AllCategoriesViewModel> Categories { get; set; } = new List<AllCategoriesViewModel>();
    }
}
