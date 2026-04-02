using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClercSystem.Data.Models
{
    [PrimaryKey(nameof(DocumentId) ,nameof(UserId))]
    public class DocumentUser
    {
        [Required]
        public Guid DocumentId { get; set; }

        public Document Document { get; set; } = null!;

        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public PermissionType Permission { get; set; }
    }
}
