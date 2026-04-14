using ClercSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Infrastructure.Interfaces
{
    public interface IDocumentLogsRepository
    {
       
        public IQueryable<DocumentLog> GetDocumentLogs();
        public Task<IEnumerable<DocumentLog>> GetDocumentLogsByDocumentIdAsync(Guid documentId);
        public Task<bool> AddAndSaveAsync(DocumentLog document);

    }
}
