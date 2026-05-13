using FluentValidation;
using Restaurant.Application.DTOS.Admin;

namespace Restaurant.Application.Validators
{
    public class AdminCreateCategoryDtoValidator : AbstractValidator<AdminCreateCategoryDto>
    {
        public AdminCreateCategoryDtoValidator()
        {
            RuleFor(x => x.NameAr)
                .NotEmpty().WithMessage("Arabic category name is required")
                .MaximumLength(100).WithMessage("Arabic name must not exceed 100 characters");

            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage("English category name is required")
                .MaximumLength(100).WithMessage("English name must not exceed 100 characters");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be a positive number");
        }
    }
}
