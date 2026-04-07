using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Category;
using ClercSystem.ViewModels.Department;
using ClercSystem.ViewModels.Document;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Services.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository documentRepository;
        private readonly IDepartmentRepository departmentRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IDocumentUserRepository documentUserRepository;

        public DocumentService(IDocumentRepository _documentRepository,
                            IDepartmentRepository _departmentRepository,
                            ICategoryRepository categoryRepository,
                            IDocumentUserRepository documentUserRepository)
        {
            this.documentRepository = _documentRepository;
            this.departmentRepository = _departmentRepository;
            this.categoryRepository = categoryRepository;
            this.documentUserRepository = documentUserRepository;
        }

        public async Task<bool> CheckIfDocumentCreatorIsValid(Guid documentId, Guid userId)
        {
            Document document = await this.documentRepository.GetByIdAsync(documentId);

            if(document == null || document.CreatedById != userId)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CheckIfDocumentExists(Guid guid)
        {
           return await this.documentRepository.GetByIdAsync(guid) != null;
        }

        // this method is used to create a document and assign the user who created it with full permissions
        public async Task<bool> CreateDocumentAsync(CreateDocumentViewModel model, Guid userId,DateTime date )
        {
            Document document = new Document
            {
                Title = model.Title,
                FilePath = model.FilePath,
                CreatedOn = date,
                TimeToAnswer = model.TimeToAnswer,
                HasBeenAnswered = model.HasBeenAnswered,
                Description = model.Description,
                DepartmentId = Guid.Parse(model.DepartmentId),
                CategoryId = Guid.Parse(model.CategoryId),
                CreatedById = userId
            };

            DocumentUser documentUser = new DocumentUser
            {
                DocumentId = document.Id,
                UserId = userId,
                Permission = PermissionType.Full // persmission should amended by admin if necessary all documents get default 
            };

            try
            {
                await this.documentRepository.AddAndSaveAsync(document);
                await this.documentUserRepository.AddAndSaveAsync(documentUser);

            } catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        // this method is used to edit a document, only the creator of the document can edit it
        public async Task<bool> EditDocumentAsync(Guid documentId, EditDocumentViewModel model)
        {
            Document document = await this.documentRepository.GetByIdAsync(documentId);
            document.Title = model.Title;
            document.FilePath = model.FilePath;
            document.TimeToAnswer = model.TimeToAnswer;
            document.Description = model.Description;
            document.DepartmentId = Guid.Parse(model.DepartmentId);
            document.CategoryId = Guid.Parse(model.CategoryId);
            document.HasBeenAnswered = model.HasBeenAnswered;
            
            bool hasBeenUpadated = await documentRepository.UpdateAndSaveAsync(document);

            return hasBeenUpadated;
        }

       

        // this method is used to get all documents with pagination and search functionality
        public async Task<(IEnumerable<AllDocumentsViewModel> Docs, int TotalCount)> GetAllDocumentsAsync(string search, int page, int pageSize)
        {
            IQueryable<Document> allDocuments =  this.documentRepository.GetAll();
         

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                allDocuments = allDocuments
                    .Where(d => !d.IsDeleted
                        || d.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                        || d.Department.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                        || d.CreatedBy.UserName.Contains(search, StringComparison.OrdinalIgnoreCase)
                        || d.Category.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase)
                        );
            }

            int totalDocuments = await allDocuments.CountAsync(); // total items in pagination

            List<AllDocumentsViewModel> documents = await allDocuments
                .Where(d => !d.IsDeleted)
                .Select(d => new AllDocumentsViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    DepartmentName = d.Department.Name,
                    Createdby = d.CreatedBy.UserName,
                    CategoryName = d.Category.CategoryName
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // here everything is being executed in the database, and only the
                                // paginated results are brought to memory. this is the right way

            return (documents, totalDocuments);
        }

        public async Task<CreateDocumentViewModel> GetCreateModelAsync() // this method is used to get the data for the create document
        {
            IQueryable<Department> allDepartments =  this.departmentRepository.GetAll();
            IQueryable<Category> allCategories =  this.categoryRepository.GetAll();

            List<AllDepartmentsViewModel> departments = await allDepartments
                .Select(d => new AllDepartmentsViewModel
                 {
                     DepartmentId = d.DepartmentId,
                     Name = d.Name
                 }).ToListAsync();

            List<AllCategoriesViewModel> categories = await allCategories
                .Select(c => new AllCategoriesViewModel
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                }).ToListAsync();

            CreateDocumentViewModel createDocumentViewModel = new CreateDocumentViewModel();
            createDocumentViewModel.Departments = departments;
            createDocumentViewModel.Categories = categories;


            return createDocumentViewModel;

        }

        // this method is used to get the details of a document, only the creator of the document can see the details
        public async Task<DocumentMoreViewModel?> GetDetailsAsync(Guid id)
        {
            Document? document = await this.documentRepository.GetAll()
                .Include(d => d.Department)
                .Include(d => d.CreatedBy)
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.Id == id);

            DocumentMoreViewModel documentMoreViewModel = new DocumentMoreViewModel
            {
                Title = document.Title,
                FilePath = document.FilePath,
                CreatedOn = document.CreatedOn,
                TimeToAnswer = document.TimeToAnswer,
                HasBeenAnswered = document.HasBeenAnswered,
                Description = document.Description,
                DepartmentName = document.Department.Name,
                CreatedBy = document.CreatedBy.UserName,
                CategoryName = document.Category.CategoryName,
            };

           return documentMoreViewModel;
        }

        // this method is used to get the data for the edit document
        public async Task<EditDocumentViewModel?> GetEditModelAsync(Guid documentId)
        {

            IQueryable<Department> allDepartments = this.departmentRepository.GetAll();
            IQueryable<Category> allCategories = this.categoryRepository.GetAll();

            List<AllDepartmentsViewModel> departments = await allDepartments
                .Select(d => new AllDepartmentsViewModel
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name,
                    Location = d.Location
                }).ToListAsync();

            List<AllCategoriesViewModel> categories = await allCategories
                .Select(c => new AllCategoriesViewModel
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                    
                }).ToListAsync();

            Document document = await this.documentRepository.GetByIdAsync(documentId);

            if(document.IsDeleted)
            {
                return null;
            }

            EditDocumentViewModel? editDocumentViewModel = new EditDocumentViewModel()
            {
                Title = document.Title,
                FilePath = document.FilePath,
                TimeToAnswer = document.TimeToAnswer,
                Description = document.Description,
                DepartmentId = document.DepartmentId.ToString(),
                CategoryId = document.CategoryId.ToString(),
                HasBeenAnswered = document.HasBeenAnswered,
                Departments = departments,
                Categories = categories,
            };

            return editDocumentViewModel;
        }

        // this method is used to soft delete a document, only the creator of the document can delete it
        public Task<bool> SoftDeleteAsync(Guid userId)
        {
            return this.documentRepository.SoftDeleteAsync(userId);
        }
    }
}
