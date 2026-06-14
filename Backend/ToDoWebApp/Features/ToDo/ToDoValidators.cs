using FluentValidation;

namespace ToDoWebApp.Features.ToDo
{
    public class CreateToDoDtoValidator : AbstractValidator<CreateToDoRequestDto>
    {
        public CreateToDoDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
        }
    }

    public class UpdateToDoDtoValidator : AbstractValidator<UpdateToDoRequestDto>
    {
        public UpdateToDoDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.Id)
               .GreaterThan(0)
               .WithMessage("ToDo Id must be greater than 0.");
        }
    }
}