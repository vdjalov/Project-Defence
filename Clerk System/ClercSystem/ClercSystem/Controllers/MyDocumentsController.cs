using ClercSystem.Data;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.MyDocuments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClercSystem.Controllers
{
    public class MyDocumentsController : BaseController
    {
        
        private readonly IMyDocumentUserService myDocumentService;

        public MyDocumentsController(IMyDocumentUserService _myDocumentService)
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
