using ClercSystem.Data;
using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;


namespace ClercSystem.Infrastructure.Implementations
{

    public class DocumentLogsRepository : IDocumentLogsRepository
    {
        private readonly AppDbContext context;

        public DocumentLogsRepository(AppDbContext _context)
        {
            this.context = _context;
        }

        // returns all logs
        public IQueryable<DocumentLog> GetDocumentLogs()
        {
           return this.context.DocumentLogs;
        }

        // returns only logs of specific document
        public async Task<IEnumerable<DocumentLog>> GetDocumentLogsByDocumentIdAsync(Guid documentId)
        {
            IEnumerable<DocumentLog> documentLogs = this.GetDocumentLogs()
                .Where(dl => dl.DocumentId == documentId);
                

            return documentLogs;
        }
    }
}
