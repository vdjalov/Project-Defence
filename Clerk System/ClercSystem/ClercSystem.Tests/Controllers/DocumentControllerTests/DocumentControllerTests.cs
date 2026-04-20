using ClercSystem.Controllers;
using ClercSystem.Services.Interfaces;
using ClercSystem.ViewModels.Document;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ClercSystem.Tests.Controllers.DocumentControllerTests
{
    public class DocumentControllerTests
    {
        private Mock<IDocumentService> documentServiceMock;
        private DocumentController controller;


        [SetUp]
        public void Setup()
        {
            this.documentServiceMock = new Mock<IDocumentService>();
            this.controller = new DocumentController(this.documentServiceMock.Object);

            this.controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TearDown]
        public void TearDown()
        {
            this.controller?.Dispose();
        }

        [Test]
        public async Task Index_ShouldReturnViewWithDocuments()
        {
            var resultData = new AllDocumentsResult
            {
                Docs = new List<AllDocumentsViewModel>
                {
                    new AllDocumentsViewModel(),
                    new AllDocumentsViewModel()
                },
                TotalCount = 10
            };


            this.documentServiceMock
             .Setup(s => s.GetAllDocumentsAsync("test", 1, 5))
             .ReturnsAsync(() => (resultData.Docs, resultData.TotalCount));

            IActionResult result = await this.controller.Index("test", 1, 5);

            ViewResult view = result as ViewResult;

            Assert.That(view, Is.Not.Null);
            Assert.That(view.Model, Is.EqualTo(resultData.Docs));
        }

        [Test]
        public async Task Create_Get_ShouldReturnView()
        {
            CreateDocumentViewModel model = new CreateDocumentViewModel();

            this.documentServiceMock
                .Setup(doc => doc.GetCreateModelAsync())
                .ReturnsAsync(model);

            IActionResult result = await this.controller.Create();

            ViewResult view = result as ViewResult;

            Assert.That(view, Is.Not.Null);
            Assert.That(view.Model, Is.EqualTo(model));
        }


        [Test]
        public async Task Create_Post_ShouldReturnView_WhenModelInvalid()
        {
            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            this.controller.ModelState.AddModelError("Title", "Required");

            var model = new CreateDocumentViewModel();

            IActionResult result = await this.controller.Create(model);

            ViewResult view = result as ViewResult;

            Assert.That(view, Is.Not.Null);
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Oops something went awire."));
        }


        [Test]
        public async Task Edit_Get_ShouldReturnView_WhenGuidInvalid()
        {
            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit("bad-id", "route");

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Invalid Id"));
        }


        [Test]
        public async Task Edit_Get_ShouldRedirect_WhenUserNotOwnerAndNotAdmin()
        {
            Guid id = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

          
            this.controller.HttpContext.User =
                new ClaimsPrincipal(new ClaimsIdentity());  // fake user

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentCreatorIsValid(id, userId))
                .ReturnsAsync(false);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(id.ToString(), "src");

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["Message"],
                Is.EqualTo("You do not have sufficient rights to work on this document or it does not exist."));
        }


        [Test]
        public async Task Edit_Get_ShouldRedirect_WhenDocumentNotFound()
        {
            Guid id = Guid.NewGuid();

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentCreatorIsValid(id, It.IsAny<Guid>()))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.GetEditModelAsync(id))
                .ReturnsAsync((EditDocumentViewModel)null);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(id.ToString(), "src");

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Document not found!"));
        }

        [Test]
        public async Task Edit_Get_ShouldReturnView_WhenValid()
        {
            Guid id = Guid.NewGuid();

            var model = new EditDocumentViewModel();

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentCreatorIsValid(id, It.IsAny<Guid>()))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.GetEditModelAsync(id))
                .ReturnsAsync(model);

            this.controller.HttpContext.User =
                new ClaimsPrincipal(new ClaimsIdentity(
                    new[] { new Claim("IsManager", "true") }
                ));

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(id.ToString(), "src");

            ViewResult view = result as ViewResult;

            Assert.That(view, Is.Not.Null);
            Assert.That(view.Model, Is.EqualTo(model));
        }


        [Test]
        public async Task Edit_Post_ShouldReturnView_WhenGuidInvalid()
        {
            var model = new EditDocumentViewModel();

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit("invalid-id", model, null);

            ViewResult view = result as ViewResult;

            Assert.That(view, Is.Not.Null);
            Assert.That(view.Model, Is.EqualTo(model));
            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Invalid Id"));
        }

        [Test]
        public async Task Edit_Post_ShouldReturnView_WhenModelStateInvalid()
        {
            var model = new EditDocumentViewModel();

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            this.controller.ModelState.AddModelError("Title", "Required");

            IActionResult result = await this.controller.Edit(Guid.NewGuid().ToString(), model, null);

            ViewResult view = result as ViewResult;

            Assert.That(view, Is.Not.Null);
            Assert.That(this.controller.TempData["ErrorMessage"],
                Is.EqualTo("Oops something went awire."));
        }
        
        [Test]
        public async Task Edit_Post_ShouldRedirect_WhenUserNotAuthorized()
        {
            Guid id = Guid.NewGuid();
            var model = new EditDocumentViewModel();

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentCreatorIsValid(id, It.IsAny<Guid>()))
                .ReturnsAsync(false);

            this.controller.HttpContext.User =
                new ClaimsPrincipal(new ClaimsIdentity()); // remove roles

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(id.ToString(), model, null);

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(this.controller.TempData["Message"],
                Is.EqualTo("You do not have sufficient rights to work on this document or it does not exist."));
        }

        [Test]
        public async Task Edit_Post_ShouldRedirect_WhenUpdateFails()
        {
            Guid id = Guid.NewGuid();
            var model = new EditDocumentViewModel();

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentCreatorIsValid(id, It.IsAny<Guid>()))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.EditDocumentAsync(id, false, model))
                .ReturnsAsync(false);

            this.controller.HttpContext.User =
                new ClaimsPrincipal(new ClaimsIdentity());

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(id.ToString(), model, null);

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(this.controller.TempData["Message"],
                Is.EqualTo("Document was not updated successfuly. You rights to edit this document might have been restricted."));
        }

        [Test]
        public async Task Edit_Post_ShouldRedirectToIndex_WhenSuccessful()
        {
            Guid id = Guid.NewGuid();
            var model = new EditDocumentViewModel();

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentCreatorIsValid(id, It.IsAny<Guid>()))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.EditDocumentAsync(id, false, model))
                .ReturnsAsync(true);

            this.controller.HttpContext.User =
                new ClaimsPrincipal(new ClaimsIdentity());

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(id.ToString(), model, null);

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Document updated successfully!"));
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));

        }

        [Test]
        public async Task Edit_Post_ShouldRedirectToAdmin_WhenSourceProvided()
        {
            Guid id = Guid.NewGuid();
            var model = new EditDocumentViewModel();

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentCreatorIsValid(id, It.IsAny<Guid>()))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.EditDocumentAsync(id, false, model))
                .ReturnsAsync(true);

            this.controller.HttpContext.User =
                new ClaimsPrincipal(new ClaimsIdentity());

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.Edit(id.ToString(), model, "admin");

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ControllerName, Is.EqualTo("DocumentManagement"));
            Assert.That(this.controller.TempData["Message"], Is.EqualTo("Document updated successfully!"));
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }


        [Test]
        public async Task More_ShouldRedirect_WhenGuidInvalid()
        {
            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.More("invalid-id", null);

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["ErrorMessage"], Is.EqualTo("Invalid Id"));
        }


        [Test]
        public async Task More_ShouldRedirect_If_DocumentDoes_not_Exist()
        {
            string documentId = Guid.NewGuid().ToString();

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentExists(Guid.Parse(documentId)))
                .ReturnsAsync(false);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.More(documentId, null);

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["ErrorMessage"], Is.EqualTo("Document not found."));
        }

        [Test]
        public async Task More_ShouldRedirect_If_DocumentDoes_Throws_BadRequest_IFDocuemnts_notFound()
        {
            string documentId = Guid.NewGuid().ToString();

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentExists(Guid.Parse(documentId)))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.GetDetailsAsync(Guid.Parse(documentId)))
                .ThrowsAsync(new Exception("Error"));

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.More(documentId, null);

            BadRequestObjectResult badRequest = result as BadRequestObjectResult;

            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest.Value, Is.EqualTo("Error"));
        }

        [Test]
        public async Task More_ShouldReturnView_IFSuccessful()
        {
            string documentId = Guid.NewGuid().ToString();
            var model = new DocumentMoreViewModel();

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentExists(Guid.Parse(documentId)))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.GetDetailsAsync(Guid.Parse(documentId)))
                .ReturnsAsync(model);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.More(documentId, "admin");

            ViewResult view = result as ViewResult;

            Assert.That(view, Is.Not.Null);
            Assert.That(view.Model, Is.EqualTo(model));
            Assert.That(this.controller.ViewBag.Source, Is.EqualTo("admin"));
        }


        [Test]
        public async Task SoftDelete_ShouldRedirect_WhenGuidInvalid()
        {
            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.SoftDelete("invalid", null, null);

            var redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["ErrorMessage"], Is.EqualTo("Invalid Id"));
        }

        [Test]
        public async Task SoftDelete_ShouldRedirect_WhenDocumentCreatorIsSomeoneElse()
        {
            Guid id = Guid.NewGuid();

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentExists(id))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentCreatorIsValid(id, It.IsAny<Guid>()))
                .ReturnsAsync(false);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.SoftDelete(id.ToString(), null, null);

            var redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["Message"], 
                Is.EqualTo("You do not have sufficient rights to work on this document or it does not exist.If you are an admin user you can restrict it."));
        }

        [Test]
        public async Task SoftDelete_ShouldRedirect_WhenDeleteFails()
        {
            Guid id = Guid.NewGuid();

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentExists(id))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentCreatorIsValid(id, It.IsAny<Guid>()))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.SoftDeleteAsync(id, It.IsAny<bool>()))
                .ReturnsAsync(false);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.SoftDelete(id.ToString(), null, null);

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(this.controller.TempData["Message"],
                Is.EqualTo("Document was not deleted successfully. You might not have sufficien rights to do it."));
        }


        [Test]
        public async Task SoftDelete_ShouldRedirectToIndex_WhenSuccessful()
        {
            Guid id = Guid.NewGuid();

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentExists(id))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentCreatorIsValid(id, It.IsAny<Guid>()))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.SoftDeleteAsync(id, It.IsAny<bool>()))
                .ReturnsAsync(true);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.SoftDelete(id.ToString(), null, null);

            var redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task SoftDelete_ShouldRedirectToReturnUrl_WhenProvided()
        {
            Guid id = Guid.NewGuid();
            string returnUrl = "some/url";

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentExists(id))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.CheckIfDocumentCreatorIsValid(id, It.IsAny<Guid>()))
                .ReturnsAsync(true);

            this.documentServiceMock
                .Setup(d => d.SoftDeleteAsync(id, It.IsAny<bool>()))
                .ReturnsAsync(true);

            this.controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            IActionResult result = await this.controller.SoftDelete(id.ToString(), returnUrl, null);

            var redirect = result as RedirectResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.Url, Is.EqualTo(returnUrl));
        }





















        //helper
        private class AllDocumentsResult
        {
            public List<AllDocumentsViewModel> Docs { get; set; }
            public int TotalCount { get; set; }
        }

    }
}
