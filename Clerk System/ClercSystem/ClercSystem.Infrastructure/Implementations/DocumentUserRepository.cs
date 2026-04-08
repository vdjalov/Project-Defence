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

        public async Task<bool> DeleteAndSaveAsync(DocumentUser documentUser) // This method deletes a DocumentUser entry from the database based on the provided DocumentUser object.
        {
           DocumentUser? existingDocumentUser = 
                await this.context.DocumentsUsers.FindAsync(documentUser.UserId, documentUser.DocumentId);

            if (existingDocumentUser == null)
            {
                return false;
            }
             this.context.DocumentsUsers.Remove(existingDocumentUser);

             return true;
        }

        public IQueryable<DocumentUser> GetAll()
        {
            return this.context.DocumentsUsers;
        }

        public async Task<DocumentUser?> GetByIdAsync(Guid userId, Guid documentId) // This method retrieves a DocumentUser entry from the database based on the provided userId and documentId.
        {
            DocumentUser? documentUser = this.context.DocumentsUsers.Find(userId, documentId);

            if(documentUser == null)
            {
                return null;
            }

            return documentUser;
        }

        public async Task SaveChangesAsync()   //   This method saves any changes made to the database context.
        {
            await this.context.SaveChangesAsync();
        }

        

        public async Task<bool> UpdateAndSaveAsync(DocumentUser documentUser)
        {
            DocumentUser? existingDocumentUser = 
                await this.context.DocumentsUsers.FindAsync(documentUser.UserId, documentUser.DocumentId);

            if(existingDocumentUser == null)
            {
                return false;
            }

            this.context.Update(documentUser);
            await this.SaveChangesAsync();

            return true;
        }
    }
}
