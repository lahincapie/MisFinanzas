namespace MisFinanzas.Application.Reports.Dtos
{
    /// <summary>Total gastado (real) en un gasto puntual dentro de un rango de meses.</summary>
    public class ExpenseSpendDto
    {
        public int ExpenseId { get; set; }
        public string ExpenseName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}