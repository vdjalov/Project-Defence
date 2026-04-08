using ClercSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Infrastructure.Interfaces
{
    public interface IDocumentUserRepository
    {

        Task AddAndSaveAsync(DocumentUser documentUser);
        IQueryable<DocumentUser> GetAll();
        Task<DocumentUser?> GetByIdAsync(Guid userId, Guid documentId);
        Task<bool> UpdateAndSaveAsync(DocumentUser documentUser);
        Task<bool> DeleteAndSaveAsync(DocumentUser documentUser);
        Task SaveChangesAsync();


    }
}
