using FluentAssertions;
using MovieApp.Core.Models;
using MovieApp.Application.Validators;
using Xunit;

namespace MovieApp.Tests.Validators
{
    public class CommentValidatorTests
    {
        private readonly CommentValidator _validator;

        public CommentValidatorTests()
        {
            _validator = new CommentValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("ab")] // Too short
        public void Validate_WithInvalidContent_ShouldFail(string content)
        {
            // Arrange
            var comment = new Comment
            {
                Content = content,
                MovieId = 1,
                UserId = "user123",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.Validate(comment);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Content");
        }

        [Fact]
        public void Validate_WithTooLongContent_ShouldFail()
        {
            // Arrange
            var longContent = new string('a', 1001);
            var comment = new Comment
            {
                Content = longContent,
                MovieId = 1,
                UserId = "user123",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.Validate(comment);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Content");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_WithInvalidMovieId_ShouldFail(int movieId)
        {
            // Arrange
            var comment = new Comment
            {
                Content = "Valid comment content",
                MovieId = movieId,
                UserId = "user123",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.Validate(comment);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "MovieId");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_WithInvalidUserId_ShouldFail(string userId)
        {
            // Arrange
            var comment = new Comment
            {
                Content = "Valid comment content",
                MovieId = 1,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.Validate(comment);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "UserId");
        }

        [Fact]
        public void Validate_WithInvalidCreatedAt_ShouldFail()
        {
            // Arrange
            var comment = new Comment
            {
                Content = "Valid comment content",
                MovieId = 1,
                UserId = "user123",
                CreatedAt = default // DateTime.MinValue
            };

            // Act
            var result = _validator.Validate(comment);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "CreatedAt");
        }

        [Fact]
        public void Validate_WithValidData_ShouldPass()
        {
            // Arrange
            var comment = new Comment
            {
                Content = "Valid comment content",
                MovieId = 1,
                UserId = "user123",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = _validator.Validate(comment);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
} 