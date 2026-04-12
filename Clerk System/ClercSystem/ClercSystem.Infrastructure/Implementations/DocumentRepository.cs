using ClercSystem.Data;
using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Infrastructure.Implementations
{
    public class DocumentRepository : IDocumentRepository
    {

        private readonly AppDbContext context;

        public DocumentRepository(AppDbContext _context)
        {
            this.context = _context;
        }

        public async Task AddAndSaveAsync(Document document)
        {
            await this.context.Documents.AddAsync(document);
            await this.context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAndSaveAsync(Document document)
        {
            Document? existingDocument = await this.GetByIdAsync(document.Id);
            if (existingDocument == null)
            {
                return false;
            }

            this.context.Documents.Remove(existingDocument);
            await this.context.SaveChangesAsync();
            return true;
        }

       
        public  IQueryable<Document> GetAll()
        {
            return  this.context.Documents;
        }

        public async Task<Document> GetByIdAsync(Guid id)
        {
            Document document = await this.context.Documents.FindAsync(id);

            return document;
        }

        public async Task<Document?> GetByTitleAsync(string title)
        {
            return await this.context.Documents
                .FirstOrDefaultAsync(d => d.Title == title);
        }

        public async Task SaveChangesAsync()
        {
            await this.context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAndSaveAsync(Document document)
        {
            Document? existingDocument = await this.GetByIdAsync(document.Id);

            if (existingDocument == null)
            {
                return false;
            }

            this.context.Documents.Update(document);
            await this.SaveChangesAsync();

            return true;
        }

        // Soft delete a document by setting its IsDeleted property to true
        public async Task<bool> SoftDeleteAsync(Guid documentId)
        {
            Document document = await this.GetByIdAsync(documentId);

            if(document == null)
            {
                return false;
            }

            document.IsDeleted = true;
            this.context.Update(document);
            await this.SaveChangesAsync();

            return true;
        }

        // Method for undelete admin only
        public async Task<Document?> GetByIdAsyncUnfiltered(Guid id)
        {
            return await this.context.Documents.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
