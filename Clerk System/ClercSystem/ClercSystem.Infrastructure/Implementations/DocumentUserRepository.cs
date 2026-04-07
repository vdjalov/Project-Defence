using ClercSystem.Data;
using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;

namespace ClercSystem.Infrastructure.Implementations
{
    public class DocumentUserRepository : IDocumentUserRepository
    {
        private readonly AppDbContext context;

        public DocumentUserRepository(AppDbContext _context)
        {
            this.context = _context;
        }

        public async Task AddAndSaveAsync(DocumentUser documentUser)
        {
            await this.context.DocumentsUsers.AddAsync(documentUser);
            await this.context.SaveChangesAsync();
        }

    }
}
