namespace MisFinanzas.Application.Incomes.Dtos
{
    /// <summary>Datos para registrar un ingreso único (real) del mes actual.</summary>
    public class CreateOneOffIncomeDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
    }
}