using ClercSystem.Data;
using ClercSystem.Data.Models;
using ClercSystem.ViewModels.Category;
using ClercSystem.ViewModels.Department;
using ClercSystem.ViewModels.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Controllers
{
    public class DocumentController : BaseController
    {
        private readonly AppDbContext context;

        public DocumentController(AppDbContext _context)
        {
            this.context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<AllDocumentsViewModel> documents = await this.context.Documents
                //.Include(d => d.Department)
                //.Include(d => d.CreatedBy)
                //.Include(d => d.Category)
                .Where(d => !d.IsDeleted)
                .Select(d => new AllDocumentsViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    DepartmentName = d.Department.Name,
                    Createdby = d.CreatedBy.UserName,
                    CategoryName = d.Category.CategoryName
                }).ToListAsync();

            return View(documents);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateDocumentViewModel createDocumentViewModel = new CreateDocumentViewModel();
            createDocumentViewModel.Departments = await this.context.Departments
                .Select(d => new AllDepartmentsViewModel
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name
                }).ToListAsync();

            createDocumentViewModel.Categories = await this.context.Categories
                 .Select(c => new AllCategoriesViewModel
                 {
                     Id = c.Id,
                     CategoryName = c.CategoryName
                 }).ToListAsync();

            return View(createDocumentViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDocumentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Oops something went awire.";
                return View(model);
            }

            Guid userId = Guid.Parse(base.GetUserId());

            DateTime date;
            bool success = DateTime.TryParse(model.CreatedOn, out date);

            if(!success)
            {
                throw new ArgumentException("Invalid date format for CreatedOn");
            }

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

            await this.context.Documents.AddAsync(document);
            await this.context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }









        [HttpGet] // soft deleting a document so that it does not get lost permanently and can be restored if needed
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            Document? document = await this.context.Documents.FindAsync(id);

            if(document == null)
            {
                TempData["ErrorMessage"] = "Oops something went awire.";
                return RedirectToAction(nameof(Index));
            }

            bool isGuidValid = base.CheckIfGuidIsValid(id);
            
            if (!isGuidValid)
            {
                TempData["ErrorMessage"] = "Invalid Id";
                return RedirectToAction(nameof(Index));
            }

           if(document.CreatedById != base.GetUserIdAsGuid())
            {
                TempData["ErrorMessage"] = "You do not have sufficient rights to work on this document";
                return RedirectToAction(nameof(Index));
            }

           document.IsDeleted = true;
           this.context.Update(document);
           await this.context.SaveChangesAsync();

           return RedirectToAction(nameof(Index));
        }


     

        

    }
}
