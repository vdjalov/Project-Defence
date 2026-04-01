using static ClercSystem.Common.ApplicationConstants.Document.DocumentConstants;

using System.ComponentModel.DataAnnotations;


namespace ClercSystem.Data.Models
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(TitleMinLength)]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;
        public string ?FilePath { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int TimeToAnswer { get; set; } = DateTime.Now.AddDays(5).Day;
        public Guid DepartmentId  { get; set; }
        public Department Department { get; set; } = null!;
        public ApplicationUser CreatedBy { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

    }
}
