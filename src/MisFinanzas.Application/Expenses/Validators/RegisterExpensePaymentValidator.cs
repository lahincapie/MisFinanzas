using FluentValidation;
using MisFinanzas.Application.Expenses.Dtos;

namespace MisFinanzas.Application.Expenses.Validators
{
    /// <summary>Reglas de validación (de forma) para registrar el pago de un gasto.</summary>
    public class RegisterExpensePaymentValidator : AbstractValidator<RegisterExpensePaymentDto>
    {
        public RegisterExpensePaymentValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("El monto debe ser mayor que cero.");

            RuleFor(x => x.PaymentMethodId)
                .GreaterThan(0).WithMessage("Debe seleccionar un medio de pago.");

            RuleFor(x => x.PaymentDate)
                .NotEmpty().WithMessage("La fecha de pago es obligatoria.")
                .LessThanOrEqualTo(_ => DateTime.UtcNow.Date.AddDays(1))
                .WithMessage("La fecha de pago no puede ser futura.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Las observaciones no pueden superar los 500 caracteres.");
        }
    }
}