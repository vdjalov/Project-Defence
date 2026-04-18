using ClercSystem.Controllers;
using ClercSystem.Services.Interfaces;
using Moq;

namespace ClercSystem.Tests.DepartmentControllerTests
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
    }
}
