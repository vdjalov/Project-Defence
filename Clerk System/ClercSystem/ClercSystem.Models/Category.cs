using static ClercSystem.Common.ApplicationConstants.Category.CategoryConstants;

using System.ComponentModel.DataAnnotations;

namespace ClercSystem.Data.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string CategoryName { get; set; } = null!;

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
