using ClercSystem.Data;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClercSystem.Controllers
{

    
    public class CategoryController : BaseController
    {
        
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService _categoryService)
        {
            this.categoryService = _categoryService;
        }


        [HttpGet]
        [Authorize(Policy = "CanRead")]
        public async Task<IActionResult> Index()
        {
            List<AllCategoriesViewModel> categories = 
                await this.categoryService.GetAllCategoriesAsync();

            return View(categories);
        }


        [HttpGet]
        [Authorize(Policy = "CanCreate")]
        public async Task<IActionResult> Create()
        {
            CreateCategoryViewModel model = new CreateCategoryViewModel();

            return View(model);
        }


        [HttpPost]
        [Authorize(Policy = "CanCreate")]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            
            bool categoryExists = await this.categoryService.CategoryExistsAsync(model.CategoryName, model.Description);

            if(categoryExists) // This check is necessary to prevent the creation of duplicate categories.
            {
                ModelState.AddModelError("CategoryName", "A category with this name already exists.");
                return View(model);
            }

            if (!ModelState.IsValid) // This check is necessary to ensure that the model is valid 
            {
                return View(model);
            }

            bool hasBeenCreated = await this.categoryService.CreateCategoryAsync(model);

            if(!hasBeenCreated) // This check is necessary to handle the case where the category creation fails for some reason (e.g., database error).
            {
                TempData["Message"] = "Category was not created.";
                return View(model);
            }

            TempData["Message"] = "Category was created successfully.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [Authorize(Policy = "CanCreate")]
        public async Task<IActionResult> Edit(string id)
        {

            bool isValidGuid = base.CheckIfGuidIsValid(id);

            if (!isValidGuid) // checking if guid is valid, if not return to index with error message
            {
                TempData["Message"] = "Invalid department ID.";
                return RedirectToAction(nameof(Index));
            }

            bool checkIfCategorytExists = await this.categoryService.CategoryExistsByIdAsync(id);

            if (!checkIfCategorytExists)
            {
                TempData["Message"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            EditCategoryViewModel model = await this.categoryService.GetCategoryForEditByIdAsync(id);
            
            return View(model);
        }


        [HttpPost]
        [Authorize(Policy = "CanCreate")]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {

            bool isValidGuid = base.CheckIfGuidIsValid(model.Id.ToString());

            if (!isValidGuid) // checking if guid is valid, if not return to index with error message
            {
                TempData["Message"] = "Invalid department ID.";
                return RedirectToAction(nameof(Index));
            }

            bool categoryExists = await this.categoryService.CategoryExistsAsync(model.CategoryName, model.Description);

            if (categoryExists) // This check is necessary to prevent the creation of duplicate categories.
            {
                ModelState.AddModelError("CategoryName", "A category with this name already exists or no changes were made.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Invalid data.";
                return View(model);
            }

            bool checkIfCategorytExists = await this.categoryService.CategoryExistsByIdAsync(model.Id.ToString());

            if (!checkIfCategorytExists) // This check is necessary to ensure that the category being edited actually exists in the database.
            {
                TempData["Message"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            bool updated = await this.categoryService.UpdateCategoryAsync(model);
            if(!updated)
            {
                TempData["Message"] = "Category was not updated.";
                return View(model);
            }

            TempData["Message"] = "Category was updated successfully.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [Authorize(Policy = "CanRead")]
        public async Task<IActionResult> More(string id)
        {

            bool isValidGuid = base.CheckIfGuidIsValid(id);

            if (!isValidGuid) // checking if guid is valid, if not return to index with error message
            {
                TempData["Message"] = "Invalid Category ID.";
                return RedirectToAction(nameof(Index));
            }

            bool checkIfCategorytExists = await this.categoryService.CategoryExistsByIdAsync(id);
           
            if (!checkIfCategorytExists) // This check is necessary to ensure that the category being viewed actually exists in the database.
            {
                TempData["Message"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            CategoryMoreViewModel model = await this.categoryService.GetCategoryDetailsByIdAsync(id);
           
            return View(model);
        }


        [HttpGet]
        [Authorize(Policy = "CanDelete")]
        public async Task<IActionResult> Delete(string id)
        {
            bool isValidGuid = base.CheckIfGuidIsValid(id);

            if (!isValidGuid) // checking if guid is valid, if not return to index with error message
            {
                TempData["Message"] = "Invalid Category ID.";
                return RedirectToAction(nameof(Index));
            }

            bool checkIfCategorytExists = await this.categoryService.CategoryExistsByIdAsync(id);

            if (!checkIfCategorytExists) // This check is necessary to ensure that the category being viewed actually exists in the database.
            {
                TempData["Message"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            bool checkIfThereIsAnyDocumentWithThisCategory = 
                await this.categoryService.CheckIfThereIsAnyDocumentWithThisCategoryAsync(id);

            if(checkIfThereIsAnyDocumentWithThisCategory) // This check is necessary to prevent the deletion of a category that has associated documents with it.
            {
                TempData["Message"] = "Cannot delete category with associated documents.";
                return RedirectToAction(nameof(Index));
            };

           var (success, message) = await this.categoryService.DeleteCategoryAsync(id);
           TempData["Message"] = message;

           return RedirectToAction(nameof(Index));

        }


    }
}
