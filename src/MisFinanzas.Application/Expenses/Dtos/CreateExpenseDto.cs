using MisFinanzas.Domain.Common;

namespace MisFinanzas.Application.Expenses.Dtos
{
    /// <summary>Datos para crear un gasto (plantilla recurrente).</summary>
    public class CreateExpenseDto
    {
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Periodicity Periodicity { get; set; }
        public bool IsVariable { get; set; }
        public decimal? ExpectedAmount { get; set; }
        public int CutoffDay { get; set; }
        public int DueDay { get; set; }
        public int SuspensionDay { get; set; }
        public string? StartMonth { get; set; }
        public string? EndMonth { get; set; }
        public string? Reference { get; set; }
        public string? Contract { get; set; }
    }
}