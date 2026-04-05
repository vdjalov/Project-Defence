using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClercSystem.ViewModels.Document
{
    public class DocumentMoreViewModel
    {
        public string Title { get; set; } = null!;

        public string? FilePath { get; set; }

        public DateTime CreatedOn { get; set; }

        public string TimeToAnswer { get; set; }

        public bool HasBeenAnswered { get; set; } 

        public string Description { get; set; } 

        public string DepartmentName { get; set; }

        public string CreatedBy { get; set; }

        public string CategoryName { get; set; }
        
    }
}
