
using static ClercSystem.Common.ApplicationConstants.Category.CategoryConstants;

using System.ComponentModel.DataAnnotations;


namespace ClercSystem.ViewModels.Category
{
    public class CreateCategoryViewModel
    {
        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string CategoryName { get; set; } = null!;

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;
    }
}
