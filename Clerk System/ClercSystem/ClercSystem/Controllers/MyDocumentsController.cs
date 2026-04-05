using ClercSystem.Data;
using ClercSystem.ViewModels.MyDocuments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Controllers
{
    [Authorize]
    public class MyDocumentsController : BaseController
    {
        private readonly AppDbContext context;

        public MyDocumentsController(AppDbContext context_)
        {
            this.context = context_;    
        }

        public async Task<IActionResult> Index()
        {
            Guid id = base.GetUserIdAsGuid();

             List<MyDocumentsViewModel> documents = await this.context.Documents
                //.Include(d => d.Department)
                //.Include(d => d.CreatedBy)
                //.Include(d => d.Category)
                .Where(d => !d.IsDeleted && d.CreatedById == id)
                .Select(d => new MyDocumentsViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    DepartmentName = d.Department.Name,
                    Createdby = d.CreatedBy.UserName,
                    CategoryName = d.Category.CategoryName
                }).ToListAsync();

            return View(documents);
            
        }
    }
}
