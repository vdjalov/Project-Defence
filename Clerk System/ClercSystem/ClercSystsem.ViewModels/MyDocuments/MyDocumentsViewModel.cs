using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.ViewModels.MyDocuments
{
    public class MyDocumentsViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string Createdby { get; set; } = null!;
        public string CategoryName { get; set; } = null!;

    }
}
