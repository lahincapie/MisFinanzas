using FluentValidation;
using MisFinanzas.Application.Expenses.Dtos;

namespace MisFinanzas.Application.Expenses.Validators
{
    /// <summary>Reglas de validación (de forma) para crear un gasto.</summary>
    public class CreateExpenseValidator : AbstractValidator<CreateExpenseDto>
    {
        public CreateExpenseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Debe seleccionar una categoría.");

            // Los tres días, entre 1 y 31
            RuleFor(x => x.CutoffDay)
                .InclusiveBetween(1, 31).WithMessage("El día de corte debe estar entre 1 y 31.");
            RuleFor(x => x.DueDay)
                .InclusiveBetween(1, 31).WithMessage("El día de pago oportuno debe estar entre 1 y 31.");
            RuleFor(x => x.SuspensionDay)
                .InclusiveBetween(1, 31).WithMessage("El día de suspensión debe estar entre 1 y 31.");

            // El orden del ciclo: Corte <= PagoOportuno <= Suspensión
            RuleFor(x => x.DueDay)
                .GreaterThanOrEqualTo(x => x.CutoffDay)
                .WithMessage("El día de pago oportuno debe ser mayor o igual al día de corte.");
            RuleFor(x => x.SuspensionDay)
                .GreaterThanOrEqualTo(x => x.DueDay)
                .WithMessage("El día de suspensión debe ser mayor o igual al día de pago oportuno.");

            // Fijo/variable: si es fijo, valor esperado > 0
            RuleFor(x => x.ExpectedAmount)
                .Must(amount => amount.HasValue && amount.Value > 0)
                .When(x => !x.IsVariable)
                .WithMessage("Un gasto fijo debe tener un valor esperado mayor que cero.");

            // Si es variable, no debe traer valor esperado
            RuleFor(x => x.ExpectedAmount)
                .Null()
                .When(x => x.IsVariable)
                .WithMessage("Un gasto variable no debe tener valor esperado.");

            // Vigencia: si vienen, deben tener formato YYYY-MM
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