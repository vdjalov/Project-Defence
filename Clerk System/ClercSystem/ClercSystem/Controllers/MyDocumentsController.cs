using ClercSystem.Data;
using ClercSystem.Services.Interfaces;
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
        private readonly IMyDocumentUserService myDocumentService;

        public MyDocumentsController(AppDbContext context_, IMyDocumentUserService _myDocumentService)
        {
            this.context = context_;    
            this.myDocumentService = _myDocumentService;
        }

        public async Task<IActionResult> Index()
        {

            Guid id = base.GetUserIdAsGuid();
            List<MyDocumentsViewModel> documents = await this.myDocumentService.GetMyDocumentsAsync(id);

            return View(documents);
            
        }
    }
}
