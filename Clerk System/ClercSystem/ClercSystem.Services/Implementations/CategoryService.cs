using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        
        public Task<List<AllCategoriesViewModel>> GetAllCategoriesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
