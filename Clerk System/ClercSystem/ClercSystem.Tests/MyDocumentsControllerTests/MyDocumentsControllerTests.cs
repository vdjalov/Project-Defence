using ClercSystem.Controllers;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.MyDocuments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;


namespace ClercSystem.Tests.MyDocumentsControllerTests
{
    public class MyDocumentsControllerTests
    {
        private Mock<IMyDocumentUserService> myDocumentServiceMock;
        private MyDocumentsController controller;
    

        [SetUp]
        public void Setup()
        {
            this.myDocumentServiceMock = new Mock<IMyDocumentUserService>();
            this.controller = new MyDocumentsController( this.myDocumentServiceMock.Object);

            this.controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>());
        }

        [TearDown]
        public void TearDown()
        {
            this.controller?.Dispose();
        }

        [Test]
        public async Task Index_ShouldReturnViewWithUserDocuments()
        {
            Guid userId = Guid.NewGuid();

            List<MyDocumentsViewModel> documents = new List<MyDocumentsViewModel>
        {
            new MyDocumentsViewModel(),
            new MyDocumentsViewModel()
        };

            this.myDocumentServiceMock
                .Setup(s => s.GetMyDocumentsAsync(userId))
                .ReturnsAsync(documents);

            // mock BaseController behavior
            this.controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Fake user ID (depends on your BaseController implementation!)
            this.controller.ControllerContext.HttpContext.User = TestClaimsPrincipalFactory.Create(userId);


            IActionResult result = await this.controller.Index();

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(documents));
        }


        // helper class to provide with fake Authenticated user id
        private static class TestClaimsPrincipalFactory
        {
            public static ClaimsPrincipal Create(Guid userId)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

                var identity = new ClaimsIdentity(claims, "TestAuth");
                return new ClaimsPrincipal(identity);
            }
        }
    }
 }
