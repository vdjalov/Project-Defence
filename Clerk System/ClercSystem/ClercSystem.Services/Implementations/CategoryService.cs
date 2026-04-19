using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Category;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        public CategoryService(ICategoryRepository _categoryRepository)
        {
            this.categoryRepository = _categoryRepository;
        }

        //  This method checks if a category exists by its name and dr.
        //  It retrieves the category using the repository and returns true if it exists, otherwise false.
        public async Task<bool> CategoryExistsAsync(string categoryName, string categoryDescription)
        {
            Category? category = await this.categoryRepository.GetByNameAndDescriptionAsync(categoryName, categoryDescription);

            if(category == null)
            {
                return false;
            }

            return true; 
        }

        // This method checks if a category exists by its ID. 
        public async Task<bool> CategoryExistsByIdAsync(string id)
        {
            Category? category = await this.categoryRepository.GetByIdAsync(Guid.Parse(id));

            if(category == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CheckIfThereIsAnyDocumentWithThisCategoryAsync(string id)
        {
            Category? category = await this.categoryRepository.GetByIdAsync(Guid.Parse(id));

            if (category.Documents.Count > 0)
            {
                return true;
            }

            return false;
        }

        // This method creates a new category based on the provided CreateCategoryViewModel and saves it to the database.
        public async Task<bool> CreateCategoryAsync(CreateCategoryViewModel model)
        {
            Data.Models.Category category = new Data.Models.Category

            {
                CategoryName = model.CategoryName,
                Description = model.Description
            };

            try 
            {
                await this.categoryRepository.AddAndSaveAsync(category);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"An error occurred while creating the category: {ex.Message}");
                return false;
            }

            return true;
        }

        // This method retrieves all categories and maps them to a list of AllCategoriesViewModel.
        public async Task<List<AllCategoriesViewModel>> GetAllCategoriesAsync()
        {
            IQueryable<Category> categories = categoryRepository.GetAll();

            List<AllCategoriesViewModel> categoriesView = await categories
               .Select(c => new AllCategoriesViewModel
               {
                   Id = c.Id,
                   CategoryName = c.CategoryName
               })
               .ToListAsync();

            return categoriesView;
        }

        // This method retrieves a category by its ID and maps it to an EditCategoryViewModel.
        public async Task<EditCategoryViewModel> GetCategoryForEditByIdAsync(string id)
        {
            Category category = await this.categoryRepository.GetByIdAsync(Guid.Parse(id));

            EditCategoryViewModel model = new EditCategoryViewModel
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description
            };

            return model;
        }

        // This method retrieves a category by its ID and maps it to a CategoryMoreViewModel.
        public async Task<CategoryMoreViewModel> GetCategoryDetailsByIdAsync(string id)
        {
            Category? category = await this.categoryRepository.GetByIdAsync(Guid.Parse(id));

            CategoryMoreViewModel model = new CategoryMoreViewModel
            {
                CategoryName = category.CategoryName,
                Description = category.Description
            };

            return model;
        }

        // This method updates an existing category based on the provided EditCategoryViewModel.
        public async Task<bool> UpdateCategoryAsync(EditCategoryViewModel model)
        {
            Category? category = await this.categoryRepository.GetByIdAsync(model.Id);

            category?.CategoryName = model.CategoryName;
            category?.Description = model.Description;

            bool hasBeenUpdated = await this.categoryRepository.UpdateAndSaveAsync(category);

            if(hasBeenUpdated)
            {
                return true;
            }

            return false;
        }

        public async Task<(bool success, string? errorMessage)> DeleteCategoryAsync(string id)
        {
            Guid guidId = Guid.Parse(id);
            Category? category = await this.categoryRepository.GetByIdAsync(guidId);

            bool hasBeenDeleted = await this.categoryRepository.DeleteAndSaveAsync(category);

            if(hasBeenDeleted)
            {
                return (true, $"Category - {category.CategoryName} - has been deleted");
            }
            return (false, $"Category - {category.CategoryName} - was NOT deleted");
        }
    }
}
