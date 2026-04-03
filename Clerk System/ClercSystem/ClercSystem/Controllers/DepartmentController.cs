using ClercSystem.Data;
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
                Name = d.Name
            }).ToListAsync();

            return View(allDepartments); ;
        }

    //    [HttpGet]
    //    public async Task<IActionResult> Create()
    //    {

    //    }
    }
}
