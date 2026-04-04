using ClercSystem.Data;
using ClercSystem.ViewModels.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Controllers
{
    public class DocumentController : Controller
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
                .Include(d => d.Department)
                .Include(d => d.CreatedBy)
                .Include(d => d.Category)
                .Select(d => new AllDocumentsViewModel
            {
                Id = d.Id,
                Title = d.Title,
                DepartmentName = d.Department.Name,
                Createdby = d.CreatedBy.UserName,
                CategoryName = d.Category.CategoryName
            }).ToListAsync();

            return View();
        }
    }
}
