using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClercSystem.ViewModels.Document
{
    public class AllDocumentsViewModel
    {

        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string Createdby { get; set; } = null!;
        public string CategoryName { get; set; } = null!;






    }
}
