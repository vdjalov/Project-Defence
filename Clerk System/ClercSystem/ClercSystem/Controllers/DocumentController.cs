using ClercSystem.Data;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClercSystem.Controllers
{
    [Authorize]
    public class DocumentController : BaseController
    {
        
        private readonly IDocumentService documentService;

        public DocumentController(IDocumentService _documentService)
        {
            this.documentService = _documentService;
        }

        [HttpGet]
        [Authorize(Policy = "CanReadNoAdmin")]
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
        [Authorize(Policy = "CanUpdate")]
        public async Task<IActionResult> Create()
        {
            CreateDocumentViewModel createDocumentViewModel = await documentService.GetCreateModelAsync();
            
            return View(createDocumentViewModel);
        }

        [HttpPost]
        [Authorize(Policy = "CanUpdate")]
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
        [Authorize(Policy = "CanUpdate")]
        public async Task<IActionResult> Edit(string id, string source) // edit document get view, parameter source used for redirection purposes 
        {
            bool isGuidValid = base.CheckIfGuidIsValid(id);

            if (!isGuidValid)  // checking if guid is valid 
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
                if (!User.IsInRole("Admin"))
                {
                    TempData["Message"] = "You do not have sufficient rights to work on this document or it does not exist.";
                    return RedirectToAction(nameof(Index));
                }
            }
            
            EditDocumentViewModel editDocumentViewModel = await this.documentService.GetEditModelAsync(Guid.Parse(id));

            if(editDocumentViewModel == null) // checking if document exists  
            {
                TempData["Message"] = "Document not found!";
                return RedirectToAction(nameof(Index));
            }

            bool isManager = User.Claims.Any(c => c.Type == "IsManager" && c.Value.ToLower() == "true");

            ViewBag.Ismanager = isManager;
            ViewBag.Source = source;
            return View(editDocumentViewModel);
        }

        [HttpPost]
        [Authorize(Policy = "CanUpdate")]
        public async Task<IActionResult> Edit(string id, EditDocumentViewModel model, string source)
        {
            bool isGuidValid = base.CheckIfGuidIsValid(id);

            if (!isGuidValid) // checking if the id is a valid guid to prevent errors when trying to find the document in the database
            {
                TempData["Message"] = "Invalid Id";
                return View(model);
            }

            if(User.IsInRole("User")) // If User so that source is true for redirection purposes.
            {
                ModelState.Remove("source");
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
                if(!User.IsInRole("Admin"))
                {
                    TempData["Message"] = "You do not have sufficient rights to work on this document or it does not exist.";
                    return RedirectToAction(nameof(Index));
                }
            }

            bool isUserInRoleAdmin = User.IsInRole("Admin");

            bool documentUpdated = await this.documentService.EditDocumentAsync(Guid.Parse(id),isUserInRoleAdmin, model);

           if(!documentUpdated)
            {
                TempData["Message"] = "Document was not updated successfuly. You rights to edit this document might have been restricted.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Document updated successfully!";
            if(source != null)
            {
                return RedirectToAction("Index", "DocumentManagement", new { area = "Admin" });
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [Authorize(Policy = "CanRead")]
        public async Task<IActionResult> More(string id, string source) // view more details about a document
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
            DocumentMoreViewModel model = null;

            try // throws bad request if cant get the data
            {
                model = await this.documentService.GetDetailsAsync(Guid.Parse(id));
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            ViewBag.Source = source;

            return View(model);
        }


        [HttpPost] // soft deleting a document so that it does not get lost permanently and can be restored if needed
        [Authorize(Policy = "CanUpdate")]
        public async Task<IActionResult> SoftDelete(string id, string returnUrl)
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
                TempData["Message"] = "You do not have sufficient rights to work on this document or it does not exist.If you are an admin user you can restrict it.";
                return RedirectToAction(nameof(Index));
            }

            bool userIsInRoleAdmin = User.IsInRole("Admin");

            bool documentSoftDeleted = await this.documentService.SoftDeleteAsync(Guid.Parse(id), userIsInRoleAdmin);  

            if(documentSoftDeleted == false)
            {
                TempData["Message"] = "Document was not deleted successfully. You might not have sufficien rights to do it.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Document deleted successfully!";
            if(!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
