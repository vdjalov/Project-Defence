using ClercSystem.Areas.Admin.Models.DocumentManagement;
using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DocumentManagementController : Controller
    {


        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly IDocumentRepository documentRepository;

        public DocumentManagementController(UserManager<ApplicationUser> _userManager,
                                            RoleManager<IdentityRole<Guid>> _roleManager,
                                            IDocumentRepository _documentRepository)
        {
            this.userManager = _userManager;
            this.roleManager = _roleManager;
            this.documentRepository = _documentRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            IEnumerable<AllDocumentsAdminViewModel> allDocuments = await this.documentRepository
                                            .GetAll()
                                            .IgnoreQueryFilters() // ignores global query filters
                                            .Select(d => new AllDocumentsAdminViewModel
                                                
                                                {
                                                    Id = d.Id,
                                                    Title = d.Title,
                                                    DepartmentName = d.Department.Name,
                                                    Createdby = d.CreatedBy.UserName,
                                                    CategoryName = d.Category.CategoryName,
                                                    IsDeleted = d.IsDeleted,
                                                }).ToListAsync();


            return View(allDocuments);
        }

        [HttpPost]
        public async Task<IActionResult> UndeleteDocument(string id) 
        {

            Document document = await documentRepository.GetByIdAsyncUnfiltered(Guid.Parse(id)) ;
                
            if(document == null)
            {
                return NotFound();
            }

            document.IsDeleted = false;
            bool undeletionSuccess  = await this.documentRepository.UpdateAndSaveAsync(document);

            if(!undeletionSuccess)
            {
                return BadRequest();
            }

            TempData["Message"] = "Document has been restored.";
            return RedirectToAction(nameof(Index));
        }
    }
}
