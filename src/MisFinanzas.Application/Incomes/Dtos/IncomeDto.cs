using MisFinanzas.Domain.Common;

namespace MisFinanzas.Application.Incomes.Dtos
{
    /// <summary>Datos de un ingreso que se devuelven al usuario.</summary>
    public class IncomeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Periodicity Periodicity { get; set; }
        public bool IsVariable { get; set; }
        public decimal? ExpectedAmount { get; set; }
        public int ExpectedReceiptDay { get; set; }
        public string? StartMonth { get; set; }
        public string? EndMonth { get; set; }
    }
}