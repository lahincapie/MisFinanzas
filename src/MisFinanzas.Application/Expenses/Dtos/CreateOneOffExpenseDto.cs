namespace MisFinanzas.Application.Expenses.Dtos
{
    /// <summary>Datos para registrar un gasto único (real) del mes actual.</summary>
    public class CreateOneOffExpenseDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int PaymentMethodId { get; set; }
        public string? Notes { get; set; }
    }
}