using PersonalPhotos.Controllers;
using Xunit;
using Moq;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalPhotos.Models;
using System.Threading.Tasks;
using Core.Models;

namespace PersonalPhotos.Test
{
    public class LoginsTests
    {
        private readonly LoginsController _controller;
        private readonly Mock<ILogins> _logins;
        private readonly Mock<IHttpContextAccessor> _accessor;

        //constructor
        public LoginsTests()
        {
            _logins = new Mock<ILogins>();

            _accessor = new Mock<IHttpContextAccessor>();
            var session = Mock.Of<ISession>();
            var httpContext = Mock.Of<HttpContext>(x => x.Session == session);
                        
            _accessor.Setup(x => x.HttpContext).Returns(httpContext);

            _controller = new LoginsController(_logins.Object, _accessor.Object);
        }

        [Fact]
        public void Index_GivenNorReturnUrl_ReturnLoginView2()
        {
            var result = _controller.Index();

            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ViewResult>(result);
            //casting to ViewResult, if fails is null
            var result2 = (_controller.Index() as ViewResult);
            Assert.NotNull(result2);
            //asserting for the ViewName, is Login
            Assert.Equal("Login", result2.ViewName, ignoreCase: true);
        }


        [Fact]
        public void Index_GivenNorReturnUrl_ReturnLoginView()
        {
            var result = (_controller.Index() as ViewResult);

            Assert.NotNull(result);
            Assert.Equal("Login", result.ViewName, ignoreCase: true);
        }

        //Is loguin modelState is invalid, then return Loguin View, user is not logged in
        [Fact]
        public async Task Login_GivenModelStateInvalid_ReturnLoginView()
        {
            _controller.ModelState.AddModelError("Test", "Test");

            var result = await _controller.Login(Mock.Of<LoginViewModel>()) as ViewResult;
            Assert.Equal("Login", result.ViewName, ignoreCase: true);
        }

        //Testing password
        [Fact]
        public async Task Login_GivenCorrectPassword_RedirectToDisplayAction()
        {
            const string password = "123"; //test
            var modelView = Mock.Of<LoginViewModel>(x=> x.Email == "mike@gmail.com" && x.Password== password);
            var model = Mock.Of<User>(x=> x.Password == password);

            _logins.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(model);

            var result = await _controller.Login(modelView);

            Assert.IsType<RedirectToActionResult>(result);
            var action = result as RedirectToActionResult;
            Assert.Equal("Display", action.ActionName);
            Assert.Equal("Photos", action.ControllerName);
        }

        //Testing wrong password
        [Fact]
        public async Task Login_GivenWrongPassword_ReturnloguinView()
        {
            const string password = "12345"; //test
            //data passed
            var modelView = Mock.Of<LoginViewModel>(x => x.Email == "mike@gmail.com" && x.Password == "123");
            //data stored
            var model = Mock.Of<User>(x => x.Password == password);

            _logins.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(model);

            var result = await _controller.Login(modelView);

            Assert.IsType<ViewResult>(result);
            var result2 = (result as ViewResult);
            Assert.NotNull(result2);
            //asserting for the ViewName, is Login
            Assert.Equal("Login", result2.ViewName, ignoreCase: true);

        }

        //Testing wrong user
        [Fact]
        public async Task Login_GivenWrongUserReturnloguinView()
        {
            const string password = "123"; //test
            var modelView = Mock.Of<LoginViewModel>(x => x.Email == "esme@gmail.com" && x.Password == password);
            var model = Mock.Of<User>(x => x.Password == password);

            _logins.Setup(x => x.GetUser("mike@gmail.com")).ReturnsAsync(model);

            var result = await _controller.Login(modelView);

            Assert.IsType<ViewResult>(result);
            var result2 = (result as ViewResult);
            Assert.NotNull(result2);
            //asserting for the ViewName, is Login
            Assert.Equal("Login", result2.ViewName, ignoreCase: true);

        }
    }
}
