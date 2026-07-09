using FluentValidation;
using MisFinanzas.Application.Incomes.Dtos;

namespace MisFinanzas.Application.Incomes.Validators
{
    /// <summary>Reglas de validación (de forma) para editar un ingreso.</summary>
    public class UpdateIncomeValidator : AbstractValidator<UpdateIncomeDto>
    {
        public UpdateIncomeValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("El Id no es válido.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.ExpectedReceiptDay)
                .InclusiveBetween(1, 31).WithMessage("El día esperado de recepción debe estar entre 1 y 31.");

            RuleFor(x => x.ExpectedAmount)
                .Must(amount => amount.HasValue && amount.Value > 0)
                .WithMessage("El valor esperado debe ser mayor que cero.");

            RuleFor(x => x.StartMonth)
                .Matches(@"^\d{4}-(0[1-9]|1[0-2])$")
                .When(x => !string.IsNullOrWhiteSpace(x.StartMonth))
                .WithMessage("El mes de inicio debe tener formato YYYY-MM.");
            RuleFor(x => x.EndMonth)
                .Matches(@"^\d{4}-(0[1-9]|1[0-2])$")
                .When(x => !string.IsNullOrWhiteSpace(x.EndMonth))
                .WithMessage("El mes de fin debe tener formato YYYY-MM.");
        }
    }
}