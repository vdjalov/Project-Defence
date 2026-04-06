using ClercSystem.Data;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClercSystem.Controllers
{
    [Authorize]
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService departmentService;

        public DepartmentController(IDepartmentService _departmentService)
        {
            this.departmentService = _departmentService;
           
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<AllDepartmentsViewModel> allDepartments = 
                await this.departmentService.GetAllDepartmentsAsync();
             
            return View(allDepartments); ;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateDepartmentViewModel model = this.departmentService.GetCreateModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartmentViewModel model)
        {
            string departmentName = model.Name;
            string departmentLocation = model.Location;
            bool departmentExists = await this.departmentService.DepartmentExistsAsync(departmentName, departmentLocation);

            if (departmentExists)
            {
                TempData["ErrorMessage"] = "A department with the same name and location already exists.";
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View(model);
            }

            try
            {
                await this.departmentService.CreateDepartmentAsync(model);
                TempData["ErrorMessage"] = "Department created successfully:";

            } 
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while creating the department: {ex.Message}";
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            bool isValidGuid = base.CheckIfGuidIsValid(id);

            if (!isValidGuid) // checking if guid is valid, if not return to index with error message
            {
                TempData["ErrorMessage"] = "Invalid department ID.";
                return RedirectToAction(nameof(Index));
            }

            Guid departmentId = Guid.Parse(id);
            bool departmentExists = await this.departmentService.DepartmentExistsByIdAsync(departmentId);

            if (!departmentExists) // checking if department exists, if not return to index with error message
            {
                TempData["ErrorMessage"] = "Department does not exist.";
                return RedirectToAction(nameof(Index));
            }

            EditDepartmentViewModel model = await this.departmentService.GetEditModelAsync(departmentId);
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditDepartmentViewModel model)
        {
            string departmentName = model.Name;
            string departmentLocation = model.Location;
            bool departmentWithSameNameAndLocationExists = await this.departmentService.DepartmentExistsAsync(departmentName, departmentLocation);

            if(departmentWithSameNameAndLocationExists)
            {
                TempData["ErrorMessage"] = "A department with the same name and location already exists.";
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View(model);
            }

            bool isValidGuid = base.CheckIfGuidIsValid(model.DepartmentId.ToString());

            if (!isValidGuid) // checking if guid is valid, if not return to index with error message
            {
                TempData["ErrorMessage"] = "Invalid department ID.";
                return View(model) ;
            }

           bool departmentExists = await this.departmentService.DepartmentExistsByIdAsync(model.DepartmentId);

            if (!departmentExists) // checking if department exists, if not return to index with error message
            {
                TempData["ErrorMessage"] = "Department does not exist.";
                return View(model);
            }

            bool isUpdated = await this.departmentService.EditDepartmentAsync(model);

           if(!isUpdated) // checking if department is updated successfully
            {
                TempData["ErrorMessage"] = "An error occurred while updating the department. Please try again.";
                return View(model);
            }

            TempData["ErrorMessage"] = "Department updated successfully.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> More(string id)
        {
            bool isValidGuid = base.CheckIfGuidIsValid(id);

            if (!isValidGuid) // checking if guid is valid, if not return to index with error message
            {
                TempData["ErrorMessage"] = "Invalid department ID.";
                return RedirectToAction(nameof(Index));
            }

            bool departmentExists = await this.departmentService.DepartmentExistsByIdAsync(Guid.Parse(id));

            if (!departmentExists) // checking if department exists
            {
                TempData["ErrorMessage"] = "Department does not exist.";
                return RedirectToAction(nameof(Index));
            }

            DepartmentMoreViewModel? department = await this.departmentService.GetDepartmentDetailsAsync(id);
               
            return View(department);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            bool isValidGuid = base.CheckIfGuidIsValid(id);

            if (!isValidGuid) // checking if guid is valid, if not return to index with error message
            {
                TempData["ErrorMessage"] = "Invalid department ID.";
                return RedirectToAction(nameof(Index));
            }

            bool departmentExists = await this.departmentService.DepartmentExistsByIdAsync(Guid.Parse(id));

            if (!departmentExists) // checking if department exists
            {
                TempData["ErrorMessage"] = "Department not found.";
                return RedirectToAction(nameof(Index));
            }

           bool checkIfThereAreDocumentsAssociatedWithDepartment = 
                    await this.departmentService.CheckIfThereAreDocumentsAssociatedWithDepartmentAsync(id);

            if(checkIfThereAreDocumentsAssociatedWithDepartment)
            {
                TempData["ErrorMessage"] = "Cannot delete department. It is associated with documents:";
                return RedirectToAction(nameof(Index));
            }

            try
            {

                bool departmentDeletionSiccess = await this.departmentService.DeleteDepartmentAsync(id);
                TempData["ErrorMessage"] = "Department deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Unable to delete. Check if there is any documents associated with the department?";
                return RedirectToAction(nameof(Index));

            }

            return RedirectToAction(nameof(Index));
        }
    }
}
