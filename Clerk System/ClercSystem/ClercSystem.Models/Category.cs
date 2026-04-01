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
        public string Name { get; set; } = null!;

        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}