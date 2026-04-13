using ClercSystem.Areas.Admin.ViewModels.DocumentLogs;
using ClercSystem.Infrastructure.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DocumentLogsController : Controller
    {
        private readonly IDocumentLogsRepository documentLogsRepository;

        public DocumentLogsController(IDocumentLogsRepository _documentLogsRepository)
        {
            this.documentLogsRepository = _documentLogsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<AllDocumentLogsViewModel> allDocumentLogsViewModels = 
                await this.documentLogsRepository.GetDocumentLogs()
                        .Select(dl => new AllDocumentLogsViewModel()
                        {
                            Id = dl.Id,
                            VersionNumber = dl.VersionNumber,
                            CreatedOn = dl.CreatedOn,
                            AmendedOn = dl.AmendedOn,
                            CreatedByName = dl.CreatedBy.UserName,
                            Desription = dl.Desription,
                        }).ToListAsync();
                


            return View(allDocumentLogsViewModels);
        }
    }
}
