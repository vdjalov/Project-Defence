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


        public DocumentService(IDocumentRepository _documentRepository,
                            IDepartmentRepository _departmentRepository,
                            ICategoryRepository categoryRepository)
        {
            this.documentRepository = _documentRepository;
            this.departmentRepository = _departmentRepository;
            this.categoryRepository = categoryRepository;
        }

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



            //documents = documents // pagination logic
            //    .Skip((page - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToList();

            return (documents, totalDocuments);
        }

        public async Task<CreateDocumentViewModel> GetCreateModelAsync()
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

    }
}
