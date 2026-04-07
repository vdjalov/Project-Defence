using ClercSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Infrastructure.Interfaces
{
    public interface IDocumentRepository
    {
        Task<List<Document>> GetAllAsync();
        Task<Document?> GetByIdAsync(Guid id);
        Task<Document?> GetByTitleAsync(string name);
        Task AddAndSaveAsync(Document department);
        Task<bool> UpdateAndSaveAsync(Document document);
        Task<bool> DeleteAndSaveAsync(Document document);
        Task SaveChangesAsync();
    }
}
