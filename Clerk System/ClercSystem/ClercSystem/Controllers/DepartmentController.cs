using ClercSystem.Data;
using ClercSystem.Data.Models;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Controllers
{
    [Authorize]
    public class DepartmentController : BaseController
    {
        private readonly AppDbContext context;
        private readonly IDepartmentService departmentService;

        public DepartmentController(AppDbContext _context, IDepartmentService _departmentService)
        {
            this.departmentService = _departmentService;
            this.context = _context;
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

            if (!departmentExists)
            {
                TempData["ErrorMessage"] = "Department does not exist.";
                return View(model);
            }

            bool isUpdated = await this.departmentService.EditDepartmentAsync(model);

           if(!isUpdated)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the department. Please try again.";
                return View(model);
            }

            TempData["ErrorMessage"] = "Department updated successfully.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> More(Guid id)
        {
            DepartmentMoreViewModel ?department = await this.context.Departments
                .Where(d => d.DepartmentId == id)
                .Select(d => new DepartmentMoreViewModel
                {
                    Name = d.Name,
                    Location = d.Location
                }).FirstOrDefaultAsync();

            if(department == null)
            {
                TempData["ErrorMessage"] = "Department not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            Department? department = await this.context.Departments.FindAsync(id);

            if (department == null)
            {
                TempData["ErrorMessage"] = "Department not found.";
                return RedirectToAction(nameof(Index));
            }

            List<Document> documents = await this.context.Documents
                .Where(d => d.DepartmentId == id)
                .ToListAsync();
            string documentTitles = string.Join(", ", documents.Select(d => d.Title));

            if(documentTitles.Length > 0)
            {
                TempData["ErrorMessage"] = $"Cannot delete department. It is associated with the following documents: {documentTitles}. Please reassign or delete these documents first.";
                return RedirectToAction(nameof(Index));
            }

            this.context.Departments.Remove(department);
            await this.context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
