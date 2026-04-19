using ClercSystem.Controllers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom;

namespace ClercSystem.Tests.ErrorControllerTests
{
    public class ErrorControllerTests
    {
        private ErrorController controller;
        private DefaultHttpContext httpContext;
    
     [SetUp]
        public void Setup()
        {
            this.controller = new ErrorController(); // real instance of the controller

            this.httpContext = new DefaultHttpContext(); // simulates request response

            this.controller.ControllerContext = new ControllerContext // manualy inject controller contex and populate http context
            {
                HttpContext = this.httpContext
            };
        }

        [TearDown]
        public void TearDown()
        {
            this.controller?.Dispose();
        }


        [Test]
        public void Error500_ShouldWorkWithoutExceptionFeature()
        {
            // Act
            IActionResult result = this.controller.Error500();

            // Assert
            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName, Is.EqualTo("500"));
            Assert.That(this.httpContext.Response.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public void HandleStatusCode_404_ShouldReturn404View()
        {
            IActionResult result = this.controller.HandleStatusCode(404);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName, Is.EqualTo("404"));
            Assert.That(this.httpContext.Response.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public void HandleStatusCode_403_ShouldReturn403View()
        {
            IActionResult result = this.controller.HandleStatusCode(403);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult.ViewName, Is.EqualTo("403"));
            Assert.That(this.httpContext.Response.StatusCode, Is.EqualTo(403));
        }

        [Test]
        public void HandleStatusCode_401_ShouldReturn401View()
        {
            IActionResult result = this.controller.HandleStatusCode(401);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult.ViewName, Is.EqualTo("401"));
            Assert.That(this.httpContext.Response.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public void HandleStatusCode_400_ShouldReturn400View()
        {
            IActionResult result = this.controller.HandleStatusCode(400);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult.ViewName, Is.EqualTo("400"));
            Assert.That(this.httpContext.Response.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void HandleStatusCode_Default_ShouldReturnGenericErrorView()
        {
            IActionResult result = this.controller.HandleStatusCode(166);

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName, Is.EqualTo("Error"));
        }
    }

}
