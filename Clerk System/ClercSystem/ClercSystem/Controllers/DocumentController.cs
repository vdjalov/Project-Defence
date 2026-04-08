using ClercSystem.Data;
using ClercSystem.Data.Models;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Category;
using ClercSystem.ViewModels.Department;
using ClercSystem.ViewModels.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Controllers
{
    [Authorize]
    public class DocumentController : BaseController
    {
        private readonly AppDbContext context;
        private readonly IDocumentService documentService;

        public DocumentController(AppDbContext _context, IDocumentService _documentService)
        {
            this.context = _context;
            this.documentService = _documentService;
        }

        [HttpGet] 
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 5) // view all documents with pagination and search functionality
        {

            var result = await documentService.GetAllDocumentsAsync(search, page, pageSize);

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);

            return View(result.Docs);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateDocumentViewModel createDocumentViewModel = await documentService.GetCreateModelAsync();
            
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
            if(userId == Guid.Empty)
            {
                TempData["Message"] = "User not found!";
                return View(model);
            }
           
            DateTime date;
            bool success = DateTime.TryParse(model.CreatedOn, out date);

            if(!success)
            {
                TempData["Message"] = "Invalid date format.";
                return View(model);
            }

            bool hasDocumentBeenCreated = await this.documentService.CreateDocumentAsync(model, userId, date);

            if(!hasDocumentBeenCreated)
            {
                TempData["Message"] = "Oops something went awire.";
                return View(model);
            }

            TempData["Message"] = "Document created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id) // edit document get view
        {
            bool isGuidValid = base.CheckIfGuidIsValid(id);

            if (!isGuidValid)
            {
                TempData["Message"] = "Invalid Id";
                return RedirectToAction(nameof(Index));
            }

            Guid userId = base.GetUserIdAsGuid();   
            bool checkIfDocumentCreatorIsValidOrDocumentExists = 
                await this.documentService.CheckIfDocumentCreatorIsValid(Guid.Parse(id), userId);


            //only the creator of the document can edit it ?????
            if (!checkIfDocumentCreatorIsValidOrDocumentExists)
            {
                TempData["Message"] = "You do not have sufficient rights to work on this document or it does not exist.";
                return RedirectToAction(nameof(Index));
            }

            EditDocumentViewModel editDocumentViewModel = await this.documentService.GetEditModelAsync(Guid.Parse(id));

            if(editDocumentViewModel == null)
            {
                TempData["Message"] = "Document not found!";
                return RedirectToAction(nameof(Index));
            }
           
           return View(editDocumentViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditDocumentViewModel model)
        {
            bool isGuidValid = base.CheckIfGuidIsValid(id);

            if (!isGuidValid) // checking if the id is a valid guid to prevent errors when trying to find the document in the database
            {
                TempData["Message"] = "Invalid Id";
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Oops something went awire.";
                return View(model);
            }

            Guid userId = base.GetUserIdAsGuid();
            bool checkIfDocumentCreatorIsValidOrDocumentExists =
                await this.documentService.CheckIfDocumentCreatorIsValid(Guid.Parse(id), userId);


            //only the creator of the document can edit it ?????
            if (!checkIfDocumentCreatorIsValidOrDocumentExists)
            {
                TempData["Message"] = "You do not have sufficient rights to work on this document or it does not exist.";
                return RedirectToAction(nameof(Index));
            }


            bool documentUpdated = await this.documentService.EditDocumentAsync(Guid.Parse(id), model);

           if(!documentUpdated)
            {
                TempData["Message"] = "Document was not updated successfuly.";
                return View(model);
            }

            TempData["Message"] = "Document updated successfully!";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> More(string id) // view more details about a document
        {
            bool isGuidValid = base.CheckIfGuidIsValid(id);
            if (!isGuidValid)
            {
                TempData["ErrorMessage"] = "Invalid Id";
                return RedirectToAction(nameof(Index));
            }

            bool checkIfDocumentExists = await this.documentService.CheckIfDocumentExists(Guid.Parse(id));
            if(checkIfDocumentExists == false) 
            {
                TempData["ErrorMessage"] = "Document not found.";
                return RedirectToAction(nameof(Index));
            }

            DocumentMoreViewModel model = await this.documentService.GetDetailsAsync(Guid.Parse(id));

            return View(model);
        }


        [HttpGet] // soft deleting a document so that it does not get lost permanently and can be restored if needed
        public async Task<IActionResult> SoftDelete(string id)
        {

            bool isGuidValid = base.CheckIfGuidIsValid(id);
            if (!isGuidValid) // checking if the id is a valid guid to prevent errors when trying to find the document in the database
            {
                TempData["ErrorMessage"] = "Invalid Id";
                return RedirectToAction(nameof(Index));
            }

            bool checkIfDocumentExists = await this.documentService.CheckIfDocumentExists(Guid.Parse(id));
            if (checkIfDocumentExists == false) // checking if the document exists in the database before trying to delete it
            {
                TempData["ErrorMessage"] = "Document not found.";
                return RedirectToAction(nameof(Index));
            }

            Guid userId = base.GetUserIdAsGuid();
            bool checkIfDocumentCreatorIsValidOrDocumentExists =
                await this.documentService.CheckIfDocumentCreatorIsValid(Guid.Parse(id), userId);


            //only the creator of the document can edit it ?????
            if (!checkIfDocumentCreatorIsValidOrDocumentExists)
            {
                TempData["Message"] = "You do not have sufficient rights to work on this document or it does not exist.";
                return RedirectToAction(nameof(Index));
            }

            bool documentSoftDeleted = await this.documentService.SoftDeleteAsync(Guid.Parse(id));  

            if(documentSoftDeleted == false)
            {
                TempData["Message"] = "Document was not deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Document deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
