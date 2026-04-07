using ClercSystem.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Services.Interfaces
{
    public interface IDocumentService
    {
        
        Task<(IEnumerable<AllDocumentsViewModel> Docs, int TotalCount)> GetAllDocumentsAsync(string search, int page, int pageSize);
        Task<CreateDocumentViewModel> GetCreateModelAsync();
        Task<bool> CreateDocumentAsync(CreateDocumentViewModel model, Guid userId, DateTime date);
        Task<EditDocumentViewModel?> GetEditModelAsync(Guid DocumentId);
        Task<bool> EditDocumentAsync( Guid userId, EditDocumentViewModel model);
        Task<bool> CheckIfDocumentCreatorIsValid(Guid documentId, Guid userId);
        Task<bool> CheckIfDocumentExists(Guid guid);
        Task<DocumentMoreViewModel?> GetDetailsAsync(Guid id);
        Task<bool> SoftDeleteAsync(Guid userId);

    }
}
