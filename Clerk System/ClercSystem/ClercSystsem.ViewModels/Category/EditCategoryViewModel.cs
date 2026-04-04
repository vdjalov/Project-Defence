using System.ComponentModel.DataAnnotations;
using static ClercSystem.Common.ApplicationConstants.Category.CategoryConstants;

namespace ClercSystem.ViewModels.Category
{
    public  class EditCategoryViewModel
    {
        public Guid Id { get; set; }

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
