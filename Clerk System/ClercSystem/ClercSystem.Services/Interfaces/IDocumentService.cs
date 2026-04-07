using ClercSystem.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Services.Interfaces
{
    public interface IDocumentService
    {
        public interface IDocumentService
        {
            
            
            
            
            
            Task<(IEnumerable<AllDocumentsViewModel> Docs, int TotalCount)> GetAllAsync(string search, int page, int pageSize);
            //Task<CreateDocumentViewModel> GetCreateModelAsync();
            //Task CreateAsync(CreateDocumentViewModel model, Guid userId);
            //Task<EditDocumentViewModel?> GetEditModelAsync(string id, Guid userId);
            //Task<bool> EditAsync(string id, EditDocumentViewModel model, Guid userId);
            //Task<DocumentMoreViewModel?> GetDetailsAsync(string id);
            //Task<bool> SoftDeleteAsync(string id, Guid userId);
        }
    }
}
