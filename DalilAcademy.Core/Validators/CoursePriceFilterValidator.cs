using DalilAcademy.Core.DTOs.Courses;
using FluentValidation;

namespace DalilAcademy.Core.Validators;

public class CoursePriceFilterValidator : AbstractValidator<CoursePriceFilter>
{
    public CoursePriceFilterValidator()
    {
        RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue);
        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue);
        RuleFor(x => x)
            .Must(x => x.MinPrice <= x.MaxPrice)
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
            .WithName("MinPrice")
            .WithMessage("minPrice cannot be greater than maxPrice.");
    }
}
