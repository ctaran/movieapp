using FluentValidation;
using MovieApp.Core.Models;

namespace MovieApp.Application.Validators
{
    public class CommentValidator : AbstractValidator<Comment>
    {
        public CommentValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(1000)
                .WithMessage("Comment must be between 3 and 1000 characters");

            RuleFor(x => x.MovieId)
                .GreaterThan(0)
                .WithMessage("MovieId must be greater than 0");
        }
    }
} 