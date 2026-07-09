using MisFinanzas.Domain.Common;

namespace MisFinanzas.Application.Incomes.Dtos
{
    /// <summary>Datos para crear un ingreso (plantilla recurrente).</summary>
    public class CreateIncomeDto
    {
        public string Name { get; set; } = string.Empty;
        public Periodicity Periodicity { get; set; }
        public bool IsVariable { get; set; }
        public decimal? ExpectedAmount { get; set; }
        public int ExpectedReceiptDay { get; set; }
        public string? StartMonth { get; set; }
        public string? EndMonth { get; set; }
    }
}