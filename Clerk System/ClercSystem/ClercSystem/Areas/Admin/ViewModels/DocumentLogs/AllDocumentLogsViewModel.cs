using ClercSystem.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClercSystem.Areas.Admin.ViewModels.DocumentLogs
{
    public class AllDocumentLogsViewModel
    {
        
        public Guid Id { get; set; }

        [Required]
        public string DocumentName { get; set; } = null!;

        public Guid DocumentId { get; set; }

        [Required]
        public int VersionNumber { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now; // initial values when firstly created 

        public DateTime AmendedOn { get; set; } = DateTime.Now; // initial values when firstly created 

        [Required]
        public string CreatedByName { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Desription { get; set; } = null!;
    }
}
