using ClercSystem.Controllers;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace ClercSystem.Tests.CategoryControllertests
{
    public class CategoryControllerTests
    {
        private Mock<ICategoryService> categoryServiceMock;
        private CategoryController controller;


        [SetUp]
        public void Setup()
        {
            this.categoryServiceMock = new Mock<ICategoryService>();
            this.controller = new CategoryController(categoryServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            this.controller?.Dispose();
        }


        [Test]
        public async Task Index_ReturnsViewWithCategories()
        {
            List<AllCategoriesViewModel> categories = new List<AllCategoriesViewModel> // this is the expected result 
            {
                new AllCategoriesViewModel { CategoryName = "Test", }
            };  

            this.categoryServiceMock // when GetAllCategoriesAsync() is called returns the predefined categories list result 
                .Setup(c => c.GetAllCategoriesAsync())
                .ReturnsAsync(categories);

            IActionResult result = await this.controller.Index(); // colling the index controller returns IActionResult

            ViewResult viewResult = result as ViewResult; // result shoud be a view result so i cast it to a view result

            Assert.That(viewResult, Is.Not.Null); // verify that is that controller returns a view that is not null
            Assert.That(categories, Is.EqualTo(viewResult.Model)); // making shure the model passed is exactly what i expected 

        }





    }
}
