using InsureGo_MVC.Controllers;
using InsureGo_MVC.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UnitTest
{
    [TestClass]
    public class AccountControllerTests
    {
        private AccountController _controller;

        [TestInitialize]
        public void Setup()
        {
            _controller = new AccountController();
            ControllerTestHelper.SetFakeControllerContext(_controller);
        }


        // TEST 1 : GET Login

        [TestMethod]
        public void Login_Get_Returns_View()
        {
            // Act
            var result = _controller.Login() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }


        // TEST 2 : POST Login - Invalid Model

        [TestMethod]
        public async Task Login_Post_InvalidModel_Returns_View()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");

            var model = new LoginViewModel
            {
                EmailId = "",
                Password = ""
            };

            // Act
            var result = await _controller.Login(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }


        // TEST 3 : POST Login - Invalid Credentials

        [TestMethod]
        public async Task Login_Post_InvalidCredentials_Returns_View_With_Error()
        {
            var model = new LoginViewModel
            {
                EmailId = "wrong@test.com",
                Password = "wrong123"
            };

            var result = await _controller.Login(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
            Assert.IsNotNull(result.ViewBag.Error);
        }


    }
}
