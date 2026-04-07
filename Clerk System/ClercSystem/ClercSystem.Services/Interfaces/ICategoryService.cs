using ClercSystem.ViewModels.Category;

namespace ClercSystem.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<bool> CategoryExistsAsync(string categoryName, string categoryDescription);
        Task<List<AllCategoriesViewModel>> GetAllCategoriesAsync();
        Task<bool> CreateCategoryAsync(CreateCategoryViewModel model);
        Task<bool> CategoryExistsByIdAsync(string id);
        Task<bool> UpdateCategoryAsync(EditCategoryViewModel model);
        Task<EditCategoryViewModel> GetCategoryForEditByIdAsync(string id);
        Task<CategoryMoreViewModel> GetCategoryDetailsByIdAsync(string id);
        Task<bool> CheckIfThereIsAnyDocumentWithThisCategoryAsync(string id);
        Task<(bool success, string? errorMessage)> DeleteCategoryAsync(string id);
       

    }
}
