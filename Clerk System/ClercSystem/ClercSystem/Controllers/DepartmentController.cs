using ClercSystem.Data;
using ClercSystem.Data.Models;
using ClercSystem.ViewModels.Department;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly AppDbContext context;

        public DepartmentController(AppDbContext _context)
        {
            this.context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<AllDepartmentsViewModel> allDepartments =
               await this.context.Departments
                .Select(d => new AllDepartmentsViewModel
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name,
                    Location = d.Location,
                }).ToListAsync();

            return View(allDepartments); ;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateDepartmentViewModel model = new CreateDepartmentViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartmentViewModel model)
        {
            // todo need to check if department exists ont that address already may be address entity also 

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Data.Models.Department department = new Data.Models.Department

            {
                Name = model.Name,
                Location = model.Location
            };
            await this.context.Departments.AddAsync(department);
            await this.context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            Data.Models.Department? department = await this.context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            EditDepartmentViewModel model = new EditDepartmentViewModel
            {
                DepartmentId = department.DepartmentId,
                Name = department.Name,
                Location = department.Location
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditDepartmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Data.Models.Department? department = await this.context.Departments.FindAsync(model.DepartmentId);

            if (department == null)
            {
                return NotFound();
            }

            department.Name = model.Name;
            department.Location = model.Location;

            this.context.Departments.Update(department);
            await this.context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            Department department = await this.context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            this.context.Departments.Remove(department);
            await this.context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
