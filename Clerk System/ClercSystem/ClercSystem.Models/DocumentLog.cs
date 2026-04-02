using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClercSystem.Data.Models
{
    public class DocumentLog
    {
            public int Id { get; set; }

            public Guid DocumentId { get; set; }

            [ForeignKey(nameof(DocumentId))]
            public Document Document { get; set; } = null!;

            public int VersionNumber { get; set; }

            public DateTime CreatedAt { get; set; }
        
            public Guid CreatedById { get; set; }

            [ForeignKey(nameof(CreatedById))]
            public ApplicationUser CreatedBy { get; set; } = null!;

            


    }
}
