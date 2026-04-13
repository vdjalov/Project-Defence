using static ClercSystem.Common.ApplicationConstants.DocumentLogs.DocumentLogsConstants;

using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClercSystem.Data.Models
{
    public class DocumentLog
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public Document Document { get; set; } = null!;

        [Required]
        public int VersionNumber { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now; // initial values when firstly created 

        public DateTime AmendedOn { get; set; } = DateTime.Now; // initial values when firstly created 

        public Guid CreatedById { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public ApplicationUser CreatedBy { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength =DescriptionMinLength)]
        public string Desription { get; set; } = null!;


    }
}
