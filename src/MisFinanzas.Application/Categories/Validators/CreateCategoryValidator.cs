using FluentValidation;
using MisFinanzas.Application.Categories.Dtos;

namespace MisFinanzas.Application.Categories.Validators
{
    /// <summary>
    /// Reglas de validación para crear una categoría (validación de forma).
    /// La unicidad del nombre se valida en el servicio, porque requiere la base de datos.
    /// </summary>
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
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