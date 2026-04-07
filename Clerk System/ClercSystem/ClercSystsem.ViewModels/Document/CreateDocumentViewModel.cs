using ClercSystem.ViewModels.Category;
using ClercSystem.ViewModels.Department;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ClercSystem.Common.ApplicationConstants.Document.DocumentConstants;

namespace ClercSystem.ViewModels.Document
{
    public class CreateDocumentViewModel
    {

        [Required]
        [MinLength(TitleMinLength)]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        public string? FilePath { get; set; }

        public string CreatedOn { get; set; } = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");

        [Required]
        public string TimeToAnswer { get; set; } = DateTime.Now.AddDays(TimeToAnswerInDays).ToString();

        public bool HasBeenAnswered { get; set; } = false;

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
