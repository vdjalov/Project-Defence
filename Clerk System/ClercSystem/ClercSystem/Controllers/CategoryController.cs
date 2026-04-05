using ClercSystem.Data;
using ClercSystem.Data.Models;
using ClercSystem.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Controllers
{

    [Authorize]
    public class CategoryController : BaseController
    {
        private readonly AppDbContext context;

        public CategoryController(AppDbContext _context)
        {
            this.context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<AllCategoriesViewModel> categories = await context.Categories
                .Select(c => new AllCategoriesViewModel
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateCategoryViewModel model = new CreateCategoryViewModel();

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Data.Models.Category category = new Data.Models.Category

            {
                CategoryName = model.CategoryName,
                Description = model.Description
            };

            await this.context.Categories.AddAsync(category);
            await this.context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            Data.Models.Category? category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            EditCategoryViewModel model = new EditCategoryViewModel
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Data.Models.Category? category = await context.Categories.FindAsync(model.Id);

            if (category == null)
            {
                return NotFound();
            }

            category.CategoryName = model.CategoryName;
            category.Description = model.Description;
            this.context.Categories.Update(category);

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> More(Guid id)
        {
            Data.Models.Category? category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }
            CategoryMoreViewModel model = new CategoryMoreViewModel
            {
                CategoryName = category.CategoryName,
                Description = category.Description
            };
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            Category? category = await context.Categories.FindAsync(id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            List<Document> documents = await context.Documents
                .Where(d => d.CategoryId == id)
                .ToListAsync();

            if(documents.Count > 0)
            {
                string documentTitles = string.Join(", ", documents.Select(d => d.Title));
                TempData["ErrorMessage"] = "Cannot delete category with associated documents." + documentTitles;
                return RedirectToAction(nameof(Index));
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


    }
}
