using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using MovieApp.Application.DTOs;
using MovieApp.Application.Services;
using Xunit;

namespace MovieApp.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Setup UserManager mock
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object,
                null, null, null, null, null, null, null, null);

            // Setup Configuration mock
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x["Jwt:Key"]).Returns("your-256-bit-secret-key-here-123456");
            _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("test-issuer");
            _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("test-audience");
            _configurationMock.Setup(x => x["Jwt:ExpiryInHours"]).Returns("1");

            _authService = new AuthService(_userManagerMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ShouldSucceed()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "ValidPass123!",
                ConfirmPassword = "ValidPass123!"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Token.Should().NotBeNullOrEmpty();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task RegisterAsync_WhenUserCreationFails_ShouldReturnErrors()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "ValidPass123!",
                ConfirmPassword = "ValidPass123!"
            };

            var errors = new[] { new IdentityError { Description = "User already exists" } };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Token.Should().BeNull();
            result.Errors.Should().Contain("User already exists");
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldSucceed()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "ValidPass123!"
            };

            var user = new IdentityUser { Email = request.Email };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Token.Should().NotBeNullOrEmpty();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task LoginAsync_WithInvalidEmail_ShouldFail()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "ValidPass123!"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Token.Should().BeNull();
            result.Errors.Should().Contain("Invalid email or password");
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ShouldFail()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword123!"
            };

            var user = new IdentityUser { Email = request.Email };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(false);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Token.Should().BeNull();
            result.Errors.Should().Contain("Invalid email or password");
        }
    }
} 