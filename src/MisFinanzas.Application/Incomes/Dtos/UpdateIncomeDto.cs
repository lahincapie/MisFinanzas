using MisFinanzas.Domain.Common;

namespace MisFinanzas.Application.Incomes.Dtos
{
    /// <summary>Datos para editar un ingreso existente.</summary>
    public class UpdateIncomeDto
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