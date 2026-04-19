using ClercSystem.Controllers;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Category;
using ClercSystem.ViewModels.Department;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace ClercSystem.Tests.Controllers.DepartmentControllerTests
{
    public class DepartmentControllerTests
    {
        private Mock<IDepartmentService> departmentServiceMock;
        private DepartmentController controller;

        [SetUp]
        public void Setup()
        {
            this.departmentServiceMock = new Mock<IDepartmentService>();
            this.controller = new DepartmentController(departmentServiceMock.Object);
        }


        [TearDown]
        public void TearDown()
        {
            this.controller?.Dispose();
        }


        [Test]
        public async Task Index_ShouldReturnViewWithDepartments()
        {
            var departments = new List<AllDepartmentsViewModel>
            {
                new AllDepartmentsViewModel(),
                new AllDepartmentsViewModel()
            };

            this.departmentServiceMock.Setup(s => s.GetAllDepartmentsAsync())
                        .ReturnsAsync(departments);

            IActionResult result = await this.controller.Index();

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(departments, Is.EqualTo(viewResult.Model));
        }

        [Test]
        public async Task Create_Get_Returns_Model_View()
        {
            CreateDepartmentViewModel model = new CreateDepartmentViewModel();

            this.departmentServiceMock
                .Setup(s => s.GetCreateModel())
                .Returns(model);

            IActionResult result = await this.controller.Create();

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }


        [Test]
        public async Task Create_Post_ShouldReturnView_WhenDepartmentExists()
        {
            CreateDepartmentViewModel model = new CreateDepartmentViewModel
            {
                Name = "HR",
                Location = "Sofia"
            };

            this.departmentServiceMock
                .Setup(s => s.DepartmentExistsAsync(model.Name, model.Location))
                .ReturnsAsync(true);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            IActionResult result = await this.controller.Create(model);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(model));
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("A department with the same name and location already exists."));
        }

        [Test]
        public async Task Create_Post_ShouldReturnView_WhenModelStateInvalid()
        {
            CreateDepartmentViewModel model = new CreateDepartmentViewModel
            {
                Name = "HR",
                Location = "Sofia"
            };

            this.controller.ModelState.AddModelError("Name", "Required");

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Create(model);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Please correct the errors in the form."));
        }

        [Test]
        public async Task Create_Post_ShouldRedirect_WhenSuccessful()
        {
            CreateDepartmentViewModel model = new CreateDepartmentViewModel
            {
                Name = "HR",
                Location = "Sofia"
            };

            this.departmentServiceMock
                .Setup(s => s.DepartmentExistsAsync(model.Name, model.Location))
                .ReturnsAsync(false);

            this.departmentServiceMock
                .Setup(s => s.CreateDepartmentAsync(model))
                .Returns(Task.CompletedTask);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Create(model);

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Department created successfully."));
        }


        [Test]
        public async Task Edit_Get_ShouldRedirect_WhenInvalidGuid()
        {
            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit("invalid");

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Invalid department ID."));
        }


        [Test]
        public async Task Edit_Get_ShouldRedirect_WhenDepartmentNotFound()
        {
           
            Guid id = Guid.NewGuid();

            this.departmentServiceMock
                .Setup(s => s.DepartmentExistsByIdAsync(id))
                .ReturnsAsync(false);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(id.ToString());

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Department does not exist."));
        }


        [Test]
        public async Task Edit_Get_ShouldRetrun_ViewResult()
        {
            EditDepartmentViewModel model = new EditDepartmentViewModel
            {
                Name = "HR",
                Location = "Sofia"
            };

            Guid id = Guid.NewGuid();

            this.departmentServiceMock
                .Setup(s => s.DepartmentExistsByIdAsync(id))
                .ReturnsAsync(true);

            this.departmentServiceMock
                .Setup(dep => dep.GetEditModelAsync(id))
                .ReturnsAsync(model);

            IActionResult result = await this.controller.Edit(id.ToString());

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.SameAs(model));
        }

        [Test]
        public async Task Edit_Post_ShouldReturnView_WhenDuplicateExists()
        {
            var model = new EditDepartmentViewModel
            {
                DepartmentId = Guid.NewGuid(),
                Name = "HR",
                Location = "Sofia"
            };

            this.departmentServiceMock
                .Setup(s => s.DepartmentExistsAsync(model.Name, model.Location))
                .ReturnsAsync(true);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(model);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("A department with the same name and location already exists."));
        }


        [Test]
        public async Task Edit_Post_ShouldReturnView_When_ModelState_Invalid()
        {
            var model = new EditDepartmentViewModel
            {
                DepartmentId = Guid.NewGuid(),
                Name = "HR",
                Location = "Sofia"
            };

            this.departmentServiceMock
                .Setup(dep => dep.DepartmentExistsAsync(model.Name, model.Location))
                .ReturnsAsync(false);

            this.controller.ModelState.AddModelError("Name", "Required");
            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(model);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Please correct the errors in the form."));
        }

        [Test]
        public async Task Edit_Post_ShouldReturnView_When_Department_DoesNotExistById_Invalid()
        {
            var model = new EditDepartmentViewModel
            {
                DepartmentId = Guid.NewGuid(),
                Name = "HR",
                Location = "Sofia"
            };

            this.departmentServiceMock
                .Setup(dep => dep.DepartmentExistsAsync(model.Name, model.Location))
                .ReturnsAsync(false);

            this.departmentServiceMock
                .Setup(dep => dep.DepartmentExistsByIdAsync(model.DepartmentId)) 
                .ReturnsAsync(false);
           
            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(model);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Department does not exist."));
        }

        [Test]
        public async Task Edit_Post_ShouldReturnView_When_Department_EditDepartmentAsync_Invalid()
        {
            var model = new EditDepartmentViewModel
            {
                DepartmentId = Guid.NewGuid(),
                Name = "HR",
                Location = "Sofia"
            };

            this.departmentServiceMock
                .Setup(dep => dep.DepartmentExistsAsync(model.Name, model.Location))
                .ReturnsAsync(false);

            this.departmentServiceMock
                .Setup(dep => dep.DepartmentExistsByIdAsync(model.DepartmentId))
                .ReturnsAsync(true);

            this.departmentServiceMock
                .Setup(dep => dep.EditDepartmentAsync(model))
                .ReturnsAsync(false);


            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(model);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("An error occurred while updating the department. Please try again."));
        }

        [Test]
        public async Task Edit_Post_ShouldReturnView_When_Department_Update_Success()
        {
            var model = new EditDepartmentViewModel
            {
                DepartmentId = Guid.NewGuid(),
                Name = "HR",
                Location = "Sofia"
            };

            this.departmentServiceMock
                .Setup(dep => dep.DepartmentExistsAsync(model.Name, model.Location))
                .ReturnsAsync(false);

            this.departmentServiceMock
                .Setup(dep => dep.DepartmentExistsByIdAsync(model.DepartmentId))
                .ReturnsAsync(true);

            this.departmentServiceMock
                .Setup(dep => dep.EditDepartmentAsync(model))
                .ReturnsAsync(true);


            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(model);

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Department updated successfully."));
        }

        [Test]
        public async Task More_GuidWrong_ShouldRedirectTo_Index()
        {

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.More("wrong-guid");

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null); // result should not be null but a view
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Invalid department ID."));
        }

        [Test]
        public async Task More_DepartmentNotFound_ShouldRedirectToIndex()
        {
            Guid id = Guid.NewGuid();

            DepartmentMoreViewModel model = new DepartmentMoreViewModel()
            {
                Name = "pulp",
                Location = "fiction",
            };

            this.departmentServiceMock
                .Setup(dep => dep.DepartmentExistsByIdAsync(id))
                .ReturnsAsync(false);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.More(id.ToString());

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null); // result should not be null but a view
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Department does not exist."));
        }

        [Test]
        public async Task More_DepartmentFound_Success()
        {
            Guid id = Guid.NewGuid();

            DepartmentMoreViewModel model = new DepartmentMoreViewModel()
            {
                Name = "pulp",
                Location = "fiction",
            };

            this.departmentServiceMock
                .Setup(dep => dep.DepartmentExistsByIdAsync(id))
                .ReturnsAsync(true);

            this.departmentServiceMock
                .Setup(dep => dep.GetDepartmentDetailsAsync(id.ToString()))
                .ReturnsAsync(model);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.More(id.ToString());

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null); // result should not be null but a view
            Assert.That(viewResult.Model, Is.SameAs(model));
        }

        [Test]
        public async Task Delete_ShouldRedirect_WhenGuidInvalid()
        {
            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Delete("Invalid-guid");

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Invalid department ID."));
        }

        [Test]
        public async Task Delete_ShouldRedirect_When_DepartmentDoesNotExist_ToIndex()
        {
            Guid id = Guid.NewGuid();

            this.departmentServiceMock
                .Setup(s => s.DepartmentExistsByIdAsync(id))
                .ReturnsAsync(false);


            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Delete(id.ToString());

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Department not found."));
        }

        [Test]
        public async Task Delete_ShouldRedirect_WhenHasDocuments()
        {
            Guid id = Guid.NewGuid();

            this.departmentServiceMock
                .Setup(s => s.DepartmentExistsByIdAsync(id))
                .ReturnsAsync(true);

            this.departmentServiceMock
                .Setup(s => s.CheckIfThereAreDocumentsAssociatedWithDepartmentAsync(id.ToString()))
                .ReturnsAsync(true);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Delete(id.ToString());

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Cannot delete department. It is associated with documents:"));
        }

        [Test]
        public async Task Delete_ShouldRedirect_ToIndex_When_Success()
        {
            Guid id = Guid.NewGuid();

            this.departmentServiceMock
                .Setup(dep => dep.DepartmentExistsByIdAsync(id))
                .ReturnsAsync(true);

            this.departmentServiceMock
                .Setup(dep => dep.CheckIfThereAreDocumentsAssociatedWithDepartmentAsync(id.ToString()))
                .ReturnsAsync(false);

            this.departmentServiceMock
                .Setup(dep => dep.DeleteDepartmentAsync(id.ToString()))
                .ReturnsAsync(true);
                
            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Delete(id.ToString());

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Department deleted successfully."));
        }

        [Test]
        public async Task Delete_ShouldRedirect_ToIndex_When_Unsuccessful()
        {
            Guid id = Guid.NewGuid();

            this.departmentServiceMock
                .Setup(dep => dep.DepartmentExistsByIdAsync(id))
                .ReturnsAsync(true);

            this.departmentServiceMock
                .Setup(dep => dep.CheckIfThereAreDocumentsAssociatedWithDepartmentAsync(id.ToString()))
                .ReturnsAsync(false);

            this.departmentServiceMock
                .Setup(dep => dep.DeleteDepartmentAsync(id.ToString()))
                .ReturnsAsync(false);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Delete(id.ToString());

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Unable to delete. Check if there is any documents associated with the department."));
        }

    }
}
