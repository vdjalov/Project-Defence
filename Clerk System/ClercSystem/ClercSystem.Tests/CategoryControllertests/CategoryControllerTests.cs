using ClercSystem.Controllers;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

        [Test]
        public async Task Index_ReturnsModeCreateView()
        {
           
            IActionResult result = await this.controller.Create();

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.InstanceOf<CreateCategoryViewModel>());
        }


        [Test]
        public async Task Create_ReturnsCategoryAlreadyExists()
        {
            var model = new CreateCategoryViewModel
            {
                CategoryName = "Cars",
                Description = "Exists"
            };

            this.categoryServiceMock
                .Setup(cat => cat.CategoryExistsAsync(model.CategoryName, model.Description))
                .ReturnsAsync(true);

            IActionResult result = await this.controller.Create(model);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task Create_ReturnsCategoryWasNotCreated()
        {
            CreateCategoryViewModel model = new CreateCategoryViewModel
            {
                CategoryName = "Cars",
                Description = "Exists"
            };

            this.categoryServiceMock
                .Setup(cat => cat.CreateCategoryAsync(model))
                .ReturnsAsync(false);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(),Mock.Of<ITempDataProvider>());

            IActionResult result = await this.controller.Create(model);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null); // result should not be null but a view 
            Assert.That(viewResult.Model, Is.SameAs(model)); // result modal should be same as modal
            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Category was not created.")); // controller tempdata message
        }

        [Test]
        public async Task Create_ReturnsCategoryCreated()
        {
            CreateCategoryViewModel model = new CreateCategoryViewModel
            {
                CategoryName = "Cars",
                Description = "Exists"
            };

            this.categoryServiceMock
            .Setup(s => s.CategoryExistsAsync(model.CategoryName, model.Description))
            .ReturnsAsync(false);

            this.categoryServiceMock
                .Setup(cat => cat.CreateCategoryAsync(model))
                .ReturnsAsync(true);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            IActionResult result = await this.controller.Create(model);

            RedirectToActionResult redirectResult = result as RedirectToActionResult;

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Category was created successfully."));
        }

        [Test]
        public async Task Edit_Get_InvalidGuid_ShouldRedirectToIndex()
        {
            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await this.controller.Edit("guid");
            
            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Invalid department ID."));
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Edit_Get_Category_Does_not_Exist_ShouldRedirectToIndex()
        {

            this.categoryServiceMock.Setup(cat => cat.CategoryExistsByIdAsync("3a5a09be-5ca2-4c43-87d4-5fcea87b7b00"))
                        .ReturnsAsync(false); // returns false so hits checkpoint

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await this.controller.Edit("3a5a09be-5ca2-4c43-87d4-5fcea87b7b00"); // real guid 

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Category not found."));
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }


        [Test]
        public async Task Edit_Get_Category_Created()
        {
           

            this.categoryServiceMock.Setup(cat => cat.CategoryExistsByIdAsync("3a5a09be-5ca2-4c43-87d4-5fcea87b7b00"))
                        .ReturnsAsync(true);

             
              this.categoryServiceMock
                .Setup(cat => cat.GetCategoryForEditByIdAsync("3a5a09be-5ca2-4c43-87d4-5fcea87b7b00"))
                .ReturnsAsync(new EditCategoryViewModel());

            var result = await this.controller.Edit("3a5a09be-5ca2-4c43-87d4-5fcea87b7b00");

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult.Model, Is.Not.Null); // result should not be null but a view 
        }

        [Test]
        public async Task Edit_Post_InvalidGuid_ShouldRedirectToIndex()
        {
            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await this.controller.Edit("guid");

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Invalid department ID."));
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }


        [Test]
        public async Task Edit_Post_Category_Does_not_Exist_ShouldRedirectToIndex() // hanging on existbyidasync check 
        {
            EditCategoryViewModel model = new EditCategoryViewModel
            {
                Id = Guid.Parse("3a5a09be-5ca2-4c43-87d4-5fcea87b7b00"),
                CategoryName = "Cars",
                Description = "Exists"
            };

            this.categoryServiceMock.Setup(cat => cat.CategoryExistsAsync("pulp", "fiction"))
                        .ReturnsAsync(true); // returns false so hits checkpoint

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await this.controller.Edit(model); // real guid 

            RedirectToActionResult redirect = result as RedirectToActionResult; 

            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Category not found."));
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));

        }

        [Test]
        public async Task Edit_Post_Category_Does_not_Exist_CategoryWasNotUpdated() // hanging on existbyidasync check 
        {
            EditCategoryViewModel model = new EditCategoryViewModel
            {
                Id = Guid.Parse("3a5a09be-5ca2-4c43-87d4-5fcea87b7b00"),
                CategoryName = "Cars",
                Description = "Exists"
            };

            this.categoryServiceMock.Setup(cat => cat.CategoryExistsAsync("pulp", "fiction"))
                        .ReturnsAsync(false); // returns false so hits checkpoint

            this.categoryServiceMock.Setup(cat => cat.CategoryExistsByIdAsync("3a5a09be-5ca2-4c43-87d4-5fcea87b7b00"))
                        .ReturnsAsync(true);

            this.categoryServiceMock.Setup(cat => cat.UpdateCategoryAsync(model))
                        .ReturnsAsync(false);    

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await this.controller.Edit(model); // real guid 

            ViewResult viewResult = result as ViewResult;


            
            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Category was not updated."));
            Assert.That(viewResult.Model, Is.Not.Null); // result should not be null but a view 

        }

        [Test]
        public async Task Edit_Post_Category_Does_not_Exist_CategoryWasUpdatedSuccess() // hanging on existbyidasync check 
        {
            EditCategoryViewModel model = new EditCategoryViewModel
            {
                Id = Guid.Parse("3a5a09be-5ca2-4c43-87d4-5fcea87b7b00"),
                CategoryName = "Cars",
                Description = "Exists"
            };

            this.categoryServiceMock.Setup(cat => cat.CategoryExistsAsync("pulp", "fiction"))
                        .ReturnsAsync(false); // returns false so hits checkpoint

            this.categoryServiceMock.Setup(cat => cat.CategoryExistsByIdAsync("3a5a09be-5ca2-4c43-87d4-5fcea87b7b00"))
                        .ReturnsAsync(true);

            this.categoryServiceMock.Setup(cat => cat.UpdateCategoryAsync(model))
                        .ReturnsAsync(true);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = await this.controller.Edit(model); // real guid 

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Category was updated successfully."));
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task More_CategoryNotFound_ShouldRedirectToIndex()
        {
            string id = Guid.NewGuid().ToString();

            this.categoryServiceMock
                .Setup(s => s.CategoryExistsByIdAsync(id))
                .ReturnsAsync(false);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var result = await this.controller.More(id);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());

            var redirect = result as RedirectToActionResult;

            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Category not found."));
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }


        [Test]
        public async Task More_CategoryNotFound_ShouldReturnMoreView()
        {
            string id = Guid.NewGuid().ToString();

            CategoryMoreViewModel model = new CategoryMoreViewModel()
            {

                CategoryName = "pulp",
                Description = "fiction",
            };

            this.categoryServiceMock
                .Setup(cat => cat.CategoryExistsByIdAsync(id))
                .ReturnsAsync(true);

            this.categoryServiceMock
                 .Setup(cat => cat.GetCategoryDetailsByIdAsync(id))
                 .ReturnsAsync(model);

            
            IActionResult result = await this.controller.More(id);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult.Model, Is.Not.Null); // result should not be null but a view
            Assert.That(viewResult.Model, Is.SameAs(model));
        }

        [Test]
        public async Task Delete_CategoryHasDocuments_ShouldRedirectToIndex()
        {
            string id = Guid.NewGuid().ToString();

            this.categoryServiceMock
                .Setup(cat => cat.CategoryExistsByIdAsync(id))
                .ReturnsAsync(true);

            this.categoryServiceMock
                .Setup(cat => cat.CheckIfThereIsAnyDocumentWithThisCategoryAsync(id))
                .ReturnsAsync(true);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var result = await this.controller.Delete(id);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Cannot delete category with associated documents."));
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Delete_CategoryHasDocuments_ShouldReturnSuccess()
        {
            string id = Guid.NewGuid().ToString();

            this.categoryServiceMock
                .Setup(cat => cat.CategoryExistsByIdAsync(id))
                .ReturnsAsync(true);

            this.categoryServiceMock
                .Setup(cat => cat.CheckIfThereIsAnyDocumentWithThisCategoryAsync(id))
                .ReturnsAsync(false);

            this.categoryServiceMock
                .Setup(cat => cat.DeleteCategoryAsync(id))
                .ReturnsAsync((true, "has been deleted"));

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var result = await this.controller.Delete(id);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(this.controller.TempData["Message"], Is.EqualTo("has been deleted"));
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }
    }
}
