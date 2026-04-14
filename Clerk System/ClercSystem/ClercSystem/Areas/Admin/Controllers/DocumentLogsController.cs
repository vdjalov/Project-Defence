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
                            DocumentName = dl.Document.Title,
                           
                        })
                        .OrderByDescending(dl => dl.CreatedOn)
                        .ToListAsync();
                


            return View(allDocumentLogsViewModels);
        }

        [HttpGet] // Getting all logs for the specific document and return it to the partial view 
        public async Task<IActionResult> GetLogDetails(string documentId)
        {
            var logs = await this.documentLogsRepository
                .GetDocumentLogs()
                .Where(dl => dl.DocumentId == Guid.Parse(documentId))
                .ToListAsync();

            if (logs == null || !logs.Any())
            {
                return NotFound();
            }

            return PartialView("_LogDetailsPartial", logs);
        }
    }
}
