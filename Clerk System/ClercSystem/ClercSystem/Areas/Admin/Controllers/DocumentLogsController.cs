using ClercSystem.Areas.Admin.ViewModels.DocumentLogs;
using ClercSystem.Data.Models;
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
        public async Task<IActionResult> Index(string search)
        {
            IQueryable<DocumentLog> allDocuments = this.documentLogsRepository.GetDocumentLogs();

            if(search != null)
            {
                allDocuments = allDocuments
                            .Where(dl =>
                               dl.CreatedBy.UserName.ToLower().Contains(search)
                            || dl.Document.Title.ToLower().Contains(search)
                            || dl.Desription.ToLower().Contains(search)
                            || dl.CreatedOn.ToString().ToLower().Contains(search)
                            || dl.AmendedOn.ToString().ToLower().Contains(search)
                            );
            }

            List<AllDocumentLogsViewModel> allDocumentLogsViewModels = 
                await allDocuments.Select(dl => new AllDocumentLogsViewModel()
                        {
                            Id = dl.Id,
                            VersionNumber = dl.VersionNumber,
                            CreatedOn = dl.CreatedOn,
                            AmendedOn = dl.AmendedOn,
                            CreatedByName = dl.CreatedBy.UserName,
                            Desription = dl.Desription,
                            DocumentName = dl.Document.Title,
                            DocumentId = dl.DocumentId,
                           
                        })
                        .OrderByDescending(dl => dl.CreatedOn)
                        .ToListAsync();
                


            return View(allDocumentLogsViewModels);
        }

        [HttpGet] // Getting all logs for the specific document and return it to the partial view 
        public async Task<IActionResult> GetLogDetails(string documentId)
        {

            List<DocumentLogDetailsViewMode> logs = await this.documentLogsRepository
                .GetDocumentLogs()
                .Where(dl => dl.DocumentId == Guid.Parse(documentId))
                .Select(dl => new DocumentLogDetailsViewMode()
                {
                    DocumentName = dl.Document.Title,
                    VersionNumber = dl.VersionNumber,
                    CreatedOn = dl.CreatedOn,
                    AmendedOn= dl.AmendedOn,
                    CreatedByName = dl.CreatedBy.UserName,
                    Desription = dl.Desription,
                })
                .OrderByDescending (dl => dl.CreatedOn)
                .ToListAsync();

            if (logs == null || !logs.Any())
            {
                return NotFound();
            }

            return PartialView("_LogDetailsListPartial", logs);
        }
    }
}
