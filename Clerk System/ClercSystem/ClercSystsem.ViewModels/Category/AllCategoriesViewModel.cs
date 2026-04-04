
using System.ComponentModel.DataAnnotations;
using static ClercSystem.Common.ApplicationConstants.Category.CategoryConstants;

namespace ClercSystem.ViewModels.Category
{
    public class AllCategoriesViewModel
    {

        public Guid Id { get; set; }

        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string CategoryName { get; set; } = null!;
    }
}
