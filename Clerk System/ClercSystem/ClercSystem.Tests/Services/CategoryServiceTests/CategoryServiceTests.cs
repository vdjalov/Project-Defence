using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;
using ClercSystem.Services.Implementations;
using ClercSystem.ViewModels.Category;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Tests.Services.CategoryServiceTests
{
    public class CategoryServiceTests
    {
        private Mock<ICategoryRepository> categoryRepositoryMock;
        private CategoryService service;

        [SetUp]
        public void Setup()
        {
            this.categoryRepositoryMock = new Mock<ICategoryRepository>();
            this.service = new CategoryService(this.categoryRepositoryMock.Object);
        }


        [Test]
        public async Task CategoryExistsAsync_ShouldReturnFalse_WhenItDoesNotExists()
        {
            this.categoryRepositoryMock
                .Setup(c => c.GetByNameAndDescriptionAsync("IT", "false"))
                .ReturnsAsync((Category)null);

            bool result = await this.service.CategoryExistsAsync("IT", "false");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CategoryExistsAsync_ShouldReturnTrue_WhenExists()
        {
            this.categoryRepositoryMock
                .Setup(c => c.GetByNameAndDescriptionAsync("name", "true"))
                .ReturnsAsync(new Category());

            bool result = await this.service.CategoryExistsAsync("name", "true");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CategoryExistsByIdAsync_ShouldReturnFalse_WhenItDoesNotExists()
        {
            Guid id = Guid.NewGuid();

            this.categoryRepositoryMock
                .Setup(c => c.GetByIdAsync(id))
                .ReturnsAsync((Category)null);

            bool result = await this.service.CategoryExistsByIdAsync(id.ToString());

            Assert.That(result, Is.False);
        }


        [Test]
        public async Task CategoryExistsByIdAsync_ShouldReturnTrue_WhenExists()
        {
            Guid id = Guid.NewGuid();

            this.categoryRepositoryMock
                .Setup(c => c.GetByIdAsync(id))
                .ReturnsAsync(new Category());

            bool result = await this.service.CategoryExistsByIdAsync(id.ToString());

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CheckAssociatedDocumentsWithThisCategory_ShouldReturnFalse_WhenItDoesNotExists()
        {
            Guid id = Guid.NewGuid();
            Category category = new Category()
            {
                Documents = new List<Document> { new Document() }
            };


            this.categoryRepositoryMock
                .Setup(c => c.GetByIdAsync(id))
                .ReturnsAsync(category);

            bool result = await this.service.CheckIfThereIsAnyDocumentWithThisCategoryAsync(id.ToString());

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CheckAssociatedDocumentsWithThisCategory_ShouldReturn_True_Exists()
        {
            Guid id = Guid.NewGuid();
            Category category = new Category() { };


            this.categoryRepositoryMock
                .Setup(c => c.GetByIdAsync(id))
                .ReturnsAsync(category);

            bool result = await this.service.CheckIfThereIsAnyDocumentWithThisCategoryAsync(id.ToString());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CreateCategoryAsync_ShouldReturn_if_It_does_goThrough()
        {
            CreateCategoryViewModel model = new CreateCategoryViewModel
            {
                CategoryName = "Cat",
                Description = "IT"
            };

            this.categoryRepositoryMock
            .Setup(r => r.AddAndSaveAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

            var result = await this.service.CreateCategoryAsync(model);

            Assert.That(result, Is.True);

        }
        [Test]
        public async Task CreateCategoryAsync_ShouldReturns_FALSE_if_It_does_NOT_goThrough()
        {
            CreateCategoryViewModel model = new CreateCategoryViewModel
            {
                CategoryName = "Cat",
                Description = "IT"
            };

            this.categoryRepositoryMock
            .Setup(r => r.AddAndSaveAsync(It.IsAny<Category>()))
            .ThrowsAsync(new Exception());

            bool result = await this.service.CreateCategoryAsync(model);

            Assert.That(result, Is.False);
        }

        [Test] // not working some issue with IQueryable.....try to fix
        public async Task GetAllCategoriesAsync_Should_Return_List_Does_Not_Work_with_IQueriable()
        {
            IQueryable<Category> categories = new List<Category>() {

                new Category {Id = new Guid(), CategoryName = "IT", Description = "Nerds with brooms."},
                new Category {Id = new Guid(), CategoryName = "Accounting", Description = "Nerds with brooms."},
                new Category {Id = new Guid(), CategoryName = "TRZ", Description = "Nerds with brooms."},
            }.AsQueryable();


            this.categoryRepositoryMock
                .Setup(cat => cat.GetAll())
                .Returns(categories);

            List<AllCategoriesViewModel> result = await this.service.GetAllCategoriesAsync();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That (result, Is.EquivalentTo(categories));
        }


        [Test]
        public async Task GetCategoryForEditByIdAsync_ShouldMapCorrectly()
        {
            Guid id = Guid.NewGuid();

            Category category = new Category
            {
                Id = id,
                CategoryName = "IT",
                Description = "Nerds with brooms."
            };

            this.categoryRepositoryMock
                .Setup(cat => cat.GetByIdAsync(id))
                .ReturnsAsync(category);

            EditCategoryViewModel result = await this.service.GetCategoryForEditByIdAsync(id.ToString());

            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.CategoryName, Is.EqualTo("IT"));
        }

        [Test]
        public async Task GetCategoryForDetailsByIdAsync_ShouldMapCorrectly()
        {
            Guid id = Guid.NewGuid();

            Category category = new Category
            {
                Id = id,
                CategoryName = "IT",
                Description = "Nerds with brooms."
            };

            this.categoryRepositoryMock
                .Setup(cat => cat.GetByIdAsync(id))
                .ReturnsAsync(category);

            EditCategoryViewModel result = await this.service.GetCategoryForEditByIdAsync(id.ToString());

            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.CategoryName, Is.EqualTo("IT"));
        }

        [Test]
        public async Task UpdateCategoryAsync_ShouldReturnTrue_WhenUpdated()
        {
            var model = new EditCategoryViewModel
            {
                Id = Guid.NewGuid(),
                CategoryName = "Id",
                Description = "Nerds with brooms."
            };

            this.categoryRepositoryMock
                .Setup(cat => cat.GetByIdAsync(model.Id))
                .ReturnsAsync(new Category());

            this.categoryRepositoryMock
                .Setup(cat => cat.UpdateAndSaveAsync(It.IsAny<Category>()))
                .ReturnsAsync(true);

            var result = await this.service.UpdateCategoryAsync(model);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UpdateCategoryAsync_ShouldReturnFalse_WhenUpdated()
        {
            var model = new EditCategoryViewModel
            {
                Id = Guid.NewGuid(),
                CategoryName = "Id",
                Description = "Nerds with brooms."
            };

            this.categoryRepositoryMock
                .Setup(cat => cat.GetByIdAsync(model.Id))
                .ReturnsAsync(new Category());

            this.categoryRepositoryMock
                .Setup(cat => cat.UpdateAndSaveAsync(It.IsAny<Category>()))
                .ReturnsAsync(false);

            var result = await this.service.UpdateCategoryAsync(model);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteCategoryAsync_ShouldReturnSuccessTuple_WhenDeleted()
        {
            Guid id = Guid.NewGuid();

            var category = new Category
            {
                Id = id,
                CategoryName = "It",
                Description = "Nerds with brooms."
            };

            this.categoryRepositoryMock
                .Setup(cat => cat.GetByIdAsync(id))
                .ReturnsAsync(category);

            this.categoryRepositoryMock
                .Setup(cat => cat.DeleteAndSaveAsync(category))
                .ReturnsAsync(true);

            var result = await this.service.DeleteCategoryAsync(id.ToString());

            Assert.That(result.success, Is.True);
            Assert.That(result.errorMessage, Does.Contain("has been deleted"));
        }

        [Test]
        public async Task DeleteCategoryAsync_ShouldReturnSuccessTuple_WhenNOTDeleted()
        {
            Guid id = Guid.NewGuid();

            var category = new Category
            {
                Id = id,
                CategoryName = "It",
                Description = "Nerds with brooms."
            };

            this.categoryRepositoryMock
                .Setup(cat => cat.GetByIdAsync(id))
                .ReturnsAsync(category);

            this.categoryRepositoryMock
                .Setup(cat => cat.DeleteAndSaveAsync(category))
                .ReturnsAsync(false);

            var result = await this.service.DeleteCategoryAsync(id.ToString());

            Assert.That(result.success, Is.False);
            Assert.That(result.errorMessage, Does.Contain("was NOT deleted"));
        }
    }
}
