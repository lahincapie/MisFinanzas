using FluentValidation;
using MisFinanzas.Application.Incomes.Dtos;

namespace MisFinanzas.Application.Incomes.Validators
{
    /// <summary>Reglas de validación (de forma) para crear un ingreso.</summary>
    public class CreateIncomeValidator : AbstractValidator<CreateIncomeDto>
    {
        public CreateIncomeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.ExpectedReceiptDay)
                .InclusiveBetween(1, 31).WithMessage("El día esperado de recepción debe estar entre 1 y 31.");

            // A diferencia del gasto: el ingreso SIEMPRE debe tener valor esperado > 0
            // (fijo o variable; el variable lo tiene, pero no proyecta — regla del SRS).
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