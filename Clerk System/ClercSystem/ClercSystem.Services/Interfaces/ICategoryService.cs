using ClercSystem.ViewModels.Category;

namespace ClercSystem.Services.Interfaces
{
    public interface ICategoryService
    {

        Task<List<AllCategoriesViewModel>> GetAllCategoriesAsync();


        //Task<EditCategoryViewModel?> GetCategoryByIdAsync(Guid id);
        //Task<bool> CreateCategoryAsync(CreateCategoryViewModel model);
        //Task<bool> UpdateCategoryAsync(EditCategoryViewModel model);
        //Task<CategoryMoreViewModel?> GetCategoryDetailsAsync(Guid id);
        //Task<(bool success, string? errorMessage)> DeleteCategoryAsync(Guid id);
        //bool IsValidGuid(string id);

    }
}
