using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Implementations;
using ClercSystem.Infrastructure.Interfaces;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.MyDocuments;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Services.Implementations
{
    public class MyDocumentUserService : IMyDocumentUserService
    {
        private readonly IDocumentUserRepository documentUserRepository;

        public MyDocumentUserService(IDocumentUserRepository _documentUserRepository)
        {
             this.documentUserRepository = _documentUserRepository;
        }

        // This method retrieves a list of documents associated with a specific user, identified by their userId. It queries the DocumentUser repository to find all entries where the UserId matches the provided userId and the associated document is not marked as deleted. The results are then projected into a list of MyDocumentsViewModel objects, which contain relevant information about each document, such as its title, department name, creator's username, and category name. Finally, the method returns the list of MyDocumentsViewModel objects.
        public async Task<List<MyDocumentsViewModel>> GetMyDocumentsAsync(Guid userId)
        {
            IQueryable<DocumentUser> documentUsers =  this.documentUserRepository.GetAll(); // returns empty collection if no documents are found for the user

            List<MyDocumentsViewModel> userDocuments = await documentUsers 
                                .Where(d => d.UserId == userId && !d.Document.IsDeleted)
                                .Select(d => new MyDocumentsViewModel
                                {
                                    Id = d.DocumentId,
                                    Title = d.Document.Title,
                                    DepartmentName = d.Document.Department.Name,
                                    Createdby = d.Document.CreatedBy.UserName,
                                    CategoryName = d.Document.Category.CategoryName
                                })
                                .ToListAsync();

            return userDocuments;
        }
    }
}
