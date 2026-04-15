using ClercSystem.Data;
using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;


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
           return this.context.DocumentLogs
                            .Include(dl => dl.Document)
                            .Include(dl => dl.CreatedBy);
                    }

        // returns only logs of specific document
        public async Task<IEnumerable<DocumentLog>> GetDocumentLogsByDocumentIdAsync(Guid documentId)
        {
            IEnumerable<DocumentLog> documentLogs = this.GetDocumentLogs()
                .Where(dl => dl.DocumentId == documentId);
                

            return documentLogs;
        }

        public async Task<bool> AddAndSaveAsync(DocumentLog documentLog)
        {
            DocumentLog log = await this.context.DocumentLogs.FirstOrDefaultAsync(dl => dl.Id == documentLog.Id);

            if (log != null)
            {
                return false;
            }

            await this.context.DocumentLogs.AddAsync(documentLog);
            await this.context.SaveChangesAsync();

            return true;
        }
    }
}
