using ClercSystem.Data.Models;
using ClercSystem.Infrastructure.Interfaces;
using ClercSystem.Services.Implementations;
using ClercSystem.ViewModels.Department;
using Moq;

namespace ClercSystem.Tests.Services.DepartmentServiceTests
{
    public class DepartmentServiceTests
    {

        private Mock<IDepartmentRepository> departmentRepositoryMock;
        private DepartmentService service;


        [SetUp]
        public void Setup()
        {
            this.departmentRepositoryMock = new Mock<IDepartmentRepository>();
            this.service = new DepartmentService(departmentRepositoryMock.Object);
        }


        [Test]
        public async Task CheckIfThereAreDocumentsAssociatedWithDepartmentAsync_ShouldReturnTrue_WhenDocumentsExist()
        {
            Guid departmentId = new Guid();

            Department department = new Department
            {
                Documents = new List<Document> { new Document() }
            };

            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(department);

            bool result = await service.CheckIfThereAreDocumentsAssociatedWithDepartmentAsync(departmentId.ToString());

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CheckIfThereAreDocumentsAssociatedWithDepartmentAsync_ShouldReturnFalse_WhenDocumentsDoNotExist()
        {
            Guid departmentId = new Guid();

            Department department = null;

            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(department);

            bool result = await service.CheckIfThereAreDocumentsAssociatedWithDepartmentAsync(departmentId.ToString());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CheckIfThereAreDocumentsAssociatedWithDepartmentAsync_ShouldReturnFalse_WhenDocumentsDocumentNull()
        {
            Guid departmentId = new Guid();

            Department department = new Department
            {
                Documents = new List<Document>()
            };

            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(department);

            bool result = await service.CheckIfThereAreDocumentsAssociatedWithDepartmentAsync(departmentId.ToString());

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CreateDepartmentAsync_ShouldReturnTrue_AndCallRepository()
        {
            var model = new CreateDepartmentViewModel
            {
                Name = "IT",
                Location = "Plovdiv"
            };

            bool result = await this.service.CreateDepartmentAsync(model);

            Assert.That(result, Is.True);

            this.departmentRepositoryMock.Verify(r => r.AddAndSaveAsync( // verifies if i have sent the department object, sent once and message correct 
                It.Is<Department>(d =>
                    d.Name == model.Name &&
                    d.Location == model.Location)),
                Times.Once);
        }

        [Test]
        public async Task DeleteDepartmentAsync_ShouldReturnTrue_WhenDeleteSucceeds()
        {
            var department = new Department
            {
                DepartmentId = Guid.NewGuid()
            };

            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(department);

            this.departmentRepositoryMock
                .Setup(dep => dep.DeleteAndSaveAsync(department))
                .ReturnsAsync(true);

            bool result = await service.DeleteDepartmentAsync(department.DepartmentId.ToString());

            Assert.That(result, Is.True);

            this.departmentRepositoryMock.Verify(r => r.DeleteAndSaveAsync(department), Times.Once);
        }

        [Test]
        public async Task DeleteDepartmentAsync_ShouldReturnFalse_WhenDepartmentIsNull()
        {
            string departmentId = Guid.NewGuid().ToString();
            Department department = null;

            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(department);

            bool result = await service.DeleteDepartmentAsync(departmentId);

            Assert.That(result, Is.False);

            this.departmentRepositoryMock.Verify(r => r.DeleteAndSaveAsync(department), Times.Never);
        }

        [Test]
        public async Task DeleteDepartmentAsync_ShouldReturnFalse_WhenDeleteFails()
        {
            var department = new Department
            {
                DepartmentId = Guid.NewGuid()
            };

            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(department);

            this.departmentRepositoryMock
                .Setup(dep => dep.DeleteAndSaveAsync(department))
                .ReturnsAsync(false);

            bool result = await service.DeleteDepartmentAsync(department.DepartmentId.ToString());

            Assert.That(result, Is.False);

            this.departmentRepositoryMock.Verify(r => r.DeleteAndSaveAsync(department), Times.Once);
        }

        [Test]
        public async Task DepartmentExistsAsync_ShouldReturnTrue_WhenRepositoryReturnsTrue()
        {
            this.departmentRepositoryMock
                .Setup(dep => dep.ExistsAsync("IT", "Plovdiv"))
                .ReturnsAsync(true);

            bool result = await service.DepartmentExistsAsync("IT", "Plovdiv");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DepartmentExistsAsync_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            this.departmentRepositoryMock
                .Setup(dep => dep.ExistsAsync("IT", "Plovdiv"))
                .ReturnsAsync(false);

            bool result = await service.DepartmentExistsAsync("IT", "Plovdiv");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DepartmentExistsByIdAsync_ShouldReturnTrue_WhenDepartmentExists()
        {
            Guid departmentId = Guid.NewGuid();
            Department department = new Department()
            {
                DepartmentId = departmentId
            };

            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(departmentId))
                .ReturnsAsync(department);

            bool result = await service.DepartmentExistsByIdAsync(departmentId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DepartmentExistsByIdAsync_ShouldReturnFalse_WhenDepartmentDoesNotExists()
        {
            Guid departmentId = Guid.NewGuid();
            Department department = null;

            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(departmentId))
                .ReturnsAsync(department);

            bool result = await service.DepartmentExistsByIdAsync(departmentId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditDepartmentAsync_ShouldReturnFalse_WhenModelIsNull()
        {
            bool result = await service.EditDepartmentAsync(null);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditDepartmentAsync_ShouldReturnFalse_WhenDepartmentNotFound()
        {
            var model = new EditDepartmentViewModel
            {
                DepartmentId = Guid.NewGuid(),
                Name = "It",
                Location = "Plovdiv"
            };

            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(model.DepartmentId))
                .ReturnsAsync((Department)null);

            var result = await service.EditDepartmentAsync(model);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task EditDepartmentAsync_ShouldReturnTrue_AndUpdateDepartment()
        {
            var departmentId = Guid.NewGuid();

            var existingDepartment = new Department
            {
                DepartmentId = departmentId,
                Name = "It",
                Location = "Plovdiv"
            };

            var model = new EditDepartmentViewModel
            {
                DepartmentId = departmentId,
                Name = "It",
                Location = "Plovdiv"
            };

            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(departmentId))
                .ReturnsAsync(existingDepartment);

            departmentRepositoryMock
                .Setup(dep => dep.UpdateAndSaveAsync(existingDepartment))
                .ReturnsAsync(true);

            bool result = await this.service.EditDepartmentAsync(model);

            Assert.That(result, Is.True);

            Assert.That(existingDepartment.Name, Is.EqualTo("It"));
            Assert.That(existingDepartment.Location, Is.EqualTo("Plovdiv"));

            this.departmentRepositoryMock.Verify(r =>
                r.UpdateAndSaveAsync(existingDepartment), // verifies databse entered only once
                Times.Once);
        }

        [Test]
        public void GetCreateModel_ShouldReturnCorrectType()
        {
            CreateDepartmentViewModel result = service.GetCreateModel();

            Assert.That(result, Is.TypeOf<CreateDepartmentViewModel>());
        }

        [Test]
        public async Task GetDepartmentDetailsAsync_ShouldReturnNull_WhenDepartmentNotFound()
        {
            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Department)null);

            DepartmentMoreViewModel result = await service.GetDepartmentDetailsAsync(Guid.NewGuid().ToString());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetDepartmentDetailsAsync_ShouldReturnMappedViewModel_WhenDepartmentExists()
        {
            Guid departmentId = Guid.NewGuid();

            Department department = new Department
            {
                DepartmentId = departmentId,
                Name = "IT",
                Location = "Plovdiv"
            };

            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(departmentId))
                .ReturnsAsync(department);

            DepartmentMoreViewModel result = await service.GetDepartmentDetailsAsync(departmentId.ToString());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("IT"));
            Assert.That(result.Location, Is.EqualTo("Plovdiv"));
        }

        [Test]
        public async Task GetEditModelAsync_ShouldReturnNull_WhenDepartmentNotFound()
        {
            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Department)null);

            EditDepartmentViewModel result = await service.GetEditModelAsync(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetEditModelAsync_ShouldReturnMappedViewModel_WhenDepartmentExists()
        {
            Guid departmentId = Guid.NewGuid();

            Department department = new Department
            {
                DepartmentId = departmentId,
                Name = "IT",
                Location = "Plovdiv"
            };

            this.departmentRepositoryMock
                .Setup(dep => dep.GetByIdAsync(departmentId))
                .ReturnsAsync(department);

            EditDepartmentViewModel result = await service.GetEditModelAsync(departmentId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.DepartmentId, Is.EqualTo(departmentId));
            Assert.That(result.Name, Is.EqualTo("IT"));
            Assert.That(result.Location, Is.EqualTo("Plovdiv"));
        }
    }
}
