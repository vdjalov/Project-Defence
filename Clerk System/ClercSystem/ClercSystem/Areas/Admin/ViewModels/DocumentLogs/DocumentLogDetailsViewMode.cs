using System.ComponentModel.DataAnnotations;

namespace ClercSystem.Areas.Admin.ViewModels.DocumentLogs
{
    public class DocumentLogDetailsViewMode
    {


        [Required]
        public string DocumentName { get; set; } = null!;

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
