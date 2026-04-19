using ClercSystem.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ClercSystem.Tests.HomeControllerTests
{
    public class HomeControllerTests
    {
        private HomeController controller;

        [SetUp]
        public void Setup()
        {
            this.controller = new HomeController();

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
        public void Index_ShouldReturnView_WhenUserNotAuthenticated()
        {
            // Arrange
            this.controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // user no identity

            // Act
            IActionResult result = this.controller.Index();

            // Assert
            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName, Is.EqualTo("Index"));
        }

        [Test]
        public void Index_ShouldRedirectToDocument_WhenUserAuthenticated()
        {
            ClaimsIdentity identity = new ClaimsIdentity("TestAuth");
            identity.AddClaim(new Claim(ClaimTypes.Name, "test-user")); // user authenticated

            this.controller.HttpContext.User = new ClaimsPrincipal(identity);

            IActionResult result = this.controller.Index();

            RedirectToActionResult redirect = result as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Document"));
        }

        [Test]
        public void Privacy_ShouldReturnView_Privacy()
        {
            IActionResult result = this.controller.Privacy();

            ViewResult viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ViewName, Is.EqualTo("Privacy"));
        }
    }
}
