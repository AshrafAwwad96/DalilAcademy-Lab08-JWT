using DalilAcademy.Core.DTOs.Courses;
using FluentValidation;

namespace DalilAcademy.Core.Validators;

// Shape of the input only — rules that need the database (duplicate title) live in the service/repository.
public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be zero or more.");

        RuleFor(x => x.InstructorId)
            .GreaterThan(0);

        RuleFor(x => x.Level).IsInEnum();
    }
}

public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
{
    public UpdateCourseDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Price must be zero or more.");
        RuleFor(x => x.Level).IsInEnum();
    }
}
