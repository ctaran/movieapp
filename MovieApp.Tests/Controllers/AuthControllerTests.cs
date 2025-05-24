using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MovieApp.API.Controllers;
using MovieApp.Application.DTOs;
using MovieApp.Application.Services;
using Xunit;

namespace MovieApp.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
        {
            // Arrange
            var request = new RegisterRequest { Email = "test@example.com", Password = "ValidPass123!", ConfirmPassword = "ValidPass123!" };
            var result = new AuthResult { Success = true, Token = "token", Errors = new string[0] };
            _authServiceMock.Setup(x => x.RegisterAsync(request)).ReturnsAsync(result);

            // Act
            var response = await _controller.Register(request);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            var okResult = response.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var request = new RegisterRequest { Email = "test@example.com", Password = "ValidPass123!", ConfirmPassword = "ValidPass123!" };
            var result = new AuthResult { Success = false, Token = null, Errors = new[] { "Error" } };
            _authServiceMock.Setup(x => x.RegisterAsync(request)).ReturnsAsync(result);

            // Act
            var response = await _controller.Register(request);

            // Assert
            response.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = response.Result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenLoginSucceeds()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@example.com", Password = "ValidPass123!" };
            var result = new AuthResult { Success = true, Token = "token", Errors = new string[0] };
            _authServiceMock.Setup(x => x.LoginAsync(request)).ReturnsAsync(result);

            // Act
            var response = await _controller.Login(request);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            var okResult = response.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenLoginFails()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@example.com", Password = "WrongPassword!" };
            var result = new AuthResult { Success = false, Token = null, Errors = new[] { "Invalid email or password" } };
            _authServiceMock.Setup(x => x.LoginAsync(request)).ReturnsAsync(result);

            // Act
            var response = await _controller.Login(request);

            // Assert
            response.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = response.Result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(result);
        }
    }
} 