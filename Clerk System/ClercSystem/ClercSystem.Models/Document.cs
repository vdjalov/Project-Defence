using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ClercSystem.Common.ApplicationConstants.Document.DocumentConstants;
using static ClercSystem.Data.Models.DocumentLog;


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

        public string? FilePath { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public int TimeToAnswer { get; set; } = DateTime.Now.AddDays(TimeToAnswerInDays).Day;

        public bool HasBeenAnswered { get; set; } = false;

        public Guid DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;

        public ApplicationUser CreatedBy { get; set; } = null!;

        public Guid CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        // navigational property for document user
        public ICollection<DocumentUser> DocumentsUsers { get; set; } = new List<DocumentUser>();
        public ICollection<DocumentLog> DocumentsLogs { get; set; } = new List<DocumentLog>();
    }
        
}
