using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;
using ClercSystem.Services.Implementations;
using ClercSystem.ViewModels.Document;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClercSystem.Tests.Services.DocumentServiceTests
{
    public class DocumentServiceTests
    {
        private Mock<IDocumentRepository> documentRepositoryMock;
        private Mock<IDepartmentRepository> departmentRepositoryMock;
        private Mock<ICategoryRepository> categoryRepositoryMock;
        private Mock<IDocumentUserRepository> documentUserRepositoryMock;
        private Mock<IDocumentLogsRepository> documentLogsRepositoryMock;

        private DocumentService service;

        [SetUp]
        public void Setup()
        {
            this.documentRepositoryMock = new Mock<IDocumentRepository>();
            this.departmentRepositoryMock = new Mock<IDepartmentRepository>();
            this.categoryRepositoryMock = new Mock<ICategoryRepository>();
            this.documentUserRepositoryMock = new Mock<IDocumentUserRepository>();
            this.documentLogsRepositoryMock = new Mock<IDocumentLogsRepository>();

            this.service = new DocumentService(
                this.documentRepositoryMock.Object,
                this.departmentRepositoryMock.Object,
                this.categoryRepositoryMock.Object,
                this.documentUserRepositoryMock.Object,
                this.documentLogsRepositoryMock.Object
            );
        }


        [Test]
        public async Task CheckIfDocumentCreatorIsValid_ShouldReturnFalse_WhenDocumentIsNull()
        {
            Guid docId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            this.documentRepositoryMock
                .Setup(doc => doc.GetByIdAsync(docId))
                .ReturnsAsync((Document)null);

            bool result = await service.CheckIfDocumentCreatorIsValid(docId, userId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CheckIfDocumentCreatorIsValid_ShouldReturnFalse_WhenUserIsNotCreator()
        {
            var docId = Guid.NewGuid();

            this.documentRepositoryMock
                .Setup(r => r.GetByIdAsync(docId))
                .ReturnsAsync(new Document
                {
                    CreatedById = Guid.NewGuid() // different user
                });

            bool result = await service.CheckIfDocumentCreatorIsValid(docId, Guid.NewGuid());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CheckIfDocumentCreatorIsValid_ShouldReturnTrue_WhenUserIsCreator()
        {
            Guid userId = Guid.NewGuid();
            Guid docId = Guid.NewGuid();

            this.documentRepositoryMock
                .Setup(r => r.GetByIdAsync(docId))
                .ReturnsAsync(new Document
                {
                    CreatedById = userId
                });

            bool result = await service.CheckIfDocumentCreatorIsValid(docId, userId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CheckIfDocumentExists_ShouldReturnTrue_WhenDocumentExists()
        {
            Guid id = Guid.NewGuid();

            this.documentRepositoryMock
                .Setup(doc => doc.GetByIdAsync(id))
                .ReturnsAsync(new Document());

            bool result = await service.CheckIfDocumentExists(id);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CheckIfDocumentExists_ShouldReturnFalse_WhenDocumentDoesNotExists()
        {
            Guid id = Guid.NewGuid();

            documentRepositoryMock
                .Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync((Document)null);

            bool result = await service.CheckIfDocumentExists(id);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CreateDocumentAsync_ShouldReturnTrue_WhenAllOperationsSucceed()
        {
            CreateDocumentViewModel model = new CreateDocumentViewModel
            {
                Title = "Decision",
                FilePath = "file.pdf",
                Description = "Some",
                DepartmentId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString(),
                TimeToAnswer = "10",
                HasBeenAnswered = false
            };

            Guid userId = Guid.NewGuid();
            var date = DateTime.UtcNow;

            this.documentRepositoryMock
                .Setup(d => d.AddAndSaveAsync(It.IsAny<Document>()))
                .Returns(Task.CompletedTask);

            this.documentUserRepositoryMock
                .Setup(du => du.AddAndSaveAsync(It.IsAny<DocumentUser>()))
                .Returns(Task.CompletedTask);

            this.documentLogsRepositoryMock
                .Setup(dl => dl.AddAndSaveAsync(It.IsAny<DocumentLog>()))
                .ReturnsAsync(true);

            var result = await service.CreateDocumentAsync(model, userId, date);

            Assert.That(result, Is.True);

            this.documentRepositoryMock.Verify(r =>
                r.AddAndSaveAsync(It.IsAny<Document>()), Times.Once);

            this.documentUserRepositoryMock.Verify(r =>
                r.AddAndSaveAsync(It.IsAny<DocumentUser>()), Times.Once);

            this.documentLogsRepositoryMock.Verify(r =>
                r.AddAndSaveAsync(It.IsAny<DocumentLog>()), Times.Once);
        }

        [Test]
        public async Task CreateDocumentAsync_ShouldReturnFalse_WhenExceptionIsThrown()
        {
            CreateDocumentViewModel model = new CreateDocumentViewModel
            {
                DepartmentId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString()
            };

            this.documentRepositoryMock
                .Setup(r => r.AddAndSaveAsync(It.IsAny<Document>()))
                .ThrowsAsync(new Exception("DB error"));

            bool result = await service.CreateDocumentAsync(model, Guid.NewGuid(), DateTime.UtcNow);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditDocumentAsync_ShouldReturnFalse_WhenDocumentIsNull()
        {
            Guid docId = Guid.NewGuid();

            this.documentRepositoryMock
                .Setup(r => r.GetByIdAsync(docId))
                .ReturnsAsync((Document)null);

            var result = await service.EditDocumentAsync(
                docId,
                false,
                new EditDocumentViewModel());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditDocumentAsync_ShouldReturnFalse_WhenDocumentUserIsNull()
        {
            Guid docId = Guid.NewGuid();
            Document document = new Document()
            {
                Title = "Decision",
                FilePath = "file.pdf",
                Description = "Some",
                DepartmentId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                TimeToAnswer = "10",
                HasBeenAnswered = false
            };

            this.documentRepositoryMock
                .Setup(d => d.GetByIdAsync(docId))
                .ReturnsAsync(document);

            this.documentUserRepositoryMock
                .Setup(du => du.GetByIdAsync(It.IsAny<Guid>(), docId))
                .ReturnsAsync((DocumentUser)null);

            EditDocumentViewModel model = new EditDocumentViewModel()
            {
                DepartmentId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString(),
            };

            bool result = await service.EditDocumentAsync(docId, false, model);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditDocumentAsync_ShouldReturnFalse_WhenUserHasReadPermission()
        {
            var docId = Guid.NewGuid();

            this.documentRepositoryMock
                .Setup(d => d.GetByIdAsync(docId))
                .ReturnsAsync(new Document
                {
                    Id = docId,
                    CreatedById = Guid.NewGuid()
                });

            this.documentUserRepositoryMock
                .Setup(du => du.GetByIdAsync(It.IsAny<Guid>(), docId))
                .ReturnsAsync(new DocumentUser
                {
                    Permission = PermissionType.Read
                });

            EditDocumentViewModel model = new EditDocumentViewModel()
            {
                DepartmentId = Guid.NewGuid().ToString(),
                CategoryId = Guid.NewGuid().ToString(),
            };

            bool result = await service.EditDocumentAsync(docId, false, model);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditDocumentAsync_ShouldReturnTrue_WhenAdminUpdatesSuccessfully()
        {
            var docId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var document = new Document
            {
                Id = docId,
                CreatedById = userId
            };

            this.documentRepositoryMock
                .Setup(d => d.GetByIdAsync(docId))
                .ReturnsAsync(document);

            this.documentUserRepositoryMock
                .Setup(du => du.GetByIdAsync(userId, docId))
                .ReturnsAsync(new DocumentUser
                {
                    Permission = PermissionType.Read
                });

            this.documentRepositoryMock
                .Setup(r => r.UpdateAndSaveAsync(It.IsAny<Document>()))
                .ReturnsAsync(true);

            this.documentUserRepositoryMock
                .Setup(du => du.UpdateAndSaveAsync(It.IsAny<DocumentUser>()))
                .ReturnsAsync(true);

            this.documentLogsRepositoryMock
                .Setup(dl => dl.AddAndSaveAsync(It.IsAny<DocumentLog>()))
                .ReturnsAsync(true);

            var result = await service.EditDocumentAsync(docId, true, new EditDocumentViewModel
                {
                    DepartmentId = Guid.NewGuid().ToString(),
                    CategoryId = Guid.NewGuid().ToString(),
                    PermissionType = "Full"
                });

            Assert.That(result, Is.True);
        }

        


    }
}
