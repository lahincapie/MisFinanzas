using FluentValidation;
using MisFinanzas.Application.Incomes.Dtos;

namespace MisFinanzas.Application.Incomes.Validators
{
    /// <summary>Reglas de validación (de forma) para registrar la recepción de un ingreso.</summary>
    public class RegisterIncomeReceiptValidator : AbstractValidator<RegisterIncomeReceiptDto>
    {
        public RegisterIncomeReceiptValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("El monto debe ser mayor que cero.");

            RuleFor(x => x.ReceiptDate)
                .NotEmpty().WithMessage("La fecha de recepción es obligatoria.")
                .LessThanOrEqualTo(_ => DateTime.UtcNow.Date.AddDays(1))
                .WithMessage("La fecha de recepción no puede ser futura.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Las observaciones no pueden superar los 500 caracteres.");
        }
    }
}