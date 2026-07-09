using FluentValidation;
using MisFinanzas.Application.Categories.Dtos;

namespace MisFinanzas.Application.Categories.Validators
{
    /// <summary>Reglas de validación (de forma) para editar una categoría.</summary>
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("El Id no es válido.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");
            RuleFor(x => x.Description)
                .MaximumLength(300).WithMessage("La descripción no puede superar los 300 caracteres.");
            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0).WithMessage("El orden no puede ser negativo.");
        }
    }
}