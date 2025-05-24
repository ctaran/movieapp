using FluentAssertions;
using MovieApp.Application.DTOs;
using MovieApp.Application.Validators;
using Xunit;

namespace MovieApp.Tests.Validators
{
    public class RegisterRequestValidatorTests
    {
        private readonly RegisterRequestValidator _validator;

        public RegisterRequestValidatorTests()
        {
            _validator = new RegisterRequestValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("invalid-email")]
        [InlineData("test@")]
        [InlineData("@test.com")]
        public void Validate_WithInvalidEmail_ShouldFail(string email)
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = email,
                Password = "ValidPass123!",
                ConfirmPassword = "ValidPass123!"
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Email");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("short")]
        [InlineData("no-upper-123!")]
        [InlineData("NO-LOWER-123!")]
        [InlineData("NoSpecialChar123")]
        [InlineData("NoNumbers!!")]
        public void Validate_WithInvalidPassword_ShouldFail(string password)
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = password,
                ConfirmPassword = password
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Password");
        }

        [Fact]
        public void Validate_WithNonMatchingPasswords_ShouldFail()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "ValidPass123!",
                ConfirmPassword = "DifferentPass123!"
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "ConfirmPassword");
        }

        [Fact]
        public void Validate_WithValidData_ShouldPass()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                Password = "ValidPass123!",
                ConfirmPassword = "ValidPass123!"
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }

    public class LoginRequestValidatorTests
    {
        private readonly LoginRequestValidator _validator;

        public LoginRequestValidatorTests()
        {
            _validator = new LoginRequestValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("invalid-email")]
        [InlineData("test@")]
        [InlineData("@test.com")]
        public void Validate_WithInvalidEmail_ShouldFail(string email)
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = email,
                Password = "ValidPass123!"
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Email");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_WithInvalidPassword_ShouldFail(string password)
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = password
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Password");
        }

        [Fact]
        public void Validate_WithValidData_ShouldPass()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "ValidPass123!"
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
} 