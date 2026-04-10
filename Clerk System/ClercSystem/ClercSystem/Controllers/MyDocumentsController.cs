using ClercSystem.Data;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.MyDocuments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Controllers
{
    
    public class MyDocumentsController : BaseController
    {
        
        private readonly IMyDocumentUserService myDocumentService;

        public MyDocumentsController(AppDbContext context_, IMyDocumentUserService _myDocumentService)
        {
            this.myDocumentService = _myDocumentService;
        }
        [Authorize(Policy = "CanRead")]
        public async Task<IActionResult> Index()
        {

            Guid id = base.GetUserIdAsGuid();
            List<MyDocumentsViewModel> documents = await this.myDocumentService.GetMyDocumentsAsync(id);

            return View(documents);
            
        }
    }
}
